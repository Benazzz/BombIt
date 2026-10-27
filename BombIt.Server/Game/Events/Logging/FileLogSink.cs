namespace BombIt.Server.Game.Events.Logging;

// Bridge Pattern
public sealed class FileLogSink : ILogSink
{
    private readonly string _path;
    private readonly object _lock = new();

    public FileLogSink(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        _path = path;
    }

    public void Write(string line)
    {
        lock (_lock)
        {
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                File.AppendAllText(_path, line + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Zurnalo klaida niekada neturi numusti zaidimo ciklo (Tick vykdomas GameLoopService viduje)
                Console.Error.WriteLine($"[LOG] Cannot write to '{_path}': {ex.Message}");
            }
        }
    }
}
