using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;

namespace BannerChecklist
{
	public class ChestBrowserPlayer : ModPlayer
	{
        private TagCompound chestBrowserData;

        public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (BannerChecklist.instance.HotKey.JustPressed)
			{
                BannerChecklist.instance.bannerChecklistTool.visible = !BannerChecklist.instance.bannerChecklistTool.visible;
                if (BannerChecklist.instance.bannerChecklistTool.visible)
                {
                    BannerChecklistUI.instance.updateNeeded = true;
                }
            }
        }

		public override TagCompound Save()
		{
            return new TagCompound
            {
                ["BannerChecklistUI"] = BannerChecklist.instance.bannerChecklistTool.uistate.Save(),
            };
        }

		public override void Load(TagCompound tag)
		{
            if (tag.ContainsKey("BannerChecklistUI"))
            {
                if (tag.Get<object>("BannerChecklistUI").GetType().Equals(typeof(TagCompound)))
                {
                    chestBrowserData = tag.Get<TagCompound>("BannerChecklistUI");
                }
            }
        }

        public override void OnEnterWorld(Player player)
        {
            BannerChecklistUI.instance.InitializeUI();
            if (chestBrowserData != null)
            {
                BannerChecklist.instance.bannerChecklistTool.uistate.Load(chestBrowserData);
            }
        }

    }
}
