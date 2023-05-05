using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes
{
    public class DialogueButton
    {
        private string text;
        private int id;

        public int ID { get { return id; } }
        public string Text { get { return text; } }

        private Rectangle selectionBox;
        public Rectangle SelectionBox { get { return selectionBox; } set { selectionBox = value; } }

        public float lineHeight { get; set; }
        public bool IsHover { get; set; }

        public bool IsUsed { get; set; }
        public bool IsActive { get; set; }

        public ConversationAction action;

        public DialogueButton(int ID, string Text)
        {
            id = ID;
            text = Text;
        }
    }

    public class DialogueOptions : ConversationAction
    {
        private List<DialogueButton> buttons = new List<DialogueButton>();
        public List<DialogueButton> Buttons { get { return buttons; } }

        public static Debugging.DebugManager debug;

        public DialogueOptions() { }
        public DialogueOptions(int ID, bool IsReusable, List<DialogueButton> Buttons) : base(ID, -1, IsReusable)
        {
            buttons = Buttons;
        }

        public void SetTargets(List<ConversationAction> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                for (int j = 0; j < buttons.Count; j++)
                {
                    if (actions[i].ID == buttons[j].ID)
                    {
                        buttons[j].action = actions[i];
                        break;
                    }
                }
            }
        }

        public new DialogueOptions Parse(string parseName, string line, string characterName)
        {
            string[] words = line.Split(' ');
            DialogueOptions obj = null;

            if (line.ToUpper().StartsWith(parseName.ToUpper()))
            {
                try
                {
                    //[1 "Button1", 5 "Button2", ... ]
                    string buttons = line.FromWithin('[', ']', 1);
                    string[] buttonWords = buttons.Split("\", ");

                    List<DialogueButton> allButtons = new List<DialogueButton>();

                    for (int i = 0; i < buttonWords.Length; i++)
                    {
                        if (i < buttonWords.Length - 1)
                            buttonWords[i] += "\"";

                        string[] idTake = buttonWords[i].Split(' ');
                        allButtons.Add(new DialogueButton(int.Parse(idTake[0]), buttonWords[i].FromWithin("\"", 1))); 
                    }
                    
                    obj = new DialogueOptions(int.Parse(words[1]), bool.Parse(words[2]), allButtons);
                }
                catch
                {
                    Logger.AppendLine("Error adding DialogueOptions[" + line + "] in " + characterName + ".ds!");
                }
            }

            return obj;
        }
    }
}
