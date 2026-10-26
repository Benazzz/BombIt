namespace BombIt.Server.Game.MapFactories.Products;

// Abstract Factory Pattern - Concrete Product
public sealed class DesertTheme : MapTheme
{
    public override string Name => "Desert";
    public override string EmptyColor => "#EDC9AF";
    public override string DestructibleColor => "#C2703D";
    public override string IndestructibleColor => "#6B4423";
    public override string GridLineColor => "#B8956A";
}
