using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct TurretBullet : IComponentData
{
    public Entity Target;
    public Entity Head;
    public float Speed;
    public int Damage;
    public float3 StartPos;

    public float CurrentTime;
}