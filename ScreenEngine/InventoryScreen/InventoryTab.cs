using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.Physics.Swooshes;
using Pilgrimage_Of_Embers.Physics.Swooshes.Types;
using Pilgrimage_Of_Embers.Extensions;
using System.Linq;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.Temp
{
    public class InventoryTab : BaseScreen
    {
        private BaseItem.TabType tabType;
        public BaseItem.TabType TabType { get { return tabType; } }
        private Texture2D tabIcon, pixel;
        public Texture2D TabIcon { get { return tabIcon; } set { tabIcon = value; } }

        private Vector2 screenPosition = Vector2.Zero, gridOffset;

        private float gridY, gridScrollValue, gridScrollVelocity; //Grid scroll variables
        private float descY, descScrollValue, descScrollVelocity; //Pane description scroll variables
        private float attrY, attrScrollValue, attrScrollVelocity; //Pane attribute scroll variables

        int descLineBounds, attrLineBounds;

        private List<BaseItem> itemData;

        private float currentPaneWidth = 1f;

        private SpriteFont font, largeFont;
        private Texture2D paneBG, paneEnd, tabBG;
        private Texture2D iconBG, iconBGSelected, paneButton, paneButtonHover, smallButton, smallButtonHover, paneButtonE, paneButtonHoverE;
        private Texture2D disposeOverlay, essentialIcon, entityIcon, newIcon, blankIcon;

        private Point currentFrame = new Point(0, 0);
        private BasicAnimation animation = new BasicAnimation();

        private Point tabBGWidth;
        public Point TabBGWidth { get { return tabBGWidth; } }

        private Rectangle paneRect, dragScreenRect;

        protected MenuButton paneButtonOne, paneButtonTwo, paneButtonThree, paneButtonFour, paneButtonFive, paneButtonSix;
        protected MenuButton disposeItem;

        private BaseSwoosh dragSwoosh = new AirSwoosh(Vector2.Zero, Vector2.Zero, 10, 0, 30f, Color.Lerp(Color.Transparent, Color.Black, .2f));

        private Camera camera;
        InventoryUI inventory;

        public InventoryTab(BaseItem.TabType TabType)
        {
            itemData = new List<BaseItem>();

            tabType = TabType;
            tabIcon = TabIcon;
        }
        public void SetReferences(InventoryUI inventory, ScreenManager screens, Camera camera)
        {
            this.inventory = inventory;
            this.screens = screens;
            this.camera = camera;
        }

        private const string directory = "Interface/Inventory/";
        public override void Load(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");

            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            tabBG = cm.Load<Texture2D>(directory + "Backgrounds/tabBG");
            paneBG = cm.Load<Texture2D>(directory + "Backgrounds/paneBG");
            paneEnd = cm.Load<Texture2D>(directory + "Backgrounds/paneEnd");

            iconBG = cm.Load<Texture2D>(directory + "Icons/iconBG");
            iconBGSelected = cm.Load<Texture2D>(directory + "Icons/iconBGSelect");

            paneButton = cm.Load<Texture2D>(directory + "Icons/paneButton");
            paneButtonHover = cm.Load<Texture2D>(directory + "Icons/paneButtonHover");

            smallButton = cm.Load<Texture2D>(directory + "Icons/smallButton");
            smallButtonHover = cm.Load<Texture2D>(directory + "Icons/smallButtonHover");

            paneButtonE = cm.Load<Texture2D>(directory + "Icons/paneExtendedButton");
            paneButtonHoverE = cm.Load<Texture2D>(directory + "Icons/paneExtendedButtonHover");

            disposeOverlay = cm.Load<Texture2D>(directory + "Icons/disposeIcon");

            essentialIcon = cm.Load<Texture2D>("Interface/Shared/redIcon");
            entityIcon = cm.Load<Texture2D>("Interface/Shared/blueIcon");
            newIcon = cm.Load<Texture2D>("Interface/Shared/newIcon");
            blankIcon = cm.Load<Texture2D>("Interface/Shared/whiteIcon");

            tabBGWidth = new Point(tabBG.Width / 2, tabBG.Height / 2);

            AssignPaneButtons(cm);

            dragSwoosh.Load(cm);
        }

        private void AssignPaneButtons(ContentManager cm)
        {
            paneButtonOne = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);
            paneButtonTwo = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);
            paneButtonThree = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);
            paneButtonFour = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);

            paneButtonFive = new MenuButton(Vector2.Zero, paneButtonE, paneButtonE, paneButtonHoverE, 1f, true);
            paneButtonSix = new MenuButton(Vector2.Zero, paneButtonE, paneButtonE, paneButtonHoverE, 1f, true);

            disposeItem = new MenuButton(Vector2.Zero, iconBG, iconBG, iconBGSelected, 1f, true);
        }

        private bool isItemSelected; private int draggedItemIndex;
        private Vector2 lastMousePosition;

        public void Update(GameTime gt, Vector2 screenPosition, Rectangle dragScreen)
        {
            controls.UpdateCurrent();

            this.screenPosition = screenPosition;
            this.dragScreenRect = dragScreen;

            paneRect = new Rectangle((int)screenPosition.X + tabBG.Width - 10, (int)screenPosition.Y + 40, (int)currentPaneWidth + 6, paneBG.Height);

            //ScrollGrid(gt, 50f, 500f, 300f, 10f);
            //EXPERIMENTAL!

            SmoothScroll(gt, 50f, 500f, 300f, 10f, ref gridY, ref gridScrollValue, ref gridScrollVelocity, -(((itemData.Count - 1) / 7) * (iconBG.Height + 1)) + (scissorGrid.Height - 64), scissorGrid);
            SmoothScroll(gt, 50f, 250f, 200f, 7.5f, ref descY, ref descScrollValue, ref descScrollVelocity, -(descLineBounds) + 165, scissorDesc); //-500 is temporary!
            SmoothScroll(gt, 50f, 250f, 200f, 7.5f, ref attrY, ref attrScrollValue, ref attrScrollVelocity, -(attrLineBounds) + 65, scissorAttr); //-500 is temporary!

            isItemSelected = false;

            if (inventory.transTabCombining != null)
            {
                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    inventory.transTabCombining = null; //Make the item null when left clicking anywhere.

                if (IsHoveringOnItem() == true)
                    screens.SetCursorState(Cursor.CursorState.Activate);
                else
                    screens.SetCursorState(Cursor.CursorState.Precision);
            }

            for (int i = 0; i < itemData.Count; i++)
            {
                if (itemData[i].isSelected == true)
                {
                    itemData[i].IsNew = false;

                    if (itemData[i].itemRect.Contains(controls.MousePosition))
                    {
                        if (controls.IsDoubleClicked(gt, Controls.MouseButton.RightClick) && !string.IsNullOrEmpty(itemData[i].ButtonOneText))
                            itemData[i].ButtonOne();

                        if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        {
                            if (Vector2.Distance(lastMousePosition, controls.MouseVector) > 5f)
                            {
                                draggedItem = itemData[i];
                                draggedItemIndex = i;
                            }
                        }
                        else
                            lastMousePosition = controls.MouseVector;
                    }
                }

                if (draggedItem != null)
                    CheckItemSwapping(draggedItemIndex, i);

                CheckItemSelect(itemData[i]);

                if (itemData[i].isSelected == true)
                    isItemSelected = true;

                CheckItemCombiningTransTab(i);
            }

            if (draggedItem != null)
            {
                CheckDisposeItem();
                CheckItemGifting();
            }

            if (disposeItem.ButtonRectangle.Contains(controls.MousePosition))
            {
                if (disposalItem != null)
                {
                    ToolTip.RequestStringAssign("Left-click to restore" + Environment.NewLine + "Right-click to dispose");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        disposalItem = null;
                    else if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    {
                        disposalItem.CurrentAmount = 0;
                        itemData.Remove(disposalItem);
                        disposalItem = null;

                        screens.PlaySound("DisposeItem");
                    }
                }

                if (draggedItem != null)
                {
                    if (draggedItem.IsEssential == true && draggedItem.IsEntityItem == true)
                        ToolTip.RequestStringAssign("Essential or protected items cannot be disposed!");
                    else if (draggedItem.IsEntityItem == true)
                        ToolTip.RequestStringAssign("Protected items cannot be disposed!");
                    else if (draggedItem.IsEssential == true)
                        ToolTip.RequestStringAssign("Essential items cannot be disposed!");
                }
            }

            if (tabType == BaseItem.TabType.Weapons)
                CheckWeaponControls();
            if (tabType == BaseItem.TabType.Armor)
                CheckArmorControls();
            if (tabType == BaseItem.TabType.Ammo)
                CheckAmmoControls();
            if (tabType == BaseItem.TabType.Jewellery)
                CheckJewelleryControls();

            CheckStoneholdAdding();
            CheckBarterAdding();
            CheckPurchaseQuickselling();
            CheckSmelterAdding();
            CheckBrewingAdding();

            if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                draggedItem = null;
                draggedItemIndex = -1;

                lastMousePosition = controls.MouseVector;
            }

            ChangePaneWidth(gt, isItemSelected &&
                                (!IsDraggingItem() ||
                                new Rectangle(scissorGrid.X, scissorGrid.Y,
                                              scissorGrid.Width, scissorGrid.Height + 70).Contains(controls.MousePosition)));

            ApplyProperItemGrid();
            UpdateButtons(gt);

            UpdateDragging(gt);

            currentFrame = animation.FramePosition(gt, 50, new Point(8, 0), true);

            controls.UpdateLast();
        }
        private void ApplyProperItemGrid()
        {
            gridOffset = new Vector2(screenPosition.X + 20, screenPosition.Y + 71);

            if (itemData.Count > 6)
            {
                for (int index = 0; index < itemData.Count; ++index)
                {
                    itemData[index].gridLocation = new Point(index % 7, (index / 7));
                    itemData[index].UpdateRect(gridOffset, (int)gridY);
                }
            }
            else if (itemData.Count < 7)
            {
                for (int index = 0; index < itemData.Count; ++index)
                {
                    itemData[index].gridLocation = new Point(index, 0);
                    itemData[index].UpdateRect(gridOffset, (int)gridY);
                }
            }
        }

        private int offsetY = 369;
        private void UpdateButtons(GameTime gt)
        {
            paneButtonOne.Update(gt, controls);
            paneButtonTwo.Update(gt, controls);
            paneButtonThree.Update(gt, controls);
            paneButtonFour.Update(gt, controls);
            paneButtonFive.Update(gt, controls);
            paneButtonSix.Update(gt, controls);

            paneButtonOne.Position = new Point(paneRect.X + 2, paneRect.Y + offsetY);
            paneButtonTwo.Position = new Point(paneRect.X + 2, paneRect.Y + offsetY + (paneButton.Height + 3) * 1);
            paneButtonThree.Position = new Point(paneRect.X + 2, paneRect.Y + offsetY + (paneButton.Height + 3) * 2);
            paneButtonFour.Position = new Point(paneRect.X + 2, paneRect.Y + offsetY + (paneButton.Height + 3) * 3);

            paneButtonFive.Position = new Point(paneRect.X + 185, paneRect.Y + offsetY);
            paneButtonSix.Position = new Point(paneRect.X + 185, paneRect.Y + offsetY + (paneButton.Height + 3));

            disposeItem.Update(gt, controls);
            disposeItem.Position = new Point((int)screenPosition.X + 16, (int)screenPosition.Y + (tabBG.Height - iconBG.Height) - 6);

            //Button clicking here!

            if (selectedItem != null)
            {
                if (currentEntity.SUSPENSION_Action == Performance.Suspension.SuspendState.None)
                {
                    if (paneButtonOne.IsLeftClicked == true && !string.IsNullOrEmpty(selectedItem.ButtonOneText))
                    {
                        selectedItem.ButtonOne();
                        screens.PlaySound("Button Click 6");
                    }
                    if (paneButtonTwo.IsLeftClicked == true && !string.IsNullOrEmpty(selectedItem.ButtonTwoText))
                    {
                        selectedItem.ButtonTwo();
                        screens.PlaySound("Button Click 6");
                    }
                    if (paneButtonThree.IsLeftClicked == true && !string.IsNullOrEmpty(selectedItem.ButtonThreeText))
                    {
                        selectedItem.ButtonThree();
                        screens.PlaySound("Button Click 6");
                    }
                    if (paneButtonFour.IsLeftClicked == true && !string.IsNullOrEmpty(selectedItem.ButtonFourText))
                    {
                        selectedItem.ButtonFour();
                        screens.PlaySound("Button Click 6");
                    }
                    if (paneButtonFive.IsLeftClicked == true && !string.IsNullOrEmpty(selectedItem.ButtonFiveText))
                    {
                        selectedItem.ButtonFive();
                        screens.PlaySound("Button Click 6");
                    }
                    if (paneButtonSix.IsLeftClicked == true && !string.IsNullOrEmpty(selectedItem.ButtonSixText))
                    {
                        selectedItem.ButtonSix();
                        screens.PlaySound("Button Click 6");
                    }
                }
            }
        }

        private BaseItem selectedItem, draggedItem, disposalItem;
        public bool IsDraggingItem() { return draggedItem != null; }
        private void CheckItemSelect(BaseItem i)
        {
            if (i.itemRect.Contains(controls.MousePosition))
            {
                if (inventory.transTabCombining == null)
                {
                    if (GameSettings.IsDebugging == false)
                        ToolTip.RequestStringAssign(i.ToolTipText(equipment));
                    else
                        ToolTip.RequestStringAssign(i.Name + "(" + i.ID + " -- " + i.UniqueID + ")" +
                                                    "\nType: " + i.ItemType +
                                                    "\nSubType: " + i.ItemSubType +
                                                    "\n" + i.CurrentAmount.ToString() + " / " + i.MaxAmount);

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        selectedItem = i;
                        i.isSelected = true;

                        //Assign text blocks.
                        descLines = selectedItem.Description.WrapFormatText(font, Color.White, paneBG.Width - 20);

                        selectedItem.RefreshAttributeText();
                        if (!string.IsNullOrEmpty(selectedItem.AttributeText))
                            attrLines = selectedItem.AttributeText.WrapFormatText(font, Color.White, paneBG.Width - 20);

                        //Assign scrolling boundaries for both blocks of text.
                        if (descLines.Count > 0)
                            descLineBounds = (int)descLines.LastOrDefault().Position.Y + descLines.LastOrDefault().Font.LineSpacing;
                        if (attrLines.Count > 0)
                            attrLineBounds = (int)attrLines.LastOrDefault().Position.Y + attrLines.LastOrDefault().Font.LineSpacing;

                        //D-do the jingle!
                        screens.PlaySound("Item Select");
                    }
                }
                if (inventory.transTabCombining == null && draggedItem == null)
                {
                    if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                        inventory.transTabCombining = i;
                }
            }
            else if (!i.itemRect.Contains(controls.MousePosition) && (!paneRect.Contains(controls.MousePosition) && !dragScreenRect.Contains(controls.MousePosition)))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    i.isSelected = false;
            }
        }
        private float increaseValue = 0f;
        private void ChangePaneWidth(GameTime gt, bool increase)
        {
            if (increase == true)
                increaseValue += 3f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                increaseValue -= 3f * (float)gt.ElapsedGameTime.TotalSeconds;

            increaseValue = MathHelper.Clamp(increaseValue, 0f, 1f);

            currentPaneWidth = MathHelper.SmoothStep(-4f, paneBG.Width, increaseValue);
        }

        private float draggedAngle, angleClamp; private bool isDraggedIncreasing = false;
        private void UpdateDragging(GameTime gt)
        {
            if (angleClamp >= 1f)
                isDraggedIncreasing = false;
            else if (angleClamp <= 0f)
                isDraggedIncreasing = true;

            if (isDraggedIncreasing == true)
                angleClamp += .15f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                angleClamp -= .15f * (float)gt.ElapsedGameTime.TotalSeconds;

            angleClamp = MathHelper.Clamp(angleClamp, 0f, 1f);
            draggedAngle = MathHelper.SmoothStep(-.3f, .3f, angleClamp);

            dragSwoosh.SetVariables(controls.MouseVector, 1f);
            dragSwoosh.Update(gt);
        }

        private void CheckItemSwapping(int draggedItemIndex, int swapIndex)
        {
            if (itemData[swapIndex].itemRect.Contains(controls.MousePosition))
            {
                if (itemData[swapIndex].ID != draggedItem.ID)
                    ToolTip.RequestStringAssign("Release: Swap item" + Environment.NewLine + "Right-click: Use with item");
                else
                    ToolTip.RequestStringAssign("Release: Merge stacks");

                if (!draggedItem.Equals(itemData[swapIndex]))
                {
                    if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    {
                        if (itemData[swapIndex].ID == draggedItem.ID && itemData[swapIndex].CurrentAmount < itemData[swapIndex].MaxAmount)
                        {
                            int roomFor = itemData[swapIndex].MaxAmount - itemData[swapIndex].CurrentAmount; //How many more items can be merged until the stack is full
                            int transferQuantity = (int)Math.Min(draggedItem.CurrentAmount, roomFor);

                            draggedItem.CurrentAmount -= transferQuantity;
                            itemData[swapIndex].CurrentAmount += transferQuantity;
                        }
                        else //if (itemData[swapIndex].ID != draggedItem.ID)
                        {
                            itemData.Swap(draggedItemIndex, swapIndex);
                            screens.PlaySound("SwapItem");
                        }
                    }
                }

                CheckItemCombining(swapIndex);
            }
        }
        private void CheckItemCombining(int itemIndex)
        {
            if (controls.IsClickedOnce(Controls.MouseButton.RightClick) && currentEntity.SUSPENSION_Action == Performance.Suspension.SuspendState.None)
            {
                draggedItem.CombineItem(itemData[itemIndex]);

                if (draggedItem.doesCombineHaveResults == false) //If the first way didn't work ...
                    itemData[itemIndex].CombineItem(draggedItem); //Try switching the items to check the other way around!

                if (itemData[itemIndex].doesCombineHaveResults == false && draggedItem.doesCombineHaveResults == false)
                {
                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Couldn't combine items");
                    screens.PlaySound("Invalid");
                }
            }
        }

        private bool IsHoveringOnItem()
        {
            if (inventory.transTabCombining != null)
            {
                for (int i = 0; i < itemData.Count; i++)
                {
                    if (itemData[i].itemRect.Contains(controls.MousePosition) && itemData[i].ID != inventory.transTabCombining.ID)
                        return true; //If ANY item is being hovered on, return true;
                }
            }

            return false;
        }
        private void CheckItemCombiningTransTab(int itemIndex)
        {
            if (inventory.transTabCombining != null && currentEntity.SUSPENSION_Action == Performance.Suspension.SuspendState.None) //If transTabCombining is not null
            {
                if (inventory.transTabCombining.ID != itemData[itemIndex].ID) //and the two items aren't the same!
                {
                    if (itemData[itemIndex].itemRect.Contains(controls.MousePosition)) //And the mouse is over an item
                    {
                        ToolTip.RequestStringAssign("Left-click: Use " + inventory.transTabCombining.Name + " with " + itemData[itemIndex].Name + Environment.NewLine + "Right-click: Cancel or select new item");

                        if (controls.IsClickedOnce(Controls.MouseButton.LeftClick)) //and if the mouse is right clicking
                        {
                            inventory.transTabCombining.CombineItem(itemData[itemIndex]);

                            if (inventory.transTabCombining.doesCombineHaveResults == false) //If the first way didn't work ...
                                itemData[itemIndex].CombineItem(inventory.transTabCombining); //Try switching the items to check the other way around!

                            if (itemData[itemIndex].doesCombineHaveResults == false && inventory.transTabCombining.doesCombineHaveResults == false) //If both ways are false, inform player!
                            {
                                screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Couldn't combine items");
                                screens.PlaySound("Invalid");
                            }
                        }
                    }
                }
            }
        }
        private void CheckDisposeItem()
        {
            if (draggedItem.IsEssential == false)
            {
                if (disposeItem.ButtonRectangle.Contains(controls.MousePosition))
                {
                    if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        disposalItem = draggedItem;
                }
            }
        }
        private void CheckItemGifting()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].IsPlayerControlled == false)
                {
                    Vector2 mouseToWorld = camera.ScreenToWorld(controls.MouseVector);
                    if (entities[i].EntityCircle.Contains(mouseToWorld))
                    {
                        if (draggedItem.IsEssential == true && draggedItem.IsEntityItem == true)
                            ToolTip.RequestStringAssign("Essential or protected items cannot be given away!");
                        else if (draggedItem.IsEntityItem == true)
                            ToolTip.RequestStringAssign("Protected items cannot be given away!");
                        else if (draggedItem.IsEssential == true)
                            ToolTip.RequestStringAssign("Essential items cannot be given away!");

                        if (inventory.IsClickingUI() == false)
                        {
                            if (entities[i].IsDead == false && draggedItem.IsEssential == false && draggedItem.IsEntityItem == false)
                            {
                                //if (!entities[i].IsCompanionLeader(currentEntity))
                                //{
                                    int rightClickValue = 10;

                                    if (draggedItem.CurrentAmount > 1000)
                                        rightClickValue = 100;
                                    if (draggedItem.CurrentAmount > 10000)
                                        rightClickValue = 500;
                                    if (draggedItem.CurrentAmount > 100000)
                                        rightClickValue = 1000;

                                    ToolTip.RequestStringAssign("Release: Gift One" + Environment.NewLine + "Right-click: Gift " + rightClickValue.CommaSeparation() + Environment.NewLine + "Shift + Release: Gift All");

                                    if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                                    {
                                        if (controls.CurrentKey.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || controls.CurrentKey.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                                        {
                                            entities[i].STORAGE_Gift(currentEntity, draggedItem.ID, draggedItem.CurrentAmount);
                                            equipment.RemoveEquip(draggedItem.ID);
                                            itemData.Remove(draggedItem);
                                            draggedItem = null;

                                            screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                                        }
                                        else
                                        {
                                            entities[i].STORAGE_Gift(currentEntity, draggedItem.ID, 1);
                                            draggedItem.CurrentAmount -= 1;

                                            if (draggedItem.CurrentAmount < 1)
                                            {
                                                equipment.RemoveEquip(draggedItem.ID);
                                                draggedItem = null;
                                            }

                                            screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                                        }
                                    }
                                    if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                                    {
                                        int quantity = (int)MathHelper.Clamp(draggedItem.CurrentAmount, 0, rightClickValue);

                                        entities[i].STORAGE_Gift(currentEntity, draggedItem.ID, quantity);
                                        draggedItem.CurrentAmount -= quantity;

                                        if (draggedItem.CurrentAmount < 1)
                                        {
                                            equipment.RemoveEquip(draggedItem.ID);
                                            draggedItem = null;
                                        }

                                        screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                                    }
                                /*}
                                else
                                {
                                    ToolTip.RequestStringAssign("Release: Use Item" + Environment.NewLine + "Right-click: Give Item");

                                    if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                                    {
                                        entities[i].STORAGE_AddItem(draggedItem.ID, 1);
                                        entities[i].STORAGE_UseItemButton(draggedItem.ID, draggedItem.buttonTarget);
                                    }
                                    if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                                    {
                                        entities[i].STORAGE_Gift(currentEntity, draggedItem.ID, 1);
                                        draggedItem.CurrentAmount -= 1;

                                        if (draggedItem.CurrentAmount < 1)
                                        {
                                            equipment.RemoveEquip(draggedItem.ID);
                                            draggedItem = null;
                                        }

                                        screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                                    }
                                }*/
                            }
                        }
                    }
                }
            }
        }
        private void CheckBarterAdding()
        {
            if (draggedItem != null) //if the dragged item is not null ...
            {
                if (screens.BARTERING_IsClickingUI()) //and the mouse is in the barter interface ...
                {
                    if (draggedItem.IsEssential == false && draggedItem.SellWorth >= 0)
                    {
                        ToolTip.RequestStringAssign("Release: Add All" + Environment.NewLine + "Right-click: Add One");
                        if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            if (screens.BARTERING_IsTraderWindowFull() == false)
                            {
                                int subtraction = 0;
                                screens.BARTERING_AddTraderItem(draggedItem.ID, draggedItem.CurrentAmount, out subtraction);
                                draggedItem.CurrentAmount -= subtraction;

                                if (subtraction != 0)
                                    screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                            }
                        }
                        if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                        {
                            int quantity = 1;
                            if (draggedItem.CurrentAmount > 999)
                                quantity = 10;
                            if (draggedItem.CurrentAmount > 9999)
                                quantity = 100;
                            if (draggedItem.CurrentAmount > 99999)
                                quantity = 1000;

                            int subtraction = 0;
                            screens.BARTERING_AddTraderItem(draggedItem.ID, quantity, out subtraction);
                            draggedItem.CurrentAmount -= subtraction;

                            if (draggedItem.CurrentAmount < 1)
                            {
                                equipment.RemoveEquip(draggedItem.ID);
                                draggedItem = null;
                            }
                            if (subtraction != 0)
                                screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                        }
                    }
                    else
                        ToolTip.RequestStringAssign("This item cannot be traded");
                }
            }
        }
        private void CheckStoneholdAdding()
        {
            if (draggedItem != null) //if the dragged item is not null ...
            {
                if (screens.STONEHOLD_IsTabHover()) //and the mouse is in the barter interface ...
                {
                    if (draggedItem.IsEssential == false && draggedItem.IsEntityItem == false) //Prevent depositing other people's items through mind control.
                    {
                        ToolTip.RequestStringAssign("Release: Add All");
                        if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            if (screens.BARTERING_IsTraderWindowFull() == false)
                            {
                                if (draggedItem.IsMultiStack == true)
                                {
                                    screens.STONEHOLD_DepositItem(draggedItem);
                                    DropDraggedItem();
                                }
                                else
                                {
                                    int quantityRemoved = screens.STONEHOLD_AddItemReturnDifference(draggedItem.ID, draggedItem.CurrentAmount);
                                    draggedItem.CurrentAmount -= quantityRemoved;

                                    if (draggedItem.CurrentAmount <= 0)
                                        DropDraggedItem();
                                }

                                screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                            }
                        }
                    }
                    else
                        ToolTip.RequestStringAssign("This item cannot be stored here!");
                }
            }
        }
        private void CheckPurchaseQuickselling()
        {
            if (draggedItem != null) //if the dragged item is not null ...
            {
                if (screens.PURCHASE_IsClickingUI() == true)
                {
                    if (draggedItem.ID != 1) //if the player is not trying to sell money ...
                    {
                        if (draggedItem.IsEssential == false && draggedItem.IsEntityItem == false && draggedItem.SellWorth >= 0)
                        {
                            ToolTip.RequestStringAssign("Release: Sell All\nRight-click: Sell One");

                            if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                            {
                                screens.PURCHASING_SellItem(draggedItem, draggedItem.CurrentAmount);
                                screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                            }
                            if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                            {
                                screens.PURCHASING_SellItem(draggedItem, 1);

                                if (draggedItem.CurrentAmount < 1)
                                {
                                    equipment.RemoveEquip(draggedItem.ID);
                                    draggedItem = null;
                                }

                                screens.PlayRandom("GiftItem1", "GiftItem2", "GiftItem3");
                            }
                        }
                        else
                            ToolTip.RequestStringAssign("You can't sell this item.");
                    }
                    else
                        ToolTip.RequestStringAssign("You can't sell money.");
                }
            }
        }
        private void CheckSmelterAdding()
        {
            if (draggedItem != null) //if the dragged item is not null ...
            {
                if (screens.SMELTER_IsActive)
                {
                    if (draggedItem.IsEssential == false && draggedItem.IsEntityItem == false) //Prevent depositing other people's items through mind control.
                    {
                        if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            //Add item to ore slot
                            if (screens.SMELTER_IsOreSlotHover())
                            {
                                screens.SMELTER_SetOre(draggedItem);
                                screens.PlaySound("SmelterAddOre");

                                DropDraggedItem();
                            }

                            //Added item to modifier slot
                            if (screens.SMELTER_IsModifierSlotHover())
                            {
                                screens.SMELTER_SetModifier(draggedItem);

                                DropDraggedItem();
                            }

                            //Add item to fuel slot
                            if (screens.SMELTER_IsFuelSlotHover())
                            {
                                //Ensure it's a valid fuel!
                                if (screens.SMELTER_IsValidFuel(draggedItem.ID))
                                {
                                    screens.SMELTER_SetFuel(draggedItem);

                                    DropDraggedItem();
                                }
                                else
                                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "This isn't fuel!");
                            }
                        }
                    }
                }
            }
        }
        private void CheckBrewingAdding()
        {
            if (draggedItem != null)
            {
                if (screens.BREWING_IsActive)
                {
                    if (draggedItem.IsEntityItem == false) //Prevents depositing other people's items through mind control.
                    {
                        if (screens.BREWING_IsPrimaryHover())
                        {
                            ToolTip.RequestStringAssign("Release: Add to primary ingredient slot");

                            if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                            {
                                screens.BREWING_SetPrimary(draggedItem);
                                DropDraggedItem();
                            }
                        }

                        if (screens.BREWING_IsSecondaryHover())
                        {
                            ToolTip.RequestStringAssign("Release: Add to secondary ingredient slot");

                            if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                            {
                                screens.BREWING_SetSecondary(draggedItem);
                                DropDraggedItem();
                            }
                        }

                        if (screens.BREWING_IsCombinerHover())
                        {
                            ToolTip.RequestStringAssign("Release: Add to bottle slot");

                            if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                            {
                                if (draggedItem.ItemType.ToUpper().Equals("CONTAINER"))
                                {
                                    screens.BREWING_SetCombiner(draggedItem);
                                    DropDraggedItem();
                                }
                                else
                                    screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "This can't be used!");
                            }
                        }
                    }
                }
            }
        }
        private void DropDraggedItem()
        {
            equipment.RemoveEquip(draggedItem.ID);
            storage.RemoveItem(draggedItem);

            draggedItem = null;
        }

        public void AlwaysUpdate(GameTime gt, Controls passedControls)
        {
            CheckEquimentControls(gt, passedControls);
        }
        private void CheckEquimentControls(GameTime gt, Controls passedControls)
        {
            if (GameSettings.IsHidingHUD == false && screens.EFFECTS_IsWidescreen() == false)
            {
                for (int i = 0; i < equipment.QuickSlots.Count; i++)
                {
                    if (equipment.QuickSlots[i].slotRect.Contains(passedControls.MousePosition))
                    {
                        if (equipment.QuickSlots[i].item != null)
                            ToolTip.RequestStringAssign(equipment.QuickSlots[i].item.Name);

                        if (passedControls.IsClickedOnce(Controls.MouseButton.LeftClick))
                            equipment.SelectQuickSlot(i);

                        if (passedControls.IsClickedOnce(Controls.MouseButton.RightClick) && inventory.IsActive == true)
                        {
                            equipment.QuickSlots[i].item = null;
                            equipment.QuickSlots[i].ID = -1;
                            equipment.QuickSlots[i].Icon = null;
                        }

                        if (draggedItem != null && passedControls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            equipment.EquipQuickSlot(i, draggedItem);
                            screens.PlaySound("EquipItem");
                        }
                    }
                }
            }
        }

        private void CheckWeaponControls()
        {
            CheckWeaponEquipDrag(equipment.WeaponOne, 1, "First Primary Weapon", true);
            CheckWeaponEquipDrag(equipment.WeaponTwo, 2, "Second Primary Weapon", true);
            CheckWeaponEquipDrag(equipment.WeaponThree, 3, "Third Primary Weapon", true);

            CheckWeaponEquipDrag(equipment.OffhandOne, 4, "First Offhand Weapon", false);
            CheckWeaponEquipDrag(equipment.OffhandTwo, 5, "Second Offhand Weapon", false);
            CheckWeaponEquipDrag(equipment.OffhandThree, 6, "Third Offhand Weapon", false);
        }
        private void CheckWeaponEquipDrag(EquipSlot slot, int index, string slotName, bool isPrimary)
        {
            if (slot.slotRect.Contains(controls.MousePosition))
            {
                if (slot.item != null)
                    ToolTip.RequestStringAssign(slot.item.Name);
                else
                    ToolTip.RequestStringAssign(slotName);

                if (draggedItem != null && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    equipment.EquipWeapon(index, (Weapon)draggedItem);
                    screens.PlaySound("EquipItem");
                }

                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    slot.item = null;

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    if (isPrimary)
                        currentEntity.EQUIPMENT_SwitchPrimary((EntityEquipment.WeaponSlot)index - 1);
                    else
                        currentEntity.EQUIPMENT_SwitchOffhand((EntityEquipment.WeaponSlot)index - 4);
                }
            }

            if (slot.deleteRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    slot.item = null;
                }
            }

            if (slot.item != null)
            {
                if (slot.item.CurrentAmount <= 0)
                    slot.item = null;
            }
        }

        private void CheckArmorControls()
        {
            CheckArmor(equipment.HeadSlot, "Head");
            CheckArmor(equipment.TorsoSlot, "Torso");
            CheckArmor(equipment.LegsSlot, "Legs");
            CheckArmor(equipment.FeetSlot, "Feet");
            CheckArmor(equipment.HandsSlot, "Hands");
            CheckArmor(equipment.CapeSlot, "Cape");
        }
        private void CheckArmor(EquipSlot slot, string slotName)
        {
            if (slot.slotRect.Contains(controls.MousePosition))
            {
                if (slot.item != null)
                    ToolTip.RequestStringAssign(slot.item.Name);
                else
                    ToolTip.RequestStringAssign(slotName);

                if (draggedItem != null && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    equipment.EquipArmor((Armor)draggedItem);
                    screens.PlaySound("EquipItem");
                }

                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    slot.item = null;
            }

            if (slot.deleteRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    slot.item = null;
                }
            }

            if (slot.item != null)
            {
                if (slot.item.CurrentAmount <= 0)
                    slot.item = null;
            }
        }

        private void CheckAmmoControls()
        {
            CheckAmmo(equipment.PrimaryAmmo1, 1, "Primary 1");
            CheckAmmo(equipment.PrimaryAmmo2, 2, "Primary 2");
            CheckAmmo(equipment.SecondaryAmmo1, 3, "Secondary 1");
            CheckAmmo(equipment.SecondaryAmmo2, 4, "Secondary 2");
        }
        private void CheckAmmo(EquipSlot slot, int index, string slotName)
        {
            if (slot.slotRect.Contains(controls.MousePosition))
            {
                if (slot.item != null)
                    ToolTip.RequestStringAssign(slot.item.Name);
                else
                    ToolTip.RequestStringAssign(slotName);

                if (draggedItem != null && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    equipment.EquipAmmo(index, (Ammo)draggedItem);
                    screens.PlaySound("EquipItem");
                }

                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    slot.item = null;
            }

            if (slot.deleteRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    slot.item = null;
            }
            if (slot.item != null)
            {
                if (slot.item.CurrentAmount <= 0)
                    slot.item = null;
            }
        }

        private void CheckJewelleryControls()
        {
            CheckJewellery(equipment.Ring1Slot, 1, "Right-hand Ring");
            CheckJewellery(equipment.Ring2Slot, 2, "Right-hand Ring");
            CheckJewellery(equipment.Ring3Slot, 3, "Left-hand Ring");
            CheckJewellery(equipment.Ring4Slot, 4, "Left-hand Ring");
            CheckJewellery(equipment.NeckSlot, 5, "Amulet");
        }
        private void CheckJewellery(EquipSlot slot, int index, string slotName)
        {
            if (slot.slotRect.Contains(controls.MousePosition))
            {
                if (slot.item != null)
                    ToolTip.RequestStringAssign(slot.item.Name);
                else
                    ToolTip.RequestStringAssign(slotName);

                if (draggedItem != null && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    equipment.EquipJewellery(index, (Jewellery)draggedItem);
                    screens.PlaySound("EquipItem");
                }

                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    slot.item = null;
            }

            if (slot.deleteRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    slot.item = null;
            }
            if (slot.item != null)
            {
                if (slot.item.CurrentAmount <= 0)
                    slot.item = null;
            }
        }

        private Texture2D tabShadow, paneShadow, paneEndShadow;
        public void CreateShadows(SpriteBatch sb, float shadowIntensity)
        {
            tabShadow = tabBG.CreateShadow(sb, shadowIntensity);
            paneShadow = paneBG.CreateShadow(sb, shadowIntensity);
            paneEndShadow = paneEnd.CreateShadow(sb, shadowIntensity);
        }
        public void UnloadShadows()
        {
            tabShadow.Dispose();
            paneShadow.Dispose();
        }
        public void DrawShadows(SpriteBatch sb, Vector2 offset)
        {
            panePosition = screenPosition + new Vector2(tabBG.Width - 10, 40);

            sb.Draw(tabShadow, screenPosition + offset, Color.White);
            sb.Draw(paneShadow, new Rectangle((int)panePosition.X + (int)offset.X, (int)panePosition.Y + (int)offset.Y, paneRect.Width - (int)offset.X - 2, paneRect.Height), Color.White);
            sb.Draw(paneEndShadow, new Vector2((panePosition.X + paneRect.Width) - 2, panePosition.Y + offset.Y + 1), Color.White);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();

            DrawPane(sb);

            sb.Draw(tabBG, screenPosition, Color.White);

            disposeItem.DrawButton(sb, Color.White);

            if (disposalItem != null)
                sb.Draw(disposalItem.Icon, disposeItem.Center, Color.White, disposalItem.Icon.Center(), 0f, 1f);

            sb.Draw(disposeOverlay, disposeItem.Position.ToVector2(), Color.White);

            sb.End();

            DrawScissored(sb);
        }
        public void DrawDrag(SpriteBatch sb)
        {
            sb.Begin();

            if (draggedItem != null)
            {
                //dragSwoosh.Draw(sb);
                sb.Draw(draggedItem.Icon, controls.MouseVector, Color.White, draggedItem.Icon.Center(), draggedAngle, 1f);
                //sb.DrawStringBordered(largeFont, draggedItem.CurrentAmount.ToString(), controls.MouseVector - draggedItem.Icon.Center(), Color.White, Color.Black);
            }

            sb.End();
        }
        private Vector2 panePosition;
        private void DrawPane(SpriteBatch sb)
        {
            sb.Draw(paneBG, panePosition, new Rectangle(0, 0, (int)currentPaneWidth, paneBG.Height), Color.White);
            sb.Draw(paneEnd, panePosition + new Vector2(currentPaneWidth, 1), Color.White);
        }

        private Rectangle scissorGrid, scissorPane, scissorDesc, scissorAttr;
        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        private Color noRequirements = Color.Lerp(Color.Lerp(Color.White, Color.Red, .5f), Color.Transparent, .5f);

        public override void DrawScissored(SpriteBatch sb)
        {
            DrawScissoredGrid(sb);
            DrawScissoredPane(sb);
            DrawScissoredDescription(sb);
            DrawScissoredAttributes(sb);
        }

        private void DrawScissoredGrid(SpriteBatch sb)
        {
            scissorGrid = new Rectangle((int)screenPosition.X + 15, (int)screenPosition.Y + 71, 466, 379);

            DrawInside(sb, scissorGrid, () =>
            {
                for (int i = 0; i < itemData.Count; i++)
                {
                    if (itemData[i].isSelected == true)
                        sb.Draw(iconBGSelected, new Vector2(itemData[i].itemRect.X, itemData[i].itemRect.Y), new Rectangle(0, 0, 64, 64), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, .95f);
                    else
                        sb.Draw(iconBG, new Vector2(itemData[i].itemRect.X, itemData[i].itemRect.Y), new Rectangle(0, 0, 64, 64), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, .95f);

                    if (currentEntity.SUSPENSION_Action == Performance.Suspension.SuspendState.Suspended)
                        sb.Draw(iconBG, new Vector2(itemData[i].itemRect.X, itemData[i].itemRect.Y), new Rectangle(0, 0, 64, 64), Color.Gray, 0, Vector2.Zero, 1f, SpriteEffects.None, .95f);


                    if (itemData[i].MeetsRequirements() && itemData[i].CurrentDurability > 0)
                        sb.Draw(itemData[i].Icon, new Vector2(itemData[i].itemRect.X, itemData[i].itemRect.Y), new Rectangle(0, 0, itemData[i].Icon.Width, itemData[i].Icon.Height), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, .95f);
                    else
                        sb.Draw(itemData[i].Icon, new Vector2(itemData[i].itemRect.X, itemData[i].itemRect.Y), new Rectangle(0, 0, itemData[i].Icon.Width, itemData[i].Icon.Height), noRequirements, 0, Vector2.Zero, 1f, SpriteEffects.None, .95f);

                    if (itemData[i].IsNew == true)
                        sb.Draw(newIcon, new Vector2(itemData[i].itemRect.X + 31, itemData[i].itemRect.Y + 51), new Rectangle(currentFrame.X * 9, currentFrame.Y * 9, 9, 9), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    else
                        sb.Draw(blankIcon, new Vector2(itemData[i].itemRect.X + 31, itemData[i].itemRect.Y + 51), new Color(.25f, .25f, .25f, .5f));

                    if (itemData[i].IsEssential == true)
                        sb.Draw(essentialIcon, new Vector2(itemData[i].itemRect.X + 51, itemData[i].itemRect.Y + 51), Color.White);
                    else
                        sb.Draw(blankIcon, new Vector2(itemData[i].itemRect.X + 51, itemData[i].itemRect.Y + 51), new Color(.25f, .25f, .25f, .5f));

                    if (itemData[i].IsEntityItem == true)
                        sb.Draw(entityIcon, new Vector2(itemData[i].itemRect.X + 41, itemData[i].itemRect.Y + 51), Color.White);
                    else
                        sb.Draw(blankIcon, new Vector2(itemData[i].itemRect.X + 41, itemData[i].itemRect.Y + 51), new Color(.25f, .25f, .25f, .5f));

                    if (itemData[i].MaxAmount >= 1 && itemData[i].CurrentAmount > 1) //If both max and current amount are greater than 1, draw current amount on item
                    {
                        if (itemData[i].CurrentAmount > 999)
                        {
                            float itemAmount = (float)itemData[i].CurrentAmount * .001f;
                            Color numberColor = Color.LightGreen;

                            if (itemData[i].CurrentAmount > 100000)
                                numberColor = Color.Lerp(Color.LightGreen, Color.Green, .5f);

                            sb.DrawString(font, string.Format("{0:0.#}" + "K", itemAmount), new Vector2(itemData[i].itemRect.X + 4, itemData[i].itemRect.Y + 1), numberColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                        }
                        else
                            sb.DrawString(font, itemData[i].CurrentAmount.ToString(), new Vector2(itemData[i].itemRect.X + 4, itemData[i].itemRect.Y + 1), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    }
                    //barFiller.Width * entity.Skills.health.CurrentHP / entity.Skills.health.MaxHP

                    if (itemData[i].CurrentDurability < itemData[i].MaxDurability && itemData[i].CurrentDurability > 0)
                    {
                        int test = 56 * itemData[i].CurrentDurability / itemData[i].MaxDurability;

                        sb.DrawBoxBordered(pixel, new Rectangle(itemData[i].itemRect.X + 4,
                                                                itemData[i].itemRect.Y + itemData[i].itemRect.Height - 5, test,
                                                                1), Color.LightGray, Color.Black, 1f);
                    }
                }
            });
        }
        private void DrawScissoredPane(SpriteBatch sb)
        {
            scissorPane = new Rectangle((int)screenPosition.X + (tabBG.Width) - 10, (int)screenPosition.Y + 40, (int)currentPaneWidth + 4, paneBG.Height);

            DrawInside(sb, scissorPane, () =>
            {
                sb.Draw(iconBG, new Rectangle(scissorPane.X + 1, scissorPane.Y + 9, iconBG.Width, iconBG.Height), Color.White);

                if (selectedItem != null)
                {
                    sb.Draw(selectedItem.Icon, new Rectangle(scissorPane.X + 1, scissorPane.Y + 9, selectedItem.Icon.Width, selectedItem.Icon.Height), Color.White);
                    sb.DrawString(largeFont, selectedItem.Name, new Vector2(scissorPane.X + (paneBG.Width / 2) + 25, scissorPane.Y + 31), ColorHelper.UI_Gold, 0f, selectedItem.Name.LineCenter(largeFont), 1f, SpriteEffects.None, 1f);

                    if (!string.IsNullOrEmpty(selectedItem.TypeWords))
                        sb.DrawString(largeFont, selectedItem.TypeWords, new Vector2(scissorPane.X + (paneBG.Width) - 10, scissorPane.Y + 72), largeFont.MeasureString(selectedItem.TypeWords), Color.White, 1f);

                    sb.DrawStringShadow(font, selectedItem.WorthWords, new Vector2(scissorPane.X + (paneBG.Width / 2), scissorPane.Y + 352),
                                        selectedItem.WorthWords.LineCenter(font), 0f, 1f, 1f, ColorHelper.UI_Gold, Color.Black);

                    DrawPaneButton(sb, paneButtonOne, selectedItem.ButtonOneText);
                    DrawPaneButton(sb, paneButtonTwo, selectedItem.ButtonTwoText);
                    DrawPaneButton(sb, paneButtonThree, selectedItem.ButtonThreeText);
                    DrawPaneButton(sb, paneButtonFour, selectedItem.ButtonFourText);
                    DrawPaneButton(sb, paneButtonFive, selectedItem.ButtonFiveText);
                    DrawPaneButton(sb, paneButtonSix, selectedItem.ButtonSixText);
                }
            });
        }

        private List<TextBlock> descLines = new List<TextBlock>(), attrLines = new List<TextBlock>();
        private void DrawScissoredDescription(SpriteBatch sb)
        {
            scissorDesc = new Rectangle((int)screenPosition.X + (tabBG.Width) - 10, (int)screenPosition.Y + 123, (int)currentPaneWidth + 4, 178);

            DrawInside(sb, scissorDesc, () =>
            {
                if (selectedItem != null)
                {
                    for (int i = 0; i < descLines.Count; i++)
                        sb.DrawString(descLines[i].Font, descLines[i].Text, descLines[i].Position + new Vector2(scissorPane.X + 10, scissorPane.Y + 86 + descY), descLines[i].Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            });
        }
        private void DrawScissoredAttributes(SpriteBatch sb)
        {
            scissorAttr = new Rectangle((int)screenPosition.X + (tabBG.Width) - 10, (int)screenPosition.Y + 305, (int)currentPaneWidth + 4, 73);

            DrawInside(sb, scissorAttr, () =>
            {
                if (selectedItem != null)
                {
                    for (int i = 0; i < attrLines.Count; i++)
                        sb.DrawString(attrLines[i].Font, attrLines[i].Text, attrLines[i].Position + new Vector2(scissorPane.X + 10, scissorPane.Y + 268 + attrY), attrLines[i].Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            });
        }

        private Color halfTransparent = Color.Lerp(Color.Transparent, Color.White, .5f);
        private void DrawPaneButton(SpriteBatch sb, MenuButton button, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                button.DrawButton(sb, Color.White);
                sb.DrawString(font, text, button.Center, Color.White, 0f, text.LineCenter(font), 1f, SpriteEffects.None, 1f);
            }
            else
                button.DrawButtonIdle(sb, halfTransparent);
        }
        private void DrawQuickButton(SpriteBatch sb, MenuButton button, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                button.DrawButton(sb, Color.White);
            }
            else
                button.DrawButtonIdle(sb, halfTransparent);
        }

        private EntityStorage storage; private EntityEquipment equipment; private BaseEntity currentEntity;
        private List<BaseEntity> entities = new List<BaseEntity>();
        public void SetItemData(BaseEntity currentEntity, EntityStorage storage, EntityEquipment equipment)
        {
            this.currentEntity = currentEntity;

            if (selectedItem != null)
                selectedItem.isSelected = false;
            
            itemData = storage.GetItemsTab(tabType);

            this.storage = storage;
            this.equipment = equipment;

            selectedItem = null;
            isItemSelected = false;
        }
        public void SetEntityData(List<BaseEntity> allEntities) { entities = allEntities; }

        public bool IsPaneClicking() { return paneRect.Contains(controls.MousePosition); }
    }
}
