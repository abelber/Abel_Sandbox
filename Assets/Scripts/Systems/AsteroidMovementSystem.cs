using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Transforms;
using Random = UnityEngine.Random;

[AlwaysSynchronizeSystem]
public class AsteroidMovementSystem : JobComponentSystem
{
    protected override void OnStartRunning()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((Rotation entityRotation, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, in AsteroidMovementData mData) =>
        {
            var linearVelocity = physicsVelocity.Linear.xyz;

            var rndX = Random.Range(-1, 1);
            var rndZ = Random.Range(-1, 1);

            linearVelocity += new float3(rndX, 0, rndZ).xyz * deltaTime * (Random.Range(mData.speedMin, mData.speedMax));

            physicsVelocity.ApplyLinearImpulse(physicsMass, linearVelocity);
        }).Run();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return default;
    }
}