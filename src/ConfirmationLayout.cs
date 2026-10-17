using HarmonyLib;
using UnityEngine;

namespace BelmontsCurseUltrawide;

[HarmonyPatch(typeof(ConfirmationPopup), nameof(ConfirmationPopup.Show))]
[GameSignature(typeof(void), typeof(ConfirmationPopup.Texts), typeof(Il2CppSystem.Action), typeof(Il2CppSystem.Action))]
[GameField(typeof(ConfirmationPopup), nameof(ConfirmationPopup.container), typeof(Doozy.Runtime.UIManager.Containers.UIContainer))]
[HarmonyWrapSafe]
internal static class ConfirmationLayout
{
    internal static readonly Feature Fix = new("confirmation dialog");

    private static void Prefix(ConfirmationPopup __instance)
    {
        if (!Fix.Enabled) return;
        try
        {
            var group = __instance.container.rectTransform.Find("PopUpGroup")?.TryCast<RectTransform>();
            if (!group) return;
            // This group is authored at (1920, 1080) from the bottom-left.
            // Anchor it to the center instead, keeping its size and children.
            group!.anchorMin = group.anchorMax = new Vector2(.5f, .5f);
            group.anchoredPosition = Vector2.zero;
        }
        catch (Exception e) { Fix.Disable(e); }
    }
}
