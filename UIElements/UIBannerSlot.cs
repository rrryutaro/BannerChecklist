using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;


namespace BannerChecklist.UIElements
{
	class UIBannerSlot : UIItemSlot
	{
		public static Texture2D selectedBackgroundTexture = Main.inventoryBack15Texture;
		public static Texture2D recentlyDiscoveredBackgroundTexture = Main.inventoryBack10Texture;

        public UIBannerSlot(Item item) : base(item)
		{
        }

        public override int CompareTo(object obj)
        {
            int result = 0;

            bool isRevers = BannerChecklistUI.instance.isSortRevers();
            switch (BannerChecklistUI.instance.GetSortMode())
            {
                case SortMode.ItemID:
                    if (isRevers)
                        result = item.netID > (obj as UIBannerSlot).item.netID ? -1 : 1;
                    else
                        result = item.netID < (obj as UIBannerSlot).item.netID ? -1 : 1;
                    break;

                case SortMode.KillCount:
                    if (isRevers)
                        result = CheckBanner.BannerItemToKillCount(item).Max(x=> x.Value)  > CheckBanner.BannerItemToKillCount((obj as UIBannerSlot).item).Max(x => x.Value) ? -1 : 1;
                    else
                        result = CheckBanner.BannerItemToKillCount(item).Max(x => x.Value) < CheckBanner.BannerItemToKillCount((obj as UIBannerSlot).item).Max(x => x.Value) ? -1 : 1;
                    break;
            }

            return result;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Dictionary<int, int> killCount = CheckBanner.BannerItemToKillCount(item);
            disable = killCount.Max(x => x.Value) <= 50;

            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Tool.tooltip = $"{item.Name} [{string.Join(",", killCount.Select(x => x.Value))}]";
                BannerChecklist.instance.bannerChecklistTool.drawNPCList = CheckBanner.BannerItemToNPCs(item);
            }
        }
    }
}
