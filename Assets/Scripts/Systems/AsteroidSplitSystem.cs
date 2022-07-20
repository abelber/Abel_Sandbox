using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using System.Numerics;
using System.Collections.Generic;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class AsteroidSplitSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    float cont = 0;
    List<Entity> asteroidSplitInfo;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        asteroidSplitInfo = new List<Entity>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();//.ToConcurrent();
        var asteroidsToSplit = asteroidSplitInfo;

        Entities.ForEach((Entity asteroidEntity, in AsteroidSplitData asteroidSplitData) =>
        {
            if(asteroidSplitData.split)
            {
                asteroidsToSplit.Add(asteroidEntity);
            }
        }).WithoutBurst().Run();

        Entities.ForEach((Entity entity, in LocalToWorld location, in AsteroidSplitInfoData mData) =>
        {
            if(asteroidsToSplit.Contains(entity))
            {
                var instance = commandBuffer.Instantiate(mData.prefab);
                var instance2 = commandBuffer.Instantiate(mData.prefab);

                commandBuffer.SetComponent(instance, new Translation { Value = location.Position });
                commandBuffer.SetComponent(instance, new Rotation { Value = location.Rotation });
                commandBuffer.SetComponent(instance2, new Translation { Value = location.Position });

                commandBuffer.DestroyEntity(entity);
            }
        }).WithoutBurst().Run();

        //commandBuffer.Dispose();
    }
}