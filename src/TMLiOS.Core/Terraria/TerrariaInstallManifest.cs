namespace TMLiOS.Core.Terraria;

public sealed record TerrariaInstallManifest(
    string RootPath,
    string? TerrariaExePath,
    string? ContentPath,
    string? TModLoaderPath,
    DateTimeOffset CreatedAt)
{
    public bool HasMinimumFiles => Directory.Exists(ContentPath ?? string.Empty);
}
