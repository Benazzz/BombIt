using Microsoft.AspNetCore.SignalR;
using BombIt.Server.Hubs;
using BombIt.Shared.DTOs;
using BombIt.Shared.Enums;

namespace BombIt.Server.Game;

public class GameLoopService : BackgroundService
{
    private readonly GameStateManager _gameStateManager;
    private readonly IHubContext<GameHub> _hubContext;
    private const int TickRateMs = 1000 / 30; // ~33ms

    public GameLoopService(GameStateManager gameStateManager, IHubContext<GameHub> hubContext)
    {
        _gameStateManager = gameStateManager;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _gameStateManager.Tick();

            var state = new GameStateDto
            {
                Phase = GamePhase.Playing,
                Players = _gameStateManager.GetAllPlayers()
                    .Select(p => p.ToDto())
                    .ToList()
            };

            await _hubContext.Clients.All.SendAsync("ReceiveGameState", state, stoppingToken);
            await Task.Delay(TickRateMs, stoppingToken);
        }
    }
}