using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct EnemyComponent : IComponentData
{
    public bool OnPool;
    public float3 SpawnPosition;
    public float3 Position;
    public float Speed;
    public int Health;
    public int CurrentHealth;
    public int Type;

    public int TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        return CurrentHealth;
    }
}