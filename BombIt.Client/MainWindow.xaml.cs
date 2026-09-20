using System.Windows;
using System.Windows.Input;
using BombIt.Client.Services;
using BombIt.Client.Rendering;
using BombIt.Shared.Commands;
using BombIt.Shared.Enums;

namespace BombIt.Client;

public partial class MainWindow : Window
{
    private readonly SignalRClient _signalRClient;
    private readonly GameRenderer _renderer;
    private Direction _currentDirection = Direction.None;

    public MainWindow()
    {
        InitializeComponent();
        _signalRClient = new SignalRClient();
        _signalRClient.Connected += OnConnected;
        _signalRClient.GameStateReceived += OnGameStateReceived;
        _renderer = new GameRenderer(MapCanvas);
    }

    private async void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        ConnectButton.IsEnabled = false;
        StatusText.Text = "Connecting...";
        try
        {
            await _signalRClient.StartAsync();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Connection failed: {ex.Message}";
            ConnectButton.IsEnabled = true;
        }
    }

    private async void OnConnected(string connectionId)
    {
        try
        {
            var map = await _signalRClient.GetMapAsync();

            await Dispatcher.InvokeAsync(() =>
            {
                StatusText.Text = $"Connected! ID: {connectionId}";
                _renderer.DrawMap(map);
            });
        }
        catch (Exception ex)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                StatusText.Text = $"Error: {ex.Message}";
            });
        }
    }

    private void OnGameStateReceived(BombIt.Shared.DTOs.GameStateDto state)
    {
        Dispatcher.Invoke(() =>
        {
            _renderer.DrawPlayers(state.Players);
        });
    }

    private async void Window_KeyDown(object sender, KeyEventArgs e)
    {
        var direction = e.Key switch
        {
            Key.W or Key.Up => Direction.Up,
            Key.S or Key.Down => Direction.Down,
            Key.A or Key.Left => Direction.Left,
            Key.D or Key.Right => Direction.Right,
            _ => _currentDirection
        };

        if (direction != _currentDirection)
        {
            _currentDirection = direction;
            await _signalRClient.SendInputAsync(new PlayerInputCommand { Direction = direction });
        }
    }

    private async void Window_KeyUp(object sender, KeyEventArgs e)
    {
        _currentDirection = Direction.None;
        await _signalRClient.SendInputAsync(new PlayerInputCommand { Direction = Direction.None });
    }
}