using Unity.Entities;

public class TowerHeadBaker : Baker<TowerHeadAuthoring>
{
    public override void Bake(TowerHeadAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

        var comp = authoring.TowerHeadComponent;
        comp.BulletPrefab = GetEntity(authoring.Tower.TowerData.BulletPrefab, TransformUsageFlags.Dynamic);
        comp.BulletSpawnOffset = authoring.Tower.TowerData.BulletSpawnOffset;
        comp.AttackSpeed = authoring.Tower.TowerData.AttackSpeed;
        comp.type = authoring.Tower.TowerData.Type;
        comp.AttackArea = authoring.Tower.TowerData.AttackArea;

        AddComponent(entity, comp);
    }
}