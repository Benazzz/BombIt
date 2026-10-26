using BombIt.Server.Game.MapFactories.Products;
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.MapFactories;

// Abstract Factory Pattern - Abstract Factory
// Kiekviena konkreti fabrika sukuria vieno zemelapio "seima":
// isdestyma (Map), vizualine tema (MapTheme) ir atsiradimo vietas (ISpawnPointProvider).
public interface IMapFactory
{
    string Name { get; }
    Map CreateMap();
    MapTheme CreateTheme();
    ISpawnPointProvider CreateSpawnPoints(Map map);
}
