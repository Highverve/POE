using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.ParticleEngine.ParticleTypes
{
    public class HiddenItem : BaseEmitter
    {
        public HiddenItem() : base(-1, -1, 0f) { }
        public HiddenItem(int ID, int CurrentFloor, float DepthOrigin, Vector2 Offset) : base(ID, CurrentFloor, DepthOrigin)
        {
            position = Offset;

            isUseTileLocation = false;
        }

        public override void Load(ContentManager cm)
        {
            values = new ParticleValues(cm.Load<Texture2D>("World/Objects/Particles/triangleFadeShape"), new Vector2(16, 16), .5f, 5, 1, 1500);

            if (GameSettings.LimitParticles == true)
            {
                values.spawnQuantity = LimitSpawnQuantity(values.spawnQuantity);
            }
        }

        private float windValue;
        public override void Update(GameTime gt)
        {
            SpawnParticles(gt, 5, 1, () =>
            {
                Particle particle = NextRecycle();
                particle.SetMainRecycledVariables(values.Texture, values.position, values.velocity, values.angle,
                                                  values.color, values.size, values.lifeTime, Depth);
                particles.Add(particle);
            }, true);

            if (weather.HasWindChanged == true)
                windValue = weather.RetrieveHorizontalWindSpeed(camera.WorldToScreen(Position).X) * 30;

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
            particles[i].Velocity.Y -= (40) * (float)gt.ElapsedGameTime.TotalSeconds;

            particles[i].Color = Color.Lerp(Color.Transparent, Color.Lerp(new Color(1f, .7f, .76f, .5f), new Color(1f, 1f, .46f, .25f), (float)particles[i].TimeLeft / 1000), (float)particles[i].TimeLeft / 1500);
            particles[i].Angle += .02f;

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
            //player.ParticleCircle.PushOut(particles[i].circle);
            //player.ParticleCircle.Repel(particles[i].circle, .005f);
            //particles[i].velocity -= (camera.Motion / 1.5f);
            base.ApplyInteractionBehavior(gt, i);
        }

        public HiddenItem Parse(string parseName, string line, ContentManager cm, string mapName, int currentLine, Action<string, MapIssue.MessageType> issue)
        {
            string[] words = line.Split(' ');
            HiddenItem obj = null;

            this.ParseName = parseName;

            if (CheckParse(line) == true)
            {
                try
                {
                    obj = new HiddenItem(int.Parse(words[1]), int.Parse(words[2]), float.Parse(words[3]), new Vector2().Parse(words[4], words[5]));
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
            suggestLines.Add("HiddenItem int ID, int CurrentFloor, float DepthOrigin, Vector2 Offset");
        }

        public override string MapOutputLine()
        {
            return ParseName + " " + ID + " " + CurrentFloor + " " + depthOrigin + " " + (int)position.X + " " + (int)position.Y;
        }
    }
}
