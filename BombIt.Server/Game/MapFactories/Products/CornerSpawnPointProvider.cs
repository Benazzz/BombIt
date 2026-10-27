namespace BombIt.Server.Game.MapFactories.Products;

// Abstract Factory Pattern - Concrete Product
public sealed class CornerSpawnPointProvider : ISpawnPointProvider
{
    private readonly int _width;
    private readonly int _height;

    public CornerSpawnPointProvider(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public (double X, double Y)[] GetSpawnPoints() => new[]
    {
        (1.5, 1.5),
        (_width - 1.5, 1.5),
        (1.5, _height - 1.5),
        (_width - 1.5, _height - 1.5)
    };
}
