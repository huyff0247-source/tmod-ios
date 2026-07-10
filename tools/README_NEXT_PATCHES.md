# Next patch list

When the bootstrap works on device, apply these patches in order:

1. Add `extern/tModLoader` source.
2. Create `TMLiOS.TModBridge` project.
3. Reference tModLoader projects that can compile without desktop backends.
4. Replace the `.tmod` stub with upstream `TmodFile` reader.
5. Add iOS path provider.
6. Stub/disable Steamworks.
7. Replace desktop window/input/audio classes.
8. Add crash-safe mod loading.
