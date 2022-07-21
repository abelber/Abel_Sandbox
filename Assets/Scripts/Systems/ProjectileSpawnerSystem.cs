using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;
using Unity.Burst;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ProjectileSpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();//.ToConcurrent();

        Entities.WithBurst(FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true).ForEach((Entity entity, ShipPowerUp shipPowerUp, in LocalToWorld location, in ProjectileSpawnerData mData) =>
        {
            switch(shipPowerUp.powerUp)
            {
                case ShipPowerUp.PowerUp.Single:
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            var instance = commandBuffer.Instantiate(mData.prefab);

                            commandBuffer.SetComponent(instance, new Translation { Value = location.Position });
                            commandBuffer.SetComponent(instance, new Rotation { Value = location.Rotation });
                        }

                        break;
                    }
                case ShipPowerUp.PowerUp.Automatic:
                    {
                        if (Input.GetKey(KeyCode.Space))
                        {
                            var instance = commandBuffer.Instantiate(mData.prefab);

                            commandBuffer.SetComponent(instance, new Translation { Value = location.Position });
                            commandBuffer.SetComponent(instance, new Rotation { Value = location.Rotation });
                        }

                        break;
                    }
                case ShipPowerUp.PowerUp.Triple:
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            var instance = commandBuffer.Instantiate(mData.prefab);
                            var instance2 = commandBuffer.Instantiate(mData.prefab);
                            var instance3 = commandBuffer.Instantiate(mData.prefab);

                            commandBuffer.SetComponent(instance, new Translation { Value = location.Position });
                            commandBuffer.SetComponent(instance, new Rotation { Value = location.Rotation });
                            commandBuffer.SetComponent(instance2, new Rotation { Value = location.Rotation });
                            commandBuffer.SetComponent(instance2, new Translation { Value = location.Position - new Unity.Mathematics.float3(1, 0, 0) });
                            commandBuffer.SetComponent(instance3, new Rotation { Value = location.Rotation });
                            commandBuffer.SetComponent(instance3, new Translation { Value = location.Position + new Unity.Mathematics.float3(1, 0, 0) });
                        }

                        break;
                    }
            }
        }).Run();
    }
}