namespace BombIt.Server.Game.Events.Logging;

// Bridge Pattern 
// rasoma i serverio konsole
public sealed class ConsoleLogSink : ILogSink
{
    public void Write(string line) => Console.WriteLine(line);
}
