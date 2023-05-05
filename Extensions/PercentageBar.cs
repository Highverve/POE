using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Extensions
{
    public class PercentageBar
    {
        private string text;
        private Vector2 position;
        public Vector2 Position { set { position = value; } }

        private Texture2D background, pixel;

        private Color color, shadowColor;

        private int barOffset, width;

        private float percentage;
        public float Percentage { get { return percentage; } set { percentage = MathHelper.Clamp(value, 0f, 1f); } }

        public PercentageBar(Texture2D Background, int BarOffset, int Width, Color PercentageColor, string ToolTipText)
        {
            this.background = Background;
            this.barOffset = BarOffset;
            this.width = Width;

            color = PercentageColor;
            shadowColor = Color.Lerp(color, Color.Black, .5f);
            text = ToolTipText;
        }

        public void Load(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");
        }

        /// <summary>
        /// Implement to allow percentage to be dragged with mouse. Not required for readonly percentage bars!
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="isDragging"></param>
        public void Update(Point mousePosition, bool isDragging, int dragMarginsY)
        {
            if (mousePosition.Y >= (position.Y - dragMarginsY) && mousePosition.Y < (position.Y + background.Height + dragMarginsY)) //If is in Y position
            {
                if (mousePosition.X >= (position.X + barOffset) && mousePosition.X < (position.X + background.Width))
                {
                    if (isDragging) //If the mouse button is down
                        Percentage = (mousePosition.X - (position.X + barOffset)) / width;
                    if (!string.IsNullOrEmpty(text))
                        ToolTip.RequestStringAssign(text);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(background, position, Color.White);

            sb.Draw(pixel, new Rectangle((int)position.X + barOffset, (int)position.Y + background.Height / 2 - 1, (int)(width * percentage), 2), color);
            sb.Draw(pixel, new Rectangle((int)position.X + barOffset, (int)position.Y + background.Height / 2 + 1, (int)(width * percentage), 1), shadowColor);
        }

        public void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(background, position, Color.White);

            sb.Draw(pixel, new Rectangle((int)position.X + barOffset, (int)position.Y + background.Height / 2 - 1, (int)(width * percentage), 2), color);
            sb.Draw(pixel, new Rectangle((int)position.X + barOffset, (int)position.Y + background.Height / 2 + 1, (int)(width * percentage), 1), Color.Lerp(color, Color.Black, .5f));
        }

    }
}
