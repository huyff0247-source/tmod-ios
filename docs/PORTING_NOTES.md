# Porting Notes

## Current approach

This repo starts from a native .NET iOS app and moves toward a PC-style tModLoader runtime.

Final target:

```text
iOS App
  -> Mono/.NET 8 runtime with interpreter/JIT-friendly config
  -> tModLoader core
  -> Terraria-compatible platform services
  -> iOS graphics/audio/input backend
  -> PC mod loader
```

## What must be patched in tModLoader later

- Path provider.
- Steam/Desktop-only services.
- Native window creation.
- Graphics backend.
- Audio backend.
- Keyboard/mouse input into touch/controller abstraction.
- Mod browser/download path.
- Crash-safe mod disable.

## Compatibility categories

### Good first target

- Pure C# mods.
- No native dependencies.
- No Windows-only APIs.
- No desktop UI APIs.

### Risky

- Mods using P/Invoke.
- Mods requiring x86/x64 native libraries.
- Mods using Steamworks directly.
- Mods assuming Windows path separators or fixed Terraria install paths.
