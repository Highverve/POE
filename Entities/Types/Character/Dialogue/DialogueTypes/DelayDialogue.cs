using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class DelayDialogue : ConversationAction
    {
        int delayTime, count;

        public DelayDialogue() { }
        public DelayDialogue(int ID, int GoToID, int DelayTime, bool IsReusable) : base(ID, GoToID, IsReusable)
        {
            delayTime = DelayTime;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            count += gt.ElapsedGameTime.Milliseconds;
            if (count >= delayTime)
            {
                conversations.SetCurrentLine(goToID);
            }
        }

        protected override void UpdateOnce(GameTime gt)
        {
            count = 0;
            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            count = 0;

            base.Terminate();
        }

        public new DelayDialogue Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            DelayDialogue obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new DelayDialogue(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), bool.Parse(words[4]));
                }
                catch
                {
                    Logger.AppendLine("Error adding Delay[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
