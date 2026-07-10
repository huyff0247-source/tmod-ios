# Create a test DLL

Use this on a desktop machine with .NET 8 installed:

```bash
mkdir TestMod
cd TestMod
dotnet new classlib -n TestMod
cat > TestMod/Class1.cs <<'CS'
namespace TestMod;

public sealed class Entry
{
    public static string Hello() => "Hello from imported DLL";
}
CS
dotnet build TestMod/TestMod.csproj -c Release
```

Import this file into the iOS app:

```text
TestMod/bin/Release/net8.0/TestMod.dll
```
