using HarmonyLib;
using RoR2;

namespace risk_of_sharing.patches
{
    [HarmonyPatch]
    public class LunarCoinPatch
    {
        [HarmonyPatch(typeof(LunarCoinDef), nameof(LunarCoinDef.GrantPickup))]
        [HarmonyPrefix]
        static bool AlteredGrantPickup(ref PickupDef.GrantContext context, LunarCoinDef __instance)
        {
            NetworkUser networkUser = Util.LookUpBodyNetworkUser(context.body);
            if (networkUser)
            {
                networkUser.AwardLunarCoins(__instance.coinValue);
                context.shouldDestroy = true;
                context.shouldNotify = true;

                if (Settings.shareLunarCoins.Value)
                {
                    // award lunar coins to the other players too
                    foreach (NetworkUser nwUser in NetworkUser.instancesList)
                    {
                        if (nwUser != networkUser) { nwUser.AwardLunarCoins(__instance.coinValue); }
                    }
                }
            }
            else
            {
                context.shouldNotify = false;
                context.shouldDestroy = false;
            }
            return false;
        }
    }
}