using Pilgrimage_Of_Embers.Entities.NPC;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue
{
    public class ConversationAction
    {
        protected int id;
        public int ID { get { return id; } }

        protected int goToID;
        public int GoToID { get { return goToID; } }

        private bool isReusable = true;
        public bool isUsed = false;

        public bool IsUsed { get { return isUsed; } }
        public bool IsReusable { get { return isReusable; } }

        protected ConversationManager conversations;
        protected BaseEntity entity, controlledEntity;
        protected ScreenManager screens;

        protected Controls controls = new Controls();

        public ConversationAction() { }
        public ConversationAction(int ID, int GoToID, bool IsReusable)
        {
            id = ID;
            goToID = GoToID;
            isReusable = IsReusable;
        }
        public void SetReferences(ConversationManager conversations, BaseEntity entity, BaseEntity controlledEntity, ScreenManager screens, Controls controls)
        {
            this.conversations = conversations;
            this.entity = entity;
            this.controlledEntity = controlledEntity;
            this.screens = screens;
            this.controls = controls;
        }

        protected bool isUpdatedOnce = false;
        protected virtual void UpdateOnce(GameTime gt) { }
        public virtual void Update(GameTime gt)
        {
            if (isUpdatedOnce == false)
            {
                UpdateOnce(gt);
                isUpdatedOnce = true;
            }
        }
        public virtual void Terminate()
        {
            isUsed = true;
        }

        public ConversationAction Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            ConversationAction obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new ConversationAction(int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]));
                }
                catch
                {
                    Logger.AppendLine("Error loading ConversationAction[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }

        public virtual string SaveData()
        {
            return "Action " + id + " " + isUsed;
        }
        public virtual void LoadData(string line)
        {
            string[] words = line.Split(' ');
            isUsed = bool.Parse(words[2]);
        }
    }
}
