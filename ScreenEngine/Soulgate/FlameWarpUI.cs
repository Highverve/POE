using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.TileEngine.Objects.Soulgates;
using System;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    public class FlameWarpUI
    {
        private Vector2 position;
        private Vector2 Position { set { position = value; } }

        private Rectangle dragArea, clickRect;

        private Texture2D bg, pane, paneEnd, longButton, longButtonHover, button, buttonHover, tab, tabSelected;
        private Texture2D southernLandsIcon, sloughlandIcon, stormridgeIcon, jaggedSeaboardIcon, cultureIcon, loneLandsIcon, otherLandsIcon, memorizedIcon;

        private SpriteFont font, largeFont;

        private float paneWidth = 0, locationY = 0;

        private Rectangle southernLandRect, sloughlandRect, stormridgeRect, jaggedSeaboardRect, cultureRect, loneLandsRect, otherLandsRect;

        private List<BaseCheckpoint> currentSoulgates = new List<BaseCheckpoint>();
        private BaseCheckpoint.LocationType currentTab = BaseCheckpoint.LocationType.SouthernLands;

        private MenuButton warpToButton, memorizeButton;

        public bool IsActive { get; set; }

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        private Controls controls = new Controls();
        private ScreenManager screens;
        private WorldManager world;
        private Camera camera;
        private TileMap map;

        private BaseEntity controlledEntity;
        private BaseCheckpoint currentGate;

        private bool isTabHover = false;

        public FlameWarpUI()
        {
            IsActive = true;
        }
        public void SetReferences(ScreenManager screens, TileMap map, WorldManager world, Camera camera)
        {
            this.screens = screens;
            this.world = world;
            this.camera = camera;
        }
        public void SetControlledEntity(BaseEntity controlledEntity)
        {
            this.controlledEntity = controlledEntity;
        }
        public void SetGate(BaseCheckpoint currentGate)
        {
            this.currentGate = currentGate;
            SortGates();
        }

        public void Load(ContentManager cm)
        {
            bg = cm.Load<Texture2D>("Interface/Soulgate/Travelling/bg");
            pane = cm.Load<Texture2D>("Interface/Soulgate/Travelling/pane");
            paneEnd = cm.Load<Texture2D>("Interface/Soulgate/Travelling/paneEnd");
            longButton = cm.Load<Texture2D>("Interface/Soulgate/Travelling/longButton");
            longButtonHover = cm.Load<Texture2D>("Interface/Soulgate/Travelling/longButtonHover");

            button = cm.Load<Texture2D>("Interface/Global/paneExtendedButton");
            buttonHover = cm.Load<Texture2D>("Interface/Global/paneExtendedButtonHover");

            tab = cm.Load<Texture2D>("Interface/Global/tabButton");
            tabSelected = cm.Load<Texture2D>("Interface/Global/tabButtonSelect");

            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            southernLandsIcon = cm.Load<Texture2D>("Interface/Soulgate/Travelling/tempIcon");
            sloughlandIcon = cm.Load<Texture2D>("Interface/Soulgate/Travelling/tempIcon");
            stormridgeIcon = cm.Load<Texture2D>("Interface/Soulgate/Travelling/tempIcon");
            jaggedSeaboardIcon = cm.Load<Texture2D>("Interface/Soulgate/Travelling/tempIcon");
            cultureIcon = cm.Load<Texture2D>("Interface/Soulgate/Travelling/tempIcon");
            loneLandsIcon = cm.Load<Texture2D>("Interface/Soulgate/Travelling/tempIcon");
            otherLandsIcon = cm.Load<Texture2D>("Interface/Soulgate/Travelling/tempIcon");

            warpToButton = new MenuButton(Vector2.Zero, button, buttonHover, buttonHover, 1f, false);
            memorizeButton = new MenuButton(Vector2.Zero, button, buttonHover, buttonHover, 1f, false);

            position = new Vector2(GameSettings.VectorCenter.X - bg.Center().X, GameSettings.VectorCenter.Y - bg.Center().Y);
            mouseDragOffset = new Vector2(bg.Center().X, 12);

            LoadWindowButtons(cm);
        }
        protected virtual void LoadWindowButtons(ContentManager cm)
        {
            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");
        }

        public void SortGates()
        {
            currentSoulgates.Clear();

            for (int i = 0; i < MonumentDatabase.Soulgates.Count; i++)
            {
                if (MonumentDatabase.Soulgates[i] != currentGate)
                {
                    if (MonumentDatabase.Soulgates[i].Location == currentTab)
                    {
                        if (MonumentDatabase.Soulgates[i].IsActivated)
                        {
                            currentSoulgates.Add(MonumentDatabase.Soulgates[i]);
                        }
                    }
                }
            }

            currentSoulgates.Sort((a, b) => a.CompareGate(b));
        }

        private string hints = "Flame Warp Tips:\n\n" +
            "The flame warper is for travelling great distances very quickly.\n" +
            "The gates are sorted by region. Only active monuments can be warped to.\n\n" +
            "Additionally, all favorited gates will be displayed at the top of each region's tab.";

        private BaseCheckpoint selectedGate = null;
        private bool isSelected = false;
        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            if (IsActive == true)
            {
                isSelected = false;

                clickRect = new Rectangle((int)position.X, (int)position.Y + 20, bg.Width + (int)paneWidth, bg.Height - 20);

                CheckDragScreen();
                UpdateWindowButtons(gt, hints);

                for (int i = 0; i < currentSoulgates.Count; i++)
                {
                    currentSoulgates[i].RectUI = new Rectangle((int)position.X + 16, (int)position.Y + 58 + (i * (longButton.Height + 1)) + (int)locationY, longButton.Width, longButton.Height);

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        if (currentSoulgates[i].RectUI.Contains(controls.MousePosition))
                        {
                            screens.PlaySound("Item Select");

                            selectedGate = currentSoulgates[i];
                            currentSoulgates[i].IsSelected = true;
                        }
                        else if (!scissorPane.Contains(controls.MousePosition) && !dragArea.Contains(controls.MousePosition))
                            currentSoulgates[i].IsSelected = false;
                    }

                    if (currentSoulgates[i].IsSelected == true)
                        isSelected = true;
                }

                UpdateTabClicking(gt);
                UpdateButtonClicking(gt);
                ChangePaneWidth(gt, isSelected);

                if (scissorGrid.Contains(controls.MousePosition))
                    ScrollGrid(gt, 50f, 500f, 300f, 10f);
            }

            BeginTeleportation(gt);

            controls.UpdateLast();
        }
        protected virtual void UpdateWindowButtons(GameTime gt, string hintHover)
        {
            hintRect = new Rectangle((int)position.X + 237, (int)position.Y, windowButton.Width - 20, windowButton.Height);
            hideRect = new Rectangle((int)position.X + 264, (int)position.Y, windowButton.Width - 20, windowButton.Height);

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
                ToolTip.RequestStringAssign("Hide Flame Warper");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 1");
                    IsActive = false;
                }
            }
            else
                isHideHover = false;
        }
        private void UpdateTabClicking(GameTime gt)
        {
            isTabHover = false;

            southernLandRect = new Rectangle((int)position.X - 33, (int)position.Y + 37, tab.Width, tab.Height);
            sloughlandRect = new Rectangle((int)position.X - 33, (int)position.Y + 37 + (tab.Height), tab.Width, tab.Height);
            stormridgeRect = new Rectangle((int)position.X - 33, (int)position.Y + 37 + (tab.Height * 2), tab.Width, tab.Height);
            jaggedSeaboardRect = new Rectangle((int)position.X - 33, (int)position.Y + 37 + (tab.Height * 3), tab.Width, tab.Height);
            cultureRect = new Rectangle((int)position.X - 33, (int)position.Y + 37 + (tab.Height * 4), tab.Width, tab.Height);
            loneLandsRect = new Rectangle((int)position.X - 33, (int)position.Y + 37 + (tab.Height * 5), tab.Width, tab.Height);
            otherLandsRect = new Rectangle((int)position.X - 33, (int)position.Y + 37 + (tab.Height * 6), tab.Width, tab.Height);

            CheckTabClick(southernLandRect, BaseCheckpoint.LocationType.SouthernLands, "The Southern Lands");
            CheckTabClick(sloughlandRect, BaseCheckpoint.LocationType.Sloughland, "The Sloughland");
            CheckTabClick(stormridgeRect, BaseCheckpoint.LocationType.Stormridge, "The Stormridge");
            CheckTabClick(jaggedSeaboardRect, BaseCheckpoint.LocationType.JaggedSeaboard, "Jagged Seaboard");
            CheckTabClick(cultureRect, BaseCheckpoint.LocationType.Cultures, "Central Lands");
            CheckTabClick(loneLandsRect, BaseCheckpoint.LocationType.LoneLands, "The Lone Lands");
            CheckTabClick(otherLandsRect, BaseCheckpoint.LocationType.OtherLands, "Other Lands");
        }
        private void CheckTabClick(Rectangle rect, BaseCheckpoint.LocationType type, string name)
        {
            if (rect.Contains(controls.MousePosition))
            {
                isTabHover = true;
                ToolTip.RequestStringAssign(name);

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 2");

                    currentTab = type;
                    SortGates();
                }
            }
        }

        private void UpdateButtonClicking(GameTime gt)
        {
            warpToButton.Position = new Point((int)position.X + 292, (int)position.Y + 283);
            memorizeButton.Position = new Point((int)position.X + 292, (int)position.Y + 304);

            warpToButton.Update(gt, controls);
            memorizeButton.Update(gt, controls);

            if (selectedGate != null && isSelected == true)
            {
                if (warpToButton.IsHover == true)
                    ToolTip.RequestStringAssign("Teleport to selected monument");
                if (memorizeButton.IsHover == true)
                    ToolTip.RequestStringAssign("Memorize monument");

                if (warpToButton.IsLeftClicked == true)
                {
                    isWarping = true;

                    //Close current soulgate interfaces
                    screens.SOULGATE_CloseAllUIs();
                    paneWidth = 0f;

                    for (int i = 0; i < currentSoulgates.Count; i++)
                        currentSoulgates[i].IsSelected = false;

                    isSelected = false;

                    warpToButton.IsLeftClicked = false;
                }

                if (memorizeButton.IsLeftClicked == true)
                {
                    selectedGate.IsMemorized = !selectedGate.IsMemorized;
                    currentSoulgates.Sort((a, b) => a.CompareGate(b));
                }
            }
        }

        private int delayTime = 0, pauseZoomTime = 0;
        private bool isWarping = false, initializeTeleportVariables = false;
        private void BeginTeleportation(GameTime gt)
        {
            if (isWarping == true)
            {
                if (initializeTeleportVariables == false)
                {
                    camera.SmoothZoom(1.2f, 1f, true, 0);
                    initializeTeleportVariables = true;
                }

                delayTime += (int)gt.ElapsedGameTime.TotalMilliseconds;

                if (delayTime >= 500)
                {
                    map.FadeVariables();
                    screens.EFFECTS_BeginTransition(ScreenEffects.TransitionType.Fade, Color.Black, 1000, 1.5f, 2f);

                    if (screens.EFFECTS_IsTransitionFaded)
                    {
                        selectedGate.LoadMapTo();
                        delayTime = 0;
                        isWarping = false;

                        increaseValue = 0f;
                        selectedGate = null;
                    }
                }
            }
            else
            {
                pauseZoomTime += (int)gt.ElapsedGameTime.TotalMilliseconds;
                if (pauseZoomTime >= 500)
                {
                    if (initializeTeleportVariables == true)
                    {
                        camera.SmoothZoom(1f, 1f, true, 0);
                        initializeTeleportVariables = false;
                    }
                    pauseZoomTime = 0;
                }
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

            paneWidth = MathHelper.SmoothStep(-4f, pane.Width, increaseValue);
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
            locationY += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -maxScrollSpeed, maxScrollSpeed);

            if (scrollVelocity > clampSpeed)
                scrollVelocity -= scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -clampSpeed)
                scrollVelocity += scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -clampSpeed && scrollVelocity < clampSpeed)
                scrollVelocity = 0f;

            float longBounds = -(int)((currentSoulgates.Count - 1) * (longButton.Height + 1)) + (scissorGrid.Height - longButton.Height);

            
            if (longBounds >= 0f)
                longBounds = 0f;

            if (locationY > 0f)
                scrollVelocity = 0f;
            else if (locationY < longBounds)
                scrollVelocity = 0f;

            locationY = MathHelper.Clamp(locationY, longBounds, 0f);
        }

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private void CheckDragScreen()
        {
            dragArea = new Rectangle((int)position.X + 74, (int)position.Y, 154, 20);

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

        private Rectangle scissorGrid, scissorPane;
        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        private Color memorizedColor = new Color(255, 150, 112, 255);

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin();

                sb.Draw(pane, new Vector2(position.X + 292, position.Y + 55), new Rectangle(0, 0, (int)paneWidth, pane.Height), Color.White);
                sb.Draw(paneEnd, new Vector2((position.X + 292) + paneWidth, position.Y + 56), Color.White);

                DrawTabButton(sb, southernLandsIcon, southernLandRect, BaseCheckpoint.LocationType.SouthernLands, 1);
                DrawTabButton(sb, sloughlandIcon, sloughlandRect, BaseCheckpoint.LocationType.Sloughland, 2);
                DrawTabButton(sb, stormridgeIcon, stormridgeRect, BaseCheckpoint.LocationType.Stormridge, 3);
                DrawTabButton(sb, jaggedSeaboardIcon, jaggedSeaboardRect, BaseCheckpoint.LocationType.JaggedSeaboard, 4);
                DrawTabButton(sb, cultureIcon, cultureRect, BaseCheckpoint.LocationType.Cultures, 5);
                DrawTabButton(sb, loneLandsIcon, loneLandsRect, BaseCheckpoint.LocationType.LoneLands, 6);
                DrawTabButton(sb, otherLandsIcon, otherLandsRect, BaseCheckpoint.LocationType.OtherLands, 7);

                sb.Draw(bg, position, Color.White);
                sb.DrawString(largeFont, "Flame Warper", new Vector2(position.X + bg.Width / 2, position.Y + 13), "Flame Warping".LineCenter(largeFont), ColorHelper.UI_Gold, 1f, 1f);

                DrawTabName(sb, currentTab);

                DrawWindowButtons(sb);

                sb.End();

                scissorGrid = new Rectangle((int)position.X + 15, (int)position.Y + 56, 272, 313);
                scissorPane = new Rectangle((int)position.X + 292, (int)position.Y + 50, (int)paneWidth, pane.Height);

                //List drawing
                DrawInside(sb, scissorGrid, () =>
                {
                    for (int i = 0; i < currentSoulgates.Count; i++)
                    {
                        if (currentSoulgates[i].IsMemorized == true)
                        {
                            if (currentSoulgates[i].IsSelected == true)
                            {
                                sb.Draw(longButtonHover, currentSoulgates[i].RectUI, memorizedColor);
                                sb.DrawString(font, currentSoulgates[i].Name, currentSoulgates[i].RectUI.Center.ToVector2(), currentSoulgates[i].Name.LineCenter(font), memorizedColor, 1f);
                            }
                            else
                            {
                                sb.Draw(longButton, currentSoulgates[i].RectUI, memorizedColor);
                                sb.DrawString(font, currentSoulgates[i].Name, currentSoulgates[i].RectUI.Center.ToVector2(), currentSoulgates[i].Name.LineCenter(font), Color.White, 1f);
                            }
                        }
                        else
                        {
                            if (currentSoulgates[i].IsSelected == true)
                            {
                                sb.Draw(longButtonHover, currentSoulgates[i].RectUI, Color.White);
                                sb.DrawString(font, currentSoulgates[i].Name, currentSoulgates[i].RectUI.Center.ToVector2(), currentSoulgates[i].Name.LineCenter(font), ColorHelper.UI_Gold, 1f);
                            }
                            else
                            {
                                sb.Draw(longButton, currentSoulgates[i].RectUI, Color.White);
                                sb.DrawString(font, currentSoulgates[i].Name, currentSoulgates[i].RectUI.Center.ToVector2(), currentSoulgates[i].Name.LineCenter(font), Color.White, 1f);
                            }
                        }
                    }
                });

                //Pane drawing
                DrawInside(sb, scissorPane, () =>
                {
                    if (selectedGate != null && paneWidth >= 5)
                    {
                        if (selectedGate.WarpHeader != null)
                            sb.Draw(selectedGate.WarpHeader, new Rectangle((int)position.X + 292, (int)position.Y + 62, 190, 80), Color.White);

                        sb.DrawString(font, selectedGate.Description.WrapText(font, 178), new Vector2(position.X + 300, position.Y + 147), Color.LightGray);

                        warpToButton.DrawButton(sb, Color.White);
                        memorizeButton.DrawButton(sb, Color.White);

                        sb.DrawString(font, "Flame Warp", warpToButton.ButtonRectangle.Center.ToVector2(), "Flame Warp".LineCenter(font), Color.White, 1f);
                        sb.DrawString(font, "Memorize", memorizeButton.ButtonRectangle.Center.ToVector2(), "Memorize".LineCenter(font), Color.White, 1f);
                    }
                });
            }
        }
        private void DrawInside(SpriteBatch sb, Rectangle scissorRect, Action contents)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorRect;

            contents.Invoke();

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }
        private void DrawTabName(SpriteBatch sb, BaseCheckpoint.LocationType type)
        {
            switch(type)
            {
                case BaseCheckpoint.LocationType.SouthernLands: sb.DrawString(largeFont, "The Southern Lands", new Vector2(position.X + bg.Width / 2, position.Y + 40), "The Southern Lands".LineCenter(largeFont), Color.White, 1f, 1f); break;
                case BaseCheckpoint.LocationType.Sloughland: sb.DrawString(largeFont, "The Sloughland", new Vector2(position.X + bg.Width / 2, position.Y + 40), "The Sloughland".LineCenter(largeFont), Color.White, 1f, 1f); break;
                case BaseCheckpoint.LocationType.Stormridge: sb.DrawString(largeFont, "The Stormridge", new Vector2(position.X + bg.Width / 2, position.Y + 40), "The Stormridge".LineCenter(largeFont), Color.White, 1f, 1f); break;
                case BaseCheckpoint.LocationType.JaggedSeaboard: sb.DrawString(largeFont, "Jagged Seaboard", new Vector2(position.X + bg.Width / 2, position.Y + 40), "Jagged Seaboard".LineCenter(largeFont), Color.White, 1f, 1f); break;
                case BaseCheckpoint.LocationType.Cultures: sb.DrawString(largeFont, "Central Lands", new Vector2(position.X + bg.Width / 2, position.Y + 40), "Central Lands".LineCenter(largeFont), Color.White, 1f, 1f); break;
                case BaseCheckpoint.LocationType.LoneLands: sb.DrawString(largeFont, "The Lone Lands", new Vector2(position.X + bg.Width / 2, position.Y + 40), "The Lone Lands".LineCenter(largeFont), Color.White, 1f, 1f); break;
                case BaseCheckpoint.LocationType.OtherLands: sb.DrawString(largeFont, "Other Lands", new Vector2(position.X + bg.Width / 2, position.Y + 40), "Other Lands".LineCenter(largeFont), Color.White, 1f, 1f); break;
            }
        }

        private Color halfTransparent = Color.Lerp(Color.White, Color.Transparent, .5f);
        private void DrawTabButton(SpriteBatch sb, Texture2D icon, Rectangle rect, BaseCheckpoint.LocationType type, int index)
        {
            if (currentTab == type)
            {
                sb.Draw(tabSelected, new Rectangle(rect.X - 4, rect.Y, rect.Width, rect.Height), Color.White);
                sb.Draw(icon, new Rectangle(rect.X - 4, rect.Y, rect.Width, rect.Height), Color.White);
            }
            else
            {
                sb.Draw(tab, rect, Color.White);
                sb.Draw(icon, rect, halfTransparent);
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

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return isDragging || clickRect.Contains(controls.MousePosition) || isTabHover ||
                hideRect.Contains(controls.MousePosition) || hintRect.Contains(controls.MousePosition);
            else
                return false;
        }
        public void ResetPosition()
        {
            Position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);
        }
    }
}
