using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Pilgrimage_Of_Embers.ScreenEngine.Options
{
    public class GameSettings
    {
        //Display
        public static Point WindowResolution { get; private set; }
        public static Point WindowCenter { get; private set; }
        public static Vector2 VectorResolution { get; private set; }
        public static Vector2 VectorCenter { get; private set; }

        private static bool isFullScreen = false;
        public static bool IsFullScreen { get { return isFullScreen; } set { isFullScreen = value; } }

        private static bool isVSync = false;
        public static bool IsVSync { get { return isVSync; } set { isVSync = value; } }

        private static bool isUseWidescreen = true;
        public static bool IsUseWidescreen { get { return isUseWidescreen; } set { isUseWidescreen = value; } }

        private static bool isHidingHUD = false;
        public static bool IsHidingHUD { get { return isHidingHUD; } set { isHidingHUD = value; } }

        private static float gamma = .5f, shakeMultiplier, hiddenNEOs; //Non-essential objects
        public static float Gamma { get { return gamma; } set { gamma = MathHelper.Clamp(value, 0, 1f); } }
        public static float ShakeMultiplier { get { return shakeMultiplier; } set { shakeMultiplier = MathHelper.Clamp(value, 0f, 1f); } }
        public static float HiddenNEOsPct { get { return hiddenNEOs; } set { hiddenNEOs = MathHelper.Clamp(value, 0f, 1f); } }

        private static bool displayFPS = true;
        public static bool DisplayFPS { get { return displayFPS; } set { displayFPS = value; } }

        private static bool displayCursor = true;
        public static bool DisplayCursor { get { return displayCursor; } set { displayCursor = value; } }

        //Particles
        private static bool showParticles = true;

        private static bool limitParticles = false;
        public static bool LimitParticles { get { return limitParticles; } set { limitParticles = value; } }

        private static float limitParticlePercentage = .25f; //Take off 75% of all particle spawn quantity, leave 25%

        private static bool particleInteraction = true;
        public static bool ParticleInteraction { get { return particleInteraction; } set { particleInteraction = value; } }

        private static bool limitShaders = false;
        public static bool LimitShaders { get { return limitShaders; } set { limitShaders = value; } }

        private static float masterVolume = 1f, soundVolume = 1f, ambienceVolume = 1f, musicVolume = 1f;
        public static float MasterVolume { get { return masterVolume; } set { masterVolume = MathHelper.Clamp(value, 0f, 1f); } }
        public static float SoundVolume { get { return soundVolume; } set { soundVolume = MathHelper.Clamp(value, 0f, 1f); } }
        public static float AmbienceVolume { get { return ambienceVolume; } set { ambienceVolume = MathHelper.Clamp(value, 0f, 1f); } }
        public static float MusicVolume { get { return musicVolume; } set { musicVolume = MathHelper.Clamp(value, 0f, 1f); } }

        private static int mapSaveSpeed = 20; //in seconds, how often the map saves (max : 20; Min : 10)
        private static int playerSaveSpeed = 10; // in seconds, how often the player is saved (max : 15)
        private static int worldSaveSpeed = 15;

        private static bool noClip = false;

        public const string FileName = "settings.data";

        private static Controls controls = new Controls();

        private static StringBuilder SaveSettings()
        {
            StringBuilder data = new StringBuilder();

            data.AppendLine("[Display]");

            data.AppendLine("Resolution " + WindowResolution.X + " " + WindowResolution.Y);
            data.AppendLine("IsFullscreen " + isFullScreen.ToString());
            data.AppendLine("IsDisplayFPS " + displayFPS.ToString());
            data.AppendLine("IsDisplayCursor " + displayCursor.ToString());
            data.AppendLine("LimitParticles " + limitParticles.ToString());
            data.AppendLine("ParticleInteraction " + particleInteraction.ToString());
            data.AppendLine("LimitShaders " + limitShaders.ToString());
            data.AppendLine("isVSync " + isVSync);
            data.AppendLine("Gamma " + gamma.ToString());
            data.AppendLine("Shake " + shakeMultiplier.ToString());
            data.AppendLine("HiddenObjects " + hiddenNEOs.ToString());

            data.AppendLine("[/Display]");

            data.AppendLine();

            data.AppendLine("[Audio]");

            data.AppendLine("MasterVolume " + masterVolume.ToString());
            data.AppendLine("SoundVolume " + soundVolume.ToString());
            data.AppendLine("AmbienceVolume " + ambienceVolume.ToString());
            data.AppendLine("MusicVolume " + musicVolume.ToString());

            data.AppendLine("[/Audio]");

            data.AppendLine();

            return data;
        }
        public static void SaveData()
        {
            if (!File.Exists(FileName)) //If the file doesn't exist, create it!
                File.Create(FileName);

            try
            {
                File.WriteAllText(FileName, SaveSettings().ToString() + controls.CurrentControls.SaveCustomControls().ToString());
            }
            catch
            {
                Logger.AppendLine("Error saving settings.data!");
            }
        }

        private static List<string> settingsData = new List<string>();
        public static void ReadData()
        {
            settingsData.Clear();

            if (File.Exists(FileName))
            {
                using (StreamReader sr = new StreamReader(FileName))
                {
                    while (!sr.EndOfStream)
                    {
                        settingsData.Add(sr.ReadLine());
                    }
                }
            }
        }
        public static void LoadSettings()
        {
            for (int i = 0; i < settingsData.Count; i++)
            {
                string[] words = settingsData[i].Split(' ');

                #region Resolution
                if (settingsData[i].ToUpper().StartsWith("RESOLUTION"))
                {
                    try
                    {
                        int x = int.Parse(words[1]);
                        int y = int.Parse(words[2]);

                        AssignResolution(x, y);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: resolution values were incorrect. Defaulting to previous value.");
                    }
                }
                #endregion
                #region Fullscreen
                if (settingsData[i].ToUpper().StartsWith("ISFULLSCREEN"))
                {
                    try
                    {
                        isFullScreen = bool.Parse(words[1]);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: isFullscreen value was incorrect. Defaulting to false.");
                        isFullScreen = false;
                    }
                }
                #endregion
                #region Display FPS
                if (settingsData[i].ToUpper().StartsWith("ISDISPLAYFPS"))
                {
                    try
                    {
                        displayFPS = bool.Parse(words[1]);
                    }
                    catch(Exception e)
                    {
                        Logger.AppendLine("Error reading settings.data: isDisplayFPS value was incorrect. Defaulting to false.");
                        displayFPS = false;
                    }
                }
                #endregion
                #region Display Cursor
                if (settingsData[i].ToUpper().StartsWith("ISDISPLAYCURSOR"))
                {
                    try
                    {
                        displayCursor = bool.Parse(words[1]);
                    }
                    catch (Exception e)
                    {
                        Logger.AppendLine("Error reading settings.data: isDisplayCursor value was incorrect. Defaulting to true.");
                        displayCursor = true;
                    }
                }
                #endregion
                #region Limit Particles
                if (settingsData[i].ToUpper().StartsWith("LIMITPARTICLES"))
                {
                    try
                    {
                        limitParticles = bool.Parse(words[1]);
                    }
                    catch (Exception e)
                    {
                        Logger.AppendLine("Error reading settings.data: limitParticles value was incorrect. Defaulting to false.");
                        limitParticles = false;
                    }
                }
                #endregion
                #region Particle Interaction
                if (settingsData[i].ToUpper().StartsWith("PARTICLEINTERACTION"))
                {
                    try
                    {
                        particleInteraction = bool.Parse(words[1]);
                    }
                    catch (Exception e)
                    {
                        Logger.AppendLine("Error reading settings.data: particleInteraction value was incorrect. Defaulting to true.");
                        particleInteraction = true;
                    }
                }
                #endregion
                #region Limit Shaders
                if (settingsData[i].ToUpper().StartsWith("LIMITSHADERS"))
                {
                    try
                    {
                        limitShaders = bool.Parse(words[1]);
                    }
                    catch (Exception e)
                    {
                        Logger.AppendLine("Error reading settings.data: limitShaders value was incorrect. Defaulting to true.");
                        limitShaders = true;
                    }
                }
                #endregion
                #region VSync
                if (settingsData[i].ToUpper().StartsWith("ISVSYNC"))
                {
                    try
                    {
                        isVSync = bool.Parse(words[1]);
                    }
                    catch (Exception e)
                    {
                        Logger.AppendLine("Error reading settings.data: isVSync value was incorrect. Defaulting to true.");
                        isVSync = true;
                    }
                }
                #endregion
                #region Gamma
                if (settingsData[i].ToUpper().StartsWith("GAMMA"))
                {
                    try
                    {
                        gamma = float.Parse(words[1]);
                    }
                    catch (Exception e)
                    {
                        Logger.AppendLine("Error reading settings.data: gamma value was incorrect. Defaulting to 50% (0.5).");
                        gamma = .5f;
                    }
                }
                #endregion
                #region Shake
                if (settingsData[i].ToUpper().StartsWith("SHAKE"))
                {
                    try
                    {
                        shakeMultiplier = float.Parse(words[1]);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: shake value was incorrect. Defaulting to 100%.");
                        shakeMultiplier = 1f;
                    }
                }
                #endregion
                #region Hidden Objects
                if (settingsData[i].ToUpper().StartsWith("HIDDENOBJECTS"))
                {
                    try
                    {
                        hiddenNEOs = float.Parse(words[1]);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: HiddenObjects value was incorrect. Defaulting to 0%.");
                        hiddenNEOs = 0f;
                    }
                }
                #endregion

                #region Audio
                if (settingsData[i].ToUpper().StartsWith("MASTERVOLUME"))
                {
                    try
                    {
                        masterVolume = float.Parse(words[1]);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: masterVolume value was incorrect. Defaulting to 100% (1).");
                        masterVolume = 1f;
                    }
                }
                if (settingsData[i].ToUpper().StartsWith("SOUNDVOLUME"))
                {
                    try
                    {
                        soundVolume = float.Parse(words[1]);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: soundVolume value was incorrect. Defaulting to 100% (1).");
                        soundVolume = 1f;
                    }
                }
                if (settingsData[i].ToUpper().StartsWith("AMBIENCEVOLUME"))
                {
                    try
                    {
                        ambienceVolume = float.Parse(words[1]);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: ambienceVolume value was incorrect. Defaulting to 100% (1).");
                        ambienceVolume = 1f;
                    }
                }
                if (settingsData[i].ToUpper().StartsWith("MUSICVOLUME"))
                {
                    try
                    {
                        musicVolume = float.Parse(words[1]);
                    }
                    catch
                    {
                        Logger.AppendLine("Error reading settings.data: musicVolume value was incorrect. Defaulting to 100% (1).");
                        musicVolume = 1f;
                    }
                }
                #endregion
            }
        }

        public static void AssignResolution(int x, int y)
        {
            WindowResolution = new Point(x, y);
            WindowCenter = new Point(x / 2, y / 2);

            VectorResolution = new Vector2(x, y);
            VectorCenter = new Vector2(x / 2, y / 2);
        }

        public static void ApplyDefaultDisplaySettings()
        {
            //Apply exact screen resolution
            //Fullscreen should be untouched
        }
        public static void ApplyDefaultAudioSettings()
        {
            soundVolume = 1f; musicVolume = 1f;
        }
        public static void ApplyDefaultGameSettings()
        {
            showParticles = true;
            limitParticles = false;
            particleInteraction = true;

            playerSaveSpeed = 10;
            mapSaveSpeed = 15;
            worldSaveSpeed = 20;
        }
        public static void ApplyDefaultControlSettings()
        {
            //Controls.SetDefaultControls();

            //Leave game controller bool untouched
        }

        public static void ApplyChanges(WorldManager world, GraphicsDeviceManager graphics, Game game)
        {
            //Change resolution
            graphics.PreferredBackBufferWidth = GameSettings.WindowResolution.X;
            graphics.PreferredBackBufferHeight = GameSettings.WindowResolution.Y;

            //Fullscreen
            graphics.IsFullScreen = GameSettings.IsFullScreen;

            //Display cursor
            game.IsMouseVisible = !GameSettings.DisplayCursor;

            //VSync
            graphics.SynchronizeWithVerticalRetrace = isVSync;

            //Apply changes
            graphics.ApplyChanges();
            world.ResetRenderTargets();

            SaveData();
        }

        public static bool ShowParticles { get { return showParticles; } set { showParticles = value; } }
        public static float LimitPercentage() { return limitParticlePercentage; }

        public static int MapSaveSpeed() { return mapSaveSpeed; }
        public static int PlayerSaveSpeed() { return playerSaveSpeed; }
        public static int WorldSaveSpeed() { return worldSaveSpeed; }

        // ----- Do not display below in settings interface! -----
        private static bool isDebugging = false;
        public static bool IsDebugging { get { return isDebugging; } set { isDebugging = value; } }
        public static bool NoClip { get { return noClip; } set { noClip = value; } }
    }
}
