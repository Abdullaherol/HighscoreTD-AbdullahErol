using Components.Projectile;
using Unity.Entities;

namespace Baker.Projectile
{
    public class TurretBulletGameObjectBaker : Baker<TurretBulletGameObjectAuthoring>
    {
        public override void Bake(TurretBulletGameObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new TurretBulletGameObjectPrefab() { Value = authoring.GameObject });
        }
    }
}