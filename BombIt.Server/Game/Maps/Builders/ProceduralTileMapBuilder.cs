namespace BombIt.Server.Game.Maps.Builders;

using BombIt.Server.Game.Models;
using BombIt.Shared.Enums;

// Builder Pattern - Concrete Builder 2: Generates random map layout
public sealed class ProceduralTileMapBuilder : IMapBuilder
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

    public IMapBuilder AddBorderWalls()
    {
        var tiles = RequireTiles();
        for (int x = 0; x < _width; x++)
        {
            tiles[0][x] = TileType.Indestructible;
            tiles[_height - 1][x] = TileType.Indestructible;
        }
        for (int y = 0; y < _height; y++)
        {
            tiles[y][0] = TileType.Indestructible;
            tiles[y][_width - 1] = TileType.Indestructible;
        }
        return this;
    }

    public IMapBuilder AddPillarGrid()
    {
        var tiles = RequireTiles();
        for (int y = 2; y < _height - 1; y += 2)
        {
            for (int x = 2; x < _width - 1; x += 2)
            {
                tiles[y][x] = TileType.Indestructible;
            }
        }
        return this;
    }

    public IMapBuilder AddRandomDestructibles(double density, Random random)
    {
        var tiles = RequireTiles();
        density = Math.Clamp(density, 0.0, 1.0);

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (tiles[y][x] == TileType.Empty && random.NextDouble() < density)
                {
                    tiles[y][x] = TileType.Destructible;
                }
            }
        }
        return this;
    }

    public IMapBuilder ClearSpawnZones()
    {
        var tiles = RequireTiles();
        int right = _width - 2;
        int bottom = _height - 2;
        var corners = new[]
        {
            (x: 1, y: 1, dx: 1, dy: 1),
            (x: right, y: 1, dx: -1, dy: 1),
            (x: 1, y: bottom, dx: 1, dy: -1),
            (x: right, y: bottom, dx: -1, dy: -1)
        };

        foreach (var c in corners)
        {
            ClearCell(tiles, c.x, c.y);
            ClearCell(tiles, c.x + c.dx, c.y);
            ClearCell(tiles, c.x, c.y + c.dy);
        }
        return this;
    }

    // Procedural builder doesn't need a layout file
    public IMapBuilder FromLayout(string[] layout) => this;

    public Map Build()
    {
        var tiles = RequireTiles();
        var map = new Map(_width, _height, tiles);

        _tiles = null;
        _width = 0;
        _height = 0;
        return map;
    }

    private void ClearCell(TileType[][] tiles, int x, int y)
    {
        if (x > 0 && y > 0 && x < _width - 1 && y < _height - 1)
        {
            if (tiles[y][x] == TileType.Destructible)
                tiles[y][x] = TileType.Empty;
        }
    }

    private TileType[][] RequireTiles() =>
        _tiles ?? throw new InvalidOperationException("Must call Reset() before building.");
}
