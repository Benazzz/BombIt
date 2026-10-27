using Microsoft.AspNetCore.SignalR.Client;
using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;

namespace BombIt.Client.Services;

public class SignalRClient
{
    private readonly HubConnection _connection;

    public event Action<string>? Connected;
    public event Action<GameStateDto>? GameStateReceived;

    public SignalRClient()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5071/gamehub")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<GameStateDto>("ReceiveGameState", state =>
        {
            GameStateReceived?.Invoke(state);
        });
    }

    public async Task<bool> StartAsync()
    {
        await _connection.StartAsync();

        bool joined = await _connection.InvokeAsync<bool>("Join");
        if (!joined)
        {
            await _connection.StopAsync();
            return false;
        }

        Connected?.Invoke(_connection.ConnectionId ?? "unknown");
        return true;
    }

    public async Task<MapDto> GetMapAsync()
    {
        return await _connection.InvokeAsync<MapDto>("GetMap");
    }

    public async Task SendInputAsync(PlayerInputCommand input)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("SendInput", input);
        }
    }

    public async Task StartGameAsync(int rounds, int timeSeconds, string mapChoice)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("StartGame", rounds, timeSeconds, mapChoice);
        }
    }

    public async Task StartCustomGameAsync(int rounds, int timeSeconds, MapThemeDto theme)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("StartCustomGame", rounds, timeSeconds, theme);
        }
    }

    public async Task StopAsync()
    {
        await _connection.StopAsync();
    }
}