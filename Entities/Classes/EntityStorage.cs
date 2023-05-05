using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.Souls.Types;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Souls;
using Pilgrimage_Of_Embers.ScreenEngine.Notification;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook;
using System;
using System.IO;

namespace Pilgrimage_Of_Embers.Entities
{
    public class EntityStorage
    {
        List<BaseItem> consumablesList = new List<BaseItem>();
        List<BaseItem> weaponsList = new List<BaseItem>();
        List<BaseItem> armorList = new List<BaseItem>();
        List<BaseItem> ammoList = new List<BaseItem>();
        List<BaseItem> jewelleryList = new List<BaseItem>();
        List<BaseItem> resourcesList = new List<BaseItem>();
        List<BaseItem> miscellaneousList = new List<BaseItem>();

        public List<BaseItem> TotalItems()
        {
            List<BaseItem> items = new List<BaseItem>();

            items.AddRange(consumablesList);
            items.AddRange(weaponsList);
            items.AddRange(armorList);
            items.AddRange(ammoList);
            items.AddRange(jewelleryList);
            items.AddRange(resourcesList);
            items.AddRange(miscellaneousList);

            return items;
        }

        List<Point> itemPoints;

        List<BaseSpell> spells = new List<BaseSpell>();
        public List<BaseSpell> Spells { get { return spells; } }
        public BaseSpell SpellByID(int id)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i].ID == id)
                    return spells[i];
            }

            return null;
        }

        List<BaseSoul> souls = new List<BaseSoul>();
        public List<BaseSoul> Souls { get { return souls; } }

        private ScreenManager screens;
        private TileMap tileMap;
        private BaseEntity currentEntity;
        private Camera camera;

        /// <summary>
        /// </summary>
        /// <param name="Items">X == ID, Y == Quantity</param>
        /// <param name="Souls">Value == ID of soul. Maximum of 4, excess will be removed.</param>
        public EntityStorage(List<Point> Items)
        {
            itemPoints = Items;
        }
        public EntityStorage(string StateFilePath)
        {
            stateFilePath = StateFilePath;
            LoadFromFile(stateFilePath);
        }
        public EntityStorage() { }

        public void SetReferences(ScreenManager Screens, TileMap Maps, BaseEntity CurrentEntity, Camera camera)
        {
            screens = Screens;
            tileMap = Maps;
            currentEntity = CurrentEntity;
            this.camera = camera;
        }
        public void AddPoints()
        {
            for (int i = 0; i < itemPoints.Count; i++)
            {
                AddItem(itemPoints[i].X, itemPoints[i].Y, false, false);
            }
        }
        public void AddItem(int id, int quantity, bool isProtected, bool displayNotification)
        {
            BaseItem item = ItemDatabase.Item(id);

            int blah = 0;

            if (item != null)
            {
                item = item.Copy(screens, tileMap, currentEntity, camera);

                switch (item.tabType)
                {
                    case BaseItem.TabType.Consumables: AddItemToTab(id, quantity, out blah, item, consumablesList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Weapons: AddItemToTab(id, quantity, out blah, item, weaponsList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Armor: AddItemToTab(id, quantity, out blah, item, armorList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Ammo: AddItemToTab(id, quantity, out blah, item, ammoList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Jewellery: AddItemToTab(id, quantity, out blah, item, jewelleryList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Resources: AddItemToTab(id, quantity, out blah, item, resourcesList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Miscellaneous: AddItemToTab(id, quantity, out blah, item, miscellaneousList, isProtected, displayNotification); break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <param name="quantityAdded"></param>
        /// <param name="isProtected"></param>
        /// <param name="displayNotification"></param>
        /// <returns>Returns the total quantity added.</returns>
        public int AddItemGetDifference(int id, int quantity, bool isProtected, bool displayNotification)
        {
            int quantityAdded = 0;
            BaseItem item = ItemDatabase.Item(id);

            if (item != null)
            {
                switch (item.tabType)
                {
                    case BaseItem.TabType.Consumables: AddItemToTab(id, quantity, out quantityAdded, item, consumablesList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Weapons: AddItemToTab(id, quantity, out quantityAdded, item, weaponsList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Armor: AddItemToTab(id, quantity, out quantityAdded, item, armorList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Ammo: AddItemToTab(id, quantity, out quantityAdded, item, ammoList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Jewellery: AddItemToTab(id, quantity, out quantityAdded, item, jewelleryList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Resources: AddItemToTab(id, quantity, out quantityAdded, item, resourcesList, isProtected, displayNotification); break;
                    case BaseItem.TabType.Miscellaneous: AddItemToTab(id, quantity, out quantityAdded, item, miscellaneousList, isProtected, displayNotification); break;
                }
            }

            return quantityAdded;
        }
        private void AddItemToTab(int id, int quantity, out int quantityAdded, BaseItem item, List<BaseItem> itemData, bool isProtected, bool displayNotification)
        {
            int startQuantity = quantity;

            if (CheckForItem(id) == true)
            {
                for (int i = 0; i < itemData.Count; i++)
                {
                    if (itemData[i].ID == id)
                    {
                        if (itemData[i].CurrentAmount < itemData[i].MaxAmount)
                        {
                            //Return the difference or current quantity
                            int difference = Math.Min(quantity, itemData[i].MaxAmount - itemData[i].CurrentAmount);

                            //Add difference to current amount, then subtract it from the quantity
                            itemData[i].CurrentAmount += difference;
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
                        itemData.Add(item.Copy(screens, tileMap, currentEntity, camera));
                        itemData.Last().CurrentAmount = quantity;
                        itemData.Last().IsEntityItem = isProtected;

                        quantity = 0;
                    }
                }
            }
            else
            {
                //Scenario 1
                itemData.Add(item.Copy(screens, tileMap, currentEntity, camera));
                itemData.Last().CurrentAmount = quantity;
                itemData.Last().IsEntityItem = isProtected;

                quantity = 0;
            }

            quantityAdded = startQuantity - quantity;
        }

        public void AddItem(BaseItem item, bool displayNotification)
        {
            if (item != null)
            {
                switch (item.tabType)
                {
                    case BaseItem.TabType.Consumables: AddItemToTab(item, consumablesList, displayNotification); break;
                    case BaseItem.TabType.Weapons: AddItemToTab(item, weaponsList, displayNotification); break;
                    case BaseItem.TabType.Armor: AddItemToTab(item, armorList, displayNotification); break;
                    case BaseItem.TabType.Ammo: AddItemToTab(item, ammoList, displayNotification); break;
                    case BaseItem.TabType.Jewellery: AddItemToTab(item, jewelleryList, displayNotification); break;
                    case BaseItem.TabType.Resources: AddItemToTab(item, resourcesList, displayNotification); break;
                    case BaseItem.TabType.Miscellaneous: AddItemToTab(item, miscellaneousList, displayNotification); break;
                }
            }
        }
        private void AddItemToTab(BaseItem item, List<BaseItem> itemData, bool displayNotification)
        {
            bool containsItem = false;
            int leftoverAmount = 0;

            int id = item.ID;
            int quantity = item.CurrentAmount;

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
                        itemData.Add(item.Copy(screens, tileMap, currentEntity, camera));
                        itemData.Last().CurrentAmount = leftoverAmount;
                    }

                    break;
                }
            }

            if (containsItem == false)
            {
                itemData.Add(item.Copy(screens, tileMap, currentEntity, camera));
                itemData.Last().CurrentAmount = quantity;

                if (displayNotification == true)
                    screens.NOTIFICATION_Add(NotificationManager.IconType.Inventory, "Added " + quantity.ToString() + " " + itemData.Last().Name);
            }
        }

        public void RemoveItem(BaseItem item)
        {
            switch (item.tabType)
            {
                case BaseItem.TabType.Consumables: consumablesList.Remove(item); break;
                case BaseItem.TabType.Weapons: weaponsList.Remove(item); break;
                case BaseItem.TabType.Armor: armorList.Remove(item); break;
                case BaseItem.TabType.Ammo: ammoList.Remove(item); break;
                case BaseItem.TabType.Jewellery: jewelleryList.Remove(item); break;
                case BaseItem.TabType.Resources: resourcesList.Remove(item); break;
                case BaseItem.TabType.Miscellaneous: miscellaneousList.Remove(item); break;
            }
        }
        public void RemoveItem(int id, int quantity = 1)
        {
            BaseItem item = GetItem(id);

            if (item != null)
                item.CurrentAmount -= quantity;
        }
        public void ClearItem(int id, int removeAllButX = 0)
        {
            BaseItem item = GetItem(id);

            if (item != null)
                item.CurrentAmount = removeAllButX;
        }

        public void UseItemButton(int id, int button)
        {
            BaseItem item = GetItem(id);

            if (item != null)
            {
                if (item.CurrentAmount >= 1)
                {
                    switch (button)
                    {
                        case 1: if (!string.IsNullOrEmpty(item.ButtonOneText)) item.ButtonOne(); break;
                        case 2: if (!string.IsNullOrEmpty(item.ButtonTwoText)) item.ButtonTwo(); break;
                        case 3: if (!string.IsNullOrEmpty(item.ButtonThreeText)) item.ButtonThree(); break;
                        case 4: if (!string.IsNullOrEmpty(item.ButtonFourText)) item.ButtonFour(); break;
                        case 5: if (!string.IsNullOrEmpty(item.ButtonFiveText)) item.ButtonFive(); break;
                        case 6: if (!string.IsNullOrEmpty(item.ButtonSixText)) item.ButtonSix(); break;
                    }
                }
            }
        }

        private int consumablesIndex, weaponsIndex, armorIndex, ammoIndex, jewelleryIndex, resourcesIndex, miscellaneousIndex;
        public void Update(GameTime gt)
        {
            totalMoney = 0;
            consumablesIndex = weaponsIndex = armorIndex = ammoIndex = jewelleryIndex = resourcesIndex = miscellaneousIndex = 0;

            for (int i = 0; i < CompareGreatestItemCount(); i++)
            {
                consumablesIndex = Max(i, consumablesList.Count - 1);
                weaponsIndex = Max(i, weaponsList.Count - 1);
                armorIndex = Max(i, armorList.Count - 1);
                ammoIndex = Max(i, ammoList.Count - 1);
                jewelleryIndex = Max(i, jewelleryList.Count - 1);
                resourcesIndex = Max(i, resourcesList.Count - 1);
                miscellaneousIndex = Max(i, miscellaneousList.Count - 1);

                if (i <= consumablesIndex && consumablesList.Count > 0)
                {
                    consumablesList[consumablesIndex].UpdateItem(gt);

                    if (consumablesList[consumablesIndex].CurrentAmount <= 0)
                        consumablesList.RemoveAt(consumablesIndex);
                }

                if (i <= weaponsIndex && weaponsList.Count > 0)
                {
                    weaponsList[weaponsIndex].UpdateItem(gt);

                    if (weaponsList[weaponsIndex].CurrentAmount <= 0)
                        weaponsList.RemoveAt(weaponsIndex);
                }

                if (i <= armorIndex && armorList.Count > 0)
                {
                    armorList[armorIndex].UpdateItem(gt);

                    if (armorList[armorIndex].CurrentAmount <= 0)
                        armorList.RemoveAt(armorIndex);
                }

                if (i <= ammoIndex && ammoList.Count > 0)
                {
                    ammoList[ammoIndex].UpdateItem(gt);

                    if (ammoList[ammoIndex].CurrentAmount <= 0)
                        ammoList.RemoveAt(ammoIndex);
                }

                if (i <= jewelleryIndex && jewelleryList.Count > 0)
                {
                    jewelleryList[jewelleryIndex].UpdateItem(gt);

                    if (jewelleryList[jewelleryIndex].CurrentAmount <= 0)
                        jewelleryList.RemoveAt(jewelleryIndex);
                }

                if (i <= resourcesIndex && resourcesList.Count > 0)
                {
                    resourcesList[resourcesIndex].UpdateItem(gt);

                    if (resourcesList[resourcesIndex].CurrentAmount <= 0)
                        resourcesList.RemoveAt(resourcesIndex);
                }

                if (i <= miscellaneousIndex && miscellaneousList.Count > 0)
                {
                    miscellaneousList[miscellaneousIndex].UpdateItem(gt);

                    if (miscellaneousList[miscellaneousIndex].ID == 1) //money id
                        totalMoney += miscellaneousList[miscellaneousIndex].CurrentAmount;

                    if (miscellaneousList[miscellaneousIndex].CurrentAmount <= 0)
                        miscellaneousList.RemoveAt(miscellaneousIndex);
                }
            }
        }
        private int CompareGreatestItemCount()
        {
            int count = consumablesList.Count;

            if (weaponsList.Count > count)
                count = weaponsList.Count;
            if (armorList.Count > count)
                count = armorList.Count;
            if (ammoList.Count > count)
                count = ammoList.Count;
            if (jewelleryList.Count > count)
                count = jewelleryList.Count;
            if (resourcesList.Count > count)
                count = resourcesList.Count;
            if (miscellaneousList.Count > count)
                count = miscellaneousList.Count;

            return count;
        }
        private int Max(int currentValue, int max)
        {
            return MathHelper.Clamp(currentValue, 0, max);
        }

        /* Add in searching options for the following tabs:
         * 
         * - Consumable
         * - Weapon
         * - Armor
         * - Ammo
         * - Jewellery
         * 
         * These will be specified in individual item classes that derive from said tab.
         * 
         * E.G, some potions would contain the word "HEALING", for weapons it could be "MELEE", "ARCHERY", "MAGIC". Same for armor.
         * 
         * For some of these the AI will compare stats against other weapons held, and use that. Also check for weaknesses.
         */

        /// <summary>
        /// Returns the first occurence of the specified "itemType".
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int SearchStorage(string itemType)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (!string.IsNullOrEmpty(TotalItems()[i].ItemType))
                {
                    if (TotalItems()[i].ItemType.ToUpper().Equals(itemType.ToUpper()))
                        return TotalItems()[i].ID;
                }
            }

            return -1;
        }
        /// <summary>
        /// Returns the first occurence of the specified "itemType" and "itemSubType".
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemSubType"></param>
        /// <returns></returns>
        public int SearchStorage(string itemType, string itemSubType)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (!string.IsNullOrEmpty(TotalItems()[i].ItemType) && !string.IsNullOrEmpty(TotalItems()[i].ItemSubType))
                {
                    if (TotalItems()[i].ItemType.ToUpper().Equals(itemType.ToUpper()) &&
                    TotalItems()[i].ItemSubType.ToUpper().Equals(itemSubType.ToUpper()))
                    {
                        return TotalItems()[i].ID;
                    }
                }
            }

            return -1;
        }

        public BaseItem SearchStorageByItem(string itemType)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ItemType.ToUpper() == itemType.ToUpper())
                    return TotalItems()[i];
            }

            return null;
        }
        public BaseItem SearchStorageByItem(string itemType, string itemSubType)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ItemType.ToUpper() == itemType.ToUpper() &&
                    TotalItems()[i].ItemSubType.ToUpper() == itemSubType.ToUpper())
                    return TotalItems()[i];
            }

            return null;
        }

        public List<BaseItem> SearchForItem(string itemType)
        {
            List<BaseItem> items = new List<BaseItem>();

            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ItemType.ToUpper() == itemType.ToUpper())
                    items.Add(TotalItems()[i]);
            }

            return items;
        }
        public List<BaseItem> SearchForItem(string itemType, string itemSubType)
        {
            List<BaseItem> items = new List<BaseItem>();

            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ItemType.ToUpper() == itemType.ToUpper() &&
                    TotalItems()[i].ItemSubType.ToUpper() == itemSubType.ToUpper())
                    items.Add(TotalItems()[i]);
            }

            return items;
        }


        /// <summary>
        /// If storage contains at least one item specified by the keyword, returns true.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public bool CheckStorage(string itemType)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ItemType.ToUpper().Equals(itemType.ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// If storage contains at least one item specified by the two keywords, returns true.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemSubType"></param>
        /// <returns></returns>
        public bool CheckStorage(string itemType, string itemSubType)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ItemType.ToUpper().Equals(itemType.ToUpper()) &&
                    TotalItems()[i].ItemSubType.ToUpper().Equals(itemSubType.ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }
        public bool CheckForItem(int id)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ID == id)
                    return true;
            }

            return false;
        }
        public bool CheckForItem(int id, int quantity)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ID == id)
                {
                    if (TotalItems()[i].CurrentAmount >= quantity)
                        return true;
                    else
                        return false;
                }
            }

            return false;
        }
        public bool IsFullStack(int id)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ID == id)
                {
                    if (TotalItems()[i].CurrentAmount >= TotalItems()[i].MaxAmount)
                        return true;
                }
            }

            return false;
        }
        public int RetrieveAmount(int id)
        {
            for (int i = 0; i < TotalItems().Count; i++)
            {
                if (TotalItems()[i].ID == id)
                    return TotalItems()[i].CurrentAmount;
            }

            return -1;
        }

        public List<BaseItem> GetItemsTab(BaseItem.TabType type)
        {
            if (type == BaseItem.TabType.Consumables)
                return consumablesList;
            else if (type == BaseItem.TabType.Weapons)
                return weaponsList;
            else if (type == BaseItem.TabType.Armor)
                return armorList;
            else if (type == BaseItem.TabType.Ammo)
                return ammoList;
            else if (type == BaseItem.TabType.Jewellery)
                return jewelleryList;
            else if (type == BaseItem.TabType.Resources)
                return resourcesList;

            return miscellaneousList;
        }
        public BaseItem GetItem(int id)
        {
            BaseItem sorter = ItemDatabase.Item(id), returnItem = null;

            if (sorter != null)
            {
                switch (sorter.tabType)
                {
                    case BaseItem.TabType.Consumables: returnItem = Item(id, consumablesList); break;
                    case BaseItem.TabType.Weapons: returnItem = Item(id, weaponsList); break;
                    case BaseItem.TabType.Armor: returnItem = Item(id, armorList); break;
                    case BaseItem.TabType.Ammo: returnItem = Item(id, ammoList); break;
                    case BaseItem.TabType.Jewellery: returnItem = Item(id, jewelleryList); break;
                    case BaseItem.TabType.Resources: returnItem = Item(id, resourcesList); break;
                    case BaseItem.TabType.Miscellaneous: returnItem = Item(id, miscellaneousList); break;
                }
            }

            return returnItem;
        }
        public Consumable GetConsumable(int id)
        {
            for (int i = 0; i < consumablesList.Count; i++)
            {
                if (id == consumablesList[i].ID)
                    return (Consumable)consumablesList[i];
            }
            return null;
        }
        private BaseItem Item(int id, List<BaseItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                    return items[i];
            }
            return null;
        }

        public void Gift(BaseItem item, bool displayNotification)
        {
            AddItem(item.ID, item.CurrentAmount, false, displayNotification);
        }

        private Miscellaneous money = null;
        public void AddMoney(int quantity)
        {
            if (money == null)
                RefreshMoney();

            if (money != null)
            {
                money.CurrentAmount += quantity;

                if (currentEntity.IsPlayerControlled == true)
                {
                    screens.PlayRandom("GetCoins1", "GetCoins2", "GetCoins3");
                }
            }
        }
        public void TakeMoney(int quantity)
        {
            if (money == null)
                RefreshMoney();

            if (money != null)
                money.CurrentAmount -= quantity;
        }
        public bool CheckForMoney(int quantity)
        {
            if (money == null)
                RefreshMoney();

            if (money != null)
                return money.CurrentAmount >= quantity;
            else
                return false;
        }
        public int MoneyQuantity
        {
            get
            {
                if (money == null)
                    RefreshMoney();

                if (money != null)
                    return money.CurrentAmount;
                else
                    return 0;
            }
        }
        public int TotalMoneyStacks()
        {
            int count = 0;
            for (int i = 0; i < miscellaneousList.Count; i++)
            {
                if (miscellaneousList[i].ID == 1)
                    count++;
            }

            return count;
        }
        private int totalMoney = 0;
        public int TotalMoneyQuantity() { return totalMoney; }

        private void RefreshMoney()
        {
            for (int i = 0; i < miscellaneousList.Count; i++)
            {
                if (miscellaneousList[i].ID == 1)
                    money = (Miscellaneous)miscellaneousList[i]; // Get's the last
            }
        }

        //Spellbook Methods
        public void AddSpell(int id)
        {
            if (ContainsSpell(id)) //Prevent duplicate spells from joining
            {
                BaseSpell spell = SpellDatabase.Spell(id);

                if (spell != null)
                    spells.Add(spell.Copy());
            }
        }
        public bool ContainsSpell(int id)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i].ID == id)
                    return true;
            }

            return false;
        }

        //Soul Methods
        public void SetSoulData(EntityStorage storage, EntityEquipment equipment) { }
        public void AddSoul(int id, bool bypassNotification = false)
        {
            bool isAdd = true;
            for (int i = 0; i < SoulsDatabase.Souls.Count; i++)
            {
                if (SoulsDatabase.Souls[i].ID == id)
                {
                    for (int j = 0; j < souls.Count; j++)
                    {
                        if (souls[j].ID == id)
                        {
                            isAdd = false;
                        }
                    }

                    if (isAdd == true)
                    {
                        souls.Add(SoulsDatabase.Souls[i].DeepCopy(tileMap, currentEntity, screens));

                        if (bypassNotification == false)
                            screens.NOTIFICATION_Add(NotificationManager.IconType.Flames, "Added \"" + souls.Last().Name + "\"");
                    }
                }
            }
        }
        public void RemoveSoul(int id)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                {
                    screens.NOTIFICATION_Add(NotificationManager.IconType.Flames, "Removed \"" + souls[i].Name + "\"");
                    souls.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SOUL_ForceCharge(int id)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                {
                    souls[i].ForceCharge();
                }
            }
        }
        public void ForceChargeAll()
        {
            for (int i = 0; i < souls.Count; i++)
            {
                souls[i].ForceCharge();
            }

            for (int i = 0; i < spells.Count; i++)
                spells[i].Recharge();
        }
        public void ForceDepleteCharge(int id, uint deplete)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                {
                    souls[i].SoulCharges -= deplete;
                }
            }
        }
        public void ForceDepleteAll(uint deplete)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                souls[i].SoulCharges -= deplete;
            }
        }

        public void ForceResetDelay(int id)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                {
                    souls[i].ResetDelay();
                }
            }
        }
        public void ForceResetDelayAll()
        {
            for (int i = 0; i < souls.Count; i++)
                souls[i].ResetDelay();
        }

        public void ForceDelay(int id)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                {
                    souls[i].ForceDelay();
                }
            }
        }
        public void ForceDelayAll()
        {
            for (int i = 0; i < souls.Count; i++)
            {
                souls[i].ForceDelay();
            }
        }
        public void ForceDelay(int id, int delayTime)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                {
                    souls[i].ForceDelay(delayTime);
                }
            }
        }
        public void ForceDelayAll(int delayTime)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                souls[i].ForceDelay(delayTime);
            }
        }

        public void ReinforceSoul(int id)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id) //&& inventory.GetItemQuantity(Essense of Souls) > soulLeveling[souls[i].SoulLevel - 1])
                {
                    if (souls[i].CanLevelUp == true)
                    {
                        souls[i].SoulLevel++;
                        //screens.RemoveItem(Essense of Souls, soulLeveling[souls[i].SoulLevel - 1]);

                        screens.NOTIFICATION_Add(NotificationManager.IconType.Flames, "Reinforced \"" + souls[i].Name + "\" to level " + souls[i].SoulLevel);
                    }
                }
            }
        }
        public bool IsMaxLevel(int id)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                {
                    if (souls[i].SoulLevel >= 10)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public StringBuilder SaveData()
        {
            StringBuilder temp = new StringBuilder();

            for (int i = 0; i < TotalItems().Count; i++)
                temp.AppendLine(TotalItems()[i].SaveData().ToString());

            temp.AppendLine();

            for (int i = 0; i < spells.Count; i++)
                temp.AppendLine(spells[i].SaveData().ToString());

            temp.AppendLine();

            for (int s = 0; s < souls.Count; s++)
                temp.AppendLine(souls[s].SaveData().ToString());

            return temp;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string[] words = data[i].Split(' ');

                if (words[0].ToUpper().Equals("CONSUMABLES")) { LoadItem(int.Parse(words[1]), consumablesList, data[i]); }
                if (words[0].ToUpper().Equals("WEAPONS")) { LoadItem(int.Parse(words[1]), weaponsList, data[i]); }
                if (words[0].ToUpper().Equals("ARMOR")) { LoadItem(int.Parse(words[1]), armorList, data[i]); }
                if (words[0].ToUpper().Equals("AMMO")) { LoadItem(int.Parse(words[1]), ammoList, data[i]); }
                if (words[0].ToUpper().Equals("JEWELLERY")) { LoadItem(int.Parse(words[1]), jewelleryList, data[i]); }
                if (words[0].ToUpper().Equals("RESOURCES")) { LoadItem(int.Parse(words[1]), resourcesList, data[i]); }
                if (words[0].ToUpper().Equals("MISCELLANEOUS")) { LoadItem(int.Parse(words[1]), miscellaneousList, data[i]); }

                if (words[0].ToUpper().Equals("SPELL")) { int id = int.Parse(words[1]); LoadSpell(id, data[i]); }

                if (words[0].ToUpper().Equals("SOUL"))
                {
                    AddSoul(int.Parse(words[1]), true);
                    souls.Last().LoadData(data[i]);
                }
            }
        }
        private void LoadItem(int id, List<BaseItem> itemData, string data)
        {
            BaseItem item = ItemDatabase.Item(id);

            if (item != null)
            {
                item = item.Copy(screens, tileMap, currentEntity, camera);

                item.CurrentAmount = 1;

                //If the itemData already has a stack ...
                if (CheckForItem(id) == true)
                {
                    //... and the item can have multiple stacks, ...
                    if (item.IsMultiStack == true)
                    {
                        //Add the items
                        item.LoadData(data);
                        itemData.Add(item);
                    }
                }
                else
                {
                    //If the itemData does not have a stack, go ahead and add it.
                    item.LoadData(data);
                    itemData.Add(item);
                }
            }
        }
        private void LoadSpell(int id, string data)
        {
            if (SpellDatabase.Spell(id) != null)
            {
                BaseSpell spell = SpellDatabase.Spell(id).Copy();

                spell.LoadData(data);
                spells.Add(spell);
            }
        }

        private string stateFilePath;
        public void ReloadState()
        {
            if (!string.IsNullOrEmpty(stateFilePath))
                LoadFromFile(stateFilePath);
        }
        public void LoadFromFile(string file)
        {
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                ParseFromFile(lines.ToList());
            }
        }
        public void ParseFromFile(List<string> data)
        {
            bool isReading = false;

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToUpper().StartsWith("[STORAGE]"))
                {
                    isReading = false;
                    break;
                }
                if (data[i].ToUpper().StartsWith("[/STORAGE]"))
                    isReading = true;

                if (isReading == true)
                {
                    int id = -1, quantity = -1;
                    string[] words = data[i].Split(' ');

                    try
                    {
                        id = int.Parse(words[1]);
                        quantity = int.Parse(words[2]);
                    }
                    catch (Exception e)
                    {
                        Logger.AppendLine("Error loading item/quantity variable from state file. " + e.Message);
                    }

                    itemPoints.Add(new Point(id, quantity));
                }
            }
        }

        public EntityStorage Copy(ScreenManager screens, TileMap map, BaseEntity entity, Camera camera)
        {
            EntityStorage copy = (EntityStorage)this.MemberwiseClone();

            copy.SetReferences(screens, map, entity, camera);

            copy.consumablesList = new List<BaseItem>();
            copy.weaponsList = new List<BaseItem>();
            copy.armorList = new List<BaseItem>();
            copy.ammoList = new List<BaseItem>();
            copy.jewelleryList = new List<BaseItem>();
            copy.resourcesList = new List<BaseItem>();
            copy.miscellaneousList = new List<BaseItem>();

            copy.itemPoints = this.itemPoints.ToList();
            copy.AddPoints();

            copy.souls = new List<BaseSoul>();

            return copy;
        }
    }
}
