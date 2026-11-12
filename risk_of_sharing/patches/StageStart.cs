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
            ItemArchive.initItemsToShare();
        }
    }
}