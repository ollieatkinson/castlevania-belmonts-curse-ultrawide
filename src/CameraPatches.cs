using HarmonyLib;
using UnityEngine;

namespace BelmontsCurseUltrawide;

[HarmonyPatch(typeof(CameraScreenSize), "get_screenRatio")]
internal static class FixedWidthCameras
{
    private static void Postfix(ref float __result) => __result = Math.Min(__result, Plugin.NormalAspect);
}

[HarmonyPatch(typeof(CameraManagerSystem), nameof(CameraManagerSystem.ClampToBounds))]
internal static class LockedFightCameras
{
    private static bool Prefix(Vector3 __0, float __1, CameraTarget __2, ref Vector3 __result)
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

    // Preserve the game's behavior when a camera footprint exceeds its bounds.
    private static float Clamp(float value, float min, float max) => min > value ? min : value <= max ? value : max;
}
