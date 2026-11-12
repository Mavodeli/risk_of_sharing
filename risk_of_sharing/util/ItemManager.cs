using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;

namespace risk_of_sharing
{
    class ItemManager
    {
        public static bool IsItemSharable(PickupDef pickupDef)
        {
            return Settings.itemTiersToShare.Contains(pickupDef.itemTier);
        }

        public static bool IsItemSharable(ItemIndex itemIndex)
        {
            return ItemArchive.itemsToShare.Values.Any(itemTierIndexes => itemTierIndexes.Contains(itemIndex));
        }

        public static void GiveItem(CharacterMaster initialCharacterMaster, PickupDef pickupDef, bool temporary = false, int amount = 1)
        {
            // handle the one picking up the item first
            GiveItemToPlayer(
                initialCharacterMaster,
                Settings.randomizeOwnItems ? GetRandomizedItem(pickupDef) : pickupDef.itemIndex,
                temporary, amount);

            // then handle the remaining players
            foreach (CharacterMaster playerCharacterMaster in PlayerCharacterMasterController.instances.Where(pcmc => pcmc.master != initialCharacterMaster).Select(pcmc => pcmc.master))
            {
                GiveItemToPlayer(playerCharacterMaster,
                Settings.randomizeSharedItems ? GetRandomizedItem(pickupDef) : pickupDef.itemIndex,
                temporary, amount);
            }
        }

        private static void GiveItemToPlayer(CharacterMaster characterMaster, ItemIndex itemIndex, bool temporary = false, int amount = 1)
        {
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
            List<ItemIndex> availableItems = ItemArchive.itemsToShare[pickupDef.itemTier];

            return availableItems[random.Next(availableItems.Count)];
        }
    }

    class ItemArchive
    {
        public static Dictionary<ItemTier, List<ItemIndex>> itemsToShare;

        public static void initItemsToShare()
        {
            // init/reset Dictionary
            itemsToShare = new Dictionary<ItemTier, List<ItemIndex>>();

            foreach (ItemTier targetTier in Settings.itemTiersToShare)
            {
                itemsToShare[targetTier] = ItemCatalog.allItemDefs.Where(itemdef => itemdef.tier == targetTier).Select(def => def.itemIndex).ToList();
            }










            // DEBUG DEBUG DEBUG
            foreach (CharacterMaster playerCharacterMaster in PlayerCharacterMasterController.instances.Select(pcmc => pcmc.master))
            {
                playerCharacterMaster.GiveMoney(5000, true);
            }

        }
    }
}