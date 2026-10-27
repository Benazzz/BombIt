namespace BombIt.Server.Game.Events.Logging;

//loggina tik raundu ir maco rezultatus
public sealed class HighlightsGameEventLog : GameEventLog
{
    public HighlightsGameEventLog(ILogSink sink) : base(sink) { }

    protected override string? Describe(GameEvent gameEvent) => gameEvent switch
    {
        RoundEndedEvent { IsDraw: true } e => $"[RESULT] Round {e.RoundNumber}: draw",
        RoundEndedEvent e => $"[RESULT] Round {e.RoundNumber}: {e.Winner!.Name} won",
        MatchEndedEvent e => $"[RESULT] Match winner: {e.Winner?.Name ?? "nobody"}",
        _ => null
    };
}
