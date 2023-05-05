using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using System;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Screen_based
{
    class DepleteMeter : BaseEmitter
    {
        private Color color;
        private Vector2 velocity;
        private int lifeTime;

        public DepleteMeter(Color Color, Vector2 Velocity, int LifeTime) : base(-1, 0, 0)
        {
            this.color = Color;
            velocity = Velocity;
            this.lifeTime = LifeTime;

            tileLocation = TileLocation;
            position = Offset;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("Particles/particle_square"), new Vector2(3, 8), .25f, 0, 4, trueRandom.Next(lifeTime / 5, lifeTime));
        }

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, values.respawnTime, values.spawnQuantity, () =>
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
            values.velocity = new Vector2(trueRandom.NextFloat(-30f, 30f), trueRandom.NextFloat(-30f, 30f));

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            particles[i].Color = Color.Lerp(Color.Transparent, color, (float)particles[i].TimeLeft / (float)particles[i].Lifetime);
            particles[i].Velocity += velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Angle += 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Size -= .1f * (float)gt.ElapsedGameTime.TotalSeconds;

            base.ApplyParticleBehavior(gt, i);
        }
    }
}
