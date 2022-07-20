using Unity.Entities;

public struct TriggerCollisionData : IComponentData
{
    public float GravityFactor;
    public float DampingFactor;
}