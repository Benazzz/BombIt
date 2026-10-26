using BombIt.Server.Game.MapFactories.Products;
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.MapFactories;

// Abstract Factory Pattern - Concrete Factory
public sealed class ClassicMapFactory : IMapFactory
{
    public string Name => "Classic";

    public Map CreateMap() => Map.LoadFromFile("default-map.json");

    public MapTheme CreateTheme() => new ClassicTheme();

    public ISpawnPointProvider CreateSpawnPoints(Map map) =>
        new CornerSpawnPointProvider(map.Width, map.Height);
}
