using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Screen_based
{
    public class DeathEmbers : BaseEmitter
    {
        public DeathEmbers() : base(-1, 0, 0)
        {
            position = new Vector2(0, GameSettings.VectorResolution.Y);
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("Particles/white"), new Vector2(GameSettings.VectorResolution.X + 100, 50), .2f, 0, 2, 3000);

            if (GameSettings.LimitParticles == true)
            {
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
            }
        }

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, 5, 1, () =>
            {
                Particle particle = NextRecycle();
                particle.SetMainRecycledVariables(values.Texture, values.position, values.velocity, values.angle,
                                                  values.color, values.size, values.lifeTime, values.depth);
                particles.Add(particle);
            }, false);

            base.Update(gt);
        }

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (float)(trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (float)(trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));

            values.lifeTime = trueRandom.Next(4000, 6000);

            values.velocity.Y = (float)trueRandom.Next(-55, 5);
            values.velocity.X = (float)trueRandom.Next(-5, 25);

            base.ApplyRandomValues();
        }
        private Color orangeHalf = Color.Lerp(Color.Orange, Color.Transparent, .45f);
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            particles[i].Velocity.X += 10f * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Velocity.Y -= 20f * (float)gt.ElapsedGameTime.TotalSeconds;

            particles[i].Color = Color.Lerp(Color.Transparent, orangeHalf, (float)particles[i].TimeLeft / 4000);
            particles[i].Size -= .025f * (float)gt.ElapsedGameTime.TotalSeconds;

            if (this.IsActivated == false)
            {
                particles[i].Velocity.Y += 50f * (float)gt.ElapsedGameTime.TotalSeconds;
                particles[i].TimeLeft -= gt.ElapsedGameTime.Milliseconds * 3;
            }

            base.ApplyParticleBehavior(gt, i);
        }

        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }

    }
}
