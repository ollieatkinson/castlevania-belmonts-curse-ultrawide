# Belmont's Curse Ultrawide

A drop-in [SUWSF](https://github.com/PhantomGamers/SUWSF) configuration for Castlevania: Belmont’s Curse. Automatically uses your monitor resolution, including **3440×1440** and **5120×2160**, with correct gameplay proportions and height-based UI scaling. The HUD keeps its original screen-edge anchoring, avoiding the exposed edge of its background artwork.

![5120×2160 gameplay with SUWSF](https://raw.githubusercontent.com/ollieatkinson/castlevania-belmonts-curse-ultrawide/trunk/.github/assets/gameplay.png)

## Install

1. Close the game and download the ZIP from [Releases](https://github.com/ollieatkinson/castlevania-belmonts-curse-ultrawide/releases/latest).
2. Extract its contents beside `Castlevania Belmont's Curse.exe` (Steam → Manage → Browse local files).
3. Launch and select **Fullscreen Windowed** and the highest resolution entry. The entry is still labelled **3840×2160**, but now uses your configured resolution.

**Linux/Proton:** add `WINEDLLOVERRIDES="version=n,b" %command%` to Steam launch options.

`SUWSF.ini` defaults to `Resolution="auto"`. To specify a resolution, use e.g. `Resolution="3440x1440"`. Restart after changing it. Windows may require the [x64 Visual C++ redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe).

## Remove

Close the game and delete `version.dll`, `version.ini`, `SUWSF.asi`, `SUWSF.ini` and the `BelmontsCurseUltrawide` documentation folder. On Proton, also remove the launch option. Game files and saves are unchanged.

## Limitations

Some rooms, including save rooms, retain their foreground side masks. Menu background artwork can also have visible edges outside its original framing. This runtime version does not modify scene assets. The HUD is anchored to the display edge; menus stay centered.

Verified in-game on Linux/Proton at 5120×2160 and in a 3440×1440 window, using Steam build **25766122**. Windows gameplay testing is pending.

## Build

The patch source is `src/SUWSF.ini`; `src/version.ini` preloads the game's IL2CPP module before scanning. Run `python3 src/package.py` to package the pinned upstream binaries and configuration into `dist/`. Python is only needed to build the ZIP. Actions does this on `trunk`; `v*` tags publish releases. No game files are needed to build.

Configuration and packaging: MIT. [Third-party licenses](THIRD_PARTY_NOTICES.md). Unofficial community fix.
