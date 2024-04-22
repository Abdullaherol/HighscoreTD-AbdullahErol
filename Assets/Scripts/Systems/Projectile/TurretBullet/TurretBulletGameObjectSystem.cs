using Components.Projectile;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TurretBulletSystem))]
public partial struct TurretBulletGameObjectSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (bulletReference, entity) in SystemAPI.Query<TurretBulletGameObjectPrefab>()
                     .WithNone<TurretBulletReference>().WithEntityAccess())
        {
            var CompanionGameObject = Object.Instantiate(bulletReference.Value);
            var transformReference = new TurretBulletReference()
            {
                ParticleSystem = CompanionGameObject.GetComponent<ParticleSystem>()
            };

            transformReference.ParticleSystem.Pause();
            
            ecb.AddComponent(entity,transformReference);
        }

        foreach (var (transform,transformReference) in SystemAPI.Query<LocalTransform,TurretBulletReference>())
        {
            transformReference.ParticleSystem.transform.position = transform.Position;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

}