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
    class EmberPileSparks : BaseEmitter
    {
        public EmberPileSparks() : base(-1, -1, 0f) { }
        public EmberPileSparks(int ID, int CurrentFloor, float DepthOrigin, Point TileLocation, Vector2 Offset) : base(ID, CurrentFloor, DepthOrigin)
        {
            tileLocation = TileLocation;
            position = Offset;

            isManualDepth = false;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(16, 16), .25f, 5, 1, 0);

            if (GameSettings.LimitParticles == true)
            {
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
            }
        }

        private Color startColor = new Color(64, 64, 64, 255);
        int randomTime = 500;
        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, randomTime, 5, () =>
            {
                Particle particle = NextRecycle();
                particle.SetMainRecycledVariables(values.Texture, values.position, new Vector2(trueRandom.NextFloat(-30f, 30f), trueRandom.NextFloat(-30f, 30f)),
                                                  0f, Color.Orange, trueRandom.NextFloat(.125f, .25f), trueRandom.Next(500, 1000), Depth);

                particles.Add(particle);

                randomTime = trueRandom.Next(150, 750);
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
            particles[i].Velocity += new Vector2(0, trueRandom.NextFloat(-200f, 220f)) * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Color = Color.Lerp(Color.Transparent, particles[i].StartingColor, (float)particles[i].TimeLeft / particles[i].Lifetime);

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }

        public EmberPileSparks Parse(string parseName, string line, ContentManager cm, string mapName, int currentLine, Action<string, MapIssue.MessageType> issue)
        {
            string[] words = line.Split(' ');
            EmberPileSparks obj = null;

            this.ParseName = parseName;

            if (CheckParse(line) == true)
            {
                try
                {
                    obj = new EmberPileSparks(int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]), new Point().Parse(words[4], words[5]), new Vector2().Parse(words[6], words[7]));
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
