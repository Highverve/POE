using Microsoft.Xna.Framework;
using System;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class AddRumor : ConversationAction
    {
        int rumorID;

        public AddRumor() { }
        public AddRumor(int ID, int GoToID, bool IsReusable, int RumorID) : base(ID, GoToID, IsReusable)
        {
            rumorID = RumorID;
        }

        public override void Update(GameTime gt)
        {

            base.Update(gt);
        }

        protected override void UpdateOnce(GameTime gt)
        {
            screens.RUMOR_Add(rumorID);
            conversations.SetCurrentLine(goToID);

            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            base.Terminate();
        }

        public new AddRumor Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            AddRumor obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new AddRumor(int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]), int.Parse(words[4]));
                }
                catch (Exception e)
                {
                    Logger.AppendLine("Error adding AddRumor[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
