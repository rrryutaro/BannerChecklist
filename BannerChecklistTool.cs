using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BannerChecklist
{
    class BannerChecklistTool : Tool
    {
        public List<NPC> drawNPCList;
        public bool isDisplayTarget = true;

        public BannerChecklistTool() : base(typeof(BannerChecklistUI))
        {
        }

        internal override void UIUpdate()
        {
            drawNPCList = null;
            base.UIUpdate();
        }

        internal override void TooltipDraw()
        {
            if (visible && !string.IsNullOrEmpty(tooltip))
            {
                if (isDisplayTarget && drawNPCList != null)
                {
                    tooltip += $"{Environment.NewLine}[Target]";
                    SpriteBatch spriteBatch = Main.spriteBatch;

                    Vector2 pos = Main.MouseScreen;

                    pos.Y += Main.fontMouseText.MeasureString(tooltip).Y + 20;
                    int maxHeight = 0;
                    foreach (var npc in drawNPCList)
                    {
                        maxHeight = maxHeight < npc.frame.Height ? npc.frame.Height : maxHeight;

                        Main.instance.LoadNPC(npc.type);
                        Texture2D npcTexture = Main.npcTexture[npc.type];
                        npc.frame.Width = npcTexture.Width;
                        npc.frame.Height = npcTexture.Height / Main.npcFrameCount[npc.type];

                        if (Main.screenWidth < pos.X + npc.frame.Width)
                        {
                            pos.X = Main.MouseScreen.X;
                            pos.Y += maxHeight + 4;
                            maxHeight = 0;
                        }
                        spriteBatch.Draw(npcTexture, pos, new Rectangle?(npc.frame), Color.White, 0, new Vector2(), 1.0f, SpriteEffects.None, 0f);
                        if (npc.color != default(Microsoft.Xna.Framework.Color))
                        {
                            Main.spriteBatch.Draw(npcTexture, pos, new Rectangle?(npc.frame), npc.color, 0, new Vector2(), 1.0f, SpriteEffects.None, 0f);
                        }
                        pos.X += npc.frame.Width + 4;
                    }
                }

                Main.hoverItemName = tooltip;
            }
        }
    }
}
