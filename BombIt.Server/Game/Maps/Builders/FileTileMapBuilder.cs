namespace BombIt.Server.Game.Maps.Builders;

using BombIt.Server.Game.Models;
using BombIt.Shared.Enums;

// Builder Pattern - Concrete Builder 1: Map from file layout
public sealed class FileTileMapBuilder : IMapBuilder
{
    private int _width;
    private int _height;
    private TileType[][]? _tiles;

    public IMapBuilder Reset(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 3);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 3);

        _width = width;
        _height = height;
        _tiles = new TileType[height][];
        for (int y = 0; y < height; y++)
        {
            _tiles[y] = new TileType[width]; // Default: TileType.Empty
        }

        return this;
    }

    public IMapBuilder FromLayout(string[] layout)
    {
        var tiles = RequireTiles();
        ArgumentNullException.ThrowIfNull(layout);

        for (int y = 0; y < Math.Min(_height, layout.Length); y++)
        {
            for (int x = 0; x < Math.Min(_width, layout[y].Length); x++)
            {
                tiles[y][x] = layout[y][x] switch
                {
                    '#' => TileType.Indestructible,
                    '+' => TileType.Destructible,
                    _ => TileType.Empty
                };
            }
        }
        return this;
    }

    // In maps from file everything are already set
    public IMapBuilder AddBorderWalls() => this;
    public IMapBuilder AddPillarGrid() => this;
    public IMapBuilder AddRandomDestructibles(double density, Random random) => this;
    public IMapBuilder ClearSpawnZones() => this;

    public Map Build()
    {
        var tiles = RequireTiles();
        var map = new Map(_width, _height, tiles);

        _tiles = null;
        _width = 0;
        _height = 0;
        return map;
    }

    private TileType[][] RequireTiles() =>
        _tiles ?? throw new InvalidOperationException("Must call Reset() before building.");
}
