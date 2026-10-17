using HarmonyLib;
using UnityEngine;

namespace BelmontsCurseUltrawide;

internal static class MapTextures
{
    internal static readonly Feature Fix = new("map textures");

    internal static void Resize(RenderTexture? texture, bool force = false)
    {
        if (!texture) return;
        var height = Screen.height;
        var width = (height * 16 + 4) / 9;
        if (!force && texture!.width == width && texture.height == height) return;
        texture!.Release();
        texture.width = width;
        texture.height = height;
        Plugin.Logger.LogDebug($"Map texture {texture.name}: {width}x{height}");
    }
}

[HarmonyPatch(typeof(UIMapLevelManager), nameof(UIMapLevelManager.WarmupRenderTexture))]
[HarmonyWrapSafe]
internal static class AreaMapTexture
{
    internal static readonly Feature Fix = MapTextures.Fix;

    private static void Postfix(UIMapLevelManager __instance)
    {
        if (!Fix.Enabled) return;
        try
        {
            MapTextures.Resize(__instance._levelMapRenderTexture);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(UIMapLevelManager), nameof(UIMapLevelManager.ResizeRenderTexture))]
[HarmonyWrapSafe]
internal static class ResizeAreaMap
{
    internal static readonly Feature Fix = MapTextures.Fix;

    private static bool Prefix(UIMapLevelManager __instance)
    {
        if (!Fix.Enabled) return true;
        try
        {
            MapTextures.Resize(__instance._levelMapRenderTexture, force: true);
            return false;
        }
        catch (Exception e) { Fix.Disable(e); }
        return true;
    }
}

[HarmonyPatch(typeof(UIMapLevelManager), nameof(UIMapLevelManager.ApplyRenderTexture))]
[HarmonyWrapSafe]
internal static class ApplyAreaMap
{
    internal static readonly Feature Fix = MapTextures.Fix;

    private static bool Prefix(UIMapLevelManager __instance, LevelMap __0)
    {
        if (!Fix.Enabled) return true;
        try
        {
            // The native method inlines warmup and resize. Keep their order and
            // finish sizing the texture before assigning it to the map camera.
            __instance.WarmupRenderTexture();
            if (!Fix.Enabled) return true;
            MapTextures.Resize(__instance._levelMapRenderTexture, force: true);
            __0.mapCamera.mapCamera.targetTexture = __instance._levelMapRenderTexture;
            return false;
        }
        catch (Exception e) { Fix.Disable(e); }
        return true;
    }
}

[HarmonyPatch(typeof(UIMapWorldManager), nameof(UIMapWorldManager.WarmupRenderTexture))]
[HarmonyWrapSafe]
internal static class WorldMapTexture
{
    internal static readonly Feature Fix = MapTextures.Fix;

    private static void Postfix(UIMapWorldManager __instance)
    {
        if (!Fix.Enabled) return;
        try
        {
            MapTextures.Resize(__instance._worldMapRenderTexture);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(UIMapWorldManager), nameof(UIMapWorldManager.ResizeRenderTexture))]
[HarmonyWrapSafe]
internal static class ResizeWorldMap
{
    internal static readonly Feature Fix = MapTextures.Fix;

    private static bool Prefix(UIMapWorldManager __instance)
    {
        if (!Fix.Enabled) return true;
        try
        {
            MapTextures.Resize(__instance._worldMapRenderTexture, force: true);
            return false;
        }
        catch (Exception e) { Fix.Disable(e); }
        return true;
    }
}

[HarmonyPatch(typeof(UIMapWorldManager), nameof(UIMapWorldManager.ApplyRenderTexture))]
[HarmonyWrapSafe]
internal static class ApplyWorldMap
{
    internal static readonly Feature Fix = MapTextures.Fix;

    private static bool Prefix(UIMapWorldManager __instance, WorldMap __0)
    {
        if (!Fix.Enabled) return true;
        try
        {
            __instance.WarmupRenderTexture();
            if (!Fix.Enabled) return true;
            MapTextures.Resize(__instance._worldMapRenderTexture, force: true);
            __0.mapCamera.mapCamera.targetTexture = __instance._worldMapRenderTexture;
            return false;
        }
        catch (Exception e) { Fix.Disable(e); }
        return true;
    }
}
