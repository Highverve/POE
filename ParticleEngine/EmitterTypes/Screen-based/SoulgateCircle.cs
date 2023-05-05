using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Screen_based
{
    public class SoulgateCircle : BaseEmitter
    {
        public SoulgateCircle() : base(-1, 0, 0)
        {
            tileLocation = TileLocation;
            position = Offset;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("Particles/white"), new Vector2(22, 12), .25f, 10, 3, 2000);

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
            values.position = new Vector2(Position.X + (float)(trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (float)(trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));

            values.velocity.Y = (float)trueRandom.Next(-10, 10);
            values.velocity.X = (float)trueRandom.Next(-10, 10);

            base.ApplyRandomValues();
        }
        private Color half = Color.Lerp(ColorHelper.Charcoal, Color.Transparent, .35f);
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            //particles[i].Velocity.X += 15f * (float)gt.ElapsedGameTime.TotalSeconds;
            //particles[i].Velocity.Y -= 15f * (float)gt.ElapsedGameTime.TotalSeconds;

            particles[i].Color = Color.Lerp(Color.Transparent, half, (float)particles[i].TimeLeft / 1000);
            particles[i].Angle += .5f * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Size -= .1f * (float)gt.ElapsedGameTime.TotalSeconds;

            base.ApplyParticleBehavior(gt, i);
        }

        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }
    }

}
