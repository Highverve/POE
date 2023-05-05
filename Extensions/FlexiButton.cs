using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Extensions
{
    public class FlexiButton
    {
        Controls controls = new Controls();

        private Texture2D button;
        private Color tintHover, tintClick;

        public Vector2 Position { get; set; }
        private Rectangle buttonRect;

        public FlexiButton(Texture2D Button, Rectangle ClickRect, Color HoverTint, Color ClickTint, Vector2 ButtonPosition = new Vector2())
        {
            button = Button;
            buttonRect = ClickRect;

            tintHover = HoverTint;
            tintClick = ClickTint;

            Position = ButtonPosition;
        }

        public void Load(ContentManager cm)
        {

        }
        public void Update(GameTime gt)
        {
            controls.UpdateLast();
            controls.UpdateCurrent();

            if (buttonRect.Contains(controls.MousePosition))
            {
                IsHover = true;

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    IsLeftClicked = true;
                else
                    IsLeftClicked = false;

                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    IsRightClicked = true;
                else
                    IsRightClicked = false;

                if (controls.IsClickedOnce(Controls.MouseButton.MiddleClick))
                    IsMiddleClicked = true;
                else
                    IsMiddleClicked = false;
            }
            else
                IsHover = false;
        }
        public void Draw(SpriteBatch sb)
        {

        }

        public bool IsHover { set; get; }
        public bool IsLeftClicked { set; get; }
        public bool IsRightClicked { set; get; }
        public bool IsMiddleClicked { set; get; }
    }
}
