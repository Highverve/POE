using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Performance;

namespace Pilgrimage_Of_Embers.ScreenEngine.EquipmentScreens
{
    public class EquipmentUI
    {
        private Vector2 quickSlotOffset;
        private Texture2D icon, iconSelected, quickSlotBG, equipBG, deleteSlot, pixel, emberIcon, renownIcon;
        private SpriteFont font, counterFont, largeFont;
        private string directory = "Interface/Inventory/";

        private float equipLerp = 1f, equipTargetLerp = 1f;
        private float quickslotLerp = 1f, quickslotTargetLerp = 1, nameQuickslotLerp = 0f, hideLerp = 1f;
        private bool isHUDHidden;

        private Color highlight = new Color(165, 174, 124, 255);
        private Color noRequirements = Color.Lerp(Color.Lerp(Color.White, Color.Red, .5f), Color.Transparent, .5f);
        private Color fadeWhite, counterText, emberText, unavailableQS, nameQS;

        private EntityEquipment equipment;
        private ScreenManager screens;
        private BaseEntity currentEntity;
        private Controls controls = new Controls();

        private int lastQuickslot = -1, nameFadeTime = 0;

        EquippedSlots equippedSlots;

        public float EquipTargetLerp { get { return equipTargetLerp; } set { equipTargetLerp = MathHelper.Clamp(value, 0f, 1f); } }
        public float QuickslotTargetLerp { get { return quickslotTargetLerp; } set { quickslotTargetLerp = MathHelper.Clamp(value, 0f, 1f); } }

        public EquipmentUI(Game game)
        {
            equippedSlots = new EquippedSlots();
        }

        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;

            equippedSlots.SetReferences(screens);
        }
        public void SetControlledEntity(EntityEquipment equipment, BaseEntity entity)
        {
            this.equipment = equipment;
            this.currentEntity = entity;

            equippedSlots.SetControlledEntity(entity, equipment);
        }

        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/boldOutlined");
            counterFont = cm.Load<SpriteFont>("Fonts/emberCounterFont");
            largeFont = cm.Load<SpriteFont>("Fonts/titleFont");

            icon = cm.Load<Texture2D>(directory + "Icons/iconBG");
            iconSelected = cm.Load<Texture2D>(directory + "Icons/iconBGSelect");
            quickSlotBG = cm.Load<Texture2D>(directory + "Quickslot/quickslotBGNew");
            deleteSlot = cm.Load<Texture2D>(directory + "Quickslot/deleteQuickslot");

            equipBG = cm.Load<Texture2D>(directory + "Backgrounds/equipBG");

            renownIcon = cm.Load<Texture2D>("Interface/HUD/Icons/arisonteTokenIcon");
            emberIcon = cm.Load<Texture2D>("Interface/HUD/Icons/emberXP");

            pixel = cm.Load<Texture2D>("rect");

            quickSlotOffset = new Vector2(GameSettings.WindowCenter.X - (quickSlotBG.Width / 2), GameSettings.WindowResolution.Y - quickSlotBG.Height);

            equippedSlots.Load(cm);
        }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            UpdateControlShortcuts(gt);

            UpdateQuickSlots(gt);
            UpdateWeaponSlots();
            UpdateArmorSlots();
            UpdateAmmoSlots();
            UpdateJewellerySlots();

            UpdateSpellSlots();

            UpdateEmbers(gt);

            controls.UpdateLast();

            Lerp(gt);

            equippedSlots.LerpHidden(gt, equipLerp * hideLerp);

            if (equipment.NumberSlot != lastQuickslot)
            {
                nameQuickslotLerp = 1f;
                nameFadeTime = 0;
            }

            lastQuickslot = equipment.NumberSlot;
        }

        int scrollValue = 0, quickslotLerpTimer = 0, equipLerpTimer = 0;
        private bool isQSLerpSet = false, isEquipLerpSet = false;
        public void UpdateControlShortcuts(GameTime gt)
        {
            if (quickslotLerpTimer >= 3000)
            {
                if (isQSLerpSet == false)
                {
                    quickslotTargetLerp = 0f;
                    isQSLerpSet = true;
                }
            }
            else
            {
                quickslotLerpTimer += gt.ElapsedGameTime.Milliseconds;
                isQSLerpSet = false;
            }

            if (equipLerpTimer >= 3000)
            {
                if (isEquipLerpSet == false)
                {
                    equipTargetLerp = 0f;
                    isEquipLerpSet = true;
                }
            }
            else
            {
                equipLerpTimer += gt.ElapsedGameTime.Milliseconds;
                isEquipLerpSet = false;
            }

            if (screens.IsClickingUI == false)
            {
                if (controls.CurrentKey.IsKeyDown(controls.CurrentControls.Sneak)) //If modifier key is held (sneak), change button target instead!
                {
                    if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                    {
                        equipment.ChangeButtonTarget(1);
                        screens.PlaySound("ButtonTargetScroll");
                        quickslotLerpTimer = 0;
                        quickslotTargetLerp = .9f;
                    }
                    else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                    {
                        equipment.ChangeButtonTarget(-1);
                        screens.PlaySound("ButtonTargetScroll");
                        quickslotLerpTimer = 0;
                        quickslotTargetLerp = .9f;
                    }
                }
                else
                {
                    if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                    {
                        equipment.AdjustQuickSlot(1);
                        screens.PlayRandom("SelectQuickSlot", "SelectQuickSlotHigher");
                        quickslotLerpTimer = 0;
                        quickslotTargetLerp = .9f;
                    }
                    else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                    {
                        equipment.AdjustQuickSlot(-1);
                        screens.PlayRandom("SelectQuickSlot", "SelectQuickSlotHigher");
                        quickslotLerpTimer = 0;
                        quickslotTargetLerp = .9f;
                    }
                }
            }
            scrollValue = controls.CurrentMS.ScrollWheelValue;
        }

        private CallLimiter limitQSCheck = new CallLimiter(1000);
        private void UpdateQuickSlots(GameTime gt)
        {
            qsContainer = new Rectangle((int)quickSlotOffset.X, (int)quickSlotOffset.Y, quickSlotBG.Width, quickSlotBG.Height);

            for (int i = 0; i < equipment.QuickSlots.Count; i++)
            {
                if (IsInventoryOpen == true)
                {
                    equipment.QuickSlots[i].deleteRect = new Rectangle(equipment.QuickSlots[i].slotRect.X + icon.Width - deleteSlot.Width,
                                                                       equipment.QuickSlots[i].slotRect.Y, deleteSlot.Width, deleteSlot.Height);

                    if (equipment.QuickSlots[i].deleteRect.Contains(controls.MousePosition))
                    {
                        if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        {
                            equipment.QuickSlots[i].item = null;
                            equipment.QuickSlots[i].ID = -1;
                            equipment.QuickSlots[i].Icon = null;
                        }
                    }
                }

                if (equipment.QuickSlots[i].item != null)
                {
                    if (equipment.QuickSlots[i].item.CurrentAmount <= 0)
                        equipment.QuickSlots[i].item = null;
                }
            }

            //Code for checking items
            if (limitQSCheck.IsCalling(gt))
            {
                for (int i = 0; i < equipment.QuickSlots.Count; i++)
                {
                    if (equipment.QuickSlots[i].Icon != null && equipment.QuickSlots[i].ID != -1)
                    {
                        InventoryScreen.ItemTypes.BaseItem item = currentEntity.STORAGE_GetItem(equipment.QuickSlots[i].ID);

                        if (item != null)
                            equipment.QuickSlots[i].item = item; //Auto-equip item to empty previous quickslot.
                    }
                }
            }
        }
        private void UpdateWeaponSlots()
        {
            equipment.WeaponOne.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 21, (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.WeaponTwo.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + (icon.Width + 1), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.WeaponThree.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 2), (int)screens.INVENTORY_Position.Y + 526, 64, 64);

            equipment.OffhandOne.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 4), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.OffhandTwo.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 5), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.OffhandThree.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 6), (int)screens.INVENTORY_Position.Y + 526, 64, 64);

            equipment.WeaponOne.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.WeaponTwo.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + (icon.Width + 1) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.WeaponThree.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 2) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);

            equipment.OffhandOne.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 4) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.OffhandTwo.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 5) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.OffhandThree.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 21 + ((icon.Width + 1) * 6) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
        }
        private void UpdateArmorSlots()
        {
            equipment.HeadSlot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55, (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.TorsoSlot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + (icon.Width + 1), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.LegsSlot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 2), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.FeetSlot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 3), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.HandsSlot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 4), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.CapeSlot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 5), (int)screens.INVENTORY_Position.Y + 526, 64, 64);

            equipment.HeadSlot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.TorsoSlot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + (icon.Width + 1) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.LegsSlot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 2) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.FeetSlot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 3) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.HandsSlot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 4) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.CapeSlot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + ((icon.Width + 1) * 5) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
        }
        private void UpdateAmmoSlots()
        {
            equipment.PrimaryAmmo1.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 80, (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.PrimaryAmmo2.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 84 + (icon.Width + 1), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.SecondaryAmmo1.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 156 + ((icon.Width + 1) * 2), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.SecondaryAmmo2.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 160 + ((icon.Width + 1) * 3), (int)screens.INVENTORY_Position.Y + 526, 64, 64);

            equipment.PrimaryAmmo1.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 80 + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.PrimaryAmmo2.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 84 + (icon.Width + 1) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.SecondaryAmmo1.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 156 + ((icon.Width + 1) * 2) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.SecondaryAmmo2.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 160 + ((icon.Width + 1) * 3) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
        }
        private void UpdateJewellerySlots()
        {
            equipment.Ring1Slot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55, (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.Ring2Slot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + (icon.Width + 1), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.NeckSlot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 85 + ((icon.Width + 1) * 2), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.Ring3Slot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 115 + ((icon.Width + 1) * 3), (int)screens.INVENTORY_Position.Y + 526, 64, 64);
            equipment.Ring4Slot.slotRect = new Rectangle((int)screens.INVENTORY_Position.X + 115 + ((icon.Width + 1) * 4), (int)screens.INVENTORY_Position.Y + 526, 64, 64);

            equipment.Ring1Slot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.Ring2Slot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 55 + (icon.Width + 1) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.NeckSlot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 85 + ((icon.Width + 1) * 2) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.Ring3Slot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 115 + ((icon.Width + 1) * 3) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
            equipment.Ring4Slot.deleteRect = new Rectangle((int)screens.INVENTORY_Position.X + 115 + ((icon.Width + 1) * 4) + (icon.Width - deleteSlot.Width), (int)screens.INVENTORY_Position.Y + 526, deleteSlot.Width, deleteSlot.Height);
        }
        private void UpdateEmbers(GameTime gt)
        {
            lastEmbers = currentEmbers;
            currentEmbers = currentEntity.Skills.ExperiencePoints;

            if (currentEmbers != lastEmbers)
                emberDifference += (currentEmbers - lastEmbers);

            if (emberDifference > 0)
            {
                emberFadeTimer += gt.ElapsedGameTime.Milliseconds;

                if (emberFadeTimer > 5500)
                {
                    emberDifference = 0;
                    emberFadeTimer = 0;
                }

                if (currentEmbers != lastEmbers)
                    emberFadeTimer = 0;

                if (emberFadeTimer < 5000)
                    emberText = Color.Lerp(Color.Orange, ColorHelper.DarkGray, ((float)emberFadeTimer / 5000));
                else
                    emberText = Color.Lerp(ColorHelper.DarkGray, Color.Transparent, ((float)(emberFadeTimer - 5000) / 500));
            }
        }

        private void UpdateSpellSlots()
        {
            equipment.SpellOne.slotRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 227, (int)screens.SPELLBOOK_Position.Y + 192, 64, 64);
            equipment.SpellTwo.slotRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 162, (int)screens.SPELLBOOK_Position.Y + 192, 64, 64);
            equipment.SpellThree.slotRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 97, (int)screens.SPELLBOOK_Position.Y + 192, 64, 64);
            equipment.SpellFour.slotRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 32, (int)screens.SPELLBOOK_Position.Y + 192, 64, 64);
            equipment.SpellFive.slotRect = new Rectangle((int)screens.SPELLBOOK_Position.X + 33, (int)screens.SPELLBOOK_Position.Y + 192, 64, 64);
            equipment.SpellSix.slotRect = new Rectangle((int)screens.SPELLBOOK_Position.X + 98, (int)screens.SPELLBOOK_Position.Y + 192, 64, 64);
            equipment.SpellSeven.slotRect = new Rectangle((int)screens.SPELLBOOK_Position.X + 163, (int)screens.SPELLBOOK_Position.Y + 192, 64, 64);

            equipment.SpellOne.deleteRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 227 + (icon.Width - deleteSlot.Width), (int)screens.SPELLBOOK_Position.Y + 192, deleteSlot.Width, deleteSlot.Height);
            equipment.SpellTwo.deleteRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 162 + (icon.Width - deleteSlot.Width), (int)screens.SPELLBOOK_Position.Y + 192, deleteSlot.Width, deleteSlot.Height);
            equipment.SpellThree.deleteRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 97 + (icon.Width - deleteSlot.Width), (int)screens.SPELLBOOK_Position.Y + 192, deleteSlot.Width, deleteSlot.Height);
            equipment.SpellFour.deleteRect = new Rectangle((int)screens.SPELLBOOK_Position.X - 32 + (icon.Width - deleteSlot.Width), (int)screens.SPELLBOOK_Position.Y + 192, deleteSlot.Width, deleteSlot.Height);
            equipment.SpellFive.deleteRect = new Rectangle((int)screens.SPELLBOOK_Position.X + 33 + (icon.Width - deleteSlot.Width), (int)screens.SPELLBOOK_Position.Y + 192, deleteSlot.Width, deleteSlot.Height);
            equipment.SpellSix.deleteRect = new Rectangle((int)screens.SPELLBOOK_Position.X + 98 + (icon.Width - deleteSlot.Width), (int)screens.SPELLBOOK_Position.Y + 192, deleteSlot.Width, deleteSlot.Height);
            equipment.SpellSeven.deleteRect = new Rectangle((int)screens.SPELLBOOK_Position.X + 163 + (icon.Width - deleteSlot.Width), (int)screens.SPELLBOOK_Position.Y + 192, deleteSlot.Width, deleteSlot.Height);
        }

        public bool IsInventoryOpen { get; set; }

        private Rectangle qsContainer;

        private void Lerp(GameTime gt)
        {
            isHUDHidden = screens.EFFECTS_IsWidescreen() || GameSettings.IsHidingHUD;
            if (isHUDHidden == true)
                hideLerp -= 5f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                hideLerp += 5f * (float)gt.ElapsedGameTime.TotalSeconds;
            hideLerp = MathHelper.Clamp(hideLerp, 0f, 1f);


            if (quickslotLerp < (quickslotTargetLerp - .01f))
                quickslotLerp += ((quickslotTargetLerp - quickslotLerp) * 3f) * (float)gt.ElapsedGameTime.TotalSeconds;
            if (quickslotLerp > (quickslotTargetLerp - .01f))
                quickslotLerp -= ((quickslotLerp - quickslotTargetLerp) * 3f) * (float)gt.ElapsedGameTime.TotalSeconds;

            if (equipLerp < (equipTargetLerp - .01f))
                equipLerp += ((equipTargetLerp - equipLerp) * 3f) * (float)gt.ElapsedGameTime.TotalSeconds;
            if (equipLerp > (equipTargetLerp - .01f))
                equipLerp -= ((equipLerp - equipTargetLerp) * 3f) * (float)gt.ElapsedGameTime.TotalSeconds;

            quickslotLerp = MathHelper.Clamp(quickslotLerp, .65f, 1f);
            equipLerp = MathHelper.Clamp(equipLerp, .65f, 1f);

            fadeWhite = Color.Lerp(Color.Transparent, Color.White, quickslotLerp * hideLerp);
            highlight = Color.Lerp(Color.Transparent, new Color(165, 174, 124, 255), quickslotLerp * hideLerp);
            counterText = Color.Lerp(Color.Transparent, emberText, quickslotLerp * hideLerp);
            unavailableQS = Color.Lerp(Color.Transparent, new Color(128, 128, 128, 128), quickslotLerp * hideLerp);

            if (nameQuickslotLerp > 0f)
            {
                if (nameFadeTime > 1000)
                {
                    nameQuickslotLerp -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                    nameQuickslotLerp = MathHelper.Clamp(nameQuickslotLerp, 0f, 1f);
                }
                else
                    nameFadeTime += gt.ElapsedGameTime.Milliseconds;

                nameQS = Color.Lerp(Color.Transparent, Color.White, nameQuickslotLerp * hideLerp);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            DrawQuickslots(sb);
            DrawEmbersMoney(sb);

            if (IsInventoryOpen == true)
            {
                if (screens.INVENTORY_Tab == InventoryUI.CurrentTab.Weapons)
                    DrawWeaponSlots(sb);
                else if (screens.INVENTORY_Tab == InventoryUI.CurrentTab.Armor)
                    DrawArmorSlots(sb);
                else if (screens.INVENTORY_Tab == InventoryUI.CurrentTab.Ammo)
                    DrawAmmoSlots(sb);
                else if (screens.INVENTORY_Tab == InventoryUI.CurrentTab.Jewellery)
                    DrawJewellerySlots(sb);
            }

            equippedSlots.Draw(sb);
        }
        private void DrawQuickslots(SpriteBatch sb)
        {
            sb.Draw(quickSlotBG, quickSlotOffset, fadeWhite, Vector2.Zero, 0f, 1f);

            for (int i = 0; i < equipment.QuickSlots.Count; i++)
            {
                Vector2 position = new Vector2(quickSlotOffset.X + (i * 65) + 163, quickSlotOffset.Y + 14);
                equipment.QuickSlots[i].slotRect = new Rectangle((int)position.X, (int)position.Y, icon.Width, icon.Height);

                if (equipment.selectedQuickSlot == (EntityEquipment.QSlotSelect)i)
                {
                    if (equipment.QuickSlots[i].item != null && nameQuickslotLerp > 0f)
                    {
                        sb.DrawStringShadow(largeFont, equipment.QuickSlots[i].item.Name, new Vector2(GameSettings.VectorCenter.X, GameSettings.VectorResolution.Y - (quickSlotBG.Height + 30)),
                                            equipment.QuickSlots[i].item.Name.LineCenter(largeFont), 0f, 1f, 1f, nameQS, Color.Lerp(Color.Transparent, ColorHelper.Charcoal, nameQuickslotLerp * hideLerp));
                    }

                    sb.Draw(iconSelected, position, fadeWhite, Vector2.Zero, 0f, 1f);
                }
                else
                    sb.Draw(icon, position, fadeWhite, Vector2.Zero, 0f, 1f);

                if (equipment.QuickSlots[i].item != null)
                {
                    if (equipment.QuickSlots[i].item.CurrentAmount > 0)
                    {
                        sb.Draw(equipment.QuickSlots[i].item.Icon, position, fadeWhite, Vector2.Zero, 0f, 1f);

                        if (currentEntity.SUSPENSION_Action == Performance.Suspension.SuspendState.Suspended)
                            sb.Draw(equipment.QuickSlots[i].item.Icon, position, Color.Gray, Vector2.Zero, 0f, 1f);

                        sb.DrawString(font, equipment.QuickSlots[i].item.CurrentAmount.ToString(), position + new Vector2(4, 0), Vector2.Zero, fadeWhite, 1f);

                        if (IsInventoryOpen)
                            sb.Draw(deleteSlot, equipment.QuickSlots[i].deleteRect, fadeWhite);

                        if (equipment.NumberSlot == i) //If the slot is selected, draw this slot's button text
                        {
                            DrawButtonText(sb, equipment.QuickSlots[i].item.ButtonOneText, 1, equipment.QuickSlots[i].item.buttonTarget);
                            DrawButtonText(sb, equipment.QuickSlots[i].item.ButtonTwoText, 2, equipment.QuickSlots[i].item.buttonTarget);
                            DrawButtonText(sb, equipment.QuickSlots[i].item.ButtonThreeText, 3, equipment.QuickSlots[i].item.buttonTarget);
                            DrawButtonText(sb, equipment.QuickSlots[i].item.ButtonFourText, 4, equipment.QuickSlots[i].item.buttonTarget);
                        }

                        if (equipment.QuickSlots[i].item.CurrentDurability < equipment.QuickSlots[i].item.MaxDurability)
                        {
                            int barWidth = 56 * equipment.QuickSlots[i].item.CurrentDurability / equipment.QuickSlots[i].item.MaxDurability;

                            sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 4, (int)position.Y + 59, barWidth, 1), Color.LightGray, Color.Black);
                        }
                    }
                }
                else
                {
                    if (equipment.QuickSlots[i].ID != -1)
                    {
                        if (equipment.QuickSlots[i].Icon != null)
                        {
                            sb.Draw(equipment.QuickSlots[i].Icon, position, unavailableQS, Vector2.Zero, 0f, 1f);
                            sb.DrawString(font, "0", position + new Vector2(4, 0), Vector2.Zero, unavailableQS, 1f);

                            if (IsInventoryOpen)
                                sb.Draw(deleteSlot, equipment.QuickSlots[i].deleteRect, fadeWhite);
                        }
                    }
                }
            }
        }

        private void DrawWeaponSlots(SpriteBatch sb)
        {
            sb.Draw(equipBG, screens.INVENTORY_Position + new Vector2(14, 524), Color.White);

            DrawWeaponSlot(sb, equipment.WeaponOne, EntityEquipment.WeaponSlot.One, true);
            DrawWeaponSlot(sb, equipment.WeaponTwo, EntityEquipment.WeaponSlot.Two, true);
            DrawWeaponSlot(sb, equipment.WeaponThree, EntityEquipment.WeaponSlot.Three, true);
            DrawWeaponSlot(sb, equipment.OffhandOne, EntityEquipment.WeaponSlot.One, false);
            DrawWeaponSlot(sb, equipment.OffhandTwo, EntityEquipment.WeaponSlot.Two, false);
            DrawWeaponSlot(sb, equipment.OffhandThree, EntityEquipment.WeaponSlot.Three, false);

            DrawItem(sb, equipment.WeaponOne);
            DrawItem(sb, equipment.WeaponTwo);
            DrawItem(sb, equipment.WeaponThree);
            DrawItem(sb, equipment.OffhandOne);
            DrawItem(sb, equipment.OffhandTwo);
            DrawItem(sb, equipment.OffhandThree);
        }
        private void DrawArmorSlots(SpriteBatch sb)
        {
            sb.Draw(equipBG, screens.INVENTORY_Position + new Vector2(14, 524), Color.White);

            sb.Draw(icon, equipment.HeadSlot.slotRect, Color.White);
            sb.Draw(icon, equipment.TorsoSlot.slotRect, Color.White);
            sb.Draw(icon, equipment.LegsSlot.slotRect, Color.White);
            sb.Draw(icon, equipment.FeetSlot.slotRect, Color.White);
            sb.Draw(icon, equipment.HandsSlot.slotRect, Color.White);
            sb.Draw(icon, equipment.CapeSlot.slotRect, Color.White);

            DrawItem(sb, equipment.HeadSlot);
            DrawItem(sb, equipment.TorsoSlot);
            DrawItem(sb, equipment.LegsSlot);
            DrawItem(sb, equipment.FeetSlot);
            DrawItem(sb, equipment.HandsSlot);
            DrawItem(sb, equipment.CapeSlot);
        }
        private void DrawAmmoSlots(SpriteBatch sb)
        {
            sb.Draw(equipBG, screens.INVENTORY_Position + new Vector2(14, 524), Color.White);

            sb.Draw(icon, equipment.PrimaryAmmo1.slotRect, Color.White);
            sb.Draw(icon, equipment.PrimaryAmmo2.slotRect, Color.White);
            sb.Draw(icon, equipment.SecondaryAmmo1.slotRect, Color.White);
            sb.Draw(icon, equipment.SecondaryAmmo2.slotRect, Color.White);

            DrawItem(sb, equipment.PrimaryAmmo1);
            DrawItem(sb, equipment.PrimaryAmmo2);
            DrawItem(sb, equipment.SecondaryAmmo1);
            DrawItem(sb, equipment.SecondaryAmmo2);
        }
        private void DrawJewellerySlots(SpriteBatch sb)
        {
            sb.Draw(equipBG, screens.INVENTORY_Position + new Vector2(14, 524), Color.White);

            sb.Draw(icon, equipment.NeckSlot.slotRect, Color.White);
            sb.Draw(icon, equipment.Ring1Slot.slotRect, Color.White);
            sb.Draw(icon, equipment.Ring2Slot.slotRect, Color.White);
            sb.Draw(icon, equipment.Ring3Slot.slotRect, Color.White);
            sb.Draw(icon, equipment.Ring4Slot.slotRect, Color.White);

            DrawItem(sb, equipment.NeckSlot);
            DrawItem(sb, equipment.Ring1Slot);
            DrawItem(sb, equipment.Ring2Slot);
            DrawItem(sb, equipment.Ring3Slot);
            DrawItem(sb, equipment.Ring4Slot);
        }

        public void DrawSpellSlots(SpriteBatch sb)
        {
            DrawSpell(sb, equipment.SpellOne, 1);
            DrawSpell(sb, equipment.SpellTwo, 2);
            DrawSpell(sb, equipment.SpellThree, 3);
            DrawSpell(sb, equipment.SpellFour, 4);
            DrawSpell(sb, equipment.SpellFive, 5);
            DrawSpell(sb, equipment.SpellSix, 6);
            DrawSpell(sb, equipment.SpellSeven, 7);
        }

        private void DrawButtonText(SpriteBatch sb, string buttonText, int index, int currentIndex)
        {
            if (!string.IsNullOrEmpty(buttonText))
            {
                if (currentIndex == index)
                {
                    if (index == 1)
                        sb.Draw(pixel, new Rectangle((int)quickSlotOffset.X + 180, (int)quickSlotOffset.Y + 5, 152, 1), highlight);
                    if (index == 2)
                        sb.Draw(pixel, new Rectangle((int)quickSlotOffset.X + 335, (int)quickSlotOffset.Y + 5, 150, 1), highlight);
                    if (index == 3)
                        sb.Draw(pixel, new Rectangle((int)quickSlotOffset.X + 488, (int)quickSlotOffset.Y + 5, 150, 1), highlight);
                    if (index == 4)
                        sb.Draw(pixel, new Rectangle((int)quickSlotOffset.X + 641, (int)quickSlotOffset.Y + 5, 154, 1), highlight);

                    sb.DrawString(font, buttonText, quickSlotOffset + new Vector2(255 + (154 * (index - 1)), 0), buttonText.LineCenter(font), highlight, 1f);
                }
                else
                    sb.DrawString(font, buttonText, quickSlotOffset + new Vector2(255 + (154 * (index - 1)), +2), buttonText.LineCenter(font), fadeWhite, 1f);
            }
        }
        private void DrawItem(SpriteBatch sb, EquipSlot slot)
        {
            if (slot.item != null)
            {
                if (slot.item.MeetsRequirements() == true)
                    sb.Draw(slot.item.Icon, new Rectangle(slot.slotRect.X, slot.slotRect.Y, slot.item.Icon.Width, slot.item.Icon.Height), Color.White);
                else
                    sb.Draw(slot.item.Icon, new Rectangle(slot.slotRect.X, slot.slotRect.Y, slot.item.Icon.Width, slot.item.Icon.Height), noRequirements);
            }

            sb.Draw(deleteSlot, slot.deleteRect, Color.White);
        }
        private void DrawWeaponSlot(SpriteBatch sb, EquipSlot slot, EntityEquipment.WeaponSlot index, bool isPrimary)
        {
            if (isPrimary)
            {
                if ((EntityEquipment.WeaponSlot)(equipment.PrimaryWeaponIndex - 1) == index) sb.Draw(iconSelected, slot.slotRect, Color.White);
                else sb.Draw(icon, slot.slotRect, Color.White);
            }
            else
            {
                if ((EntityEquipment.WeaponSlot)(equipment.OffhandWeaponIndex - 1) == index) sb.Draw(iconSelected, slot.slotRect, Color.White);
                else sb.Draw(icon, slot.slotRect, Color.White);
            }
        }
        private void DrawSpell(SpriteBatch sb, SpellSlot slot, int slotIndex)
        {
            if (slotIndex <= currentEntity.MAGIC_SpellSlots())
            {
                sb.Draw(icon, slot.slotRect, Color.White);

                if (slot.spell != null)
                {
                    sb.Draw(slot.spell.Icon, new Rectangle(slot.slotRect.X, slot.slotRect.Y, slot.spell.Icon.Width, slot.spell.Icon.Height), Color.White);
                    sb.Draw(deleteSlot, slot.deleteRect, Color.White);
                }
            }
            else
            {
                sb.Draw(icon, slot.slotRect, new Color(128, 128, 128, 128));
            }
        }

        private int currentEmbers, lastEmbers, emberDifference = 0, emberFadeTimer = 0;
        private void DrawEmbersMoney(SpriteBatch sb)
        {
            string embers = currentEntity.Skills.ExperiencePoints.CommaSeparation();
            string renown = ((int)currentEntity.RENOWN_Value).ToString();

            if (emberDifference > 0)
                sb.DrawString(counterFont, "+" + emberDifference, new Vector2(quickSlotOffset.X + 135, quickSlotOffset.Y + 45), new Vector2(counterFont.MeasureString(emberDifference.ToString()).X, counterFont.MeasureString(emberDifference.ToString()).Y / 2), emberText, 1f);

            sb.DrawString(counterFont, embers, new Vector2(quickSlotOffset.X + 150, quickSlotOffset.Y + 69), new Vector2(counterFont.MeasureString(embers).X, counterFont.MeasureString(embers).Y / 2), counterText, 1f);
            sb.DrawString(counterFont, renown, new Vector2(quickSlotOffset.X + 825, quickSlotOffset.Y + 69), new Vector2(0, counterFont.MeasureString(renown).Y / 2), counterText, 1f);

            sb.Draw(emberIcon, new Vector2(quickSlotOffset.X + 7, quickSlotOffset.Y + 60), fadeWhite);
            //sb.Draw(renownIcon, new Vector2(quickSlotOffset.X + 950, quickSlotOffset.Y + 60), fadeWhite);
        }

        public bool IsClickingUI()
        {
            return qsContainer.Contains(controls.MousePosition);
        }
        public void ResetLerp()
        {
            equipLerpTimer = 0;
            equipTargetLerp = .9f;
        }
        public void ResetPosition()
        {
            quickSlotOffset = new Vector2(GameSettings.WindowCenter.X - (quickSlotBG.Width / 2), GameSettings.WindowResolution.Y - quickSlotBG.Height);
            equippedSlots.ResetPosition();
        }
    }
}
