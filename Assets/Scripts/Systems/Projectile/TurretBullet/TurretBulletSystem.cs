using System.Linq;
using Components.Projectile;
using Managers.Tower;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct TurretBulletSystem : ISystem
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
            if(head.ValueRO.type != TowerType.Turret) continue;
            
            if (head.ValueRO.Target == Entity.Null) continue;

            var parentEntity = parent.ValueRO.Value;

            if (!state.EntityManager.GetComponentData<TowerComponent>(parentEntity).Placed) continue;

            var towerComp = state.EntityManager.GetComponentData<TowerComponent>(parentEntity);

            if (!head.ValueRO.CanShot()) return;

            var bulletPrefab = head.ValueRO.BulletPrefab;
            
            var pos = state.EntityManager.GetComponentData<LocalTransform>(parentEntity)
                .TransformPoint(headTransform.ValueRO.Position + head.ValueRO.BulletSpawnOffset);

            var bullet = GetBullet(state, bulletPrefab,pos);

            var turretBulletComp = state.EntityManager.GetComponentData<TurretBullet>(bullet);
            turretBulletComp.Target = head.ValueRO.Target;
            turretBulletComp.Head = headEntity;
            turretBulletComp.CurrentTime = 0;

            turretBulletComp.StartPos = pos;
            turretBulletComp.Damage = towerComp.AttackPower;

            state.EntityManager.SetComponentData(bullet, turretBulletComp);

            head.ValueRW.Shot();
        }
    }

    private Entity GetBullet(SystemState state, Entity prefab, float3 pos)
    {
        if (_pool.Length > 0)
        {
            var last = _pool[^1];
            _pool.RemoveAt(_pool.Length - 1);

            var particle = state.EntityManager.GetComponentData<TurretBulletReference>(last).ParticleSystem;
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
        foreach (var (bullet, bulletEntity) in SystemAPI
                     .Query<RefRW<TurretBullet>>().WithEntityAccess())
        {
            if (bullet.ValueRO.Target == Entity.Null) continue;

            bullet.ValueRW.CurrentTime += Time.deltaTime;

            float percentage = bullet.ValueRO.CurrentTime / bullet.ValueRO.Speed;

            if (percentage < 1) continue;

            var enemyComp = state.EntityManager.GetComponentData<EnemyComponent>(bullet.ValueRO.Target);
            enemyComp.TakeDamage(bullet.ValueRO.Damage);

            state.EntityManager.SetComponentData(bullet.ValueRO.Target, enemyComp);

            bullet.ValueRW.Target = Entity.Null;

            _pool.Add(bulletEntity);
        }
    }

    private void MoveBullet(SystemState state)
    {
        foreach (var (bullet, bulletTransform, bulletReference) in SystemAPI
                     .Query<RefRW<TurretBullet>, RefRW<LocalTransform>, TurretBulletReference>())
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