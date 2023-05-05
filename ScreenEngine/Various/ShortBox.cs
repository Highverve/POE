using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ScreenEngine.Various
{
    public class InteractBox
    {
        private Texture2D background;
        private string lineOne, lineTwo;

        private SpriteFont largeFont, font;
        private Vector2 position;

        public bool IsActive { get; set; }

        public InteractBox() { }
        public void SetValues(string lineOne, string lineTwo)
        {
            this.lineOne = lineOne;
            this.lineTwo = lineTwo;
        }

        public void Load(ContentManager cm)
        {
            background = cm.Load<Texture2D>("Interface/Various/shortBox");

            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");

            position = GameSettings.VectorCenter + new Vector2(0, GameSettings.VectorResolution.Y / 4);
        }

        private Color halfTrans = Color.Lerp(Color.Transparent, Color.White, .5f);
        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                sb.Draw(background, position, Color.White, background.Center(), 0f, 1f);
                sb.DrawString(largeFont, lineOne, position - new Vector2(0, largeFont.LineSpacing / 2), lineOne.LineCenter(largeFont), Color.White, 1f);
                sb.DrawString(font, lineTwo, position + new Vector2(0, font.LineSpacing / 2), lineTwo.LineCenter(font), ColorHelper.UI_Gold, 1f);
            }
        }

        public void ResetPosition()
        {
            position = GameSettings.VectorCenter + new Vector2(0, GameSettings.VectorResolution.Y / 5);
        }
    }
}
