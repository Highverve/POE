using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Screen_based
{
    public class LogoEmbers : BaseEmitter
    {
        public LogoEmbers() : base(-1, 0, 0)
        {
            position = new Vector2(0, GameSettings.VectorResolution.Y);
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("Particles/white"), new Vector2(100, 0), .2f, 0, 10, 3000);

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
            float degree = MathHelper.ToRadians(trueRandom.Next(-180, 180));
            values.position = Position + (degree.ToVector2() * trueRandom.NextFloat(50, values.spawnArea.X));

            values.lifeTime = trueRandom.Next(3000, 4000);

            values.velocity = degree.ToVector2() * trueRandom.NextFloat(-60, 60);
            //values.velocity.X = random.NextFloat(-1f, -3f);
            //values.velocity.Y = random.NextFloat(-1f, -3f);

            base.ApplyRandomValues();
        }

        private Color orangeHalf = Color.Lerp(Color.Transparent, Color.Orange, .75f);
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            particles[i].Color = Color.Lerp(Color.Transparent, orangeHalf, (particles[i].Size / .1f));
            particles[i].Size -= .03f * (float)gt.ElapsedGameTime.TotalSeconds;

            particles[i].Velocity += new Vector2(0, trueRandom.NextFloat(-200f, 220f)) * (float)gt.ElapsedGameTime.TotalSeconds;

            if (particles[i].Size <= 0f)
                particles[i].TimeLeft = 0;

            if (IsActivated == false)
                particles[i].TimeLeft -= gt.ElapsedGameTime.Milliseconds * 3;

            base.ApplyParticleBehavior(gt, i);
        }

        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
