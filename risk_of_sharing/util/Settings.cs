using System.Collections.Generic;
using RoR2;

namespace risk_of_sharing
{
    class Settings
    {
        public static bool randomizeSharedItems = false;
        public static bool randomizeOwnItems = false;
        public static List<ItemTier> itemTiersToShare = new List<ItemTier>
        {
            ItemTier.Tier1,     // white
            ItemTier.Tier2,     // green
            ItemTier.Tier3,     // red
            ItemTier.Boss       // yellow
        };
    }
}

