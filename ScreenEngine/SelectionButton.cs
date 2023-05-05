using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public class SelectionButton
    {
        Controls controls = new Controls();

        private enum ButtonState { None, Hover, Selected }
        private ButtonState state = ButtonState.None;

        private Texture2D background, backgroundHover, backgroundClick;
        private Vector2 offset, position;
        private string buttonText;
        private SpriteFont font;
        private Rectangle selectionRect;

        public Vector2 Offset { get { return offset; } set { offset = value; } }
        public Point Size { get { return new Point(background.Width, background.Height); } }

        public SelectionButton(Texture2D Background, Texture2D BackgroundHover, Texture2D BackgroundClick,
                               Vector2 Position, string ButtonText, SpriteFont Font)
        {
            background = Background;
            backgroundHover = BackgroundHover;
            backgroundClick = BackgroundClick;

            position = Position;
            buttonText = ButtonText;
            font = Font;
        }

        public void Update(GameTime gt, Vector2 mousePosition)
        {
            selectionRect = new Rectangle((int)position.X + (int)offset.X, (int)position.Y + (int)offset.Y, background.Width, background.Height);

            controls.UpdateLast();
            controls.UpdateCurrent();

            if (selectionRect.Contains(mousePosition))
            {
                state = ButtonState.Hover;

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    state = ButtonState.Selected;
            }
            else
                state = ButtonState.None;
        }
        public void Draw(SpriteBatch sb)
        {
            if (IsHover())
                sb.Draw(backgroundHover, position + offset, Color.White);
            else
                sb.Draw(background, position + offset, Color.White);

            sb.DrawString(font, buttonText, new Vector2(position.X + offset.X + background.Center().X, position.Y + offset.Y + background.Center().Y), buttonText.LineCenter(font), Color.White, 1f);
        }

        public bool IsHover() { return state == ButtonState.Hover; }
        public bool IsSelected() { return state == ButtonState.Selected; }
    }
}
