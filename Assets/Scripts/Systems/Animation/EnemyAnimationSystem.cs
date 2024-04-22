using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

public partial struct EnemyAnimationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (enemyGameObjectPrefab, entity) in SystemAPI.Query<EnemyGameObjectPrefab>()
                     .WithNone<EnemyAnimatorReference>().WithEntityAccess())
        {
            var CompanionGameObject = Object.Instantiate(enemyGameObjectPrefab.Value);
            var animatorReference = new EnemyAnimatorReference()
            {
                Value = CompanionGameObject.GetComponent<Animator>()
            };
            ecb.AddComponent(entity,animatorReference);
        }

        foreach (var (transform,animatorReference) in SystemAPI.Query<LocalTransform,EnemyAnimatorReference>())
        {
            animatorReference.Value.transform.position = transform.Position;
            animatorReference.Value.transform.rotation = transform.Rotation;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}