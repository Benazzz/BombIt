using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Rendering.PlayerVisuals;

public sealed class NameTagDecorator : PlayerVisualDecorator
{
    private static readonly Brush LabelBackground = new SolidColorBrush(Color.FromArgb(235, 15, 23, 42));

    public NameTagDecorator(IPlayerVisual wrappee) : base(wrappee) { }

    public override void Draw(Canvas canvas, PlayerStateDto player, int tileSize, bool isHost)
    {
        base.Draw(canvas, player, tileSize, isHost);

        var nameTag = new Border
        {
            Background = LabelBackground,
            BorderBrush = isHost ? Brushes.Goldenrod : Brushes.SlateGray,
            BorderThickness = new Thickness(0.75),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(5, 1, 5, 2),
            MaxWidth = tileSize * 2,
            Child = new TextBlock
            {
                Text = player.Name,
                Foreground = Brushes.WhiteSmoke,
                FontFamily = new FontFamily("Segoe UI"),
                FontWeight = FontWeights.SemiBold,
                FontSize = 10,
                TextTrimming = TextTrimming.CharacterEllipsis
            }
        };
        nameTag.Measure(new Size(tileSize * 2, double.PositiveInfinity));
        Canvas.SetLeft(nameTag, -nameTag.DesiredSize.Width / 2);
        Canvas.SetTop(nameTag, tileSize * 0.4);
        canvas.Children.Add(nameTag);
    }
}
