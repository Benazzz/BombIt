using BombIt.Client.Settings;

namespace BombIt.Client.Commands;

// Command Pattern
public sealed class ResetSettingsCommand : ISettingsCommand
{
    private readonly MatchSettings _settings;
    private int _previousRounds;
    private int _previousSeconds;

    public ResetSettingsCommand(MatchSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
    }

    public string Description => $"Reset: {_previousRounds} x {_previousSeconds} s -> defaults";

    public void Execute()
    {
        _previousRounds = _settings.Rounds;
        _previousSeconds = _settings.RoundTimeSeconds;
        _settings.SetRounds(MatchSettings.DefaultRounds);
        _settings.SetRoundTime(MatchSettings.DefaultRoundTime);
    }

    public void Undo()
    {
        _settings.SetRounds(_previousRounds);
        _settings.SetRoundTime(_previousSeconds);
    }
}