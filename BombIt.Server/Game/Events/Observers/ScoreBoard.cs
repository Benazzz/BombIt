// Observer Pattern
using BombIt.Server.Game.Models;

namespace BombIt.Server.Game.Events.Observers;

public class ScoreBoard : IGameEventObserver
{
    private readonly Dictionary<string, int> _scores = new();

    public void Update(GameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case MatchStartedEvent:
                _scores.Clear();
                break;

            case RoundEndedEvent { Winner: not null } roundEnded:
                var id = roundEnded.Winner.ConnectionId;
                _scores[id] = GetScore(id) + 1;
                break;
        }
    }

    public int GetScore(string playerId) => _scores.GetValueOrDefault(playerId, 0);

    // Žaidėjai su daugiausiai taškų (jei >1 – lygybė, reikės tie-break)
    public List<Player> GetLeaders(IEnumerable<Player> players)
    {
        var list = players.ToList();
        if (list.Count == 0) return list;

        int max = list.Max(p => GetScore(p.ConnectionId));
        return list.Where(p => GetScore(p.ConnectionId) == max).ToList();
    }
}