using TMLiOS.Core.Compatibility;

namespace TMLiOS.Core.Mods;

public sealed record ModScanResult(
    string Path,
    string FileName,
    ModKind Kind,
    long SizeBytes,
    bool Extracted,
    string? ExtractedPath,
    IReadOnlyList<CompatibilityIssue> Issues)
{
    public string SizeLabel => SizeBytes < 1024 * 1024
        ? $"{SizeBytes / 1024.0:0.0} KB"
        : $"{SizeBytes / 1024.0 / 1024.0:0.0} MB";
}
