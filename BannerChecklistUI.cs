using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using BannerChecklist.UIElements;

namespace BannerChecklist
{
    public enum SortMode
    {
        ItemID,
        KillCount
    }

    class BannerChecklistUI : UIModState
	{
		static internal BannerChecklistUI instance;

        static internal int menuIconSize = 28;
        static internal int menuMargin = 4;

        internal UIDragablePanel panelMain;
        internal UISplitterPanel panelSplitter;
        internal UIPanel panelFilterChestType;
        internal UIPanel panelBanner;
        internal UIGrid gridFilterChestType;
        internal UIGrid gridBanner;
        internal UIHoverImageButton btnClose;
        internal UIImageListButton btnDisplayTarget;
        internal UIImageListButton btnIconSize;
        internal UIImageListButton btnSort;
        internal UIImageListButton btnSortReverse;
        internal UIImageListButton btnFilter;
        internal UIImageListButton btnFilterMod;

        internal bool updateNeeded;
        internal string caption = $"Banner Checklist v{BannerChecklist.instance.Version} Count:??";

        public SortMode GetSortMode()
        {
            SortMode result = btnSort.GetValue<SortMode>();
            return result;
        }
        public bool isSortRevers()
        {
            bool result = btnSortReverse.GetValue<bool>();
            return result;
        }

        public BannerChecklistUI(UserInterface ui) : base(ui)
		{
			instance = this;
		}

        public override void OnInitialize()
        {
        }

        public void InitializeUI()
        {
            RemoveAllChildren();

            //メインパネル
            panelMain = new UIDragablePanel(true, true, true);
            panelMain.caption = caption;
            panelMain.SetPadding(6);
			panelMain.Left.Set(400f, 0f);
			panelMain.Top.Set(400f, 0f);
            panelMain.Width.Set(314f, 0f);
            panelMain.MinWidth.Set(314f, 0f);
            panelMain.MaxWidth.Set(1393f, 0f);
            panelMain.Height.Set(131f, 0f);
            panelMain.MinHeight.Set(131f, 0f);
            panelMain.MaxHeight.Set(1000f, 0f);
			Append(panelMain);

            //フィルターパネル
            panelFilterChestType = new UIPanel();
            panelFilterChestType.SetPadding(6);
            panelFilterChestType.MinWidth.Set(100, 0);
            gridFilterChestType = new UIGrid();
            gridFilterChestType.Width.Set(-20f, 1f);
            gridFilterChestType.Height.Set(0, 1f);
            gridFilterChestType.ListPadding = 2f;
            panelFilterChestType.Append(gridFilterChestType);
            var filterGridScrollbar = new FixedUIScrollbar(userInterface);
            filterGridScrollbar.SetView(100f, 1000f);
            filterGridScrollbar.Height.Set(0, 1f);
            filterGridScrollbar.Left.Set(-20, 1f);
            panelFilterChestType.Append(filterGridScrollbar);
            gridFilterChestType.SetScrollbar(filterGridScrollbar);
            //バナーパネル
            panelBanner = new UIPanel();
            panelBanner.SetPadding(6);
            panelBanner.MinWidth.Set(100, 0);
            gridBanner = new UIGrid();
            gridBanner.Width.Set(-20f, 1f);
            gridBanner.Height.Set(0, 1f);
            gridBanner.ListPadding = 2f;
            panelBanner.Append(gridBanner);
            var chestGridScrollbar = new FixedUIScrollbar(userInterface);
            chestGridScrollbar.SetView(100f, 1000f);
            chestGridScrollbar.Height.Set(0, 1f);
            chestGridScrollbar.Left.Set(-20, 1f);
            panelBanner.Append(chestGridScrollbar);
            gridBanner.SetScrollbar(chestGridScrollbar);
            //スプリッターパネル
            panelSplitter = new UISplitterPanel(panelFilterChestType, panelBanner);
            panelSplitter.SetPadding(0);
            panelSplitter.Top.Pixels = menuIconSize + menuMargin * 2;
            panelSplitter.Width.Set(0, 1f);
            panelSplitter.Height.Set(-26 - menuIconSize, 1f);
            panelSplitter.Panel1Visible = false;
            panelMain.Append(panelSplitter);

            //閉じるボタン
            Texture2D texture = BannerChecklist.instance.GetTexture("UIElements/closeButton");
            btnClose = new UIHoverImageButton(texture, "Close");
            btnClose.OnClick += (a, b) => BannerChecklist.instance.bannerChecklistTool.visible = false;
            btnClose.Left.Set(-20f, 1f);
            btnClose.Top.Set(3f, 0f);
            panelMain.Append(btnClose);

            //ターゲット表示ボタン
            btnDisplayTarget = new UIImageListButton(
                new List<Texture2D>() {
                    Main.inventoryTickOnTexture.Resize(menuIconSize),
                    Main.inventoryTickOffTexture.Resize(menuIconSize)},
                new List<object>() { true, false },
                new List<string>() { "Display target npc", "Hide target npc" });
            btnDisplayTarget.OnClick += (a, b) =>
            {
                btnDisplayTarget.NextIamge();
                BannerChecklist.instance.bannerChecklistTool.isDisplayTarget = btnDisplayTarget.GetValue<bool>();
            };
            btnDisplayTarget.Left.Set(btnClose.Left.Pixels - menuMargin - menuIconSize, 1f);
            btnDisplayTarget.Top.Set(3f, 0f);
            panelMain.Append(btnDisplayTarget);

            //アイコンサイズボタン
            btnIconSize = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.Chest].Resize(menuIconSize),
                    Main.itemTexture[ItemID.Chest].Resize((int)(menuIconSize * 0.8f)),
                    Main.itemTexture[ItemID.Chest].Resize((int)(menuIconSize * 0.6f))},
                new List<object>() { 1.0f, 0.8f, 0.6f },
                new List<string>() { "Icon size large", "Icon size medium", "Icon size small" });
            btnIconSize.OnClick += (a, b) =>
            {
                btnIconSize.NextIamge();
                UIItemSlot.scale = btnIconSize.GetValue<float>();
            };
            btnIconSize.Left.Set(btnDisplayTarget.Left.Pixels - menuMargin - menuIconSize, 1f);
            btnIconSize.Top.Set(3f, 0f);
            panelMain.Append(btnIconSize);

            //フィルターボタン
            btnFilter = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.AlphabetStatueF].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatueF].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatueF].Resize(menuIconSize),
                },
                new List<object>() { FilterMode.All, FilterMode.Unacquired, FilterMode.Acquired },
                new List<string>() { "Filter: All", "Filter: Unacquired", "Filter: Acquired" });
            btnFilter.OnClick += (a, b) =>
            {
                btnFilter.NextIamge();
                updateNeeded = true;
            };
            btnFilter.Left.Set(menuMargin, 0f);
            btnFilter.Top.Set(3f, 0f);
            panelMain.Append(btnFilter);

            //ソートボタン
            btnSort = new UIImageListButton(
                new List<Texture2D>() {
                    Main.itemTexture[ItemID.AlphabetStatueS].Resize(menuIconSize),
                    Main.itemTexture[ItemID.AlphabetStatueS].Resize(menuIconSize),
                },
                new List<object>() { SortMode.ItemID, SortMode.KillCount },
                new List<string>() { "Sort: ItemID", "Sort: KillCount" });
            btnSort.OnClick += (a, b) =>
            {
                btnSort.NextIamge();
                gridBanner.UpdateOrder();
            };
            btnSort.Left.Set(btnFilter.Left.Pixels + (menuIconSize + menuMargin) * 2, 0f);
            btnSort.Top.Set(3f, 0f);
            panelMain.Append(btnSort);

            //ソートリバースボタン
            btnSortReverse = new UIImageListButton(
                new List<Texture2D>() {
                    BannerChecklist.instance.GetTexture("UIElements/reverseButton2").Resize(menuIconSize),
                    BannerChecklist.instance.GetTexture("UIElements/reverseButton1").Resize(menuIconSize) },
                new List<object>() { false, true },
                new List<string>() { "Off sort reverse", "On sort reverse" });
            btnSortReverse.OnClick += (a, b) =>
            {
                btnSortReverse.NextIamge();
                gridBanner.UpdateOrder();
            };
            btnSortReverse.Left.Set(btnSort.Left.Pixels + menuIconSize + menuMargin, 0f);
            btnSortReverse.Top.Set(3f, 0f);
            panelMain.Append(btnSortReverse);

            //Modフィルターボタン
            if (btnFilterMod != null)
            {
                btnFilterMod.Left.Set(btnSortReverse.Left.Pixels + (menuIconSize + menuMargin) * 2, 0f);
                btnFilterMod.Top.Set(3f, 0f);
                panelMain.Append(btnFilterMod);
            }
        }

        public void AddModFilter()
        {
            if (0 < CheckBanner.modNames.Count)
            {
                //Modフィルターボタン
                List<Texture2D> listTexture = new List<Texture2D>();
                List<object> listObject = new List<object>();
                List<string> listTooltip = new List<string>();
                listTexture.Add(Main.itemTexture[ItemID.AlphabetStatueM]);
                listObject.Add("All");
                listTooltip.Add("All");
                listTexture.Add(Main.itemTexture[ItemID.AlphabetStatueM]);
                listObject.Add("Vanilla");
                listTooltip.Add("Vanilla");
                foreach (var modName in CheckBanner.modNames)
                {
                    listTexture.Add(Main.itemTexture[ItemID.AlphabetStatueM]);
                    listObject.Add(modName);
                    listTooltip.Add(modName);
                }

                btnFilterMod = new UIImageListButton(listTexture, listObject, listTooltip);
                btnFilterMod.OnClick += (a, b) =>
                {
                    btnFilterMod.NextIamge();
                    updateNeeded = true;
                };
            }
        }

        private void Clear()
        {
            gridFilterChestType.Clear();
            gridBanner.Clear();
            panelMain.DragTargetClear();

            panelSplitter.Recalculate();
        }

        internal void UpdateGrid()
		{
			if (!updateNeeded) { return; }
			updateNeeded = false;

            Clear();

            foreach (var item in CheckBanner.allNPCBanners.Where(x => FilterCheck(x)))
            { 
                var box = new UIBannerSlot(item);
                gridBanner._items.Add(box);
                gridBanner._innerList.Append(box);
            }
            gridBanner.UpdateOrder();
            gridBanner._innerList.Recalculate();


            panelMain.caption = caption.Replace("??", $"{gridBanner._items.Count}");
        }

        private enum FilterMode
        {
            All,
            Acquired,
            Unacquired,
        }
        private bool FilterCheck(Item item)
        {
            bool result = false;
            switch (btnFilter.GetValue<FilterMode>())
            {
                case FilterMode.All:
                    result = true;
                    break;

                case FilterMode.Acquired:
                    result = CheckBanner.isAcquired(item);
                    break;

                case FilterMode.Unacquired:
                    result = !CheckBanner.isAcquired(item);
                    break;
            }
            if (result && btnFilterMod != null)
            {
                switch (btnFilterMod.GetValue<string>())
                {
                    case "All":
                        break;

                    case "Vanilla":
                        result = item.modItem == null;
                        break;

                    default:
                        result = item.modItem == null ? false : item.modItem.mod.Name.Equals(btnFilterMod.GetValue<string>());
                        break;
                }
            }
            return result;
        }

        public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			UpdateGrid();
		}

        public override TagCompound Save()
        {
            TagCompound result = base.Save();

            if (panelMain != null)
            {
                result.Add("position", panelMain.SavePositionJsonString());
                result.Add("btnDisplayTarget", btnDisplayTarget.Index);
                result.Add("btnIconSize", btnIconSize.Index);
                result.Add("btnFilter", btnFilter.Index);
                result.Add("btnSort", btnSort.Index);
                result.Add("btnSortReverse", btnSortReverse.Index);
            }
            return result;
        }

        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            if (tag.ContainsKey("position"))
            {
                panelMain.LoadPositionJsonString(tag.GetString("position"));
            }
            if (tag.ContainsKey("btnDisplayTarget"))
            {
                btnDisplayTarget.Index = tag.GetInt("btnDisplayTarget");
                BannerChecklist.instance.bannerChecklistTool.isDisplayTarget = btnDisplayTarget.GetValue<bool>();
            }
            if (tag.ContainsKey("btnIconSize"))
            {
                btnIconSize.Index = tag.GetInt("btnIconSize");
                UIItemSlot.scale = btnIconSize.GetValue<float>();
            }
            if (tag.ContainsKey("btnFilter"))
            {
                btnFilter.Index = tag.GetInt("btnFilter");
            }
            if (tag.ContainsKey("btnSort"))
            {
                btnSort.Index = tag.GetInt("btnSort");
            }
            if (tag.ContainsKey("btnSortReverse"))
            {
                btnSortReverse.Index = tag.GetInt("btnSortReverse");
            }
            updateNeeded = true;
        }
    }
}
