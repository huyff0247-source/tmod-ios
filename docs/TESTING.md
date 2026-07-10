# Testing Notes

## Test 1: app boot

Expected:

```text
TMLiOS launcher booted
Documents path printed
Mods path printed
```

## Test 2: runtime probe

Press **Run Runtime Probe**.

Expected:

- Expression compile test should pass or log the exact exception.
- Reflection.Emit test may pass or fail depending on runtime/JIT state. Failure here does not block DLL load testing.

## Test 3: DLL import/load

1. Build a tiny managed DLL on PC/Mac.
2. Import it with **Import Mod/File**.
3. Press **Scan Mods**.
4. Press **Load Managed DLLs**.

Example mod-test class library:

```csharp
namespace TestMod;

public sealed class Entry
{
    public static string Hello() => "Hello from imported DLL";
}
```

## Test 4: `.tmod`

The current parser only tries ZIP-compatible packages and marks real binary `.tmod` as needing upstream parser work. Next milestone is to plug in tModLoader's real `TmodFile` reader.
