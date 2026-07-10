namespace TMLiOS.Core.Diagnostics;

public sealed record LogEntry(DateTimeOffset Timestamp, LogLevel Level, string Message, Exception? Exception = null)
{
    public override string ToString()
    {
        var line = $"[{Timestamp:HH:mm:ss}] {Level}: {Message}";
        return Exception == null ? line : line + Environment.NewLine + Exception;
    }
}
