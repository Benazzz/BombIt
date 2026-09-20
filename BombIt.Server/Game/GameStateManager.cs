using System.Collections.Concurrent;
using BombIt.Shared.Commands;
using BombIt.Shared.Enums;

namespace BombIt.Server.Game;

public class GameStateManager
{
    private readonly ConcurrentDictionary<string, Player> _players = new();
    private readonly ConcurrentDictionary<string, Direction> _pendingInputs = new();

    private const double MoveSpeed = 3.0; // langeliai per sekundę
    private const double TickInterval = 1.0 / 30.0; // 30 Hz

    public Player AddPlayer(string connectionId)
    {
        var player = new Player(connectionId, $"Player-{connectionId[..4]}", startX: 1.5, startY: 1.5);
        _players[connectionId] = player;
        return player;
    }

    public void RemovePlayer(string connectionId)
    {
        _players.TryRemove(connectionId, out _);
        _pendingInputs.TryRemove(connectionId, out _);
    }

    public void SetInput(string connectionId, Direction direction)
    {
        _pendingInputs[connectionId] = direction;
    }

    public IReadOnlyCollection<Player> GetAllPlayers()
    {
        return _players.Values.ToList();
    }

    public void Tick()
    {
        foreach (var (connectionId, player) in _players)
        {
            if (!player.IsAlive) continue;

            var direction = _pendingInputs.GetValueOrDefault(connectionId, Direction.None);
            var distance = MoveSpeed * TickInterval;

            switch (direction)
            {
                case Direction.Up: player.Y -= distance; break;
                case Direction.Down: player.Y += distance; break;
                case Direction.Left: player.X -= distance; break;
                case Direction.Right: player.X += distance; break;
            }
        }
    }
}