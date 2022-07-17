using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

[AlwaysSynchronizeSystem]
public class RotationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in RotationData rotationData) =>
        {
            rotation.Value = math.mul(rotation.Value, quaternion.RotateX(math.radians(rotationData.rotationSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(rotationData.rotationSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(rotationData.rotationSpeed * deltaTime)));
        }).Run();

        return default;
    }
}