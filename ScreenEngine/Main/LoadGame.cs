using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.Various;
using System.Collections.Generic;
using System.IO;

namespace Pilgrimage_Of_Embers.ScreenEngine.Main
{
    class SavedFile
    {
        private string name;

        public string Name { get { return name; } }
        public Rectangle Rect { get; set; }
        public bool IsSelected { get; set; }
        public bool IsHover { get; set; }

        public SavedFile(string Name)
        {
            name = Name;
        }
    }

    public class LoadGame
    {
        private SpriteFont font, boldFont;
        private Texture2D button, buttonHover, buttonClick, largeButton, largebuttonClick, largeButtonHover, bg, pixel;
        public Vector2 Position { get; set; }
        public Vector2 PositionOffset { get; private set; }

        private float scrollPosition = 0;

        private List<SavedFile> files = new List<SavedFile>();
        private SavedFile selectedFile;

        private MenuButton loadGame, deleteGame, returnButton;
        private SelectionBox deleteCheck;

        private Rectangle scissorGrid;
        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        private Controls controls = new Controls();

        private Camera camera;
        private ScreenManager screens;
        private WorldManager world;
        private MainScreen main;

        public LoadGame() { }

        public void SetReferences(Camera camera, WorldManager world, ScreenManager screens, MainScreen main) { this.camera = camera; this.world = world; this.screens = screens; this.main = main; }

        public void Load(ContentManager cm, Texture2D button, Texture2D buttonHover, Texture2D buttonClick)
        {
            pixel = cm.Load<Texture2D>("rect");
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            boldFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            bg = cm.Load<Texture2D>("Interface/Main/LoadGame/loadBG");
            PositionOffset = bg.Center();

            this.button = button;
            this.buttonHover = buttonHover;
            this.buttonClick = buttonClick;

            largeButton = cm.Load<Texture2D>("Interface/Main/LoadGame/MenuButton");
            largebuttonClick = cm.Load<Texture2D>("Interface/Main/LoadGame/MenuButtonClick");
            largeButtonHover = cm.Load<Texture2D>("Interface/Main/LoadGame/MenuButtonHover");

            RefreshSaves();

            loadGame = new MenuButton(Vector2.Zero, button, buttonClick, buttonHover, 1f);
            deleteGame = new MenuButton(Vector2.Zero, button, buttonClick, buttonHover, 1f);
            returnButton = new MenuButton(Vector2.Zero, button, buttonClick, buttonHover, 1f);

            deleteCheck = new SelectionBox("Delete Save Game", "This save file will be removed from your system for all eternity. Are you sure?,", "Yes...", "No!");
            deleteCheck.Load(cm);
        }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            if (deleteCheck.IsActive == false)
                ScrollGrid(gt, 30f, 250f, 300f, 10f);

            CheckButtons(gt);

            controls.UpdateLast();
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
            scrollPosition += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -maxScrollSpeed, maxScrollSpeed);

            if (scrollVelocity > clampSpeed)
                scrollVelocity -= scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -clampSpeed)
                scrollVelocity += scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -clampSpeed && scrollVelocity < clampSpeed)
                scrollVelocity = 0f;

            float longBounds = -((files.Count * 24) + (Position.Y - 200) - scissorGrid.Height);
            if (longBounds >= 0f)
                longBounds = 0f;

            if (scrollPosition > 0f)
                scrollVelocity = 0f;
            else if (scrollPosition < camera.WorldToScreen(new Vector2(0, longBounds)).Y)
                scrollVelocity = 0f;

            scrollPosition = MathHelper.Clamp(scrollPosition, longBounds, 0f);
        }
        private void CheckButtons(GameTime gt)
        {
            loadGame.PositionCenter = (Position + new Vector2(15, 452)).ToPoint();
            deleteGame.PositionCenter = (Position + new Vector2(205, 452)).ToPoint();
            returnButton.PositionCenter = (Position + new Vector2(15, 474)).ToPoint();

            loadGame.Update(gt, controls);
            deleteGame.Update(gt, controls);
            returnButton.Update(gt, controls);

            scissorGrid = new Rectangle((int)Position.X, (int)Position.Y + 53, bg.Width + 6, 393);

            if (deleteCheck.IsActive == false)
            {
                if (selectedFile != null)
                {
                    if (controls.IsDoubleClicked(gt, Controls.MouseButton.LeftClick))
                    {
                        if (selectedFile.Rect.Contains(controls.MousePosition))
                            isLoadingSave = true;
                    }
                }

                for (int i = 0; i < files.Count; i++)
                {
                    files[i].Rect = new Rectangle((int)Position.X + 16, (int)(Position.Y + 55) + (i * largeButton.Height + 2) + (int)scrollPosition, largeButton.Width, largeButton.Height);

                    if (scissorGrid.Contains(controls.MouseVector))
                    {
                        if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        {
                            if (files[i].Rect.Contains(controls.MouseVector))
                            {
                                files[i].IsSelected = true;
                                selectedFile = files[i];
                            }
                            else
                                files[i].IsSelected = false;
                        }

                        if (files[i].Rect.Contains(controls.MouseVector))
                            files[i].IsHover = true;
                        else
                            files[i].IsHover = false;
                    }
                }
            }

            deleteCheck.Update(gt);

            if (deleteCheck.IsActive == false)
            {
                if (selectedFile != null)
                {
                    if (deleteGame.IsLeftClicked == true)
                    {
                        deleteCheck.IsActive = true;
                    }

                    if (loadGame.IsLeftClicked == true)
                        isLoadingSave = true;
                }

                if (returnButton.IsLeftClicked == true)
                    main.GoToMainScreen();
            }

            if (deleteCheck.CurrentSelection == 0)
            {
                DeleteSave(selectedFile);
                deleteCheck.IsActive = false;
            }
            if (deleteCheck.CurrentSelection == 1)
                deleteCheck.IsActive = false;

            if (isLoadingSave == true)
            {
                screens.EFFECTS_BeginTransition(ScreenEffects.TransitionType.Fade, Color.Black, 1000, 2f, 1f);

                if (screens.EFFECTS_IsTransitionFaded == true)
                    LoadSaveFile();
            }
        }

        private bool isLoadingSave = false;
        private void LoadSaveFile()
        {
            world.LoadSaveFile(selectedFile.Name);
            selectedFile = null;

            isLoadingSave = false;
        }

        private void DeleteSave(SavedFile file)
        {
            if (Directory.Exists("Saves/" + file.Name))
            {
                Directory.Delete("Saves/" + file.Name, true);
                files.Remove(file);

                selectedFile = null;
            }
        }
        private void ValidateFile(SavedFile file)
        {
            if (Directory.Exists("Saves/" + file.Name))
            {
                if (Directory.GetFiles("Saves/" + file.Name, "*.data").Length == 0)
                    files.Remove(file);
            }
        }

        private void RefreshSaves()
        {
            files.Clear();

            string[] folders = Directory.GetDirectories("Saves/");

            for (int i = 0; i < folders.Length; i++)
                files.Add(new SavedFile(folders[i].Replace("Saves/", "")));

            for (int i = 0; i < files.Count; i++)
                ValidateFile(files[i]);
        }

        private Color selectBorder = Color.Lerp(new Color(242, 136, 70, 255), Color.Transparent, .25f) , selectInside = new Color(193, 108, 56, 255), bgColor = Color.Lerp(ColorHelper.Charcoal, Color.White, .05f);

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(bg, Position, Color.White);
            sb.DrawString(boldFont, "Resume Adventure", Position + new Vector2(bg.Width / 2, 12), "Resume Adventure".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

            loadGame.DrawButton(sb, Color.White);
            deleteGame.DrawButton(sb, Color.White);
            returnButton.DrawButton(sb, Color.White);

            DrawButtonText(sb, loadGame, "Load");
            DrawButtonText(sb, deleteGame, "Delete");
            DrawButtonText(sb, returnButton, "Back");
        }

        public void DrawScreen(SpriteBatch sb)
        {
            deleteCheck.Draw(sb);
        }

        private void DrawButtonText(SpriteBatch sb, MenuButton button, string text)
        {
            if (button.IsHover)
                sb.DrawString(font, text, button.Position.ToVector2() + this.button.Center(), text.LineCenter(font), ColorHelper.UI_Gold, 1f);
            else if (button.IsButtonPressed)
                sb.DrawString(font, text, button.Position.ToVector2() + this.button.Center(), text.LineCenter(font), ColorHelper.UI_DarkerGold, 1f);
            else
                sb.DrawString(font, text, button.Position.ToVector2() + this.button.Center(), text.LineCenter(font), Color.White, 1f);
        }

        public void DrawGrid(SpriteBatch sb)
        {
            sb.Begin(samplerState: SamplerState.PointClamp, rasterizerState: scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissorGrid;

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].IsSelected == true || files[i].IsHover == true)
                    sb.Draw(largeButtonHover, files[i].Rect, Color.White);
                else
                    sb.Draw(largeButton, files[i].Rect, Color.White);

                if (files[i].IsSelected == true || files[i].IsHover == true)
                    sb.DrawString(font, files[i].Name, new Vector2(files[i].Rect.X + (files[i].Rect.Width / 2), files[i].Rect.Y + (files[i].Rect.Height / 2)), files[i].Name.LineCenter(font), ColorHelper.UI_Gold, 1f);
                else
                    sb.DrawString(font, files[i].Name, new Vector2(files[i].Rect.X + (files[i].Rect.Width / 2), files[i].Rect.Y + (files[i].Rect.Height / 2)), files[i].Name.LineCenter(font), Color.White, 1f);
            }

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }
    }
}
