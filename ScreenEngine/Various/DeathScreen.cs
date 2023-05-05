using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine.Objects.Soulgates;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Screen_based;
using Pilgrimage_Of_Embers.TileEngine;

namespace Pilgrimage_Of_Embers.ScreenEngine.Various
{
    public class DeathScreen
    {
        Controls controls = new Controls();

        private Texture2D pixel, separator;
        private SpriteFont counterFont, largeFont;
        private float fadeLerp = 0f;
        private int waitAfterDeath = 0, askContinueTime = 0, lostExp = 0;
        private bool askForContinue = false, sentToCheckpoint = false, initiateDeath = false;

        private DeathEmbers embers = new DeathEmbers();

        private Random random;

        private BaseEntity player;
        private ScreenManager screens;
        private WorldManager world;
        private TileMap map;
        private Camera camera;

        ShaderType blur;

        public DeathScreen()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public void SetReferences(WorldManager world, TileMap map, BaseEntity player, ScreenManager screens, Camera camera)
        {
            this.world = world;
            this.map = map;
            this.player = player;
            this.screens = screens;
            this.camera = camera;
        }

        public void Load(ContentManager cm)
        {
            counterFont = cm.Load<SpriteFont>("Fonts/emberCounterFont");
            largeFont = cm.Load<SpriteFont>("Fonts/titleFont");

            pixel = cm.Load<Texture2D>("rect");
            separator = cm.Load<Texture2D>("Interface/Various/separator");

            embers.Load(cm);
            embers.IsManualDepth = true;
            embers.IsActivated = false;

            blur = world.GetShader("Blur");
        }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            UpdateState(gt);

            controls.UpdateLast();
        }
        private void UpdateState(GameTime gt)
        {
            fadeLerp = MathHelper.Clamp(fadeLerp, 0f, 1f);

            UpdateLerp(gt);

            embers.Update(gt);
            embers.Offset = new Vector2(GameSettings.VectorCenter.X - 150, GameSettings.VectorResolution.Y + 50);
            embers.IsActivated = (waitAfterDeath >= 4000);

            if (sentToCheckpoint == true)
                SendToCheckpoint();

            if (player.IsDead == true)
            {
                if (initiateDeath == false)
                {
                    screens.HUD_EquipTargetLerp = 0f;
                    screens.HUD_QuickslotTargetLerp = 0f;
                    initiateDeath = true;
                }

                if (isZoomApplied == false)
                {
                    camera.SmoothZoom(1.15f, 1f, true, 0);
                    isZoomApplied = true;
                }

                lostExp = player.Skills.ExperiencePoints;
                waitAfterDeath += gt.ElapsedGameTime.Milliseconds;

                if (controls.IsKeyPressedOnce(controls.CurrentControls.Activate))
                    waitAfterDeath = 4000;

                if (waitAfterDeath >= 4000)
                {
                    map.FadeVariables();
                    screens.EFFECTS_BeginTransition(ScreenEffects.TransitionType.Fade, Color.Black, 100000000, 1f, 1f);

                    if (screens.EFFECTS_IsTransitionFaded == true)
                    {
                        fadeLerp += 1f * (float)gt.ElapsedGameTime.TotalSeconds;

                        if (fadeLerp >= 1f)
                        {
                            if (askForContinue == false)
                            {
                                askContinueTime += gt.ElapsedGameTime.Milliseconds;

                                if (askContinueTime >= 1000)
                                {
                                    askForContinue = true;
                                    askContinueTime = 0;
                                }
                            }
                            else
                            {
                                if (controls.IsKeyPressedOnce(controls.CurrentControls.Activate))
                                {
                                    sentToCheckpoint = true;
                                    askForContinue = false;
                                }
                            }
                        }
                    }
                }
            }
            else
                fadeLerp -= 2f * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        private bool isZoomApplied = false;
        private void SendToCheckpoint()
        {
            BaseCheckpoint warpGate = MonumentDatabase.Soulgate(((PlayerEntity)player).CurrentSoulgateID);

            if (warpGate == null) warpGate = MonumentDatabase.Soulgate(1);

            warpGate.LoadMapTo();
            player.SOULGATE_Rest();
            player.SKILL_ClearEmbers();

            camera.SmoothZoom(1f, 1f, true, 0);

            screens.HUD_EquipTargetLerp = 1f;
            screens.HUD_QuickslotTargetLerp = 1f;

            screens.EFFECTS_ForceEndTransition();

            ResetDeathScreen();
        }

        private void ResetDeathScreen()
        {
            isZoomApplied = false;
            askForContinue = false;
            sentToCheckpoint = false;
            initiateDeath = false;

            waitAfterDeath = 0;
        }

        private bool isContinueFlashing = false, isMovingSep1Up = false, isMovingSep2Up = true; private float continueFlashLerp = 0f, moveSeparatorLerp1 = 0f, moveSeparatorLerp2 = 0f;
        private void UpdateLerp(GameTime gt)
        {
            counterColor = Color.Lerp(Color.Transparent, ColorHelper.UI_Gold, fadeLerp - .1f);
            counterFontColor = Color.Lerp(Color.Transparent, new Color(168, 89, 33, 255), fadeLerp - .1f);
            fontColor = Color.Lerp(Color.Transparent, Color.White, fadeLerp - .1f);
            separatorColor = Color.Lerp(Color.Transparent, Color.White, fadeLerp - .5f);
            edgingColor = Color.Lerp(Color.Transparent, new Color(127, 58, 12, 255), fadeLerp - .6f);

            if (isContinueFlashing == true)
                continueFlashLerp += .5f * (float)gt.ElapsedGameTime.TotalSeconds;
            if (isContinueFlashing == false)
                continueFlashLerp -= .5f * (float)gt.ElapsedGameTime.TotalSeconds;

            continueFlashLerp = MathHelper.Clamp(continueFlashLerp, 0f, 1f);

            if (continueFlashLerp >= 1f)
                isContinueFlashing = false;
            if (continueFlashLerp <= 0f)
                isContinueFlashing = true;

            if (askForContinue == false)
                continueFlashLerp = 0f;

            continueColor = Color.Lerp(Color.Transparent, Color.White, continueFlashLerp + .15f);


            //Sep1
            if (isMovingSep1Up == true)
                moveSeparatorLerp1 += .25f * (float)gt.ElapsedGameTime.TotalSeconds;
            if (isMovingSep1Up == false)
                moveSeparatorLerp1 -= .25f * (float)gt.ElapsedGameTime.TotalSeconds;

            moveSeparatorLerp1 = MathHelper.Clamp(moveSeparatorLerp1, 0f, 1f);

            if (moveSeparatorLerp1 >= 1f)
                isMovingSep1Up = false;
            if (moveSeparatorLerp1 <= 0f)
                isMovingSep1Up = true;

            //Sep2
            if (isMovingSep2Up == true)
                moveSeparatorLerp2 += .25f * (float)gt.ElapsedGameTime.TotalSeconds;
            if (isMovingSep2Up == false)
                moveSeparatorLerp2 -= .25f * (float)gt.ElapsedGameTime.TotalSeconds;

            moveSeparatorLerp2 = MathHelper.Clamp(moveSeparatorLerp2, 0f, 1f);

            if (moveSeparatorLerp2 >= 1f)
                isMovingSep2Up = false;
            if (moveSeparatorLerp2 <= 0f)
                isMovingSep2Up = true;

            sep1Pos = Vector2.Lerp(new Vector2(GameSettings.VectorCenter.X - 25, GameSettings.VectorCenter.Y - 200), new Vector2(GameSettings.VectorCenter.X + 25, GameSettings.VectorCenter.Y - 200), moveSeparatorLerp1);
            sep2Pos = Vector2.Lerp(new Vector2(GameSettings.VectorCenter.X + 25, GameSettings.VectorCenter.Y - 200), new Vector2(GameSettings.VectorCenter.X - 25, GameSettings.VectorCenter.Y - 200), moveSeparatorLerp1);

            sep3Pos = Vector2.Lerp(new Vector2(GameSettings.VectorCenter.X - 25, GameSettings.VectorCenter.Y - 25), new Vector2(GameSettings.VectorCenter.X + 25, GameSettings.VectorCenter.Y - 25), moveSeparatorLerp2);
            sep4Pos = Vector2.Lerp(new Vector2(GameSettings.VectorCenter.X + 25, GameSettings.VectorCenter.Y - 25), new Vector2(GameSettings.VectorCenter.X - 25, GameSettings.VectorCenter.Y - 25), moveSeparatorLerp2);
        }

        private Color counterColor, fontColor, continueColor, separatorColor, counterFontColor, edgingColor;
        private Vector2 sep1Pos, sep2Pos, sep3Pos, sep4Pos;

        private const string headerText = "As your sight fades, you feel a familiar force take its tithe.";
        private const string footerText = "You find yourself back at the monument.";

        private Vector2 counterOffset = new Vector2(0, 100);
        public void Draw(SpriteBatch sb)
        {
            if (screens.EFFECTS_TransitionLerp > 0f)
            {
                embers.Draw(sb);

                //Header text
                sb.DrawString(largeFont, headerText, new Vector2(GameSettings.VectorCenter.X, 200), headerText.LineCenter(largeFont), fontColor, 1f);
                sb.DrawString(largeFont, footerText, new Vector2(GameSettings.VectorCenter.X, 240), footerText.LineCenter(largeFont), fontColor, 1f);

                //Separator
                sb.Draw(separator, sep1Pos + counterOffset, fontColor, separator.Center(), 0f, 1f);
                sb.Draw(separator, sep2Pos + counterOffset, separatorColor, separator.Center(), 0f, 1f, SpriteEffects.FlipHorizontally, 1f);

                //Lost ember count
                sb.DrawString(largeFont, "Embers Lost", new Vector2(GameSettings.VectorCenter.X, GameSettings.VectorCenter.Y - 125) + counterOffset, "Embers Lost".LineCenter(largeFont), counterFontColor, 1f);
                sb.DrawString(counterFont, lostExp.CommaSeparation(), new Vector2(GameSettings.VectorCenter.X, GameSettings.VectorCenter.Y - 85) + counterOffset, lostExp.CommaSeparation().LineCenter(counterFont), counterColor, 1f);

                //Separator 2
                sb.Draw(separator, sep3Pos + counterOffset, fontColor, separator.Center(), 0f, 1f);
                sb.Draw(separator, sep4Pos + counterOffset, separatorColor, separator.Center(), 0f, 1f, SpriteEffects.FlipHorizontally, 1f);

                //Continue Text
                if (askForContinue == true)
                    sb.DrawString(largeFont, "Press 'E' to return", new Vector2(GameSettings.VectorCenter.X, GameSettings.VectorResolution.Y - 200), "Press 'E' to return".LineCenter(largeFont), continueColor, 1f);

                sb.DrawBoxBordered(pixel, new Rectangle(50, 50, GameSettings.WindowResolution.X - 100, GameSettings.WindowResolution.Y - 100), Color.Transparent, edgingColor);
            }
        }
    }
}
