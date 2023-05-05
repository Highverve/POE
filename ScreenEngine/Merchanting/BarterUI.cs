using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Entities.Types.NPE.NPC;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Performance;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.TileEngine;
using System;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.ScreenEngine.Merchanting
{
    public class BarterUI
    {
        private Vector2 position;
        private Rectangle uiContainer, dragArea;
        public bool IsClickingUI() { if (isBartering == true) return uiContainer.Contains(controls.MousePosition); else return false; }

        private Texture2D background, smallButton, smallButtonSelect, largeButton, largeButtonSelect,
                          acceptBarter, declineBarter, switchType, quicksell, pixel;
        private SpriteFont font, largeFont;

        private MenuButton acceptButton, declineButton, clearBlockedButton, quicksellButton;

        private List<BaseItem> merchantItems = new List<BaseItem>(), traderItems = new List<BaseItem>();
        private List<int> blockedItems = new List<int>();

        private int merchantItemsWorth = 0, traderItemsWorth = 0;
        public int MerchantWorth { get { return merchantItemsWorth; } }
        public int TraderWorth { get { return traderItemsWorth; } }

        private Controls controls = new Controls();
        private ScreenManager screens;
        private TileMap maps;

        private Random random;

        private bool isBartering = false;
        public bool IsBartering { get { return isBartering; } }

        private BaseItem lastItem = null;

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        public BarterUI() { random = new Random(Guid.NewGuid().GetHashCode()); }

        public void SetReferences(ScreenManager screens, TileMap maps) { this.screens = screens; this.maps = maps; }

        private string barterDir = "Interface/Barter/";
        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/RegularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            background = cm.Load<Texture2D>(barterDir + "bg");
            acceptBarter = cm.Load<Texture2D>(barterDir + "acceptBarter");
            declineBarter = cm.Load<Texture2D>(barterDir + "declineBarter");
            switchType = cm.Load<Texture2D>(barterDir + "switchType");
            quicksell = cm.Load<Texture2D>(barterDir + "quickSell");

            smallButton = cm.Load<Texture2D>("Interface/Global/buttonBG");
            smallButtonSelect = cm.Load<Texture2D>("Interface/Global/buttonBGHover");
            largeButton = cm.Load<Texture2D>("Interface/Global/iconBG");
            largeButtonSelect = cm.Load<Texture2D>("Interface/Global/iconBGSelect");

            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            pixel = cm.Load<Texture2D>("rect");

            acceptButton = new MenuButton(Vector2.Zero, smallButton, smallButtonSelect, smallButtonSelect, 1f);
            declineButton = new MenuButton(Vector2.Zero, smallButton, smallButtonSelect, smallButtonSelect, 1f);
            clearBlockedButton = new MenuButton(Vector2.Zero, smallButton, smallButtonSelect, smallButtonSelect, 1f);
            quicksellButton = new MenuButton(Vector2.Zero, smallButton, smallButtonSelect, smallButtonSelect, 1f);

            position = new Vector2(GameSettings.VectorCenter.X - background.Center().X, GameSettings.VectorCenter.Y - background.Height);
            mouseDragOffset = new Vector2(background.Center().X, 12);
        }

        private bool hasChanged = false; private string chatPhrase;
        private void SendPhrase(string phrase)
        {
            if (chatPhrase != phrase || !string.IsNullOrEmpty(phrase))
            {
                chatPhrase = phrase;
                hasChanged = true;
            }
        }
        private void UpdatePhrase()
        {
            if (hasChanged == true)
            {
                if (merchant.CHAT_Check(chatPhrase) == true)
                {
                    string line = merchant.CHAT_Retrieve(chatPhrase);

                    if (lastItem != null)
                    {
                        line = merchant.CHAT_VariableInjection(line, "%Item Name%", lastItem.Name);
                        line = merchant.CHAT_VariableInjection(line, "%Item Quantity%", lastItem.CurrentAmount.ToString());
                        line = merchant.CHAT_VariableInjection(line, "%Item Type%", lastItem.ItemType);
                        line = merchant.CHAT_VariableInjection(line, "%Item Subtype%", lastItem.ItemSubType);
                        line = merchant.CHAT_VariableInjection(line, "%Item Worth%", lastItem.SellWorth.ToString());
                        line = merchant.CHAT_VariableInjection(line, "%Item Stack Worth%", lastItem.StackWorth.ToString());
                    }

                    line = merchant.CHAT_VariableInjection(line, "%Merchant Stock%", merchant.MERCHANT_ItemStock.ToString());

                    merchant.CAPTION_SendImmediate(line);
                }

                hasChanged = false;
            }
        }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            if (isBartering == true)
            {
                //if (merchantItems.Count == 0 && traderItems.Count == 0)
                //    currentState = BarterState.NoItems;

                CheckDragScreen();

                UpdateButtons(gt);
                CheckButtonClicks(gt);

                UpdateTraderItems();
                UpdateMerchantItems(gt);

                ApplyProperItemGrid(merchantItems, new Vector2(position.X + 19, position.Y + 55));
                ApplyProperItemGrid(traderItems, new Vector2(position.X + 326, position.Y + 55));

                uiContainer = new Rectangle((int)position.X, (int)position.Y, background.Width, background.Height);

                hintRect = new Rectangle((int)position.X + 438, (int)position.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)position.X + 465, (int)position.Y, windowButton.Width - 20, windowButton.Height);

                if (hintRect.Contains(controls.MousePosition))
                {
                    isHintHover = true;

                    ToolTip.RequestStringAssign("Inventory Tips:\n\n" +
                            "Items can be dragged by left-clicking and dragging the cursor\n" +
                            "while over the item. With dragged item in hand, you can: \n\n" +
                            "1. Combine them with other items by right-clicking\n" +
                            "2. Gift them to other characters\n" +
                            "3. Use them in other UIs like the Stonehold or Bartering.\n\n" +
                            "Right-click an item then left-click a separate item to combine them.\n" +
                            "This enables combining items across different tabs.");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                    }
                }
                else
                    isHintHover = false;

                if (hideRect.Contains(controls.MousePosition))
                {
                    isHideHover = true;
                    ToolTip.RequestStringAssign("Hide Inventory");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Button Click 1");
                        isBartering = false;
                    }
                }
                else
                    isHideHover = false;

                UpdatePhrase();
            }


            controls.UpdateLast();
        }
        private void UpdateButtons(GameTime gt)
        {
            acceptButton.Update(gt, controls);
            acceptButton.Position = new Point((int)((position.X + background.Center().X) - smallButton.Center().X), (int)position.Y + 53);

            declineButton.Update(gt, controls);
            declineButton.Position = new Point((int)((position.X + background.Center().X) - smallButton.Center().X), (int)position.Y + 88);

            clearBlockedButton.Update(gt, controls);
            clearBlockedButton.Position = new Point((int)((position.X + background.Center().X) - smallButton.Center().X), (int)position.Y + 123);

            quicksellButton.Update(gt, controls);
            quicksellButton.Position = new Point((int)((position.X + background.Center().X) - smallButton.Center().X), (int)position.Y + 158);
        }
        private void CheckButtonClicks(GameTime gt)
        {
            if (acceptButton.IsHover == true)
                ToolTip.RequestStringAssign("Accept Barter");
            if (declineButton.IsHover == true)
                ToolTip.RequestStringAssign("Decline Barter");
            if (clearBlockedButton.IsHover == true)
                ToolTip.RequestStringAssign("Clear Blocked Items");
            if (quicksellButton.IsHover == true)
                ToolTip.RequestStringAssign("Quicksell Items");

            if (acceptButton.IsLeftClicked == true)
            {
                AcceptTrade();
                //screens.PlayRandom("GetCoins1", "GetCoins2", "GetCoins3");
            }

            if (declineButton.IsLeftClicked == true)
            {
                DeclineTrade();
                screens.PlaySound("Invalid");
                declineButton.IsLeftClicked = false;
            }

            if (clearBlockedButton.IsLeftClicked == true)
            {
                if (blockedItems.Count > 0)
                    SendPhrase("UnblockItems");
                else
                    SendPhrase("NoBlockedItems");

                blockedItems.Clear();
                clearBlockedButton.IsLeftClicked = false;
            }

            if (quicksellButton.IsLeftClicked == true)
            {
                Quicksell();
            }
        }

        private void ApplyProperItemGrid(List<BaseItem> list, Vector2 offset)
        {
            if (list.Count > 3)
            {
                for (int index = 0; index < list.Count; ++index)
                {
                    list[index].gridLocation = new Point(index % 4, (int)(index / 4));
                    list[index].UpdateRect(offset, (int)0f);
                }
            }
            else if (list.Count < 4)
            {
                for (int index = 0; index < list.Count; ++index)
                {
                    list[index].gridLocation = new Point(index, 0);
                    list[index].UpdateRect(offset, (int)0f);
                }
            }
        }
        private void UpdateTraderItems()
        {
            traderItemsWorth = 0;

            for (int i = 0; i < traderItems.Count; i++)
            {
                traderItemsWorth += traderItems[i].StackWorth;

                if (traderItems[i].itemRect.Contains(controls.MousePosition))
                {
                    if (GameSettings.IsDebugging == false)
                        ToolTip.RequestStringAssign(traderItems[i].Name + "\n\nLeft-click: Remove few\nCtrl + Left-click: Remove more\n\nRight-click: Remove all");
                    else
                        ToolTip.RequestStringAssign(traderItems[i].Name + Environment.NewLine + traderItems[i].StackWorth + " Tokens\nMerchant Desire: " + merchant.MERCHANT_GetItemDesire(traderItems[i]));

                    traderItems[i].isSelected = true;

                    if (controls.IsClickedOnce(Controls.MouseButton.RightClick) && screens.INVENTORY_IsItemDragging() == false)
                    {
                        RestoreItem(trader, traderItems[i], false);
                        screens.PlaySound("SwapItem");
                    }
                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick) && screens.INVENTORY_IsItemDragging() == false)
                    {
                        int quantity = 1;

                        if (traderItems[i].CurrentAmount > 999)
                            quantity = 10;
                        if (traderItems[i].CurrentAmount > 9999)
                            quantity = 100;
                        if (traderItems[i].CurrentAmount > 99999)
                            quantity = 1000;

                        if (controls.CurrentKey.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                        {
                            if (traderItems[i].CurrentAmount > 99)
                                quantity = 10;
                            if (traderItems[i].CurrentAmount > 999)
                                quantity = 100;
                            if (traderItems[i].CurrentAmount > 9999)
                                quantity = 1000;
                            if (traderItems[i].CurrentAmount > 99999)
                                quantity = 5000;
                        }

                        RestoreItem(trader, traderItems[i], quantity);
                        screens.PlaySound("SwapItem");
                    }
                }
                else
                    traderItems[i].isSelected = false;

                if (traderItems[i].CurrentAmount == 0)
                    traderItems.Remove(traderItems[i]);

            }
        }

        CallLimiter limitMerchant = new CallLimiter(750);
        private void UpdateMerchantItems(GameTime gt)
        {
            merchantItemsWorth = 0;

            for (int i = 0; i < merchantItems.Count; i++)
            {
                merchantItemsWorth += merchantItems[i].StackWorth;

                if (merchantItems[i].itemRect.Contains(controls.MousePosition))
                {
                    if (GameSettings.IsDebugging == false)
                        ToolTip.RequestStringAssign(merchantItems[i].Name);
                    else
                        ToolTip.RequestStringAssign(merchantItems[i].Name + Environment.NewLine + merchantItems[i].StackWorth + " Tokens");

                    merchantItems[i].isSelected = true;

                    if (controls.IsClickedOnce(Controls.MouseButton.RightClick) && screens.INVENTORY_IsItemDragging() == false)
                    {
                        if (!blockedItems.Contains(merchantItems[i].ID))
                        {
                            blockedItems.Add(merchantItems[i].ID);
                            SendPhrase("PlayerBlockItem");
                            //RestoreItem(merchant, merchantItems[i]);
                        }
                    }
                }
                else
                    merchantItems[i].isSelected = false;

                for (int j = 0; j < blockedItems.Count; j++)
                {
                    if (blockedItems[j] == merchantItems[i].ID)
                        RestoreItem(merchant, merchantItems[i], false);
                }

                if (merchantItems[i].CurrentAmount == 0)
                    merchantItems.Remove(merchantItems[i]);
            }

            if (merchantItemsWorth < (traderItemsWorth * merchant.MERCHANT_TradeMarkdown))
            {

                if (merchant.MERCHANT_ItemCount > 0 && IsMerchantWindowFull() == false)
                {
                    if (limitMerchant.IsCalling(gt))
                    {
                        BaseItem item = merchant.MERCHANT_GetTradeItem((int)(traderItemsWorth * merchant.MERCHANT_TradeMarkdown) - merchantItemsWorth);

                        if (item != null)
                        {
                            bool isBlocked = false;
                            for (int i = 0; i < blockedItems.Count; i++)
                            {
                                if (blockedItems[i] == item.ID)
                                {
                                    isBlocked = true;
                                }
                            }

                            if (isBlocked == false)
                            {
                                int subtractQuantity = 0;
                                MERCHANT_AddItem(item.ID, item.CurrentAmount, out subtractQuantity);
                                item.CurrentAmount -= subtractQuantity;
                                //merchant.MERCHANT_RemoveItem(item.ID, subtractQuantity);

                                screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                                limitMerchant.ChangeTime(random.Next(150, 550));

                                if (merchant.MERCHANT_ItemCount == 0)
                                    SendPhrase("EmptyStock");
                            }
                        }
                    }
                }
            }
            else if (merchantItemsWorth > (traderItemsWorth * merchant.MERCHANT_TradeMarkdown))
            {
                if (limitMerchant.IsCalling(gt))
                {
                    RestoreItem(merchant, merchantItems[random.Next(0, merchantItems.Count)], false);
                    limitMerchant.ChangeTime(random.Next(100, 400));
                }
            }
        }

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private void CheckDragScreen()
        {
            dragArea = new Rectangle((int)position.X + 169, (int)position.Y, 267, 20);

            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                position = controls.MouseVector - mouseDragOffset;
                screens.SetCursorState(Cursor.CursorState.Move);
            }
        }

        string merchantName = "Merchant: ";
        public void Draw(SpriteBatch sb)
        {
            if (isBartering == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(background, position, Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.None, 0f);
                sb.DrawStringBordered(largeFont, "Bartering", new Vector2(position.X + background.Center().X, position.Y + 13), "Bartering".LineCenter(largeFont), 0f, 1f, 1f, ColorHelper.UI_Gold, Color.Black);


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


                //string msg = merchant.MERCHANT_Message.ToString();
                //float offset = (font.MeasureString(msg).X / 2);
                //float nameOffset = (font.MeasureString(merchantName).X / 2);

                //sb.DrawString(font, msg, new Vector2(position.X + background.Center().X + nameOffset, position.Y + 37), msg.LineCenter(font), Color.White, 1f);
                //sb.DrawString(font, merchantName, new Vector2(position.X + background.Center().X - offset, position.Y + 37), merchantName.LineCenter(font), ColorHelper.UI_Money, 1f);

                DrawButtons(sb);

                DrawItemGrid(sb, merchantItems, merchant is PlayerEntity);
                DrawItemGrid(sb, traderItems, trader is PlayerEntity);

                DrawFavorability(sb);

                sb.End();
            }
        }
        private void DrawButtons(SpriteBatch sb)
        {
            acceptButton.DrawButton(sb, Color.White);
            sb.Draw(acceptBarter, acceptButton.Position.ToVector2(), Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.None, 1f);

            declineButton.DrawButton(sb, Color.White);
            sb.Draw(declineBarter, declineButton.Position.ToVector2(), Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.None, 1f);

            clearBlockedButton.DrawButton(sb, Color.White);
            sb.Draw(switchType, clearBlockedButton.Position.ToVector2(), Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.None, 1f);

            quicksellButton.DrawButton(sb, Color.White);
            sb.Draw(quicksell, quicksellButton.Position.ToVector2(), Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.None, 1f);
        }
        private void DrawItemGrid(SpriteBatch sb, List<BaseItem> list, bool isPlayer)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].isSelected == false)
                    sb.Draw(largeButton, list[i].itemRect, Color.White);
                else
                    sb.Draw(largeButtonSelect, list[i].itemRect, Color.White);

                if (isPlayer)
                {
                    if (merchant.MERCHANT_IsAcceptableItem(list[i].ID) == true)
                        sb.Draw(list[i].Icon, list[i].itemRect.Location.ToVector2(), Color.White);
                    else
                        sb.Draw(list[i].Icon, list[i].itemRect.Location.ToVector2(), Color.Firebrick);
                }
                else
                    sb.Draw(list[i].Icon, list[i].itemRect.Location.ToVector2(), Color.White);

                sb.DrawMoneyQuantity(font, list[i].CurrentAmount, new Vector2(list[i].itemRect.Location.X + 4, list[i].itemRect.Location.Y + 4));
            }
        }

        private Color traderFavor = Color.Lerp(Color.Green, Color.Transparent, .35f), merchantFavor = Color.Lerp(Color.Red, Color.Transparent, .35f);
        private void DrawFavorability(SpriteBatch sb)
        {
            int barWidth = 578;

            if (merchantItemsWorth > 0 && traderItemsWorth > 0)
            {
                int traderProfit = barWidth * merchantItemsWorth / (merchantItemsWorth + traderItemsWorth);
                int merchantProfit = barWidth * traderItemsWorth / (merchantItemsWorth + traderItemsWorth);

                sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 13, (int)position.Y + 264, merchantProfit, 4), merchantFavor, Color.Black);
                sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 13 + merchantProfit, (int)position.Y + 264, traderProfit, 4), traderFavor, Color.Black);
            }
        }

        public void BeginBartering()
        {
            if (merchant != null && trader != null)
                isBartering = true;
        }

        public void MERCHANT_AddItem(int id, int quantity, out int subtractQuantity)
        {
            AddItem(id, quantity, out subtractQuantity, merchant, merchantItems);
        }
        public void TRADER_AddItem(int id, int quantity, out int subtractQuantity)
        {
            AddItem(id, quantity, out subtractQuantity, trader, traderItems);
        }

        private void AddItem(int id, int quantity, out int subtractQuantity, BaseEntity currentEntity, List<BaseItem> items)
        {
            BaseItem item = ItemDatabase.Item(id).Copy(this.screens, this.maps, currentEntity, null);
            item.CurrentAmount = quantity;

            bool containsItem = false;
            subtractQuantity = 0;

            if (currentEntity is PlayerEntity)
            {
                lastItem = item;

                string response = null;

                if (merchant.CHAT_Check("PlayerAddItem [ID:" + id + "]"))
                    response = "PlayerAddItem [ID:" + id + "]";
                if (string.IsNullOrEmpty(response) && merchant.CHAT_Check("PlayerAddItem [TYPE:" + item.ItemType + "]"))
                    response = "PlayerAddItem [TYPE:" + item.ItemType + "]";
                if (string.IsNullOrEmpty(response) && merchant.CHAT_Check("PlayerAddItem [SUBTYPE:" + item.ItemSubType + "]"))
                    response = "PlayerAddItem [TYPE:" + item.ItemSubType + "]";
                if (string.IsNullOrEmpty(response))
                    response = "PlayerAddItem";

                SendPhrase(response);
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    containsItem = true;

                    if (items[i].CurrentAmount >= items[i].MaxAmount)
                        containsItem = false; //Set to false to start a new stack of items
                    else
                    {
                        int roomFor = items[i].MaxAmount - items[i].CurrentAmount; //How many more items can be merged until the stack is full
                        int transferQuantity = (int)Math.Min(quantity, roomFor);

                        item.CurrentAmount = quantity - transferQuantity;
                        items[i].CurrentAmount += transferQuantity;
                        subtractQuantity = transferQuantity;

                        if (item.CurrentAmount > 0)
                            containsItem = false;
                    }
                }
            }

            if (containsItem == false)
            {
                if (IsTraderWindowFull() == false)
                {
                    items.Add(item);
                    subtractQuantity += item.CurrentAmount;
                }
            }
        }

        private void AcceptTrade()
        {
            if (traderItems.Count > 0)
            {
                if (merchant.MERCHANT_IsAcceptableTrade(merchantItemsWorth, traderItemsWorth) == true)
                {
                    bool isAllItemsAccepted = true;

                    for (int i = 0; i < traderItems.Count; i++)
                    {
                        if (merchant.MERCHANT_IsAcceptableItem(traderItems[i].ID) == false)
                            isAllItemsAccepted = false;
                    }

                    if (isAllItemsAccepted == true)
                    {
                        SendPhrase("AcceptedTrade");

                        for (int i = 0; i < traderItems.Count; i++)
                            RestoreItem(merchant, traderItems[i], (merchant.IsPlayerControlled));

                        for (int i = 0; i < merchantItems.Count; i++)
                            RestoreItem(trader, merchantItems[i], (trader.IsPlayerControlled));

                        traderItems.Clear();
                        merchantItems.Clear();

                        screens.PlayRandom("GetCoins1", "GetCoins2", "GetCoins3");
                    }
                    else
                    {
                        SendPhrase("MerchantBlockItem");
                        screens.PlaySound("Invalid");
                    }
                }
                else
                {
                    SendPhrase("UnfairTrade");
                    screens.PlaySound("Invalid");
                }
            }
            else if (merchantItems.Count <= 0 && traderItems.Count <= 0)
            {
                SendPhrase("NoItemsAccept");
                screens.PlaySound("Invalid");
            }
        }
        private void DeclineTrade()
        {
            SendPhrase("PlayerDeclinedTrade");
            ForceEndBarter();
        }
        private void Quicksell()
        {
            if (traderItems.Count > 0)
            {
                bool isAllItemsAccepted = true;

                for (int i = 0; i < traderItems.Count; i++)
                {
                    if (merchant.MERCHANT_IsAcceptableItem(traderItems[i].ID) == false)
                        isAllItemsAccepted = false;
                }

                if (isAllItemsAccepted == true)
                {
                    SendPhrase("PlayerQuicksell");

                    RestoreMerchant();

                    //Apply no quicksell markdown on tokens when quickselling.
                    int valueMinusTokens = traderItemsWorth;
                    int tokenValue = 0;

                    for (int i = 0; i < traderItems.Count; i++)
                    {
                        if (traderItems[i].ID == 1)
                        {
                            valueMinusTokens -= traderItems[i].CurrentAmount;
                            tokenValue += traderItems[i].CurrentAmount;
                        }
                    }

                    int subtractItem = 0;
                    MERCHANT_AddItem(1, (int)(valueMinusTokens * merchant.MERCHANT_QuicksellMarkdown) + tokenValue, out subtractItem); //No need to do anything with subtracting, since it's just quickselling.


                    for (int i = 0; i < traderItems.Count; i++)
                        RestoreItem(merchant, traderItems[i], (merchant.IsPlayerControlled));

                    for (int i = 0; i < merchantItems.Count; i++)
                        RestoreItem(trader, merchantItems[i], (trader.IsPlayerControlled));

                    traderItems.Clear();
                    merchantItems.Clear();

                    screens.PlayRandom("GetCoins1", "GetCoins2", "GetCoins3");
                }
                else
                {
                    SendPhrase("MerchantBlockItem");
                    screens.PlaySound("Invalid");
                }
            }
            else
            {
                SendPhrase("NoItemsQuicksell");
                screens.PlaySound("Invalid");
            }
        }
        private int QuicksellValue() { return (int)(traderItemsWorth * merchant.MERCHANT_QuicksellMarkdown); }

        public void RestoreAll()
        {
            for (int i = 0; i < traderItems.Count; i++)
                RestoreItem(trader, traderItems[i], false);

            for (int i = 0; i < merchantItems.Count; i++)
                RestoreItem(merchant, merchantItems[i], false);

            traderItems.Clear();
            merchantItems.Clear();
        }
        public void RestoreMerchant()
        {
            for (int i = 0; i < merchantItems.Count; i++)
                RestoreItem(merchant, merchantItems[i], false);

            merchantItems.Clear();
        }
        public void RestoreTrader()
        {
            for (int i = 0; i < traderItems.Count; i++)
                RestoreItem(trader, traderItems[i], false);

            traderItems.Clear();
        }
        public void RestoreItem(BaseEntity party, BaseItem item, bool displayNotif)
        {
            if (party is CharacterEntity && party.MERCHANT_IsMerchant())
                party.MERCHANT_AddItem(item.ID, item.CurrentAmount);
            else
                party.STORAGE_AddItem(item.ID, item.CurrentAmount, false, (party.IsPlayerControlled));

            item.CurrentAmount = 0;
        }
        public void RestoreItem(BaseEntity party, BaseItem item, int quantity)
        {
            if (party is CharacterEntity && party.MERCHANT_IsMerchant())
                party.MERCHANT_AddItem(item.ID, quantity);
            else
                party.STORAGE_AddItem(item.ID, quantity, false, (party.IsPlayerControlled));

            item.CurrentAmount -= quantity;
        }

        private BaseEntity merchant, trader;
        public void SetMerchant(BaseEntity merchant)
        {
            this.merchant = merchant;
            merchantName = merchant.Name + ": ";

            SendPhrase("BeginTrade");
        }
        public void SetTrader(BaseEntity trader) { this.trader = trader; }

        public bool IsTraderWindowFull() { return (traderItems.Count >= 12); }
        public bool IsMerchantWindowFull() { return (merchantItems.Count >= 12); }

        public void ForceEndBarter()
        {
            RestoreAll(); //return all items to the original parties.

            traderItems.Clear();
            merchantItems.Clear();

            blockedItems.Clear();

            isBartering = false;
        }

        public void ResetPosition()
        {
            position = new Vector2(GameSettings.VectorCenter.X - (background.Width / 2), GameSettings.VectorCenter.Y - background.Height / 2);
        }
    }
}
