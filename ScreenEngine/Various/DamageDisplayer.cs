using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using System;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.ScreenEngine.Various
{
    public class DamagePoint
    {
        string value;
        Vector2 position;
        Color color;
        float colorFade;
        float maxFloorY;

        public string Value { get { return value; } }
        public Vector2 Position { get { return position; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }
        public float X { get { return position.X; } set { position.X = value; } }

        public float MaxFloorY { get { return maxFloorY; } }

        TileEngine.DepthFloor currentDepth;
        public int CurrentFloor { get { return currentDepth.CurrentFloor; } set { currentDepth.CurrentFloor = value; } }
        public int LastFloor { get; set; }
        public float Depth { get { return currentDepth.Depth; } }
        public void UpdateDepth(Camera camera) { currentDepth.UpdateDepth(camera.WorldToScreen(Position).Y); }

        public float VelocityY { get; set; }
        public float VelocityX { get; set; }

        public Color Color { get { return Color.Lerp(Color.Transparent, color, colorFade); } }
        public float ColorFade { get { return colorFade; } set { colorFade = MathHelper.Clamp(value, 0f, 1f); } }

        public bool IsDownDirection { get; set; }

        private Random random;

        public DamagePoint(string Value, Vector2 StartingPosition, Color Color, int DepthFloor, bool IsDirectionDown)
        {
            random = new Random(Guid.NewGuid().GetHashCode());

            value = Value;
            position = StartingPosition;
            color = Color;

            colorFade = 1f;
            maxFloorY = position.Y + (40f + random.Next(0, 15));

            currentDepth = new TileEngine.DepthFloor();
            currentDepth.CurrentFloor = DepthFloor;
            LastFloor = DepthFloor;

            VelocityY = -.1f;
            VelocityX = random.NextFloat(-.1f, .1f);

            IsDownDirection = IsDirectionDown;
        }
    }

    public enum DamageAnimation
    {
        Fall,
        Rise,
        SweepLeft,
        SweepRight,
    }

    public class DamageDisplayer
    {
        private SpriteFont font;
        private List<DamagePoint> damagePoints = new List<DamagePoint>();

        private Camera camera;

        public DamageDisplayer(Camera Camera) { camera = Camera; }

        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/boldOutlined");
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < damagePoints.Count; i++)
            {
                damagePoints[i].UpdateDepth(camera);

                if (damagePoints[i].IsDownDirection == true)
                {
                    damagePoints[i].ColorFade -= .5f * (float)gt.ElapsedGameTime.TotalSeconds;

                    if (damagePoints[i].Y <= damagePoints[i].MaxFloorY)
                    {
                        damagePoints[i].VelocityY += 1f * (float)gt.ElapsedGameTime.TotalSeconds;

                        damagePoints[i].Y += damagePoints[i].VelocityY * (float)gt.ElapsedGameTime.TotalMilliseconds;
                        damagePoints[i].X += damagePoints[i].VelocityX * (float)gt.ElapsedGameTime.TotalMilliseconds;
                    }

                    if (damagePoints[i].Y >= damagePoints[i].MaxFloorY - 30f)
                        damagePoints[i].CurrentFloor = damagePoints[i].LastFloor - 1;
                }
                else //Float the text up...
                {
                    damagePoints[i].ColorFade -= 1.5f * (float)gt.ElapsedGameTime.TotalSeconds;
                    damagePoints[i].VelocityY += .6f * (float)gt.ElapsedGameTime.TotalSeconds;

                    damagePoints[i].Y -= damagePoints[i].VelocityY;
                    damagePoints[i].X += damagePoints[i].VelocityX;
                }

                if (damagePoints[i].ColorFade <= 0f)
                    damagePoints.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < damagePoints.Count; i++)
            {
                sb.DrawString(font, damagePoints[i].Value, damagePoints[i].Position,
                                    damagePoints[i].Value.LineCenter(font), damagePoints[i].Color,
                                    1f, damagePoints[i].Depth);
            }
        }

        public void AddPoint(string value, Vector2 position, Color color, int depthFloor, bool isDownDirection)
        {
            damagePoints.Add(new DamagePoint(value, position, color, depthFloor, isDownDirection));
        }
    }
}
