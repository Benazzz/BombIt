using BombIt.Shared.DTOs;

namespace BombIt.Server.Game.MapFactories.Products;

// Abstract Factory Pattern - Abstract Product (vizualine tema)
public abstract class MapTheme
{
    public abstract string Name { get; }
    public abstract string EmptyColor { get; }
    public abstract string DestructibleColor { get; }
    public abstract string IndestructibleColor { get; }
    public abstract string GridLineColor { get; }

    public MapThemeDto ToDto() => new()
    {
        Name = Name,
        EmptyColor = EmptyColor,
        DestructibleColor = DestructibleColor,
        IndestructibleColor = IndestructibleColor,
        GridLineColor = GridLineColor
    };
}
