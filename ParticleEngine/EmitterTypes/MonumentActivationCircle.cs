using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes
{
    public class MonumentActivationCircle : BaseEmitter
    {
        private Color color;
        private float velocity;
        private int lifeTime;

        public MonumentActivationCircle(Color Color, float Velocity, int LifeTime) : base(-1, 0, 0)
        {
            color = Color;
            velocity = Velocity;
            lifeTime = LifeTime;

            tileLocation = TileLocation;
            position = Offset;

            trueRandom = new Random((tileLocation.X + tileLocation.Y) * ((int)position.X + (int)position.Y));
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(1, 1), .25f, 50, 360, lifeTime);
        }

        int index = 0;
        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, 50, 360, () =>
            {
                index++;

                if (index != 360)
                {
                    ConnectedParticle particle = NextConnectedRecycle();
                    particle.SetRecycledVariables(null, values.Texture, values.position, values.velocity, 0f,
                                                  color, 1f, lifeTime, .95f, particles.LastOrDefault(), 2);

                    particles.Add(particle);
                }
                else
                {
                    DualConnectedParticle particle = NextDualConnectedRecycle();
                    particle.SetRecycledVariables(null, values.Texture, values.position, values.velocity, 0f, color, 1f, lifeTime,
                                                  .95f, particles.LastOrDefault(), particles.FirstOrDefault(), 2);

                    particles.Add(particle);
                }

            }, false);

            index = 0;

            base.Update(gt);
        }

        private int currentAngle = -180;

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));
            currentAngle++;

            values.velocity = (MathHelper.ToRadians(currentAngle)).ToVector2() * new Vector2(trueRandom.NextFloat(velocity * .9f, velocity), trueRandom.NextFloat(velocity * .6f, velocity * .75f));

            if (currentAngle >= 180)
                currentAngle = -180;

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            particles[i].Color = Color.Lerp(Color.Transparent, color, (float)particles[i].TimeLeft / (float)particles[i].Lifetime);
            particles[i].Angle += 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Size -= .1f * (float)gt.ElapsedGameTime.TotalSeconds;

            base.ApplyParticleBehavior(gt, i);
        }
    }
}
