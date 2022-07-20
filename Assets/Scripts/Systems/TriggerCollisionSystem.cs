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

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        triggerGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(TriggerCollisionData), }
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle jobHandle = new TriggerCollisionJob
        {
            triggerCollisionGroup = GetComponentDataFromEntity<TriggerCollisionData>(true),
            physicsGranvityFactorGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(),
            physicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>()
        }.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        return jobHandle;
    }

    [BurstCompile]
    struct TriggerCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<TriggerCollisionData> triggerCollisionGroup;
        public ComponentDataFromEntity<PhysicsGravityFactor> physicsGranvityFactorGroup;
        public ComponentDataFromEntity<PhysicsVelocity> physicsVelocityGroup;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyATrigger = triggerCollisionGroup.HasComponent(entityA);
            bool isBodyBTrigger = triggerCollisionGroup.HasComponent(entityB);

            Debug.Log("COLLISION");

            //Ignore trigger overlapping
            if(isBodyATrigger && isBodyBTrigger)
            {
                return;
            }

            if(entityA == null && entityB.GetType() == typeof(AsteroidMovementData))
            {
                Debug.Log("SHIP / ASTEROID - Collision!");
            }

        }
    }
}
