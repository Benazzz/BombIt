using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BombIt.Shared.DTOs;
using BombIt.Shared.Enums;

namespace BombIt.Client.Rendering;

public class GameRenderer
{
    private const int TileSize = 40;
    private readonly Canvas _canvas;
    private readonly Dictionary<string, Ellipse> _playerShapes = new();

    public GameRenderer(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void DrawMap(MapDto map)
    {
        _canvas.Children.Clear();
        _playerShapes.Clear();
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
                    Fill = GetTileColor(tile),
                    Stroke = Brushes.Gray,
                    StrokeThickness = 0.5
                };

                Canvas.SetLeft(rect, x * TileSize);
                Canvas.SetTop(rect, y * TileSize);
                _canvas.Children.Add(rect);
            }
        }
    }

    public void DrawPlayers(IReadOnlyList<PlayerStateDto> players)
    {
        var activeIds = new HashSet<string>();

        foreach (var player in players)
        {
            activeIds.Add(player.PlayerId);

            if (!_playerShapes.TryGetValue(player.PlayerId, out var ellipse))
            {
                ellipse = new Ellipse
                {
                    Width = TileSize * 0.7,
                    Height = TileSize * 0.7,
                    Fill = Brushes.Blue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                _playerShapes[player.PlayerId] = ellipse;
                _canvas.Children.Add(ellipse);
            }

            Canvas.SetLeft(ellipse, player.X * TileSize - ellipse.Width / 2);
            Canvas.SetTop(ellipse, player.Y * TileSize - ellipse.Height / 2);
            ellipse.Visibility = player.IsAlive
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
        }

        // Pašaliname formas žaidėjų, kurie atsijungė
        var toRemove = _playerShapes.Keys.Where(id => !activeIds.Contains(id)).ToList();
        foreach (var id in toRemove)
        {
            _canvas.Children.Remove(_playerShapes[id]);
            _playerShapes.Remove(id);
        }
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