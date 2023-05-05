using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class EndDialogue : ConversationAction
    {
        int goToLineID;
        private bool exitDialogue;

        public EndDialogue() { }
        public EndDialogue(int ID, int GoToLineID, bool ExitDialogue)
            : base(ID, GoToLineID, true)
        {
            goToLineID = GoToLineID;
            exitDialogue = ExitDialogue;
        }

        protected override void UpdateOnce(GameTime gt)
        {
            conversations.SetCurrentLine(goToLineID);
            conversations.IsActive = !exitDialogue;
            
            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            base.Terminate();
        }

        public new EndDialogue Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            EndDialogue obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new EndDialogue(int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]));
                }
                catch
                {
                    Logger.AppendLine("Error adding EndDialogue[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
