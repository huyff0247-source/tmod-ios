using TMLiOS.Core.Diagnostics;

namespace TMLiOS.Core.Platform;

public sealed class AppPaths
{
    public AppPaths(string documentsRoot, AppLog log)
    {
        DocumentsRoot = documentsRoot;
        Root = Path.Combine(documentsRoot, "TMLiOS");
        Mods = Path.Combine(Root, "Mods");
        Cache = Path.Combine(Root, "Cache");
        Logs = Path.Combine(Root, "Logs");
        TerrariaImport = Path.Combine(Root, "TerrariaImport");
        ExtractedMods = Path.Combine(Cache, "ExtractedMods");

        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(Mods);
        Directory.CreateDirectory(Cache);
        Directory.CreateDirectory(Logs);
        Directory.CreateDirectory(TerrariaImport);
        Directory.CreateDirectory(ExtractedMods);

        log.Info($"DocumentsRoot: {DocumentsRoot}");
        log.Info($"TMLiOS Root: {Root}");
        log.Info($"Mods: {Mods}");
    }

    public string DocumentsRoot { get; }
    public string Root { get; }
    public string Mods { get; }
    public string Cache { get; }
    public string Logs { get; }
    public string TerrariaImport { get; }
    public string ExtractedMods { get; }

    public string NewLogFilePath()
    {
        var name = "tmlios-" + DateTimeOffset.Now.ToString("yyyyMMdd-HHmmss") + ".log";
        return Path.Combine(Logs, name);
    }
}
