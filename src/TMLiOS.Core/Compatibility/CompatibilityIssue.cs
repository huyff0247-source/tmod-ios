namespace TMLiOS.Core.Compatibility;

public sealed record CompatibilityIssue(CompatibilitySeverity Severity, string Code, string Message);
