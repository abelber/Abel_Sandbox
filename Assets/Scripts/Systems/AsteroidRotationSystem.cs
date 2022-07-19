using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

[AlwaysSynchronizeSystem]
public class AsteroidRotationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in AsteroidRotationData asteroidRotation) =>
        {
            rotation.Value = math.mul(rotation.Value, quaternion.RotateX(math.radians(asteroidRotation.rotationSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(asteroidRotation.rotationSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(asteroidRotation.rotationSpeed * deltaTime)));
        }).Run();

        return default;
    }
}