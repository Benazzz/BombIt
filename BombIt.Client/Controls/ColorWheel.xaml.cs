using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BombIt.Client.Controls;

// Adapter Pattern
public partial class ColorWheel : UserControl
{
    private const int Size = 200;
    private bool _dragging;

    public double Hue { get; private set; }          // 0..360
    public double Saturation { get; private set; }   // 0..1
    public double Brightness => BrightnessSlider.Value; // 0..1

    public event EventHandler? HsvChanged;

    public ColorWheel()
    {
        InitializeComponent();
        WheelImage.Source = BuildWheelBitmap();

        // Pradinė pozicija - ratas centre (balta)
        MoveMarker(Size / 2.0, Size / 2.0);
    }

    private static WriteableBitmap BuildWheelBitmap()
    {
        var bitmap = new WriteableBitmap(Size, Size, 96, 96, PixelFormats.Bgra32, null);
        var pixels = new byte[Size * Size * 4];
        double radius = Size / 2.0;

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                double dx = x + 0.5 - radius;
                double dy = y + 0.5 - radius;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance > radius) continue; // už rato ribų - permatoma

                var (r, g, b) = HsvConverter.ToRgb(AngleToHue(Math.Atan2(dy, dx)), distance / radius, 1.0);

                int i = (y * Size + x) * 4;
                pixels[i] = b;
                pixels[i + 1] = g;
                pixels[i + 2] = r;
                pixels[i + 3] = 255;
            }
        }

        bitmap.WritePixels(new Int32Rect(0, 0, Size, Size), pixels, Size * 4, 0);
        return bitmap;
    }

    private static double AngleToHue(double radians) =>
        (radians * 180.0 / Math.PI + 360.0) % 360.0;

    private void Wheel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dragging = true;
        WheelImage.CaptureMouse();
        PickAt(e.GetPosition(WheelImage));
    }

    private void Wheel_MouseMove(object sender, MouseEventArgs e)
    {
        if (_dragging) PickAt(e.GetPosition(WheelImage));
    }

    private void Wheel_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _dragging = false;
        WheelImage.ReleaseMouseCapture();
    }

    private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        HsvChanged?.Invoke(this, EventArgs.Empty);
    }

    private void PickAt(Point position)
    {
        double radius = Size / 2.0;
        double dx = position.X - radius;
        double dy = position.Y - radius;
        double angle = Math.Atan2(dy, dx);
        double distance = Math.Min(Math.Sqrt(dx * dx + dy * dy), radius);

        Hue = AngleToHue(angle);
        Saturation = distance / radius;

        MoveMarker(radius + Math.Cos(angle) * distance, radius + Math.Sin(angle) * distance);
        HsvChanged?.Invoke(this, EventArgs.Empty);
    }

    private void MoveMarker(double x, double y)
    {
        Canvas.SetLeft(Marker, x - Marker.Width / 2);
        Canvas.SetTop(Marker, y - Marker.Height / 2);
    }
}
