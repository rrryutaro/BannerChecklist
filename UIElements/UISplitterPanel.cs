﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BannerChecklist.UIElements
{
    class UISplitterPanel : UIPanel
    {
        private UIPanel Panel1;
        private UIPanel Panel2;
        public bool Panel1Visible { get; set; } = true;
        public bool Panel2Visible { get; set; } = true;
        public int BorderSize { get; set; } = 4;

        private Vector2 dragstart;
        private float dragstartSplitterLeft;

        private UISplitterBar SplitterBar;
        private class UISplitterBar : UIElement
        {
            public bool dragging;
            private static Color BorderColor = Color.White * 0.3f;
            private static Color BorderDraggingColor = Color.Yellow * 0.3f;
            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                if (dragging)
                {
                    spriteBatch.Draw(Main.magicPixel, GetDimensions().ToRectangle(), BorderDraggingColor);
                }
                else if (IsMouseHovering)
                {
                    spriteBatch.Draw(Main.magicPixel, GetDimensions().ToRectangle(), BorderColor);
                }
            }
            public override void MouseOver(UIMouseEvent evt)
            {
                base.MouseOver(evt);
            }
        }

        public UISplitterPanel(UIPanel panel1, UIPanel panel2)
        {
            this.Panel1 = panel1;
            this.Panel2 = panel2;
            SplitterBar = new UISplitterBar();
            SplitterBar.Width.Pixels = BorderSize;

            Append(panel2);
        }

        public void SetPanel1(UIPanel panel)
        {
            RemoveChild(Panel1);
            Panel1 = panel;
            Append(Panel1);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            DragStart(evt);
            base.MouseDown(evt);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            DragEnd(evt);
            base.MouseUp(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (evt.Target == this || SplitterBar.IsMouseHovering)
            {
                SplitterBar.dragging = true;
                dragstart = evt.MousePosition;
                dragstartSplitterLeft = SplitterBar.Left.Pixels;
            }
        }

        private void DragEnd(UIMouseEvent evt)
        {
            if (SplitterBar.dragging)
            {
                SplitterBar.dragging = false;
            }
        }

        public void SetSplitterBarLeft(float left)
        {
            CalculatedStyle dimensions = base.GetInnerDimensions();
            SplitterBar.Left.Pixels = left;
            if (SplitterBar.Left.Pixels < Panel1.MinWidth.Pixels)
            {
                SplitterBar.Left.Pixels = Panel1.MinWidth.Pixels;
            }
            else if (dimensions.Width - Panel2.MinWidth.Pixels - SplitterBar.Width.Pixels < SplitterBar.Left.Pixels)
            {
                SplitterBar.Left.Pixels = dimensions.Width - Panel2.MinWidth.Pixels - SplitterBar.Width.Pixels;
            }
            Recalculate();
        }
        public float GetSplitterBarLeft()
        {
            float result = SplitterBar.Left.Pixels;
            return result;
        }

        //public void InitializePosition()
        //{
        //    CalculatedStyle dimensions = base.GetInnerDimensions();
        //    float width = dimensions.Width - BorderSize;
        //    Panel1.Width.Pixels = width / 2;
        //    Panel1.Height.Pixels = dimensions.Height;
        //
        //    SplitterBar.Left.Pixels = width / 2;
        //    SplitterBar.Top.Pixels = 0;
        //    SplitterBar.Width.Pixels = BorderSize;
        //    SplitterBar.Height.Pixels = dimensions.Height;
        //
        //    Panel2.Left.Pixels = width / 2 + BorderSize;
        //    Panel2.Width.Pixels = width / 2;
        //    Panel2.Height.Pixels = dimensions.Height;
        //}

        public override void Recalculate()
        {
            base.Recalculate();
            CalculatedStyle dimensions = base.GetInnerDimensions();

            if (Panel1Visible)
            {
                Append(Panel1);
                Append(SplitterBar);

                if (SplitterBar.Left.Pixels < Panel1.MinWidth.Pixels)
                {
                    SplitterBar.Left.Pixels = Panel1.MinWidth.Pixels;
                }
                else if (dimensions.Width - Panel2.MinWidth.Pixels - SplitterBar.Width.Pixels < SplitterBar.Left.Pixels)
                {
                    SplitterBar.Left.Pixels = dimensions.Width - Panel2.MinWidth.Pixels - SplitterBar.Width.Pixels;
                }
                SplitterBar.Height.Pixels = dimensions.Height;

                Panel1.Width.Pixels = SplitterBar.Left.Pixels;
                Panel1.Height.Pixels = dimensions.Height;
                Panel2.Left.Pixels = SplitterBar.Left.Pixels + SplitterBar.Width.Pixels;
                Panel2.Width.Pixels = dimensions.Width - Panel1.Width.Pixels - SplitterBar.Width.Pixels;
                Panel2.Height.Pixels = dimensions.Height;
            }
            else
            {
                RemoveChild(Panel1);
                RemoveChild(SplitterBar);

                Panel2.Left.Pixels = 0;
                Panel2.Width.Pixels = dimensions.Width;
                Panel2.Height.Pixels = dimensions.Height;
            }
            RecalculateChildren();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SplitterBar.dragging)
            {
                CalculatedStyle dimensions = base.GetInnerDimensions();
                Point pos = dragstart.Offset(-Main.MouseScreen.X, 0).ToPoint();

                SetSplitterBarLeft(dragstartSplitterLeft - pos.X);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (Panel1Visible)
            {
                Panel1.Draw(spriteBatch);
                SplitterBar.Draw(spriteBatch);
                Panel2.Draw(spriteBatch);
            }
            else
            {
                Panel2.Draw(spriteBatch);
            }
        }
    }
}
