using Unity.Entities;
using Unity.Mathematics;

public struct MineTowerComponent : IComponentData
{
    public float AttackDuration;
    public float CurrentTime;
    public float2 Range;
    public float Damage;
}