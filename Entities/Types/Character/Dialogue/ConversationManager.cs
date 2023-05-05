using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Dialogue.DialogueTypes;

namespace Pilgrimage_Of_Embers.Entities.NPC
{
    public class ConversationManager
    {
        DialogueManager dialogueManager;

        string characterName;

        private Texture2D tab, end, button, buttonHover;
        private SpriteFont font;

        private ScreenManager screens;
        private BaseEntity entity, controlledEntity;

        private Controls controls = new Controls();
        private Vector2 position;

        public bool IsActive { get; set; }

        private Rectangle clickRect;

        public ConversationManager(string CharacterName)
        {
            characterName = CharacterName;
            dialogueManager = new DialogueManager(characterName);
        }

        public void SetReferences(ScreenManager screens, BaseEntity entity)
        {
            this.screens = screens;
            this.entity = entity;

            dialogueManager.SetReferences(screens, entity, this);
        }
        public void SetControlledEntity(BaseEntity controlledEntity)
        {
            this.controlledEntity = controlledEntity;
            dialogueManager.SetControlledEntity(controlledEntity);
        }

        public void Load(ContentManager cm)
        {
            tab = cm.Load<Texture2D>("Interface/Character/Dialogue/dialogueTab");
            end = cm.Load<Texture2D>("Interface/Character/Dialogue/dialogueSelectEnd");
            button = cm.Load<Texture2D>("Interface/Character/Dialogue/dialogueButton");
            buttonHover = cm.Load<Texture2D>("Interface/Character/Dialogue/dialogueButtonHover");

            font = cm.Load<SpriteFont>("Fonts/RegularOutlined");

            position = new Vector2(GameSettings.VectorCenter.X - button.Center().X, GameSettings.VectorCenter.Y + (GameSettings.VectorCenter.Y / 3));
            mouseDragOffset = new Vector2(tab.Width / 2, 11);
        }

        public void Update(GameTime gt, Vector2 position)
        {
            if (IsActive == true)
            {
                this.position = position + new Vector2(-(tab.Width / 2), 128);

                clickRect = new Rectangle((int)position.X, (int)position.Y + 20, tab.Width, lastHeight);

                dialogueManager.UpdateDialogue(gt);

                controls.UpdateCurrent();

                CheckDragScreen();
                UpdateOptions(gt);

                controls.UpdateLast();
            }
        }

        private int lastHeight = 0;
        private void UpdateOptions(GameTime gt)
        {
            if (CurrentAction is DialogueOptions)
            {
                DialogueOptions options = (DialogueOptions)CurrentAction;

                for (int j = 0; j < options.Buttons.Count; j++)
                {
                    if (options.Buttons[j].action.isUsed == true && options.Buttons[j].action.IsReusable == false)
                        options.Buttons[j].IsActive = false;
                    else
                        options.Buttons[j].IsActive = true;

                    if (options.Buttons[j].IsActive == true)
                    {
                        options.Buttons[j].lineHeight = font.MeasureString(options.Buttons[j].Text.WrapText(font, 340)).Y;
                        options.Buttons[j].SelectionBox = new Rectangle((int)position.X + 11, (int)(position.Y) + lastHeight + 25, (int)button.Width, (int)options.Buttons[j].lineHeight + 6);

                        lastHeight += (int)options.Buttons[j].lineHeight + 10; //font.LineSpacing;

                        if (options.Buttons[j].SelectionBox.Contains(controls.MousePosition))
                        {
                            options.Buttons[j].IsHover = true;

                            if (isDragging == false)
                            {
                                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                                {
                                    SetCurrentLine(options.Buttons[j].ID);
                                    options.Buttons[j].IsUsed = true;
                                    //controlledEntity.CAPTION_SendImmediate(options.Buttons[j].Text, false);
                                }
                            }
                        }
                        else
                            options.Buttons[j].IsHover = false;
                    }
                    else
                        options.Buttons[j].SelectionBox = Rectangle.Empty;
                }
            }
            else
                screens.ACTIVATEBOX_SetLines("Continue", "(Press \"" + controls.CurrentControls.Activate.ToString() + "\" key)");

            lastHeight = 0;
        }

        private bool isDragging = false;
        private Vector2 mouseDragOffset;
        private Rectangle dragArea;
        private void CheckDragScreen()
        {
            dragArea = new Rectangle((int)position.X + 108, (int)position.Y + 1, 152, 17);

            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                position = controls.MouseVector - mouseDragOffset;
                screens.SetCursorState(Cursor.CursorState.Move);
            }
        }

        float offset;
        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                if (CurrentAction is DialogueOptions)
                {
                    DrawOptions(sb);

                    sb.Draw(tab, position, Color.White);
                    sb.Draw(end, new Vector2(position.X, offset + 4), Color.White);

                    sb.DrawString(font, "Options", position + new Vector2(tab.Width / 2, (tab.Height / 2) - 2), ColorHelper.UI_Gold, 0f, "Options".LineCenter(font), 1f, SpriteEffects.None, 1f);
                }
            }
        }
        private void DrawOptions(SpriteBatch sb)
        {
            DialogueOptions options = (DialogueOptions)CurrentAction;

            offset = options.Buttons.LastOrDefault().SelectionBox.Location.Y + options.Buttons.LastOrDefault().lineHeight;

            for (int j = 0; j < options.Buttons.Count; j++)
            {
                if (options.Buttons[j].IsActive == true)
                {
                    if (options.Buttons[j].IsHover == false)
                    {
                        sb.DrawTexturedBox(button, 3, (int)options.Buttons[j].lineHeight, options.Buttons[j].SelectionBox.Location.ToVector2(), Color.White, 1f);

                        string text = options.Buttons[j].Text.WrapText(font, 340);
                        if (options.Buttons[j].IsUsed == true)
                            sb.DrawString(font, text, new Vector2(options.Buttons[j].SelectionBox.X + button.Center().X, options.Buttons[j].SelectionBox.Y + (button.Center().Y * text.LineCount(font)) + 2), Color.Gray, 0f,
                                                text.LineCenter(font), 1f, SpriteEffects.None, 1f);
                        else
                            sb.DrawString(font, text, new Vector2(options.Buttons[j].SelectionBox.X + button.Center().X, options.Buttons[j].SelectionBox.Y + (button.Center().Y * text.LineCount(font)) + 2), Color.White, 0f,
                                                text.LineCenter(font), 1f, SpriteEffects.None, 1f);
                    }
                    else
                    {
                        sb.DrawTexturedBox(buttonHover, 5, (int)options.Buttons[j].lineHeight - 4, options.Buttons[j].SelectionBox.Location.ToVector2(), Color.White, 1f);

                        string text = options.Buttons[j].Text.WrapText(font, 340);
                        sb.DrawString(font, text, new Vector2(options.Buttons[j].SelectionBox.X + button.Center().X, options.Buttons[j].SelectionBox.Y + (button.Center().Y * text.LineCount(font)) + 3), ColorHelper.UI_Gold, 0f,
                                            text.LineCenter(font), 1f, SpriteEffects.None, 1f);
                    }
                }
            }
        }

        // [Encapsulation] DialogueScene
        public void SetCurrentLine(int ID) { dialogueManager.SetCurrentLineID(ID); }
        public void SetCheckpoint(int id) { dialogueManager.SetCheckpointID(id); }

        public List<ConversationAction> Actions { get { return dialogueManager.Actions; } }
        public bool IsOptionSelecting { get { return dialogueManager.IsOptionSelecting; } set { dialogueManager.IsOptionSelecting = value; } }
        public ConversationAction CurrentAction { get { return dialogueManager.CurrentAction; } }

        public void Reset()
        {
            dialogueManager.ResetCurrentID();
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            builder.AppendLine("Checkpoint " + dialogueManager.Checkpoint);
            builder.AppendLine();

            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].IsUsed == true) //Don't bother saving data that doesn't need to be!
                    builder.AppendLine(Actions[i].SaveData());
            }

            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string[] words = data[i].Split(' ');
                int id = int.Parse(words[1]);

                if (data[i].ToUpper().StartsWith("CHECKPOINT"))
                {
                    dialogueManager.SetCheckpointID(id);
                    dialogueManager.SetCurrentLineID(id);
                }

                if (data[i].ToUpper().StartsWith("ACTION"))
                {
                    for (int j = 0; j < Actions.Count; j++)
                    {
                        if (id == Actions[j].ID)
                        {
                            Actions[j].LoadData(data[i]);
                            break;
                        }
                    }
                }
            }
        }

        public bool IsClickingUI()
        {
            /*
            if (IsActive == true)
                return clickRect.Contains(controls.MousePosition) || isDragging;
            else
                return false;
            */

            return IsActive;
        }
    }
}
