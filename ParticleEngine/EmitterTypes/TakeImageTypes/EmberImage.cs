using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.TakeImageTypes
{
    public class EmberImage : TakeImage
    {
        public EmberImage(int DepthCenter, Texture2D Image, Rectangle Clipping, float Scale)
            : base(-1, 1, DepthCenter, Image, Clipping, Scale)
        {

        }

        int time = 0;
        int movingToEntityTimer = 0;

        public override void Update(GameTime gt)
        {
            time += gt.ElapsedGameTime.Milliseconds;
            movingToEntityTimer += gt.ElapsedGameTime.Milliseconds;            

            base.Update(gt);

            if (time >= 32)
                time = 0;
        }

        protected override void AddParticles()
        {
            AddPixels(8500, 10000, new Vector2(-15f, -30f), new Vector2(15f, 15f), trueRandom.Next(30, 32));
        }

        private Color lightOrange = Color.Lerp(Color.White, Color.DarkOrange, .75f);
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            if (time >= 32)
            {
                if (particles[i].TimeLeft > 7000 && particles[i].TimeLeft < 8000)
                    particles[i].Color = Color.Lerp(lightOrange, particles[i].StartingColor, (float)(particles[i].TimeLeft - 7000) / 1500);

                if (particles[i].TimeLeft > 3000 && particles[i].TimeLeft < 7000)
                    particles[i].Color = Color.Lerp(ColorHelper.Charcoal, lightOrange, (float)(particles[i].TimeLeft - 3000) / 3000);

                if (particles[i].TimeLeft >= 0 && particles[i].TimeLeft < 3000)
                    particles[i].Color = Color.Lerp(Color.Transparent, ColorHelper.Charcoal, (float)particles[i].TimeLeft / 3000);
            }

            if (particles[i].TimeLeft >= 7500)
                particles[i].Size -= .25f * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (particles[i].TimeLeft >= 0 && particles[i].TimeLeft <= 4500)
                particles[i].Velocity.Y += 10f * (float)gt.ElapsedGameTime.TotalSeconds;

            if (particles[i].Position.Y >= particles[i].Floor)
            {
                particles[i].Velocity.X = 0f;
                particles[i].Position.Y = particles[i].Floor;
            }

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            for (int j = 0; j < entities.Count; j++)
            {
                if (entities[j].IsDead == false)
                {
                    if (entities[j].ParticleCircle.Intersects(particles[i].Circle))
                    {
                        particles[i].TimeLeft -= gt.ElapsedGameTime.Milliseconds * 20;
                    }
                }
            }
            
            if (particles[i].TimeLeft > 4500)
            {
                float distance = Vector2.Distance(Position, particles[i].Position);
                if (distance >= 8f)
                {
                    Vector2 direction = Vector2.Normalize((Position + new Vector2(64f + trueRandom.NextFloat(8, 8))) - particles[i].Position);
                    float speed = MathHelper.Clamp(distance, 0f, 20f);

                    particles[i].Velocity += direction * (speed * (float)gt.ElapsedGameTime.TotalSeconds);
                }
            }

            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
