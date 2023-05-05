using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    public class Stonehold
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
        private Rectangle dragArea, clickRect;
        private bool isTabHover = false;

        private Texture2D tab, pane, paneEnd, iconButton, iconButtonHover, paneButton, paneButtonHover, smallButton, smallButtonHover, tabButton, tabButtonHover, ribbon, unknown;
        private Texture2D consumables, weapons, armor, ammo, jewellery, resources, miscellaneous;
        private SpriteFont font, largeFont;

        private Rectangle consumableRect, weaponsRect, armorRect, ammoRect, jewelleryRect, resourcesRect, miscellaneousRect;
        private MenuButton buttonOne, buttonTwo, buttonThree, buttonFour; //Pane buttons.

        public bool IsActive { get; set; }
        private float scrollPosition = 0;
        private float currentPaneWidth = 0f, paneLerp = 0f; //Pane width. Increased when recipe is selected

        private BaseItem selectedItem = null;
        private bool isSelected = false;

        BaseItem.TabType currentTab = BaseItem.TabType.Consumables;

        private Controls controls = new Controls();
        private ScreenManager screens;
        private TileEngine.TileMap tileMap;
        private Camera camera;
        private BaseEntity controlledEntity;

        private List<BaseItem> totalItems = new List<BaseItem>(), currentItems = new List<BaseItem>(); //The current recipes to be displayed.

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        public Stonehold()
        {
            IsActive = false;
        }

        public void SetReferences(ScreenManager screens, TileEngine.TileMap tileMap, Camera camera)
        {
            this.screens = screens;
            this.tileMap = tileMap;
            this.camera = camera;
        }
        public void SetControlledEntity(BaseEntity controlledEntity)
        {
            this.controlledEntity = controlledEntity;
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

            ribbon = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Artisan/perfectRibbon");
            unknown = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Artisan/unknown");


            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");


            consumables = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/consumable");
            weapons = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/weapon");
            armor = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/armor");
            ammo = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/ammo");
            jewellery = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/jewellery");
            resources = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/resources");
            miscellaneous = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/miscellaneous");

            position = new Vector2(GameSettings.VectorCenter.X - (tab.Center().X + pane.Center().X),
                                   GameSettings.VectorCenter.Y - tab.Center().Y);

            mouseDragOffset = new Vector2(tab.Width / 2, 12);

            buttonOne = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonTwo = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonThree = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonFour = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);

            UpdateList(BaseItem.TabType.Consumables);
        }

        private string hints = "Stonehold Tips:\n\n" +
            "Drag and drop items from your inventory to this window to deposit.\n" +
            "The Brewmaster's Contrivances and Ore Smelter will automatically\n" +
            "deposit items if the player does not have the respective interface\n" +
            "open.";

        public void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                controls.UpdateCurrent();

                clickRect = new Rectangle((int)position.X, (int)position.Y + 20, tab.Width + (int)currentPaneWidth, tab.Height - 20);

                CheckDragScreen();
                UpdateTabButtons(gt);

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
                    ToolTip.RequestStringAssign("Hide Stonehold");

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
        }

        private void UpdateTabButtons(GameTime gt)
        {
            consumableRect = new Rectangle((int)position.X - 33 - ((currentTab == BaseItem.TabType.Consumables) ? 4 : 0), (int)position.Y + 44, tabButton.Width, tabButton.Height);
            weaponsRect = new Rectangle((int)position.X - 33 - ((currentTab == BaseItem.TabType.Weapons) ? 4 : 0), (int)position.Y + 44 + (tabButton.Height), tabButton.Width, tabButton.Height);
            armorRect = new Rectangle((int)position.X - 33 - ((currentTab == BaseItem.TabType.Armor) ? 4 : 0), (int)position.Y + 44 + (tabButton.Height * 2), tabButton.Width, tabButton.Height);
            ammoRect = new Rectangle((int)position.X - 33 - ((currentTab == BaseItem.TabType.Ammo) ? 4 : 0), (int)position.Y + 44 + (tabButton.Height * 3), tabButton.Width, tabButton.Height);
            jewelleryRect = new Rectangle((int)position.X - 33 - ((currentTab == BaseItem.TabType.Jewellery) ? 4 : 0), (int)position.Y + 44 + (tabButton.Height * 4), tabButton.Width, tabButton.Height);
            resourcesRect = new Rectangle((int)position.X - 33 - ((currentTab == BaseItem.TabType.Resources) ? 4 : 0), (int)position.Y + 44 + (tabButton.Height * 5), tabButton.Width, tabButton.Height);
            miscellaneousRect = new Rectangle((int)position.X - 33 - ((currentTab == BaseItem.TabType.Miscellaneous) ? 4 : 0), (int)position.Y + 44 + (tabButton.Height * 6), tabButton.Width, tabButton.Height);

            isTabHover = false;

            if (consumableRect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Consumables");
                isTabHover = true;
            }
            if (weaponsRect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Weapons");
                isTabHover = true;
            }
            if (armorRect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Armor");
                isTabHover = true;
            }
            if (ammoRect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Ammo");
                isTabHover = true;
            }
            if (jewelleryRect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Jewellery");
                isTabHover = true;
            }
            if (resourcesRect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Resources");
                isTabHover = true;
            }
            if (miscellaneousRect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Miscellaneous");
                isTabHover = true;
            }

            CheckTabClick(consumableRect, BaseItem.TabType.Consumables);
            CheckTabClick(weaponsRect, BaseItem.TabType.Weapons);
            CheckTabClick(armorRect, BaseItem.TabType.Armor);
            CheckTabClick(ammoRect, BaseItem.TabType.Ammo);
            CheckTabClick(jewelleryRect, BaseItem.TabType.Jewellery);
            CheckTabClick(resourcesRect, BaseItem.TabType.Resources);
            CheckTabClick(miscellaneousRect, BaseItem.TabType.Miscellaneous);
        }
        private void CheckTabClick(Rectangle button, BaseItem.TabType tab)
        {
            if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
            {
                if (button.Contains(controls.MousePosition))
                {
                    screens.PlaySound("Button Click 2");

                    currentTab = tab;
                    UpdateList(currentTab);
                }
            }
        }

        private void UpdateItems(GameTime gt)
        {
            isSelected = false;
            ApplyProperItemGrid();

            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].itemRect.Contains(controls.MousePosition))
                {
                    ToolTip.RequestStringAssign(currentItems[i].Name);

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
                    isSelected = true;
                }
            }
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

            if (selectedItem != null)
            {
                if (buttonOne.IsLeftClicked == true)
                {
                    WithdrawItem(selectedItem, 1);
                }
                if (buttonTwo.IsLeftClicked == true)
                {
                    WithdrawItem(selectedItem, (int)MathHelper.Clamp(selectedItem.CurrentAmount, 1, 10));
                }
                if (buttonThree.IsLeftClicked == true)
                {
                    WithdrawItem(selectedItem, (int)MathHelper.Clamp(selectedItem.CurrentAmount, 1, 100));
                }
                if (buttonFour.IsLeftClicked == true)
                {
                    totalItems.Remove(selectedItem);
                    currentItems.Remove(selectedItem);
                    selectedItem = null;
                }
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

            float longBounds = -((int)((totalItems.Count - 1) / 6) * (iconButton.Height + 1)) + (scissorGrid.Height - 70);
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

        private void UpdateList(BaseItem.TabType type)
        {
            currentItems.Clear();

            for (int i = 0; i < totalItems.Count; i++)
            {
                if (totalItems[i].tabType == type) //Add item to list if it's the same tab type
                {
                    if (!currentItems.Contains(totalItems[i]))
                        currentItems.Add(totalItems[i]);
                }
            }

            //Sort by "IsPerfected" >> "HasIngredients" >> "NoIngredients" >> "IsDiscovered"...
            //OR sort by A-Z. Player choice.
        }

        public void WithdrawItem(BaseItem item, int quantity)
        {
            if (item != null)
            {
                //If the storage does not have an item with matching id ...
                if (controlledEntity.STORAGE_GetItem(item.ID) == null)
                {
                    BaseItem copy = item.Copy();
                    controlledEntity.STORAGE_AddItem(copy, controlledEntity.IsPlayerControlled);

                    item.CurrentAmount -= quantity;
                    copy.CurrentAmount = quantity;

                    if (item.CurrentAmount <= 0)
                    {
                        totalItems.Remove(item);
                        currentItems.Remove(item);
                        selectedItem = null;
                    }
                }
                else
                {
                    int quantityRemoved = controlledEntity.STORAGE_AddItemGetDifference(item.ID, quantity, false, true);
                    item.CurrentAmount -= quantityRemoved;

                    if (item.CurrentAmount <= 0)
                    {
                        totalItems.Remove(item);
                        currentItems.Remove(item);
                        selectedItem = null;
                    }
                }
            }
        }
        public void DepositItem(BaseItem item)
        {
            if (item != null)
            {
                totalItems.Add(item);

                screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Deposited item to Stonehold");

                UpdateList(currentTab);
            }
        }
        public int AddItemReturnDifference(int id, int quantity)
        {
            int startQuantity = quantity;
            BaseItem item = ItemDatabase.Item(id);

            if (item != null)
            {
                if (CheckForItem(id) == true)
                {
                    for (int i = 0; i < totalItems.Count; i++)
                    {
                        if (totalItems[i].ID == id)
                        {
                            if (totalItems[i].CurrentAmount < totalItems[i].MaxAmount)
                            {
                                //Return the difference or current quantity
                                int difference = Math.Min(quantity, totalItems[i].MaxAmount - totalItems[i].CurrentAmount);

                                //Add difference to current amount, then subtract it from the quantity
                                totalItems[i].CurrentAmount += difference;
                                quantity -= difference;
                            }
                        }
                    }

                    if (item.IsMultiStack == true)
                    {
                        //If there is still items left in the stack ...
                        if (quantity > 0)
                        {
                            //Add it!
                            totalItems.Add(item.Copy(screens, tileMap, controlledEntity, camera));
                            totalItems.Last().CurrentAmount = quantity;
                            totalItems.Last().IsEntityItem = false;
                        }
                    }
                }
                else
                {
                    //Scenario 1
                    totalItems.Add(item.Copy(screens, tileMap, controlledEntity, camera));
                    totalItems.Last().CurrentAmount = quantity;
                    totalItems.Last().IsEntityItem = false;
                }
            }

            return startQuantity - quantity;
        }
        public bool CheckForItem(int id)
        {
            for (int i = 0; i < totalItems.Count; i++)
            {
                if (totalItems[i].ID == id)
                    return true;
            }

            return false;
        }
        public BaseItem GetItem(int id)
        {
            for (int i = 0; i < totalItems.Count; i++)
            {
                if (totalItems[i].ID == id)
                    return totalItems[i];
            }

            return null;
        }

        private Rectangle scissorGrid, scissorPane;
        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin();

                DrawTabs(sb);

                sb.Draw(pane, new Vector2(position.X + tab.Width - 10, position.Y + 45), new Rectangle(0, 0, (int)currentPaneWidth, pane.Height), Color.White);
                sb.Draw(paneEnd, new Vector2(position.X + (tab.Width + currentPaneWidth) - 10, position.Y + 45), Color.White);

                sb.Draw(tab, position, Color.White);

                sb.DrawString(largeFont, "Stonehold", position + new Vector2(tab.Width / 2, 12), "Stonehold".LineCenter(largeFont), ColorHelper.UI_LightGold, 1f);
                sb.DrawString(largeFont, currentTab.ToString(), position + new Vector2(tab.Width / 2, 47), currentTab.ToString().LineCenter(largeFont), Color.White, 1f);


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


                sb.End();

                DrawInside(sb);
                DrawPane(sb);
            }
        }
        private void DrawTabs(SpriteBatch sb)
        {
            DrawTabButton(sb, consumableRect, consumables, BaseItem.TabType.Consumables);
            DrawTabButton(sb, weaponsRect, weapons, BaseItem.TabType.Weapons);
            DrawTabButton(sb, armorRect, armor, BaseItem.TabType.Armor);
            DrawTabButton(sb, ammoRect, ammo, BaseItem.TabType.Ammo);
            DrawTabButton(sb, jewelleryRect, jewellery, BaseItem.TabType.Jewellery);
            DrawTabButton(sb, resourcesRect, resources, BaseItem.TabType.Resources);
            DrawTabButton(sb, miscellaneousRect, miscellaneous, BaseItem.TabType.Miscellaneous);
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
                    DrawButtonState(sb, iconButtonHover, currentItems[i].itemRect.Location.ToVector2());
                else
                    DrawButtonState(sb, iconButton, currentItems[i].itemRect.Location.ToVector2());

                DrawIconState(sb, currentItems[i], currentItems[i].itemRect.Location.ToVector2());

                sb.DrawMoneyQuantity(font, currentItems[i].CurrentAmount, new Vector2(currentItems[i].itemRect.X + 4, currentItems[i].itemRect.Y + 4));
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
            buttonTwo.DrawButton(sb, Color.White);
            buttonThree.DrawButton(sb, Color.White);
            buttonFour.DrawButton(sb, Color.White);

            if (selectedItem != null)
            {
                DrawButtonState(sb, iconButton, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));
                DrawIconState(sb, selectedItem, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));

                sb.DrawString(font, selectedItem.Name, new Vector2(scissorPane.X + 172, scissorPane.Y + 30), selectedItem.Name.LineCenter(font), ColorHelper.UI_LightGold, 1f);
                sb.DrawString(font, selectedItem.Description.WrapText(font, 260), new Vector2(scissorPane.X + 10, scissorPane.Y + 88), Color.White);

                sb.DrawString(font, "Withdraw (One)", buttonOne.Center, "Withdraw (One)".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Withdraw (Ten)", buttonTwo.Center, "Withdraw (Ten)".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Withdraw (Hundred)", buttonThree.Center, "Withdraw (Hundred)".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Destroy Stack", buttonFour.Center, "Destroy Stack".LineCenter(font), Color.White, 1f);
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }

        private void DrawIconState(SpriteBatch sb, BaseItem item, Vector2 position)
        {
            sb.Draw(item.Icon, new Vector2(position.X, position.Y), Color.White);
        }
        private void DrawButtonState(SpriteBatch sb, Texture2D texture, Vector2 position)
        {
            sb.Draw(texture, position, Color.White);
        }

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return isDragging || clickRect.Contains(controls.MousePosition) || isTabHover ||
                hideRect.Contains(controls.MousePosition) || hintRect.Contains(controls.MousePosition);
            else
                return false;
        }
        public bool IsTabHover()
        {
            if (IsActive == true)
                return scissorGrid.Contains(controls.MousePosition);
            else
                return false;
        }
        public void ResetPosition()
        {
            Position = new Vector2(GameSettings.VectorCenter.X - (tab.Width / 2), GameSettings.VectorCenter.Y - tab.Height / 2);
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            for (int i = 0; i < totalItems.Count; i++)
                builder.Append(totalItems[i].SaveData());

            builder.AppendLine();
            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string[] words = data[i].Split(' ');

                int id = int.Parse(words[1]);
                BaseItem item = ItemDatabase.Item(id);

                if (item != null)
                {
                    item = item.Copy(screens, tileMap, controlledEntity, camera); //These won't be usable, so don't worry about setting null... I think.

                    item.LoadData(data[i]);
                    totalItems.Add(item);
                }
            }
        }
        public void ResetItems() { totalItems.Clear(); }
    }
}
