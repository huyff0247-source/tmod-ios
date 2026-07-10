namespace TMLiOS.Core.Diagnostics;

public sealed class AppLog
{
    private readonly List<LogEntry> _entries = new();
    private readonly object _gate = new();

    public event Action<LogEntry>? EntryAdded;

    public IReadOnlyList<LogEntry> Entries
    {
        get
        {
            lock (_gate)
                return _entries.ToArray();
        }
    }

    public void Debug(string message) => Add(LogLevel.Debug, message);
    public void Info(string message) => Add(LogLevel.Info, message);
    public void Warning(string message) => Add(LogLevel.Warning, message);
    public void Error(string message, Exception? exception = null) => Add(LogLevel.Error, message, exception);

    public void Add(LogLevel level, string message, Exception? exception = null)
    {
        var entry = new LogEntry(DateTimeOffset.Now, level, message, exception);
        lock (_gate)
            _entries.Add(entry);
        EntryAdded?.Invoke(entry);
    }

    public string ToText()
    {
        lock (_gate)
            return string.Join(Environment.NewLine, _entries.Select(x => x.ToString()));
    }

    public void SaveTo(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, ToText());
    }
}
