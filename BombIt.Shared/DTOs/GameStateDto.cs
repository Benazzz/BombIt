using BombIt.Shared.Enums;

namespace BombIt.Shared.DTOs;

public class GameStateDto
{
    public GamePhase Phase { get; set; }
    public List<PlayerStateDto> Players { get; set; } = new();
    public string HostConnectionId { get; set; } = string.Empty;
    public int CountdownValue { get; set; }
    public int RoundTimeLeftSeconds { get; set; }
    public int MapVersion { get; set; }
    public int CurrentRound { get; set; }
    public int TotalRounds { get; set; }
    public bool IsTieBreak { get; set; }
    public string Announcement { get; set; } = string.Empty;
    public List<BombStateDto> Bombs { get; set; } = new();
    public List<ExplosionStateDto> Explosions { get; set; } = new();
    public List<PowerUpStateDto> PowerUps { get; set; } = new();
}