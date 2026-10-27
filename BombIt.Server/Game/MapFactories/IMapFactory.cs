using BombIt.Server.Game.MapFactories.Products;
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.MapFactories;

// Abstract Factory Pattern - Abstract Factory
public interface IMapFactory
{
    string Name { get; }
    Map CreateMap();
    MapTheme CreateTheme();
    ISpawnPointProvider CreateSpawnPoints(Map map);
}
