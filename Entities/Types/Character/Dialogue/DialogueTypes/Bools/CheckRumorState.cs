using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes.Bools
{
    class CheckRumorCompleted : ConversationAction
    {
        int rumorID, falseGoToID, falseGoToGroup;

        public CheckRumorCompleted() { }
        public CheckRumorCompleted(int ID, int GoToID, int FalseGoToID, int RumorID, bool IsReusable) : base(ID, GoToID, IsReusable)
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
            bool isRumorCompleted = (screens.RUMOR_State(rumorID) == ScreenEngine.RumorsNotes.Rumor.RumorState.Resolved); //If the state is resolved, set to true

            if (isRumorCompleted == true)
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

        public new CheckRumorCompleted Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            CheckRumorCompleted obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new CheckRumorCompleted(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]), bool.Parse(words[5]));
                }
                catch
                {
                    Logger.AppendLine("Error adding CheckRumorState[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
