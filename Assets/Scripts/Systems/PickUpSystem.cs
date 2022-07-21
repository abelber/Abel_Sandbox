using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics;

[AlwaysSynchronizeSystem]
public class PickUpSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        return default;
    }

    private struct TriggerJob// : ITriggerEventsJob
    {
/*        public ComponentDataFromEntity<MovementData> movementEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> entitiesToDelete;
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            if(movementEntities.HasComponent(triggerEvent.))
        }
*/
    }
}