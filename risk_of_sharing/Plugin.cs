using BepInEx;
using BepInEx.Logging;

namespace risk_of_sharing;



[BepInPlugin(pluginGUID, pluginName, pluginVersion)]
[BepInProcess("Risk of Rain 2.exe")]
public class Plugin : BaseUnityPlugin
{
    const string pluginGUID = "de.mavodeli.risk_of_sharing";
    const string pluginName = "Risk of Sharing";
    const string pluginVersion = "0.0.1";

    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        Logger.LogInfo($"Plugin {pluginGUID} is loaded!");
    }
}
