using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Audio;

namespace Pilgrimage_Of_Embers.AudioEngine
{
    public class MusicTrack
    {
        private int id;
        public int ID { get { return id; } }

        private SoundEffectInstance track;
        public SoundEffectInstance Track { get { return track; } }

        public float BaseVolume { get; private set; }
        public float Volume { get { return track.Volume; } set { track.Volume = MathHelper.Clamp(value, 0f, 1f); } }

        public bool IsActive { get; set; }

        public MusicTrack(int ID, SoundEffect Track, float Volume)
        {
            id = ID;
            track = Track.CreateInstance();
            BaseVolume = Volume;

            track.IsLooped = true;
            track.Volume = 0f;
            IsActive = false;
        }

        public void Stop()
        {
            track.Stop();
        }
        public void Play()
        {
            track.Play();
        }
    }

    public class MusicManager
    {
        private List<MusicTrack> tracks;

        public int SongID()
        {
            if (CurrentTrack() != null)
                return CurrentTrack().ID;
            else
                return -1;
        }

        public MusicManager()
        {
            tracks = MusicDatabase.Tracks.ToList();
        }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                if (tracks[i].IsActive == true)
                {
                    if (delayCount <= 0)
                    {
                        if (tracks[i].Track.State == SoundState.Stopped)
                            tracks[i].Play();

                        tracks[i].Volume += .05f * (float)gt.ElapsedGameTime.TotalSeconds;
                        tracks[i].Volume = MathHelper.Clamp(tracks[i].Volume, 0f, tracks[i].BaseVolume * (GameSettings.MusicVolume * GameSettings.MasterVolume));
                    }
                    else
                        delayCount -= gt.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    if (tracks[i].Volume <= 0f && tracks[i].Track.State == SoundState.Playing)
                        tracks[i].Stop();
                    else
                    {
                        tracks[i].Volume -= .05f * (float)gt.ElapsedGameTime.TotalSeconds;
                        tracks[i].Volume = MathHelper.Clamp(tracks[i].Volume, 0f, 1f);
                    }
                }
            }
        }

        private int delayCount = 1000;

        public void DeactivateAll()
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                tracks[i].IsActive = false;
            }
        }
        public void Play(int id, int delay)
        {
            if (id != -1)
            {
                for (int i = 0; i < tracks.Count; i++)
                {
                    if (tracks[i].ID == id)
                    {
                        tracks[i].IsActive = true;
                        delayCount = delay;
                    }
                }
            }
        }
        public MusicTrack CurrentTrack()
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                if (tracks[i].IsActive == true)
                {
                    return tracks[i];
                }
            }

            return null;
        }
    }
}
