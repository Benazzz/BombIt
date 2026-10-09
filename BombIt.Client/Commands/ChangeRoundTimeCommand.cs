using BombIt.Client.Settings;

namespace BombIt.Client.Commands;

// Command Pattern
public sealed class ChangeRoundTimeCommand : ISettingsCommand
{
    private readonly MatchSettings _settings;
    private readonly int _newSeconds;
    private int _previousSeconds;

    public ChangeRoundTimeCommand(MatchSettings settings, int newSeconds)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
        _newSeconds = newSeconds;
    }

    public string Description => $"Round time: {_previousSeconds} s -> {_newSeconds} s";

    public void Execute()
    {
        _previousSeconds = _settings.RoundTimeSeconds;
        _settings.SetRoundTime(_newSeconds);
    }

    public void Undo()
    {
        _settings.SetRoundTime(_previousSeconds);
    }
}