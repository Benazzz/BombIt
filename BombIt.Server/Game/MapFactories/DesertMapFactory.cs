using BombIt.Server.Game.MapFactories.Products;
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.MapFactories;

// Abstract Factory Pattern - Concrete Factory
public sealed class DesertMapFactory : IMapFactory
{
    public string Name => "Desert";

    public Map CreateMap() => Map.LoadFromFile("desert-map.json");

    public MapTheme CreateTheme() => new DesertTheme();

    public ISpawnPointProvider CreateSpawnPoints(Map map) =>
        new CornerSpawnPointProvider(map.Width, map.Height);
}
