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
using Pilgrimage_Of_Embers.ScreenEngine.Various;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate.Crafting
{
    public class ReinforceRecipe
    {
        public Dictionary<int, ItemHolder[]> ReinforceIngredients { get; private set; }

        public ReinforceRecipe(Dictionary<int, ItemHolder[]> ReinforceIngredients)
        {
            this.ReinforceIngredients = ReinforceIngredients;
        }
    }

    public class ReinforceWorkbench
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

        private Texture2D tab, pane, paneEnd, iconButton, iconButtonHover, paneButton, paneButtonHover, smallButton, smallButtonHover, tabButton, tabButtonHover, ribbon, unknown;
        private Texture2D consumables, weapons, armor, ammo, jewellery, resources, miscellaneous;
        private SpriteFont font, largeFont;

        private Rectangle consumableRect, weaponsRect, armorRect, ammoRect, jewelleryRect, resourcesRect, miscellaneousRect;
        private MenuButton buttonOne, buttonTwo, buttonThree, buttonFour; //Pane buttons.


        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        Dictionary<int, ReinforceRecipe> recipes = new Dictionary<int, ReinforceRecipe>();

        public bool IsActive { get; set; }
        private float scrollPosition = 0;
        private float currentPaneWidth = 0f, paneLerp = 0f; //Pane width. Increased when recipe is selected

        private BaseItem selectedItem = null;
        private ReinforceRecipe recipe = null;
        private bool isSelected = false;

        BaseItem.TabType currentTab = BaseItem.TabType.Consumables;

        private Controls controls = new Controls();
        private ScreenManager screens;
        private BaseEntity controlledEntity;

        private EntityStorage storage;
        private List<BaseItem> currentItems = new List<BaseItem>(); //The current items to be displayed.
        private List<BaseItem> totalItems = new List<BaseItem>();

        private SelectionBox confirmReinforce, confirmDeinforce;
        private bool askReinforce = true, askDeinforce = true;

        public ReinforceWorkbench() { }

        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;

            IsActive = false;
        }
        public void SetControlledEntity(BaseEntity controlledEntity, EntityStorage storage)
        {
            this.controlledEntity = controlledEntity;
            this.storage = storage;

            UpdateList(currentTab);
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

            buttonOne = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonTwo = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonThree = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);
            buttonFour = new MenuButton(Vector2.Zero, paneButton, paneButtonHover, paneButtonHover, 1f, true);

            confirmReinforce = new SelectionBox("Reinforce Item", "Are you certain you wish to reinforce this item?", "Yes, I am certain.", "No, I am not.", "Don't ask me this again.");
            confirmDeinforce = new SelectionBox("Restore Item", "This will restore the item to it's original state. All reinforcements will be lost.", "Yes, this is what I want.", "No, I don't want this.");

            confirmReinforce.Load(cm);
            confirmDeinforce.Load(cm);

            AddRecipes();
            UpdateList(BaseItem.TabType.Consumables);
        }

        private void AddRecipes()
        {
            AddRecipe(52, AssignDictionary(new Tuple<int, ItemHolder[]>(0, AssignIngredients(new Point(1, 10), new Point(2, 1))),
                                           new Tuple<int, ItemHolder[]>(1, AssignIngredients(new Point(6001, 1), new Point(5050, 2)))));

            AddRecipe(102, AssignDictionary(new Tuple<int, ItemHolder[]>(0, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(1, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(2, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(3, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(4, AssignIngredients(new Point(1, 1)))));
            AddRecipe(103, AssignDictionary(new Tuple<int, ItemHolder[]>(0, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(1, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(2, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(3, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(4, AssignIngredients(new Point(1, 1)))));
            AddRecipe(104, AssignDictionary(new Tuple<int, ItemHolder[]>(0, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(1, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(2, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(3, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(4, AssignIngredients(new Point(1, 1)))));
            AddRecipe(105, AssignDictionary(new Tuple<int, ItemHolder[]>(0, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(1, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(2, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(3, AssignIngredients(new Point(1, 1))),
                                new Tuple<int, ItemHolder[]>(4, AssignIngredients(new Point(1, 1)))));
        }
        private void AddRecipe(int id, Dictionary<int, ItemHolder[]> levels)
        {
            recipes.Add(id, new ReinforceRecipe(levels));

            ItemDatabase.Item(id).MaxReinforcement = levels.Count;
        }
        private Dictionary<int, ItemHolder[]> AssignDictionary(params Tuple<int, ItemHolder[]>[] levels)
        {
            Dictionary<int, ItemHolder[]> ingredients = new Dictionary<int, ItemHolder[]>();

            for (int i = 0; i < levels.Length; i++)
                ingredients.Add(levels[i].Item1, levels[i].Item2);

            return ingredients;
        }
        private ItemHolder[] AssignIngredients(params Point[] ingredients)
        {
            ItemHolder[] ingreds = new ItemHolder[ingredients.Length];

            for (int i = 0; i < ingreds.Length; i++)
            {
                BaseItem item = ItemDatabase.Item(ingredients[i].X);

                if (item != null)
                    ingreds[i] = new ItemHolder(ingredients[i].X, ingredients[i].Y, item.Name, item.Icon);
            }

            return ingreds;
        }

        private string hints = "Reinforcer's Workbench Tips:\n\n" +
            "Reinforcing an item makes it stronger and more effective. To reinforce\n" +
            "an item, click it, and a pane on the right side will slide open.\n\n" +
            "The required items will be listed towards the bottom.";

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
                    ToolTip.RequestStringAssign("Hide Reinforcer's Workbench");

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
        private void UpdateItems(GameTime gt)
        {
            isSelected = false;
            ApplyProperItemGrid();

            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].itemRect.Contains(controls.MousePosition))
                {
                    if (currentItems[i].HasReinforceIngredients)
                        ToolTip.RequestStringAssign(currentItems[i].Name + " (" + currentItems[i].ReinforcmentName() + ")");
                    else
                        ToolTip.RequestStringAssign(currentItems[i].Name + "\n\nYou lack necessary ingredients.");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Item Select");
                        currentItems[i].isSelected = true;
                    }
                }
                else if (!scissorPane.Contains(controls.MousePosition) &&
                         !isDragging &&
                         !confirmDeinforce.IsClickingUI() &&
                         !confirmReinforce.IsClickingUI())
                {
                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        currentItems[i].isSelected = false;
                }

                if (currentItems[i].isSelected == true)
                {
                    selectedItem = currentItems[i];
                    recipe = recipes[currentItems[i].ID];
                    isSelected = true;
                }
            }

            if (limitIngredientCheck.IsCalling(gt)) //Call periodically, but not constantly. Noticable drop in FPS (30-50) when constantly checking.
                CheckIngredients();
        }
        private void CheckIngredients()
        {
            for (int i = 0; i < currentItems.Count; i++)
                currentItems[i].HasReinforceIngredients = HasIngredients(currentItems[i], recipes[currentItems[i].ID]);
        }
        private void CheckIngredients(BaseItem item, ReinforceRecipe recipe)
        {
            item.HasReinforceIngredients = HasIngredients(item, recipe);
        }

        private void UpdateButtons(GameTime gt)
        {
            confirmReinforce.Update(gt);
            confirmDeinforce.Update(gt);

            buttonOne.Update(gt, controls);
            buttonTwo.Update(gt, controls);
            buttonThree.Update(gt, controls);
            buttonFour.Update(gt, controls);

            buttonOne.Position = new Point((int)position.X + 457, (int)position.Y + 307);
            buttonTwo.Position = new Point((int)position.X + 457, (int)position.Y + 328);
            buttonThree.Position = new Point((int)position.X + 457, (int)position.Y + 349);
            buttonFour.Position = new Point((int)position.X + 457, (int)position.Y + 370);

            if (selectedItem != null && recipe != null)
            {
                if (selectedItem.CurrentReinforcement < selectedItem.MaxReinforcement)
                {
                    for (int i = 0; i < recipe.ReinforceIngredients[selectedItem.CurrentReinforcement].Length; i++)
                    {
                        recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Rect = new Rectangle((int)position.X + 434 + (34 * i), (int)position.Y + 270, 32, 32);

                        if (recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Rect.Contains(controls.MousePosition))
                            ToolTip.RequestStringAssign(recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Name + " (x" + recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Quantity + ")");
                    }
                }

                if (buttonOne.IsHover == true)
                    ToolTip.RequestStringAssign("Reinforce Item");

                if (buttonOne.IsLeftClicked == true)
                {
                    if (askReinforce == true)
                        confirmReinforce.IsActive = true;
                    else
                        ReinforceItem(selectedItem, recipe);
                }
                if (buttonTwo.IsLeftClicked == true)
                {
                    if (askDeinforce == true)
                        confirmDeinforce.IsActive = true;
                    else
                        selectedItem.DeinforceAll();
                }

                if (confirmReinforce.IsActive == true)
                {
                    if (confirmReinforce.CurrentSelection == 0)
                    {
                        ReinforceItem(selectedItem, recipe);
                        confirmReinforce.IsActive = false;
                    }
                    if (confirmReinforce.CurrentSelection == 1)
                        confirmReinforce.IsActive = false;
                    if (confirmReinforce.CurrentSelection == 2)
                        askReinforce = false;
                }

                if (confirmDeinforce.IsActive == true)
                {
                    if (confirmDeinforce.CurrentSelection == 0)
                    {
                        selectedItem.DeinforceAll();
                        confirmDeinforce.IsActive = false;
                    }
                    if (confirmDeinforce.CurrentSelection == 1)
                        confirmDeinforce.IsActive = false;
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

        private void UpdateList(BaseItem.TabType type)
        {
            totalItems = storage.TotalItems();
            currentItems.Clear();

            for (int i = 0; i < totalItems.Count; i++)
            {
                if (recipes.ContainsKey(totalItems[i].ID))
                {
                    if (totalItems[i].tabType == type && totalItems[i].MaxReinforcement > 0) //Add recipe to list if it's the same tab type and state is not unknown
                    {
                        if (!currentItems.Contains(totalItems[i]))
                            currentItems.Add(totalItems[i]);
                    }
                }
            }

            //Or sort by A-Z. Player choice.
        }

        private void ReinforceItem(BaseItem item, ReinforceRecipe recipe)
        {
            if (item != null && recipe != null)
            {
                CheckIngredients(item, recipe); //Force check recipe before 

                if (item.IsReinforceable())
                {
                    if (item.HasReinforceIngredients)
                    {
                        for (int i = 0; i < recipe.ReinforceIngredients[item.CurrentReinforcement].Length; i++)
                            controlledEntity.STORAGE_RemoveItem(recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].ID, recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Quantity);

                        item.Reinforce();

                        screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, selectedItem.Name + " reinforced");
                    }
                    else
                    {
                        screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "You lack the necessary ingredients");
                    }
                }
                else
                {
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, selectedItem.Name + " cannot be further reinforced");
                }
            }
        }

        public bool HasIngredients(BaseItem item, ReinforceRecipe recipe)
        {
            bool hasIngredients = true;

            if (item.CurrentReinforcement < item.MaxReinforcement)
            {
                foreach (ItemHolder i in recipe.ReinforceIngredients[item.CurrentReinforcement])
                {
                    if (!controlledEntity.STORAGE_Check(i.ID, i.Quantity))
                        hasIngredients = false;
                }
            }
            else
                return false;

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


                sb.DrawString(largeFont, "Reinforcer's Workbench", position + new Vector2(tab.Width / 2, 12), "Reinforcer's Workbench".LineCenter(largeFont), ColorHelper.UI_LightGold, 1f);
                sb.DrawString(largeFont, currentTab.ToString(), position + new Vector2(tab.Width / 2, 47), currentTab.ToString().LineCenter(largeFont), ColorHelper.UI_LightGold, 1f);

                sb.End();

                DrawInside(sb);
                DrawPane(sb);

                sb.Begin();

                confirmReinforce.Draw(sb);
                confirmDeinforce.Draw(sb);

                sb.End();
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
            buttonTwo.DrawButton(sb, Color.White);
            buttonThree.DrawButtonIdle(sb, halfTransparent);
            buttonFour.DrawButtonIdle(sb, halfTransparent);

            if (selectedItem != null && recipe != null)
            {
                DrawButtonState(sb, selectedItem, iconButton, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));
                DrawIconState(sb, selectedItem, new Vector2(scissorPane.X + 1, scissorPane.Y + 13));

                if (selectedItem.CurrentReinforcement < selectedItem.MaxReinforcement)
                {
                    for (int i = 0; i < recipe.ReinforceIngredients[selectedItem.CurrentReinforcement].Length; i++)
                    {
                        DrawIngredient(sb, recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Rect,
                                           recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Icon,
                                           recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].ID,
                                           recipe.ReinforceIngredients[selectedItem.CurrentReinforcement][i].Quantity);
                    }
                }

                sb.DrawString(font, selectedItem.Name, new Vector2(scissorPane.X + 172, scissorPane.Y + 30), selectedItem.Name.LineCenter(font), ColorHelper.UI_LightGold, 1f);
                sb.DrawString(font, selectedItem.Description.WrapText(font, 260), new Vector2(scissorPane.X + 10, scissorPane.Y + 88), Color.White);

                sb.DrawString(font, "Reinforce Item", buttonOne.Center, "Reinforce Item".LineCenter(font), Color.White, 1f);
                sb.DrawString(font, "Scrap Item", buttonTwo.Center, "Scrap Item".LineCenter(font), Color.White, 1f);
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

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return confirmDeinforce.IsClickingUI() || confirmReinforce.IsClickingUI() || clickRect.Contains(controls.MousePosition) || isTabHover || hideRect.Contains(controls.MousePosition) || isDragging;
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
            builder.AppendLine("    Reinforcing (Total: " + recipes.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            int index = 0;
            foreach (KeyValuePair<int, ReinforceRecipe> item in recipes)
            {
                builder.AppendLine(ItemDatabase.Item(item.Key).Name + "[ID: " + item.Key + "]");

                foreach (KeyValuePair<int, ItemHolder[]> level in item.Value.ReinforceIngredients)
                {
                    builder.Append("    - Level " + level.Key + ": ");

                    for (int j = 0; j < level.Value.Length; j++)
                    {
                        builder.Append(level.Value[j].Name + " x" + level.Value[j].Quantity);

                        if (j < level.Value.Length - 1) //Not the last item in the index, then...
                            builder.Append(", "); //Slap a comma on it's ass!
                        else
                            builder.Append("\n");
                    }
                }

                index++;

                if (index != recipes.Count)
                    builder.AppendLine(); //Space it out between the others.
            }

            return builder;
        }
    }
}
