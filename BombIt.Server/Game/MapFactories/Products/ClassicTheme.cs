namespace BombIt.Server.Game.MapFactories.Products;

// Abstract Factory Pattern - Concrete Product (spalvos sutampa su dabartinemis klientu)
public sealed class ClassicTheme : MapTheme
{
    public override string Name => "Classic";
    public override string EmptyColor => "#90EE90";          // LightGreen
    public override string DestructibleColor => "#8B4513";   // SaddleBrown
    public override string IndestructibleColor => "#696969"; // DimGray
    public override string GridLineColor => "#808080";       // Gray
}
