using Microsoft.AspNetCore.SignalR;
using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;
using BombIt.Server.Game;

namespace BombIt.Server.Hubs;

public class GameHub : Hub
{
    private GameStateManager State => GameStateManager.Instance;

    public override async Task OnConnectedAsync()
    {
        State.AddPlayer(Context.ConnectionId);
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        State.RemovePlayer(Context.ConnectionId);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public Task SendInput(PlayerInputCommand input)
    {
        State.SetInput(Context.ConnectionId, input);
        return Task.CompletedTask;
    }

    public Task StartGame(int timeSeconds)
    {
        State.StartGame(Context.ConnectionId, timeSeconds);
        return Task.CompletedTask;
    }

    public Task<MapDto> GetMap()
    {
        var map = State.GetMap();
        return Task.FromResult(map.ToDto());
    }
}