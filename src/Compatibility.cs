using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BepInEx;

namespace BelmontsCurseUltrawide;

internal static class Compatibility
{
    private const string SupportedHash = "4904978BA2C15884185CFAEA8D348DFB8FFE69D466FAFED1F46FB963A7276152";

    internal static void Check()
    {
        foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            if (string.Equals(module.ModuleName, "SUWSF.asi", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Remove the SUWSF version of this mod before using the BepInEx version.");
        using var file = File.OpenRead(Path.Combine(Paths.GameRootPath, "GameAssembly.dll"));
        using var sha = SHA256.Create();
        if (Convert.ToHexString(sha.ComputeHash(file)) != SupportedHash)
            throw new InvalidOperationException("This patch is not compatible with your game version. Please update or remove the mod.");
    }

    internal static void Warn(string reason)
    {
        var message = $"{reason}\n\nThe game will continue without the widescreen fixes.\n\nMore details are in BepInEx/LogOutput.log.";
        try { MessageBoxW(IntPtr.Zero, message, "Ultrawide patch not applied", 0x30); }
        catch (Exception e) { Plugin.Logger.LogWarning($"Could not show compatibility warning: {e.Message}"); }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern int MessageBoxW(IntPtr window, string text, string caption, uint type);
}
