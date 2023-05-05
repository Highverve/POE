using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public class OffscreenPoint
    {
        private string toolTipName;

        private Vector2 position;
        public Vector2 Position { get { return position; } set { position = value; } }

        public Vector2 IconPosition()
        {
            return new Vector2(MathHelper.Clamp(camera.WorldToScreen(position).X, 36, GameSettings.VectorResolution.X - 36),
                               MathHelper.Clamp(camera.WorldToScreen(position).Y, 36, GameSettings.VectorResolution.Y - 36));
        }

        private Texture2D icon;
        public Texture2D Icon
        {
            get
            {
                return icon;
            }
        }

        private Color bgColor;
        public Color BackgroundColor
        {
            get
            {
                return bgColor;
            }
        }

        public enum ScreenPosition { Up, Down, Left, Right }
        public ScreenPosition ScreenIconType()
        {
            if (position.X < camera.Position.X)
                return ScreenPosition.Left; //If the position is to the left side of the screen
            else if (position.X > camera.Position.X + GameSettings.VectorResolution.X)
                return ScreenPosition.Right;

            if (position.Y < camera.Position.Y)
                return ScreenPosition.Up;
            else if (position.Y > camera.Position.Y + GameSettings.VectorResolution.Y)
                return ScreenPosition.Down;

            return ScreenPosition.Down;
        }

        public bool IsDisplaying
        {
            get
            {
                return !camera.IsOnScreen(position, new Rectangle(50, 50, 50, 50));
            }
        }
        public bool IsActive { get; set; }

        public static Color Red = new Color(172, 67, 65, 255);
        public static Color Green = new Color(62, 130, 77, 255);
        public static Color Blue = new Color(62, 97, 130, 255);
        public static Color Orange = new Color(139, 83, 42, 255);
        public static Color Purple = new Color(86, 62, 97, 255);

        private Camera camera;

        public OffscreenPoint(Texture2D Icon, Color BackgroundColor)
        {
            icon = Icon;
            bgColor = BackgroundColor;

            this.IsActive = true;
        }
        public void SetReferences(Camera camera)
        {
            this.camera = camera;
        }
    }
    public class OffscreenHUD
    {
        private List<OffscreenPoint> offPoints = new List<OffscreenPoint>();

        private Texture2D up, down, left, right;
        private Vector2 upOrigin = new Vector2(32, 32), downOrigin = new Vector2(32, 64), leftOrigin = new Vector2(0, 32), rightOrigin = new Vector2(64, 32);

        private Camera camera;

        public OffscreenHUD()
        {

        }
        public void SetReferences(Camera camera)
        {
            this.camera = camera;
        }

        public void Load(ContentManager cm)
        {
            up = cm.Load<Texture2D>("Interface/HUD/Offscreen/grayOmni");
            down = cm.Load<Texture2D>("Interface/HUD/Offscreen/grayDown");
            left = cm.Load<Texture2D>("Interface/HUD/Offscreen/grayLeft");
            right = cm.Load<Texture2D>("Interface/HUD/Offscreen/grayRight");
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < offPoints.Count; i++)
            {
                if (offPoints[i].IsActive == false)
                    offPoints.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < offPoints.Count; i++)
            {
                if (offPoints[i].IsDisplaying == true)
                {
                    switch (offPoints[i].ScreenIconType())
                    {
                        case OffscreenPoint.ScreenPosition.Up: DrawBG(sb, up, upOrigin, offPoints[i]); break;
                        case OffscreenPoint.ScreenPosition.Down: DrawBG(sb, up, upOrigin, offPoints[i]); break;
                        case OffscreenPoint.ScreenPosition.Left: DrawBG(sb, up, upOrigin, offPoints[i]); break;
                        case OffscreenPoint.ScreenPosition.Right: DrawBG(sb, up, upOrigin, offPoints[i]); break;
                    }
                }
            }
        }
        private Color endColor = Color.Lerp(Color.White, Color.Transparent, .75f);
        private void DrawBG(SpriteBatch sb, Texture2D texture, Vector2 origin, OffscreenPoint point)
        {
            float distance = Vector2.Distance(camera.Position + GameSettings.VectorCenter, point.Position) - 2500f;
            float distanceLerp = MathHelper.Clamp((distance / 1500), 0f, 1f);

            sb.Draw(texture, point.IconPosition(), Color.Lerp(point.BackgroundColor, Color.Black, distanceLerp), origin, 0f, MathHelper.Lerp(1f, 0f, distanceLerp), SpriteEffects.None, 0f);

            if (point.Icon != null)
                sb.Draw(point.Icon, point.IconPosition(), Color.Lerp(Color.White, endColor, distanceLerp), origin, 0f, MathHelper.Lerp(1f, 0f, distanceLerp), SpriteEffects.None, 0f);
        }

        public void AddPoint(OffscreenPoint point)
        {
            if (point != null)
            {
                point.SetReferences(camera);
                offPoints.Add(point);
            }
        }

    }
}
