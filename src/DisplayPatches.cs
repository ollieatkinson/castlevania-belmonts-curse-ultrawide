using System.Globalization;
using System.Runtime.InteropServices;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace BelmontsCurseUltrawide;

internal static class DisplayPatches
{
    internal static readonly Feature Fix = new("resolution");

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
[HarmonyWrapSafe]
internal static class NativeResolution
{
    internal static readonly Feature Fix = DisplayPatches.Fix;

    [HarmonyPrefix]
    internal static void ApplyPresets()
    {
        if (!Fix.Enabled) return;
        try
        {
            DisplayPatches.ReplacePreset(VideoSettings._resolutions);
            DisplayPatches.ReplacePreset(VideoSettings._16By9Resolutions);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(VideoSettings), nameof(VideoSettings.UpdateSupportedResolutions))]
[HarmonyWrapSafe]
internal static class SupportedModes
{
    internal static readonly Feature Fix = DisplayPatches.Fix;

    private static void Prefix(VideoSettings __instance, out bool? __state)
    {
        __state = null;
        if (!Fix.Enabled) return;
        try
        {
            __state = __instance.supportOnly16By9;
            __instance.supportOnly16By9 = false;
        }
        catch (Exception e) { Fix.Disable(e); }
    }

    private static Exception? Finalizer(VideoSettings __instance, bool? __state, Exception? __exception)
    {
        try
        {
            if (__state.HasValue) __instance.supportOnly16By9 = __state.Value;
        }
        catch (Exception e) { Fix.Disable(e); }
        return __exception;
    }
}
