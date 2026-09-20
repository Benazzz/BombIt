using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;
using BombIt.Shared.Enums;
using System.Text.Json;

namespace BombIt.Server.Game;

public class Map
{
    public int Width { get; }
    public int Height { get; }
    public TileType[][] Tiles { get; }

    private Map(int width, int height, TileType[][] tiles)
    {
        Width = width;
        Height = height;
        Tiles = tiles;
    }

    private class MapJson
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string[] Layout { get; set; } = Array.Empty<string>();
    }

    public static Map LoadDefault()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Maps", "default-map.json");
        var json = File.ReadAllText(path);
        var raw = JsonSerializer.Deserialize<MapJson>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidOperationException("Failed to parse map JSON.");

        var tiles = new TileType[raw.Height][];
        for (int y = 0; y < raw.Height; y++)
        {
            tiles[y] = new TileType[raw.Width];
            for (int x = 0; x < raw.Width; x++)
            {
                char c = raw.Layout[y][x];
                tiles[y][x] = c switch
                {
                    '#' => TileType.Indestructible,
                    '+' => TileType.Destructible,
                    _ => TileType.Empty
                };
            }
        }

        return new Map(raw.Width, raw.Height, tiles);
    }

    public MapDto ToDto()
    {
        return new MapDto
        {
            Width = Width,
            Height = Height,
            Tiles = Tiles
        };
    }
}