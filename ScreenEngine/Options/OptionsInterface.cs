using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pilgrimage_Of_Embers.Extensions;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Various;

namespace Pilgrimage_Of_Embers.ScreenEngine.Options
{
    public class OptionsInterface
    {
        private Vector2 position;
        private Vector2 Position { set { position = new Vector2(MathHelper.Clamp(value.X, 42, GameSettings.VectorResolution.X - bg.Width), MathHelper.Clamp(value.Y, 0, GameSettings.VectorResolution.Y - bg.Height)); } }
        public Vector2 ExternalPosition { set { position = value; } }
        private float tabPosition = 0;

        public void ResetPosition()
        {
            position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);

            if (changeResolution != null)
                changeResolution.ResetPosition();
        }

        private Rectangle dragArea;

        private Texture2D bg, tabButton, tabButtonSelect, smallButton, button, longButton, smallButtonHover, buttonHover, longButtonHover, displayIcon, audioIcon, controlsIcon;
        private SpriteFont font, largeFont;

        public bool IsActive { get; set; }
        private enum OptionsTab { Display, Audio, Controls, GameStats }
        private OptionsTab options = OptionsTab.Display;

        private Rectangle displayTabRect, audioTabRect, controlsTabRect, statsTabRect;

        MenuButton applyChanges, discardChanges, defaultSettings;

        // --- Display Stuff ---
        SelectionBox changeResolution;

        MenuButton changeResolutionButton, setFullscreen, displayFPS, displayCursor, enableVSync, limitParticles, enableParticleInteraction, limitShaders, hideHUD, useWidescreen;

        PercentageBar gammaLevel, cameraShake, nonessentialObjects;

        // --- Audio Stuff ---
        PercentageBar masterVolumePct, soundVolumePct, ambienceVolumePct, musicVolumePct;

        // --- Controls Stuff ---

        // Character movement
        MenuButton moveUp, moveDown, moveLeft, moveRight, changeActivation, quickSlot, jump, roll, sneak, sprint;
        MenuButton talk, takeScreenshot, adjustQSLeft, adjustQSRight, adjustQSButtonLeft, adjustQSButtonRight;
        MenuButton useSelectedSoul, scrollSouls, scrollSpells, assignSwapPrimary, assignSwapOffhand, assignSwapAmmo;
        MenuButton openInventory, openMagic, openSouls, openRumors, openStats, openSettings, pause;

        MenuButton companionCommand0, companionCommand1, companionCommand2, companionCommand3, companionCommand4,
                   companionCommand5, companionCommand6, companionCommand7, companionCommand8, companionCommand9;

        private Controls controls = new Controls();

        private ScreenManager screens;
        private Main.MainScreen main;
        private Game game;
        private GraphicsDeviceManager graphics;
        private WorldManager worldManager;
        private TileEngine.TileMap map;

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        public OptionsInterface(Game Game, GraphicsDeviceManager Graphics, WorldManager World) { game = Game; graphics = Graphics; worldManager = World; }
        public void SetReferences(ScreenManager screens, Main.MainScreen main, TileEngine.TileMap map)
        {
            this.screens = screens;
            this.main = main;
            this.map = map;

            IsActive = false;
        }

        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            bg = cm.Load<Texture2D>("Interface/Global/tabBG");
            tabButton = cm.Load<Texture2D>("Interface/Global/tabButton");
            tabButtonSelect = cm.Load<Texture2D>("Interface/Global/tabButtonSelect");


            smallButton = cm.Load<Texture2D>("Interface/Global/smallButton");
            smallButtonHover = cm.Load<Texture2D>("Interface/Global/smallButtonHover");

            button = cm.Load<Texture2D>("Interface/Global/button");
            buttonHover = cm.Load<Texture2D>("Interface/Global/buttonHover");

            longButton = cm.Load<Texture2D>("Interface/Global/longButton");
            longButtonHover = cm.Load<Texture2D>("Interface/Global/longButtonHover");

            displayIcon = cm.Load<Texture2D>("Interface/Various/Settings/displayIcon");
            audioIcon = cm.Load<Texture2D>("Interface/Various/Settings/audioIcon");
            controlsIcon = cm.Load<Texture2D>("Interface/Various/Settings/controlsIcon");


            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            position = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);
            mouseDragOffset = new Vector2(248, 12);

            //All tabs

            applyChanges = new MenuButton(Vector2.Zero, button, button, buttonHover, 1f);
            discardChanges = new MenuButton(Vector2.Zero, button, button, buttonHover, 1f);
            defaultSettings = new MenuButton(Vector2.Zero, button, button, buttonHover, 1f);

            LoadDisplay(cm);
            LoadSound(cm);
            LoadControls(cm);

            tabPosition = 0f;
            longBounds = -110f;
            shortbounds = 20f;
        }

        private void LoadDisplay(ContentManager cm)
        {
            changeResolutionButton = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            changeResolution = new SelectionBox("Resolution", "Change the screen's resolution", "1024 x 768", "1280 x 800", "1280 x 1024", "1366 x 768",
                                                                                                "1440 x 900", "1600 x 900", "1920 x 1080", "1920 x 1200", "Auto-detect", "Cancel");
            changeResolution.Load(cm);
            newResolution = GameSettings.WindowResolution;

            setFullscreen = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            displayFPS = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            displayCursor = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            enableVSync = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            limitParticles = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            enableParticleInteraction = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            limitShaders = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            //dataCollection = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            hideHUD = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            useWidescreen = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            gammaLevel = new PercentageBar(cm.Load<Texture2D>("Interface/Global/percentageBar"), 4, 200, ColorHelper.UI_Gold, "Determines the gamma. Default is 50%.");
            gammaLevel.Load(cm);
            gammaLevel.Percentage = GameSettings.Gamma;

            cameraShake = new PercentageBar(cm.Load<Texture2D>("Interface/Global/percentageBar"), 4, 200, ColorHelper.UI_Gold, "How much screen shake to apply. Default is 100%, 0% turns off.");
            cameraShake.Load(cm);
            cameraShake.Percentage = GameSettings.ShakeMultiplier;

            nonessentialObjects = new PercentageBar(cm.Load<Texture2D>("Interface/Global/percentageBar"), 4, 200, ColorHelper.UI_Gold, "Percentage of non-essential objects to disable. A performance tweak. Default is 0%.");
            nonessentialObjects.Load(cm);
            nonessentialObjects.Percentage = GameSettings.HiddenNEOsPct;
        }
        private void LoadSound(ContentManager cm)
        {
            masterVolumePct = new PercentageBar(cm.Load<Texture2D>("Interface/Global/percentageBar"), 4, 200, ColorHelper.UI_Gold, string.Empty);
            soundVolumePct = new PercentageBar(cm.Load<Texture2D>("Interface/Global/percentageBar"), 4, 200, ColorHelper.UI_Gold, string.Empty);
            ambienceVolumePct = new PercentageBar(cm.Load<Texture2D>("Interface/Global/percentageBar"), 4, 200, ColorHelper.UI_Gold, string.Empty);
            musicVolumePct = new PercentageBar(cm.Load<Texture2D>("Interface/Global/percentageBar"), 4, 200, ColorHelper.UI_Gold, string.Empty);

            masterVolumePct.Percentage = GameSettings.MasterVolume;
            soundVolumePct.Percentage = GameSettings.SoundVolume;
            ambienceVolumePct.Percentage = GameSettings.AmbienceVolume;
            musicVolumePct.Percentage = GameSettings.MusicVolume;

            masterVolumePct.Load(cm);
            soundVolumePct.Load(cm);
            ambienceVolumePct.Load(cm);
            musicVolumePct.Load(cm);
        }
        private void LoadControls(ContentManager cm)
        {
            moveUp = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            moveDown = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            moveLeft = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            moveRight = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            changeActivation = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            quickSlot = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            jump = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            roll = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            sneak = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            sprint = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            //Misc
            talk = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            takeScreenshot = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            adjustQSLeft = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            adjustQSRight = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            adjustQSButtonLeft = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            adjustQSButtonRight = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            //Combat-related
            useSelectedSoul = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            scrollSouls = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            scrollSpells = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            assignSwapPrimary = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            assignSwapOffhand = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            assignSwapAmmo = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            //Interface Shortcuts
            openInventory = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            openMagic = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            openSouls = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            openRumors = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            openStats = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            openSettings = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            pause = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);

            //Companion Commands
            companionCommand0 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand1 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand2 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand3 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand4 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand5 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand6 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand7 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand8 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
            companionCommand9 = new MenuButton(Vector2.Zero, longButton, longButton, longButtonHover, 1f);
        }

        private string hints = "Options Tips:\n\n" +
            "Be sure to hit the \"Apply & Save\" button before closing,\n" +
            "otherwise your unsaved changes will be lost.";

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenSettings))
            {
                IsActive = !IsActive;

                if (IsActive == true)
                    Logger.AppendLine("Opened options UI");
            }

            if (IsActive == true)
            {
                clickCheck = new Rectangle((int)position.X + 15, (int)position.Y + 20, 466, 510);

                CheckDragScreen();
                ScrollGrid(gt, 50f, 500f, 300f, 10f);

                UpdateTabClicking(gt);
                UpdateApplyButtons(gt);

                switch (options)
                {
                    case OptionsTab.Display: UpdateDisplay(gt); break;
                    case OptionsTab.Audio: UpdateAudio(gt); break;
                    case OptionsTab.Controls: UpdateControls(gt); break;
                    case OptionsTab.GameStats: break;
                }

                hintRect = new Rectangle((int)position.X + 393, (int)position.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)position.X + 420, (int)position.Y, windowButton.Width - 20, windowButton.Height);

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

                    if (main.IsActive == false)
                        ToolTip.RequestStringAssign("Hide Options");
                    else
                        ToolTip.RequestStringAssign("Return");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Button Click 1");

                        if (main.IsActive == false)
                            IsActive = false;
                        else
                            main.GoToMainScreen();
                    }
                }
                else
                    isHideHover = false;
            }

            controls.UpdateLast();
        }

        private bool isTabHover = false;
        
        float scrollValue = 0f, scrollVelocity = 0f, longBounds = 0f, shortbounds = 0f;
        private void ScrollGrid(GameTime gt, float scrollSpeed, float maxScrollSpeed, float scrollSlowdown, float clampSpeed)
        {
            if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                scrollVelocity -= scrollSpeed;
            else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                scrollVelocity += scrollSpeed;

            scrollValue = controls.CurrentMS.ScrollWheelValue;

            //Smooth scrolling code
            tabPosition += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -maxScrollSpeed, maxScrollSpeed);

            if (scrollVelocity > clampSpeed)
                scrollVelocity -= scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -clampSpeed)
                scrollVelocity += scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -clampSpeed && scrollVelocity < clampSpeed)
                scrollVelocity = 0f;

            if (tabPosition > shortbounds)
                scrollVelocity = 0f;
            if (tabPosition < longBounds)
                scrollVelocity = 0f;

            tabPosition = MathHelper.Clamp(tabPosition, longBounds, shortbounds);
        }
        private void UpdateApplyButtons(GameTime gt)
        {
            applyChanges.Update(gt, controls);
            discardChanges.Update(gt, controls);
            defaultSettings.Update(gt, controls);

            applyChanges.Position = new Point((int)position.X + 16, (int)position.Y + 470);
            discardChanges.Position = new Point((int)position.X + 171, (int)position.Y + 470);
            defaultSettings.Position = new Point((int)position.X + 326, (int)position.Y + 470);

            if (applyChanges.IsHover == true)
                ToolTip.RequestStringAssign("Apply and save changes.");
            if (discardChanges.IsHover == true)
                ToolTip.RequestStringAssign("Discard new changes.");
            if (defaultSettings.IsHover == true)
                ToolTip.RequestStringAssign("Reset tab to defaults.");
        }
        private void UpdateTabClicking(GameTime gt)
        {
            displayTabRect = new Rectangle((int)position.X - 32, (int)position.Y + 35, tabButton.Width, tabButton.Height);
            audioTabRect = new Rectangle((int)position.X - 32, (int)position.Y + 83, tabButton.Width, tabButton.Height);
            controlsTabRect = new Rectangle((int)position.X - 32, (int)position.Y + 131, tabButton.Width, tabButton.Height);
            statsTabRect = new Rectangle((int)position.X - 32, (int)position.Y + 179, tabButton.Width, tabButton.Height);

            isTabHover = false;

            if (displayTabRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    options = OptionsTab.Display;
                    screens.PlaySound("Button Click 2");

                    tabPosition = 0f;
                    longBounds = -110f;
                    shortbounds = 20f;
                }

                isTabHover = true;
            }
            if (audioTabRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    options = OptionsTab.Audio;
                    screens.PlaySound("Button Click 2");

                    tabPosition = 0f;
                    longBounds = 0f;
                    shortbounds = 0f;
                }

                isTabHover = true;
            }
            if (controlsTabRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    options = OptionsTab.Controls;
                    screens.PlaySound("Button Click 2");

                    tabPosition = 0f;
                    longBounds = -1030f;
                    shortbounds = 20f;
                }

                isTabHover = true;
            }
            if (statsTabRect.Contains(controls.MousePosition))
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    options = OptionsTab.GameStats;
                    screens.PlaySound("Button Click 2");

                    tabPosition = 0f;
                    longBounds = 0f;
                    shortbounds = 0f;
                }

                isTabHover = true;
            }
        }

        private Point newResolution; bool isDisplayingCursor = true;
        private void UpdateDisplay(GameTime gt)
        {
            changeResolutionButton.Position = new Point((int)position.X + 40, (int)position.Y + 100 + (int)tabPosition);
            changeResolutionButton.Update(gt, controls);

            setFullscreen.Position = new Point((int)position.X + 265, (int)position.Y + 100 + (int)tabPosition);
            setFullscreen.Update(gt, controls);

            displayFPS.Position = new Point((int)position.X + 40, (int)position.Y + 160 + (int)tabPosition);
            displayFPS.Update(gt, controls);

            displayCursor.Position = new Point((int)position.X + 265, (int)position.Y + 160 + (int)tabPosition);
            displayCursor.Update(gt, controls);

            enableVSync.Position = new Point((int)position.X + 40, (int)position.Y + 220 + (int)tabPosition);
            enableVSync.Update(gt, controls);

            limitParticles.Position = new Point((int)position.X + 265, (int)position.Y + 220 + (int)tabPosition);
            limitParticles.Update(gt, controls);

            enableParticleInteraction.Position = new Point((int)position.X + 40, (int)position.Y + 280 + (int)tabPosition);
            enableParticleInteraction.Update(gt, controls);

            limitShaders.Position = new Point((int)position.X + 265, (int)position.Y + 280 + (int)tabPosition);
            limitShaders.Update(gt, controls);

            //dataCollection.Position = new Point((int)position.X + 40, (int)position.Y + 340);
            //dataCollection.Update(gt, controls);

            hideHUD.Position = new Point((int)position.X + 40, (int)position.Y + 340 + (int)tabPosition);
            hideHUD.Update(gt, controls);

            useWidescreen.Position = new Point((int)position.X + 265, (int)position.Y + 340 + (int)tabPosition);
            useWidescreen.Update(gt, controls);

            gammaLevel.Position = position + new Vector2(bg.Center().X - 104, 400 + (int)tabPosition);
            gammaLevel.Update(controls.MousePosition, controls.CurrentMS.LeftButton == ButtonState.Pressed, 4);

            cameraShake.Position = position + new Vector2(bg.Center().X - 104, 460 + (int)tabPosition);
            cameraShake.Update(controls.MousePosition, controls.CurrentMS.LeftButton == ButtonState.Pressed, 4);

            nonessentialObjects.Position = position + new Vector2(bg.Center().X - 104, 520 + (int)tabPosition);
            nonessentialObjects.Update(controls.MousePosition, controls.CurrentMS.LeftButton == ButtonState.Pressed, 4);

            if (changeResolution.IsActive == true)
            {
                changeResolution.Update(gt);

                if (changeResolution.CurrentSelection == 0)
                {
                    newResolution = new Point(1024, 768);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 1)
                {
                    newResolution = new Point(1280, 800);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 2)
                {
                    newResolution = new Point(1280, 1024);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 3)
                {
                    newResolution = new Point(1366, 768);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 4)
                {
                    newResolution = new Point(1440, 900);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 5)
                {
                    newResolution = new Point(1600, 900);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 6)
                {
                    newResolution = new Point(1920, 1080);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 7)
                {
                    newResolution = new Point(1920, 1200);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 8)
                {
                    newResolution = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                    changeResolution.IsActive = false;
                }
                if (changeResolution.CurrentSelection == 9) //Cancel button
                    changeResolution.IsActive = false;
            }
            
            if (changeResolution.IsActive == false)
            {
                if (changeResolutionButton.IsHover == true)
                    ToolTip.RequestStringAssign("Set the game window size in pixels. Recommended value is \"Auto-detect\".");

                if (setFullscreen.IsHover == true)
                    ToolTip.RequestStringAssign("Sets the game to fullscreen mode. Recommended value is enabled.");

                if (displayFPS.IsHover == true)
                    ToolTip.RequestStringAssign("Displays the frames per second count in the top right corner. Default value is disabled.");

                if (displayCursor.IsHover == true)
                    ToolTip.RequestStringAssign("When enabled, the game cursor will be displayed and your default mouse cursor will not. Default is enabled.");

                if (enableVSync.IsHover == true)
                    ToolTip.RequestStringAssign("VSync will cap the framerate at your monitor's refresh rate. Default is disabled.");

                if (limitParticles.IsHover == true)
                    ToolTip.RequestStringAssign("Limits the total particle count that can be displayed. Default is disabled.");

                if (enableParticleInteraction.IsHover == true)
                    ToolTip.RequestStringAssign("Determines if characters and monsters can interact with particles.\n For performance: disable.\nFor Quality: enable.");

                if (limitShaders.IsHover == true)
                    ToolTip.RequestStringAssign("Limits the shaders than can be active.\n For performace: disable.\nFor Quality: enable.");

                //if (dataCollection.IsHover == true)
                //    ToolTip.RequestStringAssign("Game-specific anonymous stat collecting is not implemented, but it will be completely optional when it is!");

                if (hideHUD.IsHover == true)
                    ToolTip.RequestStringAssign("Hide the \"Heads Up Display\", which includes the meters and equipped items. Default is disabled.");

                if (useWidescreen.IsHover == true)
                    ToolTip.RequestStringAssign("Allows black bars at both vertical ends to display. Default is enabled.");

                if (changeResolutionButton.IsLeftClicked == true)
                    changeResolution.IsActive = !changeResolution.IsActive;

                if (setFullscreen.IsLeftClicked == true)
                    GameSettings.IsFullScreen = !GameSettings.IsFullScreen;

                if (displayFPS.IsLeftClicked == true)
                    GameSettings.DisplayFPS = !GameSettings.DisplayFPS;

                if (displayCursor.IsLeftClicked == true)
                    isDisplayingCursor = !isDisplayingCursor;

                if (enableVSync.IsLeftClicked == true)
                    GameSettings.IsVSync = !GameSettings.IsVSync;

                if (limitParticles.IsLeftClicked == true)
                    GameSettings.LimitParticles = !GameSettings.LimitParticles;

                if (enableParticleInteraction.IsLeftClicked == true)
                    GameSettings.ParticleInteraction = !GameSettings.ParticleInteraction;

                if (limitShaders.IsLeftClicked == true)
                    GameSettings.LimitShaders = !GameSettings.LimitShaders;

                if (hideHUD.IsLeftClicked == true)
                    GameSettings.IsHidingHUD = !GameSettings.IsHidingHUD;

                if (useWidescreen.IsLeftClicked == true)
                    GameSettings.IsUseWidescreen = !GameSettings.IsUseWidescreen;

                GameSettings.Gamma = gammaLevel.Percentage;
                GameSettings.ShakeMultiplier = cameraShake.Percentage;

                //Custom button stuff
                if (applyChanges.IsLeftClicked == true)
                    ApplyChanges();
            }
        }
        private void ApplyChanges()
        {
            //Reset UI positions only if the resolution has changed.
            if (newResolution != GameSettings.WindowResolution)
                screens.ResetUIPositions();

            GameSettings.AssignResolution(newResolution.X, newResolution.Y);
            GameSettings.DisplayCursor = isDisplayingCursor;
            GameSettings.HiddenNEOsPct = nonessentialObjects.Percentage;

            GameSettings.ApplyChanges(worldManager, graphics, game);
            GameSettings.ReadData();

            map.DisableUnimportantObjects(GameSettings.HiddenNEOsPct);
        }

        private void UpdateAudio(GameTime gt)
        {
            masterVolumePct.Position = position + new Vector2(bg.Center().X - 104, 100);
            masterVolumePct.Update(controls.MousePosition, controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed, 3);

            soundVolumePct.Position = position + new Vector2(bg.Center().X - 104, 150);
            soundVolumePct.Update(controls.MousePosition, controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed, 3);

            ambienceVolumePct.Position = position + new Vector2(bg.Center().X - 104, 200);
            ambienceVolumePct.Update(controls.MousePosition, controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed, 3);

            musicVolumePct.Position = position + new Vector2(bg.Center().X - 104, 250);
            musicVolumePct.Update(controls.MousePosition, controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed, 3);

            GameSettings.MasterVolume = masterVolumePct.Percentage;
            GameSettings.SoundVolume = soundVolumePct.Percentage;
            GameSettings.AmbienceVolume = ambienceVolumePct.Percentage;
            GameSettings.MusicVolume = musicVolumePct.Percentage;

            if (applyChanges.IsLeftClicked == true)
                GameSettings.SaveData();
        }

        private bool isCheckingForKey = false;
        private void UpdateControls(GameTime gt)
        {
            //Character Movement
            moveUp.Position = new Point((int)position.X + 275, (int)position.Y + 110 + (int)tabPosition);
            moveDown.Position = new Point((int)position.X + 275, (int)position.Y + 140 + (int)tabPosition);
            moveLeft.Position = new Point((int)position.X + 275, (int)position.Y + 170 + (int)tabPosition);
            moveRight.Position = new Point((int)position.X + 275, (int)position.Y + 200 + (int)tabPosition);
            changeActivation.Position = new Point((int)position.X + 275, (int)position.Y + 230 + (int)tabPosition);
            quickSlot.Position = new Point((int)position.X + 275, (int)position.Y + 260 + (int)tabPosition);
            jump.Position = new Point((int)position.X + 275, (int)position.Y + 290 + (int)tabPosition);
            roll.Position = new Point((int)position.X + 275, (int)position.Y + 320 + (int)tabPosition);
            sneak.Position = new Point((int)position.X + 275, (int)position.Y + 350 + (int)tabPosition);
            sprint.Position = new Point((int)position.X + 275, (int)position.Y + 380 + (int)tabPosition);

            //Combat-related Controls
            useSelectedSoul.Position = new Point((int)position.X + 275, (int)position.Y + 450 + (int)tabPosition);
            scrollSouls.Position = new Point((int)position.X + 275, (int)position.Y + 480 + (int)tabPosition);
            scrollSpells.Position = new Point((int)position.X + 275, (int)position.Y + 510 + (int)tabPosition);
            assignSwapPrimary.Position = new Point((int)position.X + 275, (int)position.Y + 570 + (int)tabPosition);
            assignSwapOffhand.Position = new Point((int)position.X + 275, (int)position.Y + 600 + (int)tabPosition);
            assignSwapAmmo.Position = new Point((int)position.X + 275, (int)position.Y + 630 + (int)tabPosition);

            //Interface Shortcuts
            openInventory.Position = new Point((int)position.X + 275, (int)position.Y + 700 + (int)tabPosition);
            openMagic.Position = new Point((int)position.X + 275, (int)position.Y + 730 + (int)tabPosition);
            openSouls.Position = new Point((int)position.X + 275, (int)position.Y + 760 + (int)tabPosition);
            openRumors.Position = new Point((int)position.X + 275, (int)position.Y + 790 + (int)tabPosition);
            openStats.Position = new Point((int)position.X + 275, (int)position.Y + 820 + (int)tabPosition);
            openSettings.Position = new Point((int)position.X + 275, (int)position.Y + 850 + (int)tabPosition);
            pause.Position = new Point((int)position.X + 275, (int)position.Y + 880 + (int)tabPosition);

            //Other controls
            talk.Position = new Point((int)position.X + 275, (int)position.Y + 950 + (int)tabPosition);
            takeScreenshot.Position = new Point((int)position.X + 275, (int)position.Y + 980 + (int)tabPosition);
            adjustQSLeft.Position = new Point((int)position.X + 275, (int)position.Y + 1010 + (int)tabPosition);
            adjustQSRight.Position = new Point((int)position.X + 275, (int)position.Y + 1040 + (int)tabPosition);
            adjustQSButtonLeft.Position = new Point((int)position.X + 275, (int)position.Y + 1070 + (int)tabPosition);
            adjustQSButtonRight.Position = new Point((int)position.X + 275, (int)position.Y + 1100 + (int)tabPosition);

            //Companion Commands
            companionCommand0.Position = new Point((int)position.X + 275, (int)position.Y + 1170 + (int)tabPosition);
            companionCommand1.Position = new Point((int)position.X + 275, (int)position.Y + 1200 + (int)tabPosition);
            companionCommand2.Position = new Point((int)position.X + 275, (int)position.Y + 1230 + (int)tabPosition);
            companionCommand3.Position = new Point((int)position.X + 275, (int)position.Y + 1260 + (int)tabPosition);
            companionCommand4.Position = new Point((int)position.X + 275, (int)position.Y + 1290 + (int)tabPosition);
            companionCommand5.Position = new Point((int)position.X + 275, (int)position.Y + 1320 + (int)tabPosition);
            companionCommand6.Position = new Point((int)position.X + 275, (int)position.Y + 1350 + (int)tabPosition);
            companionCommand7.Position = new Point((int)position.X + 275, (int)position.Y + 1380 + (int)tabPosition);
            companionCommand8.Position = new Point((int)position.X + 275, (int)position.Y + 1410 + (int)tabPosition);
            companionCommand9.Position = new Point((int)position.X + 275, (int)position.Y + 1440 + (int)tabPosition);

            if (isCheckingForKey == true)
                AssignKey();
            if (isCheckingForKey == false)
            {
                CheckControlButton(moveUp, ControlKeys.KeyEnum.MoveUp, gt);
                CheckControlButton(moveDown, ControlKeys.KeyEnum.MoveDown, gt);
                CheckControlButton(moveLeft, ControlKeys.KeyEnum.MoveLeft, gt);
                CheckControlButton(moveRight, ControlKeys.KeyEnum.MoveRight, gt);
                CheckControlButton(changeActivation, ControlKeys.KeyEnum.Activate, gt);
                CheckControlButton(quickSlot, ControlKeys.KeyEnum.Quickslot, gt);
                CheckControlButton(jump, ControlKeys.KeyEnum.Jump, gt);
                CheckControlButton(roll, ControlKeys.KeyEnum.Roll, gt);
                CheckControlButton(sneak, ControlKeys.KeyEnum.Sneak, gt);
                CheckControlButton(sprint, ControlKeys.KeyEnum.Sprint, gt);

                CheckControlButton(talk, ControlKeys.KeyEnum.TypeMessage, gt);
                CheckControlButton(takeScreenshot, ControlKeys.KeyEnum.TakeScreenshot, gt);
                CheckControlButton(adjustQSLeft, ControlKeys.KeyEnum.LoopQSLeft, gt);
                CheckControlButton(adjustQSRight, ControlKeys.KeyEnum.LoopQSRight, gt);
                CheckControlButton(adjustQSButtonLeft, ControlKeys.KeyEnum.ButtonTargetLeft, gt);
                CheckControlButton(adjustQSButtonRight, ControlKeys.KeyEnum.ButtonTargetRight, gt);

                CheckControlButton(useSelectedSoul, ControlKeys.KeyEnum.UseSelectedSoul, gt);
                CheckControlButton(scrollSouls, ControlKeys.KeyEnum.ScrollSouls, gt);
                CheckControlButton(scrollSpells, ControlKeys.KeyEnum.ScrollSpells, gt);
                CheckControlButton(assignSwapPrimary, ControlKeys.KeyEnum.SwapPrimary, gt);
                CheckControlButton(assignSwapOffhand, ControlKeys.KeyEnum.SwapOffhand, gt);
                CheckControlButton(assignSwapAmmo, ControlKeys.KeyEnum.SwapAmmo, gt);

                CheckControlButton(openInventory, ControlKeys.KeyEnum.OpenInventory, gt);
                CheckControlButton(openMagic, ControlKeys.KeyEnum.OpenMagic, gt);
                CheckControlButton(openSouls, ControlKeys.KeyEnum.OpenSouls, gt);
                CheckControlButton(openRumors, ControlKeys.KeyEnum.OpenRumors, gt);
                CheckControlButton(openStats, ControlKeys.KeyEnum.OpenStats, gt);
                CheckControlButton(openSettings, ControlKeys.KeyEnum.OpenSettings, gt);
                CheckControlButton(pause, ControlKeys.KeyEnum.Pause, gt);

                CheckControlButton(companionCommand0, ControlKeys.KeyEnum.Command0, gt);
                CheckControlButton(companionCommand1, ControlKeys.KeyEnum.Command1, gt);
                CheckControlButton(companionCommand2, ControlKeys.KeyEnum.Command2, gt);
                CheckControlButton(companionCommand3, ControlKeys.KeyEnum.Command3, gt);
                CheckControlButton(companionCommand4, ControlKeys.KeyEnum.Command4, gt);
                CheckControlButton(companionCommand5, ControlKeys.KeyEnum.Command5, gt);
                CheckControlButton(companionCommand6, ControlKeys.KeyEnum.Command6, gt);
                CheckControlButton(companionCommand7, ControlKeys.KeyEnum.Command7, gt);
                CheckControlButton(companionCommand8, ControlKeys.KeyEnum.Command8, gt);
                CheckControlButton(companionCommand9, ControlKeys.KeyEnum.Command9, gt);

                if (defaultSettings.IsLeftClicked == true)
                    controls.CurrentControls.SetDefaultControls();
                if (applyChanges.IsLeftClicked == true)
                    GameSettings.SaveData();
            }
        }
        private void CheckControlButton(MenuButton button, ControlKeys.KeyEnum key, GameTime gt)
        {
            button.Update(gt, controls);

            if (button.IsLeftClicked == true)
            {
                assignKey = key;
                isCheckingForKey = true;
            }
        }
        private void AssignKey()
        {
            Keys newKey = controls.GetPressedKey();

            if (newKey != Keys.None)
            {
                controls.CurrentControls.SetControl(assignKey, newKey);
                isCheckingForKey = false;
            }
        }
        private ControlKeys.KeyEnum assignKey;

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private void CheckDragScreen()
        {
            dragArea = new Rectangle((int)position.X + 114, (int)position.Y, 268, 20);

            if (main.IsActive == false)
            {
                if (dragArea.Contains(controls.MousePosition))
                    screens.SetCursorState(Cursor.CursorState.Moving);

                if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    isDragging = true;
                else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    isDragging = false;
            }

            if (isDragging == true)
            {
                Position = controls.MouseVector - mouseDragOffset;
                screens.SetCursorState(Cursor.CursorState.Move);
            }
        }

        private Rectangle scissorGrid, clickCheck;
        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        private Texture2D pixel; bool isAssigned = false;

        public void Draw(SpriteBatch sb)
        {
            if (isAssigned == false)
                pixel = TextureHelper.CreatePixel(sb, ref isAssigned);

            if (IsActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                DrawTab(sb, OptionsTab.Display, displayIcon, 0);
                DrawTab(sb, OptionsTab.Audio, audioIcon, 1);
                DrawTab(sb, OptionsTab.Controls, controlsIcon, 2);
                //DrawTab(sb, OptionsTab.Stats, 3);

                sb.Draw(bg, position, Color.White);

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

                sb.DrawString(largeFont, "Settings", position + new Vector2(bg.Width / 2, 12), "Settings".LineCenter(largeFont), ColorHelper.UI_Gold, 1f);
                sb.DrawString(largeFont, options.ToString(), position + new Vector2(bg.Width / 2, 50), options.ToString().LineCenter(largeFont), Color.White, 1f);

                applyChanges.DrawButton(sb, Color.White);
                discardChanges.DrawButton(sb, Color.White);
                defaultSettings.DrawButton(sb, Color.White);

                sb.DrawString(font, "Apply & Save", applyChanges.Center + button.Center(), "Apply & Save".LineCenter(font), Color.Wheat, 1f);
                sb.DrawString(font, "Discard Changes", discardChanges.Center + button.Center(), "Discard Changes".LineCenter(font), Color.Wheat, 1f);
                sb.DrawString(font, "Restore Default", defaultSettings.Center + button.Center(), "Restore Default".LineCenter(font), Color.Wheat, 1f);

                sb.End();

                DrawInside(sb);

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                if (options == OptionsTab.Display)
                    changeResolution.Draw(sb);

                if (options == OptionsTab.Controls)
                {
                    if (isCheckingForKey == true)
                    {
                        sb.DrawBoxBordered(pixel, new Rectangle((int)GameSettings.VectorCenter.X - 100, (int)GameSettings.VectorCenter.Y - 20, 200, 40), ColorHelper.Charcoal, ColorHelper.UI_Gold);
                        sb.DrawString(largeFont, "Awaiting Key Press...", GameSettings.VectorCenter + new Vector2(0, -10), "Awaiting Key Press...".LineCenter(largeFont), ColorHelper.UI_Gold, 1f);
                        sb.DrawString(font, "(" + assignKey.ToString() + ")", GameSettings.VectorCenter + new Vector2(0, 10), ("(" + assignKey.ToString() + ")").LineCenter(font), ColorHelper.UI_Gold, 1f);
                    }
                }

                sb.End();
            }
        }
        private void DrawTab(SpriteBatch sb, OptionsTab type, Texture2D icon, int index)
        {
            if (options == type)
            {
                sb.Draw(tabButtonSelect, position + new Vector2(10, 35 + (index * 48)), Color.White, new Vector2(48, 0), 0f, 1f, 1f);
                sb.Draw(icon, position + new Vector2(10, 35 + (index * 48)), Color.White, new Vector2(48, 0), 0f, 1f, 1f);
            }
            else
            {
                sb.Draw(tabButton, position + new Vector2(14, 35 + (index * 48)), Color.White, new Vector2(48, 0), 0f, 1f, 1f);
                sb.Draw(icon, position + new Vector2(14, 35 + (index * 48)), Color.Gray, new Vector2(48, 0), 0f, 1f, 1f);
            }
        }
        private void DrawInside(SpriteBatch sb)
        {
            scissorGrid = new Rectangle((int)position.X + 15, (int)position.Y + 71, 466, 379);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorGrid;

            switch (options)
            {
                case OptionsTab.Display: DrawDisplayTab(sb); break;
                case OptionsTab.Audio: DrawAudioTab(sb); break;
                case OptionsTab.Controls: DrawControlsTab(sb); break;
                case OptionsTab.GameStats: DrawStatsTab(sb); break;
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }

        private void DrawDisplayTab(SpriteBatch sb)
        {
            changeResolutionButton.DrawButton(sb, Color.White);
            setFullscreen.DrawButton(sb, Color.White);
            displayFPS.DrawButton(sb, Color.White);
            displayCursor.DrawButton(sb, Color.White);
            enableVSync.DrawButton(sb, Color.White);

            limitParticles.DrawButton(sb, Color.White);
            enableParticleInteraction.DrawButton(sb, Color.White);
            limitShaders.DrawButton(sb, Color.White);
            hideHUD.DrawButton(sb, Color.White);
            useWidescreen.DrawButton(sb, Color.White);


            //Change resolution text drawing
            sb.DrawString(font, "Resolution", changeResolutionButton.Center + new Vector2(longButton.Center().X, -10), "Resolution".LineCenter(font), Color.Wheat, 1f);

            if (changeResolutionButton.IsHover == false)
                sb.DrawString(font, newResolution.X + " x " + newResolution.Y, changeResolutionButton.Center + longButton.Center(), (newResolution.X + " x " + newResolution.Y).LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, newResolution.X + " x " + newResolution.Y, changeResolutionButton.Center + longButton.Center(), (newResolution.X + " x " + newResolution.Y).LineCenter(font), ColorHelper.UI_Gold, 1f);


            //Fullscreen text drawing
            sb.DrawString(font, "Fullscreen", setFullscreen.Center + new Vector2(longButton.Center().X, -10), "Fullscreen".LineCenter(font), Color.Wheat, 1f);

            string fullscreenText = GameSettings.IsFullScreen ? "Enabled" : "Disabled";
            if (setFullscreen.IsHover == false)
                sb.DrawString(font, fullscreenText, setFullscreen.Center + longButton.Center(), fullscreenText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, fullscreenText, setFullscreen.Center + longButton.Center(), fullscreenText.LineCenter(font), ColorHelper.UI_Gold, 1f);


            //Gamma bar and text drawing
            string gammaString = "Gamma (" + (int)(gammaLevel.Percentage * 100) + "%)";
            sb.DrawString(font, gammaString, new Vector2(position.X + (bg.Width / 2), position.Y + 390 + (int)tabPosition), gammaString.LineCenter(font), Color.Wheat, 1f);
            gammaLevel.Draw(sb, Color.Lerp(ColorHelper.UI_DarkerGold, ColorHelper.UI_LightGold, gammaLevel.Percentage));

            //Shake multiplier bar and text drawing
            string screenShake = "Camera Shake (" + (int)(cameraShake.Percentage * 100) + "%)";
            sb.DrawString(font, screenShake, new Vector2(position.X + (bg.Width / 2), position.Y + 450 + (int)tabPosition), screenShake.LineCenter(font), Color.Wheat, 1f);
            cameraShake.Draw(sb, Color.Lerp(ColorHelper.UI_DarkerGold, ColorHelper.UI_LightGold, cameraShake.Percentage));

            //Hidden NEO percentage bar and text drawing
            string hiddenNEOs = "Hidden Non-essential Objects (" + (int)(nonessentialObjects.Percentage * 100) + "%)";
            sb.DrawString(font, hiddenNEOs, new Vector2(position.X + (bg.Width / 2), position.Y + 510 + (int)tabPosition), hiddenNEOs.LineCenter(font), Color.Wheat, 1f);
            nonessentialObjects.Draw(sb, Color.Lerp(ColorHelper.UI_DarkerGold, ColorHelper.UI_LightGold, nonessentialObjects.Percentage));


            //FPS button text
            sb.DrawString(font, "Display FPS", displayFPS.Center + new Vector2(longButton.Center().X, -10), "Display FPS".LineCenter(font), Color.Wheat, 1f);

            string fpsText = GameSettings.DisplayFPS ? "Enabled" : "Disabled";
            if (displayFPS.IsHover == false)
                sb.DrawString(font, fpsText, displayFPS.Center + longButton.Center(), fpsText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, fpsText, displayFPS.Center + longButton.Center(), fpsText.LineCenter(font), ColorHelper.UI_Gold, 1f);


            //Cursor button text
            sb.DrawString(font, "Display Game Cursor", displayCursor.Center + new Vector2(longButton.Center().X, -10), "Display Game Cursor".LineCenter(font), Color.Wheat, 1f);

            string cursorText = isDisplayingCursor ? "Enabled" : "Disabled";
            if (displayCursor.IsHover == false)
                sb.DrawString(font, cursorText, displayCursor.Center + longButton.Center(), cursorText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, cursorText, displayCursor.Center + longButton.Center(), cursorText.LineCenter(font), ColorHelper.UI_Gold, 1f);


            //VSync button text
            sb.DrawString(font, "VSync", enableVSync.Center + new Vector2(longButton.Center().X, -10), "VSync".LineCenter(font), Color.Wheat, 1f);

            string vsyncText = GameSettings.IsVSync ? "Enabled" : "Disabled";
            if (enableVSync.IsHover == false)
                sb.DrawString(font, vsyncText, enableVSync.Center + longButton.Center(), vsyncText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, vsyncText, enableVSync.Center + longButton.Center(), vsyncText.LineCenter(font), ColorHelper.UI_Gold, 1f);


            //LimitParticles button text
            sb.DrawString(font, "Limit Particles", limitParticles.Center + new Vector2(longButton.Center().X, -10), "Limit Particles".LineCenter(font), Color.Wheat, 1f);

            string limitParticlesText = GameSettings.LimitParticles ? "Enabled" : "Disabled";
            if (limitParticles.IsHover == false)
                sb.DrawString(font, limitParticlesText, limitParticles.Center + longButton.Center(), limitParticlesText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, limitParticlesText, limitParticles.Center + longButton.Center(), limitParticlesText.LineCenter(font), ColorHelper.UI_Gold, 1f);


            //Particle interaction button text
            sb.DrawString(font, "Particle Interaction", enableParticleInteraction.Center + new Vector2(longButton.Center().X, -10), "Particle Interaction".LineCenter(font), Color.Wheat, 1f);

            string particleInteractionText = GameSettings.ParticleInteraction ? "Enabled" : "Disabled";
            if (enableParticleInteraction.IsHover == false)
                sb.DrawString(font, particleInteractionText, enableParticleInteraction.Center + longButton.Center(), particleInteractionText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, particleInteractionText, enableParticleInteraction.Center + longButton.Center(), particleInteractionText.LineCenter(font), ColorHelper.UI_Gold, 1f);


            //Limit shaders button text
            sb.DrawString(font, "Limit Shaders", limitShaders.Center + new Vector2(longButton.Center().X, -10), "Limit Shaders".LineCenter(font), Color.Wheat, 1f);

            string limitShader = GameSettings.LimitShaders ? "Enabled" : "Disabled";
            if (limitShaders.IsHover == false)
                sb.DrawString(font, limitShader, limitShaders.Center + longButton.Center(), limitShader.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, limitShader, limitShaders.Center + longButton.Center(), limitShader.LineCenter(font), ColorHelper.UI_Gold, 1f);

            //Hide HUD button text
            sb.DrawString(font, "Hide HUD", hideHUD.Center + new Vector2(longButton.Center().X, -10), "Hide HUD".LineCenter(font), Color.Wheat, 1f);

            string hideHUDText = GameSettings.IsHidingHUD ? "Enabled" : "Disabled";
            if (hideHUD.IsHover == false)
                sb.DrawString(font, hideHUDText, hideHUD.Center + longButton.Center(), hideHUDText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, hideHUDText, hideHUD.Center + longButton.Center(), hideHUDText.LineCenter(font), ColorHelper.UI_Gold, 1f);

            //Use widescreen button text
            sb.DrawString(font, "Use Widescreen", useWidescreen.Center + new Vector2(longButton.Center().X, -10), "Use Widescreen".LineCenter(font), Color.Wheat, 1f);

            string useWidescreenText = GameSettings.IsUseWidescreen ? "Enabled" : "Disabled";
            if (useWidescreen.IsHover == false)
                sb.DrawString(font, useWidescreenText, useWidescreen.Center + longButton.Center(), useWidescreenText.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, useWidescreenText, useWidescreen.Center + longButton.Center(), useWidescreenText.LineCenter(font), ColorHelper.UI_Gold, 1f);


            //Data collection button text
            //sb.DrawString(font, "Anonymous Statistics", dataCollection.Center + new Vector2(longButton.Center().X, -10), "Anonymous Statistics".LineCenter(font), Color.Wheat, 1f);

            /*string anonymousStats = false ? "Enabled" : "Disabled";
            if (dataCollection.IsHover == false)
                sb.DrawString(font, anonymousStats, dataCollection.Center + longButton.Center(), anonymousStats.LineCenter(font), Color.Silver, 1f);
            else
                sb.DrawString(font, anonymousStats, dataCollection.Center + longButton.Center(), anonymousStats.LineCenter(font), ColorHelper.UI_Gold, 1f);
            */
        }
        private void DrawAudioTab(SpriteBatch sb)
        {
            string masterString = "Master Volume (" + (int)(masterVolumePct.Percentage * 100) + "%)";
            sb.DrawString(font, masterString, new Vector2(position.X + (bg.Width / 2), position.Y + 90), masterString.LineCenter(font), Color.Wheat, 1f);
            masterVolumePct.Draw(sb, Color.Lerp(ColorHelper.UI_DarkerGold, ColorHelper.UI_LightGold, masterVolumePct.Percentage));

            string soundString = "Sound Volume (" + (int)(soundVolumePct.Percentage * 100) + "%)";
            sb.DrawString(font, soundString, new Vector2(position.X + (bg.Width / 2), position.Y + 140), soundString.LineCenter(font), Color.Wheat, 1f);
            soundVolumePct.Draw(sb, Color.Lerp(ColorHelper.UI_DarkerGold, ColorHelper.UI_LightGold, soundVolumePct.Percentage));

            string ambienceString = "Ambience Volume (" + (int)(ambienceVolumePct.Percentage * 100) + "%)";
            sb.DrawString(font, ambienceString, new Vector2(position.X + (bg.Width / 2), position.Y + 190), ambienceString.LineCenter(font), Color.Wheat, 1f);
            ambienceVolumePct.Draw(sb, Color.Lerp(ColorHelper.UI_DarkerGold, ColorHelper.UI_LightGold, ambienceVolumePct.Percentage));

            string musicString = "Music Volume (" + (int)(musicVolumePct.Percentage * 100) + "%)";
            sb.DrawString(font, musicString, new Vector2(position.X + (bg.Width / 2), position.Y + 240), musicString.LineCenter(font), Color.Wheat, 1f);
            musicVolumePct.Draw(sb, Color.Lerp(ColorHelper.UI_DarkerGold, ColorHelper.UI_LightGold, musicVolumePct.Percentage));
        }

        private Vector2 buttonTextYOrigin = new Vector2(0, 10);
        private void DrawControlsTab(SpriteBatch sb)
        {
            sb.DrawString(font, "Character Movement", position + new Vector2(bg.Center().X, 80 + tabPosition), "Character Movement".LineCenter(font), Color.Wheat, 1f);
            sb.Draw(pixel, new Rectangle((int)position.X + 80, (int)position.Y + 90 + (int)tabPosition, 350, 1), ColorHelper.UI_Gold);

            DrawControlButton(sb, moveUp, "Move Up", controls.CurrentControls.MoveUp);
            DrawControlButton(sb, moveDown, "Move Down", controls.CurrentControls.MoveDown);
            DrawControlButton(sb, moveLeft, "Move Left", controls.CurrentControls.MoveLeft);
            DrawControlButton(sb, moveRight, "Move Right", controls.CurrentControls.MoveRight);
            DrawControlButton(sb, changeActivation, "Activate", controls.CurrentControls.Activate);
            DrawControlButton(sb, quickSlot, "Use Quickslot", controls.CurrentControls.Quickslot);
            DrawControlButton(sb, jump, "Jump", controls.CurrentControls.Jump);
            DrawControlButton(sb, roll, "Roll", controls.CurrentControls.Roll);
            DrawControlButton(sb, sneak, "Sneak", controls.CurrentControls.Sneak);
            DrawControlButton(sb, sprint, "Sprint", controls.CurrentControls.Sprint);


            sb.DrawString(font, "Combat-related Controls", position + new Vector2(bg.Center().X, 420 + tabPosition), "Combat-related Controls".LineCenter(font), Color.Wheat, 1f);
            sb.Draw(pixel, new Rectangle((int)position.X + 80, (int)position.Y + 430 + (int)tabPosition, 350, 1), ColorHelper.UI_Gold);

            DrawControlButton(sb, useSelectedSoul, "Use Selected Soul", controls.CurrentControls.UseSelectedSoul);
            DrawControlButton(sb, scrollSouls, "Scroll Souls", controls.CurrentControls.ScrollSouls);
            DrawControlButton(sb, scrollSpells, "Scroll Spells", controls.CurrentControls.ScrollSpells);
            DrawControlButton(sb, assignSwapPrimary, "Swap Primary", controls.CurrentControls.SwapPrimary);
            DrawControlButton(sb, assignSwapOffhand, "Swap Offhand", controls.CurrentControls.SwapOffhand);
            DrawControlButton(sb, assignSwapAmmo, "Swap Ammo", controls.CurrentControls.SwapAmmo);


            sb.DrawString(font, "Interface Shortcuts", position + new Vector2(bg.Center().X, 670 + tabPosition), "Interface Shortcuts".LineCenter(font), Color.Wheat, 1f);
            sb.Draw(pixel, new Rectangle((int)position.X + 80, (int)position.Y + 680 + (int)tabPosition, 350, 1), ColorHelper.UI_Gold);

            DrawControlButton(sb, openInventory, "Inventory", controls.CurrentControls.OpenInventory);
            DrawControlButton(sb, openMagic, "Spellbook", controls.CurrentControls.OpenMagic);
            DrawControlButton(sb, openSouls, "Souls", controls.CurrentControls.OpenSouls);
            DrawControlButton(sb, openRumors, "Rumors", controls.CurrentControls.OpenRumors);
            DrawControlButton(sb, openStats, "Skills", controls.CurrentControls.OpenStats);
            DrawControlButton(sb, openSettings, "Settings", controls.CurrentControls.OpenSettings);
            DrawControlButton(sb, pause, "Pause", controls.CurrentControls.Pause);


            sb.DrawString(font, "Other Controls", position + new Vector2(bg.Center().X, 920 + tabPosition), "Other Controls".LineCenter(font), Color.Wheat, 1f);
            sb.Draw(pixel, new Rectangle((int)position.X + 80, (int)position.Y + 930 + (int)tabPosition, 350, 1), ColorHelper.UI_Gold);

            DrawControlButton(sb, talk, "Talk", controls.CurrentControls.TypeMessage);
            DrawControlButton(sb, takeScreenshot, "Screenshot", controls.CurrentControls.TakeScreenshot);
            DrawControlButton(sb, adjustQSLeft, "Quickslot Left", controls.CurrentControls.LoopQSLeft);
            DrawControlButton(sb, adjustQSRight, "Quickslot Right", controls.CurrentControls.LoopQSRight);
            DrawControlButton(sb, adjustQSButtonLeft, "Quickslot Button Left", controls.CurrentControls.ButtonTargetLeft);
            DrawControlButton(sb, adjustQSButtonRight, "Quickslot Button Right", controls.CurrentControls.ButtonTargetRight);


            sb.DrawString(font, "Companion Commands", position + new Vector2(bg.Center().X, 1140 + tabPosition), "Companion Commands".LineCenter(font), Color.Wheat, 1f);
            sb.Draw(pixel, new Rectangle((int)position.X + 80, (int)position.Y + 1150 + (int)tabPosition, 350, 1), ColorHelper.UI_Gold);

            DrawControlButton(sb, companionCommand0, "Companion Command One", controls.CurrentControls.Command0);
            DrawControlButton(sb, companionCommand1, "Companion Command Two", controls.CurrentControls.Command1);
            DrawControlButton(sb, companionCommand2, "Companion Command Three", controls.CurrentControls.Command2);
            DrawControlButton(sb, companionCommand3, "Companion Command Four", controls.CurrentControls.Command3);
            DrawControlButton(sb, companionCommand4, "Companion Command Five", controls.CurrentControls.Command4);
            DrawControlButton(sb, companionCommand5, "Companion Command Six", controls.CurrentControls.Command5);
            DrawControlButton(sb, companionCommand6, "Companion Command Seven", controls.CurrentControls.Command6);
            DrawControlButton(sb, companionCommand7, "Companion Command Eight", controls.CurrentControls.Command7);
            DrawControlButton(sb, companionCommand8, "Companion Command Nine", controls.CurrentControls.Command8);
            DrawControlButton(sb, companionCommand9, "Companion Command Ten", controls.CurrentControls.Command9);

            /*
            moveUp.DrawButton(sb, Color.White);
            moveDown.DrawButton(sb, Color.White);
            moveLeft.DrawButton(sb, Color.White);
            moveRight.DrawButton(sb, Color.White);
            changeActivation.DrawButton(sb, Color.White);
            quickSlot.DrawButton(sb, Color.White);
            jump.DrawButton(sb, Color.White);
            roll.DrawButton(sb, Color.White);
            sneak.DrawButton(sb, Color.White);
            sprint.DrawButton(sb, Color.White);

            sb.DrawString(font, "Move Up", new Vector2(position.X + 40, moveUp.Position.Y + longButton.Center().Y), buttonTextYOrigin, Color.Wheat, 1f);
            sb.DrawString(font, "Move Down", new Vector2(position.X + 40, moveDown.Position.Y + longButton.Center().Y), buttonTextYOrigin, Color.Wheat, 1f);
            sb.DrawString(font, "Move Left", new Vector2(position.X + 40, moveLeft.Position.Y + longButton.Center().Y), buttonTextYOrigin, Color.Wheat, 1f);
            sb.DrawString(font, "Move Right", new Vector2(position.X + 40, moveRight.Position.Y + longButton.Center().Y), buttonTextYOrigin, Color.Wheat, 1f);
            sb.DrawString(font, "Activate", new Vector2(position.X + 40, changeActivation.Position.Y + longButton.Center().Y), buttonTextYOrigin, Color.Wheat, 1f);

            sb.DrawString(font, controls.KeyString(controls.CurrentControls.MoveUp), moveUp.Center + longButton.Center(), controls.KeyString(controls.CurrentControls.MoveUp).LineCenter(font), Color.White, 1f);
            sb.DrawString(font, controls.KeyString(controls.CurrentControls.MoveDown), moveDown.Center + longButton.Center(), controls.KeyString(controls.CurrentControls.MoveDown).LineCenter(font), Color.White, 1f);
            sb.DrawString(font, controls.KeyString(controls.CurrentControls.MoveLeft), moveLeft.Center + longButton.Center(), controls.KeyString(controls.CurrentControls.MoveLeft).LineCenter(font), Color.White, 1f);
            sb.DrawString(font, controls.KeyString(controls.CurrentControls.MoveRight), moveRight.Center + longButton.Center(), controls.KeyString(controls.CurrentControls.MoveRight).LineCenter(font), Color.White, 1f);
            sb.DrawString(font, controls.KeyString(controls.CurrentControls.Activate), changeActivation.Center + longButton.Center(), controls.KeyString(controls.CurrentControls.Activate).LineCenter(font), Color.White, 1f);*/
        }
        private void DrawControlButton(SpriteBatch sb, MenuButton button, string text, Keys key)
        {
            button.DrawButton(sb, Color.White);
            sb.DrawString(font, text, new Vector2(position.X + 40, button.Position.Y + longButton.Center().Y), buttonTextYOrigin, Color.Wheat, 1f);
            sb.DrawString(font, controls.KeyString(key), button.Center + longButton.Center(), controls.KeyString(key).LineCenter(font), Color.White, 1f);
        }

        private void DrawStatsTab(SpriteBatch sb)
        {

        }

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return isTabHover || clickCheck.Contains(controls.MousePosition) || isDragging || hideRect.Contains(controls.MousePosition);
            else
                return false;
        }
    }
}
