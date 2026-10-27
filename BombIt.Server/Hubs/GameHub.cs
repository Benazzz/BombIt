using Microsoft.AspNetCore.SignalR;
using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;
using BombIt.Server.Game.Facade;

namespace BombIt.Server.Hubs;

// Facade Pattern
public class GameHub : Hub
{
    public const string PlayersGroup = "Players";

    private readonly GameFacade _game;

    public GameHub(GameFacade game)
    {
        _game = game;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public async Task<bool> Join()
    {
        if (!_game.Connect(Context.ConnectionId))
        {
            Console.WriteLine($"Client rejected (server full): {Context.ConnectionId}");
            return false;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, PlayersGroup);
        Console.WriteLine($"Client joined: {Context.ConnectionId}");
        return true;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _game.Disconnect(Context.ConnectionId);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public Task SendInput(PlayerInputCommand input)
    {
        _game.HandleInput(Context.ConnectionId, input);
        return Task.CompletedTask;
    }

    public Task StartGame(int rounds, int timeSeconds, string mapChoice)
    {
        _game.StartMatch(Context.ConnectionId, rounds, timeSeconds, mapChoice);
        return Task.CompletedTask;
    }

    public Task<MapDto> GetMap()
    {
        return Task.FromResult(_game.GetMap());
    }

    public Task StartCustomGame(int rounds, int timeSeconds, MapThemeDto theme)
    {
        _game.StartCustomMatch(Context.ConnectionId, rounds, timeSeconds, theme);
        return Task.CompletedTask;
    }
}