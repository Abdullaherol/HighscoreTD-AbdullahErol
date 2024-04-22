using Unity.Entities;

public class EnemyGameObjectPrefabBaker : Baker<EnemyAnimatorAuthoring>
{
    public override void Bake(EnemyAnimatorAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponentObject(entity, new EnemyGameObjectPrefab() { Value = authoring.EnemyGameObjectPrefab });
    }
}