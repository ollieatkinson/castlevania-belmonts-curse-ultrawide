using HarmonyLib;
using UnityEngine;

namespace BelmontsCurseUltrawide;

[HarmonyPatch(typeof(CameraScreenSize), "get_screenRatio")]
[GameSignature(typeof(float))]
[HarmonyWrapSafe]
internal static class FixedWidthCameras
{
    internal static readonly Feature Fix = new("fixed-width cameras");

    private static void Postfix(ref float __result)
    {
        if (!Fix.Enabled) return;
        try
        {
            __result = Math.Min(__result, Plugin.NormalAspect);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(CameraManagerSystem), nameof(CameraManagerSystem.ClampToBounds))]
[GameSignature(typeof(Vector3), typeof(Vector3), typeof(float), typeof(CameraTarget))]
[GameField(typeof(CameraTarget), nameof(CameraTarget.cameraConfinerBounds), typeof(Rect))]
[HarmonyWrapSafe]
internal static class LockedFightCameras
{
    internal static readonly Feature Fix = new("locked-fight cameras");

    private static bool Prefix(Vector3 __0, float __1, CameraTarget __2, ref Vector3 __result)
    {
        if (!Fix.Enabled) return true;
        try
        {
            var camera = Camera.main;
            if (!camera || !__2) return true;
            var halfWidth = Math.Min(camera.aspect, Plugin.NormalAspect) * camera.orthographicSize * __1;
            var halfHeight = camera.orthographicSize * __1;
            var bounds = __2.cameraConfinerBounds;
            __0.x = Clamp(__0.x, bounds.x + halfWidth, bounds.x + bounds.width - halfWidth);
            __0.y = Clamp(__0.y, bounds.y + halfHeight, bounds.y + bounds.height - halfHeight);
            __result = __0;
            return false;
        }
        catch (Exception e) { Fix.Disable(e); }
        return true;
    }

    // Preserve the game's behavior when a camera footprint exceeds its bounds.
    private static float Clamp(float value, float min, float max) => min > value ? min : value <= max ? value : max;
}
