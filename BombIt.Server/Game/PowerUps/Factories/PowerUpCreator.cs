using BombIt.Server.Game.Models;
using BombIt.Shared.Enums;

namespace BombIt.Server.Game.PowerUps.Factories;

public abstract class PowerUpCreator
{
    public abstract PowerUp CreatePowerUp(PowerUpType type, int x, int y);
}
