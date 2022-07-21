using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class ShipWallsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>().CreateCommandBuffer();
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, Translation entityLocation, in ShipWallCollisionData mData) =>
        {
            switch (mData.wallSide)
            {
                case ShipWallCollisionData.WallSide.Up:
                    {
                        commandBuffer.SetComponent(entity, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.None });

                        if(entityLocation.Value.z > 25)
                        {
                            commandBuffer.SetComponent(entity, new Translation { Value = new float3(entityLocation.Value.x, 0, entityLocation.Value.z - 45) });
                        }
                        break;
                    }
                case ShipWallCollisionData.WallSide.Down:
                    {
                        commandBuffer.SetComponent(entity, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.None });

                        if (entityLocation.Value.z < -25)
                        {
                            commandBuffer.SetComponent(entity, new Translation { Value = new float3(entityLocation.Value.x, 0, entityLocation.Value.z + 45) });
                        }
                        break;
                    }
                case ShipWallCollisionData.WallSide.Left:
                    {
                        commandBuffer.SetComponent(entity, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.None });

                        if (entityLocation.Value.x < -60)
                        {
                            commandBuffer.SetComponent(entity, new Translation { Value = new float3(entityLocation.Value.x + 100, 0, entityLocation.Value.z) });
                        }
                        break;
                    }
                case ShipWallCollisionData.WallSide.Right:
                    {
                        commandBuffer.SetComponent(entity, new ShipWallCollisionData { wallSide = ShipWallCollisionData.WallSide.None });

                        if (entityLocation.Value.x > 60)
                        {
                            commandBuffer.SetComponent(entity, new Translation { Value = new float3(entityLocation.Value.x - 100, 0, entityLocation.Value.z) });
                        }
                        break;
                    }
            }

        }).Run();

        return default;
    }
}