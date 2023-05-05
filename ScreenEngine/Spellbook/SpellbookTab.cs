using System.Collections.Generic;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Extensions;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;

namespace Pilgrimage_Of_Embers.ScreenEngine.Spellbook
{
    public class SpellbookTab
    {
        private List<BaseSpell> currentSpells = new List<BaseSpell>();

        private Vector2 screenPosition;
        private Rectangle paneRect;
        private float scrollPosition;

        private Texture2D iconBG, iconBGSelect, paneButton, paneButtonHover, newIcon;
        private SpriteFont regFont, font;

        private BasicAnimation animation = new BasicAnimation();
        private Point currentFrame = Point.Zero;

        private Controls controls = new Controls();

        private Point gridSize = new Point(7, 5);

        private ScreenManager screens;
        private BaseEntity controlledEntity;
        private EntityStorage storage;
        private EntityEquipment equipment;

        private BaseSpell.TabType type = BaseSpell.TabType.Destructive;

        protected MenuButton paneButtonOne, paneButtonTwo, paneButtonThree, paneButtonFour;

        public SpellbookTab(BaseSpell.TabType Type)
        {
            type = Type;
        }

        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;
        }
        public void SetControlledEntity(BaseEntity controlledEntity, EntityStorage storage, EntityEquipment equipment)
        {
            this.controlledEntity = controlledEntity;
            this.storage = storage;
            this.equipment = equipment;
        }

        private bool isSpellSelected;
        public bool IsSpellSelected { get { return isSpellSelected; } }

        private BaseSpell selectedSpell;
        public BaseSpell SelectedSpell { get { return selectedSpell; } }

        public void SetScreenPosition(Vector2 position, Rectangle paneRect) { screenPosition = position; this.paneRect = paneRect; }

        private const string directory = "Interface/Spellbook/";
        public void Load(ContentManager cm)
        {
            iconBG = cm.Load<Texture2D>("Interface/Global/iconBG");
            iconBGSelect = cm.Load<Texture2D>("Interface/Global/iconBGSelect");
            paneButton = cm.Load<Texture2D>("Interface/Spellbook/paneExtendedButton");
            paneButtonHover = cm.Load<Texture2D>("Interface/Spellbook/paneExtendedButtonHover");
            newIcon = cm.Load<Texture2D>("Interface/Shared/newIcon");

            regFont = cm.Load<SpriteFont>("Fonts/regularOutlined");
            font = cm.Load<SpriteFont>("Fonts/BoldOutlined");

            paneButtonOne = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);
            paneButtonTwo = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);
            paneButtonThree = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);
            paneButtonFour = new MenuButton(Vector2.Zero, paneButton, paneButton, paneButtonHover, 1f, true);
        }

        private int lastStackCount = 0;
        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            currentFrame = animation.FramePosition(gt, 55, new Point(8, 1), true);

            if (currentSpells.Count > gridSize.X * gridSize.Y)
                ScrollSpells(gt);

            KeepSpellsInBound();
            UpdateSpells();
            UpdateButtons(gt);

            ApplyProperItemGrid();

            if (lastStackCount != storage.Spells.Count)
                SortSpells();

            CheckSpellEquipDrag(equipment.SpellOne, 1, "Slot One");
            CheckSpellEquipDrag(equipment.SpellTwo, 2, "Slot Two");
            CheckSpellEquipDrag(equipment.SpellThree, 3, "Slot Three");
            CheckSpellEquipDrag(equipment.SpellFour, 4, "Slot Four");
            CheckSpellEquipDrag(equipment.SpellFive, 5, "Slot Five");
            CheckSpellEquipDrag(equipment.SpellSix, 6, "Slot Six");
            CheckSpellEquipDrag(equipment.SpellSeven, 7, "Slot Seven");

            CheckDragRelease();

            lastStackCount = storage.Spells.Count;

            controls.UpdateLast();
        }

        private Vector2 lastMousePosition;
        private BaseSpell draggedSpell = null;
        private int draggedSpellIndex = -1;
        private void UpdateSpells()
        {
            isSpellSelected = false;

            for (int i = 0; i < currentSpells.Count; i++)
            {
                if (currentSpells[i].spellRect.Contains(controls.MousePosition))
                {
                    ToolTip.RequestStringAssign(currentSpells[i].Name);

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        currentSpells[i].IsSelected = true;
                        currentSpells[i].IsNew = false;
                        selectedSpell = currentSpells[i];

                        screens.PlaySound("ButtonClick");
                    }
                }
                else if (controls.IsClickedOnce(Controls.MouseButton.LeftClick) && !paneRect.Contains(controls.MousePosition))
                    currentSpells[i].IsSelected = false;

                //Spell-drag checking
                if (currentSpells[i].IsSelected == true)
                {
                    isSpellSelected = true;

                    if (currentSpells[i].spellRect.Contains(controls.MousePosition))
                    {
                        if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        {
                            if (Vector2.Distance(lastMousePosition, controls.MouseVector) > 5f)
                            {
                                draggedSpell = currentSpells[i];
                                draggedSpellIndex = i;
                            }
                        }
                        else
                            lastMousePosition = controls.MouseVector;
                    }
                }

                if (draggedSpell != null)
                    CheckSpellSwapping(draggedSpellIndex, i);
            }
        }
        private void CheckDragRelease()
        {
            if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                draggedSpell = null;
                draggedSpellIndex = -1;

                lastMousePosition = controls.MouseVector;
            }
        }
        private void CheckSpellSwapping(int draggedItemIndex, int swapIndex)
        {
            if (currentSpells[swapIndex].spellRect.Contains(controls.MousePosition))
            {
                if (currentSpells[swapIndex].ID != draggedSpell.ID)
                    ToolTip.RequestStringAssign("Release: Swap item");

                if (draggedSpell.ID != currentSpells[swapIndex].ID)
                {
                    if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    {
                        currentSpells.Swap(draggedItemIndex, swapIndex);
                        screens.PlaySound("SwapItem");
                    }
                }
            }
        }

        private void CheckSpellEquipDrag(SpellSlot slot, int index, string slotName)
        {
            if (index <= controlledEntity.MAGIC_SpellSlots())
            {
                if (slot.slotRect.Contains(controls.MousePosition))
                {
                    if (screens.SOULGATE_IsNear())
                    {
                        if (slot.spell != null)
                            ToolTip.RequestStringAssign(slot.spell.Name);
                        else
                            ToolTip.RequestStringAssign(slotName);

                        if (draggedSpell != null && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            equipment.EquipSpell(draggedSpell, index);
                            screens.PlaySound("EquipItem");
                        }

                        if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                            slot.spell = null;
                    }
                    else
                        ToolTip.RequestStringAssign("Must be near a soulgate.");
                }

                if (slot.deleteRect.Contains(controls.MousePosition))
                {
                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        slot.spell = null;
                    }
                }
            }
        }

        private void UpdateButtons(GameTime gt)
        {
            paneButtonOne.Update(gt, controls);
            paneButtonTwo.Update(gt, controls);
            paneButtonThree.Update(gt, controls);
            paneButtonFour.Update(gt, controls);

            paneButtonOne.Position = new Point((int)screenPosition.X + 240, (int)screenPosition.Y + 146);
            paneButtonTwo.Position = new Point((int)screenPosition.X + 240, (int)screenPosition.Y + 146 + (paneButton.Height + 3) * 1);
            paneButtonThree.Position = new Point((int)screenPosition.X + 240, (int)screenPosition.Y + 146 + (paneButton.Height + 3) * 2);
            paneButtonFour.Position = new Point((int)screenPosition.X + 240, (int)screenPosition.Y + 146 + (paneButton.Height + 3) * 3);

            if (selectedSpell != null)
            {
                if (screens.SOULGATE_IsNear())
                {
                    if (paneButtonOne.IsLeftClicked == true && !string.IsNullOrEmpty(selectedSpell.ButtonOneText))
                    {
                        //selectedSpell.ButtonOne();
                        equipment.EquipAvailableSpell(selectedSpell);
                        screens.PlaySound("ButtonClick");
                    }
                    if (paneButtonTwo.IsLeftClicked == true && !string.IsNullOrEmpty(selectedSpell.ButtonTwoText))
                    {
                        //selectedSpell.ButtonTwo();
                        equipment.UnequipSpell(selectedSpell);
                        screens.PlaySound("ButtonClick");
                    }
                    if (paneButtonThree.IsLeftClicked == true && !string.IsNullOrEmpty(selectedSpell.ButtonThreeText))
                    {
                        selectedSpell.ButtonThree();
                        screens.PlaySound("ButtonClick");
                    }
                    if (paneButtonFour.IsLeftClicked == true && !string.IsNullOrEmpty(selectedSpell.ButtonFourText))
                    {
                        selectedSpell.ButtonFour();
                        screens.PlaySound("ButtonClick");
                    }
                }
                else
                {
                    if (paneButtonOne.IsHover || paneButtonTwo.IsHover || paneButtonThree.IsHover || paneButtonFour.IsHover)
                        ToolTip.RequestStringAssign("Must be near a soulgate.");
                }
            }
        }

        private void SortSpells()
        {
            currentSpells.Clear();

            if (storage != null)
            {
                for (int i = 0; i < storage.Spells.Count; i++)
                {
                    if (storage.Spells[i].Type == type)
                        currentSpells.Add(storage.Spells[i]);
                }
            }
        }

        float scrollValue = 0f; float scrollVelocity = 0f;
        private void ScrollSpells(GameTime gt)
        {
            if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                scrollVelocity -= 100f;
            else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                scrollVelocity += 100f;

            scrollValue = controls.CurrentMS.ScrollWheelValue;

            //Smooth scrolling code
            scrollPosition += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -1024f, 1024f);

            if (scrollVelocity > .02f)
                scrollVelocity -= 500f * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -.02f)
                scrollVelocity += 500f * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -.02f && scrollVelocity <= .02f)
                scrollVelocity = 0f;
        }

        private void KeepSpellsInBound()
        {
            int longBounds = -((currentSpells.Count / gridSize.X) * (iconBG.Height + 2) - 142);

            if (scrollPosition >= -187f)
            {
                scrollPosition = -187f;
                scrollVelocity = 0f;
            }
            else if (scrollPosition <= longBounds)
            {
                scrollPosition = longBounds;
                scrollVelocity = 0f;
            }

            /*for (int i = 0; i < spells.Count; i++)  --- Not necessary for now
            {
                if (spells[i].isSelected == true)
                {
                    int paneLongBounds = (int)boldFont.MeasureString(spells[i].Description.WrapText(boldFont, 280)).Y;

                        if (paneLocY >= paneRect.Y + 92)
                            paneLocY = paneRect.Y + 92;
                        else if (paneLocY + paneLongBounds <= paneRect.Y + 378) //Kinda glitchy still!
                            paneLocY = paneLongBounds - paneRect.Y + 378;

                }
            }*/
        }

        private void ApplyProperItemGrid()
        {
            if (currentSpells.Count > 6)
            {
                for (int index = 0; index < currentSpells.Count; ++index)
                {
                    currentSpells[index].gridLocation = new Point(index % 7, (int)(index / 7));
                    currentSpells[index].UpdateValues(screenPosition + new Vector2(-230 + (currentSpells[index].gridLocation.X * 2), currentSpells[index].gridLocation.Y * 2), 64, (int)scrollPosition);
                }
            }
            else if (currentSpells.Count < 7)
            {
                for (int index = 0; index < currentSpells.Count; ++index)
                {
                    currentSpells[index].gridLocation = new Point(index, 0);
                    currentSpells[index].UpdateValues(screenPosition + new Vector2(-230 + (currentSpells[index].gridLocation.X * 2), currentSpells[index].gridLocation.Y * 2), 64, (int)scrollPosition);
                }
            }
        }

        public void DrawPane(SpriteBatch sb)
        {
            if (selectedSpell != null)
            {
                sb.Draw(iconBG, screenPosition + new Vector2(239, -215), Color.White); //Icon Background
                sb.Draw(selectedSpell.Icon, screenPosition + new Vector2(239, -215), Color.White); //Icon

                sb.DrawString(font, selectedSpell.Name, screenPosition + new Vector2(450, -193), selectedSpell.Name.LineCenter(font), ColorHelper.UI_Gold, 1f); //Spell name
                sb.DrawString(regFont, selectedSpell.Description.WrapText(regFont, 375), screenPosition + new Vector2(245, -140), Color.White); //Description

                sb.DrawString(font, selectedSpell.MaxCharges + " Charges", screenPosition + new Vector2(417, 127), (selectedSpell.MaxCharges + " Charges").LineCenter(font), Color.White, 1f);

                DrawPaneButton(sb, paneButtonOne, selectedSpell.ButtonOneText);
                DrawPaneButton(sb, paneButtonTwo, selectedSpell.ButtonTwoText);
                DrawPaneButton(sb, paneButtonThree, selectedSpell.ButtonThreeText);
                DrawPaneButton(sb, paneButtonFour, selectedSpell.ButtonFourText);
            }
        }

        private Color halfTransparent = Color.Lerp(Color.Transparent, Color.White, .5f);
        private void DrawPaneButton(SpriteBatch sb, MenuButton button, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (screens.SOULGATE_IsNear())
                {
                    button.DrawButton(sb, Color.White);
                    sb.DrawString(regFont, text, button.Center, Color.White, 0f, text.LineCenter(regFont), 1f, SpriteEffects.None, 1f);
                }
                else
                {
                    button.DrawButtonIdle(sb, halfTransparent);
                    sb.DrawString(regFont, text, button.Center, halfTransparent, 0f, text.LineCenter(regFont), 1f, SpriteEffects.None, 1f);
                }
            }
            else
                button.DrawButtonIdle(sb, halfTransparent);
        }

        public void DrawSpells(SpriteBatch sb)
        {
            for (int i = 0; i < currentSpells.Count; i++)
            {
                if (currentSpells[i].IsSelected == true)
                    sb.Draw(iconBGSelect, currentSpells[i].spellRect, Color.White);
                else
                    sb.Draw(iconBG, currentSpells[i].spellRect, Color.White);

                if (currentSpells[i].IsNew == true)
                    sb.Draw(newIcon, new Vector2(currentSpells[i].spellRect.X + 4, currentSpells[i].spellRect.Y + 4), new Rectangle(currentFrame.X * 9, currentFrame.Y * 9, 9, 9), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

                sb.Draw(currentSpells[i].Icon, currentSpells[i].spellRect.Location.ToVector2(), Color.White, Vector2.Zero, 0f, 1f);
            }
        }
        public void DrawDrag(SpriteBatch sb)
        {
            if (draggedSpell != null)
                sb.Draw(draggedSpell.Icon, controls.MouseVector, Color.White, draggedSpell.Icon.Center(), 0f, 1f);
        }

        public bool IsClickingUI()
        {
            return paneRect.Contains(controls.MousePosition);
        }
    }
}
