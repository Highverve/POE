using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using System.Text;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    class BrewRecipe
    {
        private Point primaryIngredient, secondaryIngredient, combiner, output;
        private string name, description;
        private int brewTime;
        
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public int BrewTime { get { return brewTime; } }

        public int PrimaryID { get { return primaryIngredient.X; } }
        public int SecondaryID { get { return secondaryIngredient.X; } }
        public int CombinerID { get { return combiner.X; } }
        public int OutputID { get { return output.X; } }

        public int PrimaryQuantity { get { return primaryIngredient.Y; } }
        public int SecondaryQuantity { get { return secondaryIngredient.Y; } }
        public int CombinerQuantity { get { return combiner.Y; } }
        public int OutputQuantity { get { return output.Y; } }

        public bool IsDiscovered { get; set; }
        public Rectangle RecipeRect { get; set; }
        public bool IsHover { get; set; }

        public int BrewCount { get; set; }

        public BrewRecipe(string Name, string Description, int BrewTime, int PrimaryID, int PrimaryQuantity,
                          int SecondaryID, int SecondaryQuantity, int CombinerID, int CombinerQuantity,
                          int OutputID, int OutputQuantity)
        {
            name = Name;
            description = Description;
            brewTime = BrewTime;

            primaryIngredient = new Point(PrimaryID, PrimaryQuantity);
            secondaryIngredient = new Point(SecondaryID, SecondaryQuantity);
            combiner = new Point(CombinerID, CombinerQuantity);

            output = new Point(OutputID, OutputQuantity);
        }

        public bool IsMatching(int primaryID, int primaryQuantity, int secondaryID, int secondaryQuantity, int combinerID, int combinerQuantity)
        {
            if (PrimaryID != -1)
            {
                if (PrimaryID != primaryID)
                    return false;
                if (PrimaryQuantity > primaryQuantity)
                    return false;
            }

            if (SecondaryID != -1)
            {
                if (SecondaryID != secondaryID)
                    return false;
                if (SecondaryQuantity > secondaryQuantity)
                    return false;
            }

            if (CombinerID != -1)
            {
                if (CombinerID != combinerID)
                    return false;
                if (CombinerQuantity > combinerQuantity)
                    return false;
            }

            return true;
        }

        public string SaveData()
        {
            return "Recipe " + BrewCount + " \"" + name + "\"";
        }
        public void LoadData(string line)
        {
            if (line.FromWithin('"', 1).ToUpper().Equals(name.ToUpper()))
            {
                string[] words = line.Split(' ');
                IsDiscovered = true;
                BrewCount = int.Parse(words[1]);
            }
        }
    }

    public class BrewmastersContrivances : BaseScreen
    {
        private Texture2D bg, iconButton, iconButtonHover, smallButton, smallButtonHover, slotConnector, progressBar, pixel, recipeButton, recipeButtonHover, fillBottle, emptyBottle, startBrew;
        private SpriteFont font, boldFont;

        private Rectangle primarySlot, secondarySlot, combinerSlot, fillBottleButton, startBrewingButton;
        private bool isPrimaryHover, isSecondaryHover, isCombinerHover, isFillHover, isBrewHover;

        private BaseItem primaryIngredient, secondaryIngredient, combinerItem;

        private int currentTime, maxTime;
        private bool isBrewing = false;

        private List<BrewRecipe> recipes = new List<BrewRecipe>();
        private Rectangle scissorRect;
        private float recipeScrollPosition, scrollValue, scrollVelocity;

        public bool IsPrimaryHover { get { return isPrimaryHover; } }
        public bool IsSecondaryHover { get { return isSecondaryHover; } }
        public bool IsCombinerHover { get { return isCombinerHover; } }

        private int failedPotionID;
        private List<Point> fillItems = new List<Point>();
        private List<Point> emptyItems = new List<Point>();

        public BrewmastersContrivances() { IsActive = false; }

        public override void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            boldFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            bg = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Brewing/bg");
            recipeButton = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Brewing/recipebutton");
            recipeButtonHover = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Brewing/recipeButtonHover");

            iconButton = cm.Load<Texture2D>("Interface/Global/iconBG");
            iconButtonHover = cm.Load<Texture2D>("Interface/Global/iconBGSelect");

            smallButton = cm.Load<Texture2D>("Interface/Global/smallButton");
            smallButtonHover = cm.Load<Texture2D>("Interface/Global/smallButtonHover");

            slotConnector = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/slotConnector");
            progressBar = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/progressBar");

            fillBottle = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Brewing/fillBottle");
            emptyBottle = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Brewing/emptyBottle");
            startBrew = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Brewing/beginBrew");

            pixel = cm.Load<Texture2D>("rect");

            Position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - (bg.Height / 2));
            mouseDragOffset = new Vector2(92, 12);

            failedPotionID = 7052;
            SetRecipes();
            SetFillItems();
            SetEmptyItems();

            base.Load(cm);
        }
        public void SetRecipes()
        {
            recipes.Add(new BrewRecipe("Tonic of Force", "\n\nPrimary: x2 Brewer's Foot\nSecondary: x1 Rising Dawn\nBottle Slot: Filled Bottle",
                                       10000, 4700, 2, 4800, 1, 7051, 1, 50, 1));
            recipes.Add(new BrewRecipe("Spearguard's Stimulant", "\n\nPrimary: x2 Brewer's Foot\nSecondary: x1 Bonecap Mushroom\nBottle Slot: Filled Bottle",
                           10000, 4700, 2, 4801, 1, 7051, 1, 51, 1));
            recipes.Add(new BrewRecipe("Drop of Celerity", "\n\nPrimary: x2 Brewer's Foot\nSecondary: x1 Saint's Herb\nBottle Slot: Filled Bottle",
               10000, 4700, 2, 4802, 1, 7051, 1, 53, 1));
            recipes.Add(new BrewRecipe("Stilling Concoction", "\n\nPrimary: x2 Brewer's Foot\nSecondary: x1 Green Clover\nBottle Slot: Filled Bottle",
               10000, 4700, 2, 4803, 1, 7051, 1, 52, 1));
        }
        private void SetFillItems()
        {
            fillItems.Add(new Point(7050, 7051));
        }
        private void SetEmptyItems()
        {
            emptyItems.Add(new Point(7051, 7050));
            emptyItems.Add(new Point(7052, 7050));
        }

        private string hints = "Brewmaster's Contrivances Tips:\n\n" +
            "In order, there are three slots:\n- Primary Ingredient slot (top-left)\n- Secondary Ingredient slot (top-right)\n- Container slot (bottom-center)\n\n" +
            "Similar to other interfaces, drag and drop the respective items to their slots.\n" +
            "Most brews require water, so fill a container with water by clicking the Fill Container\n" +
            "button on the left side. You may also empty a container by clicking the button again.\n\n" +
            "Once your items are set, click the \"Brew Ingredients\" button to begin. The progress\n" +
            "bar in the center will begin increasing.\n\n" +
            "At the bottom side are the successfully brewed recipes. Hover over a name to view it's\n" +
            "required ingredients. Additionally, if you brew the wrong ingredients, a failed potion\n" +
            "will be added to your inventory.";

        public override void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                clickRect = new Rectangle((int)Position.X, (int)Position.Y, bg.Width, bg.Height);

                CheckDrag(new Rectangle((int)Position.X + 43, (int)Position.Y, 98, 20));
                UpdateWindowButtons(gt, 151, hints, "Hide Brewmaster's Contrivances");

                UpdateSlots(gt);
                UpdateRecipes(gt);
            }

            UpdateBrewing(gt);

            base.Update(gt);
        }
        private void UpdateSlots(GameTime gt)
        {
            isPrimaryHover = false; isSecondaryHover = false; isCombinerHover = false; isFillHover = false; isBrewHover = false;

            primarySlot = new Rectangle((int)Position.X + 21, (int)Position.Y + 59, iconButton.Width, iconButton.Height);
            secondarySlot = new Rectangle((int)Position.X + 149, (int)Position.Y + 59, iconButton.Width, iconButton.Height);
            combinerSlot = new Rectangle((int)Position.X + 85, (int)Position.Y + 123, iconButton.Width, iconButton.Height);

            fillBottleButton = new Rectangle((int)Position.X + 50, (int)Position.Y + 151, smallButton.Width, smallButton.Height);
            startBrewingButton = new Rectangle((int)Position.X + 154, (int)Position.Y + 151, smallButton.Width, smallButton.Height);

            if (primarySlot.Contains(controls.MousePosition))
            {
                isPrimaryHover = true;

                if (primaryIngredient != null)
                {
                    ToolTip.RequestStringAssign(primaryIngredient.Name + "\n\nLeft-click: remove item");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        ReturnItem(ref primaryIngredient);

                        if (primaryIngredient == null)
                            RefreshRecipe();
                    }
                }
                else
                    ToolTip.RequestStringAssign("Primary Ingredient Slot");
            }

            if (secondarySlot.Contains(controls.MousePosition))
            {
                isSecondaryHover = true;

                if (secondaryIngredient != null)
                {
                    ToolTip.RequestStringAssign(secondaryIngredient.Name + "\n\nLeft-click: remove item");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        ReturnItem(ref secondaryIngredient);

                        if (secondaryIngredient == null)
                            RefreshRecipe();
                    }
                }
                else
                    ToolTip.RequestStringAssign("Secondary Ingredient Slot");
            }

            if (combinerSlot.Contains(controls.MousePosition))
            {
                isCombinerHover = true;

                if (combinerItem != null)
                {
                    ToolTip.RequestStringAssign(combinerItem.Name + "\n\nLeft-click: remove item");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        ReturnItem(ref combinerItem);

                        if (combinerItem == null)
                            RefreshRecipe();
                    }
                }
                else
                    ToolTip.RequestStringAssign("Container Slot");
            }

            if (fillBottleButton.Contains(controls.MousePosition))
            {
                isFillHover = true;

                if (fillState == FillableState.Fill)
                    ToolTip.RequestStringAssign("Fill Container");
                if (fillState == FillableState.Empty)
                    ToolTip.RequestStringAssign("Empty Container");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    RefreshRecipe();

                    screens.PlaySound("Button Click 6");

                    switch (fillState)
                    {
                        case FillableState.Fill: FillBottle(); break;
                        case FillableState.Empty: EmptyBottle(); break;
                    }

                    RefreshRecipe();
                }
            }

            if (startBrewingButton.Contains(controls.MousePosition))
            {
                isBrewHover = true;

                if (isBrewing == false)
                    ToolTip.RequestStringAssign("Brew Ingredients");
                else
                    ToolTip.RequestStringAssign("Stop Brewing");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    RefreshRecipe();

                    screens.PlaySound("Button Click 6");

                    if (isBrewing == false)
                    {
                        if (primaryIngredient != null || secondaryIngredient != null || combinerItem != null)
                            BeginBrewing();
                        else
                            screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "There is nothing to brew!");
                    }
                    else
                        ForceStopBrewing();
                }
            }
        }

        private bool setSoundPlay = false;
        private void UpdateBrewing(GameTime gt)
        {
            if (isBrewing == true)
            {
                if (setSoundPlay == false)
                {
                    screens.PlaySound("BrewingLoop");
                    setSoundPlay = true;
                }

                currentTime += gt.ElapsedGameTime.Milliseconds;

                if (currentTime >= maxTime)
                {
                    if (currentRecipe != null)
                    {
                        if (currentRecipe.IsDiscovered == false)
                        {
                            currentRecipe.IsDiscovered = true;
                            screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "\"" + currentRecipe.Name + "\" discovered");
                        }
                    }

                    int iterations = 0;

                    //Add item to proper storage.
                    if (currentRecipe != null)
                        AddItem(currentRecipe.OutputID, currentRecipe.OutputQuantity, ref iterations); //Adds the expected potion if the ingredients match a recipe!
                    else
                    {
                        iterations = 1;
                        AddItem(failedPotionID, 1); //Adds a failed potion if the ingredients do not match a recipe!
                    }

                    if (iterations > 0)
                    {
                        //If the item was brewed successfully, subtract recipe ingredients
                        UseSlot(primaryIngredient);
                        UseSlot(secondaryIngredient);
                        UseSlot(combinerItem);
                    }

                    //Re-check ingredient validity and recipe
                    RefreshRecipe();
                }
            }

            if (isBrewing == false || IsActive == false)
            {
                if (setSoundPlay == true)
                {
                    screens.StopSound("BrewingLoop");
                    setSoundPlay = false;
                }
            }
        }
        private void AddItem(int id, int quantity)
        {
            if (IsActive == true)
            {
                BaseItem item = controlledEntity.STORAGE_GetItem(id);

                if (item != null)
                {
                    if (item.CurrentAmount < item.MaxAmount)
                        controlledEntity.STORAGE_AddItem(id, quantity, false, true);

                    //If the controlled entity's item stack has no more room, force stop brewing.
                    if (item.CurrentAmount == item.MaxAmount)
                        ForceStopBrewing();
                }
                else
                    controlledEntity.STORAGE_AddItem(id, quantity, false, true);
            }
            else
            {
                BaseItem item = ItemDatabase.Item(id);

                if (item != null)
                {
                    item = item.Copy(null, null, null, null);

                    item.CurrentAmount = quantity;
                    screens.STONEHOLD_DepositItem(item);
                }
            }
        }
        private void AddItem(int id, int quantity, ref int iterations)
        {
            if (IsActive == true)
            {
                BaseItem item = controlledEntity.STORAGE_GetItem(id);

                if (item != null)
                {
                    if (item.CurrentAmount < item.MaxAmount)
                        controlledEntity.STORAGE_AddItem(id, quantity, false, true);

                    //If the controlled entity's item stack has no more room, force stop brewing.
                    if (item.CurrentAmount == item.MaxAmount)
                        ForceStopBrewing();

                    iterations++;
                }
                else
                    controlledEntity.STORAGE_AddItem(id, quantity, false, true);
            }
            else
            {
                screens.STONEHOLD_AddItem(id, quantity);
                iterations++;
            }
        }
        private void ReturnItem(ref BaseItem item)
        {
            if (item != null)
            {
                if (item.MaxAmount > 1)
                {
                    int decrease = controlledEntity.STORAGE_AddItemGetDifference(item.ID, item.CurrentAmount, false, false);
                    item.CurrentAmount -= decrease;

                    if (item.CurrentAmount <= 0)
                        item = null;
                }
                else
                {
                    controlledEntity.STORAGE_AddItem(item);
                    item = null;
                }

                RefreshRecipe();
            }
        }

        private void UseSlot(BaseItem item)
        {
            if (item != null)
            {
                if (currentRecipe != null)
                    item.CurrentAmount -= currentRecipe.CombinerQuantity;
                else
                    item.CurrentAmount -= 1;

                if (item.CurrentAmount < 1)
                    item = null;
            }
        }

        private int recipeIndex;
        private void UpdateRecipes(GameTime gt)
        {
            scissorRect = new Rectangle((int)Position.X + 16, (int)Position.Y + 197, 204, 106);

            SmoothScroll(gt, 30f, 200f, 300f, 10f, ref recipeScrollPosition, ref scrollValue, ref scrollVelocity, -(((recipeIndex * (recipeButton.Height + 1)) - scissorRect.Height) + 1), scissorRect);

            recipeIndex = 0;
            for (int i = 0; i < recipes.Count; i++)
            {
                if (recipes[i].IsDiscovered == true)
                {
                    recipes[i].RecipeRect = new Rectangle((int)Position.X + 17, (int)Position.Y + 198 + (recipeIndex * (recipeButton.Height + 1) + (int)recipeScrollPosition), recipeButton.Width, recipeButton.Height);
                    recipeIndex++;

                    if (recipes[i].RecipeRect.Contains(controls.MousePosition))
                    {
                        recipes[i].IsHover = true;
                        ToolTip.RequestStringAssign(recipes[i].Description);
                    }
                    else
                        recipes[i].IsHover = false;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                //Draw BG
                sb.Draw(bg, Position, Color.White);

                sb.DrawString(boldFont, "Brewer", Position + new Vector2(93, 12), "Brewer".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

                //Progress bar
                sb.Draw(pixel, new Rectangle((int)Position.X + 91, (int)Position.Y + 93, (int)((progressBar.Width - 4) * ((float)currentTime / maxTime)), 2), Color.BlueViolet);
                sb.Draw(progressBar, Position + new Vector2(89, 91), Color.White);

                //Connectors
                sb.Draw(slotConnector, Position + new Vector2(58, 122), Color.White);
                sb.Draw(slotConnector, Position + new Vector2(149, 122), Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.FlipHorizontally, 1f);

                //Icon button drawing
                if (isPrimaryHover == true)
                    sb.Draw(iconButtonHover, primarySlot, Color.White);
                else
                    sb.Draw(iconButton, primarySlot, Color.White);

                if (isSecondaryHover == true)
                    sb.Draw(iconButtonHover, secondarySlot, Color.White);
                else
                    sb.Draw(iconButton, secondarySlot, Color.White);

                if (isCombinerHover == true)
                    sb.Draw(iconButtonHover, combinerSlot, Color.White);
                else
                    sb.Draw(iconButton, combinerSlot, Color.White);

                //Item icon drawing
                if (primaryIngredient != null)
                {
                    sb.Draw(primaryIngredient.Icon, primarySlot, Color.White);
                    sb.DrawString(boldFont, primaryIngredient.CurrentAmount.ToString(), primarySlot.Location.ToVector2() + new Vector2(4, 4), ColorHelper.UI_Gold);
                }
                if (secondaryIngredient != null)
                {
                    sb.Draw(secondaryIngredient.Icon, secondarySlot, Color.White);
                    sb.DrawString(boldFont, secondaryIngredient.CurrentAmount.ToString(), secondarySlot.Location.ToVector2() + new Vector2(4, 4), ColorHelper.UI_Gold);
                }
                if (combinerItem != null)
                {
                    sb.Draw(combinerItem.Icon, combinerSlot, Color.White);
                    sb.DrawString(boldFont, combinerItem.CurrentAmount.ToString(), combinerSlot.Location.ToVector2() + new Vector2(4, 4), ColorHelper.UI_Gold);
                }

                //Other button drawing
                if (isFillHover == true)
                    sb.Draw(smallButtonHover, fillBottleButton, Color.White);
                else
                    sb.Draw(smallButton, fillBottleButton, Color.White);

                switch(fillState)
                {
                    case FillableState.None: sb.Draw(fillBottle, fillBottleButton, Color.Lerp(Color.White, Color.Transparent, .5f)); break;
                    case FillableState.Fill: sb.Draw(fillBottle, fillBottleButton, Color.White); break;
                    case FillableState.Empty: sb.Draw(emptyBottle, fillBottleButton, Color.Lerp(Color.White, Color.Transparent, .25f)); break;
                }

                if (isBrewing == true || isBrewHover)
                {
                    sb.Draw(smallButtonHover, startBrewingButton, Color.White);
                    sb.Draw(startBrew, startBrewingButton, ColorHelper.UI_GlowingGold);
                }
                else
                {
                    sb.Draw(smallButton, startBrewingButton, Color.White);
                    sb.Draw(startBrew, startBrewingButton, Color.White);
                }

                base.Draw(sb);

                sb.End();

                DrawScissored(sb);
            }
        }
        public override void DrawScissored(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                DrawInside(sb, scissorRect, () =>
                {
                    for (int i = 0; i < recipes.Count; i++)
                    {
                        if (recipes[i].IsDiscovered == true)
                        {
                            if (recipes[i].IsHover == true)
                                sb.Draw(recipeButtonHover, recipes[i].RecipeRect, Color.White);
                            else
                                sb.Draw(recipeButton, recipes[i].RecipeRect, Color.White);

                            sb.DrawString(font, recipes[i].Name, recipes[i].RecipeRect.Location.ToVector2() + new Vector2(6, 10), new Vector2(0, font.MeasureString("A").Y / 2), ColorHelper.UI_Gold, 1f);
                        }
                    }
                });
            }

            base.DrawScissored(sb);
        }

        // [Methods]

        public void AddPrimaryIngredient(BaseItem item)
        {
            if (primaryIngredient != null)
                ReturnItem(ref primaryIngredient);

            primaryIngredient = item;

            RefreshRecipe();
        }
        public void AddSecondaryIngredient(BaseItem item)
        {
            if (secondaryIngredient != null)
                ReturnItem(ref secondaryIngredient);

            secondaryIngredient = item;

            RefreshRecipe();
        }
        public void AddCombiner(BaseItem item)
        {
            if (combinerItem != null)
                ReturnItem(ref combinerItem);

            combinerItem = item;

            RefreshRecipe();
        }

        public bool IsValidRecipe(int primaryID, int primaryQuantity, int secondaryID, int secondaryQuantity, int combinerID, int combinerQuantity)
        {
            if (RetrieveRecipe(primaryID, primaryQuantity, secondaryID, secondaryQuantity, combinerID, combinerQuantity) != null)
                return true;
            else
                return false;
        }
        private BrewRecipe RetrieveRecipe(int primaryID, int primaryQuantity, int secondaryID, int secondaryQuantity, int combinerID, int combinerQuantity)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                if (recipes[i].IsMatching(primaryID, primaryQuantity, secondaryID, secondaryQuantity, combinerID, combinerQuantity))
                    return recipes[i];
            }

            return null;
        }

        private BrewRecipe currentRecipe;
        private void RefreshRecipe()
        {
            int pID = -1, pQ = -1,
                sID = -1, sQ = -1,
                cID = -1, cQ = -1;

            if (primaryIngredient != null)
            {
                pID = primaryIngredient.ID;
                pQ = primaryIngredient.CurrentAmount;
            }
            if (secondaryIngredient != null)
            {
                sID = secondaryIngredient.ID;
                sQ = secondaryIngredient.CurrentAmount;
            }
            if (combinerItem != null)
            {
                cID = combinerItem.ID;
                cQ = combinerItem.CurrentAmount;
            }

            if (primaryIngredient == null && secondaryIngredient == null && combinerItem == null)
                ForceStopBrewing();

            currentRecipe = RetrieveRecipe(pID, pQ, sID, sQ, cID, cQ);
            currentTime = 0;

            if (currentRecipe != null)
                maxTime = currentRecipe.BrewTime;
            else
                maxTime = 10000;

            if (ValidEmptyItem() == Point.Zero && ValidFillItem() == Point.Zero)
                fillState = FillableState.None;
            else if (ValidFillItem() != Point.Zero)
                fillState = FillableState.Fill;
            else if (ValidEmptyItem() != Point.Zero)
                fillState = FillableState.Empty;
            else
                fillState = FillableState.Fill; //If both fill and empty are valid...
        }
        private void BeginBrewing()
        {
            isBrewing = true;
        }
        public void ForceStopBrewing()
        {
            isBrewing = false;
            currentTime = 0;

            currentRecipe = null;
        }

        private enum FillableState { None, Fill, Empty }
        private FillableState fillState = FillableState.None;
        private void FillBottle()
        {
            Point fillItem = ValidFillItem(); //Retrieve the correct point

            if (combinerItem != null)
            {
                if (fillItem != Point.Zero)
                {
                    int tempQuantity = combinerItem.CurrentAmount;
                    BaseItem item = ItemDatabase.Item(fillItem.Y);

                    if (item != null)
                    {
                        combinerItem = item.Copy(null, null, null, null);
                        combinerItem.CurrentAmount = tempQuantity;
                    }
                }
                else
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "You cannot fill this item!");
            }
            else
                screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "No item to fill!");
        }
        private Point ValidFillItem()
        {
            if (combinerItem != null)
            {
                for (int i = 0; i < fillItems.Count; i++)
                {
                    if (combinerItem.ID == fillItems[i].X)
                        return fillItems[i];
                }
            }

            return Point.Zero;
        }

        private void EmptyBottle()
        {
            Point emptyItem = ValidEmptyItem(); //Retrieve the correct point

            if (combinerItem != null)
            {
                if (emptyItem != Point.Zero)
                {
                    int tempQuantity = combinerItem.CurrentAmount;
                    BaseItem item = ItemDatabase.Item(emptyItem.Y);

                    if (item != null)
                    {
                        combinerItem = item.Copy(null, null, null, null);
                        combinerItem.CurrentAmount = tempQuantity;
                    }
                }
                else
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "You cannot empty this item!");
            }
            else
                screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "No item to empty!");
        }
        private Point ValidEmptyItem()
        {
            if (combinerItem != null)
            {
                for (int i = 0; i < emptyItems.Count; i++)
                {
                    if (combinerItem.ID == emptyItems[i].X)
                        return emptyItems[i];
                }
            }

            return Point.Zero;
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            if (primaryIngredient != null)
                builder.AppendLine("Primary " + primaryIngredient.SaveData());

            if (secondaryIngredient != null)
                builder.AppendLine("Secondary " + secondaryIngredient.SaveData());

            if (combinerItem != null)
                builder.AppendLine("Container " + combinerItem.SaveData());

            builder.AppendLine();

            for (int i = 0; i < recipes.Count; i++)
            {
                if (recipes[i].IsDiscovered == true)
                    builder.AppendLine(recipes[i].SaveData());
            }

            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string[] words = data[i].Split(' ');

                if (words[0].ToUpper().StartsWith("PRIMARY"))
                {
                    BaseItem item = ItemDatabase.Item(int.Parse(words[2])).Copy(screens, tileMap, controlledEntity, camera);
                    item.LoadData(data[i].Replace("Primary ", ""));

                    AddPrimaryIngredient(item);
                }
                else if (words[0].ToUpper().StartsWith("SECONDARY"))
                {
                    BaseItem item = ItemDatabase.Item(int.Parse(words[2])).Copy(screens, tileMap, controlledEntity, camera);
                    item.LoadData(data[i].Replace("Secondary ", ""));

                    AddSecondaryIngredient(item);
                }
                else if (words[0].ToUpper().StartsWith("CONTAINER"))
                {
                    BaseItem item = ItemDatabase.Item(int.Parse(words[2])).Copy(screens, tileMap, controlledEntity, camera);
                    item.LoadData(data[i].Replace("Container ", ""));

                    AddCombiner(item);
                }
                else if (words[0].ToUpper().StartsWith("RECIPE"))
                {
                    for (int j = 0; j < recipes.Count; j++)
                    {
                        if (data[i].FromWithin('"', 1).ToUpper().Equals(recipes[j].Name.ToUpper()))
                        {
                            recipes[j].LoadData(data[i]);
                            break;
                        }
                    }
                }               
            }
        }

        public StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Brews (Total: " + recipes.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            for (int i = 0; i < recipes.Count; i++)
            {
                builder.AppendLine(recipes[i].Name + " [Brew Time: " + recipes[i].BrewTime + ", Primary ID/Quantity: " + recipes[i].PrimaryID + "; " + recipes[i].PrimaryQuantity +
                                                                                             ", Secondary: " + recipes[i].SecondaryID + "; " + recipes[i].SecondaryQuantity +
                                                                                             ", Container: " + recipes[i].CombinerID + "; " + recipes[i].CombinerQuantity +
                                                                                             ", Output: " + recipes[i].OutputID + "; " + recipes[i].OutputQuantity + "]");
            }

            return builder;
        }

        public void ResetPosition()
        {
            Position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);
        }
    }
}
