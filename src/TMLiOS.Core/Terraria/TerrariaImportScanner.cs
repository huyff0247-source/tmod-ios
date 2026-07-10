using TMLiOS.Core.Diagnostics;
using TMLiOS.Core.Platform;

namespace TMLiOS.Core.Terraria;

public sealed class TerrariaImportScanner
{
    private readonly AppPaths _paths;
    private readonly AppLog _log;

    public TerrariaImportScanner(AppPaths paths, AppLog log)
    {
        _paths = paths;
        _log = log;
    }

    public TerrariaInstallManifest Scan()
    {
        var root = _paths.TerrariaImport;
        var exe = Directory.EnumerateFiles(root, "Terraria.exe", SearchOption.AllDirectories).FirstOrDefault();
        var content = Directory.EnumerateDirectories(root, "Content", SearchOption.AllDirectories).FirstOrDefault();
        var tml = Directory.EnumerateFiles(root, "tModLoader.dll", SearchOption.AllDirectories).FirstOrDefault();

        var manifest = new TerrariaInstallManifest(root, exe, content, tml, DateTimeOffset.Now);
        _log.Info($"Terraria import scan: exe={(exe != null)} content={(content != null)} tml={(tml != null)}");
        return manifest;
    }
}
