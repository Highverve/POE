using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.TileEngine.Objects.Soulgates;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ParticleEngine;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    public class SoulgateInterface
    {
        Controls controls = new Controls();

        private Texture2D button, buttonHover, mediumButton, mediumButtonHover, background, bgEffect1, bgEffect2, silverLock, goldLock;
        private SpriteFont font, headerFont;
        private Vector2 windowPosition;
        private float circleDistance = 0f, effectAngle = 0f;
        private string gateName;

        private SoulgateButton menderButton, soulsButton, levelUpButton,
                               stoneholdButton, teleportButton, anvilButton,
                               brewerButton, craftButton, smelterButton, imbueEssenceButton, empty2, rejuvenateButton;

        private List<SoulgateButton> activeButtons = new List<SoulgateButton>();

        private bool HasMender = true, HasStonehold = true, HasTeleport = true, HasAnvil = true, HasSmelter = true;

        public bool IsActive { get; set; }

        BaseEntity controlledEntity;
        Camera camera;
        TileMap tileMap;
        ScreenManager screens;

        private BaseCheckpoint currentSoulgate, lastSoulgate;

        private BaseEmitter circleParticle1, circleParticle2;

        public SoulgateInterface()
        {
        }
        public void SetReferences(Camera Camera, TileMap Maps, ScreenManager Screens)
        {
            camera = Camera;
            tileMap = Maps;
            screens = Screens;
        }

        private string directory = "Interface/Soulgate/";
        public void Load(ContentManager cm)
        {
            button = cm.Load<Texture2D>("Interface/Global/iconBG");
            buttonHover = cm.Load<Texture2D>("Interface/Global/iconBGSelect");

            mediumButton = cm.Load<Texture2D>("Interface/Global/mediumButtonBG");
            mediumButtonHover = cm.Load<Texture2D>("Interface/Global/mediumButtonBGSelect");

            silverLock = cm.Load<Texture2D>(directory + "Icons/lockMedium");
            goldLock = cm.Load<Texture2D>(directory + "Icons/lockLarge");

            background = cm.Load<Texture2D>(directory + "gateBG");
            bgEffect1 = cm.Load<Texture2D>(directory + "gateBGEffect1");
            bgEffect2 = cm.Load<Texture2D>(directory + "gateBGEffect2");

            font = cm.Load<SpriteFont>("Fonts/BoldInterface");
            headerFont = cm.Load<SpriteFont>("Fonts/LargeHeader");

            SetButtonValues(cm);

            circleParticle1 = new ParticleEngine.EmitterTypes.Screen_based.SoulgateCircle();
            circleParticle1.Load(cm);
            circleParticle1.IsManualDepth = true;

            circleParticle2 = new ParticleEngine.EmitterTypes.Screen_based.SoulgateCircle();
            circleParticle2.Load(cm);
            circleParticle2.IsManualDepth = true;

            RefreshInterfaceOptions();
        }

        public void SetSoulgate(BaseCheckpoint gate)
        {
            currentSoulgate = gate;
            lastSoulgate = currentSoulgate;
            gateName = gate.Name;

            //screens.INTEGERDISPLAY_AddInteger("Monsters revived", currentSoulgate.Position, Color.AliceBlue, controlledEntity.BaseCurrentFloor + 1, true);
        }
        public void SetControlledEntity(BaseEntity controlledEntity) { this.controlledEntity = controlledEntity; }

        private void SetButtonValues(ContentManager cm)
        {
            #region Large Buttons
            levelUpButton = new SoulgateButton("Fortify Skills", -90f, cm.Load<Texture2D>(directory + "Icons/FortifySkills"), button, buttonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.SKILLS_InvertState();
            });
            levelUpButton.IsPrimevalButton = true;
            levelUpButton.IsUnlocked = true;

            soulsButton = new SoulgateButton("Soulforge", 0f, null, button, buttonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.SOULS_InvertState();
            });
            soulsButton.IsPrimevalButton = false;
            soulsButton.IsUnlocked = false;

            teleportButton = new SoulgateButton("Soul Warping", 180f, cm.Load<Texture2D>(directory + "Icons/SoulWarp"), button, buttonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.SOULWARP_InvertState();
            });
            teleportButton.IsPrimevalButton = true;

            imbueEssenceButton = new SoulgateButton("Imbue Essence", 90f, cm.Load<Texture2D>(directory + "Icons/imbueEssence"), button, buttonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.IMBUE_Invert();
            });
            imbueEssenceButton.IsPrimevalButton = false;
            imbueEssenceButton.IsUnlocked = true;

            #endregion
            #region Medium Buttons
            craftButton = new SoulgateButton("Artisan's Counter", -60f, cm.Load<Texture2D>(directory + "Icons/ArtisansCounter"), mediumButton, mediumButtonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.OFFERINGS_InvertState();
            });
            craftButton.IsPrimevalButton = false;
            craftButton.IsUnlocked = true;

            smelterButton = new SoulgateButton("Ore Smelter", -30f, cm.Load<Texture2D>(directory + "Icons/OreSmelter"), mediumButton, mediumButtonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.SMELTER_InvertState();
            });
            smelterButton.IsPrimevalButton = true;

            menderButton = new SoulgateButton("Mender's Tools", 30f, cm.Load<Texture2D>(directory + "Icons/MendersTools"), mediumButton, mediumButtonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.MENDER_InvertState();
            });
            menderButton.IsPrimevalButton = false;

            anvilButton = new SoulgateButton("Reinforcer's Workbench", 60f, cm.Load<Texture2D>(directory + "Icons/ReinforcersWorkbench"), mediumButton, mediumButtonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.ANVIL_InvertState();
            });
            anvilButton.IsPrimevalButton = true;

            stoneholdButton = new SoulgateButton("Access Stonehold", 150f, cm.Load<Texture2D>(directory + "Icons/Stonehold"), mediumButton, mediumButtonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.STONEHOLD_InvertState();
            });
            stoneholdButton.IsPrimevalButton = true;

            brewerButton = new SoulgateButton("Brewmaster's Contrivances", 120f, cm.Load<Texture2D>(directory + "Icons/Brewing"), mediumButton, mediumButtonHover, (SoulgateInterface i) =>
            {
                screens.PlaySound("Button Click 6");
                screens.BREWING_Invert();
            });
            brewerButton.IsPrimevalButton = false;
            brewerButton.IsUnlocked = true;

            empty2 = new SoulgateButton("Empty", -150f, null, mediumButton, mediumButtonHover, (SoulgateInterface i) => { });

            rejuvenateButton = new SoulgateButton("", -120f, cm.Load<Texture2D>(directory + "Icons/Rejuvenate"), mediumButton, mediumButtonHover, (SoulgateInterface i) => { });
            rejuvenateButton.IsPrimevalButton = false;
            rejuvenateButton.IsUnlocked = false;

            #endregion

            teleportButton.IsUnlocked = HasTeleport;
            anvilButton.IsUnlocked = HasAnvil;
            menderButton.IsUnlocked = HasMender;
            smelterButton.IsUnlocked = HasSmelter;
            stoneholdButton.IsUnlocked = HasStonehold;
        }
        public void RefreshInterfaceOptions(bool forcePrimeval = false)
        {
            activeButtons.Clear();

            activeButtons.Add(levelUpButton);
            activeButtons.Add(soulsButton);
            activeButtons.Add(teleportButton);
            activeButtons.Add(brewerButton);

            activeButtons.Add(craftButton);
            activeButtons.Add(smelterButton);
            activeButtons.Add(menderButton);
            activeButtons.Add(anvilButton);
            activeButtons.Add(stoneholdButton);

            activeButtons.Add(imbueEssenceButton);
            activeButtons.Add(empty2);
            activeButtons.Add(rejuvenateButton);
        }

        public void CloseAll()
        {
            screens.OFFERINGS_IsActive = false;
            screens.STONEHOLD_IsActive = false;
            screens.MENDER_IsActive = false;
            screens.SMELTER_IsActive = false;
            screens.ANVIL_IsActive = false;
            screens.SOULWARP_IsActive = false;
            screens.IMBUE_IsActive = false;
            screens.BREWING_IsActive = false;

            IsActive = false;
            currentSoulgate = null;
        }

        private bool isLerpSet = false;
        public void Update(GameTime gt)
        {
            controls.UpdateLast();
            controls.UpdateCurrent();

            float distanceToPlayer = Vector2.Distance(windowPosition, controlledEntity.Position);
            if (distanceToPlayer <= 600f)
            {
                if (IsActive == true)
                {
                    if (isLerpSet == false)
                    {
                        camera.SetCameraState(Camera.CameraState.Cinematic);
                        camera.LookAt(windowPosition);
                        camera.DelaySpeed = 3f;

                        isLerpSet = true;
                    }
                }
                else if (IsActive == false)
                {
                    if (isLerpSet == true)
                    {
                        camera.SetCameraState(Camera.CameraState.Current);
                        camera.DelaySpeed = 3f;

                        isLerpSet = false;
                    }
                }
            }

            if (fadeScale > 0f)
            {
                CheckDistance();
                UpdateButtons(gt);
                UpdateParticles(gt);
            }

            if (currentSoulgate == null)
                CloseAll();

            circleParticle1.IsActivated = IsActive;
            circleParticle2.IsActivated = IsActive;

            CheckFade(gt);
        }
        private void UpdateParticles(GameTime gt)
        {
            circleParticle1.Offset = Circle.Rotate(effectAngle * 40f, circleDistance, windowPosition);
            circleParticle1.Update(gt);

            circleParticle2.Offset = Circle.Rotate(-effectAngle * 40f, circleDistance, windowPosition);
            circleParticle2.Update(gt);
        }

        private void CheckDistance()
        {
            if (currentSoulgate != null)
            {
                windowPosition = currentSoulgate.Position;

                if (Vector2.Distance(currentSoulgate.Position, controlledEntity.Position) >= 200f)
                    currentSoulgate = null;
            }
        }

        private bool isTabHover = false;
        private void UpdateButtons(GameTime gt)
        {
            isTabHover = false;

            for (int i = 0; i < activeButtons.Count; i++)
            {
                activeButtons[i].ButtonPosition = Circle.Rotate(activeButtons[i].Angle, circleDistance + activeButtons[i].DistanceOffset, windowPosition);

                activeButtons[i].ButtonRect = new Rectangle((int)activeButtons[i].ButtonPosition.X - activeButtons[i].BackgroundTexture.Width / 2,
                                                            (int)activeButtons[i].ButtonPosition.Y - activeButtons[i].BackgroundTexture.Height / 2,
                                                            activeButtons[i].BackgroundTexture.Width,
                                                            activeButtons[i].BackgroundTexture.Height);

                if (IsActive == true)
                {
                    if (activeButtons[i].ButtonRect.Contains(camera.ScreenToWorld(controls.MouseVector)))
                    {
                        isTabHover = true;

                        if (activeButtons[i].IsUnlocked)
                        {
                            if (!(activeButtons[i].IsPrimevalButton == true && currentSoulgate.IsPrimeval == false))
                            {
                                ToolTip.RequestStringAssign(activeButtons[i].Name);
                                activeButtons[i].DistanceLerp += 2f * (float)gt.ElapsedGameTime.TotalSeconds;
                                activeButtons[i].IsHover = true;

                                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                                    activeButtons[i].OnButtonClick(this);
                            }
                            else
                                ToolTip.RequestStringAssign("You must visit a primeval gate to use this.");
                        }
                        else
                            ToolTip.RequestStringAssign("You must unlock this before you can use it.");
                    }
                    else
                    {
                        activeButtons[i].IsHover = false;
                        activeButtons[i].DistanceLerp -= 2f * (float)gt.ElapsedGameTime.TotalSeconds;
                    }

                    activeButtons[i].DistanceLerp = MathHelper.Clamp(activeButtons[i].DistanceLerp, 0f, 1f);
                    activeButtons[i].DistanceOffset = MathHelper.SmoothStep(0f, -3f, activeButtons[i].DistanceLerp);
                }
            }
        }
        private void CheckFade(GameTime gt)
        {
            effectAngle += .25f * (float)gt.ElapsedGameTime.TotalSeconds;

            if (IsActive == false)
                fadeScale -= 4f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                fadeScale += 4f * (float)gt.ElapsedGameTime.TotalSeconds;

            fadeScale = MathHelper.Clamp(fadeScale, 0f, 1f);

            circleDistance = MathHelper.SmoothStep(0f, 150f, fadeScale);
            fadeColor = Color.Lerp(Color.Transparent, Color.White, fadeScale - .1f);
            effectColor = Color.Lerp(Color.Transparent, Color.White, fadeScale - .5f);
            inactiveButton = Color.Lerp(Color.Transparent, Color.White, fadeScale - .5f);
        }

        private Color fadeColor = Color.White, effectColor = Color.White, inactiveButton = Color.White; private float fadeScale = .0001f;
        public void Draw(SpriteBatch sb)
        {
            if (fadeScale > 0f)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.View(Vector2.One));

                sb.Draw(bgEffect1, windowPosition, effectColor, bgEffect1.Center(), effectAngle, fadeScale, 1f);
                sb.Draw(bgEffect2, windowPosition, effectColor, bgEffect2.Center(), -effectAngle, fadeScale, 1f);
                sb.Draw(background, windowPosition, fadeColor, background.Center(), 0f, fadeScale, 1f);

                circleParticle1.Draw(sb);
                circleParticle2.Draw(sb);

                for (int i = 0; i < activeButtons.Count; i++)
                {
                    if (activeButtons[i].IsHover == true)
                        sb.Draw(activeButtons[i].BackgroundTextureHover, activeButtons[i].ButtonPosition, fadeColor, activeButtons[i].BackgroundTextureHover.Center(), 0f, 1f);
                    else
                        sb.Draw(activeButtons[i].BackgroundTexture, activeButtons[i].ButtonPosition, fadeColor, activeButtons[i].BackgroundTexture.Center(), 0f, 1f);

                    if (activeButtons[i].ButtonIcon != null && currentSoulgate != null && activeButtons[i].IsUnlocked == true)
                    {
                        if ((activeButtons[i].IsPrimevalButton == true && currentSoulgate.IsPrimeval == true) ||
                            (activeButtons[i].IsPrimevalButton == false && currentSoulgate.IsPrimeval == false) ||
                            (activeButtons[i].IsPrimevalButton == false && currentSoulgate.IsPrimeval == true))
                            sb.Draw(activeButtons[i].ButtonIcon, activeButtons[i].ButtonPosition, fadeColor, activeButtons[i].ButtonIcon.Center(), 0f, 0f);
                        else if (activeButtons[i].IsPrimevalButton == true && currentSoulgate.IsPrimeval == false)
                            sb.Draw(activeButtons[i].ButtonIcon, activeButtons[i].ButtonPosition, inactiveButton, activeButtons[i].ButtonIcon.Center(), 0f, 0f);
                    }

                    if (activeButtons[i].IsUnlocked == false)
                        sb.Draw(silverLock, activeButtons[i].ButtonPosition, fadeColor, silverLock.Center(), 0f, 0f);
                }

                sb.End();
            }
        }

        public string SaveData()
        {
            return "UNLOCKS " + teleportButton.IsUnlocked.ToString() + " " + anvilButton.IsUnlocked.ToString() + " " +
                                menderButton.IsUnlocked.ToString() + " " + smelterButton.IsUnlocked.ToString() + " " +
                                stoneholdButton.IsUnlocked.ToString();
        }
        public void LoadData(string data)
        {
            string[] words = data.Split(' ');

            if (words[0].ToUpper().Equals("UNLOCKS"))
            {
                try
                {
                    HasTeleport = bool.Parse(words[1]);
                    HasAnvil = bool.Parse(words[2]);
                    HasMender = bool.Parse(words[3]);
                    HasSmelter = bool.Parse(words[4]);
                    HasStonehold = bool.Parse(words[5]);
                }
                catch(Exception e)
                {
                    Logger.AppendLine("Error Parsing soulgate button bools. " + e.Message);
                }
            }
        }

        public bool IsSoulgateNear()
        {
            if (lastSoulgate != null)
                return lastSoulgate.IsEntityClose;

            return false;
        }

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return isTabHover;
            else
                return false;
        }
    }
}
