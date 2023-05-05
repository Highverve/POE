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

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes
{
    public class MetalSparks : BaseEmitter
    {
        private Color baseSparkColor = Color.Yellow, sparkColor;
        private int colorOffset;

        public Vector2 SparkDirection { get; set; }
        private Vector2 finalDirection { get { return SparkDirection * 150f; } }

        public MetalSparks() : base(-1, -1, 0f) { baseSparkColor = Color.Lerp(Color.Yellow, Color.Beige, .5f); colorOffset = 50; }
        public MetalSparks(Color SparkColor, int ColorOffset) : base(-1, -1, 0f)
        {
            baseSparkColor = SparkColor;
            colorOffset = ColorOffset;
        }
        public MetalSparks(int ID, int CurrentFloor, float DepthOrigin, Point TileLocation, Vector2 Offset) : base(ID, CurrentFloor, DepthOrigin)
        {
            tileLocation = TileLocation;
            position = Offset;

            isManualDepth = false;
        }

        private Texture2D sparkEnd;
        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(8, 8), .25f, 5, 1, 0);
            sparkEnd = cm.Load<Texture2D>("World/Objects/Particles/triangleShape");

            if (GameSettings.LimitParticles == true)
            {
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
            }
        }

        private TrailLine[] lines;

        public override void Update(GameTime gt)
        {
            //Vector2 dir = controls.MouseVector - GameSettings.VectorCenter;

            //if (dir != Vector2.Zero)
            //    dir.Normalize();

            //SparkDirection = dir;

            SpawnParticles(gt, 10, 10, () =>
            {
                sparkColor = new Color(baseSparkColor.R + trueRandom.Next(-colorOffset, colorOffset),
                                       baseSparkColor.G + trueRandom.Next(-colorOffset, colorOffset),
                                       baseSparkColor.B + trueRandom.Next(-colorOffset, colorOffset),
                                       baseSparkColor.A + trueRandom.Next(-colorOffset, colorOffset));

                lines = new TrailLine[4];

                lines[0] = new TrailLine(Color.Transparent, 25f, 1);
                lines[1] = new TrailLine(sparkColor, 35f, 1);
                lines[2] = new TrailLine(sparkColor, 35f, 1);
                lines[3] = new TrailLine(sparkColor, 35f, 1);

                TrailParticle particle = NextTrailRecycle();
                particle.SetRecycledVariables(null, values.Texture, values.position,
                                                new Vector2(trueRandom.NextFloat(-200f + finalDirection.X, 200f + finalDirection.X),
                                                            trueRandom.NextFloat(-200f + finalDirection.Y, 200f + finalDirection.Y)),
                                                0f, sparkColor, trueRandom.NextFloat(.25f, .5f), trueRandom.Next(250, 500), Depth, lines, false);
                particles.Add(particle);
            }, true);

            base.Update(gt);
        }

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            //particles[i].Position = Offset + new Vector2(i, i);
            particles[i].Velocity += new Vector2(trueRandom.NextFloat(-1000f, 1000f), trueRandom.NextFloat(-1000f, 1000f)) * (float)gt.ElapsedGameTime.TotalSeconds;

            if (particles[i].TimeLeft <= 100)
            {
                particles[i].Velocity = Vector2.Zero;
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
                        entities[j].ParticleCircle.Repel(particles[i].Circle, .05f);
                    }
                }
            }

            base.ApplyInteractionBehavior(gt, i);
        }

        public override BaseEmitter Copy()
        {
            MetalSparks copy = (MetalSparks)MemberwiseClone();

            copy.isManualDepth = true;

            return copy;
        }
    }
}
