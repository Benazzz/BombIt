using BombIt.Server.Game.Models;
using BombIt.Server.Game.PowerUps.Products;
using BombIt.Server.Game.PowerUps.Strategies;

namespace BombIt.Server.Game.PowerUps.Factories;

public sealed class BombUpCreator : PowerUpCreator
{
    public override PowerUp CreatePowerUp(int x, int y)
    {
        return new BombUp(x, y, new BombUpStrategy());
    }
}
