using BombIt.Shared.DTOs;

namespace BombIt.Server.Game;

public class Player
{
    public string ConnectionId { get; }
    public string Name { get; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool IsAlive { get; set; } = true;

    public Player(string connectionId, string name, double startX, double startY)
    {
        ConnectionId = connectionId;
        Name = name;
        X = startX;
        Y = startY;
    }

    public PlayerStateDto ToDto()
    {
        return new PlayerStateDto
        {
            PlayerId = ConnectionId,
            Name = Name,
            X = X,
            Y = Y,
            IsAlive = IsAlive
        };
    }
}