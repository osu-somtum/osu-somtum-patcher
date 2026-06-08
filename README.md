<div align="center">
  <img src="https://assets.somtum.fun/frontend/images/newlogo.png" alt="somtum logo" width="220"/>
  <h1>osu!somtum patcher</h1>
  <p>A single-exe injector that patches osu! to connect to <a href="https://somtum.fun">somtum.fun</a>.</p>
</div>

---

## Features

- Single `patcher-ui.exe` — no extra files needed
- Dark borderless UI with auto-inject: detects running osu!, or launches it for you
- In-game settings panel (magic wand icon) with toggles for:
  - Relax / Autopilot patches
  - Faster transition time
  - Akatsuki performance calculator

## Usage

Download `patcher-ui.exe` from [Releases](../../releases) and run it. That's it.

- If osu! is already running on `somtum.fun`, it injects immediately and closes.
- If not, it launches osu! and injects automatically.

## Build from source

**Requirements**

- Visual Studio 2022 with .NET Framework 4.7.2 workload
- Rust with `i686-pc-windows-msvc` target (`rustup target add i686-pc-windows-msvc`)
- `nuget.exe` in `.tools/`

**Steps**

```powershell
# 1. Build the Rust FFI DLL
cargo build --release --target i686-pc-windows-msvc --manifest-path akatsuki-pp-ffi\Cargo.toml

# 2. Restore & build the runtime DLL
.\.tools\nuget.exe restore patcher\OsuPatcher.Runtime\packages.config -PackagesDirectory packages
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' patcher\OsuPatcher.Runtime.sln /p:Configuration=Release /p:Platform="Any CPU"

# 3. Copy DLLs into the UI payload
Copy-Item patcher\OsuPatcher.Runtime\bin\Release\OsuPatcher.Runtime.dll patcher-ui\OsuPatcher.UI\Payload\ -Force
Copy-Item patcher\OsuPatcher.Runtime\bin\Release\0Harmony.dll            patcher-ui\OsuPatcher.UI\Payload\ -Force
Copy-Item akatsuki-pp-ffi\target\i686-pc-windows-msvc\release\akatsuki_pp_ffi.dll patcher-ui\OsuPatcher.UI\Payload\ -Force

# 4. Restore & build the UI (produces a single patcher-ui.exe)
.\.tools\nuget.exe restore patcher-ui\OsuPatcher.UI\packages.config -PackagesDirectory patcher-ui\packages
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' patcher-ui\OsuPatcher.UI.sln /p:Configuration=Release /p:Platform="Any CPU"
```

Output: `patcher-ui\OsuPatcher.UI\bin\Release\patcher-ui.exe`

## Credits

- Patcher runtime source — [remeliah/osu-patcher](https://github.com/remeliah/osu-patcher)
