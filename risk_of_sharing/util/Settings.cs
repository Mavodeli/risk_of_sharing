using System.Collections.Generic;
using BepInEx.Configuration;
using RoR2;
using RiskOfOptions;
using BepInEx;
using RiskOfOptions.Options;
using System.Linq;

namespace risk_of_sharing
{
    class Settings
    {
        public static ConfigEntry<bool> debugMode;
        public static ConfigEntry<bool> randomizeSharedItems;
        public static ConfigEntry<bool> randomizeOwnItems;
        private static Dictionary<ItemTier, ConfigEntry<bool>> sharedItemTiersConfig = new Dictionary<ItemTier, ConfigEntry<bool>>();

        public static List<ItemTier> getSharedItemTiers()
        {
            if (sharedItemTiersConfig.Count > 0)
            {
                return sharedItemTiersConfig.Where(pair => pair.Value.Value == true).Select(pair => pair.Key).ToList();
            }
            else
            {
                return new List<ItemTier>();
            }
        }

        public static void awakenSettings(BaseUnityPlugin plugin)
        {
            // [Randomization]
            randomizeSharedItems = plugin.Config.Bind<bool>("Randomization", "randomizeSharedItems", false, "Whether or not to randomize the items sent to others");
            randomizeOwnItems = plugin.Config.Bind<bool>("Randomization", "randomizeOwnItems", false, "Whether or not to randomize the items you pick up");

            // [Sharing]
            sharedItemTiersConfig.Add(ItemTier.Tier1, plugin.Config.Bind<bool>("Sharing", "shareWhiteItems", true, "Whether or not to share white items"));
            sharedItemTiersConfig.Add(ItemTier.Tier2, plugin.Config.Bind<bool>("Sharing", "shareGreenItems", true, "Whether or not to share green items"));
            sharedItemTiersConfig.Add(ItemTier.Tier3, plugin.Config.Bind<bool>("Sharing", "shareRedItems", true, "Whether or not to share red items"));
            sharedItemTiersConfig.Add(ItemTier.Boss, plugin.Config.Bind<bool>("Sharing", "shareBossItems", true, "Whether or not to share boss items (yellow)"));

            // [Debug]
            debugMode = plugin.Config.Bind<bool>("Debug", "debugMode", false, "Whether or not to print debug messages to the log");



            // In-game settings
            ModSettingsManager.AddOption(new CheckBoxOption(debugMode));
            ModSettingsManager.AddOption(new CheckBoxOption(randomizeSharedItems));
            ModSettingsManager.AddOption(new CheckBoxOption(randomizeOwnItems));
            foreach (KeyValuePair<ItemTier, ConfigEntry<bool>> pair in sharedItemTiersConfig)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(pair.Value));
            }
        }
    }
}

