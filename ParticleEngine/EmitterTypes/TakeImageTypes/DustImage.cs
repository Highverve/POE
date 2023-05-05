using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.TakeImageTypes
{
    public class DustImage : TakeImage
    {
        public DustImage(int DepthCenter, Texture2D Image, Rectangle Clipping, float Scale)
            : base(-1, 1, DepthCenter, Image, Clipping, Scale)
        {

        }

        int time = 0;
        public override void Update(GameTime gt)
        {
            time += gt.ElapsedGameTime.Milliseconds;

            base.Update(gt);

            if (time >= 32)
                time = 0;
        }

        protected override void AddParticles()
        {
            AddPixels(3000, 8000, new Vector2(0f, -40f), new Vector2(30f, 0f));
        }

        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            if (time >= 32)
            {
                particles[i].Color.R = particles[i].Color.R.Change(-1);
                particles[i].Color.G = particles[i].Color.G.Change(-1);
                particles[i].Color.B = particles[i].Color.B.Change(-1);
                particles[i].Color.A = particles[i].Color.A.Change(-1);
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
                        entities[j].ParticleCircle.Repel(particles[i].Circle, .02f);

                        particles[i].Velocity += (entities[j].OutputMotion * trueRandom.NextFloat(2f, 5f));
                    }
                }

                if (particles[i].Velocity.X > 0f)
                    particles[i].Velocity.X -= .1f;
                if (particles[i].Velocity.X < 0f)
                    particles[i].Velocity.X += .1f;

                if (particles[i].Velocity.Y > 0f)
                    particles[i].Velocity.Y -= .025f;
                if (particles[i].Velocity.Y < 0f)
                    particles[i].Velocity.Y += .025f;
            }
            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
