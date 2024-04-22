using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct MineBullet : IComponentData
{
    public bool OnPool;
    public float Range;
    public int Damage;
    public float MoveDuration;
    public float CurrentTime;
    public float3 Destination;
    public float3 StartPos;
    public float3 SpawnPos;
}