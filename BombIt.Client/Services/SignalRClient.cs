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
            .WithUrl("https://localhost:7039/gamehub")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<GameStateDto>("ReceiveGameState", state =>
        {
            GameStateReceived?.Invoke(state);
        });
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
        Connected?.Invoke(_connection.ConnectionId ?? "unknown");
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

    public async Task StopAsync()
    {
        await _connection.StopAsync();
    }
}