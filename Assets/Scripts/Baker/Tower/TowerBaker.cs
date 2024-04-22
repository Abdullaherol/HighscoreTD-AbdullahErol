using Unity.Entities;

public class TowerBaker : Baker<TowerAuthoring>
{
    public override void Bake(TowerAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        var towerData = authoring.TowerData;
        
        AddComponent(entity,new TowerComponent()
        {
            Type = towerData.Type,
            AttackPower = towerData.AttackPower,
            AttackRange = towerData.AttackRange,
            AttackSpeed = towerData.AttackSpeed
        });
    }
}