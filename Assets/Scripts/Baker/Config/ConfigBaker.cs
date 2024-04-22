using Authoring.Config;
using Unity.Entities;

namespace Baker.Config
{
    public class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            
            AddComponent(entity, new ConfigComponent()
            {
                EnemyTiny = GetEntity(authoring.EnemyTiny,TransformUsageFlags.Dynamic),
                EnemyNormal = GetEntity(authoring.EnemyNormal,TransformUsageFlags.Dynamic),
                EnemyHeavy = GetEntity(authoring.EnemyHeavy,TransformUsageFlags.Dynamic),
                TowerMine = GetEntity(authoring.TowerMine,TransformUsageFlags.Dynamic),
                TowerTurret = GetEntity(authoring.TowerTurret,TransformUsageFlags.Dynamic),
                TowerMortar = GetEntity(authoring.TowerMortar,TransformUsageFlags.Dynamic),
                MinePrefab = GetEntity(authoring.MinePrefab,TransformUsageFlags.Dynamic)
            });
        }
    }
}