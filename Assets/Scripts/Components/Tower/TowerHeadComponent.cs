using Unity.Entities;
using Unity.Mathematics;

public struct TowerHeadComponent : IComponentData
{
    public Entity BulletPrefab;
    public Entity Target;
    public float3 BulletSpawnOffset;
    public float AttackSpeed;
    public float AttackArea;
    public TowerType type;
    
    public float CurrentTime;
    
    public bool CanShot()
    {
        return AttackSpeed <= CurrentTime;
    }

    public void Shot()
    {
        CurrentTime = 0;
    }
}