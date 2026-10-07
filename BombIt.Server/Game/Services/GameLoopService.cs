using Microsoft.AspNetCore.SignalR;
using BombIt.Server.Hubs;
using BombIt.Shared.DTOs;
using BombIt.Shared.Enums;

namespace BombIt.Server.Game.Services;

public class GameLoopService : BackgroundService
{
    private readonly IHubContext<GameHub> _hubContext;
    private const int TickRateMs = 1000 / 30; // ~33ms

    public GameLoopService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var stateManager = GameStateManagerService.Instance;
            stateManager.Tick();

            var state = new GameStateDto
            {
                Phase = stateManager.CurrentPhase,
                HostConnectionId = stateManager.HostConnectionId,
                CountdownValue = stateManager.CountdownTicks,
                RoundTimeLeftSeconds = stateManager.RoundTimeLeftTicks / 30,
                MapVersion = stateManager.MapVersion,
                Players = stateManager.GetAllPlayers().Select(p =>
                {
                    var dto = p.ToDto();
                    dto.Score = stateManager.GetScore(p.ConnectionId);
                    return dto;
                }).ToList(),
                Bombs = stateManager.GetBombs().Select(b => b.ToDto()).ToList(),
                Explosions = stateManager.GetExplosions().Select(e => e.ToDto()).ToList(),
                PowerUps = stateManager.GetPowerUps().Select(p => p.ToDto()).ToList(),
                CurrentRound = stateManager.CurrentRound,
                TotalRounds = stateManager.TotalRounds,
                IsTieBreak = stateManager.IsTieBreak,
                Announcement = stateManager.Announcement
            };

            await _hubContext.Clients.All.SendAsync("ReceiveGameState", state, stoppingToken);
            await Task.Delay(TickRateMs, stoppingToken);
        }
    }
}