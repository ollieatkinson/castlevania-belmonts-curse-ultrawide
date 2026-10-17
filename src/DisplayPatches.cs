using System.Globalization;
using System.Runtime.InteropServices;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace BelmontsCurseUltrawide;

internal static class DisplayPatches
{
    internal static (int Width, int Height) ReadResolution(string resolution)
    {
        if (resolution.Trim().Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            var width = GetSystemMetrics(0);
            var height = GetSystemMetrics(1);
            if (width > 0 && height > 0) return (width, height);
            throw new InvalidOperationException("Could not detect the monitor resolution.");
        }
        var parts = resolution.Trim().Split('x', 'X');
        if (parts.Length != 2
            || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var w)
            || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var h)
            || w <= 0 || h <= 0)
            throw new InvalidOperationException("Resolution must be auto or WIDTHxHEIGHT, for example 3440x1440.");
        return (w, h);
    }

    internal static void ReplacePreset(Il2CppReferenceArray<Il2CppSystem.ValueTuple<int, int>> resolutions)
    {
        // Keep the original preset index and label, as the SUWSF patch does.
        resolutions[resolutions.Length - 1] = new Il2CppSystem.ValueTuple<int, int>(Plugin.Width, Plugin.Height);
    }

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern int GetSystemMetrics(int index);
}

[HarmonyPatch(typeof(VideoSettings), nameof(VideoSettings.OnEnable))]
internal static class NativeResolution
{
    [HarmonyPrefix]
    internal static void ApplyPresets()
    {
        DisplayPatches.ReplacePreset(VideoSettings._resolutions);
        DisplayPatches.ReplacePreset(VideoSettings._16By9Resolutions);
    }
}

[HarmonyPatch(typeof(VideoSettings), nameof(VideoSettings.UpdateSupportedResolutions))]
internal static class SupportedModes
{
    private static void Prefix(VideoSettings __instance, out bool __state)
    {
        __state = __instance.supportOnly16By9;
        __instance.supportOnly16By9 = false;
    }

    private static Exception? Finalizer(VideoSettings __instance, bool __state, Exception? __exception)
    {
        __instance.supportOnly16By9 = __state;
        return __exception;
    }
}
