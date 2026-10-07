using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.PowerUps.Strategies;

public interface IPowerUpStrategy
{
    void Apply(Player player);
}
