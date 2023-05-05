using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.ScreenEngine.Merchanting
{
    public class PurchaseUI
    {
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = new Vector2(MathHelper.Clamp(value.X, 0, GameSettings.VectorResolution.X - 535), MathHelper.Clamp(value.Y, 0, GameSettings.VectorResolution.Y - 375));
            }
        }

        private Rectangle uiArea;

        private Texture2D bg, paneBG, paneEnd, iconBG, iconBGSelect, paneButton, paneButtonHover, tabButton, tabButtonSelect;
        private SpriteFont font, largeFont;

        private float locationY; //Grid's location
        private float currentPaneWidth = 0f, paneLerp = 0f; //Pane's width. Increased when item is selected

        private MenuButton examineButton, buyOneButton, buyXButton, buyAllButton;

        private List<BaseItem> merchantStock = new List<BaseItem>();
        private BaseItem selectedItem = null;
        private bool isSelected = false;

        private Controls controls = new Controls();

        private ScreenManager screens;
        private BaseEntity merchantEntity, controlledEntity;

        public bool IsActive { get; set; }

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        public PurchaseUI()
        {
            IsActive = false;
        }

        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;
        }
        public void SetMerchant(BaseEntity merchant) { merchantEntity = merchant; merchantStock = merchant.MERCHANT_Stock(); }
        public void SetControlledEntity(BaseEntity entity) { controlledEntity = entity; }

        public void Load(ContentManager cm)
        {
            bg = cm.Load<Texture2D>("Interface/Merchanting/Purchasing/bg");
            paneBG = cm.Load<Texture2D>("Interface/Merchanting/Purchasing/paneBG");
            paneEnd = cm.Load<Texture2D>("Interface/Merchanting/Purchasing/paneEnd");

            paneButton = cm.Load<Texture2D>("Interface/Merchanting/Purchasing/paneButton");
            paneButtonHover = cm.Load<Texture2D>("Interface/Merchanting/Purchasing/paneButtonHover");

            iconBG = cm.Load<Texture2D>("Interface/Global/iconBG");
            iconBGSelect = cm.Load<Texture2D>("Interface/Global/iconBGSelect");

            tabButton = cm.Load<Texture2D>("Interface/Global/tabButton");
            tabButtonSelect = cm.Load<Texture2D>("Interface/Global/tabButtonSelect");

            examineButton = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, false);
            buyOneButton = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, false);
            buyXButton = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, false);
            buyAllButton = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, false);

            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            font = cm.Load<SpriteFont>("Fonts/RegularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            mouseDragOffset = new Vector2(bg.Center().X, 12);
            position = new Vector2(GameSettings.VectorCenter.X - bg.Center().X, GameSettings.VectorCenter.Y - bg.Center().Y);
        }
        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            if (IsActive == true)
            {
                CheckDragScreen();

                uiArea = new Rectangle((int)position.X, (int)position.Y, bg.Width, bg.Height);

                ScrollGrid(gt, 50f, 500f, 300f, 10f);
                ApplyProperItemGrid();

                UpdateBehavior(gt);
                UpdateButtons(gt);

                LerpPane(gt, isSelected);

                hintRect = new Rectangle((int)position.X + 270, (int)position.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)position.X + 297, (int)position.Y, windowButton.Width - 20, windowButton.Height);

                if (hintRect.Contains(controls.MousePosition))
                {
                    isHintHover = true;

                    ToolTip.RequestStringAssign("Purchasing Tips:\n\nClick an item and click one of the buttons off to the side.\nItems marked in blue will never automatically deplete every\nday. A merchant's stock is changing every day, so check\nback often.\n\nYou may also quicksell an item by dragging and dropping it\nfrom your inventory.");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                    }
                }
                else
                    isHintHover = false;

                if (hideRect.Contains(controls.MousePosition))
                {
                    isHideHover = true;
                    ToolTip.RequestStringAssign("Hide Purchasing");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Button Click 1");
                        IsActive = false;
                    }
                }
                else
                    isHideHover = false;
            }

            controls.UpdateLast();
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
            locationY += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -maxScrollSpeed, maxScrollSpeed);

            if (scrollVelocity > clampSpeed)
                scrollVelocity -= scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -clampSpeed)
                scrollVelocity += scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -clampSpeed && scrollVelocity < clampSpeed)
                scrollVelocity = 0f;

            float longBounds = -((int)((merchantStock.Count - 1) / 5) * (iconBG.Height + 1)) + (scissorGrid.Height - 70);
            if (longBounds >= 0f)
                longBounds = 0f;

            if (locationY > 0f)
                scrollVelocity = 0f;
            if (locationY < longBounds)
                scrollVelocity = 0f;

            locationY = MathHelper.Clamp(locationY, longBounds, 0f);
        }
        private void ApplyProperItemGrid()
        {
            Vector2 gridOffset = new Vector2(position.X + 18, position.Y + 61);

            if (merchantStock.Count >= 5)
            {
                for (int index = 0; index < merchantStock.Count; ++index)
                {
                    merchantStock[index].gridLocation = new Point(index % 5, (int)(index / 5));
                    merchantStock[index].UpdateRect(gridOffset, (int)locationY);
                }
            }
            else if (merchantStock.Count < 5)
            {
                for (int index = 0; index < merchantStock.Count; ++index)
                {
                    merchantStock[index].gridLocation = new Point(index, 0);
                    merchantStock[index].UpdateRect(gridOffset, (int)locationY);
                }
            }
        }

        private void UpdateBehavior(GameTime gt)
        {
            isSelected = false;

            for (int i = 0; i < merchantStock.Count; i++)
            {
                if (merchantStock[i].itemRect.Contains(controls.MousePosition))
                {
                    if (GameSettings.IsDebugging == true)
                    {
                        ToolTip.RequestStringAssign(merchantStock[i].UniqueID + " -- " + merchantStock[i].GetHashCode());
                    }
                    else
                        ToolTip.RequestStringAssign(merchantStock[i].Name + " (" + (int)(merchantStock[i].SellWorth * merchantEntity.MERCHANT_PurchaseMarkup) + " tokens)");
                }

                if (isDragging == false && !scissorPane.Contains(controls.MousePosition))
                {
                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        if (merchantStock[i].itemRect.Contains(controls.MousePosition))
                        {
                            merchantStock[i].isSelected = true;
                            selectedItem = merchantStock[i];

                            screens.PlaySound("ButtonClick");
                        }
                        else
                            merchantStock[i].isSelected = false;
                    }
                }

                if (merchantStock[i].isSelected == true)
                    isSelected = true;
            }

            if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.I))
                merchantEntity.MERCHANT_SimulateTradeEconomy(1, Entities.NPC.Merchant.EconomyHealth.VeryHigh);
        }
        private void UpdateButtons(GameTime gt)
        {
            examineButton.Update(gt, controls);
            buyOneButton.Update(gt, controls);
            buyXButton.Update(gt, controls);
            buyAllButton.Update(gt, controls);

            examineButton.Position = new Point((int)position.X + 350, (int)position.Y + 236);
            buyOneButton.Position = new Point((int)position.X + 350, (int)position.Y + 257);
            buyXButton.Position = new Point((int)position.X + 350, (int)position.Y + 278);
            buyAllButton.Position = new Point((int)position.X + 350, (int)position.Y + 299);

            if (selectedItem != null)
            {
                if (examineButton.IsLeftClicked == true)
                {
                    screens.PlaySound("ButtonClick");
                }

                if (buyOneButton.IsLeftClicked == true)
                {
                    controlledEntity.STORAGE_AddItem(selectedItem.ID, 1, false, true);

                    int moneyRemoval = (int)(selectedItem.SellWorth * merchantEntity.MERCHANT_PurchaseMarkup);
                    controlledEntity.SKILL_RemoveEmbers(moneyRemoval);
                    merchantEntity.STORAGE_AddItem(1, moneyRemoval);

                    screens.PlaySound("GetCoins1");
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Paid " + moneyRemoval.CommaSeparation() + " tokens");

                    selectedItem.CurrentAmount -= 1;

                    if (selectedItem.CurrentAmount <= 0)
                        merchantStock.Remove(selectedItem);
                }

                if (buyXButton.IsLeftClicked == true)
                {
                    int amount = (int)(selectedItem.CurrentAmount * .1f);

                    controlledEntity.STORAGE_AddItem(selectedItem.ID, amount, false, true);

                    int moneyRemoval = (int)((amount * selectedItem.SellWorth) * merchantEntity.MERCHANT_PurchaseMarkup);
                    controlledEntity.SKILL_RemoveEmbers(moneyRemoval);
                    merchantEntity.STORAGE_AddItem(1, moneyRemoval);

                    screens.PlaySound("GetCoins2");
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Paid " + (selectedItem.SellWorth * amount).CommaSeparation() + " tokens");

                    selectedItem.CurrentAmount -= amount;

                    if (selectedItem.CurrentAmount <= 0)
                        merchantStock.Remove(selectedItem);
                }

                if (buyAllButton.IsLeftClicked == true)
                {
                    controlledEntity.STORAGE_AddItem(selectedItem.ID, selectedItem.CurrentAmount, false, true);

                    int moneyRemoval = (int)(selectedItem.StackWorth * merchantEntity.MERCHANT_PurchaseMarkup);
                    controlledEntity.SKILL_RemoveEmbers(moneyRemoval);
                    merchantEntity.STORAGE_AddItem(1, moneyRemoval);

                    screens.PlaySound("GetCoins3");
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Paid " + selectedItem.StackWorth.CommaSeparation() + " tokens");

                    selectedItem.CurrentAmount -= selectedItem.CurrentAmount;

                    if (selectedItem.CurrentAmount <= 0)
                        merchantStock.Remove(selectedItem);
                }
            }
        }

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private Rectangle dragArea;
        private void CheckDragScreen()
        {
            dragArea = new Rectangle((int)position.X + 89, (int)position.Y, 180, 20);

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
                paneLerp += 5f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                paneLerp -= 5f * (float)gt.ElapsedGameTime.TotalSeconds;

            paneLerp = MathHelper.Clamp(paneLerp, 0f, 1f);

            currentPaneWidth = MathHelper.SmoothStep(-3f, paneBG.Width, paneLerp);
        }

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(paneBG, new Rectangle((int)position.X + bg.Width - 10, (int)position.Y + 45, (int)currentPaneWidth, paneBG.Height), Color.White);
                sb.Draw(paneEnd, new Vector2(position.X + (bg.Width + currentPaneWidth) - 10, position.Y + 46), Color.White);

                sb.Draw(bg, position, Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.None, 0f);
                sb.DrawString(largeFont, "Merchant's Stock", new Vector2(position.X + bg.Center().X, position.Y + 13), ("Merchant's Stock").LineCenter(largeFont), ColorHelper.UI_Gold, 1f);

                sb.DrawMoneyQuantity(font, controlledEntity.SKILL_Embers(), " Embers", new Vector2(position.X + bg.Center().X, position.Y + 41), true);


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

                DrawGrid(sb);
                DrawPane(sb);
            }
        }

        private Rectangle scissorGrid, scissorPane;
        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        private void DrawGrid(SpriteBatch sb)
        {
            scissorGrid = new Rectangle((int)position.X + 18, (int)position.Y + 58, 324, 265);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorGrid;

            for (int i = 0; i < merchantStock.Count; i++)
            {
                if (merchantStock[i].isSelected == false)
                {
                    if (merchantStock[i].IsEntityItem == false)
                        sb.Draw(iconBG, merchantStock[i].itemRect, Color.White);
                    else
                        sb.Draw(iconBG, merchantStock[i].itemRect, Color.SkyBlue);
                }
                else
                    sb.Draw(iconBGSelect, merchantStock[i].itemRect, Color.White);

                sb.Draw(merchantStock[i].Icon, merchantStock[i].itemRect, Color.White);

                sb.DrawMoneyQuantity(font, merchantStock[i].CurrentAmount, new Vector2(merchantStock[i].itemRect.X + 4, merchantStock[i].itemRect.Y + 4));
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }
        private void DrawPane(SpriteBatch sb)
        {
            scissorPane = new Rectangle((int)position.X + 347, (int)position.Y + 32, (int)currentPaneWidth + 4, 301);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorPane;

            if (selectedItem != null)
            {
                sb.Draw(iconBG, new Vector2(scissorPane.X + 45, scissorPane.Y + 22), Color.White);
                sb.Draw(selectedItem.Icon, new Vector2(scissorPane.X + 45, scissorPane.Y + 22), Color.White);

                string test = ((int)(selectedItem.SellWorth * merchantEntity.MERCHANT_PurchaseMarkup)).CommaSeparation() + " Tokens";
                sb.DrawString(font, test, new Vector2(scissorPane.X + 80, scissorPane.Y + 170), test.LineCenter(font), Color.White, 1f);

                examineButton.DrawButton(sb, Color.White);
                buyOneButton.DrawButton(sb, Color.White);
                buyXButton.DrawButton(sb, Color.White);
                buyAllButton.DrawButton(sb, Color.White);

                sb.DrawString(font, "Examine", examineButton.Center + paneButton.Center(), "Examine".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Buy One", buyOneButton.Center + paneButton.Center(), "Buy One".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Buy 10%", buyXButton.Center + paneButton.Center(), "Buy 10%".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Buy All", buyAllButton.Center + paneButton.Center(), "Buy All".LineCenter(font), Color.White, 1f);
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }

        public bool IsUIClicking()
        {
            if (IsActive == true)
                return uiArea.Contains(controls.MousePosition) || scissorPane.Contains(controls.MousePosition);
            else
                return false;
        }

        public void SellItem(BaseItem item, int quantity)
        {
            merchantEntity.MERCHANT_AddItem(item.ID, quantity); //Transfer item
            item.CurrentAmount -= quantity;

            int payment = (int)((item.SellWorth * quantity) * merchantEntity.MERCHANT_QuicksellMarkdown); //Pay entity
            controlledEntity.SKILL_AddEmbers(payment);

            screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Sold " + quantity + " " + item.Name + " for " + payment.CommaSeparation() + " tokens");
        }

        public void ResetPosition()
        {
            Position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);
        }
    }
}
