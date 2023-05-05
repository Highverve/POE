using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class ToolTip
    {
        Texture2D rect;
        SpriteFont font;

        Vector2 location, offset;
        Color border, inside, textInside, textBorder;

        static StringBuilder toolTipText = new StringBuilder();

        Controls controls = new Controls();

        private static object lockString = new object();
        public static void RequestStringAssign(string newText)
        {
            lock (lockString)
            {
                if (toolTipText.Length == 0)
                {
                    toolTipText.Clear();
                    toolTipText.Append(newText);
                }
            }
        }

        public ToolTip()
            : this(new Color(38, 38, 38, 232), new Color(28, 28, 28, 255), Color.White, Color.Black)
        {

        }
        public ToolTip(Color Inside, Color Border, Color TextInside, Color TextBorder)
        {
            inside = Inside;
            border = Border;
            textInside = TextInside;
            textBorder = TextBorder;
        }

        public void LoadContent(ContentManager c)
        {
            rect = c.Load<Texture2D>("rect");
            font = c.Load<SpriteFont>("Fonts/regularOutlined");
        }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();
            location = controls.MouseVector;
        }

        public void Draw(SpriteBatch sb)
        {
            if (toolTipText.Length > 0)
            {
                Vector2 stringSize = new Vector2(font.MeasureString(toolTipText).X, font.MeasureString(toolTipText).Y - 1);

                if ((location.Y + stringSize.Y) + 15 >= GameSettings.VectorResolution.Y)
                    offset.Y = stringSize.Y + 30;
                else
                    offset.Y = 0;

                if ((location.X + stringSize.X) + 15 >= GameSettings.VectorResolution.X)
                    offset.X = stringSize.X + 30;
                else
                    offset.X = 0;

                SpriteBatchHelper.DrawBoxBordered(sb, rect, new Rectangle(((int)location.X + 10) - (int)offset.X, ((int)location.Y + 16) - (int)offset.Y, (int)stringSize.X + 14, (int)stringSize.Y + 8), inside, border, 1);
                sb.DrawString(font, toolTipText.ToString(), new Vector2(location.X + 17, location.Y + 20), ColorHelper.UI_LightGold, 0f, offset, 1f, SpriteEffects.None, 1f);
            }

            toolTipText.Clear();
        }
    }
}
