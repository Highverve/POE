using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.ScreenEngine;

namespace Pilgrimage_Of_Embers.AudioEngine
{
    public class WindAudio : GameObject
    {
        private SoundEffect2D sfx;

        private float windSpeed, targetSpeed;
        private float maxVolumeMultiplier, pitchMultiplier, pitchSubtractor;

        public WindAudio() : base() { }
        public WindAudio(int ID, int CurrentFloor, Vector2 Position, SoundEffect Sfx, float Radius, float MaxVolumeMultiplier, float PitchMultiplier, float PitchSubtractor)
            : base(ID, CurrentFloor, 0f)
        {
            sfx = new SoundEffect2D(Sfx, Radius);
            sfx.SetLoopValue(true);
            sfx.PlaySound();

            sfx.SetVolume(1f);

            position = Position;

            maxVolumeMultiplier = MaxVolumeMultiplier;
            pitchMultiplier = PitchMultiplier;
            pitchSubtractor = PitchSubtractor;

            isUseTileLocation = false;
            isSaveType = false;
        }

        public override void SetReferences(TileMap map, Camera camera, ScreenManager screens, PlayerEntity player, WeatherManager weather, CultureManager culture, BaseEntity controlledEntity, List<BaseEntity> entities)
        {
            base.SetReferences(map, camera, screens, player, weather, culture, controlledEntity, entities);

            sfx.SetReferences(camera);
        }

        public void Stop()
        {
            sfx.StopSoundAbruptly();
        }

        public override void Update(GameTime gt)
        {
            if (weather.HasWindChanged == true)
                targetSpeed = Math.Abs(weather.RetrieveHorizontalWindSpeed(camera.WorldToScreen(Position).X));

            windSpeed += ((targetSpeed - windSpeed) * 2) * (float)gt.ElapsedGameTime.TotalSeconds;
            float windSpeedToPct = windSpeed / weather.MaxHorizontalWindSpeed();

            sfx.SetMaxVolume(windSpeedToPct * maxVolumeMultiplier);
            sfx.SetPitch((windSpeedToPct * pitchMultiplier) - pitchSubtractor, false);

            sfx.Update(gt, Position);

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
        public override void DrawUI(SpriteBatch sb)
        {
            base.DrawUI(sb);
        }

        public override void SelectObject(GameTime gt)
        {
            //ArrowOffset = 64f;
            //screens.ACTIVATEBOX_SetLines("Title", "(Press \"" + controls.CurrentControls.Activate.ToString() + " key)");
            base.SelectObject(gt);
        }
        public override void UseObject(BaseEntity entity)
        {
            base.UseObject(entity);
        }

        #region Parse-related
        protected override GameObject GetParseObject(string line, string[] words, ContentManager cm)
        {
            return new WindAudio(int.Parse(NextWord(words)), int.Parse(NextWord(words)), new Vector2(float.Parse(NextWord(words)), float.Parse(NextWord(words))),
                                 cm.Load<SoundEffect>(NextWord(words)), float.Parse(NextWord(words)), float.Parse(NextWord(words)), float.Parse(NextWord(words)),
                                 float.Parse(NextWord(words)));
        }

        public override void SetDisplayVariables()
        {
            displayVariables.AppendLine("float <VolumeMultiplier> (" + maxVolumeMultiplier + ")");
            displayVariables.AppendLine("float <PitchMultiplier> (" + pitchMultiplier + ")");
            displayVariables.AppendLine("float <PitchSubtractor> (" + pitchSubtractor + ")");
            displayVariables.AppendLine("float <Radius> (" + sfx.Radius + ")");

            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            try
            {
                if (line.ToUpper().StartsWith("VOLUMEMULTIPLIER"))
                    maxVolumeMultiplier = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("PITCHMULTIPLIER"))
                    pitchMultiplier = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("PITCHSUBTRACTOR"))
                    pitchSubtractor = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("RADIUS"))
                    sfx.Radius = float.Parse(words[1]);
            }
            catch { }

            base.ParseEdit(line, words);
        }
        public override string RetrieveVariable(string name)
        {
            if (name.ToUpper().StartsWith("UPPERCASE"))
                return string.Empty;

            return base.RetrieveVariable(name);
        }

        public override void InitializeSuggestLine()
        {
            AddSuggest(TileEngine.Map.Editor.AutoSuggestionObject.ObjectType.Audio, "WindAudio int ID, int CurrentFloor, Vector2 Position, SoundEffect Sfx, float Radius, float MaxVolumeMultiplier, float PitchMultiplier, float PitchSubtractor");
            base.InitializeSuggestLine();
        }
        public override string MapOutputLine()
        {
            return string.Join(" ", ParseName, id.ToString(), CurrentFloor.ToString(), position.X.ToString(), position.Y.ToString(), sfx.AssetName(), sfx.Radius.ToString(),
                                    maxVolumeMultiplier.ToString(), pitchMultiplier.ToString(), pitchSubtractor.ToString());
        }
        #endregion
    }
}
