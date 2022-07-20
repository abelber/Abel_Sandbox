using Unity.Entities;

[GenerateAuthoringComponent]
public struct PowerUpData : IComponentData
{
    public ShipPowerUp.PowerUp powerType;
}