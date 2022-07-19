using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class AsteroidSpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();

        Entities.WithName("AsteroidSpawnerSystem").WithBurst(FloatMode.Default, FloatPrecision.Standard, true).ForEach((Entity entity, int entityInQueryIndex, in SpawnerData spawnerData, in LocalToWorld location) =>
        {
            for(var x=0; x < spawnerData.countX; x++)
            {
                for (var y = 0; y < spawnerData.countY; y++)
                {
                    var instance = commandBuffer.Instantiate(spawnerData.prefab);

                    var position = math.transform(location.Value, Vector3.zero);

                    commandBuffer.SetComponent(instance, new Translation {Value = position});
                }
            }

            commandBuffer.DestroyEntity(entity);
        }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}