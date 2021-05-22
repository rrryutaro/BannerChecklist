using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace BannerChecklist
{
    class CheckBanner
    {
        public static Dictionary<int, List<NPC>> itemBannerToNPCs;
        public static Dictionary<int, List<int>> itemToItemBannerID;

        public static List<Item> allNPCBanners;
        public static List<string> modNames;  

        public static void Initialize()
        {
            itemBannerToNPCs = new Dictionary<int, List<NPC>>();
            itemToItemBannerID = new Dictionary<int, List<int>>();
            allNPCBanners = new List<Item>();
            modNames = new List<string>();

            for (int i = -65; i < NPCLoader.NPCCount; i++)
            {
                NPC npc = new NPC();
                npc.SetDefaults(i);

                int itemBannerID = GetItemBannerID(npc);
                if (0 < itemBannerID)
                {
                    if (!itemBannerToNPCs.ContainsKey(itemBannerID))
                    {
                        int netID = BannerIDToItemNetID(npc);

                        itemBannerToNPCs.Add(itemBannerID, new List<NPC>());

                        // ThoriumModなどでは、同一のバナーアイテムで、別のNPCが存在し、それぞれ別カウントとなっているケースがある
                        if (!itemToItemBannerID.ContainsKey(netID))
                        {
                            itemToItemBannerID.Add(netID, new List<int>());

                            Item item = new Item();
                            item.SetDefaults(netID);
                            allNPCBanners.Add(item);

                            if (npc.modNPC != null && !modNames.Contains(npc.modNPC.mod.Name))
                            {
                                modNames.Add(npc.modNPC.mod.Name);
                            }
                        }
                        itemToItemBannerID[netID].Add(itemBannerID);
                    }
                    itemBannerToNPCs[itemBannerID].Add(npc);
                }
            }
        }

        public static int GetItemBannerID(NPC npc)
        {
            int result = npc.modNPC != null ? npc.modNPC.banner : Item.NPCtoBanner(npc.BannerID());
            return result;
        }
        public static int BannerIDToItemNetID(NPC npc)
        {
            int result = npc.modNPC != null ? npc.modNPC.bannerItem : Item.BannerToItem(GetItemBannerID(npc));
            return result;
        }

        public static Dictionary<int, int> BannerItemToKillCount(Item item)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            if (itemToItemBannerID.ContainsKey(item.netID))
            {
                foreach (var itemBannerID in itemToItemBannerID[item.netID])
                {
                    if (0 <= itemBannerID && itemBannerID < NPC.killCount.Length)
                    {
                        result.Add(itemBannerID, NPC.killCount[itemBannerID]);
                    }
                    else
                    {
                        result.Add(itemBannerID, -1);
                    }
                }
            }
            return result;
        }

        public CheckBanner()
        {
        }

        public static List<NPC> BannerItemToNPCs(Item item)
        {
            List<NPC> result = new List<NPC>();
            if (itemToItemBannerID.ContainsKey(item.netID))
            {
                foreach (var itemBannerID in itemToItemBannerID[item.netID])
                {
                    if (itemBannerToNPCs.ContainsKey(itemBannerID))
                    {
                        result.AddRange(itemBannerToNPCs[itemBannerID]);
                    }
                }
            }
            return result;
        }

        public static bool isAcquired(Item item)
        {
            bool result = BannerItemToKillCount(item).Any(x => 50 <= x.Value);
            return result;
        }
    }
}
