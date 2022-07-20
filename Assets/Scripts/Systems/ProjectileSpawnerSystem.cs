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
        var input = Input.GetKeyDown(KeyCode.Space);
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();//.ToConcurrent();

        if (input)
        {
            Entities.WithBurst(FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true).ForEach((Entity entity, in ProjectileSpawnerData mData) =>
            {
                var instance = commandBuffer.Instantiate(mData.prefab);

                commandBuffer.SetComponent(entity, new Translation { Value = Vector3.zero });
            }).ScheduleParallel();
        }
    }
}