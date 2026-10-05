using BombIt.Shared.DTOs;
using BombIt.Shared.Enums;

namespace BombIt.Server.Game;

public class Player
{
    public string ConnectionId { get; }
    public string Name { get; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool IsAlive { get; set; } = true;

    // Power-up stats
    public int MaxBombs { get; set; } = 1;
    public int BombRadius { get; set; } = 1;
    public double SpeedMultiplier { get; set; } = 1.0;
    public int InvulnerabilityTicks { get; set; } = 0;

    public Player(string connectionId, string name, double startX, double startY)
    {
        ConnectionId = connectionId;
        Name = name;
        X = startX;
        Y = startY;
    }

    public void ResetPowerUps()
    {
        MaxBombs = 1;
        BombRadius = 1;
        SpeedMultiplier = 1.0;
        InvulnerabilityTicks = 0;
    }

    public int ActiveBombCount(IEnumerable<Bomb> bombs)
    {
        return bombs.Count(b => b.OwnerId == ConnectionId);
    }

    public PlayerStateDto ToDto()
    {
        return new PlayerStateDto
        {
            PlayerId = ConnectionId,
            Name = Name,
            X = X,
            Y = Y,
            IsAlive = IsAlive,
            IsInvulnerable = InvulnerabilityTicks > 0
        };
    }
}