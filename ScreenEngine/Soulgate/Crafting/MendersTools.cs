using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Performance;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System.Collections.Generic;
using System.Text;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate.Crafting
{
    public class MendersTools
    {
        private Vector2 position;
        private Vector2 Position
        {
            set
            {
                position = new Vector2(MathHelper.Clamp(value.X, 37, GameSettings.VectorResolution.X - (tab.Width + pane.Width + 5)),
                                       MathHelper.Clamp(value.Y, 0, GameSettings.VectorResolution.Y - tab.Height));
            }
        }
        private Rectangle dragArea, clickCheck;

        private Texture2D tab, pane, paneEnd, iconButton, iconButtonHover, paneButton, paneButtonHover, smallButton, smallButtonHover, tabButton, tabButtonHover;
        private SpriteFont font, largeFont;

        private MenuButton buttonOne, buttonTwo, buttonThree, buttonFour; //Pane buttons.


        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;


        public bool IsActive { get; set; }
        private bool isListRefreshing = true;

        private float scrollPosition = 0;
        private float currentPaneWidth = 0f, paneLerp = 0f; //Pane width. Increased when recipe is selected

        private bool isSelected = false;

        BaseItem.TabType currentTab = BaseItem.TabType.Consumables;

        private Controls controls = new Controls();
        private ScreenManager screens;
        private BaseEntity controlledEntity;

        private BaseItem selectedItem = null;

        private string itemCost;

        private EntityStorage storage;
        private List<BaseItem> currentItems = new List<BaseItem>(); //The current items to be displayed.
        private List<BaseItem> totalItems = new List<BaseItem>();
        private Dictionary<int, int[]> requireItems = new Dictionary<int, int[]>(); //Item ID, Reinforcement Level Ember Cost

        public MendersTools() { }

        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;
        }
        public void SetControlledEntity(BaseEntity controlledEntity, EntityStorage storage)
        {
            this.controlledEntity = controlledEntity;
            this.storage = storage;

            UpdateList();
        }

        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            tab = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Artisan/tab");
            pane = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Artisan/pane");
            paneEnd = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Artisan/paneEnd");

            iconButton = cm.Load<Texture2D>("Interface/Global/iconBG");
            iconButtonHover = cm.Load<Texture2D>("Interface/Global/iconBGSelect");

            paneButton = cm.Load<Texture2D>("Interface/Global/paneExtendedButton");
            paneButtonHover = cm.Load<Texture2D>("Interface/Global/paneExtendedButtonHover");

            smallButton = cm.Load<Texture2D>("Interface/Global/smallButton");
            smallButtonHover = cm.Load<Texture2D>("Interface/Global/smallButtonHover");

            tabButton = cm.Load<Texture2D>("Interface/Global/tabButton");
            tabButtonHover = cm.Load<Texture2D>("Interface/Global/tabButtonSelect");


            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            position = new Vector2(GameSettings.VectorCenter.X - (tab.Center().X + pane.Center().X),
                                   GameSettings.VectorCenter.Y - tab.Center().Y);

            mouseDragOffset = new Vector2(tab.Width / 2, 12);

            buttonOne = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonTwo = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonThree = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonFour = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);

            AddIngredients();
            UpdateList();
        }

        private void AddIngredients()
        {
            AddIngredient(1005, 500);
        }
        private void AddIngredient(int id, params int[] cost)
        {
            requireItems.Add(id, cost);
        }

        private string hints = "Mender's Tools Tips:\n\n" +
            "This interface is for repairing any broken item. Only broken items\n" +
            "will appear here. If an item is not broken, resting at a soulgate\n" +
            "will fully repair it.\n\n" +
            "The required ingredients to repair an item appear in the right\n" +
            "pane when an item is selected.";

        public void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                clickCheck = new Rectangle((int)position.X, (int)position.Y + 20, tab.Width + (int)currentPaneWidth, tab.Height - 20);

                if (isListRefreshing == true)
                {
                    UpdateList();
                    isListRefreshing = false; //Check the list whenever the UI is opened.
                }

                controls.UpdateCurrent();

                CheckDragScreen();
                //UpdateTabButtons(gt);

                UpdateItems(gt);
                UpdateButtons(gt);

                LerpPane(gt, isSelected);

                hintRect = new Rectangle((int)position.X + 315, (int)position.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)position.X + 342, (int)position.Y, windowButton.Width - 20, windowButton.Height);

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
                    ToolTip.RequestStringAssign("Hide Mender's Tools");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Button Click 1");
                        IsActive = false;
                    }
                }
                else
                    isHideHover = false;

                controls.UpdateLast();
            }
            else
                isListRefreshing = true;
        }

        CallLimiter limitIngredientCheck = new CallLimiter(2500), limitItemCheck = new CallLimiter(3000);
        private void UpdateItems(GameTime gt)
        {
            isSelected = false;
            ApplyProperItemGrid();

            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].itemRect.Contains(controls.MousePosition))
                {
                    if (CanRepair(currentItems[i]))
                        ToolTip.RequestStringAssign(currentItems[i].Name);
                    else
                        ToolTip.RequestStringAssign(currentItems[i].Name + "\n\nYou lack necessary ingredients.");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Item Select");
                        currentItems[i].isSelected = true;
                    }
                }
                else if (!scissorPane.Contains(controls.MousePosition) && !isDragging)
                {
                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        currentItems[i].isSelected = false;
                }

                if (currentItems[i].isSelected == true)
                {
                    selectedItem = currentItems[i];
                    itemCost = "$ " + ItemRepairCost(selectedItem).CommaSeparation();
                    isSelected = true;
                }
            }

            if (limitItemCheck.IsCalling(gt))
                UpdateList();
        }

        private void UpdateButtons(GameTime gt)
        {
            buttonOne.Update(gt, controls);
            buttonTwo.Update(gt, controls);
            buttonThree.Update(gt, controls);
            buttonFour.Update(gt, controls);

            buttonOne.Position = new Point((int)position.X + 457, (int)position.Y + 307);
            buttonTwo.Position = new Point((int)position.X + 457, (int)position.Y + 328);
            buttonThree.Position = new Point((int)position.X + 457, (int)position.Y + 349);
            buttonFour.Position = new Point((int)position.X + 457, (int)position.Y + 370);

            if (selectedItem != null && CanRepair(selectedItem) == true && isSelected == true)
            {
                if (buttonOne.IsHover == true)
                    ToolTip.RequestStringAssign("Repair Item");
                if (buttonOne.IsLeftClicked == true)
                    RepairItem(selectedItem);
            }
        }

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private void CheckDragScreen()
        {
            dragArea = new Rectangle((int)position.X + 114, (int)position.Y, 193, 20);

            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                Position = controls.MouseVector - mouseDragOffset;
                screens.SetCursorState(Cursor.CursorState.Move);
            }
        }
        private void LerpPane(GameTime gt, bool isSelected)
        {
            if (isSelected == true)
                paneLerp += 4f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                paneLerp -= 4f * (float)gt.ElapsedGameTime.TotalSeconds;

            paneLerp = MathHelper.Clamp(paneLerp, 0f, 1f);

            currentPaneWidth = MathHelper.SmoothStep(-3f, pane.Width, paneLerp);
        }

        float scrollValue = 0f; float scrollVelocity = 0f;
        private void ScrollGrid(GameTime gt, float scrollSpeed, float maxScrollSpeed, float scrollSlowdown, float clampSpeed)
        {
            if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                scrollVelocity -= scrollSpeed;
            else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                scrollVelocity += scrollSpeed;

            scrollValue = controls.CurrentMS.ScrollWheelValue;

            //Smooth scrolling code
            scrollPosition += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -maxScrollSpeed, maxScrollSpeed);

            if (scrollVelocity > clampSpeed)
                scrollVelocity -= scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -clampSpeed)
                scrollVelocity += scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -clampSpeed && scrollVelocity < clampSpeed)
                scrollVelocity = 0f;

            float longBounds = -((int)((currentItems.Count - 1) / 6) * (iconButton.Height + 1)) + (scissorGrid.Height - 70);
            if (longBounds >= 0f)
                longBounds = 0f;

            if (scrollPosition > 0f)
                scrollVelocity = 0f;
            if (scrollPosition < longBounds)
                scrollVelocity = 0f;

            scrollPosition = MathHelper.Clamp(scrollPosition, longBounds, 0f);
        }
        private void ApplyProperItemGrid()
        {
            Vector2 gridOffset = new Vector2(position.X + 18, position.Y + 73);

            if (currentItems.Count >= 6)
            {
                for (int i = 0; i < currentItems.Count; i++)
                {
                    Point gridLocation = new Point(i % 6, (int)(i / 6));
                    currentItems[i].itemRect = new Rectangle((int)gridOffset.X + (gridLocation.X * 65),
                                                            (int)gridOffset.Y + (gridLocation.Y * 65) + (int)scrollPosition,
                                                            64, 64);
                }
            }
            else if (currentItems.Count < 6)
            {
                for (int i = 0; i < currentItems.Count; i++)
                {
                    Point gridLocation = new Point(i, 0);
                    currentItems[i].itemRect = new Rectangle(gridLocation.X * 65 + (int)gridOffset.X,
                                                            gridLocation.Y * 65 + (int)gridOffset.Y + (int)scrollPosition,
                                                            64, 64);
                }
            }
        }

        public void UpdateList()
        {
            totalItems = storage.TotalItems();
            currentItems.Clear();

            for (int i = 0; i < totalItems.Count; i++)
            {
                if (requireItems.ContainsKey(totalItems[i].ID)) //If the item can be repaired, add it to the list.
                {
                    if (totalItems[i].IsBroken())
                        currentItems.Add(totalItems[i]);
                }
            }
        }
        private void RepairItem(BaseItem item)
        {
            if (item != null)
            {
                if (CanRepair(item))
                {
                    controlledEntity.SKILL_RemoveEmbers(ItemRepairCost(item));
                    item.RepairItem();

                    currentItems.Remove(item);
                    selectedItem = null;

                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Successfully repaired item");
                }
                else
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Not enough embers");
            }
        }

        private Rectangle scissorGrid, scissorPane;
        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin();

                sb.Draw(pane, new Vector2(position.X + tab.Width - 10, position.Y + 45), new Rectangle(0, 0, (int)currentPaneWidth, pane.Height), Color.White);
                sb.Draw(paneEnd, new Vector2(position.X + (tab.Width + currentPaneWidth) - 10, position.Y + 45), Color.White);

                sb.Draw(tab, position, Color.White);


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


                sb.DrawString(largeFont, "Mender's Tools", position + new Vector2(tab.Width / 2, 12), "Mender's Tools".LineCenter(largeFont), ColorHelper.UI_LightGold, 1f);
                sb.DrawString(largeFont, "Items", position + new Vector2(tab.Width / 2, 47), "Items".LineCenter(largeFont), ColorHelper.UI_LightGold, 1f);

                sb.End();

                DrawInside(sb);
                DrawPane(sb);
            }
        }

        private Color halfTransparent = Color.Lerp(Color.White, Color.Transparent, .5f);
        private void DrawTabButton(SpriteBatch sb, Rectangle rect, Texture2D icon, BaseItem.TabType type)
        {
            if (currentTab == type)
            {
                sb.Draw(tabButtonHover, rect, Color.White);
                sb.Draw(icon, rect, Color.White);
            }
            else
            {
                sb.Draw(tabButton, rect, Color.White);
                sb.Draw(icon, rect, halfTransparent);
            }
        }

        private void DrawInside(SpriteBatch sb)
        {
            scissorGrid = new Rectangle((int)position.X + 15, (int)position.Y + 71, 391, 263);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorGrid;

            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].isSelected == true)
                    DrawButtonState(sb, currentItems[i], iconButtonHover, currentItems[i].itemRect.Location.ToVector2());
                else
                    DrawButtonState(sb, currentItems[i], iconButton, currentItems[i].itemRect.Location.ToVector2());

                DrawIconState(sb, currentItems[i], currentItems[i].itemRect.Location.ToVector2());
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }

        private void DrawPane(SpriteBatch sb)
        {
            scissorPane = new Rectangle((int)position.X + 412, (int)position.Y + 41, (int)currentPaneWidth + 4, 353);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorPane;

            buttonOne.DrawButton(sb, Color.White);
            buttonTwo.DrawButtonIdle(sb, halfTransparent);
            buttonThree.DrawButtonIdle(sb, halfTransparent);
            buttonFour.DrawButtonIdle(sb, halfTransparent);

            if (selectedItem != null)
            {
                DrawButtonState(sb, selectedItem, iconButton, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));
                DrawIconState(sb, selectedItem, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));

                sb.DrawString(largeFont, itemCost, new Vector2(scissorPane.X + 138, scissorPane.Y + 245), itemCost.LineCenter(largeFont), ColorHelper.UI_LightGold, 1f);

                sb.DrawString(font, selectedItem.Name, new Vector2(scissorPane.X + 172, scissorPane.Y + 30), selectedItem.Name.LineCenter(font), ColorHelper.UI_LightGold, 1f);
                sb.DrawString(font, selectedItem.Description.WrapText(font, 260), new Vector2(scissorPane.X + 10, scissorPane.Y + 88), Color.White);

                sb.DrawString(font, "Repair Item", buttonOne.Center, "Repair Item".LineCenter(font), Color.White, 1f);
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }

        private Color halfRed = Color.Lerp(Color.White, Color.Red, .5f);
        private void DrawIngredient(SpriteBatch sb, Rectangle rect, Texture2D icon, int id, int quantity)
        {
            Color color = Color.White;
            if (!controlledEntity.STORAGE_Check(id, quantity))
                color = halfRed; //If the controlled entity does not have the ingredient quantity, set color to red.

            if (rect.Contains(controls.MousePosition))
                sb.Draw(smallButtonHover, rect, color);
            else
                sb.Draw(smallButton, rect, color);

            sb.Draw(icon, rect, color);
            sb.DrawString(font, quantity.ToString(), rect.Location.ToVector2(), color);
        }

        private void DrawIconState(SpriteBatch sb, BaseItem recipe, Vector2 position)
        {
            sb.Draw(recipe.Icon, new Vector2(position.X, position.Y), Color.White);
        }
        private void DrawButtonState(SpriteBatch sb, BaseItem recipe, Texture2D texture, Vector2 position)
        {
            sb.Draw(texture, position, Color.White);
        }

        private int ItemRepairCost(BaseItem item)
        {
            if (requireItems.ContainsKey(item.ID))
                return requireItems[item.ID][item.CurrentReinforcement];

            //It should never reach here, ever.
            return 0;
        }
        private bool CanRepair(BaseItem item)
        {
            return controlledEntity.SKILL_Embers() >= ItemRepairCost(item);
        }

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return isDragging || clickCheck.Contains(controls.MousePosition) || hideRect.Contains(controls.MousePosition);
            else
                return false;
        }
        public void ResetPosition()
        {
            Position = new Vector2(GameSettings.VectorCenter.X - (tab.Width / 2), GameSettings.VectorCenter.Y - tab.Height / 2);
        }

        public StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Mending (Total: " + requireItems.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            foreach(KeyValuePair<int, int[]> item in requireItems)
            {
                builder.Append(item.Key + " - " + ItemDatabase.Item(item.Key).Name + "\n ");

                for (int i = 0; i < item.Value.Length; i++)
                    builder.Append("    Level " + i + ": x" + item.Value[i]);
            }

            return builder;
        }
    }
}
