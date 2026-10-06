using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.PowerUps.Strategies;

public sealed class FireUpStrategy : IPowerUpStrategy
{
    public void Apply(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        player.BombRadius = Math.Min(player.BombRadius + 1, 5);
    }
}
