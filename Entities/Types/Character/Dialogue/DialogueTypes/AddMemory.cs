using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class AddMemory : ConversationAction
    {
        string key, memory;

        public AddMemory() { }
        public AddMemory(int ID, int GoToID, bool IsReusable, string Key, string Memory) : base(ID, GoToID, IsReusable)
        {
            key = Key;
            memory = Memory;
        }

        public override void Update(GameTime gt)
        {

            base.Update(gt);
        }

        protected override void UpdateOnce(GameTime gt)
        {
            entity.MEMORY_AddLong(key, memory, -1, 1);
            conversations.SetCurrentLine(goToID);

            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            base.Terminate();
        }

        public new AddMemory Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            AddMemory obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new AddMemory(int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]), line.FromWithin('"', 1), line.FromWithin('"', 2));
                }
                catch
                {
                    Logger.AppendLine("Error adding AddMemory[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
