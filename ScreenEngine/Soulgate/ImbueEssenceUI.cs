using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    class EssenceHolder
    {
        private int id;
        private string name;
        private Texture2D icon;
        private MenuButton increase, decrease;
        
        public int ID { get { return id; } }
        public string Name { get { return name; } }
        public Texture2D Icon { get { return icon; } }
        public int QueueQuantity { get; set; }

        public bool IsDiscovered { get; set; }

        public MenuButton Increase { get { return increase; } }
        public MenuButton Decrease { get { return decrease; } }

        public Vector2 Position { get; set; }

        public EssenceHolder(int ID, string Name, Texture2D Icon, MenuButton Increase, MenuButton Decrease)
        {
            id = ID;
            name = Name;
            icon = Icon;

            increase = Increase;
            decrease = Decrease;
        }

        public void Update(GameTime gt, Controls controls, bool canIncrease, ref int inertEssenceQuantity)
        {
            decrease.Position = new Point((int)Position.X + 68, (int)Position.Y + 29);
            increase.Position = new Point((int)Position.X + 233, (int)Position.Y + 29);

            decrease.Update(gt, controls);
            increase.Update(gt, controls);

            if (QueueQuantity > 0)
            {
                if (decrease.IsLeftClicked == true)
                {
                    QueueQuantity--;
                    inertEssenceQuantity++;
                }
            }

            if (canIncrease == true)
            {
                if (increase.IsLeftClicked == true)
                {
                    QueueQuantity++;
                    inertEssenceQuantity--;
                }
            }
        }
    }

    public class ImbueEssenceUI : BaseScreen
    {
        private Texture2D bg, essenceSlot, smallButton, smallButtonHover;
        private SpriteFont font, boldFont;

        private List<EssenceHolder> essences = new List<EssenceHolder>();
        private int inertEssenceQuantity;
        private float essencePositionY, scrollValue, scrollVelocity;

        public ImbueEssenceUI()
        {
        }

        public override void Load(ContentManager cm)
        {
            bg = cm.Load<Texture2D>("Interface/Soulgate/ImbueEssences/bg");
            essenceSlot = cm.Load<Texture2D>("Interface/Soulgate/ImbueEssences/essenceSlot");

            smallButton = cm.Load<Texture2D>("Interface/Global/smallButton");
            smallButtonHover = cm.Load<Texture2D>("Interface/Global/smallButtonHover");

            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            boldFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            Position = GameSettings.VectorCenter - bg.Center();
            mouseDragOffset = new Vector2(bg.Width / 2, 12);

            AddEssences();

            base.Load(cm);
        }
        public override void SetControlledEntity(BaseEntity controlledEntity)
        {
            base.SetControlledEntity(controlledEntity);
        }

        public void CheckEssence(bool isImbuing)
        {
            if (isImbuing == true)
                ImbueEssences();
            else
                UnimbueEssence();

            if (controlledEntity.STORAGE_Check(10) == true)
            {
                inertEssenceQuantity = controlledEntity.STORAGE_GetItem(10).CurrentAmount;

                for (int i = 0; i < essences.Count; i++)
                    inertEssenceQuantity -= essences[i].QueueQuantity;
            }
            else
                inertEssenceQuantity = 0;
        }

        private void AddEssences()
        {
            AddEssence(11);        
            AddEssence(12);
            AddEssence(13);
        }
        private void AddEssence(int id)
        {
            essences.Add(new EssenceHolder(id, ItemDatabase.Item(id).Name, ItemDatabase.Item(id).Icon,
                                           new MenuButton(Vector2.Zero, smallButton, smallButtonHover, smallButtonHover, 1f, true),
                                           new MenuButton(Vector2.Zero, smallButton, smallButtonHover, smallButtonHover, 1f, true)));
        }

        private string hints = "Imbue Essence Tips:\n\n" +
            "Imbue inert essense into other, more powerful forms. To imbue, increase\n" +
            "or decrease each essence with either button. Once finished, exit.\n\n" +
            "Everytime you rest, your previous queue of essence will be re-imbued.";

        private int totalDiscovered = 0;
        public override void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                totalDiscovered = 0;

                CheckDrag(new Rectangle((int)Position.X + 94, (int)Position.Y, 112, 20));
                clickRect = new Rectangle((int)Position.X, (int)Position.Y, bg.Width, bg.Height);

                UpdateWindowButtons(gt, 216, hints, "Hide Imbue Essence");

                scissorRect = new Rectangle((int)Position.X + 15, (int)Position.Y + 51, 271, 268);
                SmoothScroll(gt, 30f, 200f, 300f, 10f, ref essencePositionY, ref scrollValue, ref scrollVelocity, -(((totalDiscovered * (essenceSlot.Height + 2)) - scissorRect.Height) + 4), scissorRect);

                for (int i = 0; i < essences.Count; i++)
                {
                    if (essences[i].IsDiscovered == true)
                    {
                        essences[i].Position = new Vector2((int)Position.X + 18, (int)Position.Y + 55 + (i * (essenceSlot.Height + 2)) + essencePositionY);
                        essences[i].Update(gt, controls, inertEssenceQuantity > 0, ref inertEssenceQuantity);

                        totalDiscovered++;
                    }
                }

                if (IsActive == false)
                    CheckEssence(true);
            }

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(bg, Position, Color.White);

                sb.DrawString(boldFont, "Imbue Essence", Position + new Vector2(150, 12), "Imbue Essence".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);
                sb.DrawString(boldFont, "Inert Essence: " + inertEssenceQuantity, Position + new Vector2(150, 40), ("Inert Essence: " + inertEssenceQuantity).LineCenter(boldFont), Color.White, 1f);

                base.Draw(sb);

                sb.End();

                DrawScissored(sb);
            }
        }

        private Rectangle scissorRect;
        public override void DrawScissored(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                DrawInside(sb, scissorRect, () =>
                {
                    for (int i = 0; i < essences.Count; i++)
                    {
                        if (essences[i].IsDiscovered == true)
                        {
                            sb.Draw(essenceSlot, essences[i].Position, Color.White);
                            sb.Draw(essences[i].Icon, essences[i].Position, Color.White);

                            sb.DrawString(boldFont, essences[i].Name, essences[i].Position + new Vector2(72, 6), ColorHelper.UI_Gold);
                            sb.DrawString(boldFont, essences[i].QueueQuantity.ToString(), essences[i].Position + new Vector2(163, 45), essences[i].QueueQuantity.ToString().LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

                            essences[i].Increase.DrawButton(sb, Color.White);
                            essences[i].Decrease.DrawButton(sb, Color.White);

                            sb.DrawString(boldFont, "-", essences[i].Decrease.Position.ToVector2() + smallButton.Center(), "-".LineCenter(boldFont), Color.White, 1f);
                            sb.DrawString(boldFont, "+", essences[i].Increase.Position.ToVector2() + smallButton.Center(), "+".LineCenter(boldFont), Color.White, 1f);
                        }
                    }
                });
            }

            base.DrawScissored(sb);
        }

        public void UnimbueEssence()
        {
            //Replace all imbued essences with inert essence.
            for (int i = 0; i < essences.Count; i++)
            {
                if (controlledEntity.STORAGE_Check(essences[i].ID))
                {
                    BaseItem essence = controlledEntity.STORAGE_GetItem(essences[i].ID);

                    int quantity = essence.CurrentAmount;

                    controlledEntity.STORAGE_AddItem(10, quantity);
                    essence.CurrentAmount -= quantity;
                }
            }
        }
        public void ImbueEssences()
        {
            //Replace all inert essence with the respective essences.
            BaseItem inert = controlledEntity.STORAGE_GetItem(10);

            if (inert != null)
            {
                for (int i = 0; i < essences.Count; i++)
                {
                    if (essences[i].QueueQuantity > 0)
                    {
                        inert.CurrentAmount -= essences[i].QueueQuantity;
                        controlledEntity.STORAGE_AddItem(essences[i].ID, essences[i].QueueQuantity);
                    }
                }
            }
        }

        public void ResetPosition()
        {
            Position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            for (int i = 0; i < essences.Count; i++)
                builder.AppendLine(essences[i].ID + " " + essences[i].QueueQuantity + " " + essences[i].IsDiscovered);

            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string[] words = data[i].Split(' ');
                try
                {
                    int id = int.Parse(words[0]);
                    int quantity = int.Parse(words[1]);
                    bool isDiscovered = bool.Parse(words[2]);

                    for (int j = 0; j < essences.Count; j++)
                    {
                        if (essences[j].ID == id)
                        {
                            essences[j].QueueQuantity = quantity;
                            essences[j].IsDiscovered = isDiscovered;
                        }
                    }
                }
                catch
                {
                    Logger.AppendLine("Error parsing ImbueEssence line: " + data[i]);
                }
            }
        }

        public StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Essence (Total: " + essences.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            for (int i = 0; i < essences.Count; i++)
            {
                builder.AppendLine(essences[i].ID + " - " + essences[i].Name);
            }

            return builder;
        }
    }
}
