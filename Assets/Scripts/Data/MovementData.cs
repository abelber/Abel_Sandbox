using Unity.Entities;

[GenerateAuthoringComponent]
public struct MovementData : IComponentData
{
    public float speedForward;
    public float speedBackward;
    public float rotationSpeed;
}
