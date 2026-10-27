using BombIt.Client.Controls;

namespace BombIt.Client.Adapters;

// Adapter Pattern - Adapter
// Adaptuoja ColorWheel, į IHexColorProvider
public sealed class ColorWheelHexAdapter : IHexColorProvider
{
    private readonly ColorWheel _wheel; // Adaptee

    public event EventHandler? ColorChanged;

    public ColorWheelHexAdapter(ColorWheel wheel)
    {
        ArgumentNullException.ThrowIfNull(wheel);
        _wheel = wheel;

        // Adaptee įvykį (HsvChanged) paverčiame Target įvykiu (ColorChanged)
        _wheel.HsvChanged += (_, _) => ColorChanged?.Invoke(this, EventArgs.Empty);
    }

    public string GetHexColor()
    {
        // 1. Nuskaitome Adaptee būseną (HSV)
        // 2. Verčiame į RGB baitus
        var (r, g, b) = HsvConverter.ToRgb(_wheel.Hue, _wheel.Saturation, _wheel.Brightness);

        // 3. Formatuojame į šešioliktainę sistemą
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}
