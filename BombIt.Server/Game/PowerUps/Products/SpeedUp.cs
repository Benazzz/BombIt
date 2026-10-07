using BombIt.Server.Game.Models;
using BombIt.Server.Game.PowerUps.Strategies;
using BombIt.Shared.Enums;

namespace BombIt.Server.Game.PowerUps.Products;

public sealed class SpeedUp : PowerUp
{
    public override PowerUpType Type => PowerUpType.SpeedUp;

    public SpeedUp(int x, int y, IPowerUpStrategy strategy)
        : base(x, y, strategy)
    {
    }
}
