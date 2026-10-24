using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Rendering.PlayerVisuals;

public sealed class HostBadgeDecorator : PlayerVisualDecorator
{
    public HostBadgeDecorator(IPlayerVisual wrappee) : base(wrappee) { }

    public override void Draw(Canvas canvas, PlayerStateDto player, int tileSize, bool isHost)
    {
        base.Draw(canvas, player, tileSize, isHost);
        if (!isHost)
            return;

        var crown = new Polygon
        {
            Points = new PointCollection
            {
                new(0, 2), new(5, 6), new(10, 0), new(15, 6),
                new(20, 2), new(17, 13), new(3, 13)
            },
            Fill = new LinearGradientBrush(Colors.LightGoldenrodYellow, Colors.Goldenrod, 90),
            Stroke = Brushes.DarkGoldenrod,
            StrokeThickness = 1,
            StrokeLineJoin = PenLineJoin.Round,
            RenderTransform = new ScaleTransform(tileSize / 40.0, tileSize / 40.0)
        };
        Canvas.SetLeft(crown, -tileSize * 0.25);
        Canvas.SetTop(crown, -tileSize * 0.65);
        canvas.Children.Add(crown);
    }
}
