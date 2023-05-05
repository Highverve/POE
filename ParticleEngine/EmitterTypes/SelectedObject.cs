using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.Particles;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System.Linq;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes
{
    public class SelectedObject : BaseEmitter
    {
        public SelectedObject(int ID, int CurrentFloor, float DepthOrigin) : base(ID, CurrentFloor, DepthOrigin)
        {
            isUseTileLocation = false;
            isManualDepth = true;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(4, 4), .25f, 5, 1, 0);
        }

        private Color startColor = new Color(64, 64, 64, 255);
        float randValue = .75f;

        int index = 0;

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, 750, 180, () =>
            {
                if (index == 180)
                {
                    DualConnectedParticle particle = NextDualConnectedRecycle();
                    particle.SetRecycledVariables(null, values.Texture, Position, values.velocity, 0f,
                                                  Color.Lerp(Color.Black, Color.White, randValue),
                                                  1f, 1000, .95f, particles.LastOrDefault(), particles[particles.Count - index], 1);

                    particles.Add(particle);
                }
                else if (index == 0)
                {
                    ConnectedParticle particle = NextConnectedRecycle();
                    particle.SetRecycledVariables(null, values.Texture, Position, values.velocity, 0f,
                                                    Color.Lerp(Color.Black, Color.White, randValue),
                                                    1f, 1000, Depth, null, 1);

                    particles.Add(particle);
                }
                else
                {
                    ConnectedParticle particle = NextConnectedRecycle();
                    particle.SetRecycledVariables(null, values.Texture, Position, values.velocity, 0f,
                                                    Color.Lerp(Color.Black, Color.White, randValue),
                                                    1f, 1000, Depth, particles.LastOrDefault(), 1);

                    particles.Add(particle);
                }
                index++;

            }, true);

            index = 0;

            base.Update(gt);
        }

        private int currentAngle = -180;
        protected override void ApplyRandomValues()
        {
            randValue += trueRandom.NextFloat(-.05f, .05f);
            randValue = MathHelper.Clamp(randValue, .75f, 1f);

            currentAngle += 2;
            values.velocity = (MathHelper.ToRadians(currentAngle)).ToVector2() * new Vector2(trueRandom.NextFloat(20f, 22f), trueRandom.NextFloat(15f, 17f));

            if (currentAngle >= 180)
                currentAngle = -180;

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            particles[i].Color = Color.Lerp(Color.Transparent, particles[i].StartingColor, (float)particles[i].TimeLeft / particles[i].Lifetime);
            particles[i].Angle += 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Size -= .1f * (float)gt.ElapsedGameTime.TotalSeconds;

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            /*
            if (entities != null)
            {
                for (int j = 0; j < entities.Count; j++)
                {
                    if (entities[j].ParticleCircle.Intersects(particles[i].Circle))
                    {
                        entities[j].ParticleCircle.Repel(particles[i].Circle, .01f);
                    }
                }
            }*/

            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
