using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BelmontsCurseUltrawide;

[BepInPlugin(Id, "Belmont's Curse Ultrawide", "2.0.0")]
public sealed class Plugin : BasePlugin
{
    public const string Id = "ollieatkinson.belmontscurseultrawide";
    internal const float NormalAspect = 16f / 9f;
    internal static ManualLogSource Logger = null!;
    internal static int Width;
    internal static int Height;
    private readonly Harmony harmony = new(Id);
    private readonly ConfinerHooks confiners = new();

    public override void Load()
    {
        Logger = Log;
        if (!Config.Bind("UserSettings", "Enabled", true, "Enable the ultrawide fixes. Restart after changing.").Value)
            return;
        try
        {
            Compatibility.Check();
            (Width, Height) = DisplayPatches.ReadResolution(Config.Bind("UserSettings", "Resolution", "auto",
                "Monitor resolution, or WIDTHxHEIGHT (for example 3440x1440). Restart after changing.").Value);
            confiners.Install();
            harmony.PatchAll(typeof(Plugin).Assembly);
            NativeResolution.ApplyPresets();
            Log.LogInfo($"Ultrawide fixes loaded for {Width}x{Height}.");
        }
        catch (Exception e)
        {
            harmony.UnpatchSelf();
            confiners.Dispose();
            Log.LogError(e);
            Compatibility.Warn(e.Message);
        }
    }

    public override bool Unload()
    {
        harmony.UnpatchSelf();
        confiners.Dispose();
        return true;
    }
}
