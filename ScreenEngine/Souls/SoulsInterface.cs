using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.Souls.Types;
using Pilgrimage_Of_Embers.Entities;

namespace Pilgrimage_Of_Embers.ScreenEngine.Souls
{
    public class SoulsInterface
    {
        private float minAngle = -2.05f, maxAngle; //Set these values
        private float radius = 246, soulBGAngle, scrollPosition = -2f;

        private Vector3 dragArea; //x = x, y = y, z = radius
        private Vector2 ScreenCenter;
        private Vector2 offset;
        private Vector2 Offset { set { offset = new Vector2(MathHelper.Clamp(value.X, -GameSettings.VectorCenter.X + 300, GameSettings.VectorCenter.X - 300), MathHelper.Clamp(value.Y, -GameSettings.VectorCenter.X + 300, GameSettings.VectorCenter.Y - 300)); } }

        private Texture2D ringBG, ringFade, iconBG, iconSelected, paneBG, levelBG, topPiece, topPieceBG, newSoul, pixel, button;
        private Texture2D soulBG1, soulBG2, soulBG3, reinforceIcon, zIcon, xIcon, cIcon, vIcon, essenseIcon;
        private Texture2D[] romanNumerals = new Texture2D[10];
        private Circle soulButtonOne, soulButtonTwo, soulButtonThree, soulButtonFour, reinforceButton; //Probably should change over to drag based, eventually.

        BasicAnimation animation = new BasicAnimation();

        private SpriteFont font, largeFont;

        private Controls controls = new Controls();
        private bool isActive = false;
        public bool IsActive { get { return isActive; } set { isActive = value; } }

        private List<BaseSoul> souls = new List<BaseSoul>();
        public List<BaseSoul> Souls { get { return souls; } }

        private ScreenManager screens;

        /// <summary>
        /// Determines how many "Essense of Souls" items you need to level up
        /// </summary>
        private int[] soulLeveling = new int[] //Adjust this if necessary
        {
            0,  //Level 1 - Starting level requires no leveling up.
            1,  //2
            1,  //3
            2,  //4
            2,  //5
            3,  //6
            3,  //7
            4,  //8
            5,  //9
            6   //10
        };

        private EntityEquipment equipment;
        private EntityStorage storage;

        public SoulsInterface() { }
        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;
        }

        public void SetSoulData(EntityEquipment equipment, EntityStorage storage)
        {
            this.equipment = equipment;
            this.storage = storage;
            souls = storage.Souls;
        }

        public bool IsSoulSelected()
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].IsSelected == true)
                    return true;
            }

            return false;
        }
        private BaseSoul SelectedSoul()
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].IsSelected == true)
                    return souls[i];
            }
            return null;
        }

        public BaseSoul RetrieveSoul(int id)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                if (souls[i].ID == id)
                    return souls[i];
            }
            return null;
        }

        private const string directory = "Interface/Souls/";

        public void Load(ContentManager cm)
        {
            ringBG = cm.Load<Texture2D>(directory + "soulsRing");
            ringFade = cm.Load<Texture2D>(directory + "soulsRingBlur");

            iconBG = cm.Load<Texture2D>(directory + "iconBG");
            iconSelected = cm.Load<Texture2D>(directory + "iconBGSelected");
            paneBG = cm.Load<Texture2D>(directory + "paneBG");
            levelBG = cm.Load<Texture2D>(directory + "levelBG");
            topPiece = cm.Load<Texture2D>(directory + "levelBG");
            topPieceBG = cm.Load<Texture2D>(directory + "levelBG2");

            newSoul = cm.Load<Texture2D>("Interface/Shared/newIcon");

            soulBG1 = cm.Load<Texture2D>(directory + "soulBG1");
            soulBG2 = cm.Load<Texture2D>(directory + "soulBG2");
            soulBG3 = cm.Load<Texture2D>(directory + "soulBG3");

            essenseIcon = cm.Load<Texture2D>("Items/Icons/Misc/Essenses/essenseOfSouls");

            font = cm.Load<SpriteFont>("Fonts/BoldInterface");
            largeFont = cm.Load<SpriteFont>("Fonts/LargeHeader");

            pixel = cm.Load<Texture2D>("rect");
            button = cm.Load<Texture2D>(directory + "buttonBG");
            reinforceIcon = cm.Load<Texture2D>(directory + "reinforceIcon");
            zIcon = cm.Load<Texture2D>(directory + "zIcon");
            xIcon = cm.Load<Texture2D>(directory + "xIcon");
            cIcon = cm.Load<Texture2D>(directory + "cIcon");
            vIcon = cm.Load<Texture2D>(directory + "vIcon");

            soulButtonOne = new Circle(Vector2.Zero, 17.5f);
            soulButtonTwo = new Circle(Vector2.Zero, 17.5f);
            soulButtonThree = new Circle(Vector2.Zero, 17.5f);
            soulButtonFour = new Circle(Vector2.Zero, 17.5f);
            reinforceButton = new Circle(Vector2.Zero, 17.5f);

            for (int i = 0; i < 10; i++)
            {
                romanNumerals[i] = cm.Load<Texture2D>(directory + "roman" + (i + 1).ToString());
            }

            ScreenCenter = GameSettings.VectorCenter;

            //AddSoul(1);
            //AddSoul(2);
            //AddSoul(3);
        }

        Point currentFrame;
        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            currentFrame = animation.FramePosition(gt, 55, new Point(8, 1), true);

            UpdateDrag(gt);

            //UpdateState();

            if (isActive == true)
            {
                soulBGAngle += .1f * (float)gt.ElapsedGameTime.TotalSeconds;

                Fade(gt);
                UpdateSouls(gt);
                UpdateButtons();

                if (souls.Count >= 20)
                    Scroll(gt);
            }
            else
            {
                for (int i = 0; i < souls.Count; i++)
                {
                    souls[i].Update(gt);
                }
            }

            controls.UpdateLast();
        }
        private void UpdateState()
        {
            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenSouls))
                isActive = !isActive;
        }
        private void UpdateSouls(GameTime gt)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                souls[i].Update(gt);

                if (checkSoulClick == true)
                {
                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        if (Vector2.Distance(controls.MouseVector, souls[i].Position) <= (iconBG.Width / 2))
                        {
                            if (souls[i].IsDisplay == true)
                            {
                                souls[i].IsSelected = true;
                                souls[i].IsNew = false;
                            }
                        }
                        else
                            souls[i].IsSelected = false;
                    }
                }

                if (souls[i].Rotation >= -1.7f || souls[i].Rotation <= -7.85f)
                    souls[i].IsDisplay = false;
                else
                    souls[i].IsDisplay = true;
            }
        }

        bool isDragging = false;
        private void UpdateDrag(GameTime gt)
        {
            dragArea = new Vector3(ScreenCenter.X + offset.X, (ScreenCenter.Y + offset.Y) - (radius + 50), 48);

            if (Vector2.Distance(new Vector2(dragArea.X, dragArea.Y), controls.MouseVector) <= dragArea.Z &&
                controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
                Offset = (controls.MouseVector - ScreenCenter) + new Vector2(0, radius + 50);
        }

        private bool checkSoulClick = true;
        private void UpdateButtons()
        {
            reinforceButton.Position = ScreenCenter + offset - new Vector2(1, 165);

            soulButtonTwo.Position = ScreenCenter + offset - new Vector2(35, 203);
            soulButtonOne.Position = ScreenCenter + offset - new Vector2(55, 245);

            soulButtonThree.Position = ScreenCenter + offset - new Vector2(-35, 203);
            soulButtonFour.Position = ScreenCenter + offset - new Vector2(-55, 245);

            if (reinforceButton.Contains(controls.MouseVector) ||
                soulButtonTwo.Contains(controls.MouseVector) ||
                soulButtonOne.Contains(controls.MouseVector) ||
                soulButtonThree.Contains(controls.MouseVector) ||
                soulButtonFour.Contains(controls.MouseVector))
            {
                checkSoulClick = false;
            }
            else
                checkSoulClick = true;

            if (reinforceButton.Contains(controls.MouseVector))
            {
                ToolTip.RequestStringAssign("Reinforce Soul");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    if (isSoulSelected == true)
                        storage.ReinforceSoul(SelectedSoul().ID);
                }
            }

            CheckButtonEquip("Equip Slot One", soulButtonOne, selectedSoul, EntityEquipment.SoulSlot.One);
            CheckButtonEquip("Equip Slot Two", soulButtonTwo, selectedSoul, EntityEquipment.SoulSlot.Two);
            CheckButtonEquip("Equip Slot Three", soulButtonThree, selectedSoul, EntityEquipment.SoulSlot.Three);
            CheckButtonEquip("Equip Slot Four", soulButtonFour, selectedSoul, EntityEquipment.SoulSlot.Four);
        }
        private void CheckButtonEquip(string text, Circle button, BaseSoul soul, EntityEquipment.SoulSlot slot)
        {
            if (button.Contains(controls.MouseVector))
            {
                if (screens.SOULGATE_IsNear())
                {
                    ToolTip.RequestStringAssign(text);

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                        equipment.EquipSoul(soul, slot);
                }
                else
                {
                    ToolTip.RequestStringAssign("Must be near a soulgate.");
                }
            }
        }

        float scrollValue = 0f; float scrollVelocity = 0f;
        private void Scroll(GameTime gt)
        {
            if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                scrollVelocity -= .5f;
            else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                scrollVelocity += .5f;

            scrollValue = controls.CurrentMS.ScrollWheelValue;

            //Smooth scrolling code
            scrollPosition += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -6f, 6f);

            if (scrollVelocity > .02f)
                scrollVelocity -= 1.5f * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -.02f)
                scrollVelocity += 1.5f * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -.02f && scrollVelocity <= .02f)
                scrollVelocity = 0f;

            maxAngle = (minAngle + ((souls.Count * .3f) - (18 * .3f)));
            scrollPosition = MathHelper.Clamp(scrollPosition, minAngle, maxAngle);

            if (scrollPosition >= maxAngle || scrollPosition <= minAngle)
                scrollVelocity = 0f;
        }

        float soulMovement;
        Color fadeColor = Color.White, bgColor = Color.Lerp(Color.Transparent, Color.White, .5f);
        bool moveSoulUp = false;

        private void Fade(GameTime gt)
        {
            if (moveSoulUp == true)
                soulMovement += .5f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                soulMovement -= .5f * (float)gt.ElapsedGameTime.TotalSeconds;

            if (soulMovement >= 1f)
                moveSoulUp = false;
            else if (soulMovement <= 0f)
                moveSoulUp = true;

            if (isSoulSelected == true)
            {
                soulScaleMultiplier += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                fadeInfo += 4f * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                soulScaleMultiplier -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                fadeInfo -= 4f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            soulScaleMultiplier = MathHelper.Clamp(soulScaleMultiplier, 1f, 1.2f);
            fadeInfo = MathHelper.Clamp(fadeInfo, 0f, 1f);

            soulMovement = MathHelper.Clamp(soulMovement, 0f, 1f);
        }

        public void Draw(SpriteBatch sb)
        {
            if (isActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(ringFade, ScreenCenter + offset, Color.Lerp(fadeColor, Color.Transparent, .75f), ringFade.Center(), 0f, 1f, 1f);

                sb.Draw(topPieceBG, ScreenCenter + offset - new Vector2(1, 250), fadeColor, topPieceBG.Center(), 0f, 1f, 1f);

                sb.Draw(ringBG, ScreenCenter + offset, fadeColor, ringBG.Center(), soulBGAngle, 1f, 1f);
                sb.Draw(ringBG, ScreenCenter + offset, fadeColor, ringBG.Center(), -soulBGAngle, 1f, 1f);

                DrawCircularSouls(sb);
                DrawSoulInfo(sb);

                sb.Draw(topPiece, ScreenCenter + offset - new Vector2(0, 240), Color.White, topPiece.Center(), 0f, 1f, 1f);

                DrawButtons(sb);

                if (GameSettings.IsDebugging == true)
                    sb.DrawString(font, scrollPosition.ToString(), Vector2.Zero, Color.White);

                sb.End();
            }
        }
        private void DrawButtons(SpriteBatch sb)
        {
            sb.Draw(button, reinforceButton.Position, fadeColor, button.Center(), 0f, 1f);
            sb.Draw(reinforceIcon, reinforceButton.Position, fadeColor, reinforceIcon.Center(), 0f, 1f);

            sb.Draw(button, soulButtonOne.Position, fadeColor, button.Center(), 0f, 1f);
            sb.Draw(zIcon, soulButtonOne.Position, fadeColor, zIcon.Center(), 0f, 1f);

            sb.Draw(button, soulButtonTwo.Position, fadeColor, button.Center(), 0f, 1f);
            sb.Draw(xIcon, soulButtonTwo.Position, fadeColor, xIcon.Center(), 0f, 1f);

            sb.Draw(button, soulButtonThree.Position, fadeColor, button.Center(), 0f, 1f);
            sb.Draw(cIcon, soulButtonThree.Position, fadeColor, cIcon.Center(), 0f, 1f);

            sb.Draw(button, soulButtonFour.Position, fadeColor, button.Center(), 0f, 1f);
            sb.Draw(vIcon, soulButtonFour.Position, fadeColor, vIcon.Center(), 0f, 1f);
        }
        private void DrawCircularSouls(SpriteBatch sb)
        {
            for (int i = 0; i < souls.Count; i++)
            {
                souls[i].Rotation = scrollPosition + (-.3f * i);

                if (souls[i].IsDisplay == true)
                {
                    souls[i].Position = Circle.Rotate(souls[i].Rotation, radius, ScreenCenter + offset);

                    if (souls[i].IsSelected == false)
                    {
                        sb.Draw(iconBG, souls[i].Position, fadeColor, iconBG.Center(), 0f, 1f, 1f);
                        sb.Draw(souls[i].Icon, souls[i].Position, fadeColor, souls[i].Icon.Center(), 0f, 1f, 1f);
                    }
                    else
                    {
                        sb.Draw(iconSelected, souls[i].Position, fadeColor, iconSelected.Center(), 0f, 1f, 1f);
                        sb.Draw(souls[i].Icon, souls[i].Position, fadeColor, souls[i].Icon.Center(), 0f, 1f * soulScaleMultiplier, 1f);
                    }

                    if (souls[i].IsNew == true)
                        sb.Draw(newSoul, souls[i].Position - new Vector2(22, 22), new Rectangle(currentFrame.X * 9, currentFrame.Y * 9, 9, 9), fadeColor); 

                    if (Vector2.Distance(controls.MouseVector, souls[i].Position) <= (iconBG.Width / 2) && checkSoulClick == true)
                        ToolTip.RequestStringAssign(souls[i].Name + " : Level " + souls[i].SoulLevel);
                }
            }
        }

        private float fadeInfo = 0f, soulScaleMultiplier = 1f;
        private Color fadeInfoColor, fadeColorWhite, fadeColorBlack;
        bool isSoulSelected = false;

        BaseSoul selectedSoul; List<string> splitDescription = new List<string>();
        private void DrawSoulInfo(SpriteBatch sb)
        {
            fadeInfoColor = Color.Lerp(Color.Transparent, new Color(.3f, .3f, .3f, .3f), fadeInfo);
            fadeColorWhite = Color.Lerp(Color.Transparent, Color.White, fadeInfo);
            fadeColorBlack = Color.Lerp(Color.Transparent, Color.Black, fadeInfo);

            sb.Draw(soulBG3, ScreenCenter + offset - new Vector2(0, 70), fadeInfoColor, soulBG3.Center(), soulBGAngle + 2f, 1f, 1f);
            sb.Draw(soulBG1, ScreenCenter + offset - new Vector2(0, 70), fadeInfoColor, soulBG1.Center(), soulBGAngle, 1f, 1f);
            sb.Draw(soulBG2, ScreenCenter + offset - new Vector2(0, 70), fadeInfoColor, soulBG2.Center(), -soulBGAngle, 1f, 1f);

            if (IsSoulSelected() == true)
            {
                selectedSoul = SelectedSoul();
                isSoulSelected = true;
            }
            else
                isSoulSelected = false;

            if (selectedSoul != null)
            {
                if (font.MeasureString(selectedSoul.Description).X >= 300)
                    splitDescription = selectedSoul.Description.SplitLines(font, 300);
                else
                {
                    splitDescription.Clear();
                    splitDescription.Add(selectedSoul.Description);
                }

                sb.Draw(selectedSoul.LargeIcon, Vector2.SmoothStep(ScreenCenter + offset - new Vector2(0, 80), ScreenCenter + offset - new Vector2(0, 70), soulMovement), Color.Lerp(Color.Transparent, Color.White, fadeInfo), selectedSoul.LargeIcon.Center(), 0f, 1f, 1f);

                sb.DrawBoxBordered(pixel, new Rectangle((int)(ScreenCenter.X + offset.X) - 160, (int)(ScreenCenter.Y + offset.Y) + 46, 320, 100), Color.Lerp(Color.Transparent, new Color(15, 15, 15, 155), fadeInfo), fadeColorBlack);

                sb.DrawStringBordered(largeFont, selectedSoul.Name, ScreenCenter + offset + new Vector2(0, 30), selectedSoul.Name.LineCenter(largeFont), 0f, 1f, 1f, fadeColorWhite, fadeColorBlack);
                for (int i = 0; i < splitDescription.Count; i++)
                {
                    sb.DrawStringBordered(font, splitDescription[i], ScreenCenter + offset + new Vector2(0, 60 + (i * font.LineSpacing)), splitDescription[i].LineCenter(font), 0f, 1f, 1f, fadeColorWhite, fadeColorBlack);
                }

                sb.Draw(romanNumerals[selectedSoul.SoulLevel - 1], ScreenCenter + offset + new Vector2(0, 10), Color.Lerp(Color.Transparent, new Color(.9f, .9f, .9f, .9f), fadeInfo), romanNumerals[selectedSoul.SoulLevel - 1].Center(), 0f, 1f, 1f);

                //sb.DrawStringBordered(largeFont, "Level " + selectedSoul.SoulLevel, ScreenCenter + Offset + new Vector2(-120, -130), fadeColorWhite, fadeColorBlack);

                if (selectedSoul.IsSelected == true)
                {
                    if (Vector2.Distance(controls.MouseVector, ScreenCenter + offset + new Vector2(-120, -110)) <= 32)
                        ToolTip.RequestStringAssign("'Essense of Souls' required");
                }

                sb.Draw(essenseIcon, ScreenCenter + offset + new Vector2(-120, -110), fadeColorWhite, essenseIcon.Center(), 0f, 1f);

                if (selectedSoul.SoulLevel < 10)
                    sb.DrawStringBordered(largeFont, soulLeveling[selectedSoul.SoulLevel].ToString(), ScreenCenter + offset + new Vector2(-120, -110), soulLeveling[selectedSoul.SoulLevel].ToString().LineCenter(largeFont), 0f, 1f, 1f, fadeColorWhite, fadeColorBlack);
                else
                    sb.DrawStringBordered(largeFont, "MAX", ScreenCenter + offset + new Vector2(-120, -110), "MAX".LineCenter(largeFont), 0f, 1f, 1f, fadeColorWhite, fadeColorBlack);
            }
        }
    }
}
