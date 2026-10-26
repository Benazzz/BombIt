namespace BombIt.Server.Game.Maps.Builders;

using BombIt.Server.Game.Models;

// Builder Pattern - Director: manages construction recipes
public class MapDirector
{
    private readonly IMapBuilder _builder;

    public MapDirector(IMapBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _builder = builder;
    }

    // Recipe 1: Construct map from text layout (JSON files)
    public Map BuildFromLayout(int width, int height, string[] layout) =>
        _builder
            .Reset(width, height)
            .FromLayout(layout)
            .Build();

    // Recipe 2: Construct classic arena with procedural random crates
    public Map BuildProceduralArena(int width, int height, double crateDensity, Random rng) =>
        _builder
            .Reset(width, height)
            .AddBorderWalls()
            .AddPillarGrid()
            .AddRandomDestructibles(crateDensity, rng)
            .ClearSpawnZones()
            .Build();
}
