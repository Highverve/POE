using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using System;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes
{
    public class JumpCircle : BaseEmitter
    {
        private Color color;
        public float Velocity { get; set; }
        private int lifeTime;

        public JumpCircle(Color Color, float Velocity, int LifeTime) : base(-1, 0, 0)
        {
            this.color = Color;
            this.Velocity = Velocity;
            this.lifeTime = LifeTime;

            tileLocation = TileLocation;
            position = Offset;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(1, 1), .25f, 0, 360, lifeTime);
        }

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, values); //new Particle(values.texture, values.position, values.velocity, values.angle, values.color, values.size, values.lifeTime, Depth));

            base.Update(gt);
        }

        private int currentAngle = -180;

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (float)(trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (float)(trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));
            values.color = color;

            currentAngle = trueRandom.Next(-180, 180);

            values.velocity = (MathHelper.ToRadians(currentAngle)).ToVector2() * trueRandom.NextFloat(Velocity * .75f, Velocity);
            values.velocity.Y *= .75f;

            values.lifeTime = trueRandom.Next((int)(lifeTime * .75f), lifeTime);

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            //particles[i].Color = Color.Lerp(Color.Transparent, color, particles[i].TimeLeft / (float)particles[i].Lifetime);
            particles[i].Size -= .1f * (float)gt.ElapsedGameTime.TotalSeconds;

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            if (entities != null)
            {
                for (int j = 0; j < entities.Count; j++)
                {
                    if (entities[j].ParticleCircle.Intersects(particles[i].Circle))
                        entities[j].ParticleCircle.Repel(particles[i].Circle, .035f);
                }
            }

            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
