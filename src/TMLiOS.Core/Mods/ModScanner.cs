using TMLiOS.Core.Compatibility;
using TMLiOS.Core.Diagnostics;
using TMLiOS.Core.Platform;

namespace TMLiOS.Core.Mods;

public sealed class ModScanner
{
    private readonly AppPaths _paths;
    private readonly AppLog _log;
    private readonly TmodExtractor _extractor;
    private readonly CompatibilityScanner _compatibilityScanner;

    public ModScanner(AppPaths paths, AppLog log)
    {
        _paths = paths;
        _log = log;
        _extractor = new TmodExtractor(paths, log);
        _compatibilityScanner = new CompatibilityScanner(log);
    }

    public IReadOnlyList<ModScanResult> Scan()
    {
        Directory.CreateDirectory(_paths.Mods);
        var files = Directory.EnumerateFiles(_paths.Mods, "*", SearchOption.TopDirectoryOnly)
            .Where(IsInterestingFile)
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _log.Info($"Scanning {files.Length} file(s) in Mods.");

        var results = new List<ModScanResult>();
        foreach (var file in files)
        {
            var kind = GetKind(file);
            var extracted = false;
            string? extractedPath = null;

            if (kind is ModKind.TmodPackage or ModKind.ZipPackage)
            {
                var extraction = _extractor.TryExtract(file);
                extracted = extraction.Success;
                extractedPath = extraction.OutputDirectory;
                _log.Info($"Package {Path.GetFileName(file)}: {extraction.Message}");
            }

            var issues = _compatibilityScanner.ScanFile(file).ToList();
            if (extractedPath != null)
            {
                foreach (var dll in Directory.EnumerateFiles(extractedPath, "*.dll", SearchOption.AllDirectories))
                    issues.AddRange(_compatibilityScanner.ScanFile(dll));
            }

            var info = new FileInfo(file);
            var result = new ModScanResult(
                file,
                Path.GetFileName(file),
                kind,
                info.Length,
                extracted,
                extractedPath,
                issues);
            results.Add(result);
            _log.Info($"Found {result.FileName} kind={result.Kind} size={result.SizeLabel} issues={result.Issues.Count}");
        }

        return results;
    }

    private static bool IsInterestingFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".dll" or ".tmod" or ".zip" or ".exe" or ".json";
    }

    private static ModKind GetKind(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".dll" => ModKind.ManagedDll,
            ".tmod" => ModKind.TmodPackage,
            ".zip" => ModKind.ZipPackage,
            ".exe" => ModKind.TerrariaImportCandidate,
            _ => ModKind.Unknown
        };
    }
}
