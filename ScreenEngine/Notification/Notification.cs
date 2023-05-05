using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.ScreenEngine.Notification
{
    public class Notification
    {
        Texture2D icon;
        string text, fullText;
        private Vector2 position = new Vector2(0, -30);

        public Texture2D Icon { get { return icon; } }
        public string Text { get { return text; } }
        public string FullText { get { return fullText; } }
        public float PositionY { get { return position.Y; } set { position.Y = MathHelper.Clamp(value, -30f, 8f); } }

        public const int maxLength = 27;
        public const int maxLengthWide = 53;

        public Notification(Texture2D Icon, string Text, bool isWidescreen)
        {
            icon = Icon;
            text = Text;
            fullText = Text;

            if (isWidescreen == false)
            {
                if (text.Length > maxLength)
                {
                    text = text.Substring(0, maxLength - 4);
                    text += "...";
                }
            }
            else
            {
                if (text.Length > maxLengthWide)
                {
                    text = text.Substring(0, maxLengthWide - 4);
                    text += "...";
                }
            }
        }

        public override bool Equals(object obj)
        {
            Notification n = (Notification)obj;
            return (this.text == n.text && this.icon == n.icon);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
