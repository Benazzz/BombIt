using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.PowerUps.Factories;

public abstract class PowerUpCreator
{
    public abstract PowerUp CreatePowerUp(int x, int y);
}
