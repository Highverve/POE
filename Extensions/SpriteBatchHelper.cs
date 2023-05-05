using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public static class SpriteBatchHelper
    {
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, Vector2 origin, float angle, float depth)
        {
            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), color, angle, origin, 1f, SpriteEffects.None, depth);
        }
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, Vector2 origin, float angle, float scale, float depth)
        {
            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), color, angle, origin, scale, SpriteEffects.None, depth);
        }
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, Vector2 origin, float angle, float scale, SpriteEffects effects, float depth)
        {
            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), color, angle, origin, scale, effects, depth);
        }

        public static void DrawString(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Vector2 origin, Color color, float scale)
        {
            sb.DrawString(font, text, position, color, 0f, origin, scale, SpriteEffects.None, 1f);
        }
        public static void DrawString(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Vector2 origin, Color color, float scale, float depth)
        {
            sb.DrawString(font, text, position, color, 0f, origin, scale, SpriteEffects.None, depth);
        }

        public static void DrawStringBordered(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color inside, Color border, int outlineWidth = 1)
        {
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X - outlineWidth, position.Y + outlineWidth), border);
                spriteBatch.DrawString(font, text, new Vector2(position.X + outlineWidth, position.Y + outlineWidth), border);
                spriteBatch.DrawString(font, text, new Vector2(position.X - outlineWidth, position.Y - outlineWidth), border);
                spriteBatch.DrawString(font, text, new Vector2(position.X + outlineWidth, position.Y - outlineWidth), border);

                spriteBatch.DrawString(font, text, position, inside);
            }
        }
        public static void DrawStringBordered(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Vector2 origin, float angle, float scale, float depth, Color inside, Color border, int outlineWidth = 1, float depthOffset = .0001f)
        {
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X - outlineWidth, position.Y + outlineWidth), border, angle, origin, scale, SpriteEffects.None, depth - depthOffset);
                spriteBatch.DrawString(font, text, new Vector2(position.X + outlineWidth, position.Y + outlineWidth), border, angle, origin, scale, SpriteEffects.None, depth - depthOffset);
                spriteBatch.DrawString(font, text, new Vector2(position.X - outlineWidth, position.Y - outlineWidth), border, angle, origin, scale, SpriteEffects.None, depth - depthOffset);
                spriteBatch.DrawString(font, text, new Vector2(position.X + outlineWidth, position.Y - outlineWidth), border, angle, origin, scale, SpriteEffects.None, depth - depthOffset);

                spriteBatch.DrawString(font, text, position, inside, angle, origin, scale, SpriteEffects.None, depth);
            }
        }

        public static void DrawStringShadow(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color inside, Color border, int outlineWidth = 1)
        {
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X + outlineWidth, position.Y + outlineWidth), border);
                spriteBatch.DrawString(font, text, position, inside);
            }
        }
        public static void DrawStringShadow(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Vector2 origin, float angle, float scale, float depth, Color inside, Color border, int outlineWidth = 1)
        {
            if (!string.IsNullOrEmpty(text))
            {
                spriteBatch.DrawString(font, text, new Vector2(position.X + outlineWidth, position.Y + outlineWidth), border, angle, origin, scale, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, position, inside, angle, origin, scale, SpriteEffects.None, depth);
            }
        }

        public static void DrawMoneyQuantity(this SpriteBatch sb, SpriteFont font, int itemQuantity, Vector2 position)
        {
            if (itemQuantity > 999)
            {
                float itemAmount = (float)itemQuantity * .001f;
                Color numberColor = Color.LightGreen;

                if (itemQuantity > 100000)
                    numberColor = Color.Lerp(Color.LightGreen, Color.Green, .5f);

                sb.DrawString(font, string.Format("{0:0.#}" + "K", itemAmount), position, Vector2.Zero, numberColor, 1f);
            }
            else
                sb.DrawString(font, itemQuantity.ToString(), position, Vector2.Zero, Color.White, 1f);
        }
        public static void DrawMoneyQuantity(this SpriteBatch sb, SpriteFont font, int itemQuantity, string afterText, Vector2 position, bool isCenterOrigin)
        {
            if (itemQuantity > 999)
            {
                float itemAmount = (float)itemQuantity * .001f;
                Color numberColor = Color.LightGreen;

                if (itemQuantity > 100000)
                    numberColor = Color.Lerp(Color.LightGreen, Color.Green, .5f);

                string formatted = string.Format("{0:0.#}" + "K", itemAmount) + afterText;

                Vector2 origin = Vector2.Zero;
                if (isCenterOrigin == true)
                    origin = formatted.LineCenter(font);

                sb.DrawString(font, formatted, position, numberColor, 0f, origin, 1f, SpriteEffects.None, 1f);
            }
            else
                sb.DrawString(font, itemQuantity.ToString() + afterText, position, Vector2.Zero, Color.White, 1f);
        }

        public static void DrawBoxBordered(this SpriteBatch spriteBatch, Texture2D pixel, Rectangle rectangle, Color inside, Color outside, int outlineWidth = 1)
        {
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X - outlineWidth, rectangle.Y - outlineWidth, rectangle.Width + (outlineWidth * 2), outlineWidth), outside); //Top
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X - outlineWidth, rectangle.Y + rectangle.Height, rectangle.Width + (outlineWidth * 2), outlineWidth), outside); //Bottom
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X - outlineWidth, rectangle.Y, outlineWidth, rectangle.Height), outside); //Left
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, outlineWidth, rectangle.Height), outside); //Right

            if (!inside.Equals(Color.Transparent)) //Skip drawing if the inside color is fully transparent.
                spriteBatch.Draw(pixel, rectangle, inside);
        }
        public static void DrawBoxBordered(this SpriteBatch spriteBatch, Texture2D pixel, Rectangle rectangle, Color inside, Color outside, float depth, int outlineWidth = 1)
        {
            spriteBatch.Draw(pixel, new Vector2(rectangle.X - outlineWidth, rectangle.Y - outlineWidth), new Rectangle(0, 0, rectangle.Width + (outlineWidth * 2), outlineWidth), outside, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Top
            spriteBatch.Draw(pixel, new Vector2(rectangle.X - outlineWidth, rectangle.Y + rectangle.Height), new Rectangle(0, 0, rectangle.Width + (outlineWidth * 2), outlineWidth), outside, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Bottom
            spriteBatch.Draw(pixel, new Vector2(rectangle.X - outlineWidth, rectangle.Y), new Rectangle(0, 0, outlineWidth, rectangle.Height), outside, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Left
            spriteBatch.Draw(pixel, new Vector2(rectangle.X + rectangle.Width, rectangle.Y), new Rectangle(0, 0, outlineWidth, rectangle.Height), outside, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Right

            spriteBatch.Draw(pixel, new Vector2(rectangle.X, rectangle.Y), new Rectangle(0, 0, rectangle.Width, rectangle.Height), inside, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
        }

        public static void DrawTexturedBox(this SpriteBatch spriteBatch, Texture2D texture, int sourceVerticalSize, int boxHeight, Vector2 position, Color color, float depth)
        {
            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, sourceVerticalSize), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Draw top half
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y + sourceVerticalSize, texture.Width, boxHeight), new Rectangle(0, sourceVerticalSize, texture.Width, 2), color, 0f, Vector2.Zero, SpriteEffects.None, depth); //Draw middle section
            spriteBatch.Draw(texture, position + new Vector2(0, sourceVerticalSize + boxHeight), new Rectangle(0, texture.Height - sourceVerticalSize, texture.Width, sourceVerticalSize), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Draw bottom half
        }
        public static void DrawTexturedBox(this SpriteBatch spriteBatch, Texture2D texture, int topVerticalSize, int bottomVerticalSize, int boxHeight, Vector2 position, Color color, float depth)
        {
            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, topVerticalSize), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Draw top half
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y + topVerticalSize, texture.Width, boxHeight), new Rectangle(0, topVerticalSize, texture.Width, 2), color, 0f, Vector2.Zero, SpriteEffects.None, depth); //Draw middle section
            spriteBatch.Draw(texture, position + new Vector2(0, topVerticalSize + boxHeight), new Rectangle(0, texture.Height - bottomVerticalSize, texture.Width, bottomVerticalSize), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth); //Draw bottom half
        }


        /*
        public void DrawStringFragmented(SpriteBatch spriteBatch)
        {
            sb.DrawString(sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 3, 5), Color.Lerp(Color.White, Color.Transparent, .5f));
            sb.DrawString(sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 5, 5), Color.Lerp(Color.White, Color.Transparent, .5f));
            sb.DrawString(sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 3, 3), Color.Lerp(Color.White, Color.Transparent, .5f));
            sb.DrawString(sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 5, 3), Color.Lerp(Color.White, Color.Transparent, .5f));

            sb.DrawString(sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 4, 4), Color.Lerp(Color.White, Color.Transparent, .5f));
        }*/
    }
}
