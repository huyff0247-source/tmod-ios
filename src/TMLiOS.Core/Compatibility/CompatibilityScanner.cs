using System.Text;
using TMLiOS.Core.Diagnostics;

namespace TMLiOS.Core.Compatibility;

public sealed class CompatibilityScanner
{
    private readonly AppLog _log;

    private static readonly (string Pattern, CompatibilitySeverity Severity, string Code, string Message)[] Patterns =
    {
        ("kernel32", CompatibilitySeverity.Error, "WIN32_KERNEL", "Windows kernel32 reference found"),
        ("user32", CompatibilitySeverity.Error, "WIN32_USER", "Windows user32 reference found"),
        ("gdi32", CompatibilitySeverity.Error, "WIN32_GDI", "Windows gdi32 reference found"),
        ("System.Windows.Forms", CompatibilitySeverity.Error, "WINFORMS", "Desktop WinForms reference found"),
        ("steam_api", CompatibilitySeverity.Warning, "STEAM_NATIVE", "Steam native API reference found"),
        ("DllImport", CompatibilitySeverity.Warning, "PINVOKE", "Possible native P/Invoke usage"),
        ("Microsoft.Xna.Framework.Windows", CompatibilitySeverity.Warning, "XNA_WINDOWS", "Windows-specific XNA reference found"),
        (".x86", CompatibilitySeverity.Warning, "ARCH_X86", "Possible x86-specific dependency"),
        (".x64", CompatibilitySeverity.Warning, "ARCH_X64", "Possible x64-specific dependency"),
    };

    public CompatibilityScanner(AppLog log)
    {
        _log = log;
    }

    public IEnumerable<CompatibilityIssue> ScanFile(string path)
    {
        var issues = new List<CompatibilityIssue>();
        try
        {
            var info = new FileInfo(path);
            if (!info.Exists)
                return issues;

            if (info.Length > 128 * 1024 * 1024)
            {
                issues.Add(new CompatibilityIssue(CompatibilitySeverity.Warning, "LARGE_FILE", "Large file may be heavy for iPhone 8 Plus memory"));
                return issues;
            }

            var bytes = File.ReadAllBytes(path);
            var ascii = Encoding.ASCII.GetString(bytes.Select(b => b is >= 32 and <= 126 ? b : (byte)'.').ToArray());
            foreach (var pattern in Patterns)
            {
                if (ascii.Contains(pattern.Pattern, StringComparison.OrdinalIgnoreCase))
                    issues.Add(new CompatibilityIssue(pattern.Severity, pattern.Code, pattern.Message));
            }
        }
        catch (Exception ex)
        {
            _log.Error($"Compatibility scan failed: {path}", ex);
            issues.Add(new CompatibilityIssue(CompatibilitySeverity.Warning, "SCAN_FAILED", ex.Message));
        }

        return issues;
    }
}
