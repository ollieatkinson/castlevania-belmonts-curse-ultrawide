# Belmont's Curse Ultrawide

A drop-in [SUWSF](https://github.com/PhantomGamers/SUWSF) configuration for Castlevania: Belmont’s Curse. Automatically uses your monitor resolution, including **3440×1440** and **5120×2160**, with correct gameplay proportions and height-based UI scaling. The HUD keeps its original screen-edge anchoring, avoiding the exposed edge of its background artwork.

Also supports **1280×800 (16:10)**, including map and inventory menus that fit within the screen.

**v1.1.0** supports Steam build **25975867**, improves locked-fight and boss camera framing, and keeps pre-rendered videos proportional. Removes the real-time cutscene bars and the two save-room side masks while preserving the surrounding artwork.

Local and world maps stay proportional, and Tarot monster-hunt clues remain visible beside the card. Camera movement limits retain their original 16:9 framing with the full ultrawide view.

**v1.1.2** waits for patches to finish before game startup continues and keeps videos proportional after preparation or display-mode changes. It also prevents duplicate copies of SUWSF from patching the same process twice.

Install the complete ZIP when upgrading, including both `SUWSF.asi` and `SUWSF.ini`, and preserve any manually configured resolution. The compatibility guard checks the game DLL's SHA-256 and every required pattern before applying patches. **v1.1.3** shows a warning if these checks fail; the game continues without the fix, with details in `SUWSF.log`.

![5120×2160 gameplay with SUWSF](https://raw.githubusercontent.com/ollieatkinson/castlevania-belmonts-curse-ultrawide/trunk/.github/assets/gameplay.png)

## Install

1. Close the game and download the [latest release ZIP](https://github.com/ollieatkinson/castlevania-belmonts-curse-ultrawide/releases/latest).
2. Extract its contents beside `Castlevania Belmont's Curse.exe` (Steam → Manage → Browse local files).
3. Launch, select **Fullscreen Windowed**, then select the **3840×2160** resolution entry once. That entry now uses your configured resolution, such as **5120×1440**, even though its label stays the same.

**Linux/Proton:** add `WINEDLLOVERRIDES="version=n,b" %command%` to Steam launch options.

`SUWSF.ini` defaults to `Resolution="auto"`. To specify a resolution, use e.g. `Resolution="3440x1440"`. Restart after changing it. Windows may require the [x64 Visual C++ redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe).

## Remove

Close the game and delete `version.dll`, `version.ini`, `SUWSF.asi`, `SUWSF.ini` and the `BelmontsCurseUltrawide` documentation folder. On Proton, also remove the launch option. Game files and saves are unchanged.

## Limitations

Other room masks remain unchanged. Menu background artwork can also have visible edges outside its original framing. This runtime version does not modify scene assets. The HUD is anchored to the display edge; menus stay centered.

Some camera encounters and pre-rendered videos still need wider testing. Please include the location and resolution when reporting a problem.

Checked on Linux/Proton with Steam build **25975867**. Earlier versions were also checked in 5120×1440, 3440×1440 and 1280×800 windows. Windows gameplay testing is pending.

## Build

The configuration template is `src/SUWSF.ini.in`. The assembly files in `src/` contain the camera, menu, map, Tarot, cutscene and save-room corrections; CMake assembles them into `build/SUWSF.ini`. `src/version.ini` preloads the game's IL2CPP module before scanning. The build uses our [SUWSF fork](https://github.com/ollieatkinson/SUWSF) for compatibility checks; CPack builds the release ZIP and checksum in `dist/`.

With CMake 3.24 or later and GNU binutils for x86-64 ELF, available on Linux:

```sh
cmake -S . -B build
cmake --build build --target package
```

The SUWSF fork builds and publishes its own Windows binaries. Mod Actions and local builds download the pinned fork release and ASI loader, with SHA-256 checksums in `CMakeLists.txt`, then package the mod on Linux. `v*` tags publish releases. No game files or Python are needed. Players only need the release ZIP.

See [Updating after a game update](doc/how-to-update.md) for the Binary Ninja workflow.

Configuration and packaging: MIT. [Third-party licenses](THIRD_PARTY_NOTICES.md). Unofficial community fix.
