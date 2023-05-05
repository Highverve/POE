using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Pilgrimage_Of_Embers.ParticleEngine.ParticleTypes
{
    public class FlameThrower : BaseEmitter
    {
        float multiplier;
        Vector2 tempVel;

        public FlameThrower(int ID, Point TileLocation, Vector2 Offset, Vector2 Velocity, float DepthOffset, int CurrentFloor) : base(ID, CurrentFloor, DepthOffset)
        {
            tileLocation = TileLocation;
            position = Offset;
            tempVel = Velocity * 2f;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/triangleFadeShape"), new Vector2(8, 8), .25f, 0, 5, 0);
        }

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, values);//new Particle(values.texture, values.position, values.velocity, values.angle, values.color, values.size, values.lifeTime, Depth));
            base.Update(gt);
        }

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (float)(trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (float)(trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));

            values.size = (float)trueRandom.NextDouble() * .25f;
            values.lifeTime = 50 + trueRandom.Next(100);

            values.angle = (2f * (float)trueRandom.NextDouble());

            //if ((float)Math.Abs(tempVel.X) > (float)Math.Abs(tempVel.Y))
            //    values.velocity = new Vector2(tempVel.X, tempVel.Y * (tempVel.Y * (float)random.NextDouble()));
            //else if ((float)Math.Abs(tempVel.X) < (float)Math.Abs(tempVel.Y))
            //    values.velocity = new Vector2(tempVel.X * (tempVel.Y * (float)random.NextDouble()), tempVel.Y);

            multiplier = (float)trueRandom.NextDouble() / 1f;
            values.color = new Color(multiplier, multiplier - .2f, 0f, multiplier - .4f);

            //values.position = new Vector2((tileLocation.X * TileMap.TileWidth) + camera.Position.X + (float)(random.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2) + offset.X,
            //                              (tileLocation.Y * TileMap.TileHeight) + camera.Position.Y + (float)(random.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2) + offset.Y);


            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            particles[i].Circle.Position -= new Vector2(0, 50) * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Size += (1f * (float)trueRandom.NextDouble()) * (float)gt.ElapsedGameTime.TotalSeconds;

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            for (int j = 0; j < entities.Count; j++)
            {
                if (entities[j].ParticleCircle.Intersects(particles[i].Circle))
                {
                    particles[i].TimeLeft = 0;
                }
            }
            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
