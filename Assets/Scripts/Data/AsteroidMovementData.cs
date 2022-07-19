using Unity.Entities;

[GenerateAuthoringComponent]
public struct AsteroidMovementData : IComponentData
{
    public float speedMin;
    public float speedMax;
}
