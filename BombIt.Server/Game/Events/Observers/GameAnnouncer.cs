// Observer Pattern
namespace BombIt.Server.Game.Events.Observers;

public class GameAnnouncer : IGameEventObserver
{
    public string CurrentMessage { get; private set; } = string.Empty;

    public void Update(GameEvent gameEvent)
    {
        CurrentMessage = gameEvent switch
        {
            MatchStartedEvent e => $"Match starting! Rounds: {e.TotalRounds}",
            RoundStartedEvent { IsTieBreak: true } => "TIE-BREAK ROUND!",
            RoundStartedEvent e => $"Round {e.RoundNumber}",
            RoundEndedEvent { IsDraw: true } e => $"Round {e.RoundNumber}: draw!",
            RoundEndedEvent e => $"{e.Winner!.Name} won round {e.RoundNumber}!",
            MatchEndedEvent { Winner: not null } e => $"{e.Winner.Name} won the match!",
            MatchEndedEvent => "Match over",
            _ => CurrentMessage // kitų įvykių neliečiam
        };
    }
}