using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BelmontsCurseUltrawide;

[BepInPlugin(Id, "Belmont's Curse Ultrawide", "2.0.3")]
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
            Compatibility.CheckLoaders();
            (Width, Height) = DisplayPatches.ReadResolution(Config.Bind("UserSettings", "Resolution", "auto",
                "Monitor resolution, or WIDTHxHEIGHT (for example 3440x1440). Restart after changing.").Value);
            var patches = typeof(Plugin).Assembly.GetTypes().Where(type => type.IsDefined(typeof(HarmonyPatch), false)).ToArray();
            // Validate every member of a feature before installing any of it.
            foreach (var type in patches)
            {
                var fix = (Feature)AccessTools.Field(type, "Fix").GetValue(null)!;
                if (!fix.Enabled) continue;
                try { Compatibility.CheckPatch(type); }
                catch (Exception e) { fix.Disable(e); }
            }
            try { confiners.Install(); }
            catch (Exception e) { confiners.Fix.Disable(e); confiners.Dispose(); }
            foreach (var type in patches)
            {
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
            var enabled = Feature.All.Count(fix => fix.Enabled);
            Log.LogInfo($"Ultrawide fixes loaded for {Width}x{Height}; {enabled}/{Feature.All.Count} features enabled after compatibility checks.");
            if (enabled == 0) Compatibility.Warn("No compatible widescreen fixes were found. Please update the mod.");
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
