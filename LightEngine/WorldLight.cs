using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.LightEngine
{
    public class WorldLight
    {
        public enum LightType
        {
            Dawn,
            Morning,
            Day,
            Evening,
            Moonlit,
            Night,
            Cave,
            Cycle,
            Debug
        }
        private static LightType ambientType;
        public static LightType Ambient { get { return ambientType; } set { ambientType = value; } }

        public float ambientIntensity, saturationIntensity;
        public float AmbientIntensity { get { return ambientIntensity; } private set { ambientIntensity = MathHelper.Clamp(value, 0f, 1f); } }
        public static Color AmbientColor { get; private set; }
        public static Color ShadowColor { get; private set; }
        public static Vector2 shadowScale;

        float time, secondsPerIndex = 3; //3 seconds: 3 * tintGradient.Width = 3 * 512 = 1536 / 60(seconds) = 25 minute days
        int xIndex, lastIndex;

        List<Texture2D> tintGradients = new List<Texture2D>();
        List<Texture2D> intensityGradients = new List<Texture2D>();
        List<Texture2D> saturationGradients = new List<Texture2D>();
        List<Texture2D> shadowGradients = new List<Texture2D>();

        private int tintIndex, intensityIndex, saturationIndex, shadowIndex;
        public int TintIndex { set { tintIndex = (int)MathHelper.Clamp(value, 0, tintGradients.Count); } }
        public int IntensityIndex { set { intensityIndex = (int)MathHelper.Clamp(value, 0, intensityGradients.Count); } }
        public int SaturationIndex { set { saturationIndex = (int)MathHelper.Clamp(value, 0, saturationGradients.Count); } }
        public int ShadowScale { set { shadowIndex = (int)MathHelper.Clamp(value, 0, shadowGradients.Count); } }

        private Debugging.DebugManager debug;
        private Culture.CultureManager culture;

        public WorldLight() { }
        public void SetReferences(Debugging.DebugManager debug, Culture.CultureManager culture)
        {
            this.debug = debug;
            this.culture = culture;
        }

        private const string gradientDir = "Effects/Gradients/";
        public void LoadGradients(ContentManager cm)
        {
            AddTint(cm.Load<Texture2D>(gradientDir + "daylightGradient"), "Default");
            AddIntensity(cm.Load<Texture2D>(gradientDir + "lightIntensity"), "Default");
            AddSaturation(cm.Load<Texture2D>(gradientDir + "saturationIntensity"), "Default");
            AddShadow(cm.Load<Texture2D>(gradientDir + "shadowScale"), "Default");

            ListAllGradients();

            SetHour(culture.CALENDAR_Hours);
        }
        private void AddTint(Texture2D texture, string name)
        {
            tintGradients.Add(texture);
            tintGradients.LastOrDefault().Name = name;
        }
        private void AddIntensity(Texture2D texture, string name)
        {
            intensityGradients.Add(texture);
            intensityGradients.LastOrDefault().Name = name;
        }
        private void AddSaturation(Texture2D texture, string name)
        {
            saturationGradients.Add(texture);
            saturationGradients.LastOrDefault().Name = name;
        }
        private void AddShadow(Texture2D texture, string name)
        {
            shadowGradients.Add(texture);
            shadowGradients.LastOrDefault().Name = name;
        }

        public void ListAllGradients()
        {
            debug.OutputConsole("Tints:");
            for (int i = 0; i < tintGradients.Count; i++)
                debug.OutputConsole(i.ToString() + " - " + tintGradients[i].Name);

            debug.OutputConsole("");
            debug.OutputConsole("Intesities:");

            for (int i = 0; i < intensityGradients.Count; i++)
                debug.OutputConsole(i.ToString() + " - " + intensityGradients[i].Name);

            debug.OutputConsole("");
            debug.OutputConsole("Saturations:");
            for (int i = 0; i < saturationGradients.Count; i++)
                debug.OutputConsole(i.ToString() + " - " + saturationGradients[i].Name);

            debug.OutputConsole("");
            debug.OutputConsole("Shadows:");
            for (int i = 0; i < shadowGradients.Count; i++)
                debug.OutputConsole(i.ToString() + " - " + shadowGradients[i].Name);
        }

        public void UpdateColor(GameTime gt)
        {
            switch (ambientType)
            {
                case LightType.Dawn: xIndex = 80; break;
                case LightType.Morning: xIndex = 116; break;
                case LightType.Day: xIndex = 192; break;
                case LightType.Evening: xIndex = 300; break;
                case LightType.Moonlit: xIndex = 400; break;
                case LightType.Night: xIndex = 500; break;
                case LightType.Cave: xIndex = 512; break;
                case LightType.Debug: CycleIndex(gt); break;
            }

            CycleTime(gt);
            UpdateColors();

            lastIndex = xIndex;
            lastTime = Time;
        }

        private Color shadowBase = Color.Lerp(Color.Transparent, Color.Black, .25f);
        private void UpdateColors()
        {
            if (lastIndex != xIndex)
            {
                saturationIntensity = ((float)TextureHelper.SelectAlphaColor(saturationGradients[saturationIndex], xIndex) / 85);
                AmbientIntensity = (float)TextureHelper.SelectAlphaColor(intensityGradients[intensityIndex], xIndex) / 256; // (float)intensityGradients[intensityIndex].Width;

                AmbientColor = Color.Lerp(TextureHelper.SelectColor(tintGradients[tintIndex], xIndex), Color.Transparent, ambientIntensity);
                ShadowColor = Color.Lerp(shadowBase, Color.Transparent, ambientIntensity);
                shadowScale = new Vector2(1, MathHelper.Lerp(.25f, 2f, (float)TextureHelper.SelectAlphaColor(shadowGradients[shadowIndex], xIndex) / 256));
            }
        }

        int tempIndex = 0;
        private void CycleTime(GameTime gt)
        {
            time += gt.ElapsedGameTime.Milliseconds;

            if (time >= (secondsPerIndex * 1000))
            {
                tempIndex++;
                if (tempIndex > tintGradients[tintIndex].Width)
                    tempIndex = 0;

                time = 0;
            }

            if (ambientType == LightType.Cycle)
                xIndex = tempIndex;

            if (lastTime == TimeOfDay.Dark && Time == TimeOfDay.Light)
                culture.CALENDAR_IncrementDays();

            culture.CALENDAR_Hours = Hour;
        }
        private void CycleIndex(GameTime gt)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.T))
                xIndex += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.G))
                xIndex -= 1;

            if (xIndex > tintGradients[tintIndex].Width)
                xIndex = 0;
            else if (xIndex < 0)
                xIndex = tintGradients[tintIndex].Width;
        }

        public enum TimeOfDay { Light, Dark }
        public TimeOfDay Time
        {
            get
            {
                if (xIndex <= 70 || xIndex >= 375) return TimeOfDay.Dark;
                else return TimeOfDay.Light;
            }
        }
        private TimeOfDay lastTime;

        public int Hour { get { return (int)MathHelper.Clamp((24 * xIndex / tintGradients[tintIndex].Width) + 1, 0, 24); } }
        public void SetHour(int hour) { hour = (int)MathHelper.Clamp((hour / tintGradients[tintIndex].Width), 0, 24); }
    }
}