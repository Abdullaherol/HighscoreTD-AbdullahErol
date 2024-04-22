using System;
using Managers.Boost;
using Managers.Tower;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(EnemySystem))]
public partial struct TowerHeadSystem : ISystem
{
    private NativeList<EnemyComponent> enemies;
    private NativeList<float3> enemyPositions;
    private NativeList<Entity> enemyEntities;
    
    public void OnCreate(ref SystemState state)
    {
        enemies = new NativeList<EnemyComponent>(Allocator.Persistent);
        enemyEntities = new NativeList<Entity>(Allocator.Persistent);
        enemyPositions = new NativeList<float3>(Allocator.Persistent);
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!GameManager.Instance.IsPlaying) return;

        RotateHead(state);
        ShotTowerTimer(state);
    }

    private void RotateHead(SystemState state)
    {
        foreach (var (head, headTransform, parent, headEntity) in SystemAPI
                     .Query<RefRW<TowerHeadComponent>, RefRW<LocalTransform>, RefRO<Parent>>().WithEntityAccess())
        {
            var towerEntity = parent.ValueRO.Value;

            var towerComp = state.EntityManager.GetComponentData<TowerComponent>(towerEntity);

            var towerTransform = state.EntityManager.GetComponentData<LocalTransform>(towerEntity);

            NativeArray<int> closestEnemyIndex = new NativeArray<int>(1, Allocator.TempJob);

            Entity enemyEntity = GetClosestEnemy(state, towerTransform.Position,
                TowerManager.Instance.GetTowerDataByType((int)towerComp.Type).AttackRange,head.ValueRO.type);
            if (enemyEntity != Entity.Null)
            {
                float3 enemyPosition =
                    state.EntityManager.GetComponentData<LocalTransform>(enemyEntity).Position;
                float3 towerPosition = towerTransform.Position;

                float3 direction = enemyPosition - towerPosition;
                direction.y = 0;

                headTransform.ValueRW.Rotation = quaternion.LookRotation(direction, new float3(0, 1, 0));

                head.ValueRW.Target = enemyEntity;

                Debug.DrawLine(towerTransform.TransformPoint(headTransform.ValueRO.Position), enemyPosition, Color.red,
                    1);
            }
            else
            {
                head.ValueRW.Target = Entity.Null;
            }

            closestEnemyIndex.Dispose();
        }
    }

    private Entity GetClosestEnemy(SystemState state, float3 towerPos, float2 towerRange,TowerType towerType)
    {
        switch (towerType)
        {
            case TowerType.Mine:
                return Entity.Null;
            case TowerType.Mortar:
                return GetMortarFirstEnemy(state,towerPos,towerRange);
            case TowerType.Turret:
                return GetTurretClosestEnemy(state, towerPos, towerRange);
            default:
                return Entity.Null;
        }
    }

    private Entity GetMortarFirstEnemy(SystemState state, float3 towerPos, float2 towerRange)
    {
        enemies = new NativeList<EnemyComponent>(Allocator.Persistent);
        enemyEntities = new NativeList<Entity>(Allocator.Persistent);
        enemyPositions = new NativeList<float3>(Allocator.Persistent);

        foreach (var (enemy, enemyTransform, enemyEntity) in SystemAPI
                     .Query<RefRO<EnemyComponent>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            if(enemy.ValueRO.OnPool) continue;
            
            enemies.Add(enemy.ValueRO);
            enemyEntities.Add(enemyEntity);
            enemyPositions.Add(enemyTransform.ValueRO.Position);
        }

        var list = new NativeList<int>(Allocator.Temp);
        
        for (int i = 0; i < enemyPositions.Length; i++)
        {
            var enemyPos = enemyPositions[i];

            var distance = math.distance(towerPos, enemyPos);

            if (distance >= towerRange.x && distance <= towerRange.y)
            {
                list.Add(i);
            }
        }
        
        float minDistance = float.MaxValue;
        int index = -1;

        var baseTowerPosition = TowerManager.Instance.GetBaseTowerPosition();
        
        for (int i = 0; i < list.Length; i++)
        {
            var enemyIndex = list[i];
            
            var enemyPos = enemyPositions[enemyIndex];

            var distance = math.distance(baseTowerPosition, enemyPos);

            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            }
        }
        
        if (index == -1)
        {
            return Entity.Null;
        }
        
        return enemyEntities[index];
    } 

    private Entity GetTurretClosestEnemy(SystemState state, float3 towerPos, float2 towerRange)
    {
        enemies = new NativeList<EnemyComponent>(Allocator.Persistent);
        enemyEntities = new NativeList<Entity>(Allocator.Persistent);
        enemyPositions = new NativeList<float3>(Allocator.Persistent);

        foreach (var (enemy, enemyTransform, enemyEntity) in SystemAPI
                     .Query<RefRO<EnemyComponent>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            enemies.Add(enemy.ValueRO);
            enemyEntities.Add(enemyEntity);
            enemyPositions.Add(enemyTransform.ValueRO.Position);
        }

        float minDistance = float.MaxValue;
        int index = -1;

        for (int i = 0; i < enemyPositions.Length; i++)
        {
            var enemyPos = enemyPositions[i];

            var distance = math.distance(towerPos, enemyPos);

            if (distance < minDistance && distance <= towerRange.y && distance >= towerRange.x)
            {
                minDistance = distance;
                index = i;
            }
        }

        if (index == -1)
        {
            return Entity.Null;
        }
        
        return enemyEntities[index];
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (enemies.IsCreated)
        {
            enemies.Dispose();
        }

        if (enemyEntities.IsCreated)
        {
            enemyEntities.Dispose();
        }
    }

    private void ShotTowerTimer(SystemState state)
    {
        float timeDelta = Time.deltaTime;

        foreach (var (head, parent, headEntity) in
                 SystemAPI.Query<RefRW<TowerHeadComponent>, RefRO<Parent>>().WithEntityAccess())
        {
            var parentEntity = parent.ValueRO.Value;

            if (!state.EntityManager.GetComponentData<TowerComponent>(parentEntity).Placed) continue;

            head.ValueRW.CurrentTime += timeDelta * BoostManager.Instance.GetBoostMultiplier();
        }
    }
}