using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using System;

namespace Pilgrimage_Of_Embers.ScreenEngine.Various
{

    public class DamageSlash
    {
        private Texture2D texture;
        private Color color, startColor, targetColor;
        private float lerp, lerpSpeed;

        private Vector2 center;
        private float angle, radius;
        private Line slashLine;

        private Random random;
        public bool IsActive { get; private set; }

        public DamageSlash(Vector2 Center, Color StartColor, Color EndColor, float LerpSpeed, float Radius)
        {
            center = Center;

            startColor = StartColor;
            targetColor = EndColor;
            color = startColor;

            lerpSpeed = LerpSpeed;
            radius = Radius;

            lerp = 0f;

            random = new Random(Guid.NewGuid().GetHashCode());
            angle = MathHelper.ToRadians(random.NextFloat(-180, 180));

            Vector2 startPos = center + (angle.ToVector2() * radius);
            slashLine = new Line(startPos, startPos);
        }

        public void Load(ContentManager cm)
        {
            texture = cm.Load<Texture2D>("rect");
            IsActive = true;
        }

        public void Update(GameTime gt)
        {
            lerp += lerpSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            lerp = MathHelper.Clamp(lerp, 0f, 1f);

            slashLine.locationB = Vector2.SmoothStep(slashLine.locationA, center - (angle.ToVector2() * radius), lerp);
            color = Color.Lerp(startColor, targetColor, lerp);

            if (lerp >= 1f)
                IsActive = false;
        }

        public void Draw(SpriteBatch sb)
        {
            slashLine.DrawLine(sb, texture, color, 1, 1);
            slashLine.DrawLine(sb, texture, new Vector2(1, 1), color, 1, 1);
            slashLine.DrawLine(sb, texture, new Vector2(2, 2), color, 1, 1);
        }
    }
}
