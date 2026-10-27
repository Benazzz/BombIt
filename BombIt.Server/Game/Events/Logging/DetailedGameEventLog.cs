namespace BombIt.Server.Game.Events.Logging;

// Bridge Pattern
// loggina visus ivykius
public sealed class DetailedGameEventLog : GameEventLog
{
    public DetailedGameEventLog(ILogSink sink) : base(sink) { }

    protected override string? Describe(GameEvent gameEvent)
    {
        string text = gameEvent switch
        {
            MatchStartedEvent e => $"Match started ({e.TotalRounds} rounds)",
            RoundStartedEvent e => $"Round {e.RoundNumber} started{(e.IsTieBreak ? " (tie-break)" : "")}",
            PlayerDiedEvent e => $"{e.Player.Name} died",
            RoundEndedEvent e => e.IsDraw
                                     ? $"Round {e.RoundNumber} ended in a draw"
                                     : $"Round {e.RoundNumber} won by {e.Winner!.Name}",
            MatchEndedEvent e => $"Match won by {e.Winner?.Name ?? "nobody"}",
            _ => gameEvent.GetType().Name
        };

        return $"[EVENT] {text}";
    }
}
