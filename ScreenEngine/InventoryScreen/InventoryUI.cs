using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.Entities;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen
{
    public class InventoryUI
    {
        Controls controls = new Controls();

        Temp.InventoryTab consumables = new Temp.InventoryTab(BaseItem.TabType.Consumables);
        Temp.InventoryTab weapons = new Temp.InventoryTab(BaseItem.TabType.Weapons);
        Temp.InventoryTab armor = new Temp.InventoryTab(BaseItem.TabType.Armor);
        Temp.InventoryTab ammo = new Temp.InventoryTab(BaseItem.TabType.Ammo);
        Temp.InventoryTab jewellery = new Temp.InventoryTab(BaseItem.TabType.Jewellery);
        Temp.InventoryTab resources = new Temp.InventoryTab(BaseItem.TabType.Resources);
        Temp.InventoryTab miscellaneous = new Temp.InventoryTab(BaseItem.TabType.Miscellaneous);

        public enum CurrentTab { Consumables, Weapons, Armor, Ammo, Jewellery, Resources, Miscellaneous }
        CurrentTab tab = CurrentTab.Consumables;
        public CurrentTab Tab { get { return tab; } }

        private Texture2D tabButton, tabButtonSelect, windowButton, windowButtonHover, hintIcon, hideIcon;
        private SpriteFont largeFont;

        //private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        private Vector2 screenPosition = new Vector2(0, 0);
        public Vector2 ScreenPosition
        {
            get { return screenPosition; }
            set
            {
                screenPosition = new Vector2(MathHelper.Clamp(value.X, 40, GameSettings.WindowResolution.X - 880),
                                             MathHelper.Clamp(value.Y, 1, GameSettings.WindowResolution.Y - 525));
            }
        }

        public void ResetPosition()
        {
            screenPosition = new Vector2(GameSettings.VectorCenter.X - (consumables.TabBGWidth.X / 2), GameSettings.VectorCenter.Y - consumables.TabBGWidth.Y / 2);
        }

        private Rectangle[] buttonRects = new Rectangle[7];
        private Rectangle dragArea, tabRect;

        public bool IsActive { get; set; }

        private ScreenManager screens;
        public BaseItem transTabCombining { get; set; }

        public InventoryUI()
        {
        }
        public void SetReferences(ScreenManager screens, Camera camera)
        {
            this.screens = screens;

            consumables.SetReferences(this, screens, camera);
            weapons.SetReferences(this, screens, camera);
            armor.SetReferences(this, screens, camera);
            ammo.SetReferences(this, screens, camera);
            jewellery.SetReferences(this, screens, camera);
            resources.SetReferences(this, screens, camera);
            miscellaneous.SetReferences(this, screens, camera);
        }

        private const string directory = "Interface/Inventory/";
        public void Load(ContentManager cm)
        {
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            tabButton = cm.Load<Texture2D>(directory + "Icons/tabButton");
            tabButtonSelect = cm.Load<Texture2D>(directory + "Icons/tabButtonSelect");

            consumables.Load(cm);
            weapons.Load(cm);
            armor.Load(cm);
            ammo.Load(cm);
            jewellery.Load(cm);
            resources.Load(cm);
            miscellaneous.Load(cm);

            consumables.TabIcon = cm.Load<Texture2D>(directory + "Icons/Tabs/consumable");
            weapons.TabIcon = cm.Load<Texture2D>(directory + "Icons/Tabs/weapon");
            armor.TabIcon = cm.Load<Texture2D>(directory + "Icons/Tabs/armor");
            ammo.TabIcon = cm.Load<Texture2D>(directory + "Icons/Tabs/ammo");
            jewellery.TabIcon = cm.Load<Texture2D>(directory + "Icons/Tabs/jewellery");
            resources.TabIcon = cm.Load<Texture2D>(directory + "Icons/Tabs/resources");
            miscellaneous.TabIcon = cm.Load<Texture2D>(directory + "Icons/Tabs/miscellaneous");

            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            ScreenPosition = new Vector2(GameSettings.WindowCenter.X - consumables.TabBGWidth.X, GameSettings.WindowCenter.Y - consumables.TabBGWidth.Y);
            mouseDragOffset = new Vector2(consumables.TabBGWidth.X, 12);
        }
        public void Update(GameTime gt)
        {
            isClickingTabButton = false;
            dragArea = new Rectangle(((consumables.TabBGWidth.X / 2) - 20) + (int)screenPosition.X, (int)screenPosition.Y, 290, 20);

            if (tab == CurrentTab.Weapons || tab == CurrentTab.Armor || tab == CurrentTab.Ammo || tab == CurrentTab.Jewellery)
                tabRect = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, consumables.TabBGWidth.X * 2, consumables.TabBGWidth.Y * 2 + 74);
            else
                tabRect = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, consumables.TabBGWidth.X * 2, consumables.TabBGWidth.Y * 2);

            controls.UpdateLast();
            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenInventory))
            {
                IsActive = !IsActive;
                Logger.AppendLine("Opened inventory UI");
            }

            consumables.AlwaysUpdate(gt, controls);
            weapons.AlwaysUpdate(gt, controls);
            armor.AlwaysUpdate(gt, controls);
            ammo.AlwaysUpdate(gt, controls);
            jewellery.AlwaysUpdate(gt, controls);
            resources.AlwaysUpdate(gt, controls);
            miscellaneous.AlwaysUpdate(gt, controls);

            if (!currentEntity.IsDead)
                CheckQuickSlotControls();

            if (IsActive == true)
            {
                if (IsDraggingItem() == false)
                    CheckDragScreen();

                CheckTabClick(0, CurrentTab.Consumables);
                CheckTabClick(1, CurrentTab.Weapons);
                CheckTabClick(2, CurrentTab.Armor);
                CheckTabClick(3, CurrentTab.Ammo);
                CheckTabClick(4, CurrentTab.Jewellery);
                CheckTabClick(5, CurrentTab.Resources);
                CheckTabClick(6, CurrentTab.Miscellaneous);

                switch (tab)
                {
                    case CurrentTab.Consumables: consumables.Update(gt, screenPosition, dragArea); break;
                    case CurrentTab.Weapons: weapons.Update(gt, screenPosition, dragArea); break;
                    case CurrentTab.Armor: armor.Update(gt, screenPosition, dragArea); break;
                    case CurrentTab.Ammo: ammo.Update(gt, screenPosition, dragArea); break;
                    case CurrentTab.Jewellery: jewellery.Update(gt, screenPosition, dragArea); break;
                    case CurrentTab.Resources: resources.Update(gt, screenPosition, dragArea); break;
                    case CurrentTab.Miscellaneous: miscellaneous.Update(gt, screenPosition, dragArea); break;
                }

                isClickingTab = tabRect.Contains(controls.MousePosition);

                // --- Window Buttons Checking ---
                hintRect = new Rectangle((int)screenPosition.X + 393, (int)screenPosition.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)screenPosition.X + 420, (int)screenPosition.Y, windowButton.Width - 20, windowButton.Height);

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
                        IsActive = false;
                    }
                }
                else
                    isHideHover = false;
            }
        }
        private void CheckQuickSlotControls()
        {
            if (controls.IsKeyPressedOnce(controls.CurrentControls.LoopQSLeft))
                equipment.AdjustQuickSlot(-1);
            else if (controls.IsKeyPressedOnce(controls.CurrentControls.LoopQSRight))
                equipment.AdjustQuickSlot(1);

            if (controls.IsKeyPressedOnce(controls.CurrentControls.ButtonTargetLeft))
                equipment.ChangeButtonTarget(-1);
            else if (controls.IsKeyPressedOnce(controls.CurrentControls.ButtonTargetRight))
                equipment.ChangeButtonTarget(1);

            if (controls.IsKeyPressedOnce(controls.CurrentControls.Quickslot) && currentEntity.SUSPENSION_Action == Performance.Suspension.SuspendState.None)
                equipment.UseQuickSlotItem();

            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS1))
                equipment.SelectQuickSlot(0);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS2))
                equipment.SelectQuickSlot(1);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS3))
                equipment.SelectQuickSlot(2);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS4))
                equipment.SelectQuickSlot(3);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS5))
                equipment.SelectQuickSlot(4);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS6))
                equipment.SelectQuickSlot(5);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS7))
                equipment.SelectQuickSlot(6);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS8))
                equipment.SelectQuickSlot(7);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS9))
                equipment.SelectQuickSlot(8);
            if (controls.IsKeyPressedOnce(controls.CurrentControls.SelectQS10))
                equipment.SelectQuickSlot(9);
        }
        private void CheckTabClick(int index, CurrentTab specificTab)
        {
            if (buttonRects[index].Contains(controls.MousePosition))
            {
                isClickingTabButton = true;

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 2");
                    tab = specificTab;
                }
            }
        }
        private bool isClickingTabButton, isClickingTab;

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private void CheckDragScreen()
        {
            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                ScreenPosition = controls.MouseVector - mouseDragOffset;
                screens.SetCursorState(Cursor.CursorState.Move);
            }
        }
        public bool IsDraggingItem()
        {
            return (consumables.IsDraggingItem()) ||
                    (weapons.IsDraggingItem()) ||
                    (armor.IsDraggingItem()) ||
                    (ammo.IsDraggingItem()) ||
                    (jewellery.IsDraggingItem()) ||
                    (resources.IsDraggingItem()) ||
                    (miscellaneous.IsDraggingItem());
        }

        private bool isDropShadowsCreated = false;
        private Texture2D tabButtonShadow;
        public void Draw(SpriteBatch sb)
        {
            // --- Remove shadows ---
            if (isDropShadowsCreated == false)
            {
                tabButtonShadow = tabButton.CreateShadow(sb, .25f);

                consumables.CreateShadows(sb, .25f);
                weapons.CreateShadows(sb, .25f);
                armor.CreateShadows(sb, .25f);
                ammo.CreateShadows(sb, .25f);
                jewellery.CreateShadows(sb, .25f);
                resources.CreateShadows(sb, .25f);
                miscellaneous.CreateShadows(sb, .25f);

                isDropShadowsCreated = true; //Create the shadows only once!
            }

            if (IsActive == true)
            {
                sb.Begin();

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

                switch (tab)
                {
                    case CurrentTab.Consumables: consumables.DrawShadows(sb, new Vector2(4, 4)); break;
                    case CurrentTab.Weapons: weapons.DrawShadows(sb, new Vector2(4, 4)); break;
                    case CurrentTab.Armor: armor.DrawShadows(sb, new Vector2(4, 4)); break;
                    case CurrentTab.Ammo: ammo.DrawShadows(sb, new Vector2(4, 4)); break;
                    case CurrentTab.Jewellery: jewellery.DrawShadows(sb, new Vector2(4, 4)); break;
                    case CurrentTab.Resources: resources.DrawShadows(sb, new Vector2(4, 4)); break;
                    case CurrentTab.Miscellaneous: miscellaneous.DrawShadows(sb, new Vector2(4, 4)); break;
                }

                DrawTabButton(sb, 0, CurrentTab.Consumables, consumables);
                DrawTabButton(sb, 1, CurrentTab.Weapons, weapons);
                DrawTabButton(sb, 2, CurrentTab.Armor, armor);
                DrawTabButton(sb, 3, CurrentTab.Ammo, ammo);
                DrawTabButton(sb, 4, CurrentTab.Jewellery, jewellery);
                DrawTabButton(sb, 5, CurrentTab.Resources, resources);
                DrawTabButton(sb, 6, CurrentTab.Miscellaneous, miscellaneous);

                sb.End();

                switch (tab)
                {
                    case CurrentTab.Consumables: consumables.Draw(sb); consumables.DrawDrag(sb); break;
                    case CurrentTab.Weapons: weapons.Draw(sb); weapons.DrawDrag(sb); break;
                    case CurrentTab.Armor: armor.Draw(sb); armor.DrawDrag(sb); break;
                    case CurrentTab.Ammo: ammo.Draw(sb); ammo.DrawDrag(sb); break;
                    case CurrentTab.Jewellery: jewellery.Draw(sb); jewellery.DrawDrag(sb); break;
                    case CurrentTab.Resources: resources.Draw(sb); resources.DrawDrag(sb); break;
                    case CurrentTab.Miscellaneous: miscellaneous.Draw(sb); miscellaneous.DrawDrag(sb); break;
                }

                sb.Begin();

                sb.DrawString(largeFont, "Inventory", new Vector2(screenPosition.X + consumables.TabBGWidth.X, screenPosition.Y + 14), "Inventory".LineCenter(largeFont), ColorHelper.UI_Gold, 1f);
                sb.DrawString(largeFont, tab.ToString(), new Vector2(screenPosition.X + consumables.TabBGWidth.X, screenPosition.Y + 50), tab.ToString().LineCenter(largeFont), Color.White, 1f);

                sb.End();
            }
        }

        private Vector2 offset = new Vector2(-38, 40); private Color halfTransparent = Color.Lerp(Color.White, Color.Transparent, .5f);
        private void DrawTabButton(SpriteBatch sb, int index, CurrentTab specificTab, Temp.InventoryTab inventoryTab)
        {
            buttonRects[index] = new Rectangle((int)screenPosition.X + (int)offset.X,
                                                   (int)screenPosition.Y + (int)offset.Y + (index * tabButton.Height),
                                                   tabButton.Width, tabButton.Height);

            sb.Draw(tabButtonShadow, new Rectangle(buttonRects[index].X + 8, buttonRects[index].Y + 8, buttonRects[index].Width, buttonRects[index].Height), Color.White);

            if (tab == specificTab)
            {
                sb.Draw(tabButtonSelect, buttonRects[index], Color.White);
                sb.Draw(inventoryTab.TabIcon, new Rectangle(buttonRects[index].X, buttonRects[index].Y, inventoryTab.TabIcon.Width, inventoryTab.TabIcon.Height), Color.White);
            }
            else if (tab != specificTab)
            {
                sb.Draw(tabButton, buttonRects[index], Color.White);
                sb.Draw(inventoryTab.TabIcon, new Rectangle(buttonRects[index].X, buttonRects[index].Y, inventoryTab.TabIcon.Width, inventoryTab.TabIcon.Height), halfTransparent);
            }
        }

        //Helper Methods

        //For when all item data is controlled through "EntityStorage.cs". This will allow the player to switch to a different entity and make use of their storage.
        private EntityEquipment equipment; private BaseEntity currentEntity;
        public void SetItemData(BaseEntity currentEntity, EntityStorage storage, EntityEquipment equipment)
        {
            this.currentEntity = currentEntity;
            this.equipment = equipment;

            consumables.SetItemData(currentEntity, storage, equipment);
            weapons.SetItemData(currentEntity, storage, equipment);
            armor.SetItemData(currentEntity, storage, equipment);
            ammo.SetItemData(currentEntity, storage, equipment);
            jewellery.SetItemData(currentEntity, storage, equipment);
            resources.SetItemData(currentEntity, storage, equipment);
            miscellaneous.SetItemData(currentEntity, storage, equipment);
        }
        public void SetMapEntities(List<BaseEntity> allEntities)
        {
            consumables.SetEntityData(allEntities);
            weapons.SetEntityData(allEntities);
            armor.SetEntityData(allEntities);
            ammo.SetEntityData(allEntities);
            jewellery.SetEntityData(allEntities);
            resources.SetEntityData(allEntities);
            miscellaneous.SetEntityData(allEntities);
        }

        private bool IsPaneClickingAll()
        {
            return (consumables.IsPaneClicking()) ||
                    (weapons.IsPaneClicking()) ||
                    (armor.IsPaneClicking()) ||
                    (ammo.IsPaneClicking()) ||
                    (jewellery.IsPaneClicking()) ||
                    (resources.IsPaneClicking()) ||
                    (miscellaneous.IsPaneClicking());
        }

        public bool IsClickingUI()
        {
            if (IsActive == true)
            {
                return isClickingTabButton || isClickingTab || IsPaneClickingAll() || isDragging || hideRect.Contains(controls.MousePosition);
            }
            else
                return false;
        }
    }
}
