using Microsoft.AspNetCore.SignalR;
using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;
using BombIt.Server.Game;

namespace BombIt.Server.Hubs;

public class GameHub : Hub
{
    private readonly GameStateManager _gameStateManager;

    public GameHub(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public override async Task OnConnectedAsync()
    {
        _gameStateManager.AddPlayer(Context.ConnectionId);
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _gameStateManager.RemovePlayer(Context.ConnectionId);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public Task SendInput(PlayerInputCommand input)
    {
        _gameStateManager.SetInput(Context.ConnectionId, input.Direction);
        return Task.CompletedTask;
    }

    public Task<MapDto> GetMap()
    {
        var map = Game.Map.LoadDefault();
        return Task.FromResult(map.ToDto());
    }
}