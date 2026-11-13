using HarmonyLib;
using RoR2;
using System;

namespace risk_of_sharing.patches
{
    [HarmonyPatch]
    public class CauldronDropTagging
    {
        [HarmonyPatch(typeof(ShopTerminalBehavior), nameof(ShopTerminalBehavior.DropPickup), new Type[] { typeof(bool) })]
        [HarmonyPrefix]
        static bool PatchedDropPickup(ref bool isDuplicated, ShopTerminalBehavior __instance)
        {
            Plugin.Logger.LogDebug("PatchedDropDroplet called");
            Plugin.Logger.LogDebug("__instance: " + __instance);
            Plugin.Logger.LogDebug("__instance.gameObject: " + __instance.gameObject);
            Plugin.Logger.LogDebug("__instance.gameObject.name: " + __instance.gameObject.name);

            // Check for cauldrons (3d printers already set the duplicated tag)
            if (__instance.gameObject.name.ToLower().Contains("cauldron"))
            {
                isDuplicated = true;
            }

            // then run the normal function with potentially altered arguments
            return true;
        }
    }
}