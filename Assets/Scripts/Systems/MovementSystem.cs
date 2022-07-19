using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        float currentAcelInput = Input.GetAxis("Vertical");
        float currentRotationInput = Input.GetAxis("Horizontal");

        Entities.ForEach((ref PhysicsVelocity vel, in MovementData mData) =>
        {
            float newAngularVel = vel.Angular.y;

            newAngularVel = currentRotationInput * mData.rotationSpeed;

            vel.Angular.y = newAngularVel;
        }).Run();

        Entities.ForEach((Rotation entityRotation, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, in MovementData mData) =>
        {
            var linearVelocity = physicsVelocity.Linear.xyz;

            if (currentAcelInput > 0)
            {   //thrust to the right of where the player is facing
                linearVelocity += math.mul(entityRotation.Value, new float3(0, 0, 1)).xyz * deltaTime * mData.speedForward;
            }

            if (currentAcelInput < 0)
            {   //thrust backwards of where the player is facing
                linearVelocity -= math.mul(entityRotation.Value, new float3(0, 0, 1)).xyz * deltaTime * mData.speedBackward;
            }

            linearVelocity.x = ((int)(linearVelocity.x * 100.0f)) / 100.0f;
            linearVelocity.z = ((int)(linearVelocity.z * 100.0f)) / 100.0f;

            physicsVelocity.Linear.xyz = linearVelocity;
        }).Run();

            return default;
    }
}