// Observer Pattern
namespace BombIt.Server.Game.Events.Observers;

public class GameEventLogger : IGameEventObserver
{
    public void Update(GameEvent gameEvent)
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

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [EVENT] {text}");
    }
}