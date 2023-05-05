using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.Notification;
using Pilgrimage_Of_Embers.ScreenEngine.RumorsNotes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Souls;
using Pilgrimage_Of_Embers.Debugging;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook;
using Pilgrimage_Of_Embers.ScreenEngine.Souls.Types;
using Pilgrimage_Of_Embers.ScreenEngine.Soulgate;
using Pilgrimage_Of_Embers.ScreenEngine.Various;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.Container;
using Pilgrimage_Of_Embers.TileEngine.Objects.ContainerTypes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.ScreenEngine.EquipmentScreens;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Merchanting;
using Pilgrimage_Of_Embers.ScreenEngine.Soulgate.Crafting;
using Pilgrimage_Of_Embers.TileEngine.Objects.Soulgates;
using Pilgrimage_Of_Embers.Skills;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public class ScreenManager
    {
        InventoryUI inventoryUI = new InventoryUI();
        SpellbookInterface spellbook = new SpellbookInterface();
        SoulsInterface si;
        RumorsInterface ri = new RumorsInterface();

        BarterUI barterUI = new BarterUI();
        PurchaseUI purchaseUI = new PurchaseUI();

        EntityHUD HUD;
        NotificationManager notif = new NotificationManager();
        OffscreenHUD offscreenHUD = new OffscreenHUD();
        EquipmentUI equipmentUI;
        InteractBox activateBox = new InteractBox();

        PauseScreen ps = new PauseScreen();
        OptionsInterface optionsUI;
        DeathScreen deathScreen = new DeathScreen();

        StatsScreen skills = new StatsScreen();
        OfferingsUI offerings = new OfferingsUI();
        MendersTools mender = new MendersTools();
        Stonehold stonehold = new Stonehold();
        ReinforceWorkbench anvil = new ReinforceWorkbench();
        OreSmelter smelter = new OreSmelter();
        FlameWarpUI gateTravel = new FlameWarpUI();
        BrewmastersContrivances brewer = new BrewmastersContrivances();
        ImbueEssenceUI imbueEssence = new ImbueEssenceUI();

        ContainerUI containerUI = new Container.ContainerUI();
        private SoulgateInterface soulgateUI = new SoulgateInterface();

        InterfaceAudio audio = new InterfaceAudio();
        InterfaceEmitters emitters = new InterfaceEmitters();

        ScreenEffects screenEffects;
        Cursor cursor = new Cursor();

        GameExit exitGame;

        private Camera camera;
        private BaseEntity controlledEntity;
        private TileMap tileMap;
        private DebugManager debug;
        private WorldLight ambient;
        private CultureManager culture;
        private WorldManager world;

        SplashScreen splash;

        public enum ScreenState
        {
            None,
            Inventory, //The inventory
            Spellbook, //The spellbook
            Souls, //Simplistic viewing of souls
            Rumors, //Rumors given by characters, etc.
            Stats, //Player skills & info
            Settings, //For stuff such as exiting game, options screen, etc.
            Pause,
        }

        Controls controls = new Controls();

        public ScreenManager(Game game, GraphicsDeviceManager graphics, WorldManager wm)
        {
            HUD = new EntityHUD(game);
            si = new SoulsInterface();
            equipmentUI = new EquipmentUI(game);
            optionsUI = new OptionsInterface(game, graphics, wm);
            screenEffects = new ScreenEffects(wm);
            world = wm;

            exitGame = new GameExit();
        }

        public void SetReferences(Camera camera, TileMap tileMap, DebugManager debug, BaseEntity controlledEntity, WorldLight ambient, CultureManager culture, WorldManager world, Main.MainScreen main)
        {
            this.camera = camera;
            this.tileMap = tileMap;
            this.controlledEntity = controlledEntity;
            this.debug = debug;
            this.ambient = ambient;
            this.culture = culture;

            inventoryUI.SetReferences(this, camera);
            equipmentUI.SetReferences(this);
            spellbook.SetReferences(this, equipmentUI);
            si.SetReferences(this);
            ri.SetReferences(notif, debug, this);
            HUD.SetReferences(this, controlledEntity, ambient, culture);
            offscreenHUD.SetReferences(camera);
            notif.SetReferences(this);
            skills.SetReferences(controlledEntity.Skills, this);
            offerings.SetReferences(this);
            stonehold.SetReferences(this, tileMap, camera);
            mender.SetReferences(this);
            anvil.SetReferences(this);
            smelter.SetReferences(this, camera, tileMap);
            gateTravel.SetReferences(this, tileMap, world, camera);
            brewer.SetReferences(this, tileMap, controlledEntity, camera);
            imbueEssence.SetReferences(this, tileMap, controlledEntity, camera);

            containerUI.SetReferences(this, tileMap);
            soulgateUI.SetReferences(camera, tileMap, this);
            barterUI.SetReferences(this, tileMap);
            purchaseUI.SetReferences(this);
            optionsUI.SetReferences(this, main, tileMap);
            exitGame.SetReferences(world, this);

            splash = new SplashScreen(this, world);
            deathScreen.SetReferences(world, tileMap, controlledEntity, this, camera);

            audio.SetReferences(debug);
        }

        public void Load(ContentManager cm)
        {
            audio.LoadAudio(cm);
            emitters.Load(cm);
            cursor.Load(cm);

            spellbook.Load(cm);
            si.Load(cm);
            ri.Load(cm);
            notif.Load(cm);
            equipmentUI.Load(cm);
            HUD.LoadContent(cm);
            offscreenHUD.Load(cm);
            activateBox.Load(cm);
            ps.Load(cm);
            skills.Load(cm);
            offerings.Load(cm);
            mender.Load(cm);
            stonehold.Load(cm);
            anvil.Load(cm);
            smelter.Load(cm);
            gateTravel.Load(cm);
            brewer.Load(cm);
            imbueEssence.Load(cm);
            containerUI.Load(cm);
            soulgateUI.Load(cm);
            barterUI.Load(cm);
            purchaseUI.Load(cm);

            optionsUI.Load(cm);
            screenEffects.Load(cm);

            splash.Load(cm);
            deathScreen.Load(cm);

            exitGame.Load(cm);

            inventoryUI.Load(cm);
        }

        public void UpdatePause(GameTime gt)
        {
            controls.UpdateLast();
            controls.UpdateCurrent();

            if (world.MAIN_IsActive == false)
            {
                if (exitGame.IsActive == false)
                    if (controls.IsKeyPressedOnce(controls.CurrentControls.Pause))
                        InvertPause();

                ps.Update(gt);
            }

            emitters.Update(gt);
            optionsUI.Update(gt);
            screenEffects.Update(gt);
            cursor.Update(gt);
        }
        public void Update(GameTime gt, GraphicsDevice g)
        {
            activateBox.IsActive = false;

            controls.UpdateCurrent(); // --- Remove control updating here soon. ---

            if (world.MAIN_IsActive == false)
            {
                HUD.Update(gt);
                offscreenHUD.Update(gt);

                ri.Update(gt);
                spellbook.Update(gt);
                si.Update(gt);

                notif.Update(gt);

                skills.Update(gt);
                offerings.Update(gt);
                mender.Update(gt);
                anvil.Update(gt);
                smelter.Update(gt);
                stonehold.Update(gt);
                gateTravel.Update(gt);
                brewer.Update(gt);
                imbueEssence.Update(gt);
                containerUI.Update(gt);
                soulgateUI.Update(gt);
                inventoryUI.Update(gt);

                equipmentUI.IsInventoryOpen = inventoryUI.IsActive;
                equipmentUI.Update(gt);

                barterUI.Update(gt);
                purchaseUI.Update(gt);

                splash.Update(gt);
                deathScreen.Update(gt);

                exitGame.Update(gt);

                if ((inventoryUI.IsClickingUI() || spellbook.IsClickingUI() || HUD.IsClickingUI() || containerUI.IsClickingUI() || skills.IsClickingUI() || ri.IsClickingUI() ||
                   equipmentUI.IsClickingUI() || PURCHASE_IsClickingUI() || BARTERING_IsClickingUI() || optionsUI.IsClickingUI() || offerings.IsClickingUI() ||
                   smelter.IsClickingUI() || mender.IsClickingUI() || anvil.IsClickingUI() || imbueEssence.IsClickingUI() || brewer.IsClickingUI() || stonehold.IsClickingUI() ||
                   gateTravel.IsClickingUI() || tileMap.IsClickingUI() || soulgateUI.IsClickingUI()))
                {
                    IsClickingUI = true;
                    clickTick = 0;
                }
                else
                {
                    if (clickTick > 5)
                        IsClickingUI = false;
                    else
                        clickTick++;
                }

                if (inventoryUI.IsActive || spellbook.IsActive || containerUI.IsActive || skills.IsActive || ri.IsActive ||
                   purchaseUI.IsActive || barterUI.IsBartering || optionsUI.IsActive || offerings.IsActive || smelter.IsActive ||
                   mender.IsActive || anvil.IsActive || imbueEssence.IsActive || brewer.IsActive || stonehold.IsActive ||
                   gateTravel.IsActive || tileMap.IsUIOpen() || soulgateUI.IsActive)
                {
                    IsUIOpen = true;
                    openTick = 0;
                }
                else
                {
                    if (openTick > 250)
                        IsUIOpen = false;
                    else
                        openTick += gt.ElapsedGameTime.Milliseconds;
                }
            }
            splash.Update(gt);

            controls.UpdateLast();
        }

        public void Draw(SpriteBatch sb)
        {
            if (world.MAIN_IsActive == false)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                if (GameSettings.IsDebugging == false)
                {
                    offscreenHUD.Draw(sb);
                    HUD.Draw(sb);
                    activateBox.Draw(sb);
                    equipmentUI.Draw(sb);
                }

                notif.Draw(sb);

                sb.End();

                soulgateUI.Draw(sb);

                smelter.Draw(sb);
                skills.Draw(sb);
                brewer.Draw(sb);
                imbueEssence.Draw(sb);
                stonehold.Draw(sb);
                offerings.Draw(sb);
                mender.Draw(sb);
                anvil.Draw(sb);
                gateTravel.Draw(sb);

                barterUI.Draw(sb);
                purchaseUI.Draw(sb);

                containerUI.Draw(sb);

                ri.Draw(sb);
                si.Draw(sb);
                spellbook.Draw(sb);
                inventoryUI.Draw(sb);
            }

            optionsUI.Draw(sb);

            if (world.MAIN_IsActive == false)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            
                if (SPELLBOOK_IsActive == true)
                {
                    spellbook.DrawDrag(sb);
                }

                emitters.Draw(sb);
                exitGame.Draw(sb);

                if (ps.IsPaused == true)
                    ps.Draw(sb);
         
                sb.End();
            }
        }
        public void DrawTransition(SpriteBatch sb)
        {
            screenEffects.Draw(sb);
            deathScreen.Draw(sb);
            splash.Draw(sb);
        }
        public void DrawCursor(SpriteBatch sb)
        {
            cursor.Draw(sb);
        }

        public void LoadData(List<string> linesOfData)
        {
        }

        public void SetOnMapLoad(List<BaseEntity> allEntities)
        {
            inventoryUI.SetMapEntities(allEntities);
        }

        #region [Encapsulation] Inventory

        public void INVENTORY_SetItemData(BaseEntity controlledEntity, EntityStorage storage, EntityEquipment equipment)
        {
            inventoryUI.SetItemData(controlledEntity, storage, equipment);
            spellbook.SetControlledEntity(controlledEntity, storage, equipment);
            equipmentUI.SetControlledEntity(equipment, controlledEntity);

            BARTERING_SetTrader(controlledEntity);
            PURCHASING_SetTrader(controlledEntity);

            offerings.SetControlledEntity(controlledEntity);
            mender.SetControlledEntity(controlledEntity, storage);
            stonehold.SetControlledEntity(controlledEntity);
            anvil.SetControlledEntity(controlledEntity, storage);
            smelter.SetControlledEntity(controlledEntity);
            gateTravel.SetControlledEntity(controlledEntity);
            soulgateUI.SetControlledEntity(controlledEntity);
            brewer.SetControlledEntity(controlledEntity);
            imbueEssence.SetControlledEntity(controlledEntity);
        }
        public Vector2 INVENTORY_Position { get { return inventoryUI.ScreenPosition; } }
        public InventoryUI.CurrentTab INVENTORY_Tab { get { return inventoryUI.Tab; } }
        public bool INVENTORY_IsActive { get { return inventoryUI.IsActive; } set { inventoryUI.IsActive = value; } }
        public bool INVENTORY_IsItemDragging() { return inventoryUI.IsDraggingItem(); }

        #endregion

        #region [Encapsulation] Spellbook

        public bool SPELLBOOK_IsActive { get { return spellbook.IsActive; } set { spellbook.IsActive = value; } }
        public Vector2 SPELLBOOK_Position { get { return spellbook.Position; } }

        #endregion

        #region [Encapsulation] Rumor

        public void RUMOR_Add(int id) { ri.AddRumor(id, false); }
        public void RUMOR_Resolve(int id) { ri.ResolveRumor(id); }
        public void RUMOR_Dismiss(int id) { ri.DismissRumor(id); }
        public Rumor.RumorState RUMOR_State(int id) { return ri.GetRumorState(id); }
        public RumorsInterface.RumorCompleteStatus RUMOR_IsCompleted(int id) { return ri.IsRumorCompleted(id); }
        public bool RUMOR_HasRumor(int id) { return ri.HasRumor(id); }

        public bool RUMOR_IsActive { get { return ri.IsActive; } set { ri.IsActive = value; } }

        public StringBuilder SaveRumorData(string tag) { return ri.SaveData(tag); }
        public void LoadRumorData(List<string> lines) { ri.LoadData(lines); }

        public void RUMORS_Reset() { ri.ResetRumors(); }

        #endregion

        #region [Encapsulation] Notification

        public void NOTIFICATION_Add(NotificationManager.IconType icon, string text) { notif.AddNotification(icon, text); }
        public void NOTIFICATION_Add(Texture2D icon, string text) { notif.AddNotification(icon, text); }

        #endregion

        #region [Encapsulation] Souls

        public void SOULS_SetData(EntityEquipment equipment, EntityStorage storage) { si.SetSoulData(equipment, storage); }
        public BaseSoul SOULS_Retrieve(int id) { return si.RetrieveSoul(id); }

        public bool SOULS_IsActive { get { return si.IsActive; } set { si.IsActive = value; } }
        public void SOULS_InvertState() { si.IsActive = !si.IsActive; }

        #endregion

        #region [Encapsulation] Soulgate

        public void SOULGATE_SetGate(BaseCheckpoint gate)
        {
            Logger.AppendLine("Set soulgate at soulgateUI (" + gate.Name + ")");
            soulgateUI.SetSoulgate(gate);

            Logger.AppendLine("Set soulgate at gateTravel (" + gate.Name + ")");
            gateTravel.SetGate(gate);
        }
        public bool SOULGATE_IsActive { get { return soulgateUI.IsActive; } set { soulgateUI.IsActive = value; } }
        public void SOULGATE_CloseAllUIs() { soulgateUI.CloseAll(); }
        public string SOULGATE_SaveData() { return soulgateUI.SaveData(); }
        public void SOULGATE_LoadData(string data) { soulgateUI.LoadData(data); }
        public bool SOULGATE_IsNear() { return soulgateUI.IsSoulgateNear(); }

        #endregion

        #region [Encapsulation] Skills

        public void SKILLS_InvertState() { skills.InvertState(); }
        public void SKILLS_SetData(Skillset skills, BaseEntity controlledEntity) { this.skills.SetSkillsData(skills, controlledEntity); }

        public bool SKILLS_IsActive { get { return skills.IsActive; } set { skills.IsActive = value; } }

        #endregion

        #region [Encapsulation] Artisan's Counter

        public void OFFERINGS_InvertState() { if (offerings.IsActive == false) { Logger.AppendLine("Opened artisan UI"); } offerings.IsActive = !offerings.IsActive; }
        public bool OFFERINGS_IsActive { get { return offerings.IsActive; } set { offerings.IsActive = value; } }

        public StringBuilder OFFERINGS_SaveData(string tag) { return offerings.SaveData(tag); }
        public void OFFERINGS_LoadData(List<string> data) { offerings.LoadData(data); }
        public void OFFERINGS_Reset() { offerings.ResetRecipes(); }

        #endregion

        #region [Encapsulation] Mender's Tools

        public void MENDER_InvertState() { if (mender.IsActive == false) { Logger.AppendLine("Opened mender UI"); mender.UpdateList(); }  mender.IsActive = !mender.IsActive; }
        public bool MENDER_IsActive { get { return mender.IsActive; } set { mender.IsActive = value; } }

        #endregion

        #region [Encapsulation] Stonehold

        public void STONEHOLD_InvertState() { if (stonehold.IsActive == false) { Logger.AppendLine("Opened stonehold UI"); } stonehold.IsActive = !stonehold.IsActive; }
        public bool STONEHOLD_IsActive { get { return stonehold.IsActive; } set { stonehold.IsActive = value; } }

        public void STONEHOLD_DepositItem(BaseItem item) { Logger.AppendLine("Deposited item to stonehold (" + item.Name + ")"); stonehold.DepositItem(item); }
        public void STONEHOLD_AddItem(int id, int quantity) { stonehold.AddItemReturnDifference(id, quantity); }
        public int STONEHOLD_AddItemReturnDifference(int id, int quantity) { return stonehold.AddItemReturnDifference(id, quantity); }
        public BaseItem STONEHOLD_GetItem(int id) { return stonehold.GetItem(id); }
        public bool STONEHOLD_IsTabHover() { return stonehold.IsTabHover(); }

        public StringBuilder STONEHOLD_SaveData(string tag) { return stonehold.SaveData(tag); }
        public void STONEHOLD_LoadData(List<string> data) { stonehold.LoadData(data); }
        public void STONEHOLD_Reset() { stonehold.ResetItems(); }

        #endregion

        #region [Encapsulation] Old Anvil

        public void ANVIL_InvertState() { if (anvil.IsActive == false) { Logger.AppendLine("Opened anvil UI"); } anvil.IsActive = !anvil.IsActive; }
        public bool ANVIL_IsActive { get { return anvil.IsActive; } set { anvil.IsActive = value; } }

        #endregion

        #region [Encapsulation] Ore Smelter

        public void SMELTER_SetItem(BaseItem item) { Logger.AppendLine("Sent item to smelter (" + item.Name + ")"); smelter.SetItem(item); }
        public bool SMELTER_IsActive { get { return smelter.IsActive; } set { smelter.IsActive = value; } }
        public void SMELTER_InvertState() { if (smelter.IsActive == false) { Logger.AppendLine("Opened smelter UI"); } smelter.IsActive = !smelter.IsActive; }

        public bool SMELTER_IsOreSlotHover() { return smelter.IsOreSlot(); }
        public bool SMELTER_IsModifierSlotHover() { return smelter.IsModifierSlot(); }
        public bool SMELTER_IsFuelSlotHover() { return smelter.IsFuelSlot(); }

        public bool SMELTER_IsValidFuel(int id) { return smelter.IsValidFuel(id); }

        public void SMELTER_SetOre(BaseItem item) { smelter.SetOre(item); }
        public void SMELTER_SetModifier(BaseItem item) { smelter.SetModifier(item); }
        public void SMELTER_SetFuel(BaseItem item) { smelter.SetFuel(item); }

        public string SMELTER_SaveData(string tag) { return smelter.SaveData(tag).ToString(); }
        public void SMELTER_LoadData(List<string> data) { smelter.LoadData(data); }

        #endregion

        #region [Encapsulation] Soul Warping

        public void SOULWARP_InvertState() { if (gateTravel.IsActive == false) { Logger.AppendLine("Opened soul warp UI"); } gateTravel.IsActive = !gateTravel.IsActive; }
        public bool SOULWARP_IsActive { get { return gateTravel.IsActive; } set { gateTravel.IsActive = value; } }

        #endregion

        #region [Encapsulation] Brewing

        public void BREWING_Invert() { if (brewer.IsActive == false) { Logger.AppendLine("Opened brewing UI"); } brewer.IsActive = !brewer.IsActive; }
        public bool BREWING_IsActive { get { return brewer.IsActive; } set { brewer.IsActive = value; } }

        public void BREWING_SetPrimary(BaseItem item) { brewer.AddPrimaryIngredient(item); }
        public void BREWING_SetSecondary(BaseItem item) { brewer.AddSecondaryIngredient(item); }
        public void BREWING_SetCombiner(BaseItem item) { brewer.AddCombiner(item); }

        public bool BREWING_IsPrimaryHover() { return brewer.IsPrimaryHover; }
        public bool BREWING_IsSecondaryHover() { return brewer.IsSecondaryHover; }
        public bool BREWING_IsCombinerHover() { return brewer.IsCombinerHover; }

        public StringBuilder BREWING_SaveData(string tag) { return brewer.SaveData(tag); }
        public void BREWING_LoadData(List<string> data) { brewer.LoadData(data); }

        #endregion

        #region [Encapsulation] Imbue Essence

        public void IMBUE_Invert()
        {
            if (imbueEssence.IsActive == false)
            {
                Logger.AppendLine("Opened imbue essence UI");
            }

            imbueEssence.IsActive = !imbueEssence.IsActive;

            if (imbueEssence.IsActive == true)
                imbueEssence.CheckEssence(false);
            else
                imbueEssence.CheckEssence(true);
        }
        public bool IMBUE_IsActive { get { return imbueEssence.IsActive; } set { imbueEssence.IsActive = value; } }
        public void IMBUE_Essence()
        {
            imbueEssence.CheckEssence(false);
            imbueEssence.ImbueEssences();
        }

        public StringBuilder IMBUE_SaveData(string tag) { return imbueEssence.SaveData(tag); }
        public void IMBUE_LoadData(List<string> data) { imbueEssence.LoadData(data); }

        #endregion

        #region [Encapsulation] Container managing

        public void CONTAINER_SetData(MultiItem container) { containerUI.SetContainerData(container); }
        public void Container_SetEntityData(BaseEntity entity, EntityStorage storage) { containerUI.SetEntityData(entity, storage); }

        #endregion

        #region [Encapsulation] HUD managing

        public void HUD_Set(BaseEntity controlledEntity)
        {
            HUD.SetEntity(controlledEntity);
        }
        public Vector2 HUD_Offset { get { return HUD.Offset; } }
        public float HUD_MainOpacity { get { return HUD.MainOpacity; } }

        public void HUD_ResetEquipLerp() { equipmentUI.ResetLerp(); }
        public float HUD_EquipTargetLerp { get { return equipmentUI.EquipTargetLerp; } set { equipmentUI.EquipTargetLerp = value; } }
        public float HUD_QuickslotTargetLerp { get { return equipmentUI.QuickslotTargetLerp; } set { equipmentUI.QuickslotTargetLerp = value; } }

        public void OFFSCREEN_AddPoint(OffscreenPoint point) { offscreenHUD.AddPoint(point); }

        #endregion

        #region [Encapsulation] Bartering

        public void BARTERING_Begin()
        {
            Logger.AppendLine("Opened bartering UI"); barterUI.BeginBartering();
        }
        public void BARTERING_End()
        {
            barterUI.ForceEndBarter();
        }
        public bool BARTERING_IsActive()
        {
            return barterUI.IsBartering;
        }

        public void BARTERING_SetMerchant(BaseEntity merchant)
        {
            Logger.AppendLine("Barter merchant has been set (" + merchant.Name + ")"); barterUI.SetMerchant(merchant);
        }
        public void BARTERING_SetTrader(BaseEntity trader)
        {
            Logger.AppendLine("Barter trader has been set (" + trader.Name + ")"); barterUI.SetTrader(trader);
        }
        public bool BARTERING_IsClickingUI()
        {
            return barterUI.IsClickingUI();
        }
        public void BARTERING_AddTraderItem(int id, int quantity, out int subtractQuantity)
        {
            Logger.AppendLine("Sent trade item (" + id + ", " + quantity + ")");
            barterUI.TRADER_AddItem(id, quantity, out subtractQuantity);
        }
        public bool BARTERING_IsTraderWindowFull()
        {
            return barterUI.IsTraderWindowFull();
        }

        #endregion

        #region [Encapsulation] Purchasing

        public void PURCHASING_SetMerchant(BaseEntity merchant)
        {
            Logger.AppendLine("Purchase merchant has been set (" + merchant.Name + ")");
            purchaseUI.SetMerchant(merchant);
        }
        public void PURCHASING_SetTrader(BaseEntity controlledEntity)
        {
            Logger.AppendLine("Purchase trader has been set (" + controlledEntity.Name + ")");
            purchaseUI.SetControlledEntity(controlledEntity);
        }

        public void PURCHASING_Begin()
        {
            Logger.AppendLine("Opened purchasing UI"); purchaseUI.IsActive = true;
        }
        public void PURCHASING_End()
        {
            purchaseUI.IsActive = false;
        }
        public bool PURCHASE_IsClickingUI()
        {
            return purchaseUI.IsUIClicking();
        }
        public bool PURCHASING_IsPurchasing()
        {
            return purchaseUI.IsActive;
        }

        public void PURCHASING_SellItem(BaseItem item, int quantity) { Logger.AppendLine("Selling item (" + item.Name + ", " + quantity + ")"); purchaseUI.SellItem(item, quantity); }

        #endregion

        #region [Encapsulation] Pause screen

        public void Pause() { ps.IsPaused = true; }
        public void Resume() { ps.IsPaused = false; }
        public void InvertPause() { ps.IsPaused = !ps.IsPaused; }
        public bool IsPaused() { return ps.IsPaused; }

        #endregion

        #region [Encapsulation] Settings

        public bool OPTIONS_IsActive { get { return optionsUI.IsActive; } set { optionsUI.IsActive = value; } }
        public void OPTIONS_ResetPosition() { optionsUI.ResetPosition(); }
        public void OPTIONS_SetPosition(Vector2 position) { optionsUI.ExternalPosition = position; }

        #endregion

        #region [Encapsulation] Activate Box
        public void ACTIVATEBOX_SetLines(string lineOne, string lineTwo)
        {
            activateBox.SetValues(lineOne, lineTwo);
            activateBox.IsActive = true;
        }
        #endregion

        #region [Encapsulation] Screen Effects

        public void EFFECTS_BeginTransition(ScreenEffects.TransitionType type, Color color, int pauseTime, float incrementSpeed, float decrementSpeed)
        {
            screenEffects.BeginTransition(type, color, pauseTime, incrementSpeed, decrementSpeed);
        }
        public bool EFFECTS_IsTransitionFaded { get { return screenEffects.IsFaded; } }
        public void EFFECTS_ForceEndTransition() { screenEffects.ForceEnd(); }
        public float EFFECTS_TransitionLerp { get { return screenEffects.FadeLerp; } set { screenEffects.FadeLerp = value; } }

        public bool EFFECTS_IsWidescreen() { return screenEffects.IsWidescreen; }

        #endregion

        #region [Encapsulation] Audio methods
        public void PlaySound(string soundName) { audio.PlaySound(soundName); }
        public void PlayRandom(params string[] soundNames) { audio.PlayRandom(soundNames); }
        public void StopSound(string soundName) { audio.StopSound(soundName); }
        public bool IsSoundPlaying(string soundName) { return audio.IsSoundPlaying(soundName); }
        #endregion

        #region [Encapsulation] Emitter Methods
        public void EMITTER_AddReference(ParticleEngine.BaseEmitter emitter)
        {
            emitters.AddEmitterReference(emitter);
        }
        #endregion

        #region [Encapsulation] Cursor

        public void SetCursorState(Cursor.CursorState state)
        {
            cursor.State = state;
        }

        #endregion

        #region [Methods] Miscellaneous

        public void INTEGERDISPLAY_AddInteger(string value, Vector2 position, Color color, int depthFloor, bool isDownDirection)
        {
            tileMap.DISPLAY_AddInteger(value, position, color, depthFloor, isDownDirection);
        }
        public bool SPLASH_IsComplete() { return splash.IsSplashComplete; }

        private int clickTick = 0, openTick = 0;
        public bool IsClickingUI { get; private set; }
        public bool IsUIOpen { get; private set; }

        public void CloseAllUIs()
        {
            inventoryUI.IsActive = false;
            spellbook.IsActive = false;
            containerUI.ForceClose();
            skills.IsActive = false;
            ri.IsActive = false;
            PURCHASING_End();
            BARTERING_End();
            optionsUI.IsActive = false;
            offerings.IsActive = false;
            smelter.IsActive = false;
            mender.IsActive = false;
            anvil.IsActive = false;
            imbueEssence.IsActive = false;
            brewer.IsActive = false;
            stonehold.IsActive = false;
            gateTravel.IsActive = false;
            tileMap.ForceCloseEntityUIs();
            soulgateUI.IsActive = false;
        }
        public void ResetUIPositions()
        {
            //Player-related
            optionsUI.ResetPosition();
            equipmentUI.ResetPosition();
            HUD.ResetPositions();
            inventoryUI.ResetPosition();
            ri.ResetPosition();

            //Entity-related
            barterUI.ResetPosition();
            purchaseUI.ResetPosition();
            containerUI.ResetPosition();

            //Soulgate UIs
            offerings.ResetPosition();
            mender.ResetPosition();
            smelter.ResetPosition();
            anvil.ResetPosition();
            stonehold.ResetPosition();
            skills.ResetPosition();
            gateTravel.ResetPosition();
            imbueEssence.ResetPosition();
            brewer.ResetPosition();

            //Miscellaneous
            activateBox.ResetPosition();
            exitGame.ResetPosition();
        }

        public StringBuilder OutputExtraCreationGuide()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(offerings.Output().ToString());
            builder.AppendLine(smelter.Output().ToString());
            builder.AppendLine(mender.Output().ToString());
            builder.AppendLine(anvil.Output().ToString());
            builder.AppendLine(brewer.Output().ToString());
            builder.AppendLine(imbueEssence.Output().ToString());

            return builder;
        }

        #endregion
    }
}
