using BombIt.Server.Game.MapFactories.Products;
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.MapFactories;

// Abstract Factory Pattern - Concrete Factory
// Prototype Pattern - Client: stores a loaded Map template prototype and deep-clones it on demand
public sealed class DesertMapFactory : IMapFactory
{
    private readonly Map _mapPrototype = Map.LoadFromFile("desert-map.json");

    public string Name => "Desert";

    public Map CreateMap() => _mapPrototype.Clone();

    public MapTheme CreateTheme() => new DesertTheme();

    public ISpawnPointProvider CreateSpawnPoints(Map map) =>
        new CornerSpawnPointProvider(map.Width, map.Height);
}
