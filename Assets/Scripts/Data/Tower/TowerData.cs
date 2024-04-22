using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    public TowerType Type;
    public float2 AttackRange;
    public float AttackSpeed;
    public float AttackArea;
    public int AttackPower;
    public int Price;
    public int PriceIncreaseRate;
    
    [Space] public Sprite Image;

    [Space] public GameObject BulletPrefab;
    public Vector3 BulletSpawnOffset;
}