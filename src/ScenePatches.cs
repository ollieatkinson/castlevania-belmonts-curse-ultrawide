using HarmonyLib;
using MotherBase.EntityFramework;
using RoomManager;
using UnityEngine;

namespace BelmontsCurseUltrawide;

[HarmonyPatch(typeof(RoomManagerSystem), nameof(RoomManagerSystem.OnRoomChanged))]
[GameSignature(typeof(void), typeof(EntityContext))]
[HarmonyWrapSafe]
internal static class SaveRoomMasks
{
    internal static readonly Feature Fix = new("save-room masks");

    private static void Postfix()
    {
        if (!Fix.Enabled) return;
        try
        {
            GameObject.Find("Foreground_Saveroom_Black/00_Foreground_BlackSquare")?.SetActive(false);
            GameObject.Find("Foreground_Saveroom_Black/00_Foreground_BlackSquare (1)")?.SetActive(false);
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}

[HarmonyPatch(typeof(MenuStackLayerUIContainer), nameof(MenuStackLayerUIContainer.OnPushLayer))]
[GameSignature(typeof(void))]
[HarmonyWrapSafe]
internal static class CutsceneBars
{
    internal static readonly Feature Fix = new("cutscene bars");

    private static bool Prefix()
    {
        return !Fix.Enabled;
    }
}
