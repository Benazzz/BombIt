using BombIt.Server.Game.Models;
using BombIt.Server.Game.PowerUps.Products;
using BombIt.Server.Game.PowerUps.Strategies;
using BombIt.Shared.Enums;

namespace BombIt.Server.Game.PowerUps.Factories;

public sealed class PowerUpFactory : PowerUpCreator
{
    public override PowerUp CreatePowerUp(PowerUpType type, int x, int y)
    {
        return type switch
        {
            PowerUpType.BombUp => new BombUp(x, y, new BombUpStrategy()),
            PowerUpType.FireUp => new FireUp(x, y, new FireUpStrategy()),
            PowerUpType.SpeedUp => new SpeedUp(x, y, new SpeedUpStrategy()),
            PowerUpType.Invulnerability =>
                new InvulnerabilityPowerUp(x, y, new InvulnerabilityStrategy()),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown power-up type.")
        };
    }
}
