using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.Particles;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Flora_based
{
    public class TreeLeaves : BaseEmitter
    {
        private List<Tuple<Texture2D, float>> particleTextureChance;
        private Vector2 spawnArea;
        private int minSpawnTime, maxSpawnTime;
        public int FloorHeight { get; set; }

        public TreeLeaves() : base(-1, -1, 0f) { }
        public TreeLeaves(int ID, int CurrentFloor, float DepthOrigin, Vector2 SpawnArea, int MinSpawnTime, int MaxSpawnTime, List<Tuple<Texture2D, float>> ParticleTextureChance) : base(ID, CurrentFloor, DepthOrigin)
        {
            position = Offset;

            particleTextureChance = ParticleTextureChance;
            spawnArea = SpawnArea;

            minSpawnTime = MinSpawnTime;
            maxSpawnTime = MaxSpawnTime;

            isUseTileLocation = false;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(null, new Vector2(16, 16), 2f, 5, 1, 5000);

            if (GameSettings.LimitParticles == true)
            {
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
            }
        }

        private float windValue;
        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, values.respawnTime, 1, () =>
            {
                FloorParticle particle = NextFloorRecycle();
                particle.SetRecycledVariables(PickTexture(), values.position, values.velocity, values.angle,
                                                  values.color, values.size, values.lifeTime, Depth, FloorHeight);
                particles.Add(particle);

                values.respawnTime = trueRandom.Next(minSpawnTime, maxSpawnTime);
            }, true);

            if (weather.HasWindChanged == true)
                windValue = weather.RetrieveHorizontalWindSpeed(camera.WorldToScreen(Position).X) * 30;

            base.Update(gt);
        }
        private Texture2D PickTexture()
        {
            Texture2D texture = null;

            float maxRange = 0f, currentPickRange = 0f;
            for (int i = 0; i < particleTextureChance.Count; i++)
                maxRange += particleTextureChance[i].Item2;

            float randomValue = trueRandom.NextFloat(0f, maxRange);

            for (int i = 0; i < particleTextureChance.Count; i++)
            {
                currentPickRange += particleTextureChance[i].Item2;

                if (randomValue <= currentPickRange)
                {
                    texture = particleTextureChance[i].Item1;
                    break;
                }
            }

            return texture;
        }

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (trueRandom.Next(1, (int)spawnArea.X)) - (spawnArea.X / 2),
                                          Position.Y + (trueRandom.Next(1, (int)spawnArea.Y)) - (spawnArea.Y / 2));

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            if (((FloorParticle)particles[i]).Position.Y > ((FloorParticle)particles[i]).PixelFloor)
            {
                particles[i].Velocity = Vector2.Zero;
                particles[i].Size -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;

                if (particles[i].Size <= 0f)
                    particles[i].TimeLeft = 0;
            }
            else
            {
                particles[i].Velocity.X -= windValue * (float)gt.ElapsedGameTime.TotalSeconds;
                particles[i].Velocity.Y += (80) * (float)gt.ElapsedGameTime.TotalSeconds;

                particles[i].Angle += .02f;
            }

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            if (entities != null)
            {
                for (int j = 0; j < entities.Count; j++)
                {
                    if (entities[j].ParticleCircle.Intersects(particles[i].Circle))
                    {
                        entities[j].ParticleCircle.Repel(particles[i].Circle, .075f);
                    }
                }
            }
            base.ApplyInteractionBehavior(gt, i);
        }
    }
}
