using System;
using System.Collections.Generic;
using System.Linq;
using Pilgrimage_Of_Embers.AudioEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public struct SimpleAudio
    {
        private string name;
        public string Name { get { return name; } }

        private SoundEffect2D soundEffect;
        public SoundEffect2D SoundEffect { get { return soundEffect; } }

        private float volume;
        public float Volume { get { return volume; } }

        public SimpleAudio(string Name, SoundEffect2D SoundEffect, float Volume)
        {
            name = Name;
            soundEffect = SoundEffect;
            volume = Volume;
        }
    }

    public class InterfaceAudio
    {
        private List<SimpleAudio> soundList = new List<SimpleAudio>();
        public List<SimpleAudio> SoundList { get { return soundList; } }

        private Random random;
        Debugging.DebugManager debug;

        public void SetReferences(Debugging.DebugManager Debug) { debug = Debug; }
        public void LoadAudio(ContentManager cm)
        {
            AddSound(cm, "StartGame", "Interface/Game Start");

            //Old SFX!
            AddSound(cm, "ButtonClick", "Interface/Button Click UI Boop");
            AddSound(cm, "SelectQuickSlot", "Interface/Button Click UI Beep");
            AddSound(cm, "SelectQuickSlotHigher", "Interface/Button Click UI Higher");
            AddSound(cm, "ButtonTargetScroll", "Interface/Button Click UI Bop");

            AddSound(cm, "SwapItem", "Interface/Swap Item (lowered)");
            AddSound(cm, "DisposeItem", "Interface/Dispose Item (lowered)");
            AddSound(cm, "EquipItem", "Interface/Equip Item");

            AddSound(cm, "GetCoins1", "Interface/Get Coins 1");
            AddSound(cm, "GetCoins2", "Interface/Get Coins 2");
            AddSound(cm, "GetCoins3", "Interface/Get Coins 3");

            AddSound(cm, "GiftItem1", "Interface/Gift Item 1");
            AddSound(cm, "GiftItem2", "Interface/Gift Item 2");
            AddSound(cm, "GiftItem3", "Interface/Gift Item 3");

            AddSound(cm, "Invalid", "Interface/Invalid Option 2");

            //Shared
            AddSound(cm, "Button Click 1", "Interface/Shared/Button Click 1");
            AddSound(cm, "Button Click 2", "Interface/Shared/Button Click 2", .75f);
            AddSound(cm, "Button Click 3", "Interface/Shared/Button Click 3");
            AddSound(cm, "Button Click 4", "Interface/Shared/Button Click 4");
            AddSound(cm, "Button Click 5", "Interface/Shared/Button Click 5");
            AddSound(cm, "Button Click 6", "Interface/Shared/Button Click 6");
            AddSound(cm, "Button Click 7", "Interface/Shared/Button Click 7");
            AddSound(cm, "Button Click 8", "Interface/Shared/Button Click 8");
            AddSound(cm, "Button Click 9", "Interface/Shared/Button Click 9");
            AddSound(cm, "Button Click 10", "Interface/Shared/Button Click 10");
            AddSound(cm, "Button Click 11", "Interface/Shared/Button Click 11");
            AddSound(cm, "Item Select", "Interface/Shared/Item Select");


            //Low Health - HUD
            AddSound(cm, "HeartbeatMedium", "Interface/HUD/HeartbeatMedium");
            AddSound(cm, "HeartbeatHigh", "Interface/HUD/HeartbeatHigh");

            //Exhaustion - HUD
            AddSound(cm, "Breath1", "Interface/HUD/Breath1");
            AddSound(cm, "Breath2", "Interface/HUD/Breath2");
            AddSound(cm, "Breath3", "Interface/HUD/Breath3");
            AddSound(cm, "Breath4", "Interface/HUD/Breath4");
            AddSound(cm, "Breath5", "Interface/HUD/Breath5");

            //Low Magic - HUD
            AddSound(cm, "LowMagic", "Interface/HUD/LowMagic");

            //Smelting
            AddSound(cm, "SmelterLoop", "Interface/Smelting/Smelter Loop", 1f, true);
            AddSound(cm, "SmelterAddOre", "Interface/Smelting/Smelter Ore Add");
            AddSound(cm, "Bellows", "Interface/Smelting/Bellows");

            //Brewing
            AddSound(cm, "BrewingLoop", "Interface/Brewing/Brewing Loop", 1f, true);


            random = new Random(Guid.NewGuid().GetHashCode());
        }
        private void AddSound(ContentManager cm, string name, string soundDirectory, float volume = 1f, bool isLooping = false)
        {
            soundList.Add(new SimpleAudio(name, new SoundEffect2D(cm.Load<SoundEffect>("Audio/Effects/" + soundDirectory), 1000f), volume));
            soundList.Last().SoundEffect.SetLoopValue(isLooping);
        }

        public void PlaySound(string soundName)
        {
            bool audioFound = false;

            for (int i = 0; i < soundList.Count; i++)
            {
                if (soundName.ToUpper().Equals(soundList[i].Name.ToUpper()))
                {
                    soundList[i].SoundEffect.SetVolume(soundList[i].Volume * GameSettings.SoundVolume * GameSettings.MasterVolume);
                    soundList[i].SoundEffect.PlaySound();

                    audioFound = true;
                    break;
                }
            }

            if (audioFound == false)
            {
                Logger.AppendLine("WARNING: No sound named \"" + soundName + "\" was found. Check for mispellings or if the sound file has been added properly.");
            }
        }
        public void PlayRandom(params string[] soundNames)
        {
            string value = soundNames[random.Next(0, soundNames.Length)];
            PlaySound(value);
        }

        public void StopSound(string soundName)
        {
            bool audioFound = false;

            for (int i = 0; i < soundList.Count; i++)
            {
                if (soundName.ToUpper().Equals(soundList[i].Name.ToUpper()))
                {
                    soundList[i].SoundEffect.StopSoundAbruptly();
                    audioFound = true;

                    break;
                }
            }

            if (audioFound == false)
            {
                Logger.AppendLine("WARNING: No sound named \"" + soundName + "\" was found. Check for mispellings or if the sound file has been added properly.");
            }
        }

        public bool IsSoundPlaying(string soundName)
        {
            bool audioFound = false;

            for (int i = 0; i < soundList.Count; i++)
            {
                if (soundName.ToUpper().Equals(soundList[i].Name.ToUpper()))
                {
                    return soundList[i].SoundEffect.State() == SoundState.Playing;
                }
            }

            if (audioFound == false)
            {
                Logger.AppendLine("WARNING: No sound named \"" + soundName + "\" was found. Check for mispellings or if the sound file has been added properly.");
            }

            return false;
        }
    }
}
