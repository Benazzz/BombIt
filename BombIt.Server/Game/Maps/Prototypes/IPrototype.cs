namespace BombIt.Server.Game.Maps.Prototypes;

// Prototype Pattern - Prototype Interface (strongly-typed alternative to ICloneable)
public interface IPrototype<out T>
{
    T Clone();
}
