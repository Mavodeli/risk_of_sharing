using System.Collections.Generic;
using BepInEx.Configuration;
using RoR2;
using RiskOfOptions;
using BepInEx;
using RiskOfOptions.Options;
using System.Linq;
using System;

namespace risk_of_sharing
{
    class Settings
    {
        private static BaseUnityPlugin plugin;
        private enum SettingsTab { Debug, Randomization, Sharing }
        private static Dictionary<SettingsTab, string> SettingsTabs = new Dictionary<SettingsTab, string>
        {
            {SettingsTab.Debug, "Debug"},
            {SettingsTab.Randomization, "Randomization"},
            {SettingsTab.Sharing, "Sharing"}
        };

        // Note: ConfigEntry variables must be marked as nullable so the config file doesn't auto generate them with their variable name
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static ConfigEntry<bool>? debugMode;
        public static ConfigEntry<string>? pluginVersion;
        public static ConfigEntry<bool>? randomizeToSameTier;
        public static ConfigEntry<bool>? randomizeSharedItems;
        public static ConfigEntry<bool>? randomizeItems;
        public static ConfigEntry<bool>? shareLunarCoins;
        private static Dictionary<ItemTier, ConfigEntry<bool>>? sharedItemTiersConfig;
        public static ConfigEntry<string>? itemWhitelist;
        public static ConfigEntry<string>? itemBlacklist;
        public static ConfigEntry<string>? baseItemBlacklist;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

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
            Settings.plugin = plugin;
            // [Randomization]
            randomizeItems = plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Randomization], "Randomize items", false, "Whether or not to randomize the shareable items you pick up\n\nUpdates immediately");
            randomizeSharedItems = plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Randomization], "Randomize shared items", false, "Whether or not to randomize the shareable items sent to others\n\nUpdates immediately");
            randomizeToSameTier = plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Randomization], "Maintain tier", true, "Whether or not randomized items should be of the same tier as the original item\n\nWhen disabled the randomized item can be from the whitelist or any allowed tier\n\nUpdates immediately");

            // [Sharing]
            itemWhitelist = plugin.Config.Bind<string>(SettingsTabs[SettingsTab.Sharing], "item whitelist", "", "Comma separated list of internal item names to share regardless of tier or blacklist\n\nUpdates when entering a new stage\n\nIMPORTANT: if you're changing this in the .cfg file with the game open, use the Reload button in the settings Debug tab!");
            itemBlacklist = plugin.Config.Bind<string>(SettingsTabs[SettingsTab.Sharing], "item blacklist", "", "Comma separated list of internal item names to not share even if their tier is shared\n\nUpdates when entering a new stage\n\nIMPORTANT: if you're changing this in the .cfg file with the game open, use the Reload button in the settings Debug tab!");
            baseItemBlacklist = plugin.Config.Bind<string>(SettingsTabs[SettingsTab.Sharing], "base item blacklist", String.Join(", ", ItemArchive.baseItemBlacklist), "WARNING: only change this if you know what you're doing!\n\nYou should really just add the items to your whitelist instead! This will reset on plugin version change!");
            shareLunarCoins = plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share lunar coins", true, "Whether or not to share Lunar coins.\n\nUpdates immediately");
            // tiers
            ConfigEntry<bool> shareVoidItems = plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share void items", false, "Whether or not to share void items (pink).\n\nUpdates when entering a new stage");
            sharedItemTiersConfig = new Dictionary<ItemTier, ConfigEntry<bool>>
            {
                { ItemTier.Tier1, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share white items", true, "Whether or not to share white items.\n\nUpdates when entering a new stage") },
                { ItemTier.Tier2, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share green items", true, "Whether or not to share green items.\n\nUpdates when entering a new stage") },
                { ItemTier.Tier3, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share red items", true, "Whether or not to share red items.\n\nUpdates when entering a new stage") },
                { ItemTier.Lunar, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share lunar items", false, "Whether or not to share lunar items (light blue).\n\nUpdates when entering a new stage") },
                { ItemTier.Boss, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share boss items", true, "Whether or not to share boss items (yellow).\n\nUpdates when entering a new stage") },
                { ItemTier.VoidTier1, shareVoidItems },
                { ItemTier.VoidTier2, shareVoidItems },
                { ItemTier.VoidTier3, shareVoidItems },
                { ItemTier.VoidBoss, shareVoidItems },
                { ItemTier.Cursed, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share cursed items", true, "Whether or not to share cursed items (purple).\n\nUpdates when entering a new stage") },
                { ItemTier.Page, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share pages", false, "Whether or not to share pages (dark blue).\n\nUpdates when entering a new stage") },
                { ItemTier.Hex, plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Sharing], "Share hex items", false, "Whether or not to share hex items (light green).\n\nUpdates when entering a new stage") }
            };

            // [Debug]
            debugMode = plugin.Config.Bind<bool>(SettingsTabs[SettingsTab.Debug], "Debug mode", false, "Whether or not to print debug messages to the log\n\nUpdates immediately");
            pluginVersion = plugin.Config.Bind<string>(SettingsTabs[SettingsTab.Debug], "Plugin version", Plugin.pluginVersion, "The current plugin version, used to detect up/downgrades of the plugin to reset base blacklist");
            // detect potential change in version and reset baseItemBlacklist
            if (pluginVersion.Value != Plugin.pluginVersion)
            {
                Debug.log("Plugin version change detected, resetting baseItemBlacklist and pluginVersion");
                baseItemBlacklist.BoxedValue = baseItemBlacklist.DefaultValue;
                pluginVersion.BoxedValue = Plugin.pluginVersion;
            }



            // In-game settings
            ModSettingsManager.AddOption(new CheckBoxOption(debugMode));
            ModSettingsManager.AddOption(new StringInputFieldOption(pluginVersion));
            ModSettingsManager.AddOption(new GenericButtonOption("Reload settings", SettingsTabs[SettingsTab.Debug], "Reload settings from .cfg file to apply changes without restarting the game.\n\nIf you changed the item white- or blacklist the changes will apply when entering the next stage", "Reload", reloadLists));
            ModSettingsManager.AddOption(new CheckBoxOption(randomizeItems));
            ModSettingsManager.AddOption(new CheckBoxOption(randomizeSharedItems));
            ModSettingsManager.AddOption(new CheckBoxOption(randomizeToSameTier));
            ModSettingsManager.AddOption(new CheckBoxOption(shareLunarCoins));
            ModSettingsManager.AddOption(new CheckBoxOption(shareVoidItems));
            foreach (KeyValuePair<ItemTier, ConfigEntry<bool>> pair in sharedItemTiersConfig)
            {
                if (new List<ItemTier> { ItemTier.VoidTier1, ItemTier.VoidTier2, ItemTier.VoidTier3, ItemTier.VoidBoss }.Contains(pair.Key)) { continue; }
                ModSettingsManager.AddOption(new CheckBoxOption(pair.Value));
            }
            ModSettingsManager.AddOption(new StringInputFieldOption(itemWhitelist));
            ModSettingsManager.AddOption(new StringInputFieldOption(itemBlacklist));
        }


        private static void reloadLists()
        {
            plugin.Config.Reload();
        }
    }
}

