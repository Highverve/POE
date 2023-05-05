using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue;
using Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Entities.NPC
{
    public class DialogueManager
    {
        /* To-Do:
         * 
         * Add player interactive lines:
         *    
         *    - AddRumor, AddItem, AddSoul, AddMoney, AddSpell, AddExp
         *    - ResolveRumor, DismissRumor, RemoveItem, RemoveExp
         *    - 
         * 
         * Add character interactive lines:
         *    
         *    - SwitchMap, MakeCompanion, 
         * 
         */

        private List<ConversationAction> actions = new List<ConversationAction>();
        public List<ConversationAction> Actions { get { return actions; } }

        private int currentLineID = 1, checkpointID = 1;
        public int CurrentLineID { get { return currentLineID; } }
        public int Checkpoint { get { return checkpointID; } }

        public bool IsOptionSelecting { get; set; }

        private ScreenEngine.ScreenManager screens;
        private BaseEntity entity, controlledEntity;
        private ConversationManager conversationManager;

        private const string fileExtension = ".ds";
        private string characterName;

        private Random random;

        private Controls controls = new Controls();

        public DialogueManager(string CharacterName)
        {
            characterName = CharacterName;
            ReadConversationFile("MainContent/Dialogue/" + characterName);
        }

        public void SetReferences(ScreenEngine.ScreenManager screens, BaseEntity entity, ConversationManager conversationManager)
        {
            this.screens = screens;
            this.entity = entity;
            this.conversationManager = conversationManager;

            random = new Random(Guid.NewGuid().GetHashCode());
        }
        public void SetControlledEntity(BaseEntity controlledEntity)
        {
            this.controlledEntity = controlledEntity;

            for (int i = 0; i < actions.Count; i++)
                actions[i].SetReferences(conversationManager, entity, controlledEntity, screens, controls);
        }

        public void ReadConversationFile(string fileName)
        {
            bool readingDialogue = false;

            fileName = fileName + fileExtension;

            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (string.IsNullOrEmpty(line.Trim()))
                            continue;

                        if (line.ToUpper().Contains("[DIALOGUE]"))
                            readingDialogue = true;
                        else if (line.ToUpper().Contains("[/DIALOGUE]"))
                            readingDialogue = false;

                        if (readingDialogue)
                        {
                            line = line.InjectRandoms(random);
                            line = line.ApplyIdentifierSyntax();

                            AddConversation(line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.AppendLine("Error loading dialogue: " + e.Message);
            }

            AddReferences();
        }
        private void AddConversation(string line)
        {
            AddToConversationList(new DialogueOptions().Parse("Options", line, characterName));

            AddToConversationList(new CharacterSay().Parse("Say", line, characterName));
            AddToConversationList(new CheckpointAction().Parse("Checkpoint", line, characterName));

            AddToConversationList(new EndDialogue().Parse("End", line, characterName));
            AddToConversationList(new AddItem().Parse("AddItem", line, characterName));
            AddToConversationList(new DelayDialogue().Parse("Delay", line, characterName));

            AddToConversationList(new AddRumor().Parse("AddRumor", line, characterName));
            AddToConversationList(new ResolveRumor().Parse("ResolveRumor", line, characterName));
            AddToConversationList(new DismissRumor().Parse("DismissRumor", line, characterName));

            AddToConversationList(new AddMemory().Parse("AddMemory", line, characterName));
        }
        private void AddToConversationList(ConversationAction conversation)
        {
            if (conversation != null)
            {
                actions.Add(conversation);
                actions.LastOrDefault().SetReferences(conversationManager, entity, controlledEntity, screens, controls);
            }
        }
        private void AddReferences()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is DialogueOptions)
                {
                    ((DialogueOptions)actions[i]).SetTargets(actions);
                }
            }
        }

        private int failIteration = 0;
        public void UpdateDialogue(GameTime gt)
        {
            if (currentAction != null)
            {
                controls.UpdateCurrent();
                currentAction.Update(gt);
                controls.UpdateLast();

                if (currentLineID != currentAction.ID)
                {
                    currentAction.Terminate();
                    currentAction = null;
                }
            }

            if (currentAction == null)
            {
                currentAction = CurrentLine(currentLineID);

                if (currentAction == null)
                {
                    failIteration++;

                    if (failIteration >= 3) //If still null... something bad has happened.
                    {
                        Logger.AppendLine("Error finding line " + currentLineID + " of " + characterName + ".ds. Fix immediately!");
                        conversationManager.IsActive = false;
                    }
                }
                else
                    failIteration = 0;
            }
        }

        public void SetCurrentLineID(int goToLineID)
        {
            if (goToLineID != -1)
                currentLineID = goToLineID;
        }
        public void SetCheckpointID(int id)
        {
            checkpointID = id;
        }
        public ConversationAction CurrentLine(int line)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].ID == line)
                {
                    //Logger.AppendLine(line + " is found.");
                    return actions[i];
                }
            }

            return null;
        }

        private ConversationAction currentAction;
        public ConversationAction CurrentAction { get { return currentAction; } }

        public void ResetCurrentID() { currentLineID = checkpointID; }
    }
}
