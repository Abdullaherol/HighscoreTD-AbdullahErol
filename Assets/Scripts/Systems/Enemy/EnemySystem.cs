using System;
using Managers.Save;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public partial struct EnemySystem : ISystem
{
    private float _currentSpawnDuration;
    private float _currentTimer;

    private bool _initialized;

    private NativeArray<Entity> _enemyPool;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        _enemyPool = new NativeArray<Entity>(1000, Allocator.Persistent);

        GetSaveData();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!_initialized)
        {
            if (EnemyManager.Instance != null)
            {
                _currentSpawnDuration = EnemyManager.Instance.GetStartSpawnInterval();
                _currentTimer = _currentSpawnDuration;
                _initialized = true;
            }
        }

        if (!_initialized ||
            !GameManager.Instance.IsPlaying ||
            !GameManager.Instance.FirstTowerPlaced ||
            EnemyManager.Instance == null) return;

        SpawnTimer(state);
        MoveEnemies(state);
    }


    private void GetSaveData()
    {
        if (SaveManager.Instance == null ||
            SaveManager.Instance.SaveData == null) return;

        var saveData = SaveManager.Instance.SaveData;

        _currentSpawnDuration = saveData.EnemySpawnInterval;
    }

    private void SpawnTimer(SystemState state)
    {
        var decreaseRate = EnemyManager.Instance.GetDecreaseRate();

        float deltaTime = UnityEngine.Time.deltaTime;

        if (_currentTimer < _currentSpawnDuration)
        {
            _currentTimer += deltaTime;
            return;
        }

        _currentTimer = 0;
        _currentSpawnDuration -= decreaseRate;

        EnemyManager.Instance.SetCurrentEnemySpawnInterval(_currentSpawnDuration);

        SpawnRandomEnemy(state);
    }

    private void SpawnRandomEnemy(SystemState state)
    {
        var config = SystemAPI.GetSingleton<ConfigComponent>();

        Entity enemyPrefab = config.GetRandom();
        Entity? reusedEnemy = null;

        int prefabType = state.EntityManager.GetComponentData<EnemyComponent>(enemyPrefab).Type;

        for (int i = 0; i < _enemyPool.Length; i++)
        {
            if (_enemyPool[i] != Entity.Null)
            {
                if (state.EntityManager.GetComponentData<EnemyComponent>(_enemyPool[i]).Type == prefabType)
                {
                    reusedEnemy = _enemyPool[i];
                    _enemyPool[i] = Entity.Null;
                    break;
                }
            }
        }

        Entity newEnemy;
        if (reusedEnemy.HasValue)
            newEnemy = reusedEnemy.Value;
        else
            newEnemy = state.EntityManager.Instantiate(enemyPrefab);

        var enemyComp = state.EntityManager.GetComponentData<EnemyComponent>(newEnemy);
        enemyComp.OnPool = false;

        var enemyTransform = state.EntityManager.GetComponentData<LocalTransform>(newEnemy);
        enemyTransform.Position = enemyComp.SpawnPosition;

        state.EntityManager.SetComponentData(newEnemy, enemyComp);
        state.EntityManager.SetComponentData(newEnemy, enemyTransform);
    }

    private void MoveEnemies(SystemState state)
    {
        float deltaTime = Time.deltaTime;

        foreach (var (enemy, enemyTransform, enemyEntity) in SystemAPI
                     .Query<RefRW<EnemyComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (enemy.ValueRO.CurrentHealth <= 0)
            {
                enemyTransform.ValueRW.Position = EnemyManager.Instance.GetEnemyPoolPosition();
                enemy.ValueRW.Position = enemyTransform.ValueRO.Position;

                enemy.ValueRW.CurrentHealth = (int)(enemy.ValueRO.Health * EnemyManager.Instance.GetDifficulty());

                enemy.ValueRW.OnPool = true;

                var index = _enemyPool.IndexOf(Entity.Null);
                _enemyPool[index] = enemyEntity;

                DataManager.Instance.AddMoneyAndScore();
                continue;
            }

            if (enemy.ValueRO.OnPool) continue;

            var enemyPosition = enemyTransform.ValueRO.Position;

            if (enemyPosition.z > EnemyManager.Instance.GetEnemyDestination().z)
            {
                enemyTransform.ValueRW.Position.z -= enemy.ValueRO.Speed * deltaTime;

                enemy.ValueRW.Position = enemyTransform.ValueRO.Position;
            }
            else
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void OnDestroy(ref SystemState state)
    {
        if (_enemyPool.IsCreated)
        {
            _enemyPool.Dispose();
        }
    }
}