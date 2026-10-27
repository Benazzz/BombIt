using BombIt.Server.Game.MapFactories.Products;
using BombIt.Server.Game.Maps.Builders;
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.MapFactories;

// Abstract Factory Pattern - Concrete Factory
// Builder Pattern - Client: uses MapDirector + ProceduralTileMapBuilder to procedurally generate a unique map each time
public sealed class DesertMapFactory : IMapFactory
{
    private readonly MapDirector _director = new(new ProceduralTileMapBuilder());

    public string Name => "Desert";

    // Builder Pattern - Procedurally constructs a new unique desert arena on every call
    public Map CreateMap() =>
        _director.BuildProceduralArena(15, 13, 0.65, Random.Shared);

    public MapTheme CreateTheme() => new DesertTheme();

    public ISpawnPointProvider CreateSpawnPoints(Map map) =>
        new CornerSpawnPointProvider(map.Width, map.Height);
}
