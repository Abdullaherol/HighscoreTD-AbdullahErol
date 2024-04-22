using Authoring.Projectile;
using Unity.Entities;

namespace Baker.Projectile
{
    public class TurretBulletBaker : Baker<TurretBulletAuthoring>
    {
        public override void Bake(TurretBulletAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity,authoring.TurretBullet);
        }
    }
}