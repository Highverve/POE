using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate
{
    public class SoulgateButton
    {
        private string name;
        private Texture2D buttonIcon, backgroundTexture, backgroundTextureHover;

        public string Name { get { return name; } }
        public int Order { get; set; }

        public Texture2D ButtonIcon { get { return buttonIcon; } }
        public Texture2D BackgroundTexture { get { return backgroundTexture; } }
        public Texture2D BackgroundTextureHover { get { return backgroundTextureHover; } }

        public float Angle { get; set; }
        public float DistanceOffset { get; set; }
        public float DistanceLerp { get; set; }

        private Rectangle buttonRect;
        public Rectangle ButtonRect { get { return buttonRect; } set { buttonRect = value; } }

        private Vector2 buttonPosition;
        public Vector2 ButtonPosition { get { return buttonPosition; } set { buttonPosition = value; } }

        public bool IsHover { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsPrimevalButton { get; set; }

        public SoulgateButton(string Name, float Angle, Texture2D ButtonIcon, Texture2D BackgroundTexture, Texture2D BackgroundTextureHover, Action<SoulgateInterface> OnClick)
        {
            name = Name;

            this.Angle = MathHelper.ToRadians(Angle);

            buttonIcon = ButtonIcon;
            backgroundTexture = BackgroundTexture;
            backgroundTextureHover = BackgroundTextureHover;

            buttonRect = new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height);

            onClick = OnClick;
        }

        Action<SoulgateInterface> onClick;
        public Action<SoulgateInterface> OnButtonClick { get { return onClick; } }

        public int CompareTo(SoulgateButton comparison)
        {
            if (comparison == null) //A null value means that this object is greater. 
                return 1;
            else
                return this.Order.CompareTo(comparison.Order);
        }
    }
}
