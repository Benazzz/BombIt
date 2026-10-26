using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BombIt.Client.Rendering.PlayerVisuals;
using BombIt.Shared.DTOs;
using BombIt.Shared.Enums;

namespace BombIt.Client.Rendering;

public class GameRenderer
{
    private const int TileSize = 40;
    private readonly Canvas _canvas;
    private readonly Dictionary<string, Canvas> _playerShapes = new();
    private readonly IPlayerVisual _playerVisual;
    private readonly BitmapImage _bombImage = new(new Uri("pack://application:,,,/Assets/bomb.png"));

    private readonly List<UIElement> _dynamicObjects = new();

    public GameRenderer(Canvas canvas)
    {
        _canvas = canvas;
        _playerVisual = new HostBadgeDecorator(
            new NameTagDecorator(
                new OutlineDecorator(new BasicPlayerVisual())));
    }

    public void DrawMap(MapDto map)
    {
        var tileBrushes = new Dictionary<TileType, Brush>
        {
            [TileType.Empty] = ThemeBrush(map.Theme?.EmptyColor, TileType.Empty),
            [TileType.Destructible] = ThemeBrush(map.Theme?.DestructibleColor, TileType.Destructible),
            [TileType.Indestructible] = ThemeBrush(map.Theme?.IndestructibleColor, TileType.Indestructible),
        };
        var gridBrush = ThemeBrush(map.Theme?.GridLineColor, null) ?? Brushes.Gray;

        _canvas.Children.Clear();
        _playerShapes.Clear();
        _dynamicObjects.Clear();
        _canvas.Width = map.Width * TileSize;
        _canvas.Height = map.Height * TileSize;

        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var tile = map.Tiles[y][x];
                var rect = new Rectangle
                {
                    Width = TileSize,
                    Height = TileSize,
                    Fill = tileBrushes.GetValueOrDefault(tile, Brushes.Magenta),
                    Stroke = gridBrush,
                    StrokeThickness = 0.5
                };

                Canvas.SetLeft(rect, x * TileSize);
                Canvas.SetTop(rect, y * TileSize);
                _canvas.Children.Add(rect);
            }
        }
    }

    private static Brush ThemeBrush(string? hex, TileType? fallbackTile)
    {
        if (!string.IsNullOrEmpty(hex))
        {
            var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
            brush.Freeze();
            return brush;
        }
        return fallbackTile is { } t ? GetTileColor(t) : Brushes.Gray;
    }

    public void DrawDynamicState(GameStateDto state)
    {
        // 1. Remove previous dynamic objects
        foreach (var obj in _dynamicObjects)
        {
            _canvas.Children.Remove(obj);
        }
        _dynamicObjects.Clear();

        // 2. Draw Power-Ups
        foreach (var pu in state.PowerUps)
        {
            var (color, label) = GetPowerUpVisual(pu.Type);
            var rect = new Rectangle
            {
                Width = TileSize * 0.6,
                Height = TileSize * 0.6,
                Fill = color,
                RadiusX = 4,
                RadiusY = 4
            };
            Canvas.SetLeft(rect, pu.X * TileSize - rect.Width / 2);
            Canvas.SetTop(rect, pu.Y * TileSize - rect.Height / 2);
            _canvas.Children.Add(rect);
            _dynamicObjects.Add(rect);

            var text = new TextBlock
            {
                Text = label,
                Foreground = Brushes.White,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(text, pu.X * TileSize - text.DesiredSize.Width / 2);
            Canvas.SetTop(text, pu.Y * TileSize - text.DesiredSize.Height / 2);
            _canvas.Children.Add(text);
            _dynamicObjects.Add(text);
        }

        // 3. Draw Bombs
        foreach (var bomb in state.Bombs)
        {
            var img = new Image
            {
                Source = _bombImage,
                Width = TileSize * 0.8,
                Height = TileSize * 0.8
            };
            Canvas.SetLeft(img, bomb.X * TileSize - img.Width / 2);
            Canvas.SetTop(img, bomb.Y * TileSize - img.Height / 2);
            _canvas.Children.Add(img);
            _dynamicObjects.Add(img);
        }

        // 4. Draw Explosions
        foreach (var exp in state.Explosions)
        {
            var rect = new Rectangle
            {
                Width = TileSize,
                Height = TileSize,
                Fill = Brushes.OrangeRed,
                Opacity = 0.8
            };
            Canvas.SetLeft(rect, exp.X * TileSize - rect.Width / 2);
            Canvas.SetTop(rect, exp.Y * TileSize - rect.Height / 2);
            _canvas.Children.Add(rect);
            _dynamicObjects.Add(rect);
        }

        // 5. Draw Players
        var activeIds = new HashSet<string>();
        foreach (var player in state.Players)
        {
            activeIds.Add(player.PlayerId);

            if (!_playerShapes.TryGetValue(player.PlayerId, out var visualCanvas))
            {
                visualCanvas = new Canvas { IsHitTestVisible = false };
                Panel.SetZIndex(visualCanvas, 1);
                _playerShapes[player.PlayerId] = visualCanvas;
                _canvas.Children.Add(visualCanvas);
            }

            visualCanvas.Children.Clear();
            Canvas.SetLeft(visualCanvas, player.X * TileSize);
            Canvas.SetTop(visualCanvas, player.Y * TileSize);
            visualCanvas.Visibility = player.IsAlive
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (player.IsAlive)
            {
                _playerVisual.Draw(visualCanvas, player, TileSize,
                    player.PlayerId == state.HostConnectionId);
            }
        }
        var toRemove = _playerShapes.Keys.Where(id => !activeIds.Contains(id)).ToList();
        foreach (var id in toRemove)
        {
            _canvas.Children.Remove(_playerShapes[id]);
            _playerShapes.Remove(id);
        }
    }

    private static (Brush color, string label) GetPowerUpVisual(PowerUpType type)
    {
        return type switch
        {
            PowerUpType.BombUp => (Brushes.Purple, "B+"),
            PowerUpType.FireUp => (Brushes.Red, "F+"),
            PowerUpType.SpeedUp => (Brushes.DeepSkyBlue, "S+"),
            PowerUpType.Invulnerability => (Brushes.Gold, "INV"),
            _ => (Brushes.Magenta, "?")
        };
    }

    private static Brush GetTileColor(TileType tile)
    {
        return tile switch
        {
            TileType.Indestructible => Brushes.DimGray,
            TileType.Destructible => Brushes.SaddleBrown,
            TileType.Empty => Brushes.LightGreen,
            _ => Brushes.Magenta
        };
    }
}