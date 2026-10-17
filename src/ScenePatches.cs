using HarmonyLib;
using RoomManager;
using UnityEngine;

namespace BelmontsCurseUltrawide;

[HarmonyPatch(typeof(RoomManagerSystem), nameof(RoomManagerSystem.OnRoomChanged))]
internal static class SaveRoomMasks
{
    private static void Postfix()
    {
        GameObject.Find("Foreground_Saveroom_Black/00_Foreground_BlackSquare")?.SetActive(false);
        GameObject.Find("Foreground_Saveroom_Black/00_Foreground_BlackSquare (1)")?.SetActive(false);
    }
}

[HarmonyPatch(typeof(MenuStackLayerUIContainer), nameof(MenuStackLayerUIContainer.OnPushLayer))]
internal static class CutsceneBars
{
    private static bool Prefix() => false;
}
