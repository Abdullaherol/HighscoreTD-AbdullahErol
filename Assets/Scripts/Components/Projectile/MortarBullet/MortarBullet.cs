using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct MortarBullet : IComponentData
{
    public Entity Target;
    public Entity Head;
    public float Speed;
    public float Area;
    public int Damage;
    public float3 StartPos;

    public float CurrentTime;
}