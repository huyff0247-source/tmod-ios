using System.Reflection;
using TMLiOS.Core.Diagnostics;
using TMLiOS.Core.Platform;

namespace TMLiOS.Core.Runtime;

public sealed class AssemblyModLoader
{
    private readonly AppPaths _paths;
    private readonly AppLog _log;

    public AssemblyModLoader(AppPaths paths, AppLog log)
    {
        _paths = paths;
        _log = log;
    }

    public IReadOnlyList<LoadedAssemblyInfo> LoadManagedDlls()
    {
        var output = new List<LoadedAssemblyInfo>();
        var dlls = Directory.EnumerateFiles(_paths.Mods, "*.dll", SearchOption.AllDirectories)
            .Concat(Directory.EnumerateFiles(_paths.ExtractedMods, "*.dll", SearchOption.AllDirectories))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _log.Info($"Found {dlls.Length} managed DLL candidate(s).");

        AppDomain.CurrentDomain.AssemblyResolve -= ResolveFromKnownFolders;
        AppDomain.CurrentDomain.AssemblyResolve += ResolveFromKnownFolders;

        foreach (var dll in dlls)
        {
            try
            {
                var bytes = File.ReadAllBytes(dll);
                var assembly = Assembly.Load(bytes);
                var name = assembly.GetName();
                var typeCount = SafeGetTypes(assembly).Length;
                var info = new LoadedAssemblyInfo(dll, name.Name ?? Path.GetFileNameWithoutExtension(dll), name.Version, typeCount);
                output.Add(info);
                _log.Info($"Loaded DLL: {info.Name} {info.Version} types={info.TypeCount}");
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to load DLL: {dll}", ex);
            }
        }

        return output;
    }

    private Assembly? ResolveFromKnownFolders(object? sender, ResolveEventArgs args)
    {
        try
        {
            var requested = new AssemblyName(args.Name).Name;
            if (string.IsNullOrWhiteSpace(requested))
                return null;

            var fileName = requested + ".dll";
            var candidates = Directory.EnumerateFiles(_paths.Mods, fileName, SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(_paths.ExtractedMods, fileName, SearchOption.AllDirectories));

            var candidate = candidates.FirstOrDefault();
            if (candidate == null)
                return null;

            _log.Info($"Resolving dependency {requested} from {candidate}");
            return Assembly.Load(File.ReadAllBytes(candidate));
        }
        catch (Exception ex)
        {
            _log.Error("AssemblyResolve failed", ex);
            return null;
        }
    }

    private static Type[] SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null).Cast<Type>().ToArray();
        }
    }
}
