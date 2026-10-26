using BombIt.Server.Game.Services;
using BombIt.Shared.Commands;
using BombIt.Shared.DTOs;

namespace BombIt.Server.Game.Facade;

// Facade Pattern
public class GameFacade
{
    private readonly object _sync = new();

    private readonly GameStateManagerService _state;
    private readonly ConnectionManager _connections;
    private readonly GameStateSnapshotBuilder _snapshotBuilder;

    public GameFacade()
    {
        _state = GameStateManagerService.Instance;
        _connections = new ConnectionManager();
        _snapshotBuilder = new GameStateSnapshotBuilder(_state);
    }

    public bool Connect(string connectionId)
    {
        lock (_sync)
        {
            if (!_connections.TryRegister(connectionId))
                return false;

            _state.AddPlayer(connectionId);
            return true;
        }
    }

    public void Disconnect(string connectionId)
    {
        lock (_sync)
        {
            if (_connections.Unregister(connectionId))
                _state.RemovePlayer(connectionId);
        }
    }

    public void HandleInput(string connectionId, PlayerInputCommand input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_sync)
        {
            if (!_connections.IsRegistered(connectionId)) return;
            _state.SetInput(connectionId, input);
        }
    }

    public void StartMatch(string connectionId, int rounds, int timeSeconds, string mapChoice)
    {
        lock (_sync)
        {
            _state.StartGame(connectionId, rounds, timeSeconds, mapChoice);
        }
    }

    public MapDto GetMap()
    {
        lock (_sync)
        {
            var dto = _state.GetMap().ToDto();
            dto.Theme = _state.GetTheme().ToDto();
            return dto;
        }
    }

    public GameStateDto Update()
    {
        lock (_sync)
        {
            _state.Tick();
            return _snapshotBuilder.Build();
        }
    }
}