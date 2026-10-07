// Observer Pattern
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.Events;

public abstract class GameEvent { }

public sealed class MatchStartedEvent : GameEvent
{
    public int TotalRounds { get; }
    public MatchStartedEvent(int totalRounds) => TotalRounds = totalRounds;
}

public sealed class RoundStartedEvent : GameEvent
{
    public int RoundNumber { get; }
    public bool IsTieBreak { get; }
    public RoundStartedEvent(int roundNumber, bool isTieBreak)
    {
        RoundNumber = roundNumber;
        IsTieBreak = isTieBreak;
    }
}

public sealed class PlayerDiedEvent : GameEvent
{
    public Player Player { get; }
    public PlayerDiedEvent(Player player) => Player = player;
}

public sealed class RoundEndedEvent : GameEvent
{
    public int RoundNumber { get; }
    public Player? Winner { get; }          // null = lygiosios
    public bool IsDraw => Winner == null;
    public RoundEndedEvent(int roundNumber, Player? winner)
    {
        RoundNumber = roundNumber;
        Winner = winner;
    }
}

public sealed class MatchEndedEvent : GameEvent
{
    public Player? Winner { get; }
    public MatchEndedEvent(Player? winner) => Winner = winner;
}