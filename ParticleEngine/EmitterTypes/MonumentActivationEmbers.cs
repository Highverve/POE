using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.Particles;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.TileEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes
{
    class MonumentActivationEmbers : BaseEmitter
    {
        public int SpawnQuantity { get; set; }
        public int SpawnTime { get; set; }

        public MonumentActivationEmbers() : base(-1, -1, 0f) { }
        public MonumentActivationEmbers(int ID, int CurrentFloor, float DepthOffset) : base(ID, CurrentFloor, DepthOffset)
        {
            tileLocation = TileLocation;
            position = Offset;

            isManualDepth = false;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(16, 16), .25f, 5, 1, 0);

            if (GameSettings.LimitParticles == true)
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);

            values.spawnArea = new Vector2(36, 100);
        }

        private TrailLine[] lines;
        private Color baseSparkColor = Color.Orange, sparkColor;
        private int colorOffset = 50;

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, SpawnTime, SpawnQuantity, () =>
            {
                sparkColor = new Color(baseSparkColor.R + trueRandom.Next(-colorOffset, colorOffset),
                       baseSparkColor.G + trueRandom.Next(-colorOffset, colorOffset),
                       baseSparkColor.B + trueRandom.Next(-colorOffset, colorOffset),
                       baseSparkColor.A + trueRandom.Next(-colorOffset, colorOffset));

                lines = new TrailLine[4];

                lines[0] = new TrailLine(Color.Transparent, 25f, 1);
                lines[1] = new TrailLine(Color.Lerp(sparkColor, Color.Transparent, .25f), 10f, 1);
                lines[2] = new TrailLine(Color.Lerp(sparkColor, Color.Transparent, .5f), 10f, 1);
                lines[3] = new TrailLine(Color.Lerp(sparkColor, Color.Transparent, .75f), 10f, 1);

                TrailParticle particle = NextTrailRecycle();
                particle.SetRecycledVariables(null, values.Texture, values.position, new Vector2(trueRandom.NextFloat(-60f, 60f), trueRandom.NextFloat(-60f, 40f)),
                                                 0f, Color.Orange, trueRandom.NextFloat(.125f, .25f), trueRandom.Next(1000, 2000), Depth, lines);

                particles.Add(particle);

            }, true); //new Particle(values.texture, values.position, values.velocity, values.angle, values.color, values.size, values.lifeTime, Depth));

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
            particles[i].Velocity += new Vector2(0, trueRandom.NextFloat(-800f, 830f)) * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Color = Color.Lerp(Color.Transparent, particles[i].StartingColor, (float)particles[i].TimeLeft / particles[i].Lifetime);

            if (particles[i].TimeLeft <= 500 || particles[i].Position.Y >= Position.Y + 90)
            {
                particles[i].Velocity = Vector2.Zero;
            }

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }

        public MonumentActivationEmbers Parse(string parseName, string line, ContentManager cm, string mapName, int currentLine, Action<string, MapIssue.MessageType> issue)
        {
            string[] words = line.Split(' ');
            MonumentActivationEmbers obj = null;

            this.ParseName = parseName;

            if (CheckParse(line) == true)
            {
                try
                {
                    obj = new MonumentActivationEmbers(int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]));
                    obj.ParseName = parseName;
                }
                catch (Exception e)
                {
                    issue.Invoke(e.Message + "(" + parseName + ")", MapIssue.MessageType.Error);
                }
            }

            return obj;
        }

        public override void SetDisplayVariables()
        {

            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            try
            {
            }
            catch
            {
            }

            base.ParseEdit(line, words);
        }

        public override void InitializeSuggestLine()
        {
            objectType = TileEngine.Map.Editor.AutoSuggestionObject.ObjectType.Emitters;
        }

        public override string MapOutputLine()
        {
            return ParseName + " " + ID + " " + CurrentFloor + " " + depthOrigin + " " + tileLocation.X + " " + tileLocation.Y + " " + (int)position.X + " " + (int)position.Y;
        }

        public override BaseEmitter Copy()
        {
            EmberPileSparks copy = (EmberPileSparks)base.Copy();

            return copy;
        }
    }
}
