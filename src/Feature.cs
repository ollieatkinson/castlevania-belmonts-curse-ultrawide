using HarmonyLib;

namespace BelmontsCurseUltrawide;

internal sealed class Feature
{
    internal static readonly List<Feature> All = new();
    internal readonly Harmony Harmony;
    internal bool Enabled { get; private set; } = true;
    private readonly string name;

    internal Feature(string name)
    {
        this.name = name;
        Harmony = new Harmony($"{Plugin.Id}.{name}");
        All.Add(this);
    }

    internal void Disable(Exception error)
    {
        if (!Enabled) return;
        Enabled = false;
        try { Plugin.Logger.LogError($"{name} disabled for this session; using the game's original behavior.\n{error}"); }
        catch { /* Logging must not let an exception escape a native callback. */ }
    }

    internal static void UnpatchAll()
    {
        foreach (var feature in All)
        {
            try { feature.Harmony.UnpatchSelf(); }
            catch (Exception e) { feature.Disable(e); }
        }
    }
}
