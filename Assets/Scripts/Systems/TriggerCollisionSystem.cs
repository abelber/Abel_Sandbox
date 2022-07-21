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
            playersTriggerCollisionGroup = GetComponentDataFromEntity<TriggerCollisionData>(true),
            playersHealthGroup = GetComponentDataFromEntity<ShipShieldData>(true),
            asteroidsGroup = GetComponentDataFromEntity<AsteroidRotationData>(true),
            powerUpsGroup = GetComponentDataFromEntity<PowerUpData>(true),
            powerUpsShieldGroup = GetComponentDataFromEntity<PowerUpShieldTag>(true),
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
        [ReadOnly] public ComponentDataFromEntity<TriggerCollisionData> playersTriggerCollisionGroup;
        [ReadOnly] public ComponentDataFromEntity<ShipShieldData> playersHealthGroup;
        [ReadOnly] public ComponentDataFromEntity<AsteroidRotationData> asteroidsGroup;
        [ReadOnly] public ComponentDataFromEntity<PowerUpData> powerUpsGroup;
        [ReadOnly] public ComponentDataFromEntity<PowerUpShieldTag> powerUpsShieldGroup;
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

            bool isBodyATrigger = playersTriggerCollisionGroup.HasComponent(entityA);
            bool isBodyBTrigger = playersTriggerCollisionGroup.HasComponent(entityB);

            Debug.Log("COLLISION");

            //Ignore trigger overlapping
            if(isBodyATrigger && isBodyBTrigger)
            {
                return;
            }

            //Player / Asteroids
            if (playersHealthGroup.HasComponent(entityA))
            {
                //Player SHIELD
                if (playersHealthGroup[entityA].shieldEnabled == false)
                {
                    if (asteroidsGroup.HasComponent(entityB))
                    {
                        Debug.Log("Player Hit Asteroid! as A");
                        entityCommandBuffer.SetComponent(entityA, new Translation { Value = Vector3.zero });
                    }
                }
            }
            else if (playersHealthGroup.HasComponent(entityB))
            {
                //Player SHIELD
                if (playersHealthGroup[entityB].shieldEnabled == false)
                {
                    if (asteroidsGroup.HasComponent(entityA))
                    {
                        Debug.Log("Player Hit Asteroid! as B");
                        entityCommandBuffer.SetComponent(entityB, new Translation { Value = Vector3.zero });
                    }
                }
            }

            //Player / Collisions without asteriods
            if (playersTriggerCollisionGroup.HasComponent(entityA))
            {
                if (powerUpsGroup.HasComponent(entityB))
                {
                    Debug.Log("Pick PowerUp");
                    entityCommandBuffer.SetComponent(entityA, new ShipPowerUp { powerUp = powerUpsGroup[entityB].powerType });
                    entityCommandBuffer.DestroyEntity(entityB);
                }

                if (powerUpsShieldGroup.HasComponent(entityB))
                {
                    Debug.Log("Pick PowerUp - HEALTH 1");
                    entityCommandBuffer.SetComponent(entityA, new ShipShieldData { shieldEnabled = true });
                    entityCommandBuffer.DestroyEntity(entityB);
                }

                if (limitsUpGroup.HasComponent(entityB))
                {
                    Debug.Log("Hit Limit UP");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Up });
                }

                if (limitsDownGroup.HasComponent(entityB))
                {
                    Debug.Log("Hit Limit DOWN");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Down });
                }

                if (limitsLeftGroup.HasComponent(entityB))
                {
                    Debug.Log("Hit Limit LEFT");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Left });
                }

                if (limitsRightGroup.HasComponent(entityB))
                {
                    Debug.Log("Hit Limit RIGHT");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Right });
                }
            }
            else if (playersTriggerCollisionGroup.HasComponent(entityB))
            {
                if (powerUpsGroup.HasComponent(entityA))
                {
                    Debug.Log("Pick PowerUp");
                    entityCommandBuffer.SetComponent(entityB, new ShipPowerUp { powerUp = powerUpsGroup[entityA].powerType });
                    entityCommandBuffer.DestroyEntity(entityA);
                }

                if (powerUpsShieldGroup.HasComponent(entityA))
                {
                    Debug.Log("Pick PowerUp - HEALTH 2");
                    entityCommandBuffer.SetComponent(entityB, new ShipShieldData { shieldEnabled = true });
                    entityCommandBuffer.DestroyEntity(entityA);
                }

                if (limitsUpGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit UP");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Up });
                }

                if (limitsDownGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit DOWN");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Down });
                }

                if (limitsLeftGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit LEFT");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Left });
                }

                if (limitsRightGroup.HasComponent(entityA))
                {
                    Debug.Log("Hit Limit RIGHT");
                    entityCommandBuffer.SetComponent(entityB, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.Right });
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
                if (limitsUpGroup.HasComponent(entityB))
                {
                    Debug.Log("Projectile Hit Limit");
                    entityCommandBuffer.DestroyEntity(entityA);
                }

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
                    Debug.Log("Projectile Hit Limit as B");
                    entityCommandBuffer.DestroyEntity(entityB);
                }
            }
            else if (asteroidsGroup.HasComponent(entityA))
            {
                if (limitsUpGroup.HasComponent(entityB) || limitsDownGroup.HasComponent(entityB) || limitsLeftGroup.HasComponent(entityB) || limitsRightGroup.HasComponent(entityB))
                {
                    Debug.Log("Projectile Hit Limit as A");
                    entityCommandBuffer.DestroyEntity(entityA);
                }
            }

            //PowerUps / Collisions with limits
            if (powerUpsGroup.HasComponent(entityA) || powerUpsShieldGroup.HasComponent(entityA))
            {
                if (limitsUpGroup.HasComponent(entityB) || (limitsDownGroup.HasComponent(entityB)) || (limitsLeftGroup.HasComponent(entityB)) || (limitsRightGroup.HasComponent(entityB)))
                {
                    entityCommandBuffer.DestroyEntity(entityA);
                }
            }
            else if (powerUpsGroup.HasComponent(entityB) || powerUpsShieldGroup.HasComponent(entityB))
            {
                if (limitsUpGroup.HasComponent(entityA) || (limitsDownGroup.HasComponent(entityA)) || (limitsLeftGroup.HasComponent(entityA)) || (limitsRightGroup.HasComponent(entityA)))
                {
                    entityCommandBuffer.DestroyEntity(entityB);
                }
            }
        }
    }
}
