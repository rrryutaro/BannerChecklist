using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.UI;
using FKTModSettings;

namespace BannerChecklist
{
    class BannerChecklist : Mod
    {
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
                bannerChecklistTool = new BannerChecklistTool();
                
                Config.LoadConfig();
                LoadedFKTModSettings = ModLoader.GetMod("FKTModSettings") != null;
                try
                {
                    if (LoadedFKTModSettings)
                    {
                        LoadModSettings();
                    }
                }
                catch { }
            }
        }

        public override void PostAddRecipes()
        {
            CheckBanner.Initialize();
            BannerChecklistUI.instance.AddModFilter();
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

        public override void PreSaveAndQuit()
        {
            //Config.SaveValues();
        }

        public override void PostUpdateInput()
        {
            try
            {
                if (LoadedFKTModSettings && !Main.gameMenu)
                {
                    UpdateModSettings();
                }
            }
            catch { }
        }

        private void LoadModSettings()
        {
            ModSetting setting = ModSettingsAPI.CreateModSettingConfig(this);
        }

        private void UpdateModSettings()
        {
            ModSetting setting;
            if (ModSettingsAPI.TryGetModSetting(this, out setting))
            {
            }
        }
    }
}
