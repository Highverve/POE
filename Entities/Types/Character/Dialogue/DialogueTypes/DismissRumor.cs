using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class DismissRumor : ConversationAction
    {
        int rumorID;

        public DismissRumor() { }
        public DismissRumor(int ID, int GoToID, int RumorID, bool IsReusable) : base(ID, GoToID, IsReusable)
        {
            rumorID = RumorID;
        }

        public override void Update(GameTime gt)
        {

            base.Update(gt);
        }

        protected override void UpdateOnce(GameTime gt)
        {
            screens.RUMOR_Dismiss(rumorID);
            conversations.SetCurrentLine(goToID);

            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            base.Terminate();
        }

        public new DismissRumor Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            DismissRumor obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new DismissRumor(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), bool.Parse(words[4]));
                }
                catch
                {
                    Logger.AppendLine("Error adding AddRumor[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
