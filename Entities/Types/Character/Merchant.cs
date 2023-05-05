using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Notification;
using Pilgrimage_Of_Embers.TileEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pilgrimage_Of_Embers.Entities.NPC
{
    public class Merchant
    {
        private List<BaseItem> stock = new List<BaseItem>(), stockPool = new List<BaseItem>();
        public List<BaseItem> Stock { get { return stock; } }
        public int StockCount { get { return stock.Count; } }
        private int totalItems;
        public int TotalItems { get { return totalItems; } }

        List<Point> itemPoints = new List<Point>(), constantPoints = new List<Point>(), itemsPool = new List<Point>(), globalPool = new List<Point>(); //"globalPool" will have select unique items from all around the game world and will be rarely added to the creature's merchant stock. This will give a greater illusion of item trading.
        private List<int> blockedItems = new List<int>();
        private List<Point4> uniqueItems = new List<Point4>(); //Item for item. If the player puts in a unique item that the merchant really wants, they will 'swap' items.
                                                               //E.G, player has a rare amulet. Merchant checks item, then checks uniqueItems list for id and quantity. If id matches, merchant adds their item (id).

        private float tradePct, quicksellPct, markupPct;
        public float TradeMarkdown { get { return tradePct; } }
        public float QuickSellMarkdown { get { return quicksellPct; } }
        public float PurchaseMarkup { get { return markupPct; } }

        private ScreenManager screens;
        private TileMap maps;
        private BaseEntity merchant;
        private Camera camera;

        private Random random;

        public Merchant(List<Point> ConstantItems, List<Point> StartingItems, List<int> BlockedItems, List<Point> ItemsPool,
                        float TradePct, float QuickSellPct, float PurchaseMarkUp)
        {
            this.constantPoints = ConstantItems;
            itemPoints = StartingItems;
            blockedItems = BlockedItems;
            itemsPool = ItemsPool;

            this.tradePct = TradePct;
            this.quicksellPct = QuickSellPct;
            this.markupPct = PurchaseMarkUp;

            random = new Random(Guid.NewGuid().GetHashCode());

            //uniqueItems.Add(new Point4(1, 1000, 7002, 1));
        }
        public void SetReferences(ScreenManager screens, TileMap maps, BaseEntity merchant, Camera camera)
        {
            this.screens = screens;
            this.maps = maps;
            this.merchant = merchant;
            this.camera = camera;
        }

        private void GenericMapParser(string file, string tag, Action<string> Parse)
        {
            bool isReadingTag = false;

            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line.Trim()))
                        continue;

                    if (line.ToUpper().Contains("[" + tag.ToUpper() + "]")) //Reading Switches
                        isReadingTag = true;
                    else if (line.ToUpper().Contains("[/" + tag.ToUpper() + "]")) //Stop reading switches
                        isReadingTag = false;

                    if (isReadingTag == true)
                    {
                        line = line.InjectRandoms(random);
                        Parse(line);
                    }
                }
            }
        }

        public void AddPoints()
        {
            for (int i = 0; i < itemPoints.Count; i++)
                AddItem(itemPoints[i].X, itemPoints[i].Y, false, false);
        }
        public void AddItem(int id, int quantity, bool isProtected, bool displayNotification)
        {
            BaseItem item = ItemDatabase.Item(id).Copy(screens, maps, merchant, camera);

            if (id != 1)
            {
                bool isConstantStock = false;
                for (int i = 0; i < constantPoints.Count; i++)
                {
                    if (constantPoints[i].X == id)
                    {
                        isConstantStock = true;
                        break;
                    }
                }

                AddItemToTab(id, quantity, item, stock, isConstantStock, displayNotification);
            }
            else
                merchant.STORAGE_AddItem(id, quantity, true, false); //If the item being added is money, bypass adding to stock and add directly to merchant's storage (A.K.A, personal items).

            SortItemsByTab();
        }
        private void AddItemToTab(int id, int quantity, BaseItem item, List<BaseItem> itemData, bool isProtected, bool displayNotification)
        {
            bool containsItem = false;
            int leftoverAmount = 0;

            for (int i = 0; i < itemData.Count; i++)
            {
                if (itemData[i].ID == id && itemData[i].CurrentAmount < itemData[i].MaxAmount)
                {
                    containsItem = true;

                    leftoverAmount = (itemData[i].CurrentAmount + quantity) - itemData[i].MaxAmount;
                    itemData[i].CurrentAmount += quantity;

                    if (displayNotification == true)
                        screens.NOTIFICATION_Add(NotificationManager.IconType.Inventory, "Added " + quantity.ToString() + " " + itemData[i].Name);

                    if (leftoverAmount > 0)
                    {
                        itemData.Add(item.Copy(screens, maps, merchant, camera));
                        itemData.Last().CurrentAmount = leftoverAmount;
                        itemData.Last().IsEntityItem = isProtected;
                    }

                    break;
                }
            }

            if (containsItem == false)
            {
                itemData.Add(item.Copy(screens, maps, merchant, camera));
                itemData.Last().CurrentAmount = quantity;
                itemData.Last().IsEntityItem = isProtected;

                if (displayNotification == true)
                    screens.NOTIFICATION_Add(NotificationManager.IconType.Inventory, "Added " + quantity.ToString() + " " + itemData.Last().Name);
            }
        }

        public void UpdateRemoval()
        {
            for (int i = 0; i < stock.Count; i++)
            {
                if (stock[i].CurrentAmount <= 0)
                    stock.Remove(stock[i]);
            }
        }

        // [Methods] Trading
        public bool MerchantHasItem(int id)
        {
            for (int i = 0; i < stock.Count; i++)
                if (stock[i].ID == id)
                    return true;

            return false;
        }
        public float GetItemDesire(BaseItem item)
        {
            float desire = 0f; //Based on a 100-point system.

            bool containsItem = false, isDamaged = false;
            int inStock = 0, maxStock = 0;
            float damagePct = 0f;

            for (int i = 0; i < stock.Count; i++)
            {
                if (stock[i].ID == item.ID)
                {
                    containsItem = true;
                    inStock = stock[i].CurrentAmount;
                    maxStock = stock[i].MaxAmount;

                    isDamaged = !(item.CurrentDurability >= item.MaxDurability);
                    damagePct = item.CurrentDurability / item.MaxDurability;

                    break;
                }
            }

            if (containsItem == true)
            {
                int itemDifference = item.CurrentAmount - inStock;
                float multiPct = itemDifference / maxStock;
                desire += 50f * multiPct;
            }
            else
                desire += 65f;

            if (isDamaged == true)
            {
                desire -= 30f * (1f - damagePct);
            }
            else
                desire += 15f;

            return MathHelper.Clamp(desire, 0f, 100f);
        }
        public int GetTokenValue(BaseItem item)
        {
            int tokenValue = item.SellWorth * (item.CurrentDurability / item.MaxDurability); //Only subtract the value by the selling price times durability of the first item on the stack.
            return tokenValue;
        }
        public int GetStackValue(BaseItem item)
        {
            int tokenValue = item.StackWorth;
            tokenValue -= item.SellWorth * item.CurrentDurability / item.MaxDurability; //Only subtract the value by the selling price times durability of the first item on the stack.

            return tokenValue;            
        }
        public int GetStackValue(BaseItem item, int customQuantity)
        {
            int tokenValue = item.SellWorth * customQuantity;
            tokenValue -= item.SellWorth * item.CurrentDurability / item.MaxDurability; //Only subtract the value by the selling price times durability of the first item on the stack.

            return tokenValue;
        }

        public BaseItem GetTradeItem(int maxPriceDifference)
        {
            BaseItem[] potentialItems = new BaseItem[15];

            int index = 0;

            totalItems = 0;

            for (int i = 0; i < stock.Count; i++)
            {
                totalItems += stock[i].CurrentAmount;

                if (stock[i].CurrentAmount > 0)
                {
                    if (GetStackValue(stock[i]) < maxPriceDifference)
                    {
                        //int maxQuantity = (int)MathHelper.Clamp(maxPriceDifference / GetTokenValue(stock[i]), 1, stock[i].CurrentAmount);
                        //int transferQuantity = maxQuantity;//random.Next(1, maxQuantity);

                        potentialItems[index] = stock[i];
                        //potentialItems[index].CurrentAmount = 0 + transferQuantity;

                        index++;

                        if (index >= potentialItems.Length - 1)
                            break;
                    }
                }
            }

            BaseItem item = potentialItems[random.Next(0, index)];

            return item;
        }
        public bool IsAcceptableTrade(int merchantPrice, int traderPrice)
        {
            if (merchantPrice < (traderPrice * (tradePct * 1.1f)))
                return true;
            else
                return false;
        }
        public bool IsAcceptableItem(int id)
        {
            bool value = true;

            for (int i = 0; i < blockedItems.Count; i++)
            {
                if (blockedItems[i] == id)
                {
                    value = false;
                    break;
                }
            }

            return value;
        }

        private StringBuilder merchantText = new StringBuilder();
        public StringBuilder MerchantText { get { return merchantText; } }

        public StringBuilder SaveMerchantData(Culture.CultureManager culture)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("DateStamp " + culture.DateStamp());
            for (int i = 0; i < stock.Count; i++)
            {
                builder.AppendLine(stock[i].SaveData());
            }

            return builder;
        }
        public void LoadMerchantData(List<string> data, Culture.CultureManager culture)
        {
            if (data.Count > 0)
                stock.Clear(); //I guess!

            Point4 date = new Point4();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToUpper().StartsWith("DATESTAMP"))
                {
                    data[i] = data[i].Replace("DATESTAMP ", "");
                    date = culture.ParseDateStamp(data[i]);
                }
                else
                {
                    try
                    {
                        string[] words = data[i].Split(' ');

                        int id = int.Parse(words[1]);
                        int quantity = int.Parse(words[2]);

                        AddItem(id, quantity, false, false);
                        stock.Last().LoadData(data[i]);
                    }
                    catch (Exception e)
                    {
                        //debug.OutputError("ERROR: Bad data line(SoulData): " + e.Message);
                    }
                }
            }

            SimulateTradeEconomy(culture.DistanceInDays(date, culture.CurrentDateStamp()), EconomyHealth.Medium);
        }

        //Economy simulation.
        public enum EconomyHealth { VeryLow, Low, Medium, High, VeryHigh }
        public void SimulateTradeEconomy(int iterations, EconomyHealth economyHealth)
        {
            int dailyIterations = 3;

            switch (economyHealth)
            {
                case EconomyHealth.VeryLow: dailyIterations = 4; break;
                case EconomyHealth.Low: dailyIterations = 8; break;
                case EconomyHealth.Medium: dailyIterations = 12; break;
                case EconomyHealth.High: dailyIterations = 16; break;
                case EconomyHealth.VeryHigh: dailyIterations = 20; break;
            }

            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < dailyIterations; j++)
                {
                    if (stock.Count > 0)
                    {
                        float whatDo = random.NextFloat(0, 100); //Decide what do!

                        if (whatDo >= 99.5f)
                        {
                            if (globalPool.Count > 0)
                            {
                                BaseItem item = GrabFromGlobalPool(random.Next(0, globalPool.Count));
                                AddItem(item.ID, random.Next(1, item.CurrentAmount), false, false);
                            }
                        }
                        else if (whatDo >= 95 && whatDo < 99.5f) //5% chance to add an item from the item pool.
                        {
                            BaseItem item = GrabFromItemPool(random.Next(0, itemsPool.Count));
                            AddItem(item.ID, random.Next(1, item.CurrentAmount), false, false);
                        }
                        else if (whatDo < 95 && whatDo > 40) //75% chance to do a range positive or negative alteration to item quantity
                        {
                            int index = random.Next(0, stock.Count);
                            AdjustStockQuantity(index, random.Next(-(int)(MathHelper.Clamp(stock[index].CurrentAmount / 4, -1, -100)),
                                                                    (int)MathHelper.Clamp(stock[index].CurrentAmount / 4, 1, 100)));
                        }
                        else if (whatDo <= 40 && whatDo > 20)
                        {
                            BaseItem item = GrabFromConstantPool(random.Next(0, constantPoints.Count));

                            bool containsItem = false;
                            for (int s = 0; s < stock.Count; s++)
                            {
                                if (stock[s].ID == item.ID)
                                {
                                    containsItem = true;
                                    break;
                                }
                            }

                            if (containsItem == false)
                            {
                                stock.Add(item); //Only add the item to the database if the stock doesn't already have one of the same item ID.
                                stock.Last().IsEntityItem = true;
                            }
                        }
                        else if (whatDo <= 20 && whatDo > 3f) //19.5% chance to subtract item quantity by a potentially bigger number.
                        {
                            int index = random.Next(0, stock.Count);
                            AdjustStockQuantity(index, random.Next(-1, (int)MathHelper.Clamp(-stock[index].CurrentAmount, -1, -100)));
                        }
                        else if (whatDo <= 3f) //0.5% chance to remove all of the item.
                        {
                            BaseItem item = stock[random.Next(0, stock.Count)];

                            if (item.IsEntityItem == false) //Don't remove items from constant pool
                                item.CurrentAmount -= item.MaxAmount;
                        }
                    }
                    else
                    {
                        BaseItem item = GrabFromItemPool(random.Next(0, itemsPool.Count));
                        AddItem(item.ID, random.Next(1, item.CurrentAmount), false, false);
                    }
                }
            }

            MergeItemStacks(); //Reduce inventory clutter by merging two item stacks that are both less than max amount.
        }

        private void AdjustStockQuantity(int index, int amount)
        {
            if (stock[index].IsEntityItem == false)
            {
                stock[index].CurrentAmount += amount;

                if (stock[index].ID == 2)
                {
                }
            }
            else
            {
                if (amount > 0) //if amount is increasing ...
                {
                    stock[index].CurrentAmount += amount;
                    CapConstantStock(stock[index].ID);
                }
            }
        }
        private BaseItem GrabFromItemPool(int index)
        {
            BaseItem item = ItemDatabase.Item(itemsPool[index].X);
            item.CurrentAmount = random.Next(1, (int)MathHelper.Clamp(itemsPool[index].Y, 1, 999));

            return item;
        }
        private BaseItem GrabFromConstantPool(int index)
        {
            BaseItem item = ItemDatabase.Item(constantPoints[index].X).Copy(screens, maps, merchant, camera);
            item.CurrentAmount = random.Next(1, (int)MathHelper.Clamp(constantPoints[index].Y, 1, 999));
            item.IsEntityItem = true;

            CapConstantStock(item.ID);

            return item;
        }
        private void CapConstantStock(int id)
        {
            for (int i = 0; i < stock.Count; i++)
            {
                for (int j = 0; j < constantPoints.Count; j++)
                {
                    if (stock[i].ID == constantPoints[j].X)
                    {
                        if (stock[i].CurrentAmount > constantPoints[j].Y)
                            stock[i].CurrentAmount = constantPoints[j].Y; //Cap constant item off.
                    }
                }
            }
        }
        private BaseItem GrabFromGlobalPool(int index)
        {
            BaseItem item = ItemDatabase.Item(globalPool[index].X);
            item.CurrentAmount = random.Next(1, (int)MathHelper.Clamp(globalPool[index].Y, 1, 99));

            return item;
        }

        private void MergeItemStacks()
        {
            for (int i = 0; i < stock.Count; i++)
            {
                for (int j = i + 1; j < stock.Count; j++)
                {
                    stock[i].MergeItemStacks(stock[j]);

                    if (stock[i].CurrentAmount <= 0)
                        stock.Remove(stock[i]);
                }
            }
        }

        public void SortItemsByTab()
        {
            stock.Sort((s1, s2) => s1.MerchantCompareTo(s2));
        }
    }
}
