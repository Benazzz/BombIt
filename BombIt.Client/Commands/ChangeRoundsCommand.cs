using BombIt.Client.Settings;

namespace BombIt.Client.Commands;

// Command Pattern
public sealed class ChangeRoundsCommand : ISettingsCommand
{
    private readonly MatchSettings _settings;
    private readonly int _newRounds;
    private int _previousRounds;

    public ChangeRoundsCommand(MatchSettings settings, int newRounds)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
        _newRounds = newRounds;
    }

    public string Description => $"Rounds: {_previousRounds} -> {_newRounds}";

    public void Execute()
    {
        _previousRounds = _settings.Rounds;
        _settings.SetRounds(_newRounds);
    }

    public void Undo()
    {
        _settings.SetRounds(_previousRounds);
    }
}