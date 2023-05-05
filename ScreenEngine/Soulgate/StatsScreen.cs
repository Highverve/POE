using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.ScreenEngine.Notification;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Performance;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    public class StatsScreen
    {
        Controls controls = new Controls();

        private Vector2 offset, bottomBox = new Vector2(30, 408), mouseDragOffset;
        private Vector2 Offset { get { return offset; } set { offset = new Vector2(MathHelper.Clamp(value.X, 0, GameSettings.VectorResolution.X - background.Height),
                                                                                   MathHelper.Clamp(value.Y, 0, GameSettings.VectorResolution.Y - background.Height)); } }

        private SpriteFont font, largeFont;
        private Texture2D background, fortifyButton, fortifyButtonHover, fortifyIcon, rowIcon;
        private Texture2D infoBG, infoIconBG, infoTextBG;

        private List<SkillButton> skillButtons = new List<SkillButton>();

        private List<InfoSection> infoSections = new List<InfoSection>();
        private List<InfoSection> statSections = new List<InfoSection>();

        private InfoSection playerName, playerClass, pathway, birthplace, faction, flameRenown, flameTitle;
        private InfoSection health, stamina, magic, physDef, projDef, magDef, stunResist, armorWeight, posPotion, negPotion;
        private InfoSection attackSpeed, accuracy, perception, stealth, creatureEfficacy, staminaRegen, magicDamage, spellSlots;
        private InfoSection luck, lockpicking, pickpocketing, physDamage, projDamage;

        private float infoScrollPosition;

        private Skillset skills;
        private BaseEntity controlledEntity;
        private ScreenManager screens;

        private Rectangle dragArea, uiRect;
        private bool isDragging = false;

        private bool isActive = false;
        public bool IsActive { get { return isActive; } set { isActive = value; } }


        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;


        public void InvertState()
        {
            isActive = !isActive;
        }

        public StatsScreen()
        {
        }
        public void SetReferences(Skillset skills, ScreenManager screens)
        {
            this.skills = skills;
            this.screens = screens;

            AddButtons();
        }
        public void SetSkillsData(Skillset skillset, BaseEntity controlledEntity)
        {
            skills = skillset;
            this.controlledEntity = controlledEntity;

            UpdateInfoValues();
            UpdateStatValues();

            GetButton("Health").SetReferences(skills.health);
            GetButton("Endurance").SetReferences(skills.endurance);
            GetButton("Agility").SetReferences(skills.agility);
            GetButton("Defense").SetReferences(skills.resistance);
            GetButton("Strength").SetReferences(skills.strength);
            GetButton("Archery").SetReferences(skills.archery);
            GetButton("Magic").SetReferences(skills.wisdom);
            GetButton("Intelligence").SetReferences(skills.intelligence);
            GetButton("Trapping").SetReferences(skills.trapping);
            GetButton("Awareness").SetReferences(skills.awareness);
            GetButton("Concealment").SetReferences(skills.concealment);
            GetButton("Looting").SetReferences(skills.looting);
        }
        private SkillButton GetButton(string name)
        {
            SkillButton button = null;

            for (int i = 0; i < skillButtons.Count; i++)
            {
                if (skillButtons[i].Name.ToUpper().Equals(name.ToUpper()))
                    button = skillButtons[i];
            }

            return button;
        }

        private string directory = "Interface/Soulgate/Skills/";
        private Texture2D agilityIcon;
        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/boldOutlined");
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            background = cm.Load<Texture2D>(directory + "bg");
            fortifyButton = cm.Load<Texture2D>(directory + "fortifyButton");
            fortifyButtonHover = cm.Load<Texture2D>(directory + "fortifyButtonHover");
            fortifyIcon = cm.Load<Texture2D>(directory + "fortifyIcon");
            rowIcon = cm.Load<Texture2D>(directory + "rowIcon");

            infoBG = cm.Load<Texture2D>(directory + "infoBG");
            infoTextBG = cm.Load<Texture2D>(directory + "infoTextBG");
            infoIconBG = cm.Load<Texture2D>(directory + "infoIconBG");

            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            mouseDragOffset = new Vector2(background.Width / 2, 12);

            agilityIcon = cm.Load<Texture2D>(directory + "Icons/agilitySmall");

            Offset = new Vector2(GameSettings.VectorCenter.X - ((background.Width + infoBG.Width) / 2), GameSettings.VectorCenter.Y - (background.Height / 2));

            AddButtons();
            AddInfoSections();
            AddStatSections(cm);
        }

        private void AddButtons()
        {
            skillButtons.Clear();

            skillButtons.Add(new SkillButton(0, "Health", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.health) == true && s.health.Level < 100)
                {
                    s.RemoveEXP(s.health);
                    s.health.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Health fortified to " + s.health.Level.ToString());
                }
            }, skills.health));

            skillButtons.Add(new SkillButton(2, "Endurance", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.endurance) == true && s.endurance.Level < 100)
                {
                    s.RemoveEXP(s.endurance);
                    s.endurance.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Endurance fortified to " + s.endurance.Level.ToString());
                }
            }, skills.endurance));

            skillButtons.Add(new SkillButton(4, "Agility", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.agility) == true && s.agility.Level < 100)
                {
                    s.RemoveEXP(s.agility);
                    s.agility.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Agility fortified to " + s.agility.Level.ToString());
                }
            }, skills.agility));

            skillButtons.Add(new SkillButton(6, "Defense", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.resistance) == true && s.resistance.Level < 100)
                {
                    s.RemoveEXP(s.resistance);
                    s.resistance.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Defense fortified to " + s.resistance.Level.ToString());
                }
            }, skills.resistance));

            skillButtons.Add(new SkillButton(8, "Strength", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.strength) == true && s.strength.Level < 100)
                {
                    s.RemoveEXP(s.strength);
                    s.strength.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Strength fortified to " + s.strength.Level.ToString());
                }
            }, skills.strength));

            skillButtons.Add(new SkillButton(10, "Archery", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.archery) == true && s.archery.Level < 100)
                {
                    s.RemoveEXP(s.archery);
                    s.archery.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Archery fortified to " + s.archery.Level.ToString());
                }
            }, skills.archery));

            skillButtons.Add(new SkillButton(12, "Magic", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.wisdom) == true && s.wisdom.Level < 100)
                {
                    s.RemoveEXP(s.wisdom);
                    s.wisdom.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Magic fortified to " + s.wisdom.Level.ToString());
                }
            }, skills.wisdom));

            skillButtons.Add(new SkillButton(14, "Intelligence", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.intelligence) == true && s.intelligence.Level < 100)
                {
                    s.RemoveEXP(s.intelligence);
                    s.intelligence.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Intelligence fortified to " + s.intelligence.Level.ToString());
                }
            }, skills.intelligence));

            skillButtons.Add(new SkillButton(16, "Trapping", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.trapping) == true && s.trapping.Level < 100)
                {
                    s.RemoveEXP(s.trapping);
                    s.trapping.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Trapping fortified to " + s.trapping.Level.ToString());
                }
            }, skills.trapping));

            skillButtons.Add(new SkillButton(18, "Awareness", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.awareness) == true && s.awareness.Level < 100)
                {
                    s.RemoveEXP(s.awareness);
                    s.awareness.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Awareness fortified to " + s.awareness.Level.ToString());
                }
            }, skills.awareness));

            skillButtons.Add(new SkillButton(20, "Concealment", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.concealment) == true && s.concealment.Level < 100)
                {
                    s.RemoveEXP(s.concealment);
                    s.concealment.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Concealment fortified to " + s.concealment.Level.ToString());
                }
            }, skills.concealment));

            skillButtons.Add(new SkillButton(22, "Looting", agilityIcon, (Skillset s) =>
            {
                if (s.CompareGreaterThan(s.looting) == true && s.looting.Level < 100)
                {
                    s.RemoveEXP(s.looting);
                    s.looting.FortifyLevel();

                    screens.NOTIFICATION_Add(NotificationManager.IconType.Skills, "Looting fortified to " + s.looting.Level.ToString());
                }
            }, skills.looting));

            skillButtons.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
        private void AddInfoSections()
        {
            playerName = new InfoSection("Name", "", "");
            playerClass = new InfoSection("Class", "", "");
            pathway = new InfoSection("Pathway", "", "");
            birthplace = new InfoSection("Birthplace", "", "");
            faction = new InfoSection("Faction", "", "");
            flameRenown = new InfoSection("Flame Renown", "", "");
            flameTitle = new InfoSection("Flame Title", "", "");

            infoSections.Add(playerName);
            infoSections.Add(playerClass);
            infoSections.Add(pathway);
            infoSections.Add(birthplace);
            infoSections.Add(faction);
            infoSections.Add(flameRenown);
            infoSections.Add(flameTitle);
        }
        private void AddStatSections(ContentManager cm)
        {
            health = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/hp"), "Health Points", "0 / 10", "The health of the controlled character.\n\n(governing skill: Health)");
            stamina = new InfoSection("Endurance Points", "0 / 100", "The stamina of the controlled character.\n\n(governing skill: Endurance)");
            magic = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/magic"), "Magic Points", "0 / 10", "The magical energy of the controlled character.\n\n(governing skill: Magic, Intelligence)");

            physDamage = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/physDamage"), "Physical Damage", "0", "Damage dealt with close-combat weapons.\n\n(governing skill: Strength)");
            projDamage = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/projDamage"), "Projectile Damage", "0", "Damage dealt with long-range physical projectiles.\n\n(governing skill: Strength)");
            magicDamage = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/magicDamage"), "Magical Damage", "0", "Damage dealt with spellcasting.\n\n(governing skill: Intelligence)");

            physDef = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/physResist"), "Physical Protection", "0", "Reduction of physical damage.\n\n(governing skill: Defense)");
            projDef = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/projResist"), "Projectile Protection", "0", "Reduction of projectile damage.\n\n(governing skill: Defense)");
            magDef = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/magResist"), "Magical Protection", "0", "Reduction of damage from spells.\n\n(governing skill: Defense)");
            stunResist = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/stunResist"), "Stun Protection", "0", "Reduction of stun build-up from attacks.\n\n(governing skill: Defense)");
            armorWeight = new InfoSection(null, "Armor Weight", "0", "Reduces movement speed, while improving stun protection. (governing skills: None) ");
            posPotion = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/posPotionEffect"), "Positive Brew Potency", "0", "The effectiveness of a positive potion or flask.\n\n(governing skill: Magic)");
            negPotion = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/negPotionEffect"), "Negative Brew Potency", "0", "The effectiveness of a negative potion or flask.\n\n(governing skill: Magic)");

            attackSpeed = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/attackSpeed"), "Attack Speed", "1", "Speed of attacks.\n\n(governing skill: Agility)");
            accuracy = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/accuracy"), "Accuracy", "0", "Accuracy with archery weapons.\n\n(governing skill: Archery)");
            staminaRegen = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/enduranceRegen"), "Endurance Regeneration", "0", "The regeneration speed of endurance.\n\n(governing skill: Endurance)");
            spellSlots = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/spellSlots"), "Spell Slots", "0", "Spell slots available for equipping.\n\n(governing skill: Magic)");

            perception = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/perception"), "Perception", "0", "Assists in discovering traps and other hidden objects.\n\n(governing skill: Awareness)");
            stealth = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/stealth"), "Stealth", "0", "Determines your sneaking ability (governing skill: Concealment)");
            creatureEfficacy = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/creatureEfficacy"), "Creature Efficacy", "0", "Special effect multiplier for creature disguises.\n\n(governing skill: Concealment)");
            luck = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/luck"), "Luck", "0", "Affects drop chance and resource collecting.\n\n(governing skill: Looting)");
            lockpicking = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/lockpicking"), "Lockpicking", "0", "(governing skill: Looting)");
            pickpocketing = new InfoSection(cm.Load<Texture2D>("Interface/Soulgate/Skills/Icons/pickpocketing"), "Pickpocketing", "0", "Improves pickpocketing chance.\n\n(governing skill: Looting)");

            statSections.Add(health);
            statSections.Add(stamina);
            statSections.Add(magic);

            statSections.Add(physDamage);
            statSections.Add(projDamage);
            statSections.Add(magicDamage);

            statSections.Add(physDef);
            statSections.Add(projDef);
            statSections.Add(magDef);
            statSections.Add(stunResist);
            statSections.Add(armorWeight);
            statSections.Add(posPotion);
            statSections.Add(negPotion);

            statSections.Add(attackSpeed);
            statSections.Add(accuracy);
            statSections.Add(staminaRegen);
            statSections.Add(spellSlots);

            statSections.Add(perception);
            statSections.Add(stealth);
            statSections.Add(creatureEfficacy);
            statSections.Add(luck);
            statSections.Add(lockpicking);
            statSections.Add(pickpocketing);
        }

        private string hints = "Fortify Skills Tips:\n\n" +
            "There are twelve skills on the level side. You may only fortify skills when\n" +
            "you are at a soulgate. The level and required embers are listed to the right of\n" + 
            "each skill's name. On the left side, the stats governed by skills are\n" +
            "displayed. When a skill is fortified, the respective stats improve.\n\n" +
            "Scroll through the stats when hovering your mouse inside of the pane.";

        public void Update(GameTime gt)
        {
            controls.UpdateLast();
            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenStats))
            {
                InvertState();
                Logger.AppendLine("Opened skills UI");
            }

            if (isActive == true)
            {
                uiRect = new Rectangle((int)offset.X, (int)offset.Y, background.Width, background.Height);

                dragArea = new Rectangle((int)offset.X + 117, (int)offset.Y, 140, 20);
                Drag();

                UpdateInfo(gt);

                for (int i = 0; i < skillButtons.Count; i++)
                {
                    skillButtons[i].ButtonRect = new Rectangle((int)offset.X + 324, ((int)offset.Y + 48) + (i * (fortifyButton.Height + 2)), fortifyButton.Width, fortifyButton.Height);

                    if (skillButtons[i].ButtonRect.Contains(controls.MousePosition))
                    {
                        skillButtons[i].IsHover = true;

                        if (screens.SOULGATE_IsNear() == true)
                        {
                            ToolTip.RequestStringAssign("Fortify Skill");

                            if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                                skillButtons[i].OnFortifyClick(skills);
                        }
                        else
                        {
                            ToolTip.RequestStringAssign("Must be near a soulgate.");
                        }
                    }
                    else
                        skillButtons[i].IsHover = false;

                    if (skillButtons[i].RowRect.Contains(controls.MousePosition))
                        skillButtons[i].IsRowHover = true;
                    else
                        skillButtons[i].IsRowHover = false;
                }

                hintRect = new Rectangle((int)offset.X + 266, (int)offset.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)offset.X + 293, (int)offset.Y, windowButton.Width - 20, windowButton.Height);

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
                    ToolTip.RequestStringAssign("Hide Skills");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Button Click 1");
                        isActive = false;
                    }
                }
                else
                    isHideHover = false;
            }

        }

        private CallLimiter limitInfoUpdate = new CallLimiter(1000), limitStatUpdate = new CallLimiter(100);
        private void UpdateInfo(GameTime gt)
        {
            infoRect = new Rectangle((int)offset.X + 343, (int)offset.Y + 46, infoBG.Width, infoBG.Height - 55);

            if (limitInfoUpdate.IsCalling(gt))
                UpdateInfoValues();

            if (limitStatUpdate.IsCalling(gt))
                UpdateStatValues();

            int infoOffsetY = (infoSections.Count * (infoTextBG.Height + 1)) + 5;
            for (int i = 0; i < infoSections.Count; i++)
            {
                infoSections[i].Rect = new Rectangle((int)offset.X + 359, (int)offset.Y + (int)infoScrollPosition + 48 + (i * (infoTextBG.Height + 1)), infoTextBG.Width, infoTextBG.Height);

                if (infoSections[i].Rect.Contains(controls.MousePosition))
                    ToolTip.RequestStringAssign(infoSections[i].Description);
            }

            for (int i = 0; i < statSections.Count; i++)
            {
                statSections[i].Rect = new Rectangle((int)offset.X + 359, (int)offset.Y + (int)(infoScrollPosition + infoOffsetY + 54) + (i * (infoTextBG.Height + 1)), infoTextBG.Width, infoTextBG.Height);

                if (statSections[i].Rect.Contains(controls.MousePosition))
                    ToolTip.RequestStringAssign(statSections[i].Description);
            }

            SmoothScroll(gt, 50f, 250f, 300f, 10f, ref infoScrollPosition, ref scrollValue, ref scrollVelocity, -(184), infoRect);
        }
        private void UpdateInfoValues()
        {
            playerName.Value = controlledEntity.Name;

            if (controlledEntity is PlayerEntity)
                playerClass.Value = ((PlayerEntity)controlledEntity).ClassType;
            else
                playerClass.Value = "Unknown";

            if (controlledEntity is PlayerEntity)
                pathway.Value = ((PlayerEntity)controlledEntity).Pathway;
            else
                pathway.Value = "Unknown";

            if (controlledEntity is PlayerEntity)
                birthplace.Value = ((PlayerEntity)controlledEntity).Birthplace;
            else
                birthplace.Value = "Unknown";

            if (controlledEntity.Faction != null)
                faction.Value = controlledEntity.Faction.Name;
            else
                faction.Value = "Factionless";

            flameRenown.Value = ((int)controlledEntity.RENOWN_Value).ToString();
            flameTitle.Value = controlledEntity.RENOWN_Title();
        }
        private void UpdateStatValues()
        {
            health.Value = controlledEntity.Skills.health.CurrentHP + "/" + controlledEntity.Skills.health.MaxHP;
            stamina.Value = (int)controlledEntity.Skills.endurance.CurrentStamina + "/" + controlledEntity.Skills.endurance.MaxStamina;
            magic.Value = controlledEntity.Skills.wisdom.CurrentEnergy + "/" + controlledEntity.Skills.wisdom.MaxEnergy;

            physDamage.Value = controlledEntity.Skills.strength.MeleeDamage.ToString();
            projDamage.Value = controlledEntity.Skills.strength.ArcheryDamage.ToString();
            magicDamage.Value = controlledEntity.Skills.intelligence.MagicDamage.ToString();

            //resistance stats go here
            physDef.Value = ((int)((1f - controlledEntity.ATTRIBUTE_GetMultiplier(BaseEntity.ATTRIBUTE_SkillsPhysicalDefense, 0)) * 100)) + "% (+" + ((controlledEntity.EQUIPMENT_PhysicalDefense() - 1) * 100) + ")";
            projDef.Value = ((int)((1f - controlledEntity.ATTRIBUTE_GetMultiplier(BaseEntity.ATTRIBUTE_SkillsProjectileDefense, 0)) * 100)) + "% (+" + ((controlledEntity.EQUIPMENT_ProjectileDefense() - 1) * 100) + ")";
            magDef.Value = ((int)((1f - controlledEntity.ATTRIBUTE_GetMultiplier(BaseEntity.ATTRIBUTE_SkillsMagicalDefense, 0)) * 100)) + "% (+" + ((controlledEntity.EQUIPMENT_MagicalDefense() - 1) * 100) + ")";
            stunResist.Value = string.Format("{0:F1}", controlledEntity.DEFENSE_MaximumStun());
            armorWeight.Value = string.Format("{0:F1}", controlledEntity.EQUIPMENT_ArmorWeight());

            posPotion.Value = ((int)(controlledEntity.Skills.wisdom.PositivePotionMultiplier + controlledEntity.EQUIPMENT_EffectAmplifier() * 100)).ToString() + "%";
            negPotion.Value = ((int)(controlledEntity.Skills.wisdom.NegativePotionMultiplier + controlledEntity.EQUIPMENT_EffectResistance() * 100)).ToString() + "%";

            attackSpeed.Value = ((int)(controlledEntity.Skills.agility.AttackSpeed * 100)).ToString() + "%";
            accuracy.Value = ((int)(controlledEntity.Skills.archery.Accuracy * 100)).ToString() + "%";
            staminaRegen.Value = controlledEntity.Skills.endurance.RegenerationMultiplier.ToString();
            spellSlots.Value = controlledEntity.Skills.wisdom.SpellSlots().ToString();

            perception.Value = ((int)(controlledEntity.Skills.awareness.Perception * 100)).ToString() + "%";
            stealth.Value = ((int)(controlledEntity.Skills.concealment.Stealth * 100)).ToString() + "%";
            creatureEfficacy.Value = ((int)(controlledEntity.Skills.concealment.CreatureMultiplier * 100)).ToString() + "%";
            luck.Value = ((int)(controlledEntity.Skills.looting.Luck * 100)).ToString() + "%";
            pickpocketing.Value = ((int)(controlledEntity.Skills.looting.Pickpocketing * 100)).ToString() + "%";

            /*
                statSections.Add(physResist);
                statSections.Add(projResist);
                statSections.Add(magResist);
                statSections.Add(stunResist);
                statSections.Add(posPotion);
                statSections.Add(negPotion);

                statSections.Add(attackSpeed);
                statSections.Add(accuracy);
                statSections.Add(staminaRegen);
                statSections.Add(spellSlots);

                statSections.Add(perception);
                statSections.Add(stealth);
                statSections.Add(creatureEfficacy);
                statSections.Add(luck);
                statSections.Add(lockpicking);
                statSections.Add(pickpocketing);
            */
        }

        float scrollValue = 0f; float scrollVelocity = 0f;
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
        private void Drag()
        {
            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                screens.SetCursorState(Cursor.CursorState.Move);
                Offset = controls.MouseVector - mouseDragOffset;
            }
        }

        private Color goldColor = new Color(182, 191, 137, 255);
        private Color fade = Color.White;
        private Color button = Color.Lerp(Color.Transparent, Color.White, .75f);
        StringBuilder exp = new StringBuilder();

        public void Draw(SpriteBatch sb)
        {
            if (isActive == true)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(background, offset, Color.White, Vector2.Zero, 0f, 1f, .5f);
                sb.Draw(infoBG, offset + new Vector2(343, 0), Color.White);

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

                sb.DrawString(font, "Fortify Skills", new Vector2(offset.X + background.Width / 2, offset.Y + 12), "Fortify Skills".LineCenter(font), goldColor, 1f);

                exp.Clear();
                exp.AppendLine("Total Level: " + skills.CombinedLevels + "   Embers: " + string.Format("{0:n0}", skills.ExperiencePoints));
                sb.DrawString(font, exp.ToString(), new Vector2(offset.X + background.Width / 2, offset.Y + 39), exp.ToString().LineCenter(font), fade, 1f);

                for (int i = 0; i < skillButtons.Count; i++)
                {
                    skillButtons[i].RowRect = new Rectangle((int)offset.X + 17, ((int)offset.Y + 48) + (i * (rowIcon.Height + 2)), 335, 27);

                    sb.Draw(rowIcon, new Vector2(skillButtons[i].RowRect.X, skillButtons[i].RowRect.Y), Color.White);

                    sb.Draw(skillButtons[i].Icon, new Vector2(skillButtons[i].RowRect.X - 2, skillButtons[i].RowRect.Y - 3), Color.White);
                    sb.DrawString(font, skillButtons[i].Name, new Vector2(skillButtons[i].RowRect.X + 36, skillButtons[i].RowRect.Y + 6), Vector2.Zero, button, 1f);

                    sb.DrawString(font, skillButtons[i].Skill.Level.ToString(), new Vector2(offset.X + 227, (offset.Y + 62) + (i * (fortifyButton.Height + 2))), skillButtons[i].Skill.Level.ToString().LineCenter(font), button, 1f);
                    sb.DrawString(font, string.Format("{0:n0}", skillButtons[i].Skill.Experience), new Vector2(offset.X + 282, (offset.Y + 62) + (i * (fortifyButton.Height + 2))), skillButtons[i].Skill.Experience.ToString().LineCenter(font), button, 1f);

                    if (skillButtons[i].IsHover == false)
                        sb.Draw(fortifyButton, skillButtons[i].ButtonRect, Color.White);
                    else
                        sb.Draw(fortifyButtonHover, skillButtons[i].ButtonRect, Color.White);

                    if (skillButtons[i].IsRowHover == true)
                        sb.DrawString(font, skillButtons[i].Skill.Description.WrapText(font, 320), offset + bottomBox, Vector2.Zero, button, 1f);

                    sb.Draw(fortifyIcon, skillButtons[i].ButtonRect, button);
                }

                sb.End();

                DrawInfo(sb);
            }
        }

        private Rectangle infoRect;
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

        public void DrawInfo(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                DrawInside(sb, infoRect, () =>
                {
                    for (int i = 0; i < infoSections.Count; i++)
                    {
                        sb.Draw(infoTextBG, infoSections[i].Rect, Color.White);

                        sb.DrawString(font, infoSections[i].Name, new Vector2(infoSections[i].Rect.X + 6, infoSections[i].Rect.Y + 4), ColorHelper.UI_Gold);

                        if (!string.IsNullOrEmpty(infoSections[i].Value))
                            sb.DrawString(font, infoSections[i].Value, new Vector2(infoSections[i].Rect.X + infoTextBG.Width - 4, infoSections[i].Rect.Y + 4), new Vector2(font.MeasureString(infoSections[i].Value).X, 0), Color.White, 1f);
                    }

                    for (int i = 0; i < statSections.Count; i++)
                    {
                        sb.Draw(infoIconBG, statSections[i].Rect, Color.White);

                        if (statSections[i].Icon != null)
                            sb.Draw(statSections[i].Icon, new Vector2(statSections[i].Rect.X + 2, statSections[i].Rect.Y + 2), Color.White);

                        sb.DrawString(font, statSections[i].Name, new Vector2(statSections[i].Rect.X + 22, statSections[i].Rect.Y + 4), ColorHelper.UI_Gold);

                        if (!string.IsNullOrEmpty(statSections[i].Value))
                            sb.DrawString(font, statSections[i].Value, new Vector2(statSections[i].Rect.X + infoIconBG.Width - 4, statSections[i].Rect.Y + 4), new Vector2(font.MeasureString(statSections[i].Value).X, 0), Color.White, 1f);
                    }
                });
            }
        }

        public bool IsClickingUI()
        {
            if (isActive == true)
                return isDragging == true || uiRect.Contains(controls.MousePosition) || infoRect.Contains(controls.MousePosition);
            else
                return false;
        }

        public void ResetPosition()
        {
            offset = new Vector2(GameSettings.VectorCenter.X - (background.Width / 2), GameSettings.VectorCenter.Y - background.Height / 2);
        }
    }

    class InfoSection
    {
        public Texture2D Icon { get; private set; }

        public string Name { get; private set; }
        public string Value { get; set; }
        public string Description { get; private set; }

        public Rectangle Rect { get; set; }

        public InfoSection(Texture2D Icon, string Name, string StartValue, string Description)
        {
            this.Icon = Icon;
            this.Name = Name;
            Value = StartValue;
            this.Description = Description;
        }
        public InfoSection(string Name, string StartValue, string Description)
        {
            this.Name = Name;
            Value = StartValue;
            this.Description = Description;
        }
    }
}
