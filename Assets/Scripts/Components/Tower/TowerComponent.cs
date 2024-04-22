using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[System.Serializable]
public struct TowerComponent : IComponentData
{
    public bool Placed;
    public TowerType Type;   
    public float2 AttackRange;
    public float AttackSpeed;
    public int AttackPower;
}

public enum TowerType
{
    Mine,
    Mortar,
    Turret
}