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
    public class BloodDrip : BaseEmitter
    {
        public BloodDrip() : base(-1, -1, 0f) { }
        public BloodDrip(int ID, int CurrentFloor, float DepthOrigin, Point TileLocation, Vector2 Offset) : base(ID, CurrentFloor, DepthOrigin)
        {
            tileLocation = TileLocation;
            position = Offset;

            isManualDepth = false;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(8, 8), .25f, 5, 1, 1500);

            if (GameSettings.LimitParticles == true)
            {
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
            }
        }

        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, trueRandom.Next(500, 750), 2, () =>
            {
                particles.Add(new FloorParticle(values.Texture, values.position, new Vector2(trueRandom.NextFloat(-10f, 10f), trueRandom.NextFloat(-50f, 0f)), 0f, startColor, .2f, trueRandom.Next(7500, 10000), Depth, 50));
            }, true); //new Particle(values.texture, values.position, values.velocity, values.angle, values.color, values.size, values.lifeTime, Depth));

            base.Update(gt);
        }

        private Color startColor = Color.Lerp(Color.Red, Color.Transparent, .5f);

        protected override void ApplyRandomValues()
        {
            values.position = new Vector2(Position.X + (trueRandom.Next(1, (int)values.spawnArea.X)) - (values.spawnArea.X / 2),
                                          Position.Y + (trueRandom.Next(1, (int)values.spawnArea.Y)) - (values.spawnArea.Y / 2));

            base.ApplyRandomValues();
        }
        protected override void ApplyParticleBehavior(GameTime gt, int i)
        {
            if (((FloorParticle)particles[i]).Position.Y > ((FloorParticle)particles[i]).PixelFloor)
            {
                if (((FloorParticle)particles[i]).BounceCount == 0)
                {
                    particles[i].Velocity.X = trueRandom.NextFloat(-5f, 5f); //Reset velocity when the ground is hit
                    particles[i].Velocity.Y = trueRandom.NextFloat(-15f, 15f);

                    ((FloorParticle)particles[i]).BounceCount++;
                }

                particles[i].Size += .25f * (float)gt.ElapsedGameTime.TotalSeconds;
                particles[i].Color = Color.Lerp(Color.Transparent, startColor, (float)particles[i].TimeLeft / particles[i].Lifetime);

                if (particles[i].Color.A <= 0) //Force kill
                    particles[i].TimeLeft = 0;
            }
            else
            {
                particles[i].Velocity.Y += 400f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            base.ApplyInteractionBehavior(gt, i);
        }

        public BloodDrip Parse(string parseName, string line, ContentManager cm, string mapName, int currentLine, Action<string, MapIssue.MessageType> issue)
        {
            string[] words = line.Split(' ');
            BloodDrip obj = null;

            this.ParseName = parseName;

            if (CheckParse(line) == true)
            {
                try
                {
                    obj = new BloodDrip(int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]), new Point().Parse(words[4], words[5]), new Vector2().Parse(words[6], words[7]));
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
            catch (Exception e)
            {
            }

            base.ParseEdit(line, words);
        }

        public override void InitializeSuggestLine()
        {
            objectType = TileEngine.Map.Editor.AutoSuggestionObject.ObjectType.Emitters;
            suggestLines.Add("BloodDrip int ID, int CurrentFloor, float DepthOrigin, Point TileLocation, Vector2 Offset");
        }

        public override string MapOutputLine()
        {
            return ParseName + " " + ID + " " + CurrentFloor + " " + depthOrigin + " " + tileLocation.X + " " + tileLocation.Y + " " + (int)position.X + " " + (int)position.Y;
        }

    }
}
