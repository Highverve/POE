using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class CharacterSay : ConversationAction
    {
        string text;
        public string Text { get { return text; } }

        public CharacterSay() { }
        public CharacterSay(int ID, int GoToID, bool IsReusable, string Text) : base(ID, GoToID, IsReusable)
        {
            text = Text;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (controls.IsKeyPressedOnce(controls.CurrentControls.Activate))
            {
                entity.CAPTION_Reset();
                conversations.SetCurrentLine(goToID);
            }
        }

        protected override void UpdateOnce(GameTime gt)
        {
            entity.CAPTION_SendImmediate(text, true);

            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            base.Terminate();
        }

        public CharacterSay Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            CharacterSay obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new CharacterSay(int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]), line.FromWithin("\"", 1));
                }
                catch
                {
                    Logger.AppendLine("Error adding Say[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
