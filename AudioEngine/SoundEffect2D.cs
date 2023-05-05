using Microsoft.Xna.Framework.Audio;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;

namespace Pilgrimage_Of_Embers.AudioEngine
{
    public class SoundEffect2D
    {
        private SoundEffect effect;
        private SoundEffectInstance effectInstance;

        private float maxVolume;

        private Circle emitterCircle;

        private bool isLooped = false;

        public string AssetName() { return effect.Name; }
        public float Radius
        {
            get { return emitterCircle.radius; }
            set { emitterCircle.radius = value; }
        }

        private Camera camera;

        public SoundEffect2D(SoundEffect Effect, float Radius)
        {   
            effect = Effect;
            effectInstance = effect.CreateInstance();

            maxVolume = 1f;
            effectInstance.Volume = 0f;
            effectInstance.Pan = 0f;

            emitterCircle = new Circle(Radius);
        }
        public void SetReferences(Camera camera)
        {
            this.camera = camera;
        }

        public void PlaySound()
        {
            effectInstance.Play();
        }

        public void PauseEffect()
        {
            effectInstance.Pause();
        }
        public void ResumeEffect()
        {
            effectInstance.Resume();
        }

        public void StopSoundAbruptly()
        {
            effectInstance.Stop();
        }
        public void StopSoundFade()
        {

        }

        public void ManipulateVolume(float change)
        {
            effectInstance.Volume = MathHelper.Clamp(effectInstance.Volume + change, 0f, 1f); ;
        }
        public void ManipulatePan(float change)
        {
            float pan = MathHelper.Clamp(effectInstance.Pan + change, 0f, 1f);
            effectInstance.Pan = pan;
        }
        public void ManipulatePitch(float change)
        {
            float pitch = MathHelper.Clamp(effectInstance.Pitch + change, 0f, 1f);
            effectInstance.Pitch = pitch;
        }

        public void SetVolume(float value)
        {
            effectInstance.Volume = MathHelper.Clamp(value, 0f, 1f); ;
        }
        public void SetPan(float value)
        {
            effectInstance.Pan = MathHelper.Clamp(value, -1, 1);
        }
        public void SetPitch(float value, bool isQueueing = true)
        {
            effectInstance.Pitch = MathHelper.Clamp(value, -1, 1);
        }

        public void SetMaxVolume(float value)
        {
            maxVolume = MathHelper.Clamp(value, 0f, 1f);
        }
        public void SetRadius(float radius)
        {
            emitterCircle.radius = radius;
        }

        public float GetVolume()
        {
            return effectInstance.Volume;
        }
        public float GetPan()
        {
            return effectInstance.Pan;
        }
        public float GetPitch()
        {
            return effectInstance.Pitch;
        }

        public void Update(GameTime gt, Vector2 emitterLocation)
        {
            //Update sound position ...
            emitterCircle.Position = emitterLocation;

            if (State() == SoundState.Playing)
            {
                if (emitterCircle.Intersects(camera.ListenerPosition, camera.ListenerRadius))
                {
                    //Get intersection distance ...
                    Vector2 intersectionDistance = emitterCircle.VectorDistance(camera.ListenerPosition, camera.ListenerRadius);
                    Vector2 multiplier = new Vector2(Math.Abs(intersectionDistance.X / (camera.ListenerRadius + emitterCircle.radius)),
                                                     Math.Abs(intersectionDistance.Y / (camera.ListenerRadius + emitterCircle.radius))) * -1;

                    float volume = Math.Abs((emitterCircle.Distance(camera.ListenerPosition, camera.ListenerRadius) / (emitterCircle.radius + camera.ListenerRadius)));

                    SetVolume((volume * maxVolume) * GameSettings.SoundVolume);
                    SetPan((camera.ListenerPosition.X - emitterCircle.Position.X) / (camera.ListenerRadius + emitterCircle.radius));

                    //Debugging.DebugManager.info.SetVariable("Volume: " + GetVolume(), 1);
                    Debugging.DebugManager.info.SetVariable("Pan: " + GetPan(), 2);
                    //Debugging.DebugManager.info.SetVariable("Pitch: " + GetPitch(), 3);
                }
                else
                    ManipulateVolume(-.01f * (float)gt.ElapsedGameTime.TotalSeconds);
            }
        }

        public void SetLoopValue(bool looped)
        {
            isLooped = looped;
            effectInstance.IsLooped = isLooped;
        }

        /// <summary>
        /// In milliseconds
        /// </summary>
        /// <returns></returns>
        public int Duration
        {
            get
            {
                return (int)effect.Duration.TotalMilliseconds;
            }
        }
        public bool IsCompleted()
        {
            return effectInstance.State == SoundState.Stopped;
        }
        public SoundState State()
        {
            return effectInstance.State;
        }
    }
}
