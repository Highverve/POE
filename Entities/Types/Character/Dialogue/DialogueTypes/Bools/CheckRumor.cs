using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes.Bools
{
    class CheckRumor : ConversationAction
    {
        int rumorID, falseGoToID, falseGoToGroup;

        public CheckRumor() { }
        public CheckRumor(int ID, int GoToID, int FalseGoToID, int RumorID, bool IsReusable) : base(ID, GoToID, IsReusable)
        {
            falseGoToID = FalseGoToID;
            rumorID = RumorID;
        }

        public override void Update(GameTime gt)
        {

            base.Update(gt);
        }

        protected override void UpdateOnce(GameTime gt)
        {
            bool containsRumor = screens.RUMOR_HasRumor(rumorID);

            if (containsRumor == true)
                conversations.SetCurrentLine(goToID);
            else
                conversations.SetCurrentLine(falseGoToID);

            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            base.Terminate();
        }

        public new CheckRumor Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            CheckRumor obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new CheckRumor(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]), bool.Parse(words[5]));
                }
                catch
                {
                    Logger.AppendLine("Error adding CheckRumor[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
