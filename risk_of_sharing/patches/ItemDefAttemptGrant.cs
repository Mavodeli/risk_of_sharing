using HarmonyLib;
using RoR2;

namespace risk_of_sharing.patches
{
    [HarmonyPatch]
    public class ItemDefAttemptGrantPatch
    {
        [HarmonyPatch(typeof(ItemDef), nameof(ItemDef.AttemptGrant))]
        [HarmonyPrefix]
        static bool AlteredGrant(ref PickupDef.GrantContext context, ItemDef __instance)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(context.pickup.pickupIndex);
            ItemIndex itemIndex1 = pickupDef != null ? pickupDef.itemIndex : ItemIndex.None;
            ItemDef itemDef1 = ItemCatalog.GetItemDef(itemIndex1);
            if (!itemDef1) { return false; }
            ItemIndex itemIndex2 = itemIndex1;
            ItemDef itemDef2 = itemDef1;
            ItemDef.Group group;
            if (ItemCatalog.IsPartOfGroup(itemDef1, DLC4Content.ItemGroupTypes.MutualExclusivity, out group) && group.IsCarryingItemInGroup(context.body.inventory, out itemDef2, itemDef1))
            {
                itemIndex2 = itemDef2.itemIndex;
                context.body.inventory.GetItemCountEffective(itemDef2);
            }
            if (itemIndex2 != itemIndex1)
            {
                context.body.inventory.ResetItemPermanent(itemIndex2);
                context.pickup = new UniquePickup()
                {
                    pickupIndex = PickupCatalog.FindPickupIndex(itemIndex2)
                };
            }
            bool flag = !context.controller || !context.controller.Duplicated;
            int itemCountEffective = context.body.inventory.GetItemCountEffective(DLC3Content.Items.Duplicator);
            if (context.pickup.isRental)
                context.body.inventory.GiveItemRented(itemIndex1);
            else if (context.pickup.isTempItem)
            {
                context.body.inventory.GiveItemTemp(itemIndex1, context.pickup.decayValue);
                if (itemCountEffective > 0 & flag)
                    HandleDuplicator(itemIndex1, context.body, 1f);
            }
            else
            {
                // share if applicable (and not duplicated)
                if (ItemManager.IsItemSharable(pickupDef))
                {
                    ItemManager.GiveItem(context.body.master, pickupDef, !context.controller.Duplicated);
                }
                else
                {
                    context.body.inventory.GiveItemPermanent(itemIndex1);
                }

                if (((!itemDef1.ContainsTag(ItemTag.CanBeTemporary) ? 0 : (itemCountEffective > 0 ? 1 : 0)) & (flag ? 1 : 0)) != 0)
                    HandleDuplicator(itemIndex1, context.body, 1f);
                else if (itemCountEffective > 0 && itemDef1.DoesNotContainTag(ItemTag.ObjectiveRelated) && itemDef1.DoesNotContainTag(ItemTag.Scrap) && itemDef1.DoesNotContainTag(ItemTag.WorldUnique))
                {
                    ItemIndex itemToGrant = Util.RollTemporaryItemFromItemIndex(itemDef1.itemIndex);
                    if (itemToGrant != ItemIndex.None & flag)
                        HandleDuplicator(itemToGrant, context.body, 1f);
                }
            }
            context.shouldDestroy = true;
            context.shouldNotify = true;

            static void HandleDuplicator(ItemIndex itemToGrant, CharacterBody pickerBody, float countToAdd)
            {
                EffectData effectData = new EffectData()
                {
                    origin = pickerBody.corePosition,
                    start = pickerBody.corePosition,
                    scale = pickerBody.radius
                };
                EffectManager.SpawnEffect(CharacterBody.CommonAssets.duplicatorPrefab, effectData, true);
                pickerBody.inventory.GiveItemTemp(itemToGrant, countToAdd);
            }

            return false;
        }
    }
}