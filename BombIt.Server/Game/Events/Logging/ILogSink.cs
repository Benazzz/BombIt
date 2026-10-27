namespace BombIt.Server.Game.Events.Logging;

// Nusako kur ir kaip eilute uzrasoma.
public interface ILogSink
{
    void Write(string line);
}
