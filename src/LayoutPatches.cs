using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BelmontsCurseUltrawide;

[HarmonyPatch(typeof(CanvasScaler), nameof(CanvasScaler.HandleScaleWithScreenSize))]
internal static class InterfaceScaling
{
    private static void Prefix(CanvasScaler __instance, out CanvasScaler.ScreenMatchMode __state)
    {
        __state = __instance.screenMatchMode;
        __instance.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
    }

    private static Exception? Finalizer(CanvasScaler __instance, CanvasScaler.ScreenMatchMode __state, Exception? __exception)
    {
        __instance.screenMatchMode = __state;
        return __exception;
    }
}

[HarmonyPatch(typeof(SwitchToWorldSpaceUi), nameof(SwitchToWorldSpaceUi.DisplayUiInWorldSpace))]
internal static class MenuCamera
{
    private static void Prefix(SwitchToWorldSpaceUi __instance)
    {
        if (__instance._isInWorldSpace && !__instance._switchingToOverlay) return;
        var camera = __instance.worldSpaceCamera;
        if (camera) camera.orthographicSize = Math.Max(1080f, 1920f / camera.aspect);
    }
}

[HarmonyPatch(typeof(HuntViewManager), nameof(HuntViewManager.UpdateDisplay))]
internal static class TarotLayout
{
    private static void Prefix(HuntViewManager __instance)
    {
        var parent = __instance.clueListContainer;
        for (var i = 0; i < 4 && parent; i++) parent = parent.parent;
        var section = parent ? parent.TryCast<RectTransform>() : null;
        if (section) section!.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3740f);
    }
}
