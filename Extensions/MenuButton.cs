using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class MenuButton
    {
        Rectangle buttonRect;
        Texture2D buttonTexture, clickTexture, hoverTexture;

        BasicAnimation animation = new BasicAnimation();

        Vector2 origin;
        public Vector2 Center { get { return new Vector2(buttonRect.X + origin.X * scale, buttonRect.Y + origin.Y * scale); } }
        public Vector2 CenterNormal { get { return new Vector2(buttonRect.X + origin.X, buttonRect.Y + origin.Y); } }

        public Rectangle ButtonRectangle { get { return buttonRect; } set { buttonRect = value; } }
        public Point Position { get { return new Point(buttonRect.X, buttonRect.Y); } set { buttonRect.X = value.X; buttonRect.Y = value.Y; } }
        public Point PositionCenter { set { buttonRect.X = value.X - (int)origin.X; buttonRect.Y = value.Y - (int)origin.Y; } }

        public float scale { get; set; }

        public string textHolder;

        /// <summary>
        /// Assign both variables manually.
        /// </summary>
        /// <param name="ButtonSize">Size of clickable region</param>
        /// <param name="ButtonTexture">Texture used for button</param>
        public MenuButton(Rectangle ButtonSize, Texture2D ButtonTexture, Texture2D ClickTexture, Texture2D HoverTexture)
        {
            buttonRect = ButtonSize;

            buttonTexture = ButtonTexture;
            clickTexture = ClickTexture;
            hoverTexture = HoverTexture;
        }

        /// <summary>
        /// Assigns the button size based entirely on the texture size
        /// </summary>
        /// <param name="ButtonTexture"></param>
        public MenuButton(Vector2 ButtonPosition, Texture2D ButtonTexture, Texture2D ClickTexture, Texture2D HoverTexture, float Scale, bool isHalfOrigin = false)
        {
            buttonTexture = ButtonTexture;
            clickTexture = ClickTexture;
            hoverTexture = HoverTexture;

            if (isHalfOrigin == true)
                origin = new Vector2(buttonTexture.Width / 2, buttonTexture.Height / 2);
            else
                origin = Vector2.Zero;

            scale = Scale;

            buttonRect = new Rectangle((int)ButtonPosition.X - (int)origin.X * (int)scale, (int)ButtonPosition.Y - (int)origin.Y * (int)scale,
                                       buttonTexture.Width * (int)scale, buttonTexture.Height * (int)scale);
        }

        public bool IsButtonPressed { get; private set; }
        public void Update(GameTime gt, Controls controls)
        {
            Update(gt, controls, controls.MouseVector);
        }
        public void Update(GameTime gt, Controls controls, Vector2 mousePosition)
        {
            IsButtonPressed = false;

            if (buttonRect.Contains(mousePosition))
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

                if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
                    controls.CurrentMS.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
                    controls.CurrentMS.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    IsButtonPressed = true;
                }
            }
            else
            {
                IsHover = false;

                IsLeftClicked = false;
                IsRightClicked = false;
                IsMiddleClicked = false;
            }
        }

        /// <summary>
        /// Draw only the button. Be sure to draw the text(with SpriteBatchHelper) after drawing this.
        /// </summary>
        /// <param name="sb"></param>
        public void DrawButton(SpriteBatch sb, Color color)
        {
            //Add in animation support for all of the button textures.
            if (IsButtonPressed == true)
                sb.Draw(clickTexture, buttonRect, color);
            else if (IsHover == true)
                sb.Draw(hoverTexture, buttonRect, color);
            else
                sb.Draw(buttonTexture, buttonRect, color);

            /* For debugging purposes only!
            Texture2D tempTex = new Texture2D(sb.GraphicsDevice, 1, 1);
            Color[] temp = new Color[] { Color.White };
            tempTex.SetData<Color>(temp);

            SpriteBatchHelper.DrawBoxBordered(sb, tempTex, new Rectangle((int)CenterNormal.X, (int)CenterNormal.Y, 3, 3), Color.Lerp(Color.Transparent, Color.White, .5f), Color.Green);
            */
        }

        public void DrawButton(SpriteBatch sb, Color color, float scale)
        {
            //Add in animation support for all of the button textures.
            if (IsHover == true)
                sb.Draw(hoverTexture, new Vector2(buttonRect.X, buttonRect.Y), color, Vector2.Zero, 0f, scale, SpriteEffects.None, 0f);
            else if (IsLeftClicked == true || IsMiddleClicked == true || IsRightClicked == true)
                sb.Draw(clickTexture, new Vector2(buttonRect.X, buttonRect.Y), color, Vector2.Zero, 0f, scale, SpriteEffects.None, 0f);
            else
                sb.Draw(buttonTexture, new Vector2(buttonRect.X, buttonRect.Y), color, Vector2.Zero, 0f, scale, SpriteEffects.None, 0f);

            /* For debugging purposes only!
            Texture2D tempTex = new Texture2D(sb.GraphicsDevice, 1, 1);
            Color[] temp = new Color[] { Color.White };
            tempTex.SetData<Color>(temp);

            SpriteBatchHelper.DrawBoxBordered(sb, tempTex, new Rectangle((int)CenterNormal.X, (int)CenterNormal.Y, 3, 3), Color.Lerp(Color.Transparent, Color.White, .5f), Color.Green);
            */
        }

        public void DrawButtonIdle(SpriteBatch sb, Color color)
        {
            sb.Draw(buttonTexture, buttonRect, color);
        }

        public bool IsHover { set; get; }
        public bool IsLeftClicked { set; get; }
        public bool IsRightClicked { set; get; }
        public bool IsMiddleClicked { set; get; }

        public void ResetButtonStates()
        {
            IsHover = false;
            IsLeftClicked = false;
            IsRightClicked = false;
            IsMiddleClicked = false;
        }
    }
}
