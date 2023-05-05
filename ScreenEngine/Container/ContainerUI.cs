using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.TileEngine.Objects.ContainerTypes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;

namespace Pilgrimage_Of_Embers.ScreenEngine.Container
{
    public class ContainerUI
    {
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = new Vector2(MathHelper.Clamp(value.X, 2, GameSettings.VectorResolution.X - 291), MathHelper.Clamp(value.Y, 2, GameSettings.VectorResolution.Y - 486));
            }
        }

        public bool IsActive { get { return container != null; } }
        public void ForceClose() { container = null; }

        private Texture2D containerBG, button, buttonHover, itemBG, itemBGHover;
        private SpriteFont font, largeFont;

        private MenuButton takeAllButton, destroyAllButton;

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        public ContainerUI()
        {
        }

        private ScreenManager screens;
        private TileMap map;
        public void SetReferences(ScreenManager screens, TileMap map)
        {
            this.screens = screens;
            this.map = map;
        }

        private const string containerDir = "Interface/Container/";
        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/LargeHeader");

            containerBG = cm.Load<Texture2D>(containerDir + "containerBG");
            button = cm.Load<Texture2D>(containerDir + "containerButton");
            buttonHover = cm.Load<Texture2D>(containerDir + "containerButtonHover");

            itemBG = cm.Load<Texture2D>("Interface/Inventory/Icons/iconBG");
            itemBGHover = cm.Load<Texture2D>("Interface/Inventory/Icons/iconBGSelect");

            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            takeAllButton = new MenuButton(Vector2.Zero, button, button, buttonHover, 1f, true);
            destroyAllButton = new MenuButton(Vector2.Zero, button, button, buttonHover, 1f, true);

            position = GameSettings.VectorCenter - containerBG.Center();
        }

        private string hints = "Loot Tips:\n\n" +
            "Left-click on an item to add it to your inventory, or right-click to destroy.\n" +
            "Alternatively, you can press one of the two buttons below.";

        private Controls controls = new Controls();
        private int TotalItemCount { get { if (container != null) return MathHelper.Clamp(container.Items.Count, 0, 23); else return 0; } }
        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            takeAllButton.Update(gt, controls);
            destroyAllButton.Update(gt, controls);

            if (container != null)
            {
                dragArea = new Rectangle((int)position.X + 50, (int)position.Y, 191, 20);
                uiRect = new Rectangle((int)position.X, (int)position.Y, containerBG.Width, containerBG.Height);

                CheckDragScreen();

                takeAllButton.Position = new Point((int)position.X + 16, (int)position.Y + containerBG.Height - 36);
                destroyAllButton.Position = new Point((int)position.X + 17 + button.Width, (int)position.Y + containerBG.Height - 36);

                ApplyProperItemGrid();

                for (int i = 0; i < TotalItemCount; i++)
                {
                    if (container.Items[i].itemRect.Contains(controls.MousePosition))
                    {
                        ToolTip.RequestStringAssign(container.Items[i].Name);

                        container.Items[i].isSelected = true;

                        if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        {
                            storage.AddItem(container.Items[i].ID, container.Items[i].CurrentAmount, false, true);
                            container.Items.RemoveAt(i);
                            --i;
                        }
                        if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                        {
                            container.Items.RemoveAt(i);
                            --i;
                        }
                    }
                    else
                        container.Items[i].isSelected = false;
                }

                if (takeAllButton.IsLeftClicked == true)
                {
                    for (int i = 0; i < container.Items.Count; i++)
                    {
                        storage.AddItem(container.Items[i].ID, container.Items[i].CurrentAmount, false, true);
                        container.Items.RemoveAt(i);
                        --i;
                    }
                }
                if (destroyAllButton.IsLeftClicked == true)
                {
                    for (int i = 0; i < container.Items.Count; i++)
                    {
                        container.Items.RemoveAt(i);
                        --i;
                    }
                }

                if (container.IsContainerEmpty() || container.IsTooFar())
                    container = null;

                hintRect = new Rectangle((int)position.X + 223, (int)position.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)position.X + 250, (int)position.Y, windowButton.Width - 20, windowButton.Height);

                if (hintRect.Contains(controls.MousePosition))
                {
                    isHintHover = true;

                    ToolTip.RequestStringAssign(hints);

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                    }
                }
                else
                    isHintHover = false;

                if (hideRect.Contains(controls.MousePosition))
                {
                    isHideHover = true;
                    ToolTip.RequestStringAssign("Hide Loot");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Button Click 1");
                        container = null;
                    }
                }
                else
                    isHideHover = false;
            }
            controls.UpdateLast();
        }

        float locationY = 0;
        private void ApplyProperItemGrid()
        {
            Vector2 gridOffset = new Vector2(position.X + 16, position.Y + 55);

            if (container.Items.Count > 3)
            {
                for (int index = 0; index < container.Items.Count; ++index)
                {
                    container.Items[index].gridLocation = new Point(index % 4, (int)(index / 4));
                    container.Items[index].UpdateRect(gridOffset, (int)locationY);
                }
            }
            else if (container.Items.Count < 4)
            {
                for (int index = 0; index < container.Items.Count; ++index)
                {
                    container.Items[index].gridLocation = new Point(index, 0);
                    container.Items[index].UpdateRect(gridOffset, (int)locationY);
                }
            }
        }

        private Rectangle dragArea;
        private bool isDragging = false;
        private Vector2 mouseDragOffset = new Vector2(145, 10);
        private void CheckDragScreen()
        {
            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                screens.SetCursorState(Cursor.CursorState.Move);
                Position = controls.MouseVector - mouseDragOffset;
            }
        }

        private Color goldColor = new Color(182, 191, 137, 255);
        public void Draw(SpriteBatch sb)
        {
            if (container != null)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(containerBG, position, Color.White, Vector2.Zero, 0f, 1f);

                sb.DrawStringBordered(largeFont, "Loot", position + new Vector2(containerBG.Width / 2, 12), "Loot".LineCenter(largeFont), 0f, 1f, 1f, goldColor, Color.Black);
                sb.DrawStringBordered(largeFont, container.ContainerName, position + new Vector2(containerBG.Width / 2, 38), container.ContainerName.LineCenter(largeFont), 0f, 1f, 1f, goldColor, Color.Black);


                if (isHintHover == true)
                    sb.Draw(windowButtonHover, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);
                else
                    sb.Draw(windowButton, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);

                if (isHideHover == true)
                    sb.Draw(windowButtonHover, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);
                else
                    sb.Draw(windowButton, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);

                sb.Draw(hintIcon, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);
                sb.Draw(hideIcon, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);


                takeAllButton.DrawButton(sb, Color.White);
                destroyAllButton.DrawButton(sb, Color.White);

                if (destroyAllButton.IsHover == true)
                    sb.DrawString(font, "Destroy All", destroyAllButton.Center, "Destroy All".LineCenter(font), goldColor, 1f);
                else
                    sb.DrawString(font, "Destroy All", destroyAllButton.Center, "Destroy All".LineCenter(font), Color.White, 1f);


                if (takeAllButton.IsHover == true)
                    sb.DrawString(font, "Take All", takeAllButton.Center, "Take All".LineCenter(font), goldColor, 1f);
                else
                    sb.DrawString(font, "Take All", takeAllButton.Center, "Take All".LineCenter(font), Color.White, 1f);

                DrawItems(sb);

                sb.End();
            }
        }
        private void DrawItems(SpriteBatch sb)
        {
            int totalCount = (int)MathHelper.Clamp(container.Items.Count, 0, 23); //0 to 23 is 24 total items

            for (int i = 0; i < totalCount; i++)
            {
                if (container.Items[i].isSelected == true)
                    sb.Draw(itemBGHover, container.Items[i].itemRect, Color.White);
                else
                    sb.Draw(itemBG, container.Items[i].itemRect, Color.White);

                sb.Draw(container.Items[i].Icon, container.Items[i].itemRect, Color.White);

                if (container.Items[i].CurrentAmount > 1)
                    sb.DrawString(font, container.Items[i].CurrentAmount.ToString(), new Vector2(container.Items[i].itemRect.X + 4, container.Items[i].itemRect.Y + 2), Vector2.Zero, Color.White, 1f);
            }
        }

        private MultiItem container = null;
        public void SetContainerData(MultiItem container)
        {
            this.container = container;
        }

        private EntityStorage storage; private BaseEntity entity;
        public void SetEntityData(BaseEntity entity, EntityStorage storage)
        {
            this.entity = entity;
            this.storage = storage;
        }

        private Rectangle uiRect;
        public bool IsClickingUI()
        {
            if (container != null)
                return uiRect.Contains(controls.MousePosition) || isDragging == true;
            else
                return false;
        }

        public void ResetPosition()
        {
            position = new Vector2(GameSettings.VectorCenter.X - (containerBG.Width / 2), GameSettings.VectorCenter.Y - containerBG.Height / 2);
        }
    }
}
