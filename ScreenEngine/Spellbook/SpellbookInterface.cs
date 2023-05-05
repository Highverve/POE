using System.Collections.Generic;
using System.Text;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.EquipmentScreens;

namespace Pilgrimage_Of_Embers.ScreenEngine.Spellbook
{
    public class SpellbookInterface
    {
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = new Vector2(MathHelper.Clamp(value.X, 290, GameSettings.VectorResolution.X - 633), MathHelper.Clamp(value.Y, 320, GameSettings.VectorResolution.Y - 270)); }
        }

        private BaseSpell.TabType currentTab = BaseSpell.TabType.Destructive;

        SpellbookTab destructiveTab, cursingsTab, blessingsTab, invocationsTab, sundriesTab;

        private Texture2D tabBG, tabButton, tabButtonSelect, paneBG, paneEnd, elementalIcon, cursingsIcon, blessingsIcon, invocationsIcon, sundriesIcon;
        private SpriteFont largeFont, font;

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        Rectangle scissorRect, paneRect;
        RasterizerState rState = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState rStateOff = new RasterizerState() { ScissorTestEnable = false };

        private Controls controls = new Controls();

        public bool IsActive { get; set; }

        private float paneWidth;

        BaseEntity controlledEntity;
        private ScreenManager screens;
        private EquipmentUI equipmentUI;

        public SpellbookInterface()
        {
            destructiveTab = new SpellbookTab(BaseSpell.TabType.Destructive);
            cursingsTab = new SpellbookTab(BaseSpell.TabType.Cursings);
            blessingsTab = new SpellbookTab(BaseSpell.TabType.Blessings);
            invocationsTab = new SpellbookTab(BaseSpell.TabType.Invocations);
            sundriesTab = new SpellbookTab(BaseSpell.TabType.Sundries);
        }

        public void SetReferences(ScreenManager screens, EquipmentUI equipmentUI)
        {
            this.screens = screens;
            this.equipmentUI = equipmentUI;

            destructiveTab.SetReferences(screens);
            cursingsTab.SetReferences(screens);
            blessingsTab.SetReferences(screens);
            invocationsTab.SetReferences(screens);
            sundriesTab.SetReferences(screens);
        }
        public void SetControlledEntity(BaseEntity controlledEntity, EntityStorage storage, EntityEquipment equipment)
        {
            this.controlledEntity = controlledEntity;

            destructiveTab.SetControlledEntity(controlledEntity, storage, equipment);
            cursingsTab.SetControlledEntity(controlledEntity, storage, equipment);
            blessingsTab.SetControlledEntity(controlledEntity, storage, equipment);
            invocationsTab.SetControlledEntity(controlledEntity, storage, equipment);
            sundriesTab.SetControlledEntity(controlledEntity, storage, equipment);
        }

        public void AddSpell(int id)
        {
           controlledEntity.STORAGE_AddSpell(id);
        }
        public void RemoveSpell(int id)
        {

        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);


            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
        }

        private const string directory = "Interface/Spellbook/";
        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            tabBG = cm.Load<Texture2D>("Interface/Global/tabBG");
            tabButton = cm.Load<Texture2D>("Interface/Global/tabButton");
            tabButtonSelect = cm.Load<Texture2D>("Interface/Global/tabButtonSelect");

            paneBG = cm.Load<Texture2D>("Interface/Inventory/Backgrounds/paneBG");
            paneEnd = cm.Load<Texture2D>("Interface/Global/paneEnd");

            elementalIcon = cm.Load<Texture2D>(directory + "Icons/negativeOther");
            cursingsIcon = cm.Load<Texture2D>(directory + "Icons/negativeSelf");
            blessingsIcon = cm.Load<Texture2D>(directory + "Icons/positiveSelf");
            invocationsIcon = cm.Load<Texture2D>(directory + "Icons/invocation");
            sundriesIcon = cm.Load<Texture2D>(directory + "Icons/positiveOther");
            
            position = GameSettings.VectorCenter;

            destructiveTab.Load(cm);
            cursingsTab.Load(cm);
            blessingsTab.Load(cm);
            invocationsTab.Load(cm);
            sundriesTab.Load(cm);

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
            controls.UpdateLast();
            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenMagic))
                IsActive = !IsActive;

            if (IsActive == true)
                UpdateBehavior(gt);
        }
        private void UpdateBehavior(GameTime gt)
        {
            CheckDrag();
            SetPositions();

            switch (currentTab)
            {
                case BaseSpell.TabType.Destructive: destructiveTab.Update(gt); ChangePaneWidth(gt, destructiveTab.IsSpellSelected); break;
                case BaseSpell.TabType.Cursings: cursingsTab.Update(gt); ChangePaneWidth(gt, cursingsTab.IsSpellSelected); break;
                case BaseSpell.TabType.Blessings: blessingsTab.Update(gt); ChangePaneWidth(gt, blessingsTab.IsSpellSelected); break;
                case BaseSpell.TabType.Invocations: invocationsTab.Update(gt); ChangePaneWidth(gt, invocationsTab.IsSpellSelected); break;
                case BaseSpell.TabType.Sundries: sundriesTab.Update(gt); ChangePaneWidth(gt, sundriesTab.IsSpellSelected); break;
            }

            isClickingTabButton = false;
            CheckTabClick(tabButtonPosition, BaseSpell.TabType.Destructive);
            CheckTabClick(tabButtonPosition + new Vector2(0, tabButton.Height), BaseSpell.TabType.Cursings);
            CheckTabClick(tabButtonPosition + new Vector2(0, tabButton.Height * 2), BaseSpell.TabType.Blessings);
            CheckTabClick(tabButtonPosition + new Vector2(0, tabButton.Height * 3), BaseSpell.TabType.Invocations);
            CheckTabClick(tabButtonPosition + new Vector2(0, tabButton.Height * 4), BaseSpell.TabType.Sundries);

            UpdateWindowButtons(gt, "");
        }
        protected virtual void UpdateWindowButtons(GameTime gt, string hintHover)
        {
            hintRect = new Rectangle((int)position.X + 145, (int)position.Y - 262, windowButton.Width - 20, windowButton.Height);
            hideRect = new Rectangle((int)position.X + 172, (int)position.Y - 262, windowButton.Width - 20, windowButton.Height);

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
                ToolTip.RequestStringAssign("Hide Spellbook");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 1");
                    IsActive = false;
                }
            }
            else
                isHideHover = false;
        }

        private bool isDragging = false; private Rectangle dragArea;
        private void CheckDrag()
        {
            dragArea = new Rectangle((int)position.X - (tabBG.Width / 2) + 100, (int)position.Y - (tabBG.Height / 2), tabBG.Width - 200, 24);

            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                Position = controls.MouseVector + new Vector2(0, tabBG.Center().Y - 12);
                screens.SetCursorState(Cursor.CursorState.Move);
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

            paneWidth = MathHelper.SmoothStep(-4f, paneBG.Width, increaseValue);
        }
        private void CheckTabClick(Vector2 position, BaseSpell.TabType tab)
        {
            if (new Rectangle((int)position.X, (int)position.Y, tabButton.Width, tabButton.Height).Contains(controls.MousePosition) && controls.IsClickedOnce(Controls.MouseButton.LeftClick))
            {
                isClickingTabButton = true;
                screens.PlaySound("Button Click 2");
                currentTab = tab;
            }
        }

        private Vector2 tabButtonPosition, selectedTab = new Vector2(-4, 0);
        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                DrawTabButton(sb, tabButtonPosition, elementalIcon, BaseSpell.TabType.Destructive);
                DrawTabButton(sb, tabButtonPosition + new Vector2(0, tabButton.Height), cursingsIcon, BaseSpell.TabType.Cursings);
                DrawTabButton(sb, tabButtonPosition + new Vector2(0, tabButton.Height * 2), blessingsIcon, BaseSpell.TabType.Blessings);
                DrawTabButton(sb, tabButtonPosition + new Vector2(0, tabButton.Height * 3), invocationsIcon, BaseSpell.TabType.Invocations);
                DrawTabButton(sb, tabButtonPosition + new Vector2(0, tabButton.Height * 4), sundriesIcon, BaseSpell.TabType.Sundries);

                sb.Draw(paneBG, Position + new Vector2(424, 10), new Rectangle(0, 0, (int)paneWidth, paneBG.Height), Color.White, 0f, paneBG.Center(), 1f, SpriteEffects.None, 1f);
                sb.Draw(paneEnd, Position + new Vector2(247 + paneWidth, 10), Color.White, paneEnd.Center(), 0f, 1f);

                sb.Draw(tabBG, Position, Color.White, tabBG.Center(), 0f, 1f, 1f);

                sb.DrawString(largeFont, "Spellbook", position + new Vector2(0, -(tabBG.Height / 2) + 12), "Spellbook".LineCenter(largeFont), ColorHelper.UI_Gold, 1f);

                DrawHeader(sb, currentTab.ToString() + " Spells", BaseSpell.TabType.Destructive);
                DrawHeader(sb, currentTab.ToString(), BaseSpell.TabType.Cursings);
                DrawHeader(sb, currentTab.ToString(), BaseSpell.TabType.Blessings);
                DrawHeader(sb, currentTab.ToString(), BaseSpell.TabType.Invocations);
                DrawHeader(sb, currentTab.ToString(), BaseSpell.TabType.Sundries);

                DrawWindowButtons(sb);

                equipmentUI.DrawSpellSlots(sb);

                sb.End();

                DrawSpells(sb);
                DrawPane(sb);
            }
        }
        public void DrawDrag(SpriteBatch sb)
        {
            switch (currentTab)
            {
                case BaseSpell.TabType.Destructive: destructiveTab.DrawDrag(sb); break;
                case BaseSpell.TabType.Cursings: cursingsTab.DrawDrag(sb); break;
                case BaseSpell.TabType.Blessings: blessingsTab.DrawDrag(sb); break;
                case BaseSpell.TabType.Invocations: invocationsTab.DrawDrag(sb); break;
                case BaseSpell.TabType.Sundries: sundriesTab.DrawDrag(sb); break;
            }
        }
        private void DrawTabButton(SpriteBatch sb, Vector2 position, Texture2D icon, BaseSpell.TabType tab)
        {
            if (currentTab == tab)
            {
                sb.Draw(tabButtonSelect, position + selectedTab, Color.White);
                sb.Draw(icon, position + selectedTab, Color.White, Vector2.Zero, 0f, 1f);
            }
            else
            {
                sb.Draw(tabButton, position, Color.White);
                sb.Draw(icon, position, Color.Gray, Vector2.Zero, 0f, 1f);
            }
        }
        private void DrawHeader(SpriteBatch sb, string text, BaseSpell.TabType tab)
        {
            if (currentTab == tab)
                sb.DrawString(largeFont, text, Position + new Vector2(0, -215), text.LineCenter(largeFont), Color.White, 1f);
        }
        private void DrawSpells(SpriteBatch sb)
        {
            scissorRect = new Rectangle((int)Position.X - (int)tabBG.Center().X, ((int)Position.Y - (int)tabBG.Center().Y) + 72, tabBG.Width, (5 * 66) + 4);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, rState);
            sb.GraphicsDevice.ScissorRectangle = scissorRect;

            switch (currentTab)
            {
                case BaseSpell.TabType.Destructive: destructiveTab.DrawSpells(sb); break;
                case BaseSpell.TabType.Cursings: cursingsTab.DrawSpells(sb); break;
                case BaseSpell.TabType.Blessings: blessingsTab.DrawSpells(sb); break;
                case BaseSpell.TabType.Invocations: invocationsTab.DrawSpells(sb); break;
                case BaseSpell.TabType.Sundries: sundriesTab.DrawSpells(sb); break;
            }

            sb.GraphicsDevice.RasterizerState = rStateOff;
            sb.End();
        }
        private void DrawPane(SpriteBatch sb)
        {
            //screenPosition + new Vector2(239, -215)
            paneRect = new Rectangle((int)Position.X + 239, (int)Position.Y - 215, (int)MathHelper.Clamp(paneWidth, 0, paneWidth), paneBG.Height);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, rState);
            sb.GraphicsDevice.ScissorRectangle = paneRect;

            switch (currentTab)
            {
                case BaseSpell.TabType.Destructive: destructiveTab.DrawPane(sb); break;
                case BaseSpell.TabType.Cursings: cursingsTab.DrawPane(sb); break;
                case BaseSpell.TabType.Blessings: blessingsTab.DrawPane(sb); break;
                case BaseSpell.TabType.Invocations: invocationsTab.DrawPane(sb); break;
                case BaseSpell.TabType.Sundries: sundriesTab.DrawPane(sb); break;
            }

            sb.GraphicsDevice.RasterizerState = rStateOff;
            sb.End();
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

        private void SetPositions()
        {
            tabButtonPosition = Position + new Vector2(-282, -222);

            destructiveTab.SetScreenPosition(Position, paneRect);
            cursingsTab.SetScreenPosition(Position, paneRect);
            blessingsTab.SetScreenPosition(Position, paneRect);
            invocationsTab.SetScreenPosition(Position, paneRect);
            sundriesTab.SetScreenPosition(Position, paneRect);
        }

        //return isClickingTabButton || isClickingTab || IsPaneClickingAll() || isDragging || hideRect.Contains(controls.MousePosition);
        private bool isClickingTabButton = false;
        public bool IsClickingPane()
        {
            return destructiveTab.IsClickingUI() ||
                   cursingsTab.IsClickingUI() ||
                   blessingsTab.IsClickingUI() ||
                   invocationsTab.IsClickingUI() ||
                   sundriesTab.IsClickingUI();
        }
        public bool IsClickingUI()
        {
            if (IsActive == true)
            {
                return isClickingTabButton || IsClickingPane() || new Rectangle((int)position.X - (tabBG.Width / 2),
                                                                                (int)position.Y - (tabBG.Height / 2),
                                                                                tabBG.Width, tabBG.Height).Contains(controls.MousePosition) ||
                       isDragging || hideRect.Contains(controls.MousePosition);
            }
            else
                return false;
        }
    }
}
