namespace TMLiOS.Core.Runtime;

public sealed record LoadedAssemblyInfo(string Path, string Name, Version? Version, int TypeCount);
