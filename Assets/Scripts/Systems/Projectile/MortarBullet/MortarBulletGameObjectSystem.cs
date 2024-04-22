using Components.Projectile;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MortarBulletSystem))]
public partial struct MortarBulletGameObjectSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (bulletReference, entity) in SystemAPI.Query<MortarBulletGameObjectPrefab>()
                     .WithNone<MortarBulletReference>().WithEntityAccess())
        {
            var CompanionGameObject = Object.Instantiate(bulletReference.Value);
            var transformReference = new MortarBulletReference()
            {
                ParticleSystem = CompanionGameObject.GetComponent<ParticleSystem>()
            };

            transformReference.ParticleSystem.Pause();
            
            ecb.AddComponent(entity,transformReference);
        }

        foreach (var (transform,transformReference) in SystemAPI.Query<LocalTransform,MortarBulletReference>())
        {
            transformReference.ParticleSystem.transform.position = transform.Position;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}