using Unity.Entities;

[GenerateAuthoringComponent]
public struct ShipPowerUp : IComponentData
{
    public PowerUp powerUp;

    public enum PowerUp
    {
        Single,
        Automatic,
        Triple
    }
}
