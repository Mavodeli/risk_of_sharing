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
            Debug.log("ShopTerminalBehavior.DropPickup called, testing for cauldron");

            // Check for cauldrons (3d printers already set the duplicated tag)
            if (__instance.gameObject.name.ToLower().Contains("cauldron"))
            {
                Debug.log("cauldron found, setting isDuplicated variable to true");

                isDuplicated = true;
            }

            // then run the normal function with potentially altered arguments
            return true;
        }
    }
}