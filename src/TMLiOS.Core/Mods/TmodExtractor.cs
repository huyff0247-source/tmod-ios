using System.IO.Compression;
using TMLiOS.Core.Diagnostics;
using TMLiOS.Core.Platform;

namespace TMLiOS.Core.Mods;

public sealed class TmodExtractor
{
    private readonly AppPaths _paths;
    private readonly AppLog _log;

    public TmodExtractor(AppPaths paths, AppLog log)
    {
        _paths = paths;
        _log = log;
    }

    public TmodExtractionResult TryExtract(string packagePath)
    {
        var name = Path.GetFileNameWithoutExtension(packagePath);
        var safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        var outDir = Path.Combine(_paths.ExtractedMods, safeName);

        try
        {
            if (Directory.Exists(outDir))
                Directory.Delete(outDir, true);
            Directory.CreateDirectory(outDir);

            using var stream = File.OpenRead(packagePath);
            if (LooksLikeZip(stream))
            {
                stream.Position = 0;
                using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
                archive.ExtractToDirectory(outDir, overwriteFiles: true);
                _log.Info($"Extracted ZIP-compatible package: {packagePath}");
                return new TmodExtractionResult(true, outDir, "ZIP-compatible package extracted");
            }

            stream.Position = 0;
            if (LooksLikeTmod(stream))
            {
                _log.Warning("Real .tmod binary format detected. Upstream tModLoader TmodFile parser must be plugged in next.");
                return new TmodExtractionResult(false, null, "Real .tmod detected; parser not wired yet");
            }

            return new TmodExtractionResult(false, null, "Unknown package format");
        }
        catch (Exception ex)
        {
            _log.Error($"Failed to extract package {packagePath}", ex);
            return new TmodExtractionResult(false, null, ex.GetType().Name + ": " + ex.Message);
        }
    }

    private static bool LooksLikeZip(Stream stream)
    {
        Span<byte> magic = stackalloc byte[4];
        if (stream.Read(magic) != magic.Length)
            return false;
        return magic[0] == 0x50 && magic[1] == 0x4B;
    }

    private static bool LooksLikeTmod(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[16];
        var read = stream.Read(buffer);
        var text = System.Text.Encoding.ASCII.GetString(buffer[..read]);
        return text.Contains("TMOD", StringComparison.OrdinalIgnoreCase) || text.Contains("tMod", StringComparison.OrdinalIgnoreCase);
    }
}
