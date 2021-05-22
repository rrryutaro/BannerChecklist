using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.UI;

namespace BannerChecklist
{
    class BannerChecklist : Mod
    {
        internal static string OldConfigFilePath = Path.Combine(Main.SavePath, "Mod Configs", "BannerChecklist.json");
        internal static BannerChecklist instance;
        internal ModHotKey HotKey;
        internal BannerChecklistTool bannerChecklistTool;

        public bool LoadedFKTModSettings = false;

        int lastSeenScreenWidth;
        int lastSeenScreenHeight;

        public BannerChecklist()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void Load()
        {
            instance = this;
            HotKey = RegisterHotKey("Toggle Banner Checklist", "V");
            if (!Main.dedServ)
            {
                // 旧設定ファイルの削除
                var oldConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "TeraBackup.json"); ;
                if (File.Exists(oldConfigPath))
                {
                    File.Delete(oldConfigPath);
                }

                bannerChecklistTool = new BannerChecklistTool();
            }
        }

        public override void PostAddRecipes()
        {
            CheckBanner.Initialize();
            if (!Main.dedServ)
            {
                BannerChecklistUI.instance.AddModFilter();
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));
            if (layerIndex != -1)
            {
                layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                    "BannerChecklist: UI",
                    delegate
                    {
                        if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight)
                        {
                            bannerChecklistTool.ScreenResolutionChanged();
                            lastSeenScreenWidth = Main.screenWidth;
                            lastSeenScreenHeight = Main.screenHeight;
                        }
                        bannerChecklistTool.UIUpdate();
                        bannerChecklistTool.UIDraw();

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            layerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (layerIndex != -1)
            {
                layers.Insert(layerIndex, new LegacyGameInterfaceLayer(
                    "ChestBrowser: Tooltip",
                    delegate
                    {
                        bannerChecklistTool.TooltipDraw();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
