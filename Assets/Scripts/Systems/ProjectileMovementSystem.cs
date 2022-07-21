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
public class ProjectileMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((Rotation entityRotation, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, in ProjectileData mData) =>
        {
            var linearVelocity = physicsVelocity.Linear.xyz;

            linearVelocity += math.mul(entityRotation.Value, Vector3.forward).xyz * deltaTime * mData.speed;

            linearVelocity.x = ((int)(linearVelocity.x * 100.0f)) / 100.0f;
            linearVelocity.z = ((int)(linearVelocity.z * 100.0f)) / 100.0f;

            physicsVelocity.Linear.xyz = linearVelocity;
        }).Run();

        return default;
    }
}