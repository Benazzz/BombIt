namespace BombIt.Client.Commands;

// Command Pattern
public interface ISettingsCommand
{
    string Description { get; }
    void Execute();
    void Undo();
}