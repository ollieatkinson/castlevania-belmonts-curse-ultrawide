using HarmonyLib;
using UnityEngine;

namespace BelmontsCurseUltrawide;

internal static class VideoFit
{
    internal static void Apply(VideoController controller)
    {
        var image = controller._videoPresenter;
        if (image is null || !image) return;
        var texture = image.texture;
        var rect = image.transform.TryCast<RectTransform>();
        var parent = rect ? rect!.parent?.TryCast<RectTransform>() : null;
        if (texture is null || !texture || rect is null || !rect || parent is null || !parent
            || texture.width <= 0 || texture.height <= 0) return;
        var size = parent.rect;
        if (size.width <= 0 || size.height <= 0) return;
        var aspect = (float)texture.width / texture.height;
        var width = Math.Min(size.width, size.height * aspect);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width / aspect);
    }
}

[HarmonyPatch(typeof(VideoController), nameof(VideoController.PlayVideo))]
internal static class InitialVideoFit
{
    private static void Postfix(VideoController __instance) => VideoFit.Apply(__instance);
}

[HarmonyPatch(typeof(VideoController), nameof(VideoController.Update))]
internal static class ResizeVideo
{
    private static void Prefix(VideoController __instance)
    {
        if (__instance._state is VideoController.State.Playing or VideoController.State.Paused)
            VideoFit.Apply(__instance);
    }
}
