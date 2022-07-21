using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;
using Unity.Burst;
using System.Collections.Generic;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnerPowerUpSystem : SystemBase
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

        if (cont > 2 && Random.Range(0,4) == 0) //2 seconds
        {
            Entities.ForEach((Entity entity, in LocalToWorld location, in SpawnerPowerUpData mData) =>
            {
                var prefabs = new List<Entity>();
                prefabs.Add(mData.prefab1);
                prefabs.Add(mData.prefab2);
                prefabs.Add(mData.prefab3);

                var instance = commandBuffer.Instantiate(prefabs[Random.Range(0, prefabs.Count)]);

                commandBuffer.SetComponent(instance, new Translation { Value = location.Position });
                commandBuffer.SetComponent(instance, new Rotation { Value = location.Rotation });

                prefabs.Clear();

            }).WithoutBurst().Run();

            cont = 0;
        }
    }
}