namespace BombIt.Server.Game.Facade;

// Facade Pattern 
public class ConnectionManager
{
    public const int MaxPlayers = 4;

    private readonly HashSet<string> _connections = new();

    public int Count => _connections.Count;
    public bool IsFull => _connections.Count >= MaxPlayers;

    public bool IsRegistered(string connectionId) => _connections.Contains(connectionId);

    public bool TryRegister(string connectionId)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionId);
        if (IsFull) return false;
        return _connections.Add(connectionId);
    }

    public bool Unregister(string connectionId)
    {
        return _connections.Remove(connectionId);
    }
}