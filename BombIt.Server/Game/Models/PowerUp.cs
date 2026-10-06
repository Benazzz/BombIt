using BombIt.Shared.Enums;
using BombIt.Shared.DTOs;
using BombIt.Server.Game.PowerUps.Strategies;

namespace BombIt.Server.Game.Models;

public abstract class PowerUp
{
    private IPowerUpStrategy _strategy;

    public int X { get; }
    public int Y { get; }
    public abstract PowerUpType Type { get; }

    protected PowerUp(int x, int y, IPowerUpStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);
        X = x;
        Y = y;
        _strategy = strategy;
    }

    public void Apply(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        _strategy.Apply(player);
    }

    public void SetStrategy(IPowerUpStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);
        _strategy = strategy;
    }

    public PowerUpStateDto ToDto()
    {
        return new PowerUpStateDto
        {
            X = X + 0.5,
            Y = Y + 0.5,
            Type = Type
        };
    }
}
