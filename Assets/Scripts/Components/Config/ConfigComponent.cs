using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct ConfigComponent : IComponentData
{
    public Entity MinePrefab;
    
    public Entity TowerTurret;
    public Entity TowerMine;
    public Entity TowerMortar;
    
    public Entity EnemyTiny;
    public Entity EnemyNormal;
    public Entity EnemyHeavy;

    public Entity GetRandom()
    {
        var index = Random.Range(0, 3);

        Entity result = EnemyTiny;
        
        switch (index)
        {
            case 0:
                result = EnemyTiny;
                break;
            case 1:
                result = EnemyNormal;
                break;
            case 2:
                result = EnemyHeavy;
                break;
        }

        return result;
    }

    public Entity GetTowerByType(TowerType type)
    {
        switch (type)
        {
            case TowerType.Mine:
                return TowerMine;
            case TowerType.Turret:
                return TowerTurret;
            case TowerType.Mortar:
                return TowerMortar;
            default:
                return TowerMine;
        }
    }
    
    public Entity GetTowerByType(int type)
    {
        switch (type)
        {
            case 0:
                return TowerMine;
            case 1:
                return TowerTurret;
            case 2:
                return TowerMortar;
            default:
                return TowerMine;
        }
    }
}