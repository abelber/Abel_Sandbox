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
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, Rotation entityRotation, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, in AsteroidMovementData mData) =>
        {
            var linearVelocity = physicsVelocity.Linear.xyz;

            linearVelocity += math.mul(entityRotation.Value, Vector3.forward).xyz * deltaTime * (Random.Range(mData.speedMin, mData.speedMax));

            physicsVelocity.ApplyLinearImpulse(physicsMass, linearVelocity);

            commandBuffer.RemoveComponent<AsteroidMovementData>(entity);
        }).Run();

        return default;
    }
}