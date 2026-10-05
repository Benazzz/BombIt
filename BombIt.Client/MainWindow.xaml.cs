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
    private string _myConnectionId = string.Empty;

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

    private async void StartGameButton_Click(object sender, RoutedEventArgs e)
    {
        int timeSeconds = (int)TimeSlider.Value;
        await _signalRClient.StartGameAsync(timeSeconds);
    }

    private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (TimeSliderLabel != null)
        {
            TimeSliderLabel.Text = $"{e.NewValue} s";
        }
    }

    private async void OnConnected(string connectionId)
    {
        _myConnectionId = connectionId;
        try
        {
            var map = await _signalRClient.GetMapAsync();

            await Dispatcher.InvokeAsync(() =>
            {
                StatusText.Text = $"Connected! ID: {connectionId}";
                _renderer.DrawMap(map);
                ConnectButton.Visibility = Visibility.Collapsed;
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

    private int _currentMapVersion = -1;

    private void OnGameStateReceived(BombIt.Shared.DTOs.GameStateDto state)
    {
        // Handle map updates (fire and forget task so we don't block the UI thread)
        if (state.MapVersion > _currentMapVersion)
        {
            _currentMapVersion = state.MapVersion;
            Task.Run(async () => 
            {
                var map = await _signalRClient.GetMapAsync();
                Dispatcher.Invoke(() => _renderer.DrawMap(map));
            });
        }

        Dispatcher.Invoke(() =>
        {
            var me = state.Players.FirstOrDefault(p => p.PlayerId == _myConnectionId);
            bool amIDead = me != null && !me.IsAlive;

            if (state.Phase == GamePhase.Lobby)
            {
                LobbyPanel.Visibility = Visibility.Visible;
                CountdownPanel.Visibility = Visibility.Hidden;
                SidePanel.Visibility = Visibility.Hidden;
                MapCanvas.Visibility = Visibility.Hidden;
                DeathPanel.Visibility = Visibility.Hidden;
                
                PlayersListText.Text = $"Players connected: {state.Players.Count}/4";
                
                bool isHost = state.HostConnectionId == _myConnectionId;
                StartGameButton.Visibility = isHost ? Visibility.Visible : Visibility.Collapsed;
                HostSettingsPanel.Visibility = isHost ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (state.Phase == GamePhase.Countdown)
            {
                LobbyPanel.Visibility = Visibility.Hidden;
                CountdownPanel.Visibility = Visibility.Visible;
                SidePanel.Visibility = Visibility.Hidden;
                MapCanvas.Visibility = Visibility.Visible; 
                DeathPanel.Visibility = Visibility.Hidden;
                
                int seconds = (state.CountdownValue / 30) + 1;
                CountdownText.Text = seconds.ToString();
            }
            else if (state.Phase == GamePhase.Playing || state.Phase == GamePhase.RoundEnd)
            {
                LobbyPanel.Visibility = Visibility.Hidden;
                CountdownPanel.Visibility = Visibility.Hidden;
                SidePanel.Visibility = Visibility.Visible;
                MapCanvas.Visibility = Visibility.Visible;

                DeathPanel.Visibility = amIDead ? Visibility.Visible : Visibility.Hidden;

                int min = state.RoundTimeLeftSeconds / 60;
                int sec = state.RoundTimeLeftSeconds % 60;
                TimeLeftText.Text = $"{min}:{sec:D2}";
            }

            _renderer.DrawDynamicState(state);
        });
    }

    private async void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await _signalRClient.SendInputAsync(new PlayerInputCommand { Direction = _currentDirection, PlaceBomb = true });
            return;
        }

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
            await _signalRClient.SendInputAsync(new PlayerInputCommand { Direction = direction, PlaceBomb = false });
        }
    }

    private async void Window_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) return;

        _currentDirection = Direction.None;
        await _signalRClient.SendInputAsync(new PlayerInputCommand { Direction = Direction.None, PlaceBomb = false });
    }
}