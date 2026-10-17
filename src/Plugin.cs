using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BelmontsCurseUltrawide;

[BepInPlugin(Id, "Belmont's Curse Ultrawide", "2.0.2-dev")]
public sealed class Plugin : BasePlugin
{
    public const string Id = "ollieatkinson.belmontscurseultrawide";
    internal const float NormalAspect = 16f / 9f;
    internal static ManualLogSource Logger = null!;
    internal static int Width;
    internal static int Height;
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
            try { confiners.Install(); }
            catch (Exception e) { confiners.Fix.Disable(e); confiners.Dispose(); }
            foreach (var type in typeof(Plugin).Assembly.GetTypes())
            {
                if (!type.IsDefined(typeof(HarmonyPatch), false)) continue;
                var fix = (Feature)AccessTools.Field(type, "Fix").GetValue(null)!;
                if (!fix.Enabled) continue;
                try { fix.Harmony.CreateClassProcessor(type).Patch(); }
                catch (Exception e)
                {
                    fix.Disable(e);
                    fix.Harmony.UnpatchSelf();
                }
            }
            NativeResolution.ApplyPresets();
            Log.LogInfo($"Ultrawide fixes loaded for {Width}x{Height}.");
        }
        catch (Exception e)
        {
            Feature.UnpatchAll();
            confiners.Dispose();
            Log.LogError(e);
            Compatibility.Warn(e.Message);
        }
    }

    public override bool Unload()
    {
        Feature.UnpatchAll();
        confiners.Dispose();
        return true;
    }
}
