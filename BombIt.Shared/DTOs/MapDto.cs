using BombIt.Shared.Enums;

namespace BombIt.Shared.DTOs;

public class MapDto
{
    public int Width { get; set; }
    public int Height { get; set; }
    public MapThemeDto? Theme { get; set; }
    public TileType[][] Tiles { get; set; } = Array.Empty<TileType[]>();
}