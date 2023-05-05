using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Screen_based;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.Various;
using Pilgrimage_Of_Embers.TileEngine;

namespace Pilgrimage_Of_Embers.ScreenEngine.Main
{
    public class MainScreen
    {
        private SpriteFont font, boldFont;
        private Texture2D logo, bg, button, buttonHover, buttonClick, largeButton, largeButtonHover, pixel;

        public enum MainState { Main, New, Load, Config, Exit }
        private MainState state = MainState.Main;

        private StartScreen startScreen;
        private NewGame newScreen;
        private LoadGame loadScreen;
        private SelectionBox exitScreen;

        MenuButton newGame, loadGame, config, exit;

        Vector2 currentPosition, lastPosition, targetPosition, mainOffset, newOffset, loadOffset, configOffset, exitOffset;
        private Vector2 position;

        private Controls controls = new Controls();

        private float positionLerp = 0f, blurLerp = 0f;
        private bool isLerping;

        private Camera camera;
        private WorldManager world;
        private ScreenManager screens;
        private TileMap map;
        public bool IsActive { get; set; }

        private LogoEmbers logoEmbers;

        public MainScreen()
        {
            logoEmbers = new LogoEmbers();
        }
        public void SetReferences(Camera camera, WorldManager world, ScreenManager screens, TileMap map)
        {
            this.camera = camera;
            this.world = world;
            this.screens = screens;
            this.map = map;
        }

        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            boldFont = cm.Load<SpriteFont>("Fonts/boldOutlined");
            pixel = cm.Load<Texture2D>("rect");

            logo = cm.Load<Texture2D>("Interface/Main/logoFinal");
            bg = cm.Load<Texture2D>("Interface/Main/mainBG");

            button = cm.Load<Texture2D>("Interface/Global/paneExtendedButton");
            buttonHover = cm.Load<Texture2D>("Interface/Global/paneExtendedButtonHover");
            buttonClick = cm.Load<Texture2D>("Interface/Global/paneExtendedButtonHover");

            largeButton = cm.Load<Texture2D>("Interface/Main/LoadGame/menuButton");
            largeButtonHover = cm.Load<Texture2D>("Interface/Main/LoadGame/menuButtonHover");

            newGame = new MenuButton(Vector2.Zero, largeButton, largeButtonHover, largeButtonHover, 1f);
            loadGame = new MenuButton(Vector2.Zero, largeButton, largeButtonHover, largeButtonHover, 1f);
            config = new MenuButton(Vector2.Zero, largeButton, largeButtonHover, largeButtonHover, 1f);
            exit = new MenuButton(Vector2.Zero, largeButton, largeButtonHover, largeButtonHover, 1f);

            startScreen = new StartScreen(screens);
            startScreen.Load(cm);

            loadScreen = new LoadGame();
            loadScreen.SetReferences(camera, world, screens, this);
            loadScreen.Load(cm, button, buttonHover, buttonClick);

            newScreen = new NewGame();
            newScreen.SetReferences(camera, world, screens, this);
            newScreen.Load(cm, button, buttonHover, buttonClick);

            exitScreen = new SelectionBox("Quit Game?", "Are you sure this is what you want?", "I must go.", "I'll stay.");
            exitScreen.Load(cm);
            exitScreen.IsActive = true;

            screens.OPTIONS_IsActive = true;
            screens.EFFECTS_BeginTransition(ScreenEffects.TransitionType.Fade, Color.Black, 4000, 1f, 1f);
            screens.EFFECTS_TransitionLerp = 1f;

            logoEmbers = new LogoEmbers();
            logoEmbers.Load(cm);
            logoEmbers.IsManualDepth = true;

            IsActive = true;
        }

        public void SetScreenPositions(Vector2 mainOffset, Vector2 newOffset, Vector2 loadOffset,
                                       Vector2 configOffset, Vector2 exitOffset)
        {
            this.mainOffset = mainOffset;
            this.newOffset = newOffset;
            this.loadOffset = loadOffset;
            this.configOffset = configOffset;
            this.exitOffset = exitOffset;

            currentPosition = mainOffset;
        }

        private bool isMusicSet = false;

        public void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                camera.ForceLookAt(currentPosition);
                UpdateLerp(gt);

                newScreen.Position = camera.WorldToScreen(newOffset - newScreen.PositionOffset);
                loadScreen.Position = camera.WorldToScreen(loadOffset - loadScreen.PositionOffset);
                exitScreen.Position = camera.WorldToScreen(exitOffset - new Vector2(180, 100));
                screens.OPTIONS_SetPosition(camera.WorldToScreen(configOffset - new Vector2(250, 300)));

                position = camera.WorldToScreen(mainOffset - bg.Center());

                controls.UpdateCurrent();

                startScreen.Update(gt);

                if (startScreen.IsActive == false)
                {
                    UpdateMain(gt);
                    newScreen.Update(gt);
                    loadScreen.Update(gt);
                    UpdateExit(gt);
                }

                if (screens.SPLASH_IsComplete() == true && startScreen.IsActive == false)
                    CheckEscKey();

                if (screens.SPLASH_IsComplete() == true)
                {
                    if (isMusicSet == false)
                    {
                        map.MUSIC_Play(1, 0);
                        isMusicSet = true;
                    }
                }

                controls.UpdateLast();
            }

            if (IsActive == false)
            {
                if (logoEmbers.IsActivated == true)
                    logoEmbers.ForceRemoveAll();

                logoEmbers.IsActivated = false;
            }
            else
                logoEmbers.IsActivated = true;

            logoEmbers.Offset = camera.ScreenToWorld(position + new Vector2(bg.Width / 2, -50));
            logoEmbers.Update(gt);
        }
        private void UpdateMain(GameTime gt)
        {
            CheckButtons(gt);
        }
        private void CheckButtons(GameTime gt)
        {
            newGame.Position = (position + new Vector2(17, 56 + offset)).ToPoint();
            loadGame.Position = (position + new Vector2(17, 97 + offset)).ToPoint();
            config.Position = (position + new Vector2(17, 138 + offset)).ToPoint();
            exit.Position = (position + new Vector2(17, 179 + offset)).ToPoint();

            newGame.Update(gt, controls);
            loadGame.Update(gt, controls);
            config.Update(gt, controls);
            exit.Update(gt, controls);

            if (newGame.IsLeftClicked == true)
            {
                state = MainState.New;

                isLerping = true;
                lastPosition = currentPosition;
                targetPosition = newOffset;

                newScreen.Reset();
                screens.PlaySound("ButtonClick");
            }

            if (loadGame.IsLeftClicked == true)
            {
                state = MainState.Load;

                isLerping = true;
                lastPosition = currentPosition;
                targetPosition = loadOffset;

                screens.PlaySound("ButtonClick");
            }

            if (config.IsLeftClicked == true)
            {
                state = MainState.Config;

                isLerping = true;
                lastPosition = currentPosition;
                targetPosition = configOffset;

                screens.PlaySound("ButtonClick");
            }

            if (exit.IsLeftClicked == true)
            {
                state = MainState.Exit;

                isLerping = true;
                lastPosition = currentPosition;
                targetPosition = exitOffset;

                screens.PlaySound("ButtonClick");
            }
        }

        private void UpdateExit(GameTime gt)
        {
            exitScreen.Update(gt);

            if (exitScreen.CurrentSelection == 0)
            {
                world.QuitGame();
                screens.PlaySound("ButtonClick");
            }
            if (exitScreen.CurrentSelection == 1)
            {
                state = MainState.Main;

                isLerping = true;
                lastPosition = currentPosition;
                targetPosition = mainOffset;

                screens.PlaySound("ButtonClick");
            }
        }

        private void UpdateLerp(GameTime gt)
        {
            positionLerp = MathHelper.Clamp(positionLerp, 0f, 1f);
            blurLerp = MathHelper.Clamp(positionLerp, 0f, 1f);

            if (isLerping == true)
            {
                currentPosition = Vector2.SmoothStep(lastPosition, targetPosition, positionLerp);
                positionLerp += .85f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            if (positionLerp >= 1f)
            {
                positionLerp = 0f;
                isLerping = false;
            }
        }

        private void CheckEscKey()
        {
            if (isLerping == false)
            {
                if (state != MainState.Main)
                {
                    if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Escape))
                        GoToMainScreen();
                }
                else
                {
                    if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Escape))
                    {
                        state = MainState.Exit;

                        isLerping = true;
                        lastPosition = currentPosition;
                        targetPosition = exitOffset;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.View(Vector2.One));

                logoEmbers.Draw(sb);

                sb.End();

                sb.Begin(samplerState: SamplerState.PointClamp);

                sb.Draw(logo, position + new Vector2(bg.Width / 2, -50), Color.White, logo.Center(), 0f, 1f);
                startScreen.Draw(sb);

                if (startScreen.IsActive == false)
                {
                    DrawMain(sb);
                    newScreen.Draw(sb);
                    loadScreen.Draw(sb);
                    exitScreen.Draw(sb);
                }

                sb.End();

                if (startScreen.IsActive == false)
                {
                    newScreen.DrawScissors(sb);
                    loadScreen.DrawGrid(sb);
                }
            }
        }

        private float offset = 150;
        private void DrawMain(SpriteBatch sb)
        {
            sb.Draw(bg, position + new Vector2(0, offset), Color.White);
            sb.DrawString(boldFont, "Main Screen", position + new Vector2(bg.Width / 2, 12 + offset), "Main Screen".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

            newGame.DrawButton(sb, Color.White);
            loadGame.DrawButton(sb, Color.White);
            config.DrawButton(sb, Color.White);
            exit.DrawButton(sb, Color.White);

            DrawButtonText(sb, newGame, "Begin Anew");
            DrawButtonText(sb, loadGame, "Resume Journey");
            DrawButtonText(sb, config, "Configure");
            DrawButtonText(sb, exit, "Exit");

            string version = GameInfo.Version();

            sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + bg.Width / 2 - 100, (int)(position.Y + offset) + bg.Height + 1, 200, 20), ColorHelper.Charcoal, ColorHelper.UI_Gold);
            sb.DrawString(font, version, position + new Vector2(bg.Width / 2, bg.Height + 3 + offset), new Vector2(font.MeasureString(version).X / 2, 0), Color.DarkOrange, 1f);
        }
        private void DrawButtonText(SpriteBatch sb, MenuButton button, string text)
        {
            if (button.IsHover)
                sb.DrawString(font, text, button.Position.ToVector2() + this.largeButton.Center(), text.LineCenter(font), ColorHelper.UI_Gold, 1f);
            else if (button.IsButtonPressed)
                sb.DrawString(font, text, button.Position.ToVector2() + this.largeButton.Center(), text.LineCenter(font), ColorHelper.UI_DarkerGold, 1f);
            else
                sb.DrawString(font, text, button.Position.ToVector2() + this.largeButton.Center(), text.LineCenter(font), Color.White, 1f);
        }

        public void DrawScreen(SpriteBatch sb)
        {
            newScreen.DrawScreen(sb);
            loadScreen.DrawScreen(sb);
        }

        public void GoToMainScreen()
        {
            state = MainState.Main;

            isLerping = true;
            lastPosition = currentPosition;
            targetPosition = mainOffset;
        }
        public void ForceMainScreen()
        {
            GoToMainScreen();
            positionLerp = 1f;
        }
    }
}
