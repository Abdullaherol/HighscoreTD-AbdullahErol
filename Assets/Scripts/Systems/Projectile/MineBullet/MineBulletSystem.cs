using Managers.Grid;
using Managers.Tower;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public partial struct MineBulletSystem : ISystem
{
    private NativeList<Entity> _minePool;

    public void OnCreate(ref SystemState state)
    {
        _minePool = new NativeList<Entity>(Allocator.Persistent);
        state.RequireForUpdate<ConfigComponent>();
    }

    public void OnDestroy(ref SystemState state)
    {
        if (_minePool.IsCreated)
        {
            _minePool.Dispose();
        }
    }

    public void OnUpdate(ref SystemState state)
    {
        if(!GameManager.Instance.IsPlaying) return;
        
        AddMineTower(state);
        MineTimer(state);
        PlaceMine(state);
        MoveMine(state);
        ExecuteMine(state);
    }

    private void AddMineTower(SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (tower, entity) in SystemAPI.Query<TowerComponent>().WithNone<MineTowerComponent>()
                     .WithEntityAccess())
        {
            if(tower.Type != TowerType.Mine) continue;

            var comp = new MineTowerComponent()
            {
                AttackDuration = tower.AttackSpeed,
                Damage = tower.AttackPower,
                Range = tower.AttackRange
            };

            ecb.AddComponent(entity,comp);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void MineTimer(SystemState state)
    {
        foreach (var mineTower in SystemAPI.Query<RefRW<MineTowerComponent>>())
        {
            mineTower.ValueRW.CurrentTime += Time.deltaTime;
        }

        foreach (var mine in SystemAPI.Query<RefRW<MineBullet>>())
        {
            if(mine.ValueRO.OnPool) continue;
            
            mine.ValueRW.CurrentTime += Time.deltaTime;
        }
    }

    private void MoveMine(SystemState state)
    {
        foreach (var (mine,transform) in SystemAPI.Query<RefRW<MineBullet>,RefRW<LocalTransform>>())
        {
            if(mine.ValueRO.OnPool) continue;
            
            var percentage = mine.ValueRO.CurrentTime / mine.ValueRO.MoveDuration;

            var pos = math.lerp(mine.ValueRO.StartPos, mine.ValueRO.Destination, math.clamp(percentage,0,1));

            transform.ValueRW.Position = pos;
        }
    }

    private void PlaceMine(SystemState state)
    {
        foreach (var (mineTower,towerTransform) in SystemAPI.Query<RefRW<MineTowerComponent>,RefRO<LocalTransform>>())
        {
            var percentage = mineTower.ValueRO.CurrentTime / mineTower.ValueRO.AttackDuration;

            if (percentage < 1) continue;

            mineTower.ValueRW.CurrentTime = 0;

            var mine = GetMine(state);

            var mineTransform = state.EntityManager.GetComponentData<LocalTransform>(mine);
            var mineComp = state.EntityManager.GetComponentData<MineBullet>(mine);
                
            var pos = towerTransform.ValueRO.Position;

            var minePos = new float3(0, mineComp.SpawnPos.y,
                pos.z + Random.Range(-mineTower.ValueRO.Range.y, mineTower.ValueRO.Range.y));

            mineComp.Destination = minePos;
            mineComp.Damage = (int)mineTower.ValueRO.Damage;
            mineComp.Range = mineTower.ValueRO.Range.x;
            mineComp.StartPos = towerTransform.ValueRO.Position;
            
            state.EntityManager.SetComponentData(mine,mineComp);
        }
    }

    private void ExecuteMine(SystemState state)
    {
        foreach (var (mine,transform,entity) in SystemAPI.Query<RefRW<MineBullet>,RefRW<LocalTransform>>().WithEntityAccess())
        {
            if(mine.ValueRO.OnPool) continue;
            
            var percentage = mine.ValueRO.CurrentTime / mine.ValueRO.MoveDuration;

            if (percentage < 1) continue;
            
            if(!HasCloseEnemy(state,transform.ValueRO.Position, mine.ValueRO.Range)) continue;

            mine.ValueRW.CurrentTime = 0;

            AreaDamage(state, transform.ValueRO.Position, mine.ValueRO.Range, mine.ValueRO.Damage);

            transform.ValueRW.Position = TowerManager.Instance.GetBulletPoolPosition();

            mine.ValueRW.OnPool = true;
            
            _minePool.Add(entity);
        }
    }

    private bool HasCloseEnemy(SystemState state, float3 pos, float range)
    {
        foreach (var (enemy, transform) in SystemAPI.Query<RefRW<EnemyComponent>, RefRO<LocalTransform>>())
        {
            var enemyPos = transform.ValueRO.Position;
            enemyPos.y = pos.y;

            var distance = math.distance(enemyPos, pos);
            
            if(distance > range) continue;

            return true;
        }

        return false;
    }
    
    private void AreaDamage(SystemState state, float3 pos, float range, int damage)
    {
        foreach (var (enemy, transform) in SystemAPI.Query<RefRW<EnemyComponent>, RefRO<LocalTransform>>())
        {
            var enemyPos = transform.ValueRO.Position;

            var distance = math.distance(enemyPos, pos);
            
            if(distance > range) continue;

            enemy.ValueRW.TakeDamage(damage);
        }
        
        ParticleManager.Instance.PlaceMineParticle(pos);
    }

    private Entity GetMine(SystemState state)
    {
        var config = SystemAPI.GetSingleton<ConfigComponent>();

        if (_minePool.Length > 0)
        {
            var last = _minePool[^1];
            _minePool.RemoveAt(_minePool.Length - 1);

            var comp = state.EntityManager.GetComponentData<MineBullet>(last);
            comp.OnPool = false;
            
            state.EntityManager.SetComponentData(last,comp);

            return last;
        }

        return state.EntityManager.Instantiate(config.MinePrefab);
    }
}