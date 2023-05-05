using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ScreenEngine.Various
{
    public class SelectionBox
    {
        Controls controls = new Controls();

        private int currentSelection = 0;
        public int CurrentSelection { get { return currentSelection; } }

        private List<SelectionButton> buttons = new List<SelectionButton>();

        private string[] buttonNames;
        private string title, message;
        private Texture2D background, button, buttonHover, buttonClick;
        private SpriteFont font;

        public void SetMessage(string message) { this.message = message; }

        private Vector2 position;
        public void ResetPosition() { position = GameSettings.VectorCenter - background.Center(); }
        public Vector2 Position { set { position = value; } }

        private float yOffset = 120;

        private Rectangle dragArea;

        public bool IsActive { get; set; }

        public SelectionBox(string Title, string BoxMessage, params string[] Buttons)
        {
            title = Title;
            message = BoxMessage;

            buttonNames = Buttons;
        }

        public void Load(ContentManager cm)
        {
            background = cm.Load<Texture2D>("Interface/Various/Selection/background");
            position = GameSettings.VectorCenter - background.Center();

            mouseOffset = new Vector2(background.Center().X, 20);

            AssignButtons(cm);
        }
        private void AssignButtons(ContentManager cm)
        {
            button = cm.Load<Texture2D>("Interface/Various/Selection/button");
            buttonHover = cm.Load<Texture2D>("Interface/Various/Selection/buttonHover");
            buttonClick = cm.Load<Texture2D>("Interface/Various/Selection/button");

            font = cm.Load<SpriteFont>("Fonts/regularOutlined");

            for (int i = 0; i < buttonNames.Length; i++)
                buttons.Add(new SelectionButton(button, buttonHover, buttonClick, Vector2.Zero, buttonNames[i], font));
        }

        public void Update(GameTime gt)
        {
            Update(gt, controls.MouseVector);
        }
        public void Update(GameTime gt, Vector2 mousePosition)
        {
            currentSelection = -1; // Reset the currentSelection to none

            clickingUI = new Rectangle((int)position.X, (int)position.Y, background.Width, background.Height + (buttons.Count * button.Height));

            controls.UpdateCurrent();

            if (IsActive == true)
            {
                dragArea = new Rectangle((int)position.X, (int)position.Y, background.Width, 40);
                Drag();

                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].Update(gt, mousePosition);
                    CheckButtonSelect(i);
                }
            }

            controls.UpdateLast();
        }
        private void CheckButtonSelect(int index)
        {
            if (buttons.Count - 1 >= index)
            {
                if (buttons[index] != null)
                {
                    if (buttons[index].IsSelected())
                        currentSelection = index;
                }
            }
        }

        private bool isDrag = false;
        private Vector2 mouseOffset;
        private void Drag()
        {
            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDrag = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDrag = false;

            if (isDrag == true)
                position = controls.MouseVector - mouseOffset;
        }

        List<string> tempMessageLines = new List<string>();
        public void Draw(SpriteBatch sb)
        {
            if (this.IsActive == true)
            {
                sb.DrawTexturedBox(background, 117, 17, ((button.Height + 2) * buttons.Count) + 5, position, Color.White, 1f);

                tempMessageLines = message.SplitLines(font, 300);

                sb.DrawString(font, title, new Vector2(position.X + background.Width / 2, position.Y + 14), title.LineCenter(font), Color.White, 1f);
                for (int i = 0; i < tempMessageLines.Count; i++)
                    sb.DrawString(font, tempMessageLines[i], new Vector2(position.X + background.Width / 2, position.Y + (i * font.LineSpacing) + 40), tempMessageLines[i].LineCenter(font), Color.White, 1f);

                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].Offset = new Vector2(position.X + ((background.Width / 2) - (buttons[i].Size.X / 2)), position.Y + (i * (buttons[i].Size.Y + 2)) + yOffset);
                    buttons[i].Draw(sb);
                }
            }
        }

        private Rectangle clickingUI;
        public bool IsClickingUI() { if (IsActive == true) return clickingUI.Contains(controls.MousePosition); else return false; }

        public static SelectionBox Empty()
        {
            return new SelectionBox(string.Empty, string.Empty, string.Empty);
        }

        public override bool Equals(object obj)
        {
            if (obj == Empty() || GetType() != obj.GetType() || obj == null)
                return false;

            SelectionBox box = (SelectionBox)obj;
            return (box.title + box.message).ToUpper() == (this.title + this.message).ToUpper();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
