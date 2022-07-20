using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
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
            asteroidsGroup = GetComponentDataFromEntity<AsteroidRotationData>(true),
            powerUpsGroup = GetComponentDataFromEntity<PowerUpData>(true),
            limitsUpGroup = GetComponentDataFromEntity<LimitsUpTag>(true),
            limitsDownGroup = GetComponentDataFromEntity<LimitsDownTag>(true),
            limitsLeftGroup = GetComponentDataFromEntity<LimitsLeftTag>(true),
            limitsRightGroup = GetComponentDataFromEntity<LimitsRightTag>(true),
            projectilesGroup = GetComponentDataFromEntity<ProjectileData>(true),
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
        [ReadOnly] public ComponentDataFromEntity<AsteroidRotationData> asteroidsGroup;
        [ReadOnly] public ComponentDataFromEntity<PowerUpData> powerUpsGroup;
        [ReadOnly] public ComponentDataFromEntity<LimitsUpTag> limitsUpGroup;
        [ReadOnly] public ComponentDataFromEntity<LimitsDownTag> limitsDownGroup;
        [ReadOnly] public ComponentDataFromEntity<LimitsLeftTag> limitsLeftGroup;
        [ReadOnly] public ComponentDataFromEntity<LimitsRightTag> limitsRightGroup;
        [ReadOnly] public ComponentDataFromEntity<ProjectileData> projectilesGroup;
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

            //Player / PowerUp
            if (playersGroup.HasComponent(entityA))
            {
                if (powerUpsGroup.HasComponent(entityB))
                {
                    entityCommandBuffer.SetComponent(entityA, new ShipPowerUp { powerUp = powerUpsGroup[entityB].powerType });
                    entityCommandBuffer.DestroyEntity(entityB);
                }
            }
            //Player collisions
            
            if (playersGroup.HasComponent(entityB))
            {
                if (asteroidsGroup.HasComponent(entityA))
                {
                    Debug.Log("Player Hit Asteroid!");
                    entityCommandBuffer.SetComponent(entityB, new Translation { Value = Vector3.zero });
                }

                if (powerUpsGroup.HasComponent(entityA))
                {
                    Debug.Log("Pick PowerUp");
                    entityCommandBuffer.SetComponent(entityB, new ShipPowerUp { powerUp = powerUpsGroup[entityA].powerType });
                    entityCommandBuffer.DestroyEntity(entityA);
                }

                if (limitsUpGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit UP");
                    //entityCommandBuffer.SetComponent(entityB, new MovementData { wallSide = MovementData.WallSide.Up });
                }

                if (limitsDownGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit");
                    //entityCommandBuffer.SetComponent(entityB, new MovementData { wallSide = MovementData.WallSide.Down });
                }

                if (limitsLeftGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit");
                    //entityCommandBuffer.SetComponent(entityB, new MovementData { wallSide = MovementData.WallSide.Left });
                }

                if (limitsRightGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit");
                    //entityCommandBuffer.SetComponent(entityB, new MovementData { wallSide = MovementData.WallSide.Right });
                }

            }

            //Projectiles Collisions
            if (projectilesGroup.HasComponent(entityB))
            {
                if (limitsUpGroup.HasComponent(entityA))
                {
                    Debug.Log("Projectile Hit Limit");
                    entityCommandBuffer.DestroyEntity(entityB);
                }

                if (asteroidsGroup.HasComponent(entityA))
                {
                    Debug.Log("Projectile Hit ASTEROID");
                    entityCommandBuffer.AddComponent(entityA, new AsteroidSplitData { split = true });
                    entityCommandBuffer.DestroyEntity(entityB);
                }
            }
            else if (projectilesGroup.HasComponent(entityA))
            {
                if (asteroidsGroup.HasComponent(entityB))
                {
                    Debug.Log("Projectile Hit ASTEROID");
                    entityCommandBuffer.AddComponent(entityB, new AsteroidSplitData { split = true });
                    entityCommandBuffer.DestroyEntity(entityA);
                }
            }

            //Asteroids Collisions
            if (asteroidsGroup.HasComponent(entityB))
            {
                if (limitsUpGroup.HasComponent(entityA) || limitsDownGroup.HasComponent(entityA) || limitsLeftGroup.HasComponent(entityA) || limitsRightGroup.HasComponent(entityA))
                {
                    Debug.Log("Projectile Hit Limit");
                    entityCommandBuffer.DestroyEntity(entityB);
                }
            }

            //entityCommandBuffer.Playback(World.DefaultGameObjectInjectionWorld.EntityManager);
            //entityCommandBuffer.Dispose();
        }
    }
}
