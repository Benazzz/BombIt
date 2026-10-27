using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;
using BombIt.Shared.Enums;
using System.Text.Json;
using BombIt.Server.Game.MapFactories;
using BombIt.Server.Game.Maps.Prototypes;
using BombIt.Server.Game.Maps.Builders;

namespace BombIt.Server.Game.Models;

// Prototype Pattern - Concrete Prototype (IPrototype<Map>)
public class Map : IPrototype<Map>
{
    public int Width { get; }
    public int Height { get; }
    public TileType[][] Tiles { get; }

    internal Map(int width, int height, TileType[][] tiles)
    {
        Width = width;
        Height = height;
        Tiles = tiles;
    }

    // Prototype Pattern - Deep copy method (for shallow copy leave return only clonedTiles-> this.Tiles)
    public Map Clone()
    {
        var clonedTiles = new TileType[Height][];
        for (int y = 0; y < Height; y++)
        {
            clonedTiles[y] = (TileType[])Tiles[y].Clone();
        }
        return new Map(Width, Height, clonedTiles);
    }
     // Deep copy vs shallow copy test
    // public Map DeepClone()
    // {
    //     var clonedTiles = new TileType[Height][];
    //     for (int y = 0; y < Height; y++)
    //     {
    //         clonedTiles[y] = (TileType[])Tiles[y].Clone();
    //     }
    //     return new Map(Width, Height, clonedTiles);
    // }
    // // Prototype Pattern - Shallow copy method
    // public Map ShallowClone()
    // {
    //     // Naujas Map objektas, tačiau Tiles masyvas rodo į TĄ PATĮ atminties adresą!
    //     return new Map(Width, Height, Tiles);
    // }
    // public Map Clone()
    // {
    //     return DeepClone(); 
    // }

  

    private class MapJson
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string[] Layout { get; set; } = Array.Empty<string>();
    }

    public static Map LoadDefault() => LoadFromFile("default-map.json");

    public static Map LoadFromFile(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Maps", fileName);
        var json = File.ReadAllText(path);
        var raw = JsonSerializer.Deserialize<MapJson>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidOperationException("Failed to parse map JSON.");

        // Builder Pattern - Director uses FileTileMapBuilder to construct the Map from layout
        var director = new MapDirector(new FileTileMapBuilder());
        return director.BuildFromLayout(raw.Width, raw.Height, raw.Layout);
    }

    public MapDto ToDto()
    {
        var clonedTiles = new TileType[Height][];
        for (int y = 0; y < Height; y++)
        {
            clonedTiles[y] = (TileType[])Tiles[y].Clone();
        }

        return new MapDto
        {
            Width = Width,
            Height = Height,
            Tiles = clonedTiles
        };
    }
}