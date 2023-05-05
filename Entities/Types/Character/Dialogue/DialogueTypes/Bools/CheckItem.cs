using Microsoft.Xna.Framework;
using System;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes.Bools
{
    class CheckItem : ConversationAction
    {
        int itemID, quantity, falseGoToID;

        public CheckItem() { }
        public CheckItem(int ID, int GoToID, int FalseGoToID, int Item, int Quantity, bool IsReusable) : base(ID, GoToID, IsReusable)
        {
            falseGoToID = FalseGoToID;

            itemID = Item;
            quantity = Quantity;
        }

        public override void Update(GameTime gt)
        {

            base.Update(gt);
        }

        protected override void UpdateOnce(GameTime gt)
        {
            bool containsItem = controlledEntity.STORAGE_Check(itemID, quantity);

            if (containsItem == true)
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

        public CheckItem Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            CheckItem obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    obj = new CheckItem(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]), int.Parse(words[5]), bool.Parse(words[6]));
                }
                catch (Exception e)
                {
                    Logger.AppendLine("Error adding CheckItem[" + line + "] in " + characterName + ".ds!");

                }
            }

            return obj;
        }
    }
}
