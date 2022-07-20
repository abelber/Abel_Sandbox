using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;
using Unity.Burst;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    float cont = 0;

    protected override void OnStartRunning()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        cont += Time.DeltaTime;
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();//.ToConcurrent();

        if (cont > 4)
        {
            Entities.WithBurst(FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true).ForEach((Entity entity, in LocalToWorld location, in SpawnerData mData) =>
            {
                var instance = commandBuffer.Instantiate(mData.prefab);

                commandBuffer.SetComponent(instance, new Translation { Value = location.Position });
                commandBuffer.SetComponent(instance, new Rotation { Value = location.Rotation });
            }).Run();

            cont = 0;
        }
    }
}