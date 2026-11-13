using EntityStates;
using EntityStates.Scrapper;
using HarmonyLib;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace risk_of_sharing.patches
{
    [HarmonyPatch]
    public class ScrapperDropTagging
    {
        [HarmonyPatch(typeof(ScrappingToIdle), nameof(ScrappingToIdle.OnEnter))]
        [HarmonyPrefix]
        static bool PatchedOnEnter(ScrappingToIdle __instance)
        {
            // relevant base OnEnter functions we need to reassign here
            __instance.scrapperController = __instance.GetComponent<ScrapperController>();

            Util.PlaySound(ScrappingToIdle.enterSoundString, __instance.gameObject);
            __instance.PlayAnimation("Base", ScrappingToIdle.ScrappingToIdleStateHash, ScrappingToIdle.ScrappingParamHash, ScrappingToIdle.duration);

            if (ScrappingToIdle.muzzleflashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(ScrappingToIdle.muzzleflashEffectPrefab, __instance.gameObject, ScrappingToIdle.muzzleString, false);
            }
            if (!NetworkServer.active) { return false; }
            __instance.foundValidScrap = false;
            if (__instance.scrapperController.IsRewardPickupQueueEmpty()) { return false; }
            UniquePickup uniquePickup = __instance.scrapperController.PopRewardPickupQueue();
            __instance.foundValidScrap = true;
            Transform modelChild = __instance.FindModelChild(ScrappingToIdle.muzzleString);
            GenericPickupController.CreatePickupInfo pickupInfo = new GenericPickupController.CreatePickupInfo()
            {
                position = modelChild.position,
                pickup = uniquePickup,
                duplicated = true // this will prevent the scrap from being shared
            };
            PickupDropletController.CreatePickupDroplet(pickupInfo, pickupInfo.position, (Vector3.up * ScrappingToIdle.dropUpVelocityStrength) + (modelChild.forward * ScrappingToIdle.dropForwardVelocityStrength));

            return false;
        }
    }
}