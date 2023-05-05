using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.AudioEngine;
using System.Text;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Entities;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class BaseItem
    {
        Texture2D icon;
        public Texture2D Icon { get { return icon; } }
        
        string name, description;
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        
        int id;
        public int ID { get { return id; } }
        
        int currentAmount;
        public int CurrentAmount
        {
            get { return currentAmount; }
            set
            {
                currentAmount = (int)MathHelper.Clamp(value, 0, maxAmount);
            }
        }
        
        int maxAmount;
        public int MaxAmount { get { return maxAmount; } }

        public int buttonTarget = 1; // Save this value!
        public int MaxButtonTargets
        {
            get
            {
                int count = 0;

                if (!string.IsNullOrEmpty(ButtonOneText))
                    count++;
                if (!string.IsNullOrEmpty(ButtonTwoText))
                    count++;
                if (!string.IsNullOrEmpty(ButtonThreeText))
                    count++;
                if (!string.IsNullOrEmpty(ButtonFourText))
                    count++;

                return count;
            }
        }

        protected int currentDurability, maxDurability;
        public int CurrentDurability { get { return currentDurability; } set { currentDurability = (int)MathHelper.Clamp(value, 0, maxDurability); } }
        public int MaxDurability { get { return maxDurability; } }

        Requirements requirements;
        public Requirements ItemRequirements { get { return requirements; } }

        protected ItemAttribute itemAttributes;
        public ItemAttribute Attributes { get { return itemAttributes; } }

        public float PhysicalDefense()
        {
            if (itemAttributes != null)
                return itemAttributes.PhysicalDefense(currentReinforcement, DurabilityMultiplier * (itemAttributes.BasePhysicalDefense * CalculateReinforcePct()));
            else
                return 0f;
        }
        public float ProjectileDefense()
        {
            if (itemAttributes != null)
                return itemAttributes.ProjectileDefense(currentReinforcement, DurabilityMultiplier * (itemAttributes.BaseProjectileDefense * CalculateReinforcePct()));
            else
                return 0f;
        }
        public float MagicDefense()
        {
            if (itemAttributes != null)
                return itemAttributes.MagicDefense(currentReinforcement, DurabilityMultiplier * (itemAttributes.BaseMagicDefense * CalculateReinforcePct()));
            else
                return 0f;
        }
        public float Weight()
        {
            if (itemAttributes != null)
                return itemAttributes.Weight(currentReinforcement, DurabilityMultiplier * (itemAttributes.BaseWeight * (CalculateReinforcePct()) * .25f));
            else
                return 0;
        }
        public float EffectAmplifier()
        {
            if (itemAttributes != null)
                return itemAttributes.EffectAmplifier(currentReinforcement, DurabilityMultiplier * (itemAttributes.BaseEffectAmplifier * (CalculateReinforcePct() * .25f)));
            else
                return 0f;
        }
        public float EffectResistance()
        {
            if (itemAttributes != null)
                return itemAttributes.EffectResistance(currentReinforcement, DurabilityMultiplier * (itemAttributes.BaseEffectResistance * (CalculateReinforcePct() * .25f)));
            else
                return 0f;
        }

        protected float CalculateReinforcePct() { return .1f - (currentReinforcement * .0025f);  }

        public virtual float DurabilityMultiplier
        {
            get
            {
                float durrPct = (float)currentDurability / maxDurability;

                float value = 1f; //Default is always 1f

                if (durrPct.IsClose(.5f, .75f) == true) //If 50% or less durability left
                    value = .95f;
                else if (durrPct.IsClose(.2f, .5f) == true)
                    value = .85f;
                else if (durrPct.IsClose(.00001f, .1999f))
                    value = .75f;
                else if (currentDurability <= 0)
                    value = .1f;

                return value;
            }
        }

        public enum TabType { Consumables, Weapons, Armor, Ammo, Jewellery, Resources, Miscellaneous }
        public TabType tabType;

        public bool isSelected;

        public Vector2 positionOffset { get; set; }
        public Point gridLocation { get; set; }

        public Rectangle itemRect;

        //For the AI, and possibly other smaller things.
        protected string type = string.Empty, subType = string.Empty;
        public string ItemType { get { return type; } }
        public string ItemSubType { get { return subType; } }

        //Button names, etc.
        protected string buttonOneText, buttonTwoText, buttonThreeText, buttonFourText, buttonFiveText, buttonSixText;
        public string ButtonOneText { get { return buttonOneText; } }
        public string ButtonTwoText { get { return buttonTwoText; } }
        public string ButtonThreeText { get { return buttonThreeText; } }
        public string ButtonFourText { get { return buttonFourText; } }
        public string ButtonFiveText { get { return buttonFiveText; } }
        public string ButtonSixText { get { return buttonSixText; } }
        public void SetButtonStrings(string buttonOneText, string buttonTwoText, string buttonThreeText,
                                     string buttonFourText, string buttonFiveText, string buttonSixText)
        {
            this.buttonOneText = buttonOneText;
            this.buttonTwoText = buttonTwoText;
            this.buttonThreeText = buttonThreeText;
            this.buttonFourText = buttonFourText;
            this.buttonFiveText = buttonFiveText;
            this.buttonSixText = buttonSixText;
        }

        public virtual string ToolTipText(EntityEquipment equipment)
        {
            string tip = name + "\n\n";

            if (currentReinforcement > 0)
                tip = ReinforcmentName() + " " + name + "\n\n";

            float durrPct = (float)currentDurability / maxDurability;

            if (currentDurability < maxDurability)
                tip += "Efficiency at " + (DurabilityMultiplier * 100).ToString("0.0") + "%";

            if (durrPct.IsClose(.65f, .95f) == true)
                tip += " (Slightly damaged)";
            else if (durrPct.IsClose(.35f, .65f) == true)
                tip += " (Damaged)";
            else if (durrPct.IsClose(.15f, .35f) == true)
                tip += " (Very damaged)";
            else if (durrPct < .15f && currentDurability >= 1)
                tip += " (Nearly broken)";
            else if (currentDurability <= 0)
                tip += " (Broken)";
                 
            if (MeetsRequirements() == false)
                tip += "\n\nYou will only be " + (RequirementPercentage() * 100).ToString("0.0") + "% effective with this item.";
            //tip += "\n\nRequirements: \n" + DisplayRequirements();

            tip += AttributeComparison(equipment);

            return tip;
        }
        protected virtual string AttributeComparison(EntityEquipment equipment)
        {
            return string.Empty;
        }
        protected string CompareAttributeText(string attributeName, float currentValue, float newValue, bool isGreaterBetter)
        {
            return attributeName + " " + string.Format("{0:F1}", currentValue) + " -> " + string.Format("{0:F1}", newValue) + " (" + Math.Abs(currentValue - newValue) + ")";
        }
        protected string CompareAttributeText(string attributeName, int currentValue, int newValue, bool isGreaterBetter)
        {
            return attributeName + " " + currentValue + " -> " + newValue + " (" + Math.Abs(currentValue - newValue) + ")";
        }

        protected bool isEssential;
        /// <summary>
        /// Essential items cannot be disposed, gifted, or sold. Similar to RuneScape's 'quest' items.
        /// </summary>
        public bool IsEssential { get { return isEssential; } }

        protected bool isEntityItem = false;
        public bool IsEntityItem { get { return isEntityItem; } set { isEntityItem = value; } }

        protected bool isNew;
        public bool IsNew { get { return isNew; } set { isNew = value; } }

        protected int sellWorth = 0;
        public int SellWorth { get { return sellWorth; } }
        public int StackWorth { get { return currentAmount * sellWorth; } }
        
        public string WorthWords { get; private set; }
        public string TypeWords { get; private set; }

        protected ScreenManager screens;
        public ScreenManager Screens { get { return screens; } }

        protected TileMap tileMap;
        public TileMap Map { get { return tileMap; } }

        protected BaseEntity currentEntity;
        public BaseEntity CurrentEntity { get { return currentEntity; } }

        protected Camera camera;
        public Camera Camera { get { return camera; } }

        public Random Random { get; protected set; }

        public SoundEffect2D UseItemSound { get; set; }
        public SoundEffect2D CombineItemSound { get; set; }

        private List<SoundEffect2D> sfx = new List<SoundEffect2D>();
        public void AddSFX(SoundEffect2D effect) { sfx.Add(effect); sfx.Last().SetLoopValue(false); }
        public SoundEffect2D GetSFX(params int[] values)
        {
            int value = Random.Next(0, values.Length);
            return sfx[value];
        }

        private string uniqueID;
        public string UniqueID { get { return uniqueID; } }
        public bool IsMultiStack { get; protected set; }

        public string AttributeText { get; protected set; }
        public virtual void RefreshAttributeText() { }

        public BaseItem(Texture2D IconDirectory, int ID, string Name, string Description, int MaxAmount, TabType TabType, int MaxDurability, 
                        bool IsEssential, Requirements ItemRequirements, int SellPrice, string ItemType, string ItemSubType)
        {
            icon = IconDirectory;

            id = ID;

            name = Name;
            description = Description;

            maxAmount = MaxAmount;

            tabType = TabType;

            maxDurability = MaxDurability;
            CurrentDurability = this.MaxDurability;

            isEssential = IsEssential;
            requirements = ItemRequirements;

            sellWorth = (int)MathHelper.Clamp(SellPrice, -1, 999999);

            WorthWords = "$ " + sellWorth.CommaSeparation() + "";

            if (sellWorth == 0)
                WorthWords = "Worthless";
            if (sellWorth == -1 || IsEssential == true)
                WorthWords = "Cannot be sold";

            type = ItemType;
            subType = ItemSubType;

            if (!string.IsNullOrEmpty(type))
                TypeWords += type;
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(subType))
                TypeWords += ", " + subType;

            if (!string.IsNullOrEmpty(subType) && string.IsNullOrEmpty(type))
                TypeWords += subType;

            IsRestRepairsBroken = false;
            IsRestRepairsUnbroken = true;

            Random = new Random(Guid.NewGuid().GetHashCode());
            uniqueID = Strings.RandomSymbols(Random, 8, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
        }

        public virtual void Load(ContentManager main, ContentManager map)
        {
            RefreshAttributeText();
        }

        //For whatever purpose you need it for.
        public virtual void UpdateItem(GameTime gt)
        {

        }

        public void UpdateRect(Vector2 GridOffset, int OffsetY)
        {
            itemRect = new Rectangle(gridLocation.X * 65 + (int)GridOffset.X,
                                     gridLocation.Y * 65 + (int)GridOffset.Y + OffsetY,
                                     64, 64);
        }

        //Left side of pane
        public virtual void ButtonOne() { if (buttonOneAction != null) buttonOneAction.Invoke(this); }
        public virtual void ButtonTwo() { if (buttonTwoAction != null) buttonTwoAction.Invoke(this); }
        public virtual void ButtonThree() { if (buttonThreeAction != null) buttonThreeAction.Invoke(this); }
        public virtual void ButtonFour() { if (buttonFourAction != null) buttonFourAction.Invoke(this); }
        //Right side of pane
        public virtual void ButtonFive() { if (buttonFiveAction != null) buttonFiveAction.Invoke(this); }
        public virtual void ButtonSix() { if (buttonSixAction != null) buttonSixAction.Invoke(this); }

        //Used for outside assigning of buttons. NOTE: generally, this is to be assigned ONLY in the item database
        private Action<BaseItem> buttonOneAction, buttonTwoAction, buttonThreeAction, buttonFourAction, buttonFiveAction, buttonSixAction;
        public Action<BaseItem> ButtonOneAction { set { buttonOneAction = value; } }
        public Action<BaseItem> ButtonTwoAction { set { buttonTwoAction = value; } }
        public Action<BaseItem> ButtonThreeAction { set { buttonThreeAction = value; } }
        public Action<BaseItem> ButtonFourAction { set { buttonFourAction = value; } }
        public Action<BaseItem> ButtonFiveAction { set { buttonFiveAction = value; } }
        public Action<BaseItem> ButtonSixAction { set { buttonSixAction = value; } }

        public bool doesCombineHaveResults = false;
        /// <summary>
        /// Use for combining an item with another item! May seem similar to crafting, though this is more of a pre-step to crafting.
        /// </summary>
        public void CombineItem(BaseItem item)
        {
            if (combineItem != null)
                combineItem.Invoke(this, item); //Always call this method!

            UseOnItem(item);
        }
        /// <summary>
        /// If overriding and putting values here, make sure to set "doesCombineHaveResults" to true in this method!
        /// </summary>
        /// <param name="item"></param>
        protected virtual void UseOnItem(BaseItem item)
        {
            /* Example code:
             * 
             * 1 - pestle and mortar
             * 2 - shriekstone
             * 3 - shriekstone dust
             * 
             * if (item.ID == 1)
             * {
             *     this.CurrentAmount -= 1; //Remove one item from this
             *     currentEntity.STORAGE_AddItem(3, 1); //Add one
             *     item.DecreaseDurability(1);
             * }
             */
        }
        /// <summary>
        /// This is useful for putting inside a constructor of an item that may be used by more than one IDs.
        /// Prevents unnecessary class creation. Example: All of the different stones and rocks that can be used with a pestle and mortar.
        /// By default, it is invoked inside of the CombineItem(...) virtual method.
        /// </summary>
        public Action<BaseItem, BaseItem> CombineItemAction { set { combineItem = value; } }
        private Action<BaseItem, BaseItem> combineItem;

        public bool MeetsRequirements() { return requirements.HasRequirements(currentEntity.Skills); }
        public bool MeetsRequirements(Skillset skills) { return requirements.HasRequirements(skills); }
        public float RequirementPercentage() { return requirements.RequirementPercentage(currentEntity.Skills, 4); }
        public float RequirementPercentage(Skillset skills) { return requirements.RequirementPercentage(skills, 4); }
        public string DisplayRequirements() { return requirements.RequirementsToString(currentEntity.Skills); }
        public string DisplayRequirements(Skillset skills) { return requirements.RequirementsToString(skills); }

        public bool HasRepairIngredients { get; set; }

        public void DecreaseDurability(int amount = 1) { CurrentDurability -= amount; }
        public void RepairItem() { CurrentDurability = maxDurability; }
        public void RepairItem(int amount) { CurrentDurability += amount; }
        public bool IsBroken()
        {
            if (maxDurability != -1)
                return (CurrentDurability <= 0);

            return false;
        }

        /// <summary>
        /// When resting, this determines if an unbroken item is repaired.
        /// </summary>
        public bool IsRestRepairsUnbroken { get; protected set; }
        /// <summary>
        /// When resting, this determines if a broken item is repaired.
        /// </summary>
        public bool IsRestRepairsBroken { get; protected set; }

        public bool IsQuantityMax { get { return currentAmount >= maxAmount; } }

        //Item Reinforcing.
        protected int currentReinforcement = 0, maxReinforcement = 0; //Maximum should really only be 10.
        protected float reinforceMultiplier;

        public int CurrentReinforcement { get { return currentReinforcement; } }
        public int MaxReinforcement { get { return maxReinforcement; } set { maxReinforcement = value; } }

        public string ReinforcmentName()
        {
            if (currentReinforcement > 0 && currentReinforcement < 5)
                return "Improved";
            if (currentReinforcement >= 5 && currentReinforcement < 10)
                return "Honed";
            if (currentReinforcement >= 10)
                return "Renown";

            return string.Empty;
        }
        public void Reinforce()
        {
            currentReinforcement++;
            currentReinforcement = (int)MathHelper.Clamp(currentReinforcement, 0, maxReinforcement + 1);
        }
        public void DeinforceAll()
        {
            currentReinforcement = 0;
        }

        //public Dictionary<int, List<SimpleItemHolder>> ReinforceIngredients { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idQuantity"> Reinforcement level, item ID, item quantity</param>
        /*public void SetReinforceIngredients(int reinforceMultiplier, params Tuple<int, List<Point>>[] levelIDQuantity)
        {
            ReinforceIngredients = new Dictionary<int, List<SimpleItemHolder>>();

            for (int i = 0; i < levelIDQuantity.Length; i++)
            {
                List<SimpleItemHolder> tempList = new List<SimpleItemHolder>();
                for (int j = 0; j < levelIDQuantity[i].Item2.Count; j++)
                {
                    BaseItem temp = ItemDatabase.Item(levelIDQuantity[i].Item2[j].X);

                    if (temp != null)
                        tempList.Add(new SimpleItemHolder(temp.ID, levelIDQuantity[i].Item2[j].Y, temp.Name, temp.Icon));
                }

                ReinforceIngredients.Add(levelIDQuantity[i].Item1, tempList);
            }

            ReinforceIngredients.Add(ReinforceIngredients.LastOrDefault().Key + 1, new List<SimpleItemHolder>());
        }*/
        public bool IsReinforceable() { return (currentReinforcement <= maxReinforcement); }
        public bool HasReinforceIngredients { get; set; }

        public virtual string SaveData()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(tabType.ToString() + " "); //The tab type
            builder.Append(id.ToString() + " "); //ID of the item
            builder.Append(currentAmount.ToString() + " "); //The current amount
            builder.Append(currentDurability.ToString() + " "); //The current durability
            builder.Append(currentReinforcement.ToString() + " "); //The current reinforcement level
            builder.Append(isNew.ToString()); //If this item has been recently added (not clicked on)

            //if (GameSettings.IsDebugging == true)
            //    builder.Append("//" + name);

            return builder.ToString();
        }
        public virtual void LoadData(string data)
        {
            string[] words = data.Split(' ');

            try
            {
                currentAmount = MathHelper.Clamp(int.Parse(words[2]), 0, MaxAmount);
                currentDurability = MathHelper.Clamp(int.Parse(words[3]), 0, MaxDurability);
                currentReinforcement = MathHelper.Clamp(int.Parse(words[4]), 0, MaxReinforcement);
                isNew = bool.Parse(words[5]);
            }
            catch
            {
                Logger.AppendLine("Error loading item data (" + data + "). Ensure the data saved is in the expected format and has not been corrupted!");
            }
        }

        public virtual BaseItem Copy(ScreenManager screens, TileMap tileMap, BaseEntity currentEntity, Camera camera)
        {
            BaseItem item = (BaseItem)this.MemberwiseClone();

            item.screens = screens;
            item.tileMap = tileMap;
            item.currentEntity = currentEntity;
            item.camera = camera;

            isNew = true;

            if (IsMultiStack == true)
                item.uniqueID = Strings.RandomSymbols(Random, 8, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");

            return item;
        }
        /// <summary>
        /// Only use this when copying existing items!
        /// </summary>
        /// <returns></returns>
        public BaseItem Copy()
        {
            return Copy(screens, tileMap, currentEntity, camera);
        }

        public void MergeItemStacks(BaseItem item)
        {
            if (this.ID == item.ID && this.CurrentAmount < this.MaxAmount)
            {
                int roomFor = this.MaxAmount - this.CurrentAmount; //How many more items can be merged until the stack is full
                int transferQuantity = (int)Math.Min(item.CurrentAmount, roomFor);

                item.CurrentAmount -= transferQuantity;
                this.CurrentAmount += transferQuantity;
            }
        }
        public int MerchantCompareTo(BaseItem item)
        {
            int sort = item.isEntityItem.CompareTo(isEntityItem);

            if (sort == 0)
                sort = tabType.CompareTo(item.tabType);
            if (sort == 0) //If both item tabs are the same ...
                sort = this.id.CompareTo(item.ID); //Sort by item id.
            if (sort == 0) //If both item ids are the same...
                sort = item.currentAmount.CompareTo(this.CurrentAmount); //Sort by item quantity.

            return sort;
        }

        public int RoomLeft() { return MaxAmount - CurrentAmount; } //How many more items can be merged until the stack is full
        public int MaxTransferQuantity(int quantity) { return Math.Min(RoomLeft(), quantity); }
    }
}
