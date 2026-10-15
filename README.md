# Belmont's Curse Ultrawide

A drop-in [SUWSF](https://github.com/PhantomGamers/SUWSF) configuration for Castlevania: Belmont’s Curse. Automatically uses your monitor resolution, including **3440×1440** and **5120×2160**, with correct gameplay proportions and height-based UI scaling. The HUD keeps its original screen-edge anchoring, avoiding the exposed edge of its background artwork.

Also supports **1280×800 (16:10)**, including map and inventory menus that fit within the screen.

**v1.0.2** keeps the original 16:9 camera movement boundaries while rendering the full ultrawide view, correcting invalid limits that can leave the player off screen in narrow passages. Normal gameplay has been checked; please test any climbs that previously lost sight of the player and report the location and resolution if they still fail. Existing users can replace `SUWSF.ini` and restart; keep your configured resolution if it is set manually.

![5120×2160 gameplay with SUWSF](https://raw.githubusercontent.com/ollieatkinson/castlevania-belmonts-curse-ultrawide/trunk/.github/assets/gameplay.png)

## Install

1. Close the game and download the [latest release ZIP](https://github.com/ollieatkinson/castlevania-belmonts-curse-ultrawide/releases/latest).
2. Extract its contents beside `Castlevania Belmont's Curse.exe` (Steam → Manage → Browse local files).
3. Launch and select **Fullscreen Windowed** and the highest resolution entry. The entry is still labelled **3840×2160**, but now uses your configured resolution.

**Linux/Proton:** add `WINEDLLOVERRIDES="version=n,b" %command%` to Steam launch options.

`SUWSF.ini` defaults to `Resolution="auto"`. To specify a resolution, use e.g. `Resolution="3440x1440"`. Restart after changing it. Windows may require the [x64 Visual C++ redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe).

## Remove

Close the game and delete `version.dll`, `version.ini`, `SUWSF.asi`, `SUWSF.ini` and the `BelmontsCurseUltrawide` documentation folder. On Proton, also remove the launch option. Game files and saves are unchanged.

## Limitations

Some rooms, including save rooms, retain their foreground side masks. Menu background artwork can also have visible edges outside its original framing. This runtime version does not modify scene assets. The HUD is anchored to the display edge; menus stay centered.

The camera-boundary correction was verified using the game's compiled code and early-room data. The specific climb toward the third boss reported by players has not yet been tested in-game.

Verified in-game on Linux/Proton at 5120×2160 and in 3440×1440 and 1280×800 windows, using Steam build **25766122**. Windows gameplay testing is pending.

## Build

The configuration template is `src/SUWSF.ini.in`. `src/menu-camera.s` contains the menu-camera correction and `src/camera-bounds.s` contains the camera-boundary correction; CMake assembles them into `build/SUWSF.ini`. `src/version.ini` preloads the game's IL2CPP module before scanning. `CMakeLists.txt` pins upstream downloads with SHA-256 hashes; CPack builds the release ZIP and checksum in `dist/`.

With CMake 3.24 or later and GNU binutils for x86-64 ELF, available on Linux:

```sh
cmake -S . -B build
cmake --build build --target package
```

Actions runs this on `trunk`; `v*` tags publish releases. No game files or Python are needed. Players only need the release ZIP.

Configuration and packaging: MIT. [Third-party licenses](THIRD_PARTY_NOTICES.md). Unofficial community fix.
