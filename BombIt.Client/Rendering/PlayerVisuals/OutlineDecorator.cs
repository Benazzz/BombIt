using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Rendering.PlayerVisuals;

public sealed class OutlineDecorator : PlayerVisualDecorator
{
    public OutlineDecorator(IPlayerVisual wrappee) : base(wrappee) { }

    public override void Draw(Canvas canvas, PlayerStateDto player, int tileSize, bool isHost)
    {
        base.Draw(canvas, player, tileSize, isHost);

        if (player.IsInvulnerable)
        {
            var halo = new Ellipse
            {
                Width = tileSize * 0.88,
                Height = tileSize * 0.88,
                Stroke = Brushes.Gold,
                StrokeThickness = tileSize * 0.08,
                Opacity = 0.35
            };
            Canvas.SetLeft(halo, -halo.Width / 2);
            Canvas.SetTop(halo, -halo.Height / 2);
            canvas.Children.Add(halo);
        }

        var outline = new Ellipse
        {
            Width = tileSize * 0.7,
            Height = tileSize * 0.7,
            Stroke = player.IsInvulnerable ? Brushes.LightGoldenrodYellow : Brushes.LightCyan,
            StrokeThickness = player.IsInvulnerable ? 2.5 : 1.5
        };
        Canvas.SetLeft(outline, -outline.Width / 2);
        Canvas.SetTop(outline, -outline.Height / 2);
        canvas.Children.Add(outline);
    }
}
