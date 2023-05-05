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
using System.Text;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate.Crafting
{
    public class SmelterFuel
    {
        int burnTime = 10000; //In milliseconds, how long the fuel takes to burn
        int temperatureRate = 5; //Applied every 1/10 of a second
        float smeltSpeed = 1f; //In 0-1 percentage, the speed multiplier of the metal smelt speed
        int burnedItemID = -1; //Once fuel has been burned, this is the item that is given to the player
        int maxTemperature = 2000;

        public int BurnTime { get { return burnTime; } }
        public int TemperatureRate { get { return temperatureRate; } }
        public int MaxTemperature { get { return maxTemperature; } }
        public float SmeltSpeed { get { return smeltSpeed; } }
        public int BurnedItemID { get { return burnedItemID; } }

        public SmelterFuel(int BurnTime, int TemperatureRate, int MaxTemperature, float SmeltSpeed, int BurnedItemID)
        {
            burnTime = BurnTime;
            temperatureRate = TemperatureRate;
            maxTemperature = MaxTemperature;
            smeltSpeed = SmeltSpeed;
            burnedItemID = BurnedItemID;
        }
    }

    public class SmelterOre
    {
        int oreID, modifierID;
        Point outputIDQuantity1, outputIDQuantity2;
        int meltingPoint;
        int meltingTime;

        public int OreID { get { return oreID; } }
        public int ModifierID { get { return modifierID; } }
        public Point OutputOne { get { return outputIDQuantity1; } }
        public Point OutputTwo { get { return outputIDQuantity2; } }
        public int MeltingPoint { get { return meltingPoint; } }
        public int MeltingTime { get { return meltingTime; } }

        public SmelterOre(int OreID, int ModifierID, Point Output1, Point Output2, int MeltingPoint, int MeltingTime)
        {
            oreID = OreID;
            modifierID = ModifierID;

            outputIDQuantity1 = Output1;
            outputIDQuantity2 = Output2;

            meltingPoint = MeltingPoint;
            meltingTime = MeltingTime;
        }
    }

    public class OreSmelter
    {
        private Vector2 position;
        private Vector2 Position { set { position = value; } }

        private Texture2D bg, iconBG, smallButton, smallButtonSelect, slotConnector, temperatureBar, temperaturePointer, progressBar, fireOn, fireOff, bellowsIcon, igniteIcon, extinguishIcon, pixel;
        private SpriteFont font;

        private Rectangle dragArea, clickCheck;

        public bool IsActive { get; set; }

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        private const int MaximumTemperature = 10000;
        private int currentTemperature = 0;

        private int currentMeltTime = 0, maxMeltTime = 10000;
        private int burnTime = 0;

        BaseItem ore, modifier, fuel;
        private Rectangle oreSlot, modifierSlot, fuelSlot;

        private Rectangle bellowsButton, igniteButton;
        private bool isIgnited = false;

        private Dictionary<int, SmelterFuel> validFuel = new Dictionary<int, SmelterFuel>();
        private List<SmelterOre> validMetals = new List<SmelterOre>();

        private SmelterOre currentRecipe;

        private float flameLerp = 0f;
        private bool isIncreaseLerp = false;
        private int flameHeight = 0;

        //Bellow variables
        private int bellowClicks = 0;
        private int temperatureIncrease = 0; //Increase in temperature rate. 0-5
        private float burnRate = 1f; //How fast the fuel burns. 0-1

        private ParticleEngine.BaseEmitter smokeEmitter, embersEmitter;

        private Controls controls = new Controls();
        private ScreenManager screens;
        private Camera camera;
        private TileEngine.TileMap tileMap;
        private BaseEntity controlledEntity;

        public OreSmelter() { IsActive = false; InitializeVariables(); }
        public void SetReferences(ScreenManager screens, Camera camera, TileEngine.TileMap tileMap)
        {
            this.screens = screens;
            this.camera = camera;
            this.tileMap = tileMap;
        }
        public void SetControlledEntity(BaseEntity controlledEntity)
        {
            this.controlledEntity = controlledEntity;
        }

        public void SetFuel(BaseItem fuel)
        {
            ReturnItem(ref this.fuel);

            this.fuel = fuel;
        }
        public void SetOre(BaseItem ore)
        {
            ReturnItem(ref this.ore);
            this.ore = ore; currentMeltTime = 0;

            RefreshRecipe();
        }
        public void SetModifier(BaseItem modifier)
        {
            ReturnItem(ref this.modifier);
            this.modifier = modifier; currentMeltTime = 0;

            RefreshRecipe();
        }
        private void RefreshRecipe()
        {
            int modifierID = (modifier != null) ? modifier.ID : -1;
            int oreID = (ore != null) ? ore.ID : -1;

            currentRecipe = GetByID(oreID, modifierID);
        }

        public bool IsOreSlot()
        {
            if (IsActive == true)
                return oreSlot.Contains(controls.MousePosition);
            else
                return false;
        }
        public bool IsModifierSlot()
        {
            if (IsActive == true)
                return modifierSlot.Contains(controls.MousePosition);
            else
                return false;
        }
        public bool IsFuelSlot()
        {
            if (IsActive == true)
                return fuelSlot.Contains(controls.MousePosition);
            else
                return false;
        }

        public bool IsValidOreCombination(int oreID, int modifierID) { return GetByID(oreID, modifierID) != null; }
        public bool IsValidFuel(int id) { return validFuel.ContainsKey(id); }

        public void SetItem(BaseItem item)
        {
            if (IsOreSlot())
            {
                SetOre(item);
                screens.PlaySound("SmelterAddOre");
            }

            if (IsModifierSlot())
                SetModifier(item);

            if (IsFuelSlot())
            {
                if (IsValidFuel(item.ID))
                    SetFuel(item);
                else
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "This isn't fuel!");
            }
        }

        private void InitializeVariables()
        {
            //Test fuel (Onyx Dust)
            validFuel.Add(5050, new SmelterFuel(20000, 7, 3500, 1f, -1)); //Bituminous
            validFuel.Add(5051, new SmelterFuel(25000, 9, 4800, 1f, -1)); //Anthracite
            validFuel.Add(5052, new SmelterFuel(10000, 10, 6500, 1.2f, -1)); //Fauxlight
            validFuel.Add(5053, new SmelterFuel(30000, 12, 6500, 1f, -1)); //Daylight
            //validFuel.Add(5054, new SmelterFuel(2000, 100, 8300, 1.5f, -1)); //Scorchcoal

            validFuel.Add(4901, new SmelterFuel(10000, 2, 700, .75f, -1)); //Palewood Log
            validFuel.Add(4906, new SmelterFuel(15000, 3, 1000, .85f, -1)); //Stillwood Log
            validFuel.Add(4910, new SmelterFuel(15000, 4, 1100, .95f, -1)); //Storgewood Log

            validMetals.Add(new SmelterOre(5000, -1, new Point(5020, 1), new Point(-1, -1), 2800, 7000)); //Iron ore
            validMetals.Add(new SmelterOre(5001, -1, new Point(5022, 1), new Point(-1, -1), 1760, 12000)); //Silver ore
            validMetals.Add(new SmelterOre(5002, -1, new Point(5023, 1), new Point(-1, -1), 1950, 15000)); //Gold ore
        }

        public void Load(ContentManager cm)
        {
            bg = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/bg");
            iconBG = cm.Load<Texture2D>("Interface/Global/iconBG");
            smallButton = cm.Load<Texture2D>("Interface/Global/smallButton");
            smallButtonSelect = cm.Load<Texture2D>("Interface/Global/smallButtonHover");
            slotConnector = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/slotConnector");
            temperatureBar = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/temperatureBar");
            temperaturePointer = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/temperaturePointer");
            progressBar = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/progressBar");

            fireOff = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/fireOff");
            fireOn = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/fireOn");

            bellowsIcon = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/bellows");
            igniteIcon = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/ignite");
            extinguishIcon = cm.Load<Texture2D>("Interface/Soulgate/Crafting/Smelter/extinguish");

            pixel = cm.Load<Texture2D>("rect");

            font = cm.Load<SpriteFont>("Fonts/boldOutlined");
            //largeFont = cm.Load<SpriteFont>("");

            position = new Vector2(GameSettings.VectorCenter.X - bg.Width / 2, GameSettings.VectorCenter.Y - bg.Height / 2);
            mouseDragOffset = new Vector2(bg.Center().X, 12);

            smokeEmitter = new ParticleEngine.EmitterTypes.Screen_based.SmelterSmoke();
            smokeEmitter.Load(cm);
            smokeEmitter.IsManualDepth = true;
            smokeEmitter.IsActivated = false;

            embersEmitter = new ParticleEngine.EmitterTypes.Screen_based.SmelterEmbers();
            embersEmitter.Load(cm);
            embersEmitter.IsManualDepth = true;
            embersEmitter.IsActivated = false;

            LoadWindowButtons(cm);
        }
        protected virtual void LoadWindowButtons(ContentManager cm)
        {
            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");
        }

        public void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                controls.UpdateCurrent();

                clickCheck = new Rectangle((int)position.X, (int)position.Y + 20, bg.Width, bg.Height - 20);
                CheckDragScreen();

                smokeEmitter.Offset = new Vector2(position.X + bg.Width / 2, position.Y + 65);
                embersEmitter.Offset = new Vector2(position.X + bg.Width / 2, position.Y + 110);

                smokeEmitter.Update(gt);
                embersEmitter.Update(gt);

                oreSlot = new Rectangle((int)position.X + 22, (int)position.Y + 59, 64, 64);
                modifierSlot = new Rectangle((int)position.X + 150, (int)position.Y + 59, 64, 64);
                fuelSlot = new Rectangle((int)position.X + 86, (int)position.Y + 123, 64, 64);

                bellowsButton = new Rectangle((int)position.X + 50, (int)position.Y + 151, 32, 32);
                igniteButton = new Rectangle((int)position.X + 154, (int)position.Y + 151, 32, 32);

                CheckButtons(gt);
                UpdateWindowButtons(gt, hints);

                currentTemperature = (int)MathHelper.Clamp(currentTemperature, 0, MaximumTemperature);

                FadeFlame(gt);

                controls.UpdateLast();
            }

            if (fuel == null)
                isIgnited = false;

            BurnFuel(gt);
            MeltOre(gt);
            CoolFurnace(gt);

            UpdateBellow(gt);

            if (IsActive == false)
            {
                if (ore == null && fuel != null)
                    isIgnited = false; //"Shut off" smelter to prevent fuel from burning completely.

                smokeEmitter.IsActivated = false;
                embersEmitter.IsActivated = false;
            }
        }

        private bool isSetSound = false;
        private CallLimiter temperatureRateLimit = new CallLimiter(100);
        private void BurnFuel(GameTime gt)
        {
            if (isIgnited == true)
            {
                if (fuel != null)
                {
                    if (isSetSound == false)
                    {
                        screens.PlaySound("SmelterLoop");
                        isSetSound = true;
                    }

                    smokeEmitter.IsActivated = true;
                    embersEmitter.IsActivated = true;

                    //x1 = Burn speed, y1 = temperature rate, x2 = smelt speed, y2 = burned item ID
                    burnTime += gt.ElapsedGameTime.Milliseconds;

                    flameHeight = (int)(50 * ((float)burnTime / (validFuel[fuel.ID].BurnTime * burnRate)));

                    if (burnTime >= (validFuel[fuel.ID].BurnTime * burnRate))
                    {
                        fuel.CurrentAmount -= 1; //Take away one fuel from the stack

                        if (validFuel[fuel.ID].BurnedItemID != -1)
                            controlledEntity.STORAGE_AddItem(validFuel[fuel.ID].BurnedItemID, 1, false, controlledEntity.IsPlayerControlled);

                        burnTime = 0;
                    }

                    if (temperatureRateLimit.IsCalling(gt) && currentTemperature <= validFuel[fuel.ID].MaxTemperature)
                        currentTemperature += (validFuel[fuel.ID].TemperatureRate + temperatureIncrease);
                }
            }
            else
            {
                smokeEmitter.IsActivated = false;
                embersEmitter.IsActivated = false;
            }

            if (isIgnited == false || IsActive == false)
            {
                if (isSetSound == true)
                {
                    screens.StopSound("SmelterLoop");
                    isSetSound = false;
                }
            }
        }
        private void MeltOre(GameTime gt)
        {
            if (currentRecipe != null)
            {
                maxMeltTime = currentRecipe.MeltingTime;

                if (currentTemperature >= currentRecipe.MeltingPoint)
                {
                    currentMeltTime += gt.ElapsedGameTime.Milliseconds;

                    if (currentMeltTime >= maxMeltTime)
                    {
                        int iterations = 0;

                        if (currentRecipe.OutputOne.X != -1)
                            AddItem(currentRecipe.OutputOne.X, currentRecipe.OutputOne.Y, ref iterations);
                        if (currentRecipe.OutputTwo.X != -1)
                            AddItem(currentRecipe.OutputTwo.X, currentRecipe.OutputTwo.Y, ref iterations);

                        //Only remove ore and modifier amounts if the melting process added anything to the stonehold/inventory. Fuel is only non-refundable item.
                        if (iterations > 0)
                        {
                            ore.CurrentAmount -= 1;

                            if (modifier != null)
                                modifier.CurrentAmount -= 1;

                            currentMeltTime = 0;
                        }
                        else
                            screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Item stack if full!");
                    }
                }
                else
                    currentMeltTime = 0;
            }
        }
        private void CoolFurnace(GameTime gt)
        {
            if (isIgnited == false)
            {
                if (temperatureRateLimit.IsCalling(gt))
                    currentTemperature -= 5;
            }
        }

        private CallLimiter bellowDegradeLimit = new CallLimiter(10000);
        private int bellowDelay = 0;
        private bool isDelayingBellows = false;
        private void UpdateBellow(GameTime gt)
        {
            if (bellowClicks > 0)
            {
                bellowClicks = MathHelper.Clamp(bellowClicks, 0, 5);

                if (bellowDegradeLimit.IsCalling(gt))
                    bellowClicks -= 1;

                temperatureIncrease = bellowClicks;
                burnRate = 1f - (float)(bellowClicks * .05);
            }

            if (isDelayingBellows == true)
            {
                bellowDelay += gt.ElapsedGameTime.Milliseconds;

                if (bellowDelay >= 2500)
                {
                    bellowDelay = 0;
                    isDelayingBellows = false;
                }
            }
        }

        private void CheckButtons(GameTime gt)
        {
            if (oreSlot.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign("Ore Slot");

                if (ore != null)
                {
                    ToolTip.RequestStringAssign("Ore Slot (" + ore.Name + ")");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        ReturnItem(ref ore);

                        if (ore == null)
                            currentMeltTime = 0;
                    }
                }
                else
                    ToolTip.RequestStringAssign("Ore Slot");
            }
            if (modifierSlot.Contains(controls.MousePosition))
            {
                if (modifier != null)
                {
                    ToolTip.RequestStringAssign("Modifier Slot (" + modifier.Name + ")");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        ReturnItem(ref modifier);

                        if (modifier == null)
                            currentMeltTime = 0;
                    }
                }
                else
                    ToolTip.RequestStringAssign("Modifier Slot");
            }
            if (fuelSlot.Contains(controls.MousePosition))
            {
                if (fuel != null)
                {
                    ToolTip.RequestStringAssign("Fuel Slot (" + fuel.Name + ")\nLeft-click: remove item");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        ReturnItem(ref fuel);

                        if (fuel == null)
                            burnTime = 0;
                    }
                }
                else
                    ToolTip.RequestStringAssign("Fuel Slot");
            }

            if (bellowsButton.Contains(controls.MousePosition))
            {
                if (bellowClicks > 0)
                    ToolTip.RequestStringAssign("Bellows\n\nBurn rate is +" + ((int)((1f - burnRate) * 100)).ToString() + "%\nTemperature increase is +" + temperatureIncrease);
                else
                    ToolTip.RequestStringAssign("Bellows");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    if (isDelayingBellows == false)
                    {
                        if (fuel != null)
                        {
                            bellowClicks++;
                            isDelayingBellows = true;
                            screens.PlaySound("Bellows");
                        }
                        else
                            screens.NOTIFICATION_Add(Pilgrimage_Of_Embers.ScreenEngine.Notification.NotificationManager.IconType.Inventory, "There is no fuel!");
                    }
                }
            }
            if (igniteButton.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    isIgnited = !isIgnited;

                    if (isIgnited == false)
                    {
                        bellowClicks = 0;
                        currentTemperature -= 100; //Due to splashing water. Get it?
                    }
                    else
                    {
                        if (fuel != null)
                            fuel.CurrentAmount--;
                    }
                }

                if (isIgnited == true)
                    ToolTip.RequestStringAssign("Extinguish Fire");
                else if (isIgnited == false)
                    ToolTip.RequestStringAssign("Ignite Fuel");
            }
        }

        private Color flameColor;
        private void FadeFlame(GameTime gt)
        {
            flameLerp = MathHelper.Clamp(flameLerp, .5f, 1f);

            if (flameLerp <= .5f)
                isIncreaseLerp = true;
            else if (flameLerp >= 1f)
                isIncreaseLerp = false;

            if (isIncreaseLerp == true)
                flameLerp += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (isIncreaseLerp == false)
                flameLerp -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;

            flameColor = Color.Lerp(Color.Transparent, Color.White, flameLerp);
        }

        private string hints = "Ore Smelter Tips:\n\n" +
            "In order, there are three slots:\n" +
            "- The ore slot (top-left)\n- The modifier slot (top-right)\n- The fuel slot (bottom)\n\n" +
            "Drag your items from the inventory to the respective slots.\n" +
            "When the slots are set, press the \"Ignite\" button on the right side.\n" +
            "Now your fuel is burning. You will initially see the temperature increasing,\n" +
            "and the fire (center) decreasing. When the fire is empty, a piece of fuel will be removed.\n" +
            "To speed up the temperature increase, press the bellows button on the left side.\n\n" +
            "Fuels have different properties, such as maximum temperature or burn rate.\n" +
            "Ores have different properties as well, such as melting point and melting speed.";

        protected void UpdateWindowButtons(GameTime gt, string hintHover)
        {
            hintRect = new Rectangle((int)position.X + 151, (int)position.Y, windowButton.Width - 20, windowButton.Height);
            hideRect = new Rectangle((int)position.X + 178, (int)position.Y, windowButton.Width - 20, windowButton.Height);

            if (hintRect.Contains(controls.MousePosition))
            {
                isHintHover = true;

                ToolTip.RequestStringAssign(hintHover);
            }
            else
                isHintHover = false;

            if (hideRect.Contains(controls.MousePosition))
            {
                isHideHover = true;
                ToolTip.RequestStringAssign("Hide Ore Smelter");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 1");
                    IsActive = false;
                }
            }
            else
                isHideHover = false;
        }

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private void CheckDragScreen()
        {
            dragArea = new Rectangle((int)position.X + 43, (int)position.Y, 100, 20);

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

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(bg, position, Color.White);

                DrawSlots(sb);

                sb.Draw(temperatureBar, position + new Vector2(18, 192), Color.White);
                sb.Draw(temperaturePointer, position + new Vector2(18 + (196 * ((float)currentTemperature / MaximumTemperature)), 189), Color.White, new Vector2(temperaturePointer.Center().X, 0), 0f, 1f);

                sb.Draw(smallButton, position + new Vector2(50, 151), Color.White); //Bellows
                sb.Draw(smallButton, position + new Vector2(154, 151), Color.White);

                if (isDelayingBellows == false)
                    sb.Draw(bellowsIcon, bellowsButton, Color.White);
                else
                    sb.Draw(bellowsIcon, bellowsButton, Color.Lerp(Color.White, Color.Transparent, .5f));

                if (isIgnited == false)
                    sb.Draw(igniteIcon, igniteButton, Color.White);
                else
                    sb.Draw(extinguishIcon, igniteButton, Color.White);

                smokeEmitter.Draw(sb);
                embersEmitter.Draw(sb);

                sb.Draw(progressBar, position + new Vector2(89, 59), Color.White);
                sb.Draw(pixel, new Rectangle((int)position.X + 91, (int)position.Y + 61, (int)(54 * ((float)currentMeltTime / maxMeltTime)), 2), Color.Orange);

                if (isIgnited == true)
                    sb.Draw(fireOn, new Vector2(position.X + 93, position.Y + 69 + flameHeight), new Rectangle(0, flameHeight, 50, 50), flameColor);

                sb.Draw(fireOff, new Vector2(position.X + 93, position.Y + 69), Color.White);

                sb.DrawString(font, "Ore Smelter", position + new Vector2((bg.Width / 2) - 25, 12), "Ore Smelter".LineCenter(font), ColorHelper.UI_Gold, 1f);

                DrawWindowButtons(sb);

                sb.End();
            }
        }
        public void DrawSlots(SpriteBatch sb)
        {
            sb.Draw(iconBG, oreSlot, Color.White); //Ore slot
            sb.Draw(iconBG, modifierSlot, Color.White); //Modifier slot
            sb.Draw(iconBG, fuelSlot, Color.White); //Fuel slot

            sb.Draw(slotConnector, position + new Vector2(59, 123), Color.White);
            sb.Draw(slotConnector, position + new Vector2(150, 123), Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.FlipHorizontally, 1f);

            if (ore != null)
            {
                sb.Draw(ore.Icon, oreSlot, Color.White);
                sb.DrawString(font, ore.CurrentAmount.ToString(), new Vector2(oreSlot.X + 3, oreSlot.Y + 3), ColorHelper.UI_Gold);
            }
            if (modifier != null)
            {
                sb.Draw(modifier.Icon, modifierSlot, Color.White);
                sb.DrawString(font, modifier.CurrentAmount.ToString(), new Vector2(modifierSlot.X + 3, modifierSlot.Y + 3), ColorHelper.UI_Gold);
            }
            if (fuel != null)
            {
                sb.Draw(fuel.Icon, fuelSlot, Color.White);
                sb.DrawString(font, fuel.CurrentAmount.ToString(), new Vector2(fuelSlot.X + 3, fuelSlot.Y + 3), ColorHelper.UI_Gold);
            }
        }

        protected virtual void DrawWindowButtons(SpriteBatch sb)
        {
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
                        isIgnited = false;

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

        public SmelterOre GetByID(int oreID, int modifierID)
        {
            for (int i = 0; i < validMetals.Count; i++)
            {
                if (validMetals[i].OreID == oreID &&
                    validMetals[i].ModifierID == modifierID)
                {
                    return validMetals[i];
                }
            }

            return null;
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            builder.AppendLine("IsBurning " + isIgnited);
            builder.AppendLine("Temperature " + currentTemperature);
            builder.AppendLine("BurnTime " + burnTime);
            builder.AppendLine("MeltTime " + currentMeltTime);
            builder.AppendLine();

            if (ore != null)
                builder.AppendLine("Ore " + ore.SaveData());
            if (modifier != null)
                builder.AppendLine("Modifier " + modifier.SaveData());
            if (fuel != null)
                builder.AppendLine("Fuel " + fuel.SaveData());

            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string[] words = data[i].Split(' ');

                if (words[0].ToUpper().StartsWith("ORE"))
                {
                    BaseItem item = ItemDatabase.Item(int.Parse(words[2])).Copy(screens, tileMap, controlledEntity, camera);
                    item.LoadData(data[i].Replace("Ore ", ""));

                    SetOre(item);
                }
                else if (words[0].ToUpper().StartsWith("MODIFIER"))
                {
                    BaseItem item = ItemDatabase.Item(int.Parse(words[2])).Copy(screens, tileMap, controlledEntity, camera);
                    item.LoadData(data[i].Replace("Modifier ", ""));

                    SetModifier(item);
                }
                else if (words[0].ToUpper().StartsWith("FUEL"))
                {
                    BaseItem item = ItemDatabase.Item(int.Parse(words[2])).Copy(screens, tileMap, controlledEntity, camera);
                    item.LoadData(data[i].Replace("Fuel ", ""));

                    SetFuel(item);
                }
                else if (words[0].ToUpper().StartsWith("ISBURNING"))
                {
                    isIgnited = bool.Parse(words[1]);
                }
                else if (words[0].ToUpper().StartsWith("TEMPERATURE"))
                {
                    currentTemperature = int.Parse(words[1]);
                }
                else if (words[0].ToUpper().StartsWith("BURNTIME"))
                {
                    burnTime = int.Parse(words[1]);
                }
                else if (words[0].ToUpper().StartsWith("MELTTIME"))
                {
                    currentMeltTime = int.Parse(words[1]);
                }
            }
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
            Position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);
        }

        public StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Smelter Fuels (Total: " + validFuel.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            foreach (KeyValuePair<int, SmelterFuel> fuel in validFuel)
            {
                builder.AppendLine(fuel.Key + " - " + ItemDatabase.Item(fuel.Key).Name + " [Burn Time: " + fuel.Value.BurnTime +
                                                                                         ", Temperature Rate: " + fuel.Value.TemperatureRate +
                                                                                         ", Smelt Speed: " + fuel.Value.SmeltSpeed +
                                                                                         ", Maximum Temperature: " + fuel.Value.MaxTemperature +
                                                                                         ", Burned Item: " + fuel.Value.BurnedItemID + "]");
            }

            builder.AppendLine();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Smelter Metals (Total: " + validMetals.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            for (int i = 0; i < validMetals.Count; i++)
            {
                builder.AppendLine(validMetals[i].OreID + " - " + ItemDatabase.Item(validMetals[i].OreID).Name + " [Melting Point: " + validMetals[i].MeltingPoint +
                                                                                                                 ", Melting Time: " + validMetals[i].MeltingTime +
                                                                                                                 "ms, Modifier: " + validMetals[i].ModifierID +
                                                                                                                 ", Output One: " + validMetals[i].OutputOne.X + "; " + validMetals[i].OutputOne.Y +
                                                                                                                 ", Output Two: " + validMetals[i].OutputTwo.X + "; " + validMetals[i].OutputTwo.Y + "]");
            }

            return builder;
        }
    }
}
