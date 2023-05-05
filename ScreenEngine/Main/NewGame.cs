using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Types.Player;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.Various;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pilgrimage_Of_Embers.ScreenEngine.Main
{
    public class ClassButton
    {
        public MenuButton Button { get; private set; }
        public string Text { get; private set; }
        public string Description { get; private set; }
        public Skills.Skillset Skillset { get; private set; }

        public ClassButton(MenuButton Button, string Text, string Description, Skills.Skillset Skillset)
        {
            this.Button = Button;
            this.Text = Text;
            this.Description = Description;
            this.Skillset = Skillset;
        }

        public void CheckButton(GameTime gt, Controls controls, NewGame game, bool canClick)
        {
            Button.Update(gt, controls);

            if (canClick == true)
            {
                if (Button.IsHover == true)
                    ToolTip.RequestStringAssign(Description);
            }
        }
    }
    public class GenericButton
    {
        public MenuButton Button { get; private set; }
        public string Text { get; private set; }
        public string Description { get; private set; }

        public GenericButton(MenuButton Button, string Text, string Description)
        {
            this.Button = Button;
            this.Text = Text;
            this.Description = Description;
        }

        public void CheckButton(GameTime gt, Controls controls, NewGame game, bool canClick)
        {
            Button.Update(gt, controls);

            if (canClick == true)
            {
                if (Button.IsHover == true)
                    ToolTip.RequestStringAssign(Description);
            }
        }
    }

    public class NewGame
    {
        private SpriteFont font, boldFont;
        private Texture2D bg, buttonContainer, itemContainer, longButton, longButtonClick, longButtonHover, skillsContainer, button, buttonHover, buttonClick, pixel, itemButton, itemButtonHover;

        private Vector2 posOffset;
        public Vector2 PositionOffset { get { return posOffset; } }
        public Vector2 Position { get; set; }
        private float classScroll, itemScroll, pathwayScroll, birthScroll;

        private string playerName;
        private MenuButton randomName;
        private TextInput typing;
        private string[] randomNames;
        private Random random;
        private Texture2D randomIcon;

        private List<ClassButton> classButtons = new List<ClassButton>();
        private StartInfo classes = new StartInfo();
        public ClassButton SelectedClass { get; set; }

        private Point[] classItems;
        private List<BaseItem> items = new List<BaseItem>();
        private List<BaseItem> selectedItems = new List<BaseItem>();
        int maxItems = 5;

        private string selectedPathway;
        private List<GenericButton> pathwayButtons = new List<GenericButton>();

        private string selectedBirthplace;
        private List<GenericButton> birthplaceButtons = new List<GenericButton>();

        private MenuButton beginGame, returnMain;
        private SelectionBox startConfirm, warnEmptyName;

        private Controls controls = new Controls();

        private Camera camera;
        private WorldManager world;
        private ScreenManager screens;
        private MainScreen main;

        public NewGame() { }
        public void SetReferences(Camera camera, WorldManager world, ScreenManager screens, MainScreen main)
        {
            this.camera = camera;
            this.world = world;
            this.screens = screens;
            this.main = main;

            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public void Load(ContentManager cm, Texture2D button, Texture2D buttonHover, Texture2D buttonClick)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            boldFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            bg = cm.Load<Texture2D>("Interface/Main/NewGame/bg");
            posOffset = bg.Center();

            buttonContainer = cm.Load<Texture2D>("Interface/Main/NewGame/buttonContainer");
            itemContainer = cm.Load<Texture2D>("Interface/Main/NewGame/itemContainer");
            skillsContainer = cm.Load<Texture2D>("Interface/Main/NewGame/skillsContainer");

            longButton = cm.Load<Texture2D>("Interface/Main/NewGame/longButton");
            longButtonClick = cm.Load<Texture2D>("Interface/Main/NewGame/longButtonClick");
            longButtonHover = cm.Load<Texture2D>("Interface/Main/NewGame/longButtonHover");

            itemButton = cm.Load<Texture2D>("Interface/Global/iconBG");
            itemButtonHover = cm.Load<Texture2D>("Interface/Global/iconBGSelect");

            randomIcon = cm.Load<Texture2D>("Interface/Main/NewGame/random");

            this.button = button;
            this.buttonHover = buttonHover;
            this.buttonClick = buttonClick;

            pixel = cm.Load<Texture2D>("rect");

            randomName = new MenuButton(Vector2.Zero, cm.Load<Texture2D>("Interface/Global/smallButton"), cm.Load<Texture2D>("Interface/Global/smallButtonHover"), cm.Load<Texture2D>("Interface/Global/smallButtonHover"), 1f);
            typing = new TextInput(TextInput.KeyType.LettersOnly);

            beginGame = new MenuButton(Vector2.Zero, longButton, longButtonClick, longButtonHover, 1f);
            returnMain = new MenuButton(Vector2.Zero, longButton, longButtonClick, longButtonHover, 1f);

            startConfirm = new SelectionBox("Start Game?", "[Combine selected variables here]", "Let's begin.", "I need to change something!");
            startConfirm.Load(cm);

            warnEmptyName = new SelectionBox("Invalid Name", "You must enter a name before starting the game!", "Oh... ok.");
            warnEmptyName.Load(cm);

            Initialize();
        }
        private void Initialize()
        {
            randomNames = new string[]
                {
                    "Aliah", "Argus",
                    "Benet", "Bogart", "Busiris",
                    "Caddok", "Catus", "Cavell", "Cadda", "Cort",
                    "Dalte", "Dareious", "Davell", "Denn", "Derward", "Durantis",
                    "Edgaar", "Earwin", "Everard",
                    "Fabron", "Farold", "Flekker",
                    "Gair", "Garth", "Gower",
                    "Hamar", "Harbin", "Howell",
                    "Ivhar", "Ives",
                    "Jarvis",
                    "Keane", "Kennard", "Kynne",
                    "Ludvik", "Leofell", "Lucian",
                    "Marcel", "Marmion", "Merel", "Morgan", "Myles", "Millere",
                    "Nestor", "Norbert",
                    "Obert", "Ogdon", "Olaf", "Olvaerr", "Oran", "Ottao",
                    "Paol", "Penn", "Prewitt",
                    "Quennel",
                    "Rankin", "Reed", "Reeve", "Ritter", "Rousset",
                    "Samualle", "Stewarde", "Sennett", "Sewell", "Straeng",
                    "Tearle", "Terell", "Thane", "Trefford", "Tynan",
                    "Ulger",
                    "Valentis",

                    //Female names
                    "Adela", "Alix", "Adelaide", "Airiana", "Alisa",
                    "Blythe", "Brooke", "Beathas",
                    "Chelsea", "Claire", "Coira",
                };

            for (int i = 0; i < classes.Classes.Count; i++)
                classButtons.Add(new ClassButton(new MenuButton(Vector2.Zero, longButton, longButtonClick, longButtonHover, 1f), classes.Classes[i].Item1, classes.Classes[i].Item4, classes.Classes[i].Item2));

            SelectedClass = classButtons.FirstOrDefault();

            //Items
            AddItem(1, 2500);
            AddItem(2, 10);
            AddItem(5050, 10);
            AddItem(5000, 30);
            AddItem(1000, 1);
            AddItem(700, 20);
            AddItem(5500, 15);
            AddItem(7000, 1);
            AddItem(7001, 1);
            AddItem(7002, 1);

            for (int i = 0; i < classes.Pathways.Count; i++)
                pathwayButtons.Add(new GenericButton(new MenuButton(Vector2.Zero, longButton, longButtonClick, longButtonHover, 1f), classes.Pathways[i].Item1, classes.Pathways[i].Item2));
            selectedPathway = pathwayButtons.FirstOrDefault().Text;

            for (int i = 0; i < classes.Birthplaces.Count; i++)
                birthplaceButtons.Add(new GenericButton(new MenuButton(Vector2.Zero, longButton, longButtonClick, longButtonHover, 1f), classes.Birthplaces[i].Item1, classes.Birthplaces[i].Item2));
            selectedBirthplace = birthplaceButtons.FirstOrDefault().Text;
        }
        private void AddItem(int id, int quantity)
        {
            BaseItem item = ItemDatabase.Item(id);

            if (item != null)
            {
                item.CurrentAmount = quantity;
                items.Add(item);
            }
            else
                Logger.AppendLine("Warning: empty item ID detected at NewGame screen.");
        }

        private void UpdateBeginMessage()
        {
            playerName = typing.ReturnText.ToString();
            startConfirm.SetMessage(playerName + " the " + SelectedClass.Text + ", born at " + selectedBirthplace + " under the " + selectedPathway + " path.");
        }

        public void Reset() { typing.DeleteAllText(); }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            scissorClasses = new Rectangle((int)Position.X + 18, (int)Position.Y + 83 + 19, buttonContainer.Width - 6, 169);
            scissorItems = new Rectangle((int)Position.X + 18, (int)Position.Y + 275 + 19, 515, 133);
            scissorPathway = new Rectangle((int)Position.X + 18, (int)Position.Y + 431 + 19, buttonContainer.Width - 6, 169);
            scissorBirthplace = new Rectangle((int)Position.X + 280, (int)Position.Y + 431 + 19, buttonContainer.Width - 6, 169);
            scissorBottom = new Rectangle((int)Position.X + 18, (int)Position.Y + 431 + 19, 515, 169);

            if (warnEmptyName.IsActive == false && startConfirm.IsActive == false)
                UpdateTyping(gt);

            UpdateClasses(gt);
            UpdateItems(gt);
            UpdatePathways(gt);
            UpdateBirthplaces(gt);

            UpdateButtons(gt);

            controls.UpdateLast();
        }
        private void UpdateTyping(GameTime gt)
        {
            typing.UpdateInput(gt);

            if (typing.ReturnText.Length > 20)
            {
                string name = typing.text.ToString().Substring(0, 20);

                typing.text.Clear();
                typing.text.Append(name);
            }

            randomName.Position = (Position + new Vector2(505, 51)).ToPoint();
            randomName.Update(gt, controls);

            if (randomName.IsLeftClicked == true)
            {
                typing.text.Clear();
                typing.text.Append(randomNames[random.Next(0, randomNames.Length)]);
            }

            if (randomName.IsHover == true)
                ToolTip.RequestStringAssign("Randomize Name");
        }

        private void UpdateClasses(GameTime gt)
        {
            SmoothScroll(gt, 30f, 250f, 300f, 10f, ref classScroll, ref classScrollValue, ref classScrollVelocity, -(((classButtons.Count * 21) + 1) - scissorClasses.Height), scissorClasses);

            for (int i = 0; i < classButtons.Count; i++)
            {
                classButtons[i].Button.Position = (Position + new Vector2(20, 103 + (i * 21) + classScroll)).ToPoint();
                classButtons[i].CheckButton(gt, controls, this, !(startConfirm.IsActive || warnEmptyName.IsActive));

                if ((startConfirm.IsActive || warnEmptyName.IsActive) == false)
                {
                    if (scissorClasses.Contains(controls.MousePosition))
                    {
                        if (classButtons[i].Button.IsLeftClicked)
                            SelectedClass = classButtons[i];
                    }
                }
            }
        }

        private void UpdateItems(GameTime gt)
        {
            SmoothScroll(gt, 30f, 250f, 300f, 10f, ref itemScroll, ref itemScrollValue, ref itemScrollVelocity, -(((items.Count / 7) * 67) - scissorItems.Height), scissorItems); //(itemData.Count - 1) / 7) * (iconBG.Height + 1))
            ApplyProperItemGrid();

            if (scissorItems.Contains(controls.MousePosition))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if ((startConfirm.IsActive || warnEmptyName.IsActive) == false)
                    {
                        if (items[i].itemRect.Contains(controls.MousePosition))
                        {
                            if (selectedItems.Contains(items[i]))
                            {
                                ToolTip.RequestStringAssign(items[i].Name + " \n\nRight-click to remove.");

                                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                                    selectedItems.Remove(items[i]);
                            }
                            else
                            {
                                if (selectedItems.Count < maxItems)
                                {
                                    ToolTip.RequestStringAssign(items[i].Name + " \n\nLeft-click to select.");

                                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                                        selectedItems.Add(items[i]);
                                }
                                else
                                {
                                    ToolTip.RequestStringAssign(items[i].Name + " \n\nYou cannot select anymore items.");
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ApplyProperItemGrid()
        {
            if (items.Count > 6)
            {
                for (int index = 0; index < items.Count; ++index)
                {
                    items[index].gridLocation = new Point(index % 7, (int)(index / 7));
                    items[index].itemRect = new Rectangle((items[index].gridLocation.X * 67) + ((int)Position.X + 45),
                                                          (items[index].gridLocation.Y * 67) + ((int)Position.Y + 295), 64, 64);
                }
            }
            else if (items.Count < 7)
            {
                for (int index = 0; index < items.Count; ++index)
                {
                    items[index].gridLocation = new Point(index, 0);
                    items[index].itemRect = new Rectangle((items[index].gridLocation.X * 67) + ((int)Position.X + 45),
                                                          (items[index].gridLocation.Y * 67) + ((int)Position.Y + 295), 64, 64);
                }
            }
        }

        private void UpdatePathways(GameTime gt)
        {
            SmoothScroll(gt, 30f, 250f, 300f, 10f, ref pathwayScroll, ref pathwayScrollValue, ref pathwayScrollVelocity, -(((pathwayButtons.Count * 21) + 1) - scissorPathway.Height), scissorPathway);

            for (int i = 0; i < pathwayButtons.Count; i++)
            {
                pathwayButtons[i].Button.Position = (Position + new Vector2(20, 451 + (i * 21) + pathwayScroll)).ToPoint();
                pathwayButtons[i].CheckButton(gt, controls, this, !(startConfirm.IsActive || warnEmptyName.IsActive));

                if (scissorPathway.Contains(controls.MousePosition))
                {
                    if ((startConfirm.IsActive || warnEmptyName.IsActive) == false)
                    {
                        if (pathwayButtons[i].Button.IsLeftClicked == true)
                            selectedPathway = pathwayButtons[i].Text;
                    }
                }
            }
        }
        private void UpdateBirthplaces(GameTime gt)
        {
            SmoothScroll(gt, 30f, 250f, 300f, 10f, ref birthScroll, ref birthplaceScrollValue, ref birthplaceScrollVelocity, -(((birthplaceButtons.Count * 21) + 1) - scissorBirthplace.Height), scissorBirthplace);

            for (int i = 0; i < birthplaceButtons.Count; i++)
            {
                birthplaceButtons[i].Button.Position = (Position + new Vector2(282, 451 + (i * 21) + birthScroll)).ToPoint();
                birthplaceButtons[i].CheckButton(gt, controls, this, !(startConfirm.IsActive || warnEmptyName.IsActive));

                if (scissorBirthplace.Contains(controls.MousePosition))
                {
                    if ((startConfirm.IsActive || warnEmptyName.IsActive) == false)
                    {
                        if (birthplaceButtons[i].Button.IsLeftClicked == true)
                            selectedBirthplace = birthplaceButtons[i].Text;
                    }
                }
            }
        }

        private void UpdateButtons(GameTime gt)
        {
            beginGame.Position = (Position + new Vector2(21, 626)).ToPoint();
            returnMain.Position = (Position + new Vector2(283, 626)).ToPoint();

            beginGame.Update(gt, controls);
            returnMain.Update(gt, controls);

            startConfirm.Update(gt);
            warnEmptyName.Update(gt);

            if (warnEmptyName.CurrentSelection == 0)
                warnEmptyName.IsActive = false;

            if (startConfirm.CurrentSelection == 0)
            {
                isStartNewGame = true;
            }
            if (startConfirm.CurrentSelection == 1)
                startConfirm.IsActive = false;

            if ((startConfirm.IsActive || warnEmptyName.IsActive) == false)
            {
                if (beginGame.IsLeftClicked == true)
                {
                    UpdateBeginMessage();

                    if (!string.IsNullOrEmpty(playerName))
                        startConfirm.IsActive = true;
                    else
                        warnEmptyName.IsActive = true;
                }

                if (returnMain.IsLeftClicked == true)
                    main.GoToMainScreen();
            }

            if (isStartNewGame == true)
            {
                screens.EFFECTS_BeginTransition(ScreenEffects.TransitionType.Fade, Color.White, 1000, 2f, 1f);

                if (screens.EFFECTS_IsTransitionFaded == true)
                {
                    StartNewGame();
                    isStartNewGame = false;
                }
            }
        }

        private bool isStartNewGame = false;
        private void StartNewGame()
        {
            classItems = classes.RetrieveClass(SelectedClass.Text).Item3;

            for (int i = 0; i < classItems.Length; i++)
            {
                BaseItem item = ItemDatabase.Item(classItems[i].X).Copy();
                item.CurrentAmount = classItems[i].Y;

                selectedItems.Add(item);
            }

            string mapName = classes.RetrieveBirthplace(selectedBirthplace).Item3;
            Point startTile = classes.RetrieveBirthplace(selectedBirthplace).Item4;
            int pathwayID = classes.RetrievePathway(selectedPathway).Item3;

            world.StartNewGame(playerName, SelectedClass.Text, SelectedClass.Skillset, selectedItems, mapName, startTile, selectedPathway, pathwayID, selectedBirthplace);
            startConfirm.IsActive = false;
        }

        float classScrollValue = 0f; float classScrollVelocity = 0f, itemScrollValue, itemScrollVelocity, pathwayScrollValue, pathwayScrollVelocity, birthplaceScrollValue, birthplaceScrollVelocity;
        private void SmoothScroll(GameTime gt, float scrollSpeed, float maxScrollSpeed, float scrollSlowdown, float clampSpeed, ref float scrollPosition, ref float scrollValue, ref float scrollVelocity, float longBounds, Rectangle container)
        {
            if (container.Contains(controls.MousePosition))
            {
                if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                    scrollVelocity -= scrollSpeed;
                else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                    scrollVelocity += scrollSpeed;
            }

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

            if (longBounds >= 0f)
                longBounds = 0f;

            if (scrollPosition > 0f)
                scrollVelocity = 0f;
            else if (scrollPosition < longBounds)
                scrollVelocity = 0f;

            scrollPosition = MathHelper.Clamp(scrollPosition, longBounds, 0f);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(bg, Position, Color.White);

            //Name textbox
            sb.DrawString(boldFont, "Character Name", Position + new Vector2(276, 38), "Character Name".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);
            sb.DrawBoxBordered(pixel, new Rectangle((int)Position.X + 18, (int)Position.Y + 55, 518, 23), new Color(32, 32, 32, 255), ColorHelper.Charcoal);
            sb.DrawString(font, typing.ReturnText.ToString() + "|", new Vector2(Position.X + 22, Position.Y + 59), Color.White);
            randomName.DrawButton(sb, Color.White);
            sb.Draw(randomIcon, randomName.Position.ToVector2(), Color.White);

            //Classes
            sb.Draw(buttonContainer, Position + new Vector2(15, 83), Color.White);
            sb.DrawString(boldFont, "Character Class", Position + new Vector2(145, 93), "Character Class".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

            //Skills
            sb.Draw(skillsContainer, Position + new Vector2(277, 83), Color.White);
            sb.DrawString(boldFont, SelectedClass.Text + "'s Levels", Position + new Vector2(403, 93), (SelectedClass.Text + "'s Levels").LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

            DrawSkills(sb);

            //Items 45 295
            sb.Draw(itemContainer, Position + new Vector2(15, 275), Color.White);
            string itemText = "Items (" + selectedItems.Count + " of " + maxItems + ")";
            sb.DrawString(boldFont, itemText, Position + new Vector2(267, 285), itemText.LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

            //Pathways
            sb.Draw(buttonContainer, Position + new Vector2(15, 431), Color.White);
            sb.DrawString(boldFont, "Pathway", Position + new Vector2(145, 441), "Pathway".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

            //Birthplaces
            sb.Draw(buttonContainer, Position + new Vector2(277, 431), Color.White);
            sb.DrawString(boldFont, "Birthplace", Position + new Vector2(403, 441), "Birthplace".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);

            //Buttons
            beginGame.DrawButton(sb, Color.White);
            returnMain.DrawButton(sb, Color.White);

            DrawButtonText(sb, beginGame, "Start Pilgrimage");
            DrawButtonText(sb, returnMain, "Back");

            //Header text
            sb.DrawString(boldFont, "New Game", Position + new Vector2(276, 12), "New Game".LineCenter(boldFont), ColorHelper.UI_Gold, 1f);
        }

        private void DrawSkills(SpriteBatch sb)
        {
            sb.DrawString(font, "Health", Position + new Vector2(285, 103), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.health.Level.ToString(), Position + new Vector2(525, 103), new Vector2(font.MeasureString(SelectedClass.Skillset.health.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Endurance", Position + new Vector2(285, 117), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.endurance.Level.ToString(), Position + new Vector2(525, 117), new Vector2(font.MeasureString(SelectedClass.Skillset.endurance.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Agility", Position + new Vector2(285, 131), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.agility.Level.ToString(), Position + new Vector2(525, 131), new Vector2(font.MeasureString(SelectedClass.Skillset.agility.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Defense", Position + new Vector2(285, 145), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.resistance.Level.ToString(), Position + new Vector2(525, 145), new Vector2(font.MeasureString(SelectedClass.Skillset.resistance.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Strength", Position + new Vector2(285, 159), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.strength.Level.ToString(), Position + new Vector2(525, 159), new Vector2(font.MeasureString(SelectedClass.Skillset.strength.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Archery", Position + new Vector2(285, 173), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.archery.Level.ToString(), Position + new Vector2(525, 173), new Vector2(font.MeasureString(SelectedClass.Skillset.archery.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Magic", Position + new Vector2(285, 189), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.wisdom.Level.ToString(), Position + new Vector2(525, 189), new Vector2(font.MeasureString(SelectedClass.Skillset.wisdom.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Intelligence", Position + new Vector2(285, 201), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.intelligence.Level.ToString(), Position + new Vector2(525, 201), new Vector2(font.MeasureString(SelectedClass.Skillset.intelligence.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Trapping", Position + new Vector2(285, 215), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.trapping.Level.ToString(), Position + new Vector2(525, 215), new Vector2(font.MeasureString(SelectedClass.Skillset.trapping.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Awareness", Position + new Vector2(285, 229), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.awareness.Level.ToString(), Position + new Vector2(525, 229), new Vector2(font.MeasureString(SelectedClass.Skillset.awareness.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Concealment", Position + new Vector2(285, 243), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.concealment.Level.ToString(), Position + new Vector2(525, 243), new Vector2(font.MeasureString(SelectedClass.Skillset.concealment.Level.ToString()).X, 0), Color.White, 1f);

            sb.DrawString(font, "Looting", Position + new Vector2(285, 257), Color.White);
            sb.DrawString(font, SelectedClass.Skillset.looting.Level.ToString(), Position + new Vector2(525, 257), new Vector2(font.MeasureString(SelectedClass.Skillset.looting.Level.ToString()).X, 0), Color.White, 1f);
        }

        private Rectangle scissorClasses, scissorItems, scissorPathway, scissorBirthplace, scissorBottom;
        public void DrawScissors(SpriteBatch sb)
        {
            DrawInside(sb, scissorClasses, () =>
            {
                for (int i = 0; i < classButtons.Count; i++)
                {
                    if (classButtons[i] == SelectedClass)
                        classButtons[i].Button.IsHover = true;

                    classButtons[i].Button.DrawButton(sb, Color.White);
                    DrawButtonText(sb, classButtons[i].Button, classButtons[i].Text);
                }
            });

            DrawInside(sb, scissorItems, () =>
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (selectedItems.Contains(items[i]) || items[i].itemRect.Contains(controls.MousePosition))
                        sb.Draw(itemButtonHover, items[i].itemRect.Location.ToVector2(), Color.White);
                    else
                        sb.Draw(itemButton, items[i].itemRect.Location.ToVector2(), Color.White);

                    sb.Draw(items[i].Icon, items[i].itemRect, Color.White);

                    if (items[i].CurrentAmount > 1)
                        sb.DrawString(font, items[i].CurrentAmount.ToString(), items[i].itemRect.Location.ToVector2() + new Vector2(4, 4), Color.White);
                }
            });

            DrawInside(sb, scissorBottom, () =>
            {
                for (int i = 0; i < pathwayButtons.Count; i++)
                {
                    if (pathwayButtons[i].Text.ToUpper().Equals(selectedPathway.ToUpper()))
                        pathwayButtons[i].Button.IsHover = true;

                    pathwayButtons[i].Button.DrawButton(sb, Color.White);
                    DrawButtonText(sb, pathwayButtons[i].Button, pathwayButtons[i].Text);
                }

                for (int i = 0; i < birthplaceButtons.Count; i++)
                {
                    if (birthplaceButtons[i].Text.ToUpper().Equals(selectedBirthplace.ToUpper()))
                        birthplaceButtons[i].Button.IsHover = true;

                    birthplaceButtons[i].Button.DrawButton(sb, Color.White);
                    DrawButtonText(sb, birthplaceButtons[i].Button, birthplaceButtons[i].Text);
                }
            });
        }
        private void DrawButtonText(SpriteBatch sb, MenuButton button, string text)
        {
            if (button.IsHover)
                sb.DrawString(font, text, button.Position.ToVector2() + longButton.Center(), text.LineCenter(font), ColorHelper.UI_Gold, 1f);
            else if (button.IsButtonPressed)
                sb.DrawString(font, text, button.Position.ToVector2() + longButton.Center(), text.LineCenter(font), ColorHelper.UI_DarkerGold, 1f);
            else
                sb.DrawString(font, text, button.Position.ToVector2() + longButton.Center(), text.LineCenter(font), Color.White, 1f);
        }

        public void DrawScreen(SpriteBatch sb)
        {
            startConfirm.Draw(sb);
            warnEmptyName.Draw(sb);
        }

        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };
        private void DrawInside(SpriteBatch sb, Rectangle scissor, Action contents)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissor;

            contents.Invoke();

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }
    }
}
