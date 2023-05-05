using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.Particles;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Weather
{
    public class Snow : BaseEmitter
    {
        int spawnCount, spawnSpeed;

        public Snow() : base(-1, -1, 0f) { }
        public Snow(int ID, int CurrentFloor, float DepthOrigin, int SpawnCount, int SpawnSpeed) : base(ID, CurrentFloor, DepthOrigin)
        {
            tileLocation = TileLocation;
            position = Offset;

            spawnCount = SpawnCount;
            spawnSpeed = SpawnSpeed;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(GameSettings.VectorResolution.X * 2, GameSettings.VectorResolution.Y * 2), .25f, spawnSpeed, spawnCount, 20000);
            values.color = new Color(255, 255, 255, 255);

            if (GameSettings.LimitParticles == true)
                spawnCount = LimitSpawnQuantity(spawnCount);
        }

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, spawnSpeed, spawnCount, () =>
            {
                FloorParticle particle = NextFloorRecycle();
                particle.SetRecycledVariables(values.Texture, values.position, values.velocity, values.angle, values.color, values.size, values.lifeTime, 1f,
                                                trueRandom.Next(300, (int)(GameSettings.WindowResolution.Y * 2f)));

                particles.Add(particle);
            }, false);

            position = camera.Position + new Vector2(GameSettings.VectorCenter.X, GameSettings.VectorCenter.Y);

            if (weather.HasWindChanged == true)
                windSpeed = (weather.RetrieveHorizontalWindSpeed(camera.WorldToScreen(Position).X) * 50) * (float)gt.ElapsedGameTime.TotalSeconds;

            base.Update(gt);
        }

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (float)(trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (float)(trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));
            values.velocity.Y = trueRandom.NextFloat(75f, 125f);

            base.ApplyRandomValues();
        }

        private float windSpeed;
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            //particles[i].Velocity.Y += (150) * (float)gt.ElapsedGameTime.TotalSeconds;

            if (((FloorParticle)particles[i]).Position.Y > ((FloorParticle)particles[i]).PixelFloor)
            {
                if (((FloorParticle)particles[i]).BounceCount == 0) //First bounce
                {
                    ((FloorParticle)particles[i]).BounceCount++;
                    particles[i].Velocity.X = 0; //Reset velocity caused by wind when the ground is hit

                    //particles[i].SetCurrentFrame(new Point(3, 0));
                }

                particles[i].Size -= .25f * (float)gt.ElapsedGameTime.TotalSeconds;

                if (particles[i].Size <= 0f)
                    particles[i].TimeLeft = 0;
            }
            else
            {
                if (((FloorParticle)particles[i]).BounceCount < 1)
                    particles[i].Velocity.X -= windSpeed;

                particles[i].Angle += .5f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
