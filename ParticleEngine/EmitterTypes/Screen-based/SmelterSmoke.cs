using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Screen_based
{
    public class SmelterSmoke : BaseEmitter
    {
        public SmelterSmoke() : base(-1, 0, 0)
        {
            tileLocation = TileLocation;
            position = Offset;
        }

        private Texture2D smoke1, smoke2, smoke3;

        public override void Load(ContentManager cm)
        {
            smoke1 = cm.Load<Texture2D>("Particles/Screen/smelterSmoke1");
            smoke2 = cm.Load<Texture2D>("Particles/Screen/smelterSmoke2");
            smoke3 = cm.Load<Texture2D>("Particles/Screen/smelterSmoke3");

            values = new ParticleValues(smoke1, new Vector2(16, 16), 1f, 500, 3, 1000);

            if (GameSettings.LimitParticles == true)
            {
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
            }
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
            int rand = trueRandom.Next(0, 3);

            if (rand == 0) values.Texture = smoke1;
            if (rand == 1) values.Texture = smoke2;
            if (rand == 2) values.Texture = smoke3;

            values.position = new Vector2(Position.X + (float)(trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (float)(trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));
            values.velocity.Y = (float)-trueRandom.Next(0, 25);

            values.respawnTime = trueRandom.Next(50, 200);
            values.spawnQuantity = trueRandom.Next(2, 10);

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            particles[i].Velocity.X += 7f * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Velocity.Y -= 30f * (float)gt.ElapsedGameTime.TotalSeconds;

            particles[i].Color = Color.Lerp(Color.Transparent, Color.White, (float)particles[i].TimeLeft / 1000);
            particles[i].Angle += .5f * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Size -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;

            base.ApplyParticleBehavior(gt, i);
        }

        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
