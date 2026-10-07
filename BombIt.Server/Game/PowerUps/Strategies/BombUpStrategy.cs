using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.PowerUps.Strategies;

public sealed class BombUpStrategy : IPowerUpStrategy
{
    public void Apply(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        player.MaxBombs = Math.Min(player.MaxBombs + 1, 5);
    }
}
