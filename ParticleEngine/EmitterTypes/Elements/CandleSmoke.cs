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

namespace Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Elements
{
    public class CandleSmoke : BaseEmitter
    {
        public CandleSmoke() : base(-1, -1, 0f) { }
        public CandleSmoke(int ID, int CurrentFloor, float DepthOrigin, Vector2 Position) : base(ID, CurrentFloor, DepthOrigin)
        {
            position = Position;

            isManualDepth = false;
            isUseTileLocation = false;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/white"), new Vector2(2, 2), .25f, 5, 1, 0);

            if (GameSettings.LimitParticles == true)
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
        }

        private float windValue;
        private Color startColor = new Color(64, 64, 64, 255);
        float randValue = .2f;
        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, 50, 1, () =>
            {
                ConnectedParticle particle = NextConnectedRecycle();
                particle.SetRecycledVariables(null, values.Texture, values.position, new Vector2(trueRandom.NextFloat(-5f, 0f), trueRandom.NextFloat(-20f, -25f)), 0f, Color.White,
                                              trueRandom.NextFloat(.125f, .25f), trueRandom.Next(2000, 3000), Depth, particles.LastOrDefault(), 2);

                particles.Add(particle);

                randValue += trueRandom.NextFloat(-.05f, .05f);
                randValue = MathHelper.Clamp(randValue, .2f, .4f);

                particles.Last().StartingColor = new Color(randValue, randValue, randValue, .5f);
                particles.Last().Color = particles.Last().StartingColor;
            }, true);

            if (weather.HasWindChanged == true)
                windValue = weather.RetrieveHorizontalWindSpeed(camera.WorldToScreen(Position).X) * 20;

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
            particles[i].Velocity.X -= windValue * (float)gt.ElapsedGameTime.TotalSeconds;
            particles[i].Color = Color.Lerp(Color.Transparent, particles[i].StartingColor, (float)particles[i].TimeLeft / particles[i].Lifetime);

            if (particles[i].Lifetime - particles[i].TimeLeft >= 2000)
                ((ConnectedParticle)particles[i]).LineWidth = 1;

            base.ApplyParticleBehavior(gt, i);
        }
        protected override void ApplyInteractionBehavior(GameTime gt, int i)
        {
            if (entities != null)
            {
                for (int j = 0; j < entities.Count; j++)
                {
                    if (entities[j].ParticleCircle.Intersects(particles[i].Circle))
                        entities[j].ParticleCircle.Repel(particles[i].Circle, .01f);
                }
            }

            base.ApplyInteractionBehavior(gt, i);
        }

        public CandleSmoke Parse(string parseName, string line, ContentManager cm, string mapName, int currentLine, Action<string, MapIssue.MessageType> issue)
        {
            string[] words = line.Split(' ');
            CandleSmoke obj = null;

            this.ParseName = parseName;

            if (CheckParse(line) == true)
            {
                try
                {
                    obj = new CandleSmoke(int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]), new Vector2().Parse(words[4], words[5]));
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
            suggestLines.Add("CandleSmoke int ID, int CurrentFloor, float DepthOrigin, Vector2 Offset");
        }

        public override string MapOutputLine()
        {
            return ParseName + " " + ID + " " + CurrentFloor + " " + depthOrigin + " " + position.X + " " + position.Y;
        }

        public override BaseEmitter Copy()
        {
            EmberPileSmoke copy = (EmberPileSmoke)base.Copy();

            return copy;
        }
    }
}
