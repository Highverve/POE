using System;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class AddItem : ConversationAction
    {
        int itemID, quantity;

        public AddItem() {}
        public AddItem(int ID, int GoToID, bool IsReusable, int Item, int Quantity) : base(ID, GoToID, IsReusable)
        {
            itemID = Item;
            quantity = Quantity;
        }

        public override void Update(GameTime gt)
        {

            base.Update(gt);
        }

        protected override void UpdateOnce(GameTime gt)
        {
            controlledEntity.STORAGE_AddItem(itemID, quantity, false, true);

            conversations.SetCurrentLine(goToID);

            base.UpdateOnce(gt);
        }
        public override void Terminate()
        {
            isUpdatedOnce = false;
            base.Terminate();
        }

        public AddItem Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            AddItem obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new AddItem(int.Parse(words[1]), int.Parse(words[2]), bool.Parse(words[3]), int.Parse(words[4]), int.Parse(words[5]));
                }
                catch (Exception e)
                {
                    Logger.AppendLine("Error loading AddItem[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
