using Microsoft.AspNetCore.SignalR;
using BombIt.Server.Hubs;
using BombIt.Server.Game.Facade;

namespace BombIt.Server.Game.Services;

// Facade Pattern 
public class GameLoopService : BackgroundService
{
    private readonly IHubContext<GameHub> _hubContext;
    private readonly GameFacade _game;
    private const int TickRateMs = 1000 / 30; // ~33ms

    public GameLoopService(IHubContext<GameHub> hubContext, GameFacade game)
    {
        _hubContext = hubContext;
        _game = game;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var state = _game.Update();

            await _hubContext.Clients.Group(GameHub.PlayersGroup)
                .SendAsync("ReceiveGameState", state, stoppingToken);

            await Task.Delay(TickRateMs, stoppingToken);
        }
    }
}