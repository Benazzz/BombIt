using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using BombIt.Shared.DTOs;

namespace BombIt.Server.Game.MapFactories.Products;

// Abstract Factory Pattern - Concrete Product (žaidėjo sukurta tema)
// Spalvos ateina iš kliento jau HEX formatu (jas paruošė Adapter Pattern kliente).
public sealed class CustomTheme : MapTheme
{
    private static readonly Regex HexPattern = new("^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    public override string Name => "Custom";
    public override string EmptyColor { get; }
    public override string DestructibleColor { get; }
    public override string IndestructibleColor { get; }
    public override string GridLineColor { get; }

    private CustomTheme(string empty, string destructible, string indestructible, string gridLine)
    {
        EmptyColor = empty;
        DestructibleColor = destructible;
        IndestructibleColor = indestructible;
        GridLineColor = gridLine;
    }

    // Serveris nepasitiki klientu: priima tik tvarkingus "#RRGGBB"
    public static bool TryCreate(MapThemeDto? dto, [NotNullWhen(true)] out CustomTheme? theme)
    {
        theme = null;
        if (dto is null) return false;

        if (!IsHex(dto.EmptyColor) || !IsHex(dto.DestructibleColor) ||
            !IsHex(dto.IndestructibleColor) || !IsHex(dto.GridLineColor))
            return false;

        theme = new CustomTheme(dto.EmptyColor, dto.DestructibleColor, dto.IndestructibleColor, dto.GridLineColor);
        return true;
    }

    private static bool IsHex(string? value) => value is not null && HexPattern.IsMatch(value);
}
