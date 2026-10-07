using System.Windows;
using System.Windows.Input;
using BombIt.Client.Services;
using BombIt.Client.Rendering;
using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;
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

    // PAKEISTA – siunčia ir raundų skaičių
    private async void StartGameButton_Click(object sender, RoutedEventArgs e)
    {
        int rounds = (int)RoundsSlider.Value;
        int timeSeconds = (int)TimeSlider.Value;
        await _signalRClient.StartGameAsync(rounds, timeSeconds);
    }

    // NAUJA
    private void RoundsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (RoundsSliderLabel != null)
        {
            RoundsSliderLabel.Text = $"{e.NewValue}";
        }
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

    // PAKEISTA – RoundEnd / MatchEnd fazės, raundas ir taškai
    private void OnGameStateReceived(GameStateDto state)
    {
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
                ResultPanel.Visibility = Visibility.Hidden;

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
                ResultPanel.Visibility = Visibility.Hidden;

                CountdownAnnouncementText.Text = state.Announcement;
                int seconds = (state.CountdownValue / 30) + 1;
                CountdownText.Text = seconds.ToString();
            }
            else if (state.Phase == GamePhase.Playing)
            {
                LobbyPanel.Visibility = Visibility.Hidden;
                CountdownPanel.Visibility = Visibility.Hidden;
                SidePanel.Visibility = Visibility.Visible;
                MapCanvas.Visibility = Visibility.Visible;
                ResultPanel.Visibility = Visibility.Hidden;

                DeathPanel.Visibility = amIDead ? Visibility.Visible : Visibility.Hidden;

                int min = state.RoundTimeLeftSeconds / 60;
                int sec = state.RoundTimeLeftSeconds % 60;
                TimeLeftText.Text = $"{min}:{sec:D2}";
                UpdateMatchInfo(state);
            }
            else if (state.Phase == GamePhase.RoundEnd || state.Phase == GamePhase.MatchEnd)
            {
                LobbyPanel.Visibility = Visibility.Hidden;
                CountdownPanel.Visibility = Visibility.Hidden;
                SidePanel.Visibility = Visibility.Visible;
                MapCanvas.Visibility = Visibility.Visible;
                DeathPanel.Visibility = Visibility.Hidden;
                ResultPanel.Visibility = Visibility.Visible;

                ResultText.Text = state.Announcement;
                ResultScoresText.Text = BuildScoresText(state);
                UpdateMatchInfo(state);
            }

            _renderer.DrawDynamicState(state);
        });
    }

    // NAUJA – raundo numeris ir taškai šoniniame skydelyje
    private void UpdateMatchInfo(GameStateDto state)
    {
        RoundText.Text = state.IsTieBreak
            ? "TIE-BREAK"
            : $"Round {state.CurrentRound}/{state.TotalRounds}";
        ScoresText.Text = BuildScoresText(state);
    }

    // NAUJA – taškų sąrašas, surikiuotas nuo didžiausio
    private string BuildScoresText(GameStateDto state)
    {
        var lines = state.Players
            .OrderByDescending(p => p.Score)
            .Select(p => $"{p.Name}{(p.PlayerId == _myConnectionId ? " (you)" : "")}: {p.Score}");

        return string.Join(Environment.NewLine, lines);
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