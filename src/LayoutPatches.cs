using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BelmontsCurseUltrawide;

[HarmonyPatch(typeof(CanvasScaler), nameof(CanvasScaler.HandleScaleWithScreenSize))]
[GameSignature(typeof(void))]
[HarmonyWrapSafe]
internal static class InterfaceScaling
{
    internal static readonly Feature Fix = new("UI scaling");

    private static void Prefix(CanvasScaler __instance, out CanvasScaler.ScreenMatchMode? __state)
    {
        __state = null;
        if (!Fix.Enabled) return;
        try
        {
            __state = __instance.screenMatchMode;
            __instance.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }
        catch (Exception e) { Fix.Disable(e); }
    }

    private static Exception? Finalizer(CanvasScaler __instance, CanvasScaler.ScreenMatchMode? __state, Exception? __exception)
    {
        try
        {
            if (__state.HasValue) __instance.screenMatchMode = __state.Value;
        }
        catch (Exception e) { Fix.Disable(e); }
        return __exception;
    }
}

[HarmonyPatch(typeof(SwitchToWorldSpaceUi), nameof(SwitchToWorldSpaceUi.DisplayUiInWorldSpace))]
[GameSignature(typeof(void))]
[GameField(typeof(SwitchToWorldSpaceUi), nameof(SwitchToWorldSpaceUi.worldSpaceCamera), typeof(Camera))]
[GameField(typeof(SwitchToWorldSpaceUi), nameof(SwitchToWorldSpaceUi._isInWorldSpace), typeof(bool))]
[GameField(typeof(SwitchToWorldSpaceUi), nameof(SwitchToWorldSpaceUi._switchingToOverlay), typeof(bool))]
[HarmonyWrapSafe]
internal static class MenuCamera
{
    internal static readonly Feature Fix = new("menu camera");

    private static void Prefix(SwitchToWorldSpaceUi __instance)
    {
        if (!Fix.Enabled) return;
        try
        {
            if (__instance._isInWorldSpace && !__instance._switchingToOverlay) return;
            var camera = __instance.worldSpaceCamera;
            if (camera) camera.orthographicSize = Math.Max(1080f, 1920f / camera.aspect);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(HuntViewManager), nameof(HuntViewManager.UpdateDisplay))]
[GameSignature(typeof(void), typeof(ArcanaCodexData))]
[GameField(typeof(HuntViewManager), nameof(HuntViewManager.clueListContainer), typeof(Transform))]
[HarmonyWrapSafe]
internal static class TarotLayout
{
    internal static readonly Feature Fix = new("Tarot layout");

    private static void Prefix(HuntViewManager __instance)
    {
        if (!Fix.Enabled) return;
        try
        {
            var parent = __instance.clueListContainer;
            for (var i = 0; i < 4 && parent; i++) parent = parent.parent;
            var section = parent ? parent.TryCast<RectTransform>() : null;
            if (section) section!.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3740f);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}
