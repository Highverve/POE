using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Content;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public class PauseScreen
    {
        SpriteFont largeFont, font;
        Texture2D pixel;

        Rectangle pixelRect;

        private Controls controls = new Controls();

        private bool isPaused = false;
        public bool IsPaused { get { return isPaused; } set { isPaused = value; } }

        public PauseScreen()
        {
        }

        public void Load(ContentManager cm)
        {
            largeFont = cm.Load<SpriteFont>("Fonts/boldOutlined");
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            pixel = cm.Load<Texture2D>("rect");
        }

        bool increaseAlpha = false;
        float width = 0, height = 0;
        public void Update(GameTime gt)
        {
            pixelRect = new Rectangle(2, 2, GameSettings.WindowResolution.X - 4, GameSettings.WindowResolution.Y - 4);

            colorAlpha = MathHelper.Clamp(colorAlpha, .25f, 1f);
            pixelAlpha = MathHelper.Clamp(pixelAlpha, 0f, .85f);

            if (colorAlpha >= 1f)
                increaseAlpha = false;
            else if (colorAlpha <= .25f)
                increaseAlpha = true;

            if (increaseAlpha == true)
                colorAlpha += 1.5f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                colorAlpha -= 1.5f * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        float colorAlpha = 1f, pixelAlpha = 1f;
        Color title = Color.SkyBlue, text, pixelColor;
        StringBuilder belowText = new StringBuilder(300);
        public void Draw(SpriteBatch sb)
        {
            text = Color.Lerp(Color.Transparent, Color.LightGray, colorAlpha);
            pixelColor = Color.Lerp(Color.Transparent, Color.Black, pixelAlpha);

            sb.DrawBoxBordered(pixel, pixelRect, pixelColor, Color.Black, 2);

            belowText.Append("(Unpause with '" + controls.KeyString(controls.CurrentControls.Pause) + "')");

            sb.DrawString(largeFont, "Paused", new Vector2(GameSettings.VectorCenter.X, GameSettings.VectorCenter.Y - 100), "Paused".LineCenter(largeFont), title, 1f);
            sb.DrawString(font, belowText.ToString(), new Vector2(GameSettings.VectorCenter.X, (GameSettings.VectorCenter.Y - 100) + largeFont.LineSpacing), belowText.ToString().LineCenter(font), text, 1f);

            belowText.Clear();
        }
    }
}
