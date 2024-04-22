using Unity.Entities;

namespace Baker.Projectile.MortarBullet
{
    public class MortarBulletGameObjectBaker : Baker<MortarBulletGameObjectAuthoring>
    {
        public override void Bake(MortarBulletGameObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new MortarBulletGameObjectPrefab() { Value = authoring.GameObject });
        }
    }
}