using HarmonyLib;
using UnityEngine;

namespace BelmontsCurseUltrawide;

internal static class VideoFit
{
    internal static readonly Feature Fix = new("video fitting");

    internal static void Apply(VideoController controller)
    {
        if (!Fix.Enabled) return;
        try
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
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(VideoController), nameof(VideoController.PlayVideo))]
[HarmonyWrapSafe]
internal static class InitialVideoFit
{
    internal static readonly Feature Fix = VideoFit.Fix;

    private static void Postfix(VideoController __instance)
    {
        if (!Fix.Enabled) return;
        try
        {
            VideoFit.Apply(__instance);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(VideoController), nameof(VideoController.Update))]
[HarmonyWrapSafe]
internal static class ResizeVideo
{
    internal static readonly Feature Fix = VideoFit.Fix;

    private static void Prefix(VideoController __instance)
    {
        if (!Fix.Enabled) return;
        try
        {
            if (__instance._state is VideoController.State.Playing or VideoController.State.Paused)
                VideoFit.Apply(__instance);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}
