using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using risk_of_sharing.patches;

namespace risk_of_sharing;



[BepInPlugin(pluginGUID, pluginName, pluginVersion)]
[BepInProcess("Risk of Rain 2.exe")]
public class Plugin : BaseUnityPlugin
{
    const string pluginGUID = "de.mavodeli.risk_of_sharing";
    const string pluginName = "Risk of Sharing";
    const string pluginVersion = "0.1.0";

    internal static new ManualLogSource Logger;
    private Harmony harmonyPatcher;

    private void Awake()
    {
        Logger = base.Logger;
        harmonyPatcher = new Harmony(pluginGUID);

        Logger.LogInfo($"Plugin {pluginGUID} is loaded!");
        Logger.LogInfo("Applying patches");

        harmonyPatcher.CreateClassProcessor(typeof(ItemDefAttemptGrantPatch)).Patch();
        harmonyPatcher.CreateClassProcessor(typeof(StageStartPatch)).Patch();
        harmonyPatcher.CreateClassProcessor(typeof(ScrapperDropTagging)).Patch();
        harmonyPatcher.CreateClassProcessor(typeof(CauldronDropTagging)).Patch();
    }
}
