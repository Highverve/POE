using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Extensions;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.Particles;
using Pilgrimage_Of_Embers.Performance;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Weather
{
    public class Rain : BaseEmitter
    {
        int spawnCount, spawnSpeed;

        GPUOffload gpu;
        CallLimiter limitApply = new CallLimiter(100);

        public Rain() : base(-1, -1, 0f) { }
        public Rain(int ID, int CurrentFloor, float DepthOrigin, int SpawnCount, int SpawnSpeed) : base(ID, CurrentFloor, DepthOrigin)
        {
            tileLocation = TileLocation;
            position = Offset;

            spawnCount = SpawnCount;
            spawnSpeed = SpawnSpeed;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/rainDrop"), new Vector2(GameSettings.VectorResolution.X * 2, 500), 1f, spawnSpeed, spawnCount, 7000);
            values.color = Color.Lerp(new Color(108, 153, 216, 255), Color.Transparent, .35f);

            if (GameSettings.LimitParticles == true)
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);

            gpu = new GPUOffload(cm.Load<Effect>("GPU/Rain"));
        }

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, spawnSpeed, spawnCount, () =>
            {
                FloorParticle particle = NextFloorRecycle();
                particle.SetRecycledVariables(values.Texture, new Point(5, 20), values.position, values.velocity, values.angle, values.color,
                                              values.size, values.lifeTime, 1f, trueRandom.Next(300, (int)(GameSettings.WindowResolution.Y * 2f)));

                particles.Add(particle);
            }, false );

            position = camera.Position + new Vector2(GameSettings.VectorCenter.X, -100);

            if (weather.HasWindChanged == true)
                windSpeed = (weather.RetrieveHorizontalWindSpeed(camera.WorldToScreen(Position).X) * 200) * (float)gt.ElapsedGameTime.TotalSeconds;

            base.Update(gt);
        }

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));
            values.velocity.Y = trueRandom.NextFloat(200f, 300f);

            base.ApplyRandomValues();
        }

        private float windSpeed;
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            if (limitApply.IsCalling(gt))
            {
                particles[i].Velocity.Y += (10000) * (float)gt.ElapsedGameTime.TotalSeconds;

                if (((FloorParticle)particles[i]).Position.Y <= ((FloorParticle)particles[i]).PixelFloor)
                {
                    particles[i].Angle = particles[i].Velocity.ToAngle();//gpu.RetrieveFloat("angle");//particles[i].Velocity.ToAngle();
                }
            }

            if (((FloorParticle)particles[i]).Position.Y > ((FloorParticle)particles[i]).PixelFloor)
            {
                if (((FloorParticle)particles[i]).BounceCount == 0) //First bounce
                {
                    ((FloorParticle)particles[i]).BounceCount++;
                    particles[i].Velocity.X = 0; //Reset velocity caused by wind when the ground is hit

                    particles[i].SetCurrentFrame(new Point(3, 0));
                }

                particles[i].Size -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;

                if (particles[i].Size <= 0f)
                    particles[i].TimeLeft = 0;
            }
            else
            {
                if (((FloorParticle)particles[i]).BounceCount < 1)
                    particles[i].Velocity.X -= windSpeed;
            }

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
                        entities[j].ParticleCircle.Repel(particles[i].Circle, .05f);
                    }
                }
            }*/
            //player.ParticleCircle.PushOut(particles[i].circle);
            //player.ParticleCircle.Repel(particles[i].circle, .005f);
            //particles[i].velocity -= (camera.Motion / 1.5f);
            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
