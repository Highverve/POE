using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Performance;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate.Crafting
{
    public class Offering
    {
        private int id, emberCost;
        public int ID { get { return id; } }
        public int EmberCost { get { return emberCost; } }

        private List<ItemHolder> input = new List<ItemHolder>(); //Input can have unlimited item slots (though really should be restricted to just a few and dictated by the UI).
        private List<Point> output = new List<Point>(); //Output can have unlimited item slots. However, the first item will be the item icon (though really should be restricted to almost always one and dictated by the UI).
        private List<Point> perfectOutput = new List<Point>();

        public List<ItemHolder> Input { get { return input; } }
        public List<Point> Output { get { return output; } }
        public List<Point> PerfectOutput { get { return perfectOutput; } }

        public enum RecipeState
        {
            Unknown,
            Discovered,
            Known,
            Perfected
        }
        private RecipeState state = RecipeState.Unknown; //Defaults to unknown.
        public RecipeState State { get { return state; } set { state = value; } }

        private int totalOffered = 0; //The amount of times the item has been crafted. Save this value for both statistics and for "perfected" items.
        private int minimumToImproved = 0; //The minimum amount of times the item needs to be crafted before the player can craft a "perfect" version of this item.

        public int TotalOffered { get { return totalOffered; } set { totalOffered = value; if (totalOffered >= minimumToImproved) state = RecipeState.Perfected; } }
        public int MinimumToImproved { get { return minimumToImproved; } }

        public bool IsImprovableOffering { get { return (minimumToImproved > 0); } }
        public bool IsImproved { get { return (totalOffered > minimumToImproved); } }

        public bool HasIngredients { get; set; }

        // --- Item variables ---
        private Texture2D icon;
        public Texture2D Icon { get { return icon; } }

        private string name;
        public string Name { get { return name; } }

        private string description;
        public string Description { get { return description; } }

        private List<Texture2D> inputIcons = new List<Texture2D>();

        private BaseItem.TabType tabType = BaseItem.TabType.Miscellaneous;
        public BaseItem.TabType Tab { get { return tabType; } }

        public Rectangle ItemRect { get; set; }
        public bool IsSelected { get; set; }

        public Offering(int ID, int EmberCost, List<Point> Input, List<Point> Output, List<Point> PerfectOutput, int MinimumToImproved)
        {
            id = ID;
            emberCost = EmberCost;

            if (Input != null)
            {
                for (int i = 0; i < Input.Count; i++)
                {
                    BaseItem temp = ItemDatabase.Item(Input[i].X);

                    if (temp != null)
                        input.Add(new ItemHolder(temp.ID, Input[i].Y, temp.Name, temp.Icon));
                }
            }

            output = Output;

            if (PerfectOutput == null)
                perfectOutput = output; //If the value is null, set it to give the player the same items as before.
            else
                perfectOutput = PerfectOutput;

            minimumToImproved = MinimumToImproved;

            BaseItem item = ItemDatabase.Item(Output.FirstOrDefault().X); //The first item is the primary output item! Always remember this.
            tabType = item.tabType;
            icon = item.Icon;
            name = item.Name;
            description = item.Description;
        }

        public string SaveData()
        {
            string builder = id.ToString() + " " + state.ToString() + " " + totalOffered.ToString() + " //" + name; //id, state, totalCrafted
            return builder;
        }
        public void LoadData(string data)
        {
            string[] words = data.Split(' ');

            try
            {
                if (int.Parse(words[0]) == id)
                {
                    state = (RecipeState)System.Enum.Parse(typeof(RecipeState), words[1]);
                    totalOffered = int.Parse(words[2]);
                }
            }
            catch (System.Exception e)
            {
            }
        }
    }

    public class OfferingsUI
    {
        private Vector2 position;
        private Vector2 Position { set { position = new Vector2(MathHelper.Clamp(value.X, 37, GameSettings.VectorResolution.X - (tab.Width + pane.Width + 5)),
                                                                MathHelper.Clamp(value.Y, 0, GameSettings.VectorResolution.Y - tab.Height)); } }
        private Rectangle dragArea, clickCheck;

        private Texture2D tab, pane, paneEnd, iconButton, iconButtonHover, paneButton, paneButtonHover, smallButton, smallButtonHover, tabButton, tabButtonHover, ribbon, unknown;
        private Texture2D consumables, weapons, armor, ammo, jewellery, resources, miscellaneous;
        private SpriteFont font, largeFont;

        private Rectangle consumableRect, weaponsRect, armorRect, ammoRect, jewelleryRect, resourcesRect, miscellaneousRect;
        private MenuButton buttonOne, buttonTwo, buttonThree, buttonFour; //Pane buttons.

        public bool IsActive { get; set; }
        private float scrollPosition = 0;
        private float currentPaneWidth = 0f, paneLerp = 0f; //Pane width. Increased when recipe is selected

        private Offering selectedRecipe = null;
        private bool isSelected = false;


        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;


        BaseItem.TabType currentTab = BaseItem.TabType.Consumables;

        private Controls controls = new Controls();
        private ScreenManager screens;
        private BaseEntity controlledEntity;

        private List<Offering> currentOfferings = new List<Offering>(); //The recipes to be displayed in the current tab.
        private List<Offering> totalOfferings = new List<Offering>(); //The list of all offerings.

        public OfferingsUI() { }

        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;

            IsActive = false;
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

            consumables = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/consumable");
            weapons = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/weapon");
            armor = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/armor");
            ammo = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/ammo");
            jewellery = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/jewellery");
            resources = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/resources");
            miscellaneous = cm.Load<Texture2D>("Interface/Inventory/Icons/Tabs/miscellaneous");

            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            position = new Vector2(GameSettings.VectorCenter.X - (tab.Center().X + pane.Center().X),
                                   GameSettings.VectorCenter.Y - tab.Center().Y);

            mouseDragOffset = new Vector2(tab.Width / 2, 12);

            AddOfferings();

            buttonOne = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonTwo = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonThree = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonFour = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
        }

        private string hints = "Artisan's Counter Tips:\n\nThere are four states of a recipe. In order, they are:\n- Unknown\n- Uncrafted\n- Known\n- Refined\n\nAn unknown recipe will not be shown at all,\nand must be discovered in the game world.\n\nAn uncrafted recipe is blacked out, and must be\ncrafted to transition to the next state.\n\nA known recipe has been crafted previously.\n\nLastly, a refined recipe is a regular recipe that has been crafted\na specific number of times. Not all recipes can be refined.\n\n--------------------\n\nThe required ingredients are at the bottom of the recipe's details\non the right side after selecting a recipe.";
        public void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                controls.UpdateCurrent();

                clickCheck = new Rectangle((int)position.X + 15, (int)position.Y + 20, tab.Width + (int)currentPaneWidth, tab.Height - 20);

                CheckDragScreen();
                UpdateTabButtons(gt);

                UpdateRecipes(gt);
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
                    ToolTip.RequestStringAssign("Hide Artisan's Counter");

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

        private bool isTabHover = false;
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

        CallLimiter limitIngredientCheck = new CallLimiter(2500);
        private void UpdateRecipes(GameTime gt)
        {
            isSelected = false;
            ApplyProperItemGrid();

            for (int i = 0; i < currentOfferings.Count; i++)
            {
                if (currentOfferings[i].ItemRect.Contains(controls.MousePosition))
                {
                    if (currentOfferings[i].State != Offering.RecipeState.Discovered)
                    {
                        if (currentOfferings[i].HasIngredients)
                            ToolTip.RequestStringAssign(currentOfferings[i].Name + "\n\n (offered x" + currentOfferings[i].TotalOffered + ")");
                        else
                            ToolTip.RequestStringAssign(currentOfferings[i].Name + "\n\nYou lack necessary ingredients.");
                    }
                    else
                    {
                        if (currentOfferings[i].HasIngredients)
                            ToolTip.RequestStringAssign("Uncertain Item");
                        else
                            ToolTip.RequestStringAssign("Uncertain Item" + "\n\nYou lack necessary ingredients.");
                    }

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Item Select");
                        currentOfferings[i].IsSelected = true;

                        descLines = currentOfferings[i].Description.WrapFormatText(font, Color.White, 260);
                    }
                }
                else if (!scissorPane.Contains(controls.MousePosition) && !isDragging)
                {
                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        currentOfferings[i].IsSelected = false;
                }

                if (currentOfferings[i].IsSelected == true)
                {
                    selectedRecipe = currentOfferings[i];
                    isSelected = true;
                }
            }

            if (limitIngredientCheck.IsCalling(gt)) //Call periodically, but not constantly. Noticable drop in FPS (30-50) when constantly checking.
                CheckIngredients();
        }
        private void CheckIngredients()
        {
            for (int i = 0; i < currentOfferings.Count; i++)
                CheckIngredients(currentOfferings[i]);
        }
        private void CheckIngredients(Offering recipe)
        {
            recipe.HasIngredients = (HasIngredients(recipe) && controlledEntity.SKILL_Embers() >= recipe.EmberCost);
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

            if (selectedRecipe != null && isSelected == true)
            {
                for (int i = 0; i < selectedRecipe.Input.Count; i++)
                {
                    selectedRecipe.Input[i].Rect = new Rectangle((int)position.X + 434 + (34 * i), (int)position.Y + 270, 32, 32);

                    if (selectedRecipe.Input[i].Rect.Contains(controls.MousePosition))
                        ToolTip.RequestStringAssign(selectedRecipe.Input[i].Name + " (x" + selectedRecipe.Input[i].Quantity + ")");
                }

                if (buttonOne.IsHover == true)
                    ToolTip.RequestStringAssign("Offer items");
                if (buttonTwo.IsHover == true)
                    ToolTip.RequestStringAssign("Offer items (x10)");

                if (buttonOne.IsLeftClicked == true)
                    OfferItem(selectedRecipe);
                if (buttonTwo.IsLeftClicked == true)
                {
                    for (int i = 0; i < 10; i++)
                        OfferItem(selectedRecipe);
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

            float longBounds = -((int)((currentOfferings.Count - 1) / 6) * (iconButton.Height + 1)) + (scissorGrid.Height - 70);
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

            if (currentOfferings.Count >= 6)
            {
                for (int i = 0; i < currentOfferings.Count; i++)
                {
                    Point gridLocation = new Point(i % 6, (int)(i / 6));
                    currentOfferings[i].ItemRect = new Rectangle((int)gridOffset.X + (gridLocation.X * 65),
                                                            (int)gridOffset.Y + (gridLocation.Y * 65) + (int)scrollPosition,
                                                            64, 64);
                }
            }
            else if (currentOfferings.Count < 6)
            {
                for (int i = 0; i < currentOfferings.Count; i++)
                {
                    Point gridLocation = new Point(i, 0);
                    currentOfferings[i].ItemRect = new Rectangle(gridLocation.X * 65 + (int)gridOffset.X,
                                                            gridLocation.Y * 65 + (int)gridOffset.Y + (int)scrollPosition,
                                                            64, 64);
                }
            }
        }

        private void UpdateList(BaseItem.TabType type)
        {
            currentOfferings.Clear();

            for (int i = 0; i < totalOfferings.Count; i++)
            {
                if (totalOfferings[i].Tab == type && totalOfferings[i].State != Offering.RecipeState.Unknown) //Add recipe to list if it's the same tab type and state is not unknown
                {
                    if (!currentOfferings.Contains(totalOfferings[i]))
                        currentOfferings.Add(totalOfferings[i]);
                }
            }
        }

        public void OfferItem(int recipeID)
        {
            OfferItem(OfferingByID(recipeID));
        }
        private void OfferItem(Offering recipe)
        {
            if (recipe != null)
            {
                CheckIngredients(recipe); //Force check recipe before, just in case

                if (recipe.HasIngredients)
                {
                    //Remove offering
                    controlledEntity.SKILL_RemoveEmbers(recipe.EmberCost);

                    if (recipe.Input != null)
                    {
                        for (int i = 0; i < recipe.Input.Count; i++)
                            controlledEntity.STORAGE_RemoveItem(recipe.Input[i].ID, recipe.Input[i].Quantity);
                    }

                    //Receive offering
                    if (recipe.IsImproved == false)
                    {
                        for (int i = 0; i < recipe.Output.Count; i++)
                            controlledEntity.STORAGE_AddItem(recipe.Output[i].X, recipe.Output[i].Y, controlledEntity.IsPlayerControlled); //IsPlayerControlled should always result to true, but just in case.
                    }
                    else
                    {
                        for (int i = 0; i < recipe.Output.Count; i++)
                            controlledEntity.STORAGE_AddItem(recipe.Output[i].X, recipe.Output[i].Y, controlledEntity.IsPlayerControlled); //IsPlayerControlled should always result to true, but just in case.
                    }

                    //Change state to "known"
                    if (recipe.State == Offering.RecipeState.Discovered)
                        recipe.State = Offering.RecipeState.Known;

                    //Increment total crafting.
                    recipe.TotalOffered++;

                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Your offer has been accepted");
                    if (recipe.IsImproved == true)
                        screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Knowledge of this offering has deepened");
                }
                else
                {
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "You lack the necessary items");
                }
            }
        }

        public bool HasIngredients(Offering recipe)
        {
            bool hasIngredients = true;
            for (int i = 0; i < recipe.Input.Count; i++)
            {
                if (!controlledEntity.STORAGE_Check(recipe.Input[i].ID, recipe.Input[i].Quantity))
                    hasIngredients = false;
            }

            return hasIngredients;
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

                sb.Draw(tab, position, Color.White);

                sb.DrawString(largeFont, "Offerings", position + new Vector2(tab.Width / 2, 12), "Offerings".LineCenter(largeFont), ColorHelper.UI_LightGold, 1f);
                sb.DrawString(largeFont, currentTab.ToString(), position + new Vector2(tab.Width / 2, 47), currentTab.ToString().LineCenter(largeFont), Color.White, 1f);

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

            for (int i = 0; i < currentOfferings.Count; i++)
            {
                if (currentOfferings[i].IsSelected == true)
                    DrawButtonState(sb, currentOfferings[i], iconButtonHover, currentOfferings[i].ItemRect.Location.ToVector2());
                else
                    DrawButtonState(sb, currentOfferings[i], iconButton, currentOfferings[i].ItemRect.Location.ToVector2());

                DrawIconState(sb, currentOfferings[i], currentOfferings[i].ItemRect.Location.ToVector2());
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }

        private string discoveredDesc = "You can't be certain what this combination may bring, but you can be hopeful that it will aid you in some way.";
        private List<TextBlock> descLines = new List<TextBlock>();

        private void DrawPane(SpriteBatch sb)
        {
            scissorPane = new Rectangle((int)position.X + 412, (int)position.Y + 41, (int)currentPaneWidth + 4, 353);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorPane;

            buttonOne.DrawButton(sb, Color.White);
            buttonTwo.DrawButton(sb, Color.White);

            if (selectedRecipe != null)
            {
                DrawButtonState(sb, selectedRecipe, iconButton, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));
                DrawIconState(sb, selectedRecipe, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));

                //Draw ember cost
                if (selectedRecipe.EmberCost > 0)
                {
                    string cost = "$" + selectedRecipe.EmberCost.CommaSeparation();
                    sb.DrawString(font, cost, new Vector2((int)position.X + 550, (int)position.Y + 255), ColorHelper.D_Orange, 0f, cost.LineCenter(font), 1f, SpriteEffects.None, 1f);
                }

                //Draw input ingredients
                if (selectedRecipe.Input != null)
                {
                    for (int i = 0; i < selectedRecipe.Input.Count; i++)
                        DrawIngredient(sb, selectedRecipe.Input[i].Rect, selectedRecipe.Input[i].Icon, selectedRecipe.Input[i].ID, selectedRecipe.Input[i].Quantity);
                }

                //Draw item name
                if (selectedRecipe.State == Offering.RecipeState.Discovered)
                    sb.DrawString(font, "Uncertain Item", new Vector2(scissorPane.X + 172, scissorPane.Y + 30), "Uncertain Item".LineCenter(font), ColorHelper.UI_LightGold, 1f);
                if (selectedRecipe.State == Offering.RecipeState.Known)
                    sb.DrawString(font, selectedRecipe.Name, new Vector2(scissorPane.X + 172, scissorPane.Y + 30), selectedRecipe.Name.LineCenter(font), ColorHelper.UI_LightGold, 1f);
                if (selectedRecipe.State == Offering.RecipeState.Perfected)
                {
                    sb.DrawString(font, selectedRecipe.Name, new Vector2(scissorPane.X + 172, scissorPane.Y + 30), selectedRecipe.Name.LineCenter(font), ColorHelper.UI_LightGold, 1f);
                    sb.DrawString(font, "(Deepened)", new Vector2(scissorPane.X + 172, scissorPane.Y + 55), "(Deepened)".LineCenter(font), ColorHelper.UI_GlowingGold, 1f);
                }

                if (selectedRecipe.State == Offering.RecipeState.Discovered)
                    sb.DrawString(font, discoveredDesc.WrapText(font, 260), new Vector2(scissorPane.X + 10, scissorPane.Y + 88), Color.White);
                if (selectedRecipe.State == Offering.RecipeState.Known || selectedRecipe.State == Offering.RecipeState.Perfected)
                {
                    for (int i = 0; i < descLines.Count; i++)
                    {
                        if (descLines[i].Position.Y <= 95)
                            sb.DrawString(descLines[i].Font, descLines[i].Text, descLines[i].Position + new Vector2(scissorPane.X + 10, scissorPane.Y + 88), descLines[i].Color);
                        else
                        {
                            sb.DrawString(descLines[i].Font, "...", descLines[i].Position + new Vector2(scissorPane.X + 10, scissorPane.Y + 88), descLines[i].Color);
                            break;
                        }
                    }
                }

                buttonThree.DrawButtonIdle(sb, halfTransparent);
                buttonFour.DrawButtonIdle(sb, halfTransparent);

                sb.DrawString(font, "Offer Items", buttonOne.Center, "Offer Item".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Offer Items (x10)", buttonTwo.Center, "Offer Items (x10)".LineCenter(font), Color.White, 1f);
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

        private void DrawIconState(SpriteBatch sb, Offering recipe, Vector2 position)
        {
            if (recipe.State == Offering.RecipeState.Discovered)
            {
                sb.Draw(recipe.Icon, new Vector2(position.X + 0, position.Y + 0), Color.Lerp(Color.Transparent, Color.Black, .25f));
                sb.Draw(recipe.Icon, new Vector2(position.X + 0, position.Y + 1), Color.Lerp(Color.Transparent, Color.Black, .25f));
                sb.Draw(recipe.Icon, new Vector2(position.X + 1, position.Y + 0), Color.Lerp(Color.Transparent, Color.Black, .25f));
                sb.Draw(recipe.Icon, new Vector2(position.X + 1, position.Y + 1), Color.Lerp(Color.Transparent, Color.Black, .25f));

                sb.Draw(unknown, position, Color.White);
            }
            if (recipe.State == Offering.RecipeState.Known)
            {
                sb.Draw(recipe.Icon, new Vector2(position.X, position.Y), Color.White);
            }
            if (recipe.State == Offering.RecipeState.Perfected)
            {
                sb.Draw(recipe.Icon, new Vector2(position.X, position.Y), Color.White);
                sb.Draw(ribbon, new Vector2(position.X + 1 + 64, position.Y), Color.White, new Vector2(ribbon.Width, 0), 0f, 1f);
            }
        }
        private void DrawButtonState(SpriteBatch sb, Offering recipe, Texture2D texture, Vector2 position)
        {
            if (recipe.State == Offering.RecipeState.Discovered)
                sb.Draw(texture, position, halfTransparent);
            if (recipe.State == Offering.RecipeState.Known)
                sb.Draw(texture, position, Color.White);
            if (recipe.State == Offering.RecipeState.Perfected)
                sb.Draw(texture, position, Color.White);
        }

        // --- Helper Methods ---
        /// <summary>
        /// Unlocks the recipe for use in the crafting window. Locked recipes will not appear here.
        /// </summary>
        /// <param name="recipeID"></param>
        public void UnlockOffering(int recipeID)
        {
            Offering recipe = OfferingByID(recipeID);

            if (recipe != null)
                recipe.State = Offering.RecipeState.Discovered;
        }

        private void AddOfferings()
        {
            AddConsumables();
            AddWeapons();
            AddArmor();
            AddAmmo();
            AddJewellery();
            AddResources();
            AddMiscellaneous();

            UnlockOffering(1);
            UnlockOffering(2);
            UnlockOffering(3);

            for (int i = 0; i < totalOfferings.Count; i++)
                totalOfferings[i].State = Offering.RecipeState.Discovered;
        }
        //0-999
        private void AddConsumables()
        {

        }
        //1000-1999
        private void AddWeapons()
        {

        }
        //2000-2999
        private void AddArmor()
        {

        }
        //3000-3999
        private void AddAmmo()
        {
            //Arrows
            totalOfferings.Add(new Offering(3000, 50, Add(new Point(4901, 1), new Point(4951, 1)), Add(new Point(3000, 5)), Add(new Point(3000, 8)), 30)); //Spearstone
            totalOfferings.Add(new Offering(3001, 100, Add(new Point(4901, 1), new Point(5020, 1)), Add(new Point(3002, 5)), Add(new Point(3002, 8)), 30)); //Iron bolt

            //Bolts
            totalOfferings.Add(new Offering(3500, 50, Add(new Point(4901, 1), new Point(4951, 1)), Add(new Point(3001, 5)), Add(new Point(3001, 8)), 30)); //Spearstone
            totalOfferings.Add(new Offering(3501, 100, Add(new Point(4901, 1), new Point(5020, 1)), Add(new Point(3003, 5)), Add(new Point(3003, 8)), 30)); //Iron bolt
        }
        //4000-4999
        private void AddJewellery()
        {
        }
        //5000-5999
        private void AddResources()
        {
            //Woods
            totalOfferings.Add(new Offering(5000, 50, null, Add(new Point(4901, 1)), Add(new Point(4901, 2)), 50));

            //Stones
            totalOfferings.Add(new Offering(5050, 50, null, Add(new Point(4950, 1)), Add(new Point(4950, 2)), 50));
            totalOfferings.Add(new Offering(5051, 75, null, Add(new Point(4951, 1)), Add(new Point(4951, 2)), 50));

            //Fuel
            totalOfferings.Add(new Offering(5100, 200, Add(new Point(4950, 1)), Add(new Point(5050, 1)), Add(new Point(5050, 2)), 50));
            totalOfferings.Add(new Offering(5101, 300, Add(new Point(4950, 2)), Add(new Point(5051, 1)), Add(new Point(5051, 2)), 30));

            //Minerals
            totalOfferings.Add(new Offering(5120, 250, Add(new Point(4950, 3)), Add(new Point(5000, 1)), Add(new Point(5000, 2)), 50));
        }
        //6000-6999
        private void AddMiscellaneous()
        {
            //Tools
            totalOfferings.Add(new Offering(6000, 1000, Add(new Point(5020, 2)), Add(new Point(7005, 1)), Add(new Point(7005, 1)), 1));
            totalOfferings.Add(new Offering(6001, 100, null, Add(new Point(7050, 1)), Add(new Point(7050, 2)), 50));
            totalOfferings.Add(new Offering(6002, 1000, Add(new Point(4950, 5)), Add(new Point(7499, 1)), Add(new Point(7499, 2)), 50));

        }
        private List<Point> Add(params Point[] ingredients)
        {
            return ingredients.ToList();
        }

        public Offering OfferingByID(int id)
        {
            for (int i = 0; i < totalOfferings.Count; i++)
            {
                if (totalOfferings[i].ID == id)
                    return totalOfferings[i];
            }

            return null;
        }

        public  StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Offerings (Total: " + totalOfferings.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            totalOfferings.OrderBy(x => x.ID);

            for (int i = 0; i < totalOfferings.Count; i++)
            {
                builder.AppendLine(totalOfferings[i].ID + " - " + totalOfferings[i].Name + " [IsRefinable: " + totalOfferings[i].IsImprovableOffering + "]");
            }

            return builder;
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            for (int i = 0; i < totalOfferings.Count; i++)
            {
                //Ignore if the recipe hasn't been discovered -- no data to bother saving, or not the right window to save from.
                if (totalOfferings[i].State != Offering.RecipeState.Unknown)
                    builder.AppendLine(totalOfferings[i].SaveData());
            }

            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            currentOfferings.Clear();

            for (int i = 0; i < data.Count; i++)
            {
                int id = int.Parse(data[i].Split(' ')[0]);
                Offering recipe = OfferingByID(id);

                if (recipe != null)
                {
                    recipe.State = Offering.RecipeState.Discovered;
                    recipe.LoadData(data[i]);
                }
            }

            UpdateList(BaseItem.TabType.Consumables);
        }

        public void ResetRecipes() { currentOfferings.Clear(); }

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return clickCheck.Contains(controls.MousePosition) || isTabHover || isDragging || hideRect.Contains(controls.MousePosition);
            else
                return false;
        }

        public void ResetPosition()
        {
            Position = new Vector2(GameSettings.VectorCenter.X - (tab.Width / 2), GameSettings.VectorCenter.Y - tab.Height / 2);
        }
    }
}
