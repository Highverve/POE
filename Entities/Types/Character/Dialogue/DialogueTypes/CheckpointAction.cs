using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class CheckpointAction : ConversationAction
    {
        int checkpointID;

        public CheckpointAction() { }
        public CheckpointAction(int ID, int GoToID, int CheckpointID, bool IsReusable) : base(ID, GoToID, IsReusable)
        {
            checkpointID = CheckpointID;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        protected override void UpdateOnce(GameTime gt)
        {
            conversations.SetCurrentLine(goToID);
            conversations.SetCheckpoint(checkpointID);

            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;

            base.Terminate();
        }

        public new CheckpointAction Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            CheckpointAction obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new CheckpointAction(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), bool.Parse(words[4]));
                }
                catch
                {
                    Logger.AppendLine("Error adding Checkpoint[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
