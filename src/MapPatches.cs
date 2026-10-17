using HarmonyLib;
using UnityEngine;

namespace BelmontsCurseUltrawide;

internal static class MapTextures
{
    internal static void Resize(RenderTexture? texture, bool force = false)
    {
        if (!texture) return;
        var height = Screen.height;
        var width = (height * 16 + 4) / 9;
        if (!force && texture!.width == width && texture.height == height) return;
        texture!.Release();
        texture.width = width;
        texture.height = height;
    }
}

[HarmonyPatch(typeof(UIMapLevelManager), nameof(UIMapLevelManager.WarmupRenderTexture))]
internal static class AreaMapTexture
{
    private static void Postfix(UIMapLevelManager __instance) => MapTextures.Resize(__instance._levelMapRenderTexture);
}

[HarmonyPatch(typeof(UIMapLevelManager), nameof(UIMapLevelManager.ResizeRenderTexture))]
internal static class ResizeAreaMap
{
    private static bool Prefix(UIMapLevelManager __instance)
    {
        MapTextures.Resize(__instance._levelMapRenderTexture, force: true);
        return false;
    }
}

[HarmonyPatch(typeof(UIMapLevelManager), nameof(UIMapLevelManager.ApplyRenderTexture))]
internal static class ApplyAreaMap
{
    private static bool Prefix(UIMapLevelManager __instance, LevelMap __0)
    {
        // The native method inlines warmup and resize. Keep their order and
        // finish sizing the texture before assigning it to the map camera.
        __instance.WarmupRenderTexture();
        MapTextures.Resize(__instance._levelMapRenderTexture, force: true);
        __0.mapCamera.mapCamera.targetTexture = __instance._levelMapRenderTexture;
        return false;
    }
}

[HarmonyPatch(typeof(UIMapWorldManager), nameof(UIMapWorldManager.WarmupRenderTexture))]
internal static class WorldMapTexture
{
    private static void Postfix(UIMapWorldManager __instance) => MapTextures.Resize(__instance._worldMapRenderTexture);
}

[HarmonyPatch(typeof(UIMapWorldManager), nameof(UIMapWorldManager.ResizeRenderTexture))]
internal static class ResizeWorldMap
{
    private static bool Prefix(UIMapWorldManager __instance)
    {
        MapTextures.Resize(__instance._worldMapRenderTexture, force: true);
        return false;
    }
}

[HarmonyPatch(typeof(UIMapWorldManager), nameof(UIMapWorldManager.ApplyRenderTexture))]
internal static class ApplyWorldMap
{
    private static bool Prefix(UIMapWorldManager __instance, WorldMap __0)
    {
        __instance.WarmupRenderTexture();
        MapTextures.Resize(__instance._worldMapRenderTexture, force: true);
        __0.mapCamera.mapCamera.targetTexture = __instance._worldMapRenderTexture;
        return false;
    }
}
