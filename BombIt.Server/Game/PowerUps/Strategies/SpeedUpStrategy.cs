using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.PowerUps.Strategies;

public sealed class SpeedUpStrategy : IPowerUpStrategy
{
    public void Apply(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        player.SpeedMultiplier = Math.Min(player.SpeedMultiplier + 0.15, 2.0);
    }
}
