using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using risk_of_sharing.patches;

namespace risk_of_sharing;



[BepInDependency("com.rune580.riskofoptions")]
[BepInPlugin(pluginGUID, pluginName, pluginVersion)]
[BepInProcess("Risk of Rain 2.exe")]
public class Plugin : BaseUnityPlugin
{
    const string pluginGUID = "de.mavodeli.risk_of_sharing";
    const string pluginName = "Risk of Sharing";
    public const string pluginVersion = "0.1.0";

    internal static new ManualLogSource Logger;
    private Harmony harmonyPatcher;

    private void Awake()
    {
        Logger = base.Logger;
        harmonyPatcher = new Harmony(pluginGUID);

        Settings.awakenSettings(this);

        Logger.LogInfo($"Plugin {pluginGUID} is loaded!");
        Debug.log("Applying patches");

        harmonyPatcher.CreateClassProcessor(typeof(ItemDefAttemptGrantPatch)).Patch();
        harmonyPatcher.CreateClassProcessor(typeof(StageStartPatch)).Patch();
        harmonyPatcher.CreateClassProcessor(typeof(ScrapperDropTagging)).Patch();
        harmonyPatcher.CreateClassProcessor(typeof(CauldronDropTagging)).Patch();
    }
}

public class Debug
{
    public static void log(object data)
    {
        if (Settings.debugMode.Value)
        {
            Plugin.Logger.LogDebug(data);
        }
    }
}