using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;

namespace risk_of_sharing
{
    static class ItemManager
    {
        public static bool IsItemShareable(PickupDef pickupDef)
        {
            return IsItemShareable(pickupDef.itemIndex);
        }

        public static bool IsItemShareable(ItemIndex itemIndex)
        {
            return ItemArchive.itemsToShare.Values.Any(itemTierIndexes => itemTierIndexes.Contains(itemIndex));
        }

        public static void GiveItem(CharacterMaster initialCharacterMaster, PickupDef pickupDef, bool share = true, bool temporary = false, int amount = 1)
        {
            // handle the one picking up the item first
            GiveItemToPlayer(
                initialCharacterMaster,
                Settings.randomizeItems.Value ? GetRandomizedItem(pickupDef) : pickupDef.itemIndex,
                temporary, amount);

            if (!share) { return; }

            // then handle the remaining players
            foreach (CharacterMaster playerCharacterMaster in PlayerCharacterMasterController.instances.Where(pcmc => pcmc.master != initialCharacterMaster).Select(pcmc => pcmc.master))
            {
                GiveItemToPlayer(playerCharacterMaster,
                Settings.randomizeSharedItems.Value ? GetRandomizedItem(pickupDef) : pickupDef.itemIndex,
                temporary, amount);
            }
        }

        private static void GiveItemToPlayer(CharacterMaster characterMaster, ItemIndex itemIndex, bool temporary = false, int amount = 1)
        {
            Debug.log("Giving tier " + ItemArchive.FindItemDef(itemIndex).tier.ToString() + " item " + ItemArchive.FindItemDef(itemIndex).name + " to " + characterMaster.name);

            if (temporary)
            {
                characterMaster.inventory.GiveItemTemp(itemIndex, amount);
                return;
            }

            characterMaster.inventory.GiveItemPermanent(itemIndex, amount);
        }

        private static ItemIndex GetRandomizedItem(PickupDef pickupDef)
        {
            Random random = new Random();
            List<ItemIndex> availableItems = Settings.randomizeToSameTier.Value ? ItemArchive.itemsToShare[pickupDef.itemTier] : ItemArchive.allSharedItems;

            return availableItems[random.Next(availableItems.Count)];
        }
    }



    static class ItemArchive
    {
        public static Dictionary<ItemTier, List<ItemIndex>> itemsToShare;
        public static List<ItemIndex> allSharedItems;
        public static List<ItemIndex> itemWhitelist;
        public static List<ItemIndex> itemBlacklist;

        public static void whitelistItem(ItemIndex itemIndex) { itemWhitelist.Add(itemIndex); }
        public static void blacklistItem(ItemIndex itemIndex) { itemBlacklist.Add(itemIndex); }

        public static void initItemsToShare()
        {
            // (re) initialize white- & blacklist
            itemBlacklist = ProcessRawListString(Settings.baseItemBlacklist.Value);
            itemBlacklist.AddRange(ProcessRawListString(Settings.itemBlacklist.Value));
            itemWhitelist = ProcessRawListString(Settings.itemWhitelist.Value);

            // init/reset Dictionary
            itemsToShare = new Dictionary<ItemTier, List<ItemIndex>>();

            foreach (ItemTier targetTier in Settings.getSharedItemTiers())
            {
                itemsToShare[targetTier] = ConvertPickupIndicesToItemIndicesWithBlacklist(getPickupTierMapping()[targetTier]);
            }

            // add whitelisted items
            foreach (ItemIndex itemIndex in itemWhitelist)
            {
                ItemDef itemDef = FindItemDef(itemIndex);

                // add the tier if not already present
                if (!itemsToShare.Keys.Contains(itemDef.tier))
                {
                    itemsToShare[itemDef.tier] = new List<ItemIndex>();
                }

                itemsToShare[itemDef.tier].Add(itemDef.itemIndex);
                Debug.log("Added item: " + itemDef.name + " to the whitelist");
            }

            // also (re) initialize allSharedItems
            allSharedItems = new List<ItemIndex>();
            foreach (ItemTier tier in itemsToShare.Keys)
            {
                allSharedItems.AddRange(ItemArchive.itemsToShare[tier]);
            }
        }

        private static List<ItemIndex> ProcessRawListString(string rawListString)
        {
            List<string> itemNames = StringHelper.ParseCommaSeparatedList(rawListString);
            List<ItemIndex> outputIndices = new List<ItemIndex>();
            foreach (string itemName in itemNames)
            {
                ItemIndex itemIndex = ItemCatalog.FindItemIndex(itemName);
                if (itemIndex == ItemIndex.None) { continue; }
                outputIndices.Add(itemIndex);
            }
            return outputIndices;
        }

        private static List<ItemIndex> ConvertPickupIndicesToItemIndices(List<PickupIndex> pickupIndices)
        {
            return PickupCatalog.allPickups.Where(pickupDef => pickupIndices.Any(pickupIndex => pickupDef.pickupIndex == pickupIndex)).Select(pickupDef => pickupDef.itemIndex).ToList();
        }

        private static List<ItemIndex> FilterBlacklistedItems(List<ItemIndex> itemList)
        {
            return itemList.Where(itemIndex => !itemBlacklist.Contains(itemIndex)).ToList();
        }

        private static List<ItemIndex> ConvertPickupIndicesToItemIndicesWithBlacklist(List<PickupIndex> pickupIndices)
        {
            return FilterBlacklistedItems(ConvertPickupIndicesToItemIndices(pickupIndices));
        }

        public static ItemDef FindItemDef(ItemIndex itemIndex)
        {
            return ItemCatalog.allItemDefs.First(itemDef => itemDef.itemIndex == itemIndex);
        }



        // base item blacklist
        public static List<string> baseItemBlacklist = new List<string>
        {
            "PowerPyramid",         // alloyed collective Sentry Key
            "PowerCube",            // alloyed collective Prison Cube
            "MasterBattery",        // alloyed collective Encrypted Cerebellum
            "TreasureCache",        // Rusted Key
        };

        public static Dictionary<ItemTier, List<PickupIndex>> getPickupTierMapping()
        {
            return new Dictionary<ItemTier, List<PickupIndex>> {
            {ItemTier.Tier1, Run.instance.availableTier1DropList},
            {ItemTier.Tier2, Run.instance.availableTier2DropList},
            {ItemTier.Tier3, Run.instance.availableTier3DropList},
            {ItemTier.Lunar, Run.instance.availableLunarItemDropList},
            {ItemTier.Boss, Run.instance.availableBossDropList},
            {ItemTier.VoidTier1, Run.instance.availableVoidTier1DropList},
            {ItemTier.VoidTier2, Run.instance.availableVoidTier2DropList},
            {ItemTier.VoidTier3, Run.instance.availableVoidTier3DropList},
            {ItemTier.VoidBoss, Run.instance.availableVoidBossDropList},
            {ItemTier.Cursed, Run.instance.availableCursedTierDropList},
            {ItemTier.Page, Run.instance.availablePageTierDropList},
            {ItemTier.Hex, Run.instance.availableHexTierDropList}
            };
        }
    }
}