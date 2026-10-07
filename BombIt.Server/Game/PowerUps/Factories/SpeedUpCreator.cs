using BombIt.Server.Game.Models;
using BombIt.Server.Game.PowerUps.Products;
using BombIt.Server.Game.PowerUps.Strategies;

namespace BombIt.Server.Game.PowerUps.Factories;

public sealed class SpeedUpCreator : PowerUpCreator
{
    public override PowerUp CreatePowerUp(int x, int y)
    {
        return new SpeedUp(x, y, new SpeedUpStrategy());
    }
}
