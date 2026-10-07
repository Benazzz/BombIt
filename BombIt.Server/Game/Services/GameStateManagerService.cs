using System.Collections.Concurrent;
using BombIt.Shared.Commands;
using BombIt.Shared.Enums;
using BombIt.Shared.DTOs;
using BombIt.Server.Game.PowerUps.Factories;
using BombIt.Server.Game.Models;
using BombIt.Server.Game.Events.Observers;
using BombIt.Server.Game.Events;

namespace BombIt.Server.Game.Services;

public class GameStateManagerService
{
    private readonly ConcurrentDictionary<string, Player> _players = new();
    private readonly ConcurrentDictionary<string, PlayerInputCommand> _pendingInputs = new();

    // Singleton Pattern Implementation
    private static readonly GameStateManagerService _instance = new GameStateManagerService();
    public static GameStateManagerService Instance => _instance;
    private readonly GameEventPublisher _events = new(); // Observer Pattern
    private readonly ScoreBoard _scoreBoard = new(); // Observer Pattern
    private readonly GameAnnouncer _announcer = new(); // Observer Pattern

    private GameStateManagerService()
    {
        // Observer Pattern
        _events.Attach(new GameEventLogger());
        _events.Attach(new DeadPlayerCleanup());
        _events.Attach(_scoreBoard);
        _events.Attach(_announcer);
    }

    public GamePhase CurrentPhase { get; private set; } = GamePhase.Lobby;
    public string HostConnectionId { get; set; } = string.Empty;
    public int CountdownTicks { get; private set; } = 0;
    public int RoundTimeLeftTicks { get; private set; } = 0;
    public int MapVersion { get; private set; } = 0;

    public int TotalRounds { get; private set; } = 3;
    public int CurrentRound { get; private set; } = 0;
    public bool IsTieBreak { get; private set; } = false;
    public string Announcement => _announcer.CurrentMessage;
    private int _roundTimeSeconds = 220;
    private int _playersAtRoundStart = 0;
    private readonly HashSet<string> _matchPlayerIds = new();
    private readonly HashSet<string> _roundPlayerIds = new();

    private Map _map = Map.LoadDefault();
    private readonly List<Bomb> _bombs = new();
    private readonly List<Explosion> _explosions = new();
    private readonly List<PowerUp> _powerUps = new();
    private readonly PowerUpCreator[] _powerUpCreators =
    {
        new BombUpCreator(),
        new FireUpCreator(),
        new SpeedUpCreator(),
        new InvulnerabilityCreator()
    };
    private readonly Random _random = new();

    private const double MoveSpeed = 3.0;
    private const double TickInterval = 1.0 / 30.0;
    private const double PowerUpSpawnChance = 0.65;
    private const int CountdownDurationTicks = 90;  
    private const int RoundEndDisplayTicks = 90;    
    private const int MatchEndDisplayTicks = 150;

    public Player AddPlayer(string connectionId)
    {
        var player = new Player(connectionId, $"Player-{connectionId[..4]}", 0, 0);

        if (CurrentPhase != GamePhase.Lobby)
        {
            player.IsAlive = false;
        }

        _players[connectionId] = player;

        if (string.IsNullOrEmpty(HostConnectionId) || !_players.ContainsKey(HostConnectionId))
        {
            HostConnectionId = connectionId;
        }

        return player;
    }

    public void RemovePlayer(string connectionId)
    {
        _players.TryRemove(connectionId, out _);
        _pendingInputs.TryRemove(connectionId, out _);

        if (HostConnectionId == connectionId)
        {
            HostConnectionId = _players.Keys.FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(HostConnectionId))
            {
                CurrentPhase = GamePhase.Lobby;
            }
        }
    }

    public void SetInput(string connectionId, PlayerInputCommand command)
    {
        if (CurrentPhase == GamePhase.Playing)
        {
            _pendingInputs[connectionId] = command;
        }
    }

    public void StartGame(string connectionId, int rounds, int timeSeconds)
    {
        if (CurrentPhase != GamePhase.Lobby || connectionId != HostConnectionId)
            return;

        TotalRounds = Math.Clamp(rounds, 1, 10);
        _roundTimeSeconds = Math.Clamp(timeSeconds, 60, 300);
        CurrentRound = 0;
        IsTieBreak = false;

        _events.Notify(new MatchStartedEvent(TotalRounds)); // Observer Pattern
        _matchPlayerIds.Clear();
        foreach (var id in _players.Keys)
            _matchPlayerIds.Add(id);

        StartNextRound(GetMatchPlayers());
    }

    private void StartNextRound(List<Player> participants)
    {
        CurrentRound++;
        CurrentPhase = GamePhase.Countdown;
        CountdownTicks = CountdownDurationTicks;
        RoundTimeLeftTicks = _roundTimeSeconds * 30;

        _bombs.Clear();
        _explosions.Clear();
        _powerUps.Clear();
        _pendingInputs.Clear();
        _map = Map.LoadDefault();
        MapVersion++;

        AssignSpawnPoints(participants);
        _roundPlayerIds.Clear();
        foreach (var p in _players.Values.Where(p => p.IsAlive))
            _roundPlayerIds.Add(p.ConnectionId);

        _playersAtRoundStart = _roundPlayerIds.Count;

        _events.Notify(new RoundStartedEvent(CurrentRound, IsTieBreak)); // Observer Pattern  
    }

    private void AssignSpawnPoints(List<Player> participants)
    {
        var corners = new[]
        {
            (1.5, 1.5),
            (13.5, 1.5),
            (1.5, 11.5),
            (13.5, 11.5)
        };

        foreach (var player in _players.Values)
        {
            player.IsAlive = false;
            player.ResetPowerUps();
        }

        int i = 0;
        foreach (var player in participants)
        {
            if (i >= corners.Length) break;

            player.X = corners[i].Item1;
            player.Y = corners[i].Item2;
            player.IsAlive = true;
            i++;
        }
    }


    public IReadOnlyCollection<Player> GetAllPlayers() => _players.Values.ToList();
    public IReadOnlyCollection<Bomb> GetBombs() => _bombs.ToList();
    public IReadOnlyCollection<Explosion> GetExplosions() => _explosions.ToList();
    public IReadOnlyCollection<PowerUp> GetPowerUps() => _powerUps.ToList();
    public Map GetMap() => _map;
    public int GetScore(string playerId) => _scoreBoard.GetScore(playerId);
    public bool IsInMatch(string playerId) => _matchPlayerIds.Contains(playerId);
    public bool IsInRound(string playerId) => _roundPlayerIds.Contains(playerId);

    // Tik mačą pradėję ir vis dar prisijungę žaidėjai
    private List<Player> GetMatchPlayers() =>
        _players.Values.Where(p => _matchPlayerIds.Contains(p.ConnectionId)).ToList();

    public void Tick()
    {
        if (CurrentPhase == GamePhase.Countdown)
        {
            CountdownTicks--;
            if (CountdownTicks <= 0)
            {
                CurrentPhase = GamePhase.Playing;
            }
        }
        else if (CurrentPhase == GamePhase.RoundEnd)
        {
            CountdownTicks--;
            if (CountdownTicks <= 0)
            {
                AdvanceMatch();
            }
        }
        else if (CurrentPhase == GamePhase.MatchEnd)
        {
            CountdownTicks--;
            if (CountdownTicks <= 0)
            {
                CurrentPhase = GamePhase.Lobby;
            }
        }
        else if (CurrentPhase == GamePhase.Playing)
        {
            if (RoundTimeLeftTicks > 0)
            {
                RoundTimeLeftTicks--;
            }

            ProcessInvulnerabilityTimers();
            ProcessInputsAndMovement();
            ProcessPowerUpPickup();
            ProcessBombTimers();
            ProcessExplosionTimers();

            CheckRoundEnd();
        }
    }

    private void CheckRoundEnd()
    {
        var alive = _players.Values.Where(p => p.IsAlive).ToList();

        // Testuojant vienam (1 dalyvis) raundas baigiasi, kai jis žūva
        bool roundOver = _playersAtRoundStart >= 2
            ? alive.Count <= 1
            : alive.Count == 0;

        if (roundOver)
        {
            EndRound(alive.Count == 1 ? alive[0] : null);
            return;
        }

        if (RoundTimeLeftTicks <= 0)
        {
            // TODO: Sudden Death. Kol kas – lygiosios.
            EndRound(null);
        }
    }

    private void EndRound(Player? winner)
    {
        _events.Notify(new RoundEndedEvent(CurrentRound, winner)); // Observer Pattern
        CurrentPhase = GamePhase.RoundEnd;
        CountdownTicks = RoundEndDisplayTicks;
    }

    private void AdvanceMatch()
    {
        var players = GetMatchPlayers();

        if (CurrentRound < TotalRounds)
        {
            StartNextRound(players);
            return;
        }

        var leaders = _scoreBoard.GetLeaders(players);
        if (leaders.Count > 1)
        {
            IsTieBreak = true;
            StartNextRound(leaders);
            return;
        }

        _events.Notify(new MatchEndedEvent(leaders.FirstOrDefault())); // Observer Pattern
        CurrentPhase = GamePhase.MatchEnd;
        CountdownTicks = MatchEndDisplayTicks;
    }


    private void ProcessInvulnerabilityTimers()
    {
        foreach (var player in _players.Values)
        {
            if (player.InvulnerabilityTicks > 0)
            {
                player.InvulnerabilityTicks--;
            }
        }
    }

    private void ProcessInputsAndMovement()
    {
        foreach (var (connectionId, player) in _players)
        {
            if (!player.IsAlive) continue;

            var input = _pendingInputs.GetValueOrDefault(connectionId, new PlayerInputCommand());

            // 1. Place Bomb (respects MaxBombs)
            if (input.PlaceBomb)
            {
                int cellX = (int)Math.Floor(player.X);
                int cellY = (int)Math.Floor(player.Y);

                bool cellHasBomb = _bombs.Any(b => (int)Math.Floor(b.X) == cellX && (int)Math.Floor(b.Y) == cellY);
                bool underLimit = player.ActiveBombCount(_bombs) < player.MaxBombs;

                if (!cellHasBomb && underLimit)
                {
                    _bombs.Add(new Bomb(connectionId, cellX + 0.5, cellY + 0.5, 60, player.BombRadius));
                }

                input.PlaceBomb = false;
            }

            // 2. Movement (respects SpeedMultiplier)
            var distance = MoveSpeed * player.SpeedMultiplier * TickInterval;
            double newX = player.X;
            double newY = player.Y;

            switch (input.Direction)
            {
                case Direction.Up: newY -= distance; break;
                case Direction.Down: newY += distance; break;
                case Direction.Left: newX -= distance; break;
                case Direction.Right: newX += distance; break;
            }

            if (!IsCollision(newX, player.Y))
                player.X = newX;

            if (!IsCollision(player.X, newY))
                player.Y = newY;
        }
    }

    private void ProcessPowerUpPickup()
    {
        for (int i = _powerUps.Count - 1; i >= 0; i--)
        {
            var pu = _powerUps[i];
            foreach (var player in _players.Values)
            {
                if (!player.IsAlive) continue;

                // Check if player hitbox overlaps the power-up cell
                double pLeft = player.X - 0.35;
                double pRight = player.X + 0.35;
                double pTop = player.Y - 0.35;
                double pBottom = player.Y + 0.35;

                double puLeft = pu.X;
                double puRight = pu.X + 1;
                double puTop = pu.Y;
                double puBottom = pu.Y + 1;

                if (pLeft < puRight && pRight > puLeft && pTop < puBottom && pBottom > puTop)
                {
                    pu.Apply(player);
                    _powerUps.RemoveAt(i);
                    break; // This power-up is consumed
                }
            }
        }
    }

    private bool IsCollision(double x, double y)
    {
        double left = x - 0.35;
        double right = x + 0.35;
        double top = y - 0.35;
        double bottom = y + 0.35;

        int minCol = (int)Math.Floor(left);
        int maxCol = (int)Math.Floor(right);
        int minRow = (int)Math.Floor(top);
        int maxRow = (int)Math.Floor(bottom);

        for (int row = minRow; row <= maxRow; row++)
        {
            for (int col = minCol; col <= maxCol; col++)
            {
                if (row >= 0 && row < _map.Height && col >= 0 && col < _map.Width)
                {
                    if (_map.Tiles[row][col] == TileType.Indestructible || _map.Tiles[row][col] == TileType.Destructible)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ProcessBombTimers()
    {
        for (int i = _bombs.Count - 1; i >= 0; i--)
        {
            var bomb = _bombs[i];
            bomb.RemainingTicks--;

            if (bomb.RemainingTicks <= 0)
            {
                ExplodeBomb(bomb);
                _bombs.RemoveAt(i);
            }
        }
    }

    private void ExplodeBomb(Bomb bomb)
    {
        int cellX = (int)Math.Floor(bomb.X);
        int cellY = (int)Math.Floor(bomb.Y);

        CreateExplosion(cellX, cellY);

        // Destroy power-ups at bomb center
        _powerUps.RemoveAll(p => p.X == cellX && p.Y == cellY);

        int[][] dirs = new[] { new[] { 0, -1 }, new[] { 0, 1 }, new[] { -1, 0 }, new[] { 1, 0 } };
        foreach (var dir in dirs)
        {
            for (int r = 1; r <= bomb.Radius; r++)
            {
                int nx = cellX + dir[0] * r;
                int ny = cellY + dir[1] * r;

                if (nx < 0 || ny < 0 || nx >= _map.Width || ny >= _map.Height) break;

                if (_map.Tiles[ny][nx] == TileType.Indestructible) break;

                CreateExplosion(nx, ny);

                // Destroy power-ups in blast path
                _powerUps.RemoveAll(p => p.X == nx && p.Y == ny);

                // Chain reaction: detonate other bombs in blast
                var chainBomb = _bombs.FirstOrDefault(b => (int)Math.Floor(b.X) == nx && (int)Math.Floor(b.Y) == ny);
                if (chainBomb != null && chainBomb.RemainingTicks > 0)
                {
                    chainBomb.RemainingTicks = 0; // Will explode next tick
                }

                if (_map.Tiles[ny][nx] == TileType.Destructible)
                {
                    _map.Tiles[ny][nx] = TileType.Empty;
                    MapVersion++;
                    TrySpawnPowerUp(nx, ny);
                    break;
                }
            }
        }
    }

    private void TrySpawnPowerUp(int x, int y)
    {
        if (_random.NextDouble() < PowerUpSpawnChance)
        {
            // Equal probability for all 4 types
            var creator = _powerUpCreators[_random.Next(_powerUpCreators.Length)];
            _powerUps.Add(creator.CreatePowerUp(x, y));
        }
    }

    private void CreateExplosion(int cellX, int cellY)
    {
        _explosions.Add(new Explosion(cellX + 0.5, cellY + 0.5, 20)); // ~0.65s
    }

    private void ProcessExplosionTimers()
    {
        for (int i = _explosions.Count - 1; i >= 0; i--)
        {
            var exp = _explosions[i];
            exp.RemainingTicks--;

            // Check if players are touching this explosion
            double left = exp.X - 0.5;
            double right = exp.X + 0.5;
            double top = exp.Y - 0.5;
            double bottom = exp.Y + 0.5;

            foreach (var player in _players.Values)
            {
                if (player.IsAlive)
                {
                    double pLeft = player.X - 0.35;
                    double pRight = player.X + 0.35;
                    double pTop = player.Y - 0.35;
                    double pBottom = player.Y + 0.35;

                    if (pLeft < right && pRight > left && pTop < bottom && pBottom > top)
                    {
                        // Invulnerable players survive
                        if (player.InvulnerabilityTicks <= 0)
                        {
                            player.IsAlive = false;
                            _events.Notify(new PlayerDiedEvent(player)); // Observer Pattern
                        }
                    }
                }
            }

            if (exp.RemainingTicks <= 0)
            {
                _explosions.RemoveAt(i);
            }
        }
    }
}
