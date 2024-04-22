using Managers.Tower;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct MortarBulletSystem : ISystem
{
    private NativeList<Entity> _pool;

    public void OnCreate(ref SystemState state)
    {
        _pool = new NativeList<Entity>(Allocator.Persistent);
    }

    public void OnUpdate(ref SystemState state)
    {
        SpawnBullet(state);
        BulletTimer(state);
        MoveBullet(state);
    }

    private void SpawnBullet(SystemState state)
    {
        foreach (var (head, headTransform, parent, headEntity) in SystemAPI
                     .Query<RefRW<TowerHeadComponent>, RefRO<LocalTransform>, RefRO<Parent>>().WithEntityAccess())
        {
            if (head.ValueRO.type != TowerType.Mortar) continue;

            if (head.ValueRO.Target == Entity.Null) continue;

            var parentEntity = parent.ValueRO.Value;

            if (!state.EntityManager.GetComponentData<TowerComponent>(parentEntity).Placed) continue;

            var towerComp = state.EntityManager.GetComponentData<TowerComponent>(parentEntity);

            if (!head.ValueRO.CanShot()) return;

            var bulletPrefab = head.ValueRO.BulletPrefab;

            var pos = state.EntityManager.GetComponentData<LocalTransform>(parentEntity)
                .TransformPoint(headTransform.ValueRO.Position + head.ValueRO.BulletSpawnOffset);

            var bullet = GetBullet(state, bulletPrefab, pos);

            var mortarBulletComp = state.EntityManager.GetComponentData<MortarBullet>(bullet);
            mortarBulletComp.Target = head.ValueRO.Target;
            mortarBulletComp.Head = headEntity;
            mortarBulletComp.CurrentTime = 0;
            mortarBulletComp.Area = head.ValueRO.AttackArea;

            mortarBulletComp.StartPos = pos;
            mortarBulletComp.Damage = towerComp.AttackPower;

            state.EntityManager.SetComponentData(bullet, mortarBulletComp);

            head.ValueRW.Shot();
        }
    }

    private Entity GetBullet(SystemState state, Entity prefab, float3 pos)
    {
        if (_pool.Length > 0)
        {
            var last = _pool[^1];
            _pool.RemoveAt(_pool.Length - 1);

            var particle = state.EntityManager.GetComponentData<MortarBulletReference>(last).ParticleSystem;
            particle.Pause();

            particle.transform.position = pos;

            particle.Clear();
            particle.Play();
            particle.GetComponent<AudioSource>().Play();

            return last;
        }

        return state.EntityManager.Instantiate(prefab);
    }

    private void BulletTimer(SystemState state)
    {
        foreach (var (bullet, transform, bulletEntity) in SystemAPI
                     .Query<RefRW<MortarBullet>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            if (bullet.ValueRO.Target == Entity.Null) continue;

            bullet.ValueRW.CurrentTime += Time.deltaTime;

            float percentage = bullet.ValueRO.CurrentTime / bullet.ValueRO.Speed;

            if (percentage < 1) continue;

            AreaDamage(state, transform.ValueRO.Position, bullet.ValueRO.Area, bullet.ValueRO.Damage);

            bullet.ValueRW.Target = Entity.Null;

            _pool.Add(bulletEntity);
        }
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
        
        ParticleManager.Instance.PlaceMortarParticle(pos);
    }

    private void MoveBullet(SystemState state)
    {
        foreach (var (bullet, bulletTransform, bulletReference) in SystemAPI
                     .Query<RefRW<MortarBullet>, RefRW<LocalTransform>, MortarBulletReference>())
        {
            if (bullet.ValueRO.Target == Entity.Null)
            {
                bulletTransform.ValueRW.Position = TowerManager.Instance.GetBulletPoolPosition();
                bulletReference.ParticleSystem.Stop();
                bulletReference.ParticleSystem.transform.position = TowerManager.Instance.GetBulletPoolPosition();
                continue;
            }

            var startPos = bullet.ValueRO.StartPos;
            var destination = state.EntityManager.GetComponentData<LocalTransform>(bullet.ValueRO.Target).Position;

            float percentage = bullet.ValueRO.CurrentTime / bullet.ValueRO.Speed;

            var pos = math.lerp(startPos, destination, percentage);

            bulletTransform.ValueRW.Position = pos;
        }
    }
}