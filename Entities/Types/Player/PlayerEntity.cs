using System.Collections.Generic;
using System.Text;
using Pilgrimage_Of_Embers.Entities.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.TakeImageTypes;
using Pilgrimage_Of_Embers.Entities.Factions;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.TileEngine.Objects.Soulgates;
using Pilgrimage_Of_Embers.Entities.AI.Agents.Type;

namespace Pilgrimage_Of_Embers.Entities.Types
{
    public sealed class PlayerEntity : BaseEntity
    {
        private int currentSoulgateID;
        public int CurrentSoulgateID { get { return currentSoulgateID; } set { currentSoulgateID = value; } }

        private int soulgateRestCount = 0;
        public int SoulgateRestCount { get { return soulgateRestCount; } }
        public void IncrementRestCount()
        {
            if (soulgateRestCount < int.MaxValue)
                soulgateRestCount++;
            else
                soulgateRestCount = 0; //This should never be reached, but... just in case.
        }

        private string savedMap;
        public string SavedMap { get { return savedMap; } }

        private List<NonPlayerEntity> currentCompanions = new List<NonPlayerEntity>();

        private string classType, pathway, birthplace;
        private int pathwayID;

        public string ClassType { get { return classType; } }
        public string Pathway { get { return pathway; } }
        public string Birthplace { get { return birthplace; } }

        public PlayerEntity(string Name, AnimationState Animation, Skillset Skills)
            : base(-1, Name, Animation, Skills, new ObjectAttributes(null, true), new EntityLoot(new List<ItemDrop>(), 0, 0), new EntityStorage(), FactionDatabase.GetFaction(1),
                   new ObjectSenses(800f, 500f, 90f, 750f, .01f, .01f, .01f), new EntityKin("Goat"), 16f, -8, 5, 30, 35, new VegetableAgent()) //TestChar ShadowOffset: 112    Center Offset: -40f    Depth: 5f    Info: 60
        {
            visualOverlay = new EntityVisual();

            equipment = new EntityEquipment();

            entityType = EntityType.Player;

            deathImage = new EmberImage(15, animation.AnimationSheet, animation.DeathTexture, 2f);
            deathImage.SetReferences(tileMap, camera, screens, null, null, null, null, mapEntities);

            jumpDust = new JumpCircle(Color.Lerp(Color.Transparent, new Color(40, 37, 31, 255), 1f), 200f, 500);
            jumpDust.SetReferences(tileMap, camera, screens, null, null, null, null, mapEntities);

            isPlayerControlled = true;
            isSavable = true;
        }

        public override void Initialize()
        {
            storage.SetReferences(screens, tileMap, this, camera);
            screens.INVENTORY_SetItemData(this, this.storage, this.equipment);
            screens.SOULS_SetData(this.equipment, this.storage);
            screens.Container_SetEntityData(this, this.storage);

            tileMap.SetControlledEntity(this);

            id = -7;
            maxBounce = 1;
            bounceHeight = .25f;
        }

        public override void Load(ContentManager cm)
        {
            base.Load(cm);
        }

        public override void Update(GameTime gt)
        {
            if (isTeleporting == true)
            {
                if (isBeginTeleporting == true)
                {
                    SUSPEND_Action(2000);
                    SUSPEND_Movement(2000);

                    map.FadeVariables();
                    screens.EFFECTS_BeginTransition(ScreenEngine.ScreenEffects.TransitionType.Fade, Color.White, 1000, 2f, 1f);
                }

                if (screens.EFFECTS_IsTransitionFaded == true)
                    SOULGATE_Teleport();
            }

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
        public override void DrawUI(SpriteBatch sb)
        {
            base.DrawUI(sb);
        }

        StringBuilder message, murderer;
        public StringBuilder Murderer { set { murderer = value; } } //Set by the last entity to strike, or map object.
        public StringBuilder Message { set { message = value; } }

        public new void AddEntities(List<BaseEntity> entities)
        {
            mapEntities = entities;
        }

        public override void FinalizeDeath()
        {
            /*
            if (isZoomApplied == false)
            {
                camera.SmoothZoom(1.15f, 1f, true, 0);
                isZoomApplied = true;
            }

            if (camera.Zoom >= 1.15f)
            {
                worldManager.BeginTransition(ScreenTransition.TransitionType.Fade, Color.Black, 1000, 1f, 1f);

                if (worldManager.transition.IsFaded)
                {
                    BaseSoulgate warpGate = SoulgateDatabase.Soulgate(currentSoulgateID);
                    if (warpGate == null)
                        warpGate = SoulgateDatabase.Soulgate(1);

                    warpGate.TeleportTo();
                    SOULGATE_Rest();

                    camera.SmoothZoom(1f, 1f, true, 0);
                    isZoomApplied = false;

                    deathImage.ForceRemoveAll();
                }
            }*/
            base.FinalizeDeath();
        }
        public override void SOULGATE_Rest()
        {
            IncrementRestCount();

            base.SOULGATE_Rest();
        }

        private bool isTeleporting = false, isBeginTeleporting = false;
        public void SOULGATE_Teleport()
        {
            BaseCheckpoint gate = MonumentDatabase.Soulgate(currentSoulgateID);

            if (gate != null)
            {
                gate.LoadMapTo();
                isTeleporting = false;
                isBeginTeleporting = false;
            }
        }
        public void SOULGATE_BeginTeleporting()
        {
            isTeleporting = true;
            isBeginTeleporting = true;
        }

        protected override void HandleMessage(MessageHolder message)
        {
            if (message.Message.ToUpper().Equals("FUS ROH DAH"))
            {
                CAPTION_Queue("DOVAH-KIIN, NOOOOO!");
            }
        }

        public override Vector2 STEERING_Direction()
        {
            return -MovementMotion;
        }
        public override Vector2 STEERING_Cross()
        {
            return MovementMotion.Cross();
        }

        public StringBuilder SaveStorageData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);
            builder.AppendLine(storage.SaveData().ToString());
            builder.AppendLine(equipment.SaveData().ToString());
            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadStorageData(List<string> linesOfData)
        {
            storage.LoadData(linesOfData);
            equipment.LoadData(linesOfData);

            storage.TotalItems().ForEach((i) => { i.RefreshAttributeText(); });
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder temp = new StringBuilder();

            temp.AppendLine(tag);

            temp.AppendLine("CurrentMap \"" + tileMap.MapPath + "\"");
            temp.AppendLine("Soulgate" + " " + currentSoulgateID.ToString() + " " + soulgateRestCount.ToString());
            temp.AppendLine("Class " + classType);
            temp.AppendLine("Pathway " + pathwayID + " \"" + pathway + "\"");
            temp.AppendLine("Birthplace \"" + birthplace + "\"");
            temp.AppendLine("Renown " + RENOWN_Value);
            temp.AppendLine("Position" + " " + position.X.ToString() + " " + position.Y.ToString());

            //Skills
            temp.AppendLine("XP" + " " + skillset.ExperiencePoints.ToString());
            temp.AppendLine(tileMap.EMBER_SavePileData(Name));

            temp.AppendLine("Health" + " " + skillset.health.Level.ToString() + " " + skillset.health.CurrentHP.ToString());
            temp.AppendLine("Endurance" + " " + skillset.endurance.Level.ToString() + " " + skillset.endurance.CurrentStamina);
            temp.AppendLine("Agility" + " " + skillset.agility.Level.ToString());
            temp.AppendLine("Defense" + " " + skillset.resistance.Level.ToString());
            temp.AppendLine("Strength" + " " + skillset.strength.Level.ToString());
            temp.AppendLine("Archery" + " " + skillset.archery.Level.ToString());
            //temp.AppendLine("Awareness");
            temp.AppendLine("Magic" + " " + skillset.wisdom.Level.ToString());
            temp.AppendLine("Intelligence" + " " + skillset.intelligence.Level.ToString());
            temp.AppendLine("Trapping" + " " + skillset.trapping.Level.ToString());
            temp.AppendLine("Awareness" + " " + skillset.awareness.Level.ToString());
            temp.AppendLine("Concealment" + " " + skillset.concealment.Level.ToString());
            temp.AppendLine("Looting" + " " + skillset.looting.Level.ToString());
            
            temp.AppendLine(tag.Replace("[", "[/"));

            return temp;
        }
        public void LoadData(List<string> linesOfData)
        {
            for (int i = 0; i < linesOfData.Count; i++)
            {
                string[] words = linesOfData[i].Split(' ');

                if (words[0].ToUpper().Equals("CURRENTMAP"))
                {
                    savedMap = linesOfData[i].FirstLastWithin("\"");
                }
                else if (words[0].ToUpper().Equals("XP"))
                {
                    skillset.ExperiencePoints = int.Parse(words[1]);
                }
                else if (words[0].ToUpper().Equals("EMBERPILE"))
                {
                    tileMap.EMBER_AddPile(int.Parse(words[1]), new Vector2().Parse(words[2], words[3]), this, int.Parse(words[4]), words[5]);
                }
                else if (words[0].ToUpper().Equals("SOULGATE"))
                {
                    currentSoulgateID = int.Parse(words[1]);
                    soulgateRestCount = int.Parse(words[2]);
                }
                else if (words[0].ToUpper().Equals("CLASS"))
                {
                    classType = words[1];
                }
                else if (words[0].ToUpper().Equals("PATHWAY"))
                {
                    pathwayID = int.Parse(words[1]);
                    pathway = linesOfData[i].FromWithin("\"", 1);
                }
                else if (words[0].ToUpper().Equals("BIRTHPLACE"))
                {
                    birthplace = linesOfData[i].FromWithin("\"", 1);
                }
                else if (words[0].ToUpper().Equals("RENOWN"))
                {
                    RENOWN_Value = int.Parse(words[1]);
                }
                else if (words[0].ToUpper().Equals("POSITION"))
                {
                    position.X = float.Parse(words[1]);
                    position.Y = float.Parse(words[2]);

                    camera.ForceLookAt(position);
                }
                else if (words[0].ToUpper().Equals("HEALTH"))
                {
                    skillset.health.SetPlayerLevel(int.Parse(words[1]));
                    skillset.health.CurrentHP = int.Parse(words[2]);
                }
                else if (words[0].ToUpper().Equals("ENDURANCE"))
                {
                    skillset.endurance.SetPlayerLevel(int.Parse(words[1]));
                    skillset.endurance.CurrentStamina = float.Parse(words[2]);
                }
                else if (words[0].ToUpper().Equals("AGILITY"))
                {
                    skillset.agility.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("DEFENSE"))
                {
                    skillset.resistance.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("STRENGTH"))
                {
                    skillset.strength.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("ARCHERY"))
                {
                    skillset.archery.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("MAGIC"))
                {
                    skillset.wisdom.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("INTELLIGENCE"))
                {
                    skillset.intelligence.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("TRAPPING"))
                {
                    skillset.trapping.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("AWARENESS"))
                {
                    skillset.awareness.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("CONCEALMENT"))
                {
                    skillset.concealment.SetPlayerLevel(int.Parse(words[1]));
                }
                else if (words[0].ToUpper().Equals("LOOTING"))
                {
                    skillset.looting.SetPlayerLevel(int.Parse(words[1]));
                }
            }
        }

        public void ResetPlayerData()
        {
            skillset = new Skillset();
            skillset.SetReferences(attributes);

            storage = new EntityStorage();
            storage.SetReferences(screens, tileMap, this, camera);

            equipment = new EntityEquipment();
            equipment.SetReferences(screens, storage, this);
        }

        public void SetPlayerData(string name, string classType, Skillset skills, List<BaseItem> items, string pathway, int pathwayID, string birthplace)
        {
            this.name = name;
            this.classType = classType;
            this.pathway = pathway;
            this.pathwayID = pathwayID;
            this.birthplace = birthplace;

            skillset = skills;

            for (int i = 0; i < items.Count; i++)
                storage.AddItem(items[i].ID, items[i].CurrentAmount, false, false);
        }
    }
}
