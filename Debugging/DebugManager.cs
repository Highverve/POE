using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Helper_Classes;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Debugging
{
    class Button
    {
        private string tooltip, shortcutKey;

        public Rectangle Rect { get; private set; }

        public Button(Rectangle Rect, string Tooltip, string ShortcutKey)
        {
            this.Rect = Rect;
            tooltip = Tooltip;
            shortcutKey = ShortcutKey;

            idleColor = Color.Lerp(Color.White, Color.Transparent, .25f);
            hoverColor = Color.Lerp(Color.Gray, Color.Transparent, .25f);
            activeColor = Color.Lerp(Color.Orange, Color.Transparent, .25f);
            activeHoverColor = Color.Lerp(Color.DarkRed, Color.Transparent, .25f);
            inactiveColor = Color.Lerp(new Color(96, 96, 96, 255), Color.Transparent, .25f);
        }

        private bool isClicked = false;
        private int tick = 0;
        public void ClickButton(Controls controls, Action action)
        {
            if (isClicked == true)
            {
                tick++;

                if (tick > 10)
                {
                    action.Invoke();

                    tick = 0;
                    isClicked = false;
                }
            }

            if (Rect.Contains(controls.MousePosition))
            {
                ToolTip.RequestStringAssign(tooltip);
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    isClicked = true;
            }
        }

        private Color currentColor, idleColor, hoverColor, activeColor, activeHoverColor, inactiveColor;
        public void DrawButton(SpriteBatch sb, Texture2D pixel, SpriteFont font, bool isHover, bool isActive, bool isUseable)
        {
            if (isUseable == false)
                currentColor = inactiveColor;
            else
            {
                if (isActive == true)
                {
                    if (isHover == true)
                        currentColor = activeHoverColor;
                    else
                        currentColor = activeColor;
                }
                else
                {
                    if (isHover == true)
                        currentColor = hoverColor;
                    else
                        currentColor = idleColor;
                }
            }

            sb.DrawBoxBordered(pixel, Rect, Color.Transparent, currentColor, 2);
            sb.DrawString(font, shortcutKey, new Vector2(Rect.Center.X, Rect.Center.Y), shortcutKey.LineCenter(font), currentColor, 1f);
        }
    }

    public class DebugManager
    {
        SpriteFont consoleFont;
        Texture2D pixel;

        FPSCounter fps;
        public static Info info;
        Console console;

        ContentManager cm;
        Controls controls = new Controls();

        private WorldManager world;
        private TileMap map;
        private ScreenManager screens;
        private PlayerEntity player;
        private Camera camera;

        private GraphicsDevice g;

        private Button tileEditor, objectEditor, infoButton, outputCreation, debugCamera, takeScreenshot;

        public DebugManager() { }
        public void SetReferences(Game Game, WorldManager World, TileMap Map, ScreenManager Screens, PlayerEntity Player, Camera Camera)
        {
            g = Game.GraphicsDevice;

            world = World;
            map = Map;
            screens = Screens;
            player = Player;
            camera = Camera;
        }

        public void LoadContent(ContentManager cm)
        {
            this.cm = cm;

            consoleFont = cm.Load<SpriteFont>("Fonts/boldOutlined");
            pixel = cm.Load<Texture2D>("rect");

            fps = new FPSCounter(consoleFont);
            info = new Info(consoleFont, pixel, map, screens, camera, player);
            console = new Console(consoleFont, pixel, world, screens, map, player, camera);

            Point size = new Point(48, 48);
            Point offset = new Point(16, 16);
            tileEditor = new Button(new Rectangle(new Point(offset.X, 16), size), "Tile Editor", "F1");
            objectEditor = new Button(new Rectangle(new Point(offset.X + 56, 16), size), "Object Editor", "F2");
            infoButton = new Button(new Rectangle(new Point(offset.X + (56 * 2), 16), size), "Info", "F4");
            outputCreation = new Button(new Rectangle(new Point(offset.X + (56 * 3), 16), size), "Output Creation File", "F10");
            debugCamera = new Button(new Rectangle(new Point(offset.X + (56 * 4), 16), size), "Debug Camera", "F11");
            takeScreenshot = new Button(new Rectangle(new Point(offset.X + (56 * 5), 16), size), "Take Screenshot", "F12");
        }

        public void Update(GameTime gameTime)
        {
            controls.UpdateCurrent();

            UpdateButtons(gameTime);
            UpdateDebugState();
            fps.Update(gameTime);
            info.Update(gameTime);

            if (GameSettings.IsDebugging == true)
            {
                if (console.showConsole == true)
                    console.Update(gameTime, cm);
            }

            controls.UpdateLast();
        }

        private void UpdateDebugState()
        {
            if (controls.IsKeyPressedOnce(controls.CurrentControls.Debug))
                GameSettings.IsDebugging = !GameSettings.IsDebugging;

            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenConsole))
            {
                if (GameSettings.IsDebugging == true)
                {
                    console.showConsole = !console.showConsole;
                    Controls.isTyping = console.showConsole;
                }
            }
        }

        private bool isButtonsActive = false;
        private void UpdateButtons(GameTime gt)
        {
            if (GameSettings.IsDebugging == true)
            {
                isButtonsActive = map.TILEEDITOR_IsActive == false && map.OBJECTEDITOR_IsActive == false;

                if (isButtonsActive)
                {
                    tileEditor.ClickButton(controls, () =>
                    {
                        map.TILEEDITOR_IsActive = !map.TILEEDITOR_IsActive;
                    });

                    objectEditor.ClickButton(controls, () =>
                    {
                        map.OBJECTEDITOR_IsActive = !map.OBJECTEDITOR_IsActive;
                    });

                    infoButton.ClickButton(controls, () =>
                    {
                        info.IsActive = !info.IsActive;
                    });

                    outputCreation.ClickButton(controls, () =>
                    {
                        world.GAM_OutputCreationGuide();
                    });

                    debugCamera.ClickButton(controls, () =>
                    {
                        if (camera.state != Camera.CameraState.Debug)
                            camera.SetCameraState(Camera.CameraState.Debug);
                        else
                            camera.SetCameraState(Camera.CameraState.Current);
                    });

                    takeScreenshot.ClickButton(controls, () =>
                    {
                        world.TakeScreenshot();
                    });
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (GameSettings.DisplayFPS == true || GameSettings.IsDebugging == true)
                fps.Draw(sb);

            if (GameSettings.IsDebugging == true)
            {
                info.Draw(sb);
                DrawButtons(sb);
            }

            if (console.showConsole == true)
                console.Draw(sb);
        }
        public void DrawConsole(SpriteBatch sb)
        {
            if (console.showConsole == true)
                console.DrawOutput(sb, g);
        }

        public void DrawButtons(SpriteBatch sb)
        {
            if (isButtonsActive)
            {
                tileEditor.DrawButton(sb, pixel, consoleFont, tileEditor.Rect.Contains(controls.MousePosition), map.TILEEDITOR_IsActive, isButtonsActive);
                objectEditor.DrawButton(sb, pixel, consoleFont, objectEditor.Rect.Contains(controls.MousePosition), map.OBJECTEDITOR_IsActive, isButtonsActive);
                infoButton.DrawButton(sb, pixel, consoleFont, infoButton.Rect.Contains(controls.MousePosition), info.IsActive, isButtonsActive);
                outputCreation.DrawButton(sb, pixel, consoleFont, outputCreation.Rect.Contains(controls.MousePosition), false, isButtonsActive);
                debugCamera.DrawButton(sb, pixel, consoleFont, debugCamera.Rect.Contains(controls.MousePosition), camera.state == Camera.CameraState.Debug, isButtonsActive);
                takeScreenshot.DrawButton(sb, pixel, consoleFont, takeScreenshot.Rect.Contains(controls.MousePosition), false, isButtonsActive);
            }
        }

        public void OutputConsole(string line)
        {
            console.OutputToConsole(line);
        }
        public void OutputError(string line)
        {
            console.OutputToError(line);
        }

        public void OutputMapError(string mapName, string type, int currentLine, Exception e)
        {
            console.OutputCommonMapError(mapName, type, currentLine, e);
        }
        public void DivideConsole()
        {
            console.OutputDividerConsole();
        }
        public void DivideError()
        {
            console.OutputDividerError();
        }

        public int FrameRate { get { return fps.FrameRate; } }

        public void SetInfoSlots(string text, int index)
        {
            info.SetVariable(text, index);
        }
    }
}
