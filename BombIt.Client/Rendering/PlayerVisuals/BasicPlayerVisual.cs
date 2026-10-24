using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Rendering.PlayerVisuals;

public sealed class BasicPlayerVisual : IPlayerVisual
{
    private static readonly Brush BodyBrush = CreateBodyBrush();

    private static Brush CreateBodyBrush()
    {
        var brush = new RadialGradientBrush
        {
            GradientOrigin = new Point(0.3, 0.2),
            Center = new Point(0.4, 0.35),
            RadiusX = 0.8,
            RadiusY = 0.8
        };
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(165, 243, 252), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(56, 189, 248), 0.35));
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(37, 99, 235), 0.75));
        brush.GradientStops.Add(new GradientStop(Color.FromRgb(30, 58, 138), 1));
        brush.Freeze();
        return brush;
    }

    public void Draw(Canvas canvas, PlayerStateDto player, int tileSize, bool isHost)
    {
        var shadow = new Ellipse
        {
            Width = tileSize * 0.7,
            Height = tileSize * 0.2,
            Fill = Brushes.Black,
            Opacity = 0.22
        };
        Canvas.SetLeft(shadow, -shadow.Width / 2);
        Canvas.SetTop(shadow, tileSize * 0.23);
        canvas.Children.Add(shadow);

        var body = new Ellipse
        {
            Width = tileSize * 0.7,
            Height = tileSize * 0.7,
            Fill = BodyBrush
        };
        Canvas.SetLeft(body, -body.Width / 2);
        Canvas.SetTop(body, -body.Height / 2);
        canvas.Children.Add(body);

        foreach (var offset in new[] { -0.12, 0.12 })
        {
            var eye = new Ellipse
            {
                Width = tileSize * 0.075,
                Height = tileSize * 0.12,
                Fill = Brushes.MidnightBlue
            };
            Canvas.SetLeft(eye, tileSize * offset - eye.Width / 2);
            Canvas.SetTop(eye, -eye.Height / 2);
            canvas.Children.Add(eye);
        }
    }
}
