using BombIt.Shared.Enums;

namespace BombIt.Shared.DTOs;

public class GameStateDto
{
    public GamePhase Phase { get; set; }
    public List<PlayerStateDto> Players { get; set; } = new();
}