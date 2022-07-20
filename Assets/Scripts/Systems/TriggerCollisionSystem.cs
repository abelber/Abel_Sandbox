using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class TriggerCollisionSystem : JobComponentSystem
{
    BuildPhysicsWorld buildPhysicsWorld;
    StepPhysicsWorld stepPhysicsWorld;

    EntityQuery triggerGroup;

    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        triggerGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(TriggerCollisionData), }
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = new TriggerCollisionJob
        {
            playersGroup = GetComponentDataFromEntity<TriggerCollisionData>(true),
            asteroidsGroup = GetComponentDataFromEntity<AsteroidMovementData>(true),
            powerUpsGroup = GetComponentDataFromEntity<PowerUpData>(true),
            limitsGroup = GetComponentDataFromEntity<LimitsTag>(true),
            physicsGranvityFactorGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(),
            physicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
            entityCommandBuffer = commandBufferSystem.CreateCommandBuffer()
        }.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        jobHandle.Complete();
        return jobHandle;
    }

    [BurstCompile]
    struct TriggerCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<TriggerCollisionData> playersGroup;
        [ReadOnly] public ComponentDataFromEntity<AsteroidMovementData> asteroidsGroup;
        [ReadOnly] public ComponentDataFromEntity<PowerUpData> powerUpsGroup;
        [ReadOnly] public ComponentDataFromEntity<LimitsTag> limitsGroup;
        public ComponentDataFromEntity<PhysicsGravityFactor> physicsGranvityFactorGroup;
        public ComponentDataFromEntity<PhysicsVelocity> physicsVelocityGroup;

        public EntityCommandBuffer entityCommandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyATrigger = playersGroup.HasComponent(entityA);
            bool isBodyBTrigger = playersGroup.HasComponent(entityB);

            Debug.Log("COLLISION");

            //Ignore trigger overlapping
            if(isBodyATrigger && isBodyBTrigger)
            {
                return;
            }

            if (playersGroup.HasComponent(entityB) && asteroidsGroup.HasComponent(entityA))
            {
                Debug.Log("Hit Asteroid!");

                entityCommandBuffer.DestroyEntity(entityB);
            }

            if (playersGroup.HasComponent(entityB) && powerUpsGroup.HasComponent(entityA))
            {
                Debug.Log("Pick PowerUp");
                 entityCommandBuffer.DestroyEntity(entityA);
            }

            if (playersGroup.HasComponent(entityB) && limitsGroup.HasComponent(entityA))
            {
                Debug.Log("Hit Limit");
                //entityCommandBuffer.DestroyEntity(entityB);
            }
        }
    }
}
