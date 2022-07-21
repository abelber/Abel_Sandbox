using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShipWallCollisionData : IComponentData
{
    public WallSide wallSide;

    public enum WallSide
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}
