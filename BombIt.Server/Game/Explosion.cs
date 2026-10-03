using BombIt.Shared.DTOs;

namespace BombIt.Server.Game;

public class Explosion
{
    public double X { get; set; }
    public double Y { get; set; }
    public int RemainingTicks { get; set; }

    public Explosion(double x, double y, int remainingTicks)
    {
        X = x;
        Y = y;
        RemainingTicks = remainingTicks;
    }

    public ExplosionStateDto ToDto()
    {
        return new ExplosionStateDto
        {
            X = X,
            Y = Y
        };
    }
}
