using BombIt.Server.Game.Models;
using BombIt.Server.Game.Services;
using BombIt.Shared.DTOs;

namespace BombIt.Server.Game.Facade;

// Facade Pattern 
public class GameStateSnapshotBuilder
{
    private const int TicksPerSecond = 30;
    private readonly GameStateManagerService _state;

    public GameStateSnapshotBuilder(GameStateManagerService state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    public GameStateDto Build()
    {
        return new GameStateDto
        {
            Phase = _state.CurrentPhase,
            HostConnectionId = _state.HostConnectionId,
            CountdownValue = _state.CountdownTicks,
            RoundTimeLeftSeconds = _state.RoundTimeLeftTicks / TicksPerSecond,
            MapVersion = _state.MapVersion,
            Players = _state.GetAllPlayers().Select(BuildPlayer).ToList(),
            Bombs = _state.GetBombs().Select(b => b.ToDto()).ToList(),
            Explosions = _state.GetExplosions().Select(e => e.ToDto()).ToList(),
            PowerUps = _state.GetPowerUps().Select(p => p.ToDto()).ToList(),
            CurrentRound = _state.CurrentRound,
            TotalRounds = _state.TotalRounds,
            IsTieBreak = _state.IsTieBreak,
            Announcement = _state.Announcement
        };
    }

    private PlayerStateDto BuildPlayer(Player player)
    {
        var dto = player.ToDto();
        dto.Score = _state.GetScore(player.ConnectionId);
        dto.IsInMatch = _state.IsInMatch(player.ConnectionId);
        dto.IsSpectator = !_state.IsInRound(player.ConnectionId);
        return dto;
    }
}