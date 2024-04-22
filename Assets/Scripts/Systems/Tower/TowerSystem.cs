using Managers.Boost;
using Managers.Grid;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;


public partial struct TowerSystem : ISystem
{
    private bool _initialized;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if(!GameManager.Instance.IsPlaying) return;

        if (!_initialized)
        {
            SpawnSaveTowers(state);
            _initialized = true;
        }
        
        SpawnTower(state);
        MoveTowers(state);
    }

    private void SpawnSaveTowers(SystemState state)
    {
        if (SaveManager.Instance == null ||
            SaveManager.Instance.SaveData == null) return;
        
        var saveData = SaveManager.Instance.SaveData;

        for (var i = 0; i < saveData.Towers.Count; i++)
        {
            var type = saveData.Towers[i];
            if (type == -1) continue;

            var pos = GridManager.Instance.GetGridNodePositionByIndex(i);

            var towerType = (TowerType)type;
            
            SpawnTowerAndSetPos(state,towerType,pos);
        }
    }

    private void SpawnTower(SystemState state)
    {
        if (!GridManager.Instance.OnTowerPlace ||
            GridManager.Instance.TowerPlaced) return;

        SpawnTowerAndSetPos(state, GridManager.Instance.SelectedTower.Type, GridManager.Instance.PlaceTowerPosition);

        GridManager.Instance.TowerPlaced = true;
    }

    private void SpawnTowerAndSetPos(SystemState state,TowerType type, float3 pos)
    {
        var config = SystemAPI.GetSingleton<ConfigComponent>();

        var prefab = config.GetTowerByType(type);

        Entity tower = state.EntityManager.Instantiate(prefab);

        var transform = state.EntityManager.GetComponentData<LocalTransform>(tower);
        transform.Position = pos;

        state.EntityManager.SetComponentData(tower, transform);
    }

    private void MoveTowers(SystemState state)
    {
        foreach (var (tower, towerTransform, towerEntity) in
                 SystemAPI.Query<RefRW<TowerComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (!tower.ValueRO.Placed)
            {
                if (GridManager.Instance.OnTowerPlace)
                {
                    towerTransform.ValueRW.Position = GridManager.Instance.PlaceTowerPosition;
                }
                else
                {
                    tower.ValueRW.Placed = true;
                }
            }
        }
    }
}