using BombIt.Shared.Enums;
using BombIt.Shared.DTOs;

namespace BombIt.Server.Game;

public class PowerUp
{
    public int X { get; set; }
    public int Y { get; set; }
    public PowerUpType Type { get; set; }

    public PowerUp(int x, int y, PowerUpType type)
    {
        X = x;
        Y = y;
        Type = type;
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
