using Unity.Entities;

public class MineBulletBaker : Baker<MineBulletAuthoring>
{
    public override void Bake(MineBulletAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        AddComponent(entity,authoring.MineBullet);
    }
}