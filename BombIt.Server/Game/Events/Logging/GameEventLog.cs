namespace BombIt.Server.Game.Events.Logging;

// Bridge Pattern
// Sprendzia kas ir kaip aprasoma, fizini rasyma deleguoja ILogSink
public abstract class GameEventLog : IGameEventObserver
{
    private readonly ILogSink _sink; // tiltas i Implementor hierarchija

    protected GameEventLog(ILogSink sink)
    {
        ArgumentNullException.ThrowIfNull(sink);
        _sink = sink;
    }

    public void Update(GameEvent gameEvent)
    {
        var text = Describe(gameEvent);
        if (text is null) return; // sis zurnalas sio ivykio nesidomi

        _sink.Write($"[{DateTime.Now:HH:mm:ss}] {text}");
    }

    // null = ivykio nelogginti
    protected abstract string? Describe(GameEvent gameEvent);
}
