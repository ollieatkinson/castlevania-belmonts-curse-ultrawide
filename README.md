# Belmont's Curse Ultrawide

An experimental BepInEx port of the SUWSF ultrawide fix for Castlevania: Belmont's Curse. Playtesting is pending.

Uses your monitor resolution, including **3440×1440** and **5120×2160**, with proportional gameplay and UI that fits **1280×800 (16:10)** too. The HUD stays at the screen edges, while menus stay centered.

Includes the camera-boundary and locked-fight corrections, proportional area/world maps, Tarot clue layout, save-room mask removal, cutscene overlay removal and uncropped video presentation. The title-menu masks remain unchanged.

![5120×2160 gameplay with the SUWSF version](.github/assets/gameplay.png)

## Install

1. Install [BepInEx for Castlevania (6.0.788)](https://www.nexusmods.com/castlevaniabelmontscurse/mods/3) and launch once.
2. If replacing our SUWSF version, close the game and remove its `SUWSF.asi`, `SUWSF.ini`, `version.dll` and `version.ini`. Keep BepInEx and other mods.
3. Copy `BelmontsCurseUltrawide.dll` into `BepInEx/plugins/`.
4. Launch, select **Fullscreen Windowed**, then select the **3840×2160** entry once. Its label stays the same, but it now uses your configured resolution.

On Linux/Proton, BepInEx uses `WINEDLLOVERRIDES="winhttp=n,b" %command%`. Keep any other launch options you need.

`BepInEx/config/ollieatkinson.belmontscurseultrawide.cfg` defaults to `Resolution = auto`. To choose a resolution, use e.g. `Resolution = 3440x1440`. Restart after changing it. `Enabled = false` disables the fixes.

The compatibility check still requires Steam build **25975867**. If it rejects your game or a hook fails to install, the mod shows a warning and leaves the game running without the widescreen fixes. Details are in `BepInEx/LogOutput.log`.

## Remove

Close the game and delete `BepInEx/plugins/BelmontsCurseUltrawide.dll`. Its configuration file can also be deleted. Keep BepInEx if other mods use it. Game files and saves are unchanged.

## Build

Install the .NET 8 SDK and run BepInEx once to generate the game's interop assemblies. Build against that installation:

```sh
dotnet build -c Release -p:GamePath="/path/to/Castlevania Belmont's Curse"
```

The DLL is in `bin/Release/net6.0/`. To package it, use `dotnet msbuild -restore -t:Package -p:Configuration=Release -p:GamePath="/path/to/game"`. The ZIP in `dist/` contains the plugin, README and license. Game and BepInEx assemblies are references only and are not included.

The source is in `src/`. Most fixes use Harmony. The two confiners use BepInEx native detours because version 6.0.788 cannot marshal their `ref CameraState` argument safely; methods are resolved through IL2CPP metadata, with no fixed game addresses or code caves. All hooks must install successfully.

See [Updating after a game update](doc/how-to-update.md).

MIT. Unofficial community fix.
