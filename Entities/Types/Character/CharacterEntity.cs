using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.Entities.Factions;
using Pilgrimage_Of_Embers.Entities.AI.Agents;
using Pilgrimage_Of_Embers.Entities.Steering_Behaviors;
using Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Interface;
using Pilgrimage_Of_Embers.Entities.NPC;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.TakeImageTypes;
using Pilgrimage_Of_Embers.Helper_Classes;
using System.IO;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.NPC
{
    public class CharacterEntity : NonPlayerEntity
    {
        private string mapLocation;
        public string MapLocation { get { return mapLocation; } set { mapLocation = value; } }

        private CharacterUI characterUI;

        public CharacterEntity(int ID, string Name, AnimationState Animation, Skillset Skills, ObjectAttributes Attributes, EntityLoot Loot, EntityStorage Storage, BaseFaction Faction, ObjectSenses Senses, EntityKin Kin,
                               float CircleRadius, float CenterOffset, float DepthOffset, float ShadowOffset, float InfoHeight, BaseAgent AgentAI, Merchant Merchant, string StartingMapName, Point StartingTile)
            : base(ID, Name, Animation, Skills, Attributes, Loot, Storage, Faction, Senses, Kin, CircleRadius, CenterOffset, DepthOffset, ShadowOffset, InfoHeight, AgentAI)
        {
            entityType = Entities.EntityType.Character;

            mapLocation = StartingMapName;
            SetTile(StartingTile);

            enemyTarget = null;
            allyTarget = null;

            steering = new SteeringBehavior(this);

            characterUI = new CharacterUI(Name);

            deathImage = new EmberImage(15, animation.AnimationSheet, animation.DeathTexture, 2f);
            deathImage.SetReferences(tileMap, camera, screens, null, null, null, null, mapEntities);

            jumpDust = new ParticleEngine.EmitterTypes.JumpCircle(Color.Lerp(Color.Transparent, ColorHelper.Charcoal, .75f), 150f, 1000);
            jumpDust.SetReferences(tileMap, camera, screens, null, null, null, null, mapEntities);

            merchant = Merchant;

            isSavable = true;
        }
        
        public void SetReferences(PlayerEntity player)
        {
            this.player = player;
            characterUI.SetReferences(screens, this, camera);
            merchant.SetReferences(screens, tileMap, this, camera);

            storage.AddPoints();
            merchant.AddPoints();
        }
        public override void SetControlledEntity(BaseEntity controlledEntity)
        {
            characterUI.SetControlledEntity(controlledEntity);

            base.SetControlledEntity(controlledEntity);
        }

        public override void UnmakeCompanion()
        {
            agentAI.RemoveAllGoals();
            base.UnmakeCompanion();
        }

        public override void Load(ContentManager cm)
        {
            characterUI.Load(cm);

            base.Load(cm);
        }

        public override void Update(GameTime gt)
        {
            controls.UpdateLast();
            controls.UpdateCurrent();

            base.Update(gt);

            characterUI.Update(gt, camera.WorldToScreen(position), !IsDead);

            if (IsDead == false)
            {
            }
            else
                characterUI.CloseAllUI();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
        public override void DrawUI(SpriteBatch sb)
        {
            characterUI.DrawShadow(sb, 1f);
            characterUI.Draw(sb);

            base.DrawUI(sb);
        }

        /// <summary>
        /// There will be no copying of NPCs! Always returns 'this' instance.
        /// </summary>
        /// <returns></returns>
        public BaseEntity Copy()
        {
            return this;
        }

        public void WriteCharacterData(string directory)
        {
            string fileName = directory + name + ".data";

            if (!File.Exists(fileName)) //If the file doesn't exist, create it!
                File.Create(fileName);

            try
            {
                File.WriteAllText(fileName, SaveData().ToString());
            }
            catch(Exception e)
            {
                Logger.AppendLine("Error saving character data[" + name + "]: " + e.Message);
            }
        }
        public void ReadCharacterData(string directory)
        {
            string fileName = directory + name + ".data";
            List<string> data = new List<string>();

            if (File.Exists(fileName))
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        data.Add(reader.ReadLine());
                    }
                }
            }

            LoadData(data);
        }
        public new StringBuilder SaveData()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("[Meta]");
            builder.AppendLine("Version \"" + GameInfo.Version() + "\"");
            builder.AppendLine("[/Meta]");
            builder.AppendLine();

            builder.AppendLine(characterUI.SaveData("[Dialogue]").ToString());

            builder.AppendLine("[Bartering]");
            builder.AppendLine(merchant.SaveMerchantData(culture).ToString());
            builder.AppendLine("[/Bartering]");

            builder.AppendLine();

            builder.AppendLine("[Memory]");
            builder.AppendLine(MEMORY_SaveData().ToString());
            builder.AppendLine("[/Memory]");

            return builder;
        }
        public void LoadData(List<string> data)
        {
            bool isReadingDialogue = false, isReadingBartering = false, isReadingMemory = false;
            List<string> dialogueData = new List<string>(), memoryData = new List<string>();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToUpper().StartsWith("[DIALOGUE]"))
                    isReadingDialogue = true;
                else if (data[i].ToUpper().StartsWith("[/DIALOGUE]"))
                    isReadingDialogue = false;

                if (isReadingDialogue == true)
                {
                    if (data[i].ToUpper().Contains("[DIALOGUE]"))
                        data[i] = "";

                    if (!string.IsNullOrEmpty(data[i]))
                        dialogueData.Add(data[i]);
                }


                if (data[i].ToUpper().StartsWith("[MEMORY]"))
                    isReadingMemory = true;
                else if (data[i].ToUpper().StartsWith("[/MEMORY]"))
                    isReadingMemory = false;

                if (isReadingMemory == true)
                {
                    if (data[i].ToUpper().Contains("[MEMORY]"))
                        data[i] = "";

                    if (!string.IsNullOrEmpty(data[i]))
                        memoryData.Add(data[i]);
                }



                if (data[i].ToUpper().StartsWith("[BARTERING]"))
                    isReadingBartering = true;
                else if (data[i].ToUpper().StartsWith("[/BARTERING]"))
                    isReadingBartering = false;
             }

            characterUI.LoadData(dialogueData);
            memory.LoadData(memoryData);
        }

        public bool IsClickingUI()
        {
            return characterUI.IsClickingUI();
        }
        public bool IsUIOpen()
        {
            return characterUI.IsUIOpen();
        }
        public void ForceCloseUIs()
        {
            characterUI.ForceCloseAll();
        }
    }
}
