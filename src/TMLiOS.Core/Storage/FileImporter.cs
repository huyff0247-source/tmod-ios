using TMLiOS.Core.Diagnostics;
using TMLiOS.Core.Platform;

namespace TMLiOS.Core.Storage;

public sealed class FileImporter
{
    private readonly AppPaths _paths;
    private readonly AppLog _log;

    public FileImporter(AppPaths paths, AppLog log)
    {
        _paths = paths;
        _log = log;
    }

    public string ImportToMods(string sourcePath)
    {
        Directory.CreateDirectory(_paths.Mods);
        var fileName = Path.GetFileName(sourcePath);
        var destination = GetNonConflictingPath(Path.Combine(_paths.Mods, fileName));
        File.Copy(sourcePath, destination, overwrite: false);
        _log.Info($"Imported to Mods: {destination}");
        return destination;
    }

    private static string GetNonConflictingPath(string path)
    {
        if (!File.Exists(path))
            return path;

        var dir = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);
        for (var i = 1; i < 1000; i++)
        {
            var candidate = Path.Combine(dir, $"{name}-{i}{ext}");
            if (!File.Exists(candidate))
                return candidate;
        }

        throw new IOException("Could not find non-conflicting file name");
    }
}
