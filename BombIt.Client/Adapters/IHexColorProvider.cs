namespace BombIt.Client.Adapters;

// Adapter Pattern 
public interface IHexColorProvider
{
    // Iškeliamas, kai žaidėjas pakeičia pasirinktą spalvą
    event EventHandler? ColorChanged;

    // Grąžina dabar pasirinktą spalvą formatu "#RRGGBB"
    string GetHexColor();
}
