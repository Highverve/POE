using Microsoft.Xna.Framework.Audio;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.AudioEngine
{
    public class MusicLayer
    {
        private SoundEffect songLayer;
        private SoundEffectInstance songInstance;

        private float volume = 0f;
        private float pan = 0f;
        private float pitch = 0f;

        public enum FadeState
        {
            FadeIn,
            FadeOut,
            Stop
        }
        FadeState fadeState = FadeState.Stop;

        public MusicLayer(SoundEffect SongLayer)
        {
            songLayer = SongLayer;
            songInstance = songLayer.CreateInstance();

            songInstance.Volume = volume; //Maybe take away Pan/Pitch options. See how it goes.
            songInstance.Pan = pan;
            songInstance.Pitch = pitch;

            songInstance.IsLooped = true;
        }

        public void Update()
        {
            switch (fadeState)
            {
                case FadeState.FadeIn: FadeIn(); break;
                case FadeState.FadeOut: FadeOut(); break;
                case FadeState.Stop: break;
            }
        }

        public void PlaySongLayer() { songInstance.Play(); }
        public void PauseSongLayer() { songInstance.Pause(); }
        public void ResumeSongLayer() { songInstance.Resume(); }
        public void StopSongLayer() { songInstance.Stop(); }
        public void SetFadeState(FadeState fs)
        {
            fadeState = fs;
        }

        private void FadeIn() { ManipulateVolume(.03f); }
        private void FadeOut() { ManipulateVolume(-.03f); }
        private void ManipulateVolume(float change)
        {
            volume += change;

            if (volume > 1f)
            {
                volume = 1f;
                fadeState = FadeState.Stop;
            }
            else if (volume < 0f)
            {
                volume = 0f;
                fadeState = FadeState.Stop;
            }

            songInstance.Volume = volume * GameSettings.MusicVolume;
        }

        public float GetVolume() { return volume; }


    }
}
