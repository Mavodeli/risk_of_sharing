using HarmonyLib;
using RoR2;

namespace risk_of_sharing.patches
{
    [HarmonyPatch]
    public class StageStartPatch
    {
        [HarmonyPatch(typeof(Stage), nameof(Stage.Start))]
        [HarmonyPostfix]
        static void initItemArchive()
        {
            Debug.log("Stage.Start called, initializing item archive");

            ItemArchive.initItemsToShare();
        }
    }
}