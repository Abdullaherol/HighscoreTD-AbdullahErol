using Unity.Entities;

public class MortarBulletBaker : Baker<MortarBulletAuthoring>
{
    public override void Bake(MortarBulletAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        AddComponent(entity,authoring.MortarBullet);
    }
}