# tModLoader iOS Bootstrap

Mục tiêu của repo này là tạo nền ban đầu cho **launcher/runtime tModLoader trên iOS** theo hướng chạy Terraria/tModLoader kiểu PC, có khả năng load mod PC managed DLL về sau.

Bản hiện tại là **v0.1 bootstrap**, không phải bản chơi full ngay. Nó cung cấp:

- App iOS native bằng .NET 8 (`net8.0-ios`).
- Cấu hình Mono interpreter/JIT-friendly cho runtime load.
- Entitlements cần cho môi trường jailbreak/TrollStore/SideStore JIT.
- UI launcher tối giản: import file, scan mods, load DLL, xem log.
- Loader thử nghiệm cho `.dll` managed mod.
- Scanner sơ bộ cho `.tmod`, `.dll`, dependency/native-risk strings.
- GitHub Actions build iOS với code signing qua secrets.
- Script build local trên macOS.

> Không có asset Terraria/tModLoader bản quyền trong repo. Người test phải tự import file hợp lệ của họ.

## Trạng thái hiện tại

| Mục | Trạng thái |
| --- | --- |
| App iOS mở được | Scaffold sẵn |
| JIT/interpreter config | Có |
| Import `.dll` / `.tmod` | Có UI import |
| Load managed DLL | Có loader thử nghiệm |
| Parse `.tmod` thật | Chưa hoàn chỉnh, có stub rõ ràng |
| Boot Terraria/tModLoader main loop | Chưa |
| Render/audio/input backend | Chưa |
| Chơi được game | Chưa |

## Yêu cầu build

- macOS runner hoặc máy Mac.
- Xcode có iOS SDK.
- .NET 8 SDK.
- Workload iOS:

```bash
 dotnet workload install ios
```

## Build local

```bash
chmod +x scripts/build-local-ios.sh
./scripts/build-local-ios.sh
```

Build iOS device thường cần signing. Nếu bạn dùng jailbreak/roothide, vẫn cần tạo app bundle/IPA rồi ký theo workflow phù hợp của bạn.

## GitHub Actions

Workflow nằm ở:

```text
.github/workflows/build-ios.yml
```

Để build IPA signed bằng GitHub Actions, thêm các secret sau vào repo:

```text
APPLE_CERTIFICATE_BASE64
APPLE_CERTIFICATE_PASSWORD
APPLE_PROVISION_PROFILE_BASE64
CODESIGN_KEY
CODESIGN_PROVISION
```

Nếu chưa có certificate/provisioning, workflow vẫn có thể fail ở bước signing. Đây là giới hạn bình thường của build iOS trong CI.

## Test trên iPhone 8 Plus roothide

Sau khi cài app:

1. Mở app.
2. Bấm **Run Runtime Probe** để test runtime expression/reflection emit.
3. Bấm **Import Mod/File** và chọn `.dll`, `.tmod`, hoặc file test.
4. Bấm **Scan Mods**.
5. Bấm **Load Managed DLLs**.
6. Gửi log crash/log trong app để debug tiếp.

Thư mục app dùng:

```text
Documents/TMLiOS/
  Mods/
  Cache/
  Logs/
  TerrariaImport/
```

## Roadmap

### v0.2

- Nối parser `.tmod` thật từ source tModLoader.
- Extract mod DLL/dependencies đúng format.
- Lưu metadata mod: name, version, author, tML version.
- Compatibility report tốt hơn.

### v0.3

- Add tModLoader source vào `extern/tModLoader`.
- Patch platform/path layer.
- Disable Steam/Desktop APIs.
- Boot được core init/log.

### v0.4

- Backend graphics/audio/input iOS.
- Touch + controller input.
- Boot world/menu đầu tiên.

### v0.5+

- Chạy mod PC pure C#.
- Safe mode khi mod crash.
- Low memory profile cho iPhone 8 Plus.

## Nguyên tắc

- Không hook Terraria Mobile.
- Không dùng binary mobile làm nền.
- Không đưa asset/binary game bản quyền vào repo.
- Chạy theo hướng port tModLoader/Terraria PC runtime sang iOS.
