using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.Entities.Types;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    class MeterFlash
    {
        public Rectangle StartRect { get; set; }
        public Color StartColor { get; set; }
        public bool IsCompleted { get; private set; }

        Rectangle currentRect;
        Color color;
        float lerp, offset;

        public MeterFlash(Rectangle Rect, Color StartColor, float Offset)
        {
            this.StartRect = Rect;
            this.StartColor = StartColor;

            offset = Offset;
            lerp = 0f;
        }

        public void Update(GameTime gt)
        {
            lerp += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
            lerp = MathHelper.Clamp(lerp, 0f, 1f);

            currentRect.X = (int)MathHelper.SmoothStep(StartRect.X, StartRect.X - offset, lerp);
            currentRect.Y = (int)MathHelper.SmoothStep(StartRect.Y, StartRect.Y - offset, lerp);
            currentRect.Width = (int)MathHelper.SmoothStep(StartRect.Width, StartRect.Width + (offset * 2), lerp);
            currentRect.Height = (int)MathHelper.SmoothStep(StartRect.Height, StartRect.Height + (offset * 2), lerp);

            color = Color.Lerp(StartColor, Color.Transparent, lerp);

            if (lerp >= 1f)
                IsCompleted = true;
        }

        public void DrawFlash(SpriteBatch sb, Texture2D pixel)
        {
            sb.DrawBoxBordered(pixel, currentRect, Color.Transparent, color, 1);
        }
    }

    public class EntityHUD
    {
        private Vector2 offset;
        public Vector2 Offset { get { return offset; } }

        Texture2D pixel, mainBG, xpMeter, button, buttonHover, coloredButton, coloredButton2, meterForeground, barFillerHP, barFillerMP, barFillerSTA, meterMarker, companionBar, companionFiller;
        Texture2D inventoryIcon, magicIcon, soulsIcon, rumorsIcon, statsIcon, settingsIcon, pauseIcon, unfilledIcon, timePiece, rect, money;
        SpriteFont largeFont, font;

        Circle inventoryButton, magicButton, soulsButton, rumorsButton, statsButton, settingsButton, pauseButton, companionButton, timeDisplay;

        private bool isInventoryHover, isSpellbookHover, isSoulsHover, isRumorsHover, isPauseHover, isStatsHover, isSettingsHover;
        private bool isMinimalistHUD = false, isHiddenHUD;

        private Color healthColor, staminaColor, magicColor, mainColor, iconColor, hideColor;
        private float mainOpacity, hideOpacity;
        public float MainOpacity { get { return mainOpacity; } }

        private ScreenManager screens;
        private BaseEntity controlledEntity;
        private WorldLight ambient;
        private CultureManager culture;

        List<MeterFlash> flashes = new List<MeterFlash>();

        private ParticleEngine.EmitterTypes.Screen_based.DepleteMeter hpEmitter, staEmitter, mpEmitter;

        Controls controls = new Controls();

        public void ResetPositions()
        {
            //if (timeChart != null)
            //    timeChart.Initialize();
        }

        public EntityHUD(Game game)
        {
            //timeChart = new PieChart(game, 0f, 14, MathHelper.ToRadians(360f), 30, Color.Transparent, Color.Black, false);
            //timeChart.Initialize();
        }

        public void SetReferences(ScreenManager screens, BaseEntity entity, WorldLight ambient, CultureManager culture)
        {
            this.screens = screens;
            controlledEntity = entity;
            this.ambient = ambient;
            this.culture = culture;
        }
        public void SetEntity(BaseEntity entity) { this.controlledEntity = entity; }

        private const string directory = "Interface/HUD/";
        public void LoadContent(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");

            mainBG = cm.Load<Texture2D>(directory + "bg3");
            xpMeter = cm.Load<Texture2D>(directory + "Meters/xpMeter");

            button = cm.Load<Texture2D>(directory + "button");
            buttonHover = cm.Load<Texture2D>(directory + "buttonHover");
            coloredButton = cm.Load<Texture2D>(directory + "colorButton");
            coloredButton2 = cm.Load<Texture2D>(directory + "colorButton2");

            meterForeground = cm.Load<Texture2D>("Interface/HUD/Meters/ironBar");

            barFillerHP = cm.Load<Texture2D>("Interface/HUD/Meters/barBackgroundHP");
            barFillerMP = cm.Load<Texture2D>("Interface/HUD/Meters/barBackgroundMP");
            barFillerSTA = cm.Load<Texture2D>("Interface/HUD/Meters/barBackgroundSTA");

            meterMarker = cm.Load<Texture2D>("Interface/HUD/Meters/meterMarker");

            companionBar = cm.Load<Texture2D>(directory + "Meters/companionBar");
            companionFiller = cm.Load<Texture2D>(directory + "Meters/companionBackground");

            money = cm.Load<Texture2D>("Interface/HUD/Icons/money");

            largeFont = cm.Load<SpriteFont>("Fonts/titleFont");
            font = cm.Load<SpriteFont>("Fonts/boldOutlined");

            inventoryIcon = cm.Load<Texture2D>("Interface/HUD/Icons/inventory");
            magicIcon = cm.Load<Texture2D>("Interface/HUD/Icons/magic");
            soulsIcon = cm.Load<Texture2D>("Interface/HUD/Icons/flames");
            rumorsIcon = cm.Load<Texture2D>("Interface/HUD/Icons/rumorsNotes");
            statsIcon = cm.Load<Texture2D>("Interface/HUD/Icons/skills");
            settingsIcon = cm.Load<Texture2D>("Interface/HUD/Icons/settings");
            pauseIcon = cm.Load<Texture2D>("Interface/HUD/Icons/pause");
            unfilledIcon = cm.Load<Texture2D>(directory + "Icons/invalid");

            timePiece = cm.Load<Texture2D>(directory + "Icons/timePiece");

            rect = cm.Load<Texture2D>("rect");

            offset = new Vector2(32, 32);

            inventoryButton = new Circle(new Vector2(offset.X - 18, offset.Y + 95) + this.button.Center(), 14);
            magicButton = new Circle(new Vector2(offset.X + 3, offset.Y + 116) + this.button.Center(), 14);
            soulsButton = new Circle(new Vector2(offset.X + 24, offset.Y + 137) + this.button.Center(), 14);
            rumorsButton = new Circle(new Vector2(offset.X + 45, offset.Y + 158) + this.button.Center(), 14);

            pauseButton = new Circle(new Vector2(offset.X + 158, offset.Y + 95) + this.button.Center(), 14);
            settingsButton = new Circle(new Vector2(offset.X + 137, offset.Y + 116) + this.button.Center(), 14);
            statsButton = new Circle(new Vector2(offset.X + 116, offset.Y + 137) + this.button.Center(), 14);
            companionButton = new Circle(new Vector2(offset.X + 95, offset.Y + 158) + this.button.Center(), 14);

            timeDisplay = new Circle(new Vector2(offset.X + 81, offset.Y + 183) + timePiece.Center(), 8);

            hpEmitter = new ParticleEngine.EmitterTypes.Screen_based.DepleteMeter(baseHealthColor, new Vector2(50, 250), 400);
            hpEmitter.IsManualDepth = true;
            hpEmitter.Load(cm);

            staEmitter = new ParticleEngine.EmitterTypes.Screen_based.DepleteMeter(baseStaminaColor, new Vector2(-100, 50), 300);
            staEmitter.IsManualDepth = true;
            staEmitter.Load(cm);

            mpEmitter = new ParticleEngine.EmitterTypes.Screen_based.DepleteMeter(baseMagicColor, new Vector2(150, -150), 400);
            mpEmitter.IsManualDepth = true;
            mpEmitter.Load(cm);
        }

        int lastHP, lastMP, healthTimer, staminaTimer, magicTimer, lerpedHP, lerpedMP;
        Vector2 hp, mp, sta;
        float hpLerp, mpLerp, staLerp, lastSTA, lerpedSTA;

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            if (isMinimalistHUD == false && isHiddenHUD == false)
                CheckActions();

            CheckFade(gt);
            UpdateCurrentMeters(gt);
            UpdateHUDLayout(gt);

            UpdateHealthFlash(gt);
            UpdateEnduranceFlash(gt);
            UpdateMagicFlash(gt);

            UpdateFlashes(gt);

            UpdateSmoothMeter(gt);
            UpdateParticles(gt);

            controls.UpdateLast();

            lastHP = controlledEntity.Skills.health.CurrentHP;
            lastSTA = (int)controlledEntity.Skills.endurance.CurrentStamina;
            lastMP = controlledEntity.Skills.wisdom.CurrentEnergy;
        }

        private void UpdateHUDLayout(GameTime gt)
        {
            if (Vector2.Distance(Vector2.Zero, controls.MouseVector) <= 450)
            {
                isMinimalistHUD = false;
                fadeTime = 0;
            }

            isHiddenHUD = screens.EFFECTS_IsWidescreen() || GameSettings.IsHidingHUD;
        }

        private void UpdateFlashes(GameTime gt)
        {
            for (int i = 0; i < flashes.Count; i++)
            {
                flashes[i].Update(gt);

                if (flashes[i].IsCompleted == true)
                    flashes.RemoveAt(i);
            }
        }
        private void UpdateHealthFlash(GameTime gt)
        {
            if (lastHP > controlledEntity.Skills.health.CurrentHP && isHiddenHUD == false)
                flashes.Add(new MeterFlash(new Rectangle((int)(offset.X + MathHelper.SmoothStep(26, 112, mainOpacity + .5f)), (int)offset.Y + 20, barFillerHP.Width - 4, barFillerHP.Height - 4), healthColor, 24));

            if (controlledEntity.Skills.health.CurrentHP < (controlledEntity.Skills.health.MaxHP / 4) && controlledEntity.Skills.health.CurrentHP > 0)
            {
                healthTimer += gt.ElapsedGameTime.Milliseconds;

                if (healthTimer >= MathHelper.Clamp((controlledEntity.Skills.health.CurrentHP / ((float)controlledEntity.Skills.health.MaxHP / 4)) * 750, 100, 750))
                {
                    if (isHiddenHUD == false)
                        flashes.Add(new MeterFlash(new Rectangle((int)(offset.X + MathHelper.SmoothStep(26, 112, mainOpacity + .5f)), (int)offset.Y + 20, barFillerHP.Width - 4, barFillerHP.Height - 4), healthColor, 24));

                    healthTimer = 0;
                    screens.PlaySound("HeartbeatHigh");
                }
            }
        }
        private void UpdateEnduranceFlash(GameTime gt)
        {
            if (controlledEntity.Skills.endurance.CurrentStamina < (controlledEntity.Skills.endurance.MaxStamina / 4))
            {
                staminaTimer += gt.ElapsedGameTime.Milliseconds;

                if (staminaTimer >= 1000)
                {
                    if (isHiddenHUD == false)
                        flashes.Add(new MeterFlash(new Rectangle((int)(offset.X + MathHelper.SmoothStep(26, 123, mainOpacity + .5f)), (int)offset.Y + 31, barFillerHP.Width - 4, barFillerHP.Height - 4), staminaColor, 24));

                    staminaTimer = 0;
                    screens.PlayRandom("Breath1", "Breath2", "Breath3", "Breath4", "Breath5");
                }
            }
        }
        private void UpdateMagicFlash(GameTime gt)
        {
            if (lastMP > controlledEntity.Skills.wisdom.CurrentEnergy)
                flashes.Add(new MeterFlash(new Rectangle((int)(offset.X + MathHelper.SmoothStep(26, 135, mainOpacity + .5f)), (int)offset.Y + 42, barFillerHP.Width - 4, barFillerHP.Height - 4), magicColor, 24));

            if (controlledEntity.Skills.wisdom.CurrentEnergy < (controlledEntity.Skills.wisdom.MaxEnergy / 4) && controlledEntity.Skills.wisdom.CurrentEnergy > 0)
            {
                magicTimer += gt.ElapsedGameTime.Milliseconds;

                if (magicTimer >= 2000)
                {
                    if (isHiddenHUD == false)
                        flashes.Add(new MeterFlash(new Rectangle((int)(offset.X + MathHelper.SmoothStep(26, 135, mainOpacity + .5f)), (int)offset.Y + 42, barFillerHP.Width - 4, barFillerHP.Height - 4), magicColor, 24));

                    magicTimer = 0;
                    screens.PlaySound("LowMagic");
                }
            }
        }

        private void UpdateSmoothMeter(GameTime gt)
        {
            if (lastHP != controlledEntity.Skills.health.CurrentHP)
            {
                if (hpLerp != 1f)
                    hp = new Vector2(lerpedHP, controlledEntity.Skills.health.CurrentHP);
                else
                    hp = new Vector2(lastHP, controlledEntity.Skills.health.CurrentHP);

                hpLerp = 0f;
            }

            if (lastSTA != (int)controlledEntity.Skills.endurance.CurrentStamina)
            {
                if (staLerp != 1f)
                    sta = new Vector2(lerpedSTA, controlledEntity.Skills.endurance.CurrentStamina);
                else
                    sta = new Vector2(lastSTA, controlledEntity.Skills.endurance.CurrentStamina);

                staLerp = 0f;
            }

            if (lastMP != controlledEntity.Skills.wisdom.CurrentEnergy)
            {
                if (mpLerp != 1f)
                    mp = new Vector2(lerpedMP, controlledEntity.Skills.wisdom.CurrentEnergy);                
                else
                    mp = new Vector2(lastMP, controlledEntity.Skills.wisdom.CurrentEnergy);

                mpLerp = 0f;
            }

            hpLerp += 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            staLerp += 4f * (float)gt.ElapsedGameTime.TotalSeconds;
            mpLerp += 2f * (float)gt.ElapsedGameTime.TotalSeconds;

            hpLerp = MathHelper.Clamp(hpLerp, 0f, 1f);
            staLerp = MathHelper.Clamp(staLerp, 0f, 1f);
            mpLerp = MathHelper.Clamp(mpLerp, 0f, 1f);

            lerpedHP = (int)MathHelper.SmoothStep(hp.X, hp.Y, hpLerp);
            lerpedSTA = MathHelper.SmoothStep(sta.X, sta.Y, staLerp);
            lerpedMP = (int)MathHelper.SmoothStep(mp.X, mp.Y, mpLerp);
        }
        private void UpdateParticles(GameTime gt)
        {
            hpEmitter.Offset = new Vector2(offset.X + (MathHelper.SmoothStep(26, 112, mainOpacity + .5f) + (barFillerHP.Width * lerpedHP / controlledEntity.Skills.health.MaxHP)), offset.Y + 22);
            staEmitter.Offset = new Vector2(offset.X + (MathHelper.SmoothStep(26, 123, mainOpacity + .5f) + (barFillerSTA.Width * lerpedSTA / controlledEntity.Skills.endurance.MaxStamina)), offset.Y + 33);
            mpEmitter.Offset = new Vector2(offset.X + (MathHelper.SmoothStep(26, 134, mainOpacity + .5f) + (barFillerMP.Width * lerpedMP / controlledEntity.Skills.wisdom.MaxEnergy)), offset.Y + 43);

            hpEmitter.Update(gt);
            staEmitter.Update(gt);
            mpEmitter.Update(gt);

            if (isHiddenHUD == false)
            {
                hpEmitter.IsActivated = (hpLerp != 1f) && (hp.X > hp.Y);
                staEmitter.IsActivated = (staLerp != 1f) && (sta.X > sta.Y);
                mpEmitter.IsActivated = (mpLerp != 1f) && (mp.X > mp.Y);
            }
        }

        private void CheckActions()
        {
            CheckForInventoryClick();
            CheckForSpellbookClick();
            //CheckForSoulsClick();
            CheckForRumorsClick();

            CheckForPauseClick();
            CheckForSkillsClick();
            CheckForOptionsclick();

            if (timeDisplay.Contains(controls.MouseVector))
            {
                string time = string.Empty;

                time += "Hour " + culture.CALENDAR_Hours.ToString() + " of the " + culture.CALENDAR_Days + Strings.NumberEnding(culture.CALENDAR_Days) + " day";
                time += Environment.NewLine + "Aghtene of " + culture.CALENDAR_CurrentAghtene().Name;
                time += Environment.NewLine + "The " + culture.CALENDAR_Passes.ToString() + Strings.NumberEnding(culture.CALENDAR_Passes) + " pass"; 

                Holiday h = culture.CALENDAR_CurrentHoliday();
                if (h != null) time += Environment.NewLine + Environment.NewLine + h.Name;

                ToolTip.RequestStringAssign(time);
            }
        }
        private void CheckForInventoryClick()
        {
            if (inventoryButton.Contains(controls.MouseVector))
            {
                isInventoryHover = true;
                ToolTip.RequestStringAssign("Inventory [" + controls.KeyString(controls.CurrentControls.OpenInventory) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 6");
                    screens.INVENTORY_IsActive = !screens.INVENTORY_IsActive;
                }
            }
            else
                isInventoryHover = false;
        }
        private void CheckForSpellbookClick()
        {
            if (magicButton.Contains(controls.MouseVector))
            {
                isSpellbookHover = true;
                ToolTip.RequestStringAssign("Spellbook [" + controls.KeyString(controls.CurrentControls.OpenMagic) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 6");
                    screens.SPELLBOOK_IsActive = !screens.SPELLBOOK_IsActive;
                }
            }
            else
                isSpellbookHover = false;
        }
        private void CheckForSoulsClick()
        {
            if (soulsButton.Contains(controls.MouseVector))
            {
                isSoulsHover = true;
                ToolTip.RequestStringAssign("Souls [" + controls.KeyString(controls.CurrentControls.OpenSouls) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 6");
                    screens.SOULS_IsActive = !screens.SOULS_IsActive;
                }
            }
            else
                isSoulsHover = false;
        }
        private void CheckForRumorsClick()
        {
            if (rumorsButton.Contains(controls.MouseVector))
            {
                isRumorsHover = true;
                ToolTip.RequestStringAssign("Rumors [" + controls.KeyString(controls.CurrentControls.OpenRumors) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 6");
                    screens.RUMOR_IsActive = !screens.RUMOR_IsActive;
                }
            }
            else
                isRumorsHover = false;
        }
        private void CheckForPauseClick()
        {
            if (pauseButton.Contains(controls.MouseVector))
            {
                isPauseHover = true;
                ToolTip.RequestStringAssign("Pause [" + controls.KeyString(controls.CurrentControls.Pause) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 6");
                    screens.InvertPause();
                }
            }
            else
                isPauseHover = false;
        }
        private void CheckForSkillsClick()
        {
            if (statsButton.Contains(controls.MouseVector))
            {
                isStatsHover = true;
                ToolTip.RequestStringAssign("Skills [" + controls.KeyString(controls.CurrentControls.OpenStats) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 6");
                    screens.SKILLS_IsActive = !screens.SKILLS_IsActive;
                }
            }
            else
                isStatsHover = false;
        }
        private void CheckForOptionsclick()
        {
            if (settingsButton.Contains(controls.MouseVector))
            {
                isSettingsHover = true;
                ToolTip.RequestStringAssign("Settings [" + controls.KeyString(controls.CurrentControls.OpenSettings) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 6");
                    screens.OPTIONS_IsActive = !screens.OPTIONS_IsActive;
                }
            }
            else
                isSettingsHover = false;
        }
        /*
        private void CheckButtonClick(Circle button, string text, ref bool isHover, ref bool isActive)
        {
            if (settingsButton.Contains(controls.MouseVector))
            {
                isSettingsHover = true;
                ToolTip.RequestStringAssign("Settings [" + Controls.KeyToString(Controls.OpenSettings) + "]");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick)) screenManager.IsSettingsOpen = !screenManager.IsSettingsOpen;
            }
            else
                isSettingsHover = false;
        }*/

        private int fadeTime = 0;
        private Color baseHealthColor = new Color(178, 102, 98, 128), baseStaminaColor = new Color(137, 178, 133, 128), baseMagicColor = new Color(108, 157, 216, 128);
        private void CheckFade(GameTime gt)
        {
            if (fadeTime >= 5000)
                isMinimalistHUD = true;
            else
                fadeTime += gt.ElapsedGameTime.Milliseconds;

            if (isMinimalistHUD == true)
                mainOpacity -= 7f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                mainOpacity += 7f * (float)gt.ElapsedGameTime.TotalSeconds;
            mainOpacity = MathHelper.Clamp(mainOpacity, -1f, 1f);

            if (isHiddenHUD == true)
                hideOpacity -= 5f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                hideOpacity += 5f * (float)gt.ElapsedGameTime.TotalSeconds;
            hideOpacity = MathHelper.Clamp(hideOpacity, 0f, 1f);

            mainColor = Color.Lerp(Color.Transparent, Color.White, mainOpacity * .95f * hideOpacity);
            iconColor = Color.Lerp(Color.Transparent, Color.White, mainOpacity * hideOpacity);

            healthColor = Color.Lerp(Color.Transparent, baseHealthColor, MathHelper.Clamp(mainOpacity - .15f, .75f, 1f) * hideOpacity);
            staminaColor = Color.Lerp(Color.Transparent, baseStaminaColor, MathHelper.Clamp(mainOpacity - .15f, .75f, 1f) * hideOpacity);
            magicColor = Color.Lerp(Color.Transparent, baseMagicColor, MathHelper.Clamp(mainOpacity - .15f, .75f, 1f) * hideOpacity);

            //For hiding the HUD
            hideColor = Color.Lerp(Color.Transparent, Color.White, hideOpacity);
        }

        private Vector2 hpMeterOffset, staMeterOffset, mpMeterOffset;
        private int currentHPOffset, currentSTAOffset, currentMPOffset;
        private void UpdateCurrentMeters(GameTime gt)
        {
            hpMeterOffset = new Vector2(offset.X + MathHelper.SmoothStep(24, 110, mainOpacity + .5f), offset.Y + 18);
            staMeterOffset = new Vector2(offset.X + MathHelper.SmoothStep(24, 121, mainOpacity + .5f), offset.Y + 29);
            mpMeterOffset = new Vector2(offset.X + MathHelper.SmoothStep(24, 132, mainOpacity + .5f), offset.Y + 40);

            currentHPOffset = barFillerHP.Width * lerpedHP / controlledEntity.Skills.health.MaxHP;
            currentSTAOffset = barFillerSTA.Width * (int)lerpedSTA / controlledEntity.Skills.endurance.MaxStamina;
            currentMPOffset = barFillerMP.Width * lerpedMP / controlledEntity.Skills.wisdom.MaxEnergy;
        }

        private Color lightCharcoal = new Color(64, 64, 64, 255), charcoal = new Color(48, 48, 48, 255);
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(rect, new Rectangle((int)offset.X + (int)MathHelper.SmoothStep(20, 104, mainOpacity + .5f), (int)offset.Y + 13, meterForeground.Width + 28, 1), Color.Lerp(Color.Transparent, lightCharcoal, hideOpacity));
            sb.Draw(rect, new Rectangle((int)offset.X + (int)MathHelper.SmoothStep(20, 104, mainOpacity + .5f), (int)offset.Y + 14, meterForeground.Width + 28, 1), Color.Lerp(Color.Transparent, charcoal, hideOpacity));

            sb.Draw(mainBG, offset, mainColor);

            if (controlledEntity.Faction != null && controlledEntity.Faction.Icon != null)
                sb.Draw(controlledEntity.Faction.Icon, offset + mainBG.Center(), mainColor, controlledEntity.Faction.Icon.Center(), 0f, 1f);

            //sb.DrawString(renownFont, ((int)controlledEntity.SoulRenown).ToString(), offset + mainBG.Center(), ((int)controlledEntity.SoulRenown).ToString().LineCenter(renownFont), mainColor, 1f);
            //sb.Draw(xpMeter, offset + new Vector2(46, 127), mainColor);

            DrawMeters(sb);
            DrawButtons(sb);

            //timeChart.Position = new Vector2(offset.X + 81 + timePiece.Center().X, offset.Y + 184 + timePiece.Center().Y);
            //timeChart.OutsideColor = Color.Lerp(Color.Transparent, Color.Black, mainOpacity * .85f);
            //timeChart.Draw();

            sb.Draw(timePiece, new Vector2(offset.X + 81, offset.Y + 183), mainColor);

            for (int i = 0; i < controlledEntity.Companions.Count; i++)
                DrawCompanionHUD(sb, controlledEntity.Companions[i], (128 * (i + 1)) + MathHelper.SmoothStep(48, 192, mainOpacity + .5f));
        }

        private void DrawMeters(SpriteBatch sb)
        {
            //Current
            sb.Draw(barFillerHP, hpMeterOffset, new Rectangle(0, 0, currentHPOffset, barFillerHP.Height), hideColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(barFillerSTA, staMeterOffset, new Rectangle(0, 0, currentSTAOffset, barFillerSTA.Height), hideColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(barFillerMP, mpMeterOffset, new Rectangle(0, 0, currentMPOffset, barFillerMP.Height), hideColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            //Meter BG
            sb.Draw(meterForeground, new Vector2(offset.X + MathHelper.SmoothStep(24, 110, mainOpacity + .5f), offset.Y + 18), new Rectangle(0, 0, meterForeground.Width, meterForeground.Height), hideColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(meterForeground, new Vector2(offset.X + MathHelper.SmoothStep(24, 121, mainOpacity + .5f), offset.Y + 29), new Rectangle(0, 0, meterForeground.Width, meterForeground.Height), hideColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(meterForeground, new Vector2(offset.X + MathHelper.SmoothStep(24, 132, mainOpacity + .5f), offset.Y + 40), new Rectangle(0, 0, meterForeground.Width, meterForeground.Height), hideColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            //Markers
            if ((controlledEntity.Skills.health.CurrentHP / (float)controlledEntity.Skills.health.MaxHP) < .99f)
                sb.Draw(meterMarker, hpMeterOffset + new Vector2(currentHPOffset - 2, -2), hideColor);
            if ((controlledEntity.Skills.endurance.CurrentStamina / controlledEntity.Skills.endurance.MaxStamina) < .99f)
                sb.Draw(meterMarker, staMeterOffset + new Vector2(currentSTAOffset - 2, -2), hideColor);
            if ((controlledEntity.Skills.wisdom.CurrentEnergy / (float)controlledEntity.Skills.wisdom.MaxEnergy) < .99f)
                sb.Draw(meterMarker, mpMeterOffset + new Vector2(currentMPOffset - 2, -2), hideColor);

            //Emitters
            hpEmitter.Draw(sb);
            staEmitter.Draw(sb);
            mpEmitter.Draw(sb);

            if (isHiddenHUD == false)
            {
                if (controlledEntity is PlayerEntity)
                {
                    sb.DrawStringBordered(largeFont, controlledEntity.Name, new Vector2(offset.X + MathHelper.SmoothStep(37, 115, mainOpacity + .5f), offset.Y - 14), Vector2.Zero, 0f, 1f, 0f, ((PlayerEntity)controlledEntity).Faction.Color, ColorHelper.Charcoal);
                    sb.DrawStringBordered(largeFont, " the " + ((PlayerEntity)controlledEntity).ClassType, new Vector2(offset.X + MathHelper.SmoothStep(37, 115, mainOpacity + .5f) + largeFont.MeasureString(controlledEntity.Name).X, offset.Y - 14), Vector2.Zero, 0f, 1f, 0f, Color.White, ColorHelper.Charcoal);
                }
                else
                    sb.DrawString(largeFont, controlledEntity.Name, new Vector2(offset.X + MathHelper.SmoothStep(37, 115, mainOpacity + .5f), offset.Y - 5), Vector2.Zero, Color.Silver, 1f);
            }

            for (int i = 0; i < flashes.Count; i++)
                flashes[i].DrawFlash(sb, pixel);
        }

        Color bg = Color.Lerp(Color.Black, Color.Transparent, .5f), border = Color.Black;
        private void DrawButtons(SpriteBatch sb)
        {
            DrawButton(sb, inventoryIcon, inventoryButton, screens.INVENTORY_IsActive, isInventoryHover);
            DrawButton(sb, magicIcon, magicButton, screens.SPELLBOOK_IsActive, isSpellbookHover);
            DrawButton(sb, unfilledIcon, soulsButton, false, false);
            DrawButton(sb, rumorsIcon, rumorsButton, screens.RUMOR_IsActive, isRumorsHover);

            DrawButton(sb, pauseIcon, pauseButton, screens.IsPaused(), isPauseHover);
            DrawButton(sb, settingsIcon, settingsButton, screens.OPTIONS_IsActive, isSettingsHover);
            DrawButton(sb, statsIcon, statsButton, screens.SKILLS_IsActive, isStatsHover);
            DrawButton(sb, unfilledIcon, companionButton, false, false);
        }
        private void DrawButton(SpriteBatch sb, Texture2D texture, Circle button, bool isOpen, bool isHover)
        {
            if (isOpen == false && isHover == false)
                sb.Draw(this.button, button.Position, mainColor, this.button.Center(), 0f, 1f);

            if (isOpen == false && isHover == true)
                sb.Draw(this.buttonHover, button.Position, mainColor, this.buttonHover.Center(), 0f, 1f);
            if ((isOpen == true && isHover == false) || (isOpen == true && isHover == true))
                sb.Draw(this.coloredButton, button.Position, mainColor, this.coloredButton.Center(), 0f, 1f);

            sb.Draw(texture, button.Position, new Rectangle(0, 0, texture.Width, texture.Height), iconColor, 0f, texture.Center(), 1f, SpriteEffects.None, 1f);
        }

        private void DrawCompanionHUD(SpriteBatch sb, BaseEntity companion, float initialOffset)
        {
            sb.Draw(rect, new Rectangle((int)offset.X + 20, (int)((offset.Y + 13) + initialOffset), companionBar.Width + 28, 1), ColorHelper.UI_Gold);
            sb.Draw(rect, new Rectangle((int)offset.X + 20, (int)((offset.Y + 14) + initialOffset), companionBar.Width + 28, 1), ColorHelper.UI_DarkerGold);

            //Current
            sb.Draw(companionFiller, new Vector2(offset.X + 32, (offset.Y + 18) + initialOffset), new Rectangle(0, 0, companionFiller.Width * companion.Skills.health.CurrentHP / companion.Skills.health.MaxHP, companionFiller.Height), healthColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(companionFiller, new Vector2(offset.X + 32, (offset.Y + 33) + initialOffset), new Rectangle(0, 0, companionFiller.Width * (int)companion.Skills.endurance.CurrentStamina / companion.Skills.endurance.MaxStamina, companionFiller.Height), staminaColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(companionFiller, new Vector2(offset.X + 32, (offset.Y + 48) + initialOffset), new Rectangle(0, 0, companionFiller.Width * companion.Skills.wisdom.CurrentEnergy / companion.Skills.wisdom.MaxEnergy, companionFiller.Height), magicColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            //Meter BG
            sb.Draw(companionBar, new Vector2(offset.X + 32, (offset.Y + 18) + initialOffset), new Rectangle(0, 0, companionBar.Width, companionBar.Height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(companionBar, new Vector2(offset.X + 32, (offset.Y + 33) + initialOffset), new Rectangle(0, 0, companionBar.Width, companionBar.Height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            sb.Draw(companionBar, new Vector2(offset.X + 32, (offset.Y + 48) + initialOffset), new Rectangle(0, 0, companionBar.Width, companionBar.Height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            sb.DrawString(font, companion.Name, new Vector2(offset.X + 37, (offset.Y - 5) + initialOffset), Vector2.Zero, ColorHelper.UI_Gold, 1f); 
        }

        public bool IsClickingUI()
        {
            return (controls.MousePosition.X <= 230 && controls.MousePosition.Y <= 230);
        }
    }
}
