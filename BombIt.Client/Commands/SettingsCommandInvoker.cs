namespace BombIt.Client.Commands;

// Command Pattern
public class SettingsCommandInvoker
{
    private readonly Stack<ISettingsCommand> _history = new();

    public bool CanUndo => _history.Count > 0;
    public string? LastDescription => _history.Count > 0 ? _history.Peek().Description : null;

    public void ExecuteCommand(ISettingsCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        command.Execute();
        _history.Push(command);
    }

    public void Undo()
    {
        if (_history.Count == 0) return;
        _history.Pop().Undo();
    }

    public void ClearHistory()
    {
        _history.Clear();
    }
}