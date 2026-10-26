namespace BombIt.Server.Game.MapFactories.Products;

// Abstract Factory Pattern - Abstract Product (zaideju atsiradimo vietos)
public interface ISpawnPointProvider
{
    (double X, double Y)[] GetSpawnPoints();
}
