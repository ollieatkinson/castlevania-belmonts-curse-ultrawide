# Updating after a game update

Keep the previous `GameAssembly.dll` and Binary Ninja database for comparison. Close the game and make a branch before starting.

Run the updated game once with BepInEx to regenerate `BepInEx/interop`, then build against those assemblies. Leave the ultrawide plugin disabled while checking the new build.

## Check the methods

Run Il2CppDumper with the new `GameAssembly.dll` and its matching `global-metadata.dat` from `Castlevania Belmont's Curse_Data/il2cpp_data/Metadata`. Open the DLL in Binary Ninja. Use `script.json` to find a method's address and `dump.cs` to check its parameters and fields.

Compare these methods with the previous build and our C# hooks:

| Fix | Methods |
| --- | --- |
| Resolution | `VideoSettings.OnEnable`, its two resolution tables, and the predicate in `UpdateSupportedResolutions` |
| UI | `CanvasScaler.HandleScaleWithScreenSize`, `SwitchToWorldSpaceUi.DisplayUiInWorldSpace` |
| Boundaries | Both `CinemachineConfiner2DSimplified.PostPipelineStageCallback` and `CinemachineConfiner2D.PostPipelineStageCallback`, plus `LensSettings.get_Aspect` |
| Locked fights and bosses | `CameraManagerSystem.ClampToBounds`, `CameraScreenSize.get_screenRatio` |
| Maps | `UIMapLevelManager` and `UIMapWorldManager`: `WarmupRenderTexture`, `ApplyRenderTexture`, `ResizeRenderTexture` |
| Tarot | `HuntViewManager.UpdateDisplay` and the clue-list hierarchy |
| Save-room masks | `RoomManagerSystem.OnRoomChanged` and the two mask object paths |
| Cutscenes | `MenuStackLayerUIContainer.OnPushLayer`, `VideoController.PlayVideo` and `Update` |

Pay particular attention to `ClampToBounds`: our replacement preserves its original clamp order, including when the camera is larger than the bounds. Check the confiner callback signatures before changing `ConfinerHooks.cs`; the native delegates forward `CameraState` without interpreting its layout.

The Tarot fix sets its ancestor panel to 3740 units wide. The menu camera uses the original 3840×2160 layout. The video fix fits the presenter's texture inside its parent rectangle. Check those scene references if the UI has changed.

## Build and play

Check the `GameSignature` and `GameField` expectations beside the Harmony patches and the native signatures in `ConfinerHooks.cs`. If a method or field changed, inspect the new code before updating the expectation and hook. There is no DLL hash to update; matching methods and fields can load on another game build.

Install the DLL with the SUWSF version removed. Check `BepInEx/LogOutput.log`, then try movement, a locked fight or boss, a save room, both maps, Tarot clues and a pre-rendered video. Check 16:10, 21:9 and 32:9 where possible.

Update the tested Steam build and testing notes in the README before releasing.
