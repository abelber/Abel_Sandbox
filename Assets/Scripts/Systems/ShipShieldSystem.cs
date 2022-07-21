using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using System.Numerics;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ShipShieldSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    float cont = 0;
    
    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();//.ToConcurrent();
        

        Entities.ForEach((Entity entity, in ShipShieldData mData) =>
        {
            if(mData.shieldEnabled)
            {
                cont += Time.DeltaTime;

                if (cont > 6) //Shield during 6 seconds
                {
                    cont = 0;
                    commandBuffer.SetComponent(entity, new ShipShieldData { shieldEnabled = false });
                }

            }
        }).WithoutBurst().Run();

    }
}