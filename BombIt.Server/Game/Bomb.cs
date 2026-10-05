using BombIt.Shared.DTOs;

namespace BombIt.Server.Game;

public class Bomb
{
    public string OwnerId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public int RemainingTicks { get; set; }
    public int Radius { get; set; }

    public Bomb(string ownerId, double x, double y, int remainingTicks, int radius)
    {
        OwnerId = ownerId;
        X = x;
        Y = y;
        RemainingTicks = remainingTicks;
        Radius = radius;
    }

    public BombStateDto ToDto()
    {
        return new BombStateDto
        {
            X = X,
            Y = Y
        };
    }
}
