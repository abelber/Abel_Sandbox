using Unity.Entities;

[GenerateAuthoringComponent]
public struct TriggerCollisionData : IComponentData
{
    public float GravityFactor;
    public float DampingFactor;
}