using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes.Weather;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.TileEngine.Map.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pilgrimage_Of_Embers.ParticleEngine
{
    class WeatherHolder
    {
        private string weatherName;
        private int weatherLength, weatherTime;
        private BaseEmitter weather;
        private SoundEffectInstance weatherSFX;
        private float maxWeatherVolume;

        public string WeatherName { get { return weatherName; } }
        public BaseEmitter WeatherEmitter { get { return weather; } }
        public Point WeatherLengthRange { get; private set; }
        public int WeatherLength { get { return weatherLength; } }
        public int WeatherTime { get { return weatherTime; } }

        public int WeatherRate { get; private set; }
        public Point RateRange { get; set; }

        public bool IsActive { get; set; }
        public bool IsTimeUp() { return weatherTime >= weatherLength; }

        public WeatherHolder(string WeatherName, BaseEmitter WeatherEmitter, SoundEffect WeatherSFX, float MaxWeatherVolume)
        {
            weatherName = WeatherName;
            weather = WeatherEmitter;

            if (WeatherSFX != null)
            {
                weatherSFX = WeatherSFX.CreateInstance();
                weatherSFX.IsLooped = true;
                weatherSFX.Volume = 0f;

                maxWeatherVolume = MaxWeatherVolume;
            }
        }

        public void Load(ContentManager cm)
        {
            if (weather != null)
                weather.Load(cm);
        }
        public void Update(GameTime gt)
        {
            if (weather != null)
                weather.Update(gt);

            if (weatherTime < weatherLength)
            {
                if (weatherSFX != null)
                {
                    if (weatherSFX.State == SoundState.Stopped)
                        weatherSFX.Play();

                    if (weatherSFX.Volume < maxWeatherVolume)
                    {
                        float volume = weatherSFX.Volume;
                        volume += .25f * (float)gt.ElapsedGameTime.TotalSeconds;
                        weatherSFX.Volume = MathHelper.Clamp(volume, 0f, maxWeatherVolume);
                    }
                }
            }

            weatherTime += gt.ElapsedGameTime.Milliseconds;
            if (weatherTime >= weatherLength)
            {
                if (weather != null && weatherSFX != null) //Both are not empty, so check both values before disabling weather
                {
                    if (weather.Particles.Count <= 0 &&
                        weatherSFX.State == SoundState.Stopped)
                    {
                        IsActive = false;
                    }

                    weather.IsActivated = false;
                    DecreaseSFXVolume(gt);
                }
                else if (weather != null && weatherSFX == null) //Emitter is not empty, but SFX is.
                {
                    if (weather.Particles.Count <= 0)
                        IsActive = false;

                    weather.IsActivated = false;
                }
                else if (weatherSFX != null && weather == null) //SFX is not empty, but emitter is.
                {
                    if (weatherSFX.State == SoundState.Stopped)
                        IsActive = false;

                    DecreaseSFXVolume(gt);
                }
                else //Both are empty, so disable immediately.
                    IsActive = false;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (weather != null)
                weather.Draw(sb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="weatherLengthRange">Measured in real minutes!</param>
        /// <param name="weatherRate">Proportionally to the other weather chances!</param>
        public void SetValues(Random random, Point weatherLengthRange, int weatherRate)
        {
            WeatherLengthRange = weatherLengthRange;
            WeatherRate = weatherRate;

            weatherLength = random.Next((WeatherLengthRange.X * 60) * 1000, (WeatherLengthRange.Y * 60) * 1000);
            weatherTime = 0;
        }
        public void Reset(Random random)
        {
            IsActive = true;

            if (weather != null)
                weather.IsActivated = true;

            weatherLength = random.Next((WeatherLengthRange.X * 60) * 1000, (WeatherLengthRange.Y * 60) * 1000);
            weatherTime = 0;
        }
        public void SmoothEnd()
        {
            weatherTime = weatherLength;
        }
        public void ForceEnd()
        {
            IsActive = false;

            if (weather != null)
            {
                weather.IsActivated = false;
                weather.ForceRemoveAll();
            }

            if (weatherSFX != null)
                weatherSFX.Stop();
        }
        private void DecreaseSFXVolume(GameTime gt)
        {
            float volume = weatherSFX.Volume;
            volume -= .1f * (float)gt.ElapsedGameTime.TotalSeconds;
            weatherSFX.Volume = MathHelper.Clamp(volume, 0f, maxWeatherVolume);

            if (weatherSFX.Volume <= 0f)
                weatherSFX.Stop();
        }

        public override string ToString()
        {
            return weatherName + " [" + weatherLength + ", " + WeatherRate + "]";
        }
    }
    class WindHolder
    {
        public float Speed { get; private set; }
        public float Width { get; private set; }
        public Vector2 Range { get; set; }

        public WindHolder(float Speed, float Width)
        {
            this.Speed = Speed;
            this.Width = Width;
        }

        public override string ToString()
        {
            return "Wind [Speed: " + Speed + ", Width: " + Width + ", Range: " + Range.X + "-" + Range.Y + "]";
        }
    }

    public class WeatherManager : GameObject
    {
        //Wind Stuffs
        private Vector2 windDirection; //Some objects, like flowers, will only be affected by horizontal wind direction (windirection.X)

        private Vector2 horizontalWindSpeedRange;
        private float horizontalWindWidth, horizontalSpeedMultiplier, horizontalWidthMultiplier;
        private int xCurrentTime, xWindTime, horizontalLengthMultiplier;

        private List<WindHolder> horizontalWind = new List<WindHolder>();
        private List<WindHolder> verticalWind = new List<WindHolder>();
        private Random random;

        public bool HasWindChanged { get; private set; }
        public float MaxHorizontalWindSpeed()
        {
            return Math.Max(horizontalWindSpeedRange.X, horizontalWindSpeedRange.Y);
        }

        private float listenerWindSpeed, listenerTargetSpeed, maxWindVolume;
        private SoundEffectInstance windSFX;

        //Weather Stuffs
        private List<WeatherHolder> totalWeather = new List<WeatherHolder>();
        private List<WeatherHolder> currentWeather = new List<WeatherHolder>();

        private WeatherHolder currentWeatherHolder = null;

        private Texture2D pixel;

        public WeatherManager() : base(-5, 1, 0)
        {
            random = new Random(Guid.NewGuid().GetHashCode());

            RecalibrateHorizontalWind();

            position = new Vector2(64, -64);
            isUseTileLocation = false;
        }

        public override void Load(ContentManager cm)
        {
            AddWeathers(cm);

            for (int i = 0; i < totalWeather.Count; i++)
                totalWeather[i].Load(cm);

            pixel = cm.Load<Texture2D>("rect");

            windSFX = cm.Load<SoundEffect>("World/Audio/Ambience/Weather/windLoop1").CreateInstance();
            windSFX.IsLooped = true;
            windSFX.Play();

            maxWindVolume = .5f;
        }
        public override void SetReferences(TileMap map, Camera camera, ScreenEngine.ScreenManager screens, Entities.Types.PlayerEntity player,
                                  WeatherManager weather, Culture.CultureManager culture, Entities.Entities.BaseEntity controlledEntity,
                                  List<BaseEntity> entities)
        {
            base.SetReferences(map, camera, screens, player, weather, culture, controlledEntity, entities);

            for (int i = 0; i < totalWeather.Count; i++)
            {
                if (totalWeather[i].WeatherEmitter != null)
                    totalWeather[i].WeatherEmitter.SetReferences(map, camera, screens, player, this, culture, controlledEntity, entities);
            }
        }

        private void AddWeathers(ContentManager cm)
        {
            totalWeather.Add(new WeatherHolder("None", null, null, 0f));
            totalWeather.Add(new WeatherHolder("LightRain", new Rain(-1, 0, 0, 7, 30) { IsManualDepth = true }, null, 0f));
            totalWeather.Add(new WeatherHolder("HeavyRain", new Rain(-1, 0, 0, 20, 20) { IsManualDepth = true }, cm.Load<SoundEffect>("World/Audio/Ambience/Weather/RainLoopHeavy"), .5f));
            totalWeather.Add(new WeatherHolder("LightSnow", new Snow(-1, 0, 0, 7, 30) { IsManualDepth = true }, null, 0f));
        }

        public override void Update(GameTime gt)
        {
            UpdateWeather(gt);
            UpdateWind(gt);
        }
        private void UpdateWeather(GameTime gt)
        {
            for (int i = 0; i < currentWeather.Count; i++)
            {
                if (currentWeather[i].IsActive == true)
                    currentWeather[i].Update(gt);
            }

            
            if (currentWeatherHolder != null)
            {
                if (currentWeatherHolder.IsTimeUp() == true)
                    currentWeatherHolder = WEATHER_Select();
            }
            else
                currentWeatherHolder = WEATHER_Select();
        }
        private void UpdateWind(GameTime gt)
        {
            HasWindChanged = false;
            ScrollWind(gt, ref horizontalWind, ref xCurrentTime, xWindTime, windDirection.X);

            if (HasWindChanged == true)
                listenerTargetSpeed = Math.Abs(RetrieveHorizontalWindSpeed(camera.WorldToScreen(camera.ListenerPosition).X));

            listenerWindSpeed += ((listenerTargetSpeed - listenerWindSpeed) * 2) * (float)gt.ElapsedGameTime.TotalSeconds;

            //If the max wind speed (either direction) is 2f, and the current listener wind speed = 1f... value is .5.
            //So the value range is 0-1. This is ideal for SFX volume. Add +.5f for pitch (multiply by decimal for a smaller range)
            float windSpeedToPct = listenerWindSpeed / MaxHorizontalWindSpeed();

            windSFX.Volume = MathHelper.Clamp(windSpeedToPct * maxWindVolume, 0, 1);
            windSFX.Pitch = MathHelper.Clamp(((windSpeedToPct * .85f) - .25f), -1, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.DrawBoxBordered(pixel, new Rectangle((int)position.X - 3, (int)position.Y - 3, 6, 6), Color.Blue, Color.White, 1f);

            for (int i = 0; i < currentWeather.Count; i++)
            {
                if (currentWeather[i].IsActive == true)
                    currentWeather[i].Draw(sb);
            }
        }

        // [Methods] Weather
        public void WEATHER_AddCategory(string weatherName, int minimumLength, int maximumLength, int rate)
        {
            for (int i = 0; i < totalWeather.Count; i++)
            {
                if (totalWeather[i].WeatherName.ToUpper().Equals(weatherName.ToUpper()) && !currentWeather.Contains(totalWeather[i]))
                {
                    currentWeather.Add(totalWeather[i]);
                    currentWeather.Last().SetValues(random, new Point(minimumLength, maximumLength), rate);
                    break;
                }
            }

            WEATHER_SetRateRanges();
        }
        public void WEATHER_RemoveCategory(string weatherName, int minimumLength, int maximumLength, int rate)
        {
            for (int i = 0; i < currentWeather.Count; i++)
            {
                if (currentWeather[i].WeatherName.ToUpper().Equals(weatherName.ToUpper()) &&
                    currentWeather[i].WeatherLengthRange.X == minimumLength &&
                    currentWeather[i].WeatherLengthRange.Y == maximumLength &&
                    currentWeather[i].WeatherRate == rate)
                {
                    currentWeather[i].ForceEnd();
                    currentWeather.Remove(currentWeather[i]);
                    break;
                }
            }
        }

        private void WEATHER_SetRateRanges()
        {
            int lastRate = 0;

            for (int i = 0; i < currentWeather.Count; i++)
            {
                currentWeather[i].RateRange = new Point(lastRate, lastRate + currentWeather[i].WeatherRate);
                lastRate += currentWeather[i].WeatherRate;
            }
        }
        private WeatherHolder WEATHER_Select()
        {
            if (currentWeather.Count > 0)
            {
                int totalRate = 0;

                for (int i = 0; i < currentWeather.Count; i++)
                    totalRate += currentWeather[i].RateRange.Y;

                int value = random.Next(0, totalRate);

                for (int i = 0; i < currentWeather.Count; i++)
                {
                    if (value >= currentWeather[i].RateRange.X &&
                        value < currentWeather[i].RateRange.Y)
                    {
                        currentWeather[i].Reset(random);
                        return currentWeather[i];
                    }
                }
            }

            return null;
        }

        public void WEATHER_Clear()
        {
            for (int i = 0; i < currentWeather.Count; i++)
                currentWeather[i].ForceEnd();
            currentWeather.Clear();

            SetWindZero();
        }
        public void WEATHER_BeginFade()
        {
            for (int i = 0; i < currentWeather.Count; i++)
                currentWeather[i].SmoothEnd();
        }

        // [Methods] Wind
        public void SetWindZero()
        {
            SetHorizontalValues(0f, Vector2.Zero, 0, -1, 1, 0, 0);

            RecalibrateHorizontalWind();
            RecalibrateVerticalWind();
        }
        public void SetHorizontalValues(float windDirection, Vector2 windSpeedRange, float windWidth, int windTransitionTime, int lengthMultiplier, float speedMultiplier, float widthMultiplier)
        {
            this.windDirection.X = windDirection;
            horizontalWindSpeedRange = windSpeedRange;
            horizontalWindWidth = windWidth;
            xWindTime = windTransitionTime;
            horizontalLengthMultiplier = lengthMultiplier;
            horizontalSpeedMultiplier = speedMultiplier;
            horizontalWidthMultiplier = widthMultiplier;
        }

        public void RecalibrateHorizontalWind()
        {
            RecalibrateWind(ref horizontalWind, horizontalWindSpeedRange, horizontalWindWidth, horizontalLengthMultiplier, horizontalSpeedMultiplier, horizontalWidthMultiplier);
        }
        public void RecalibrateVerticalWind() { }
        private void RecalibrateWind(ref List<WindHolder> wind, Vector2 windSpeedRng, float windWidth, int lengthMultiplier, float speedMultiplier, float widthMultiplier)
        {
            wind.Clear();

            int windCount = (int)(GameSettings.WindowResolution.X / windWidth) * lengthMultiplier;
            float currentSpeed = random.NextFloat(windSpeedRng.X, windSpeedRng.Y); //Start with a random value between current wind speed range

            for (int i = 0; i < windCount; i++)
            {
                currentSpeed += random.NextFloat(-(Math.Abs(windSpeedRng.Y)) * speedMultiplier, (windSpeedRng.Y) * speedMultiplier);
                currentSpeed = MathHelper.Clamp(currentSpeed, windSpeedRng.X, windSpeedRng.Y);

                wind.Add(new WindHolder(currentSpeed, (int)(windWidth + random.NextFloat(-(windWidth * widthMultiplier), windWidth * widthMultiplier))));
            }
        }

        private void ScrollWind(GameTime gt, ref List<WindHolder> wind, ref int time, int maxTime, float windDirection)
        {
            if (maxTime != -1)
            {
                time += gt.ElapsedGameTime.Milliseconds;

                if (time >= maxTime)
                {
                    MoveWindItem(windDirection < 0, ref wind); //Move wind item left if windDirection.X is negative

                    float positionX = 0;
                    for (int i = 0; i < wind.Count; i++)
                    {
                        if (wind[i] != null)
                        {
                            wind[i].Range = new Vector2(positionX, positionX + wind[i].Width);
                            positionX += wind[i].Width;
                        }
                    }

                    time = 0;
                    HasWindChanged = true;
                }
            }
        }
        private void MoveWindItem(bool isMovingNegative, ref List<WindHolder> wind)
        {
            if (isMovingNegative == false)
            {
                WindHolder temp = wind.FirstOrDefault();
                wind.Remove(temp);
                wind.Insert(wind.Count, temp);
            }
            else
            {
                WindHolder temp = wind.LastOrDefault();
                wind.Remove(temp);
                wind.Insert(0, temp);
            }
        }

        public float RetrieveHorizontalWindSpeed(float location)
        {
            if (xWindTime != -1)
            {
                for (int i = 0; i < horizontalWind.Count; i++)
                {
                    if (horizontalWind[i] != null)
                    {
                        if (horizontalWind[i].Range.X < location)
                        {
                            if (horizontalWind[i].Range.Y >= location)
                            {
                                return horizontalWind[i].Speed;
                            }
                        }
                    }
                }
            }

            return 0f; //If not in range, return 0f.
        }


        public void Parse(string line, Action<string, MapIssue.MessageType> issue)
        {
            if (line.ToUpper().StartsWith("WEATHER")) //Expected format: Weather [Rain 10 30 50, ... ]
            {
                try
                {
                    string excerpt = line.FromWithin('[', ']', 1);
                    string[] words = excerpt.Replace(", ", ",").Split(','); //Replace comma-space with comma, and split by comma only

                    string currentCategory = string.Empty;

                    //Iterate and add weather categories
                    for (int i = 0; i < words.Length; i++)
                    {
                        currentCategory = words[i];

                        try
                        {
                            string[] words2 = words[i].Split(' ');
                            WEATHER_AddCategory(words2[0], int.Parse(words2[1]), int.Parse(words2[2]), int.Parse(words2[3]));
                        }
                        catch
                        {
                            issue.Invoke("Error parsing weather category (" + currentCategory + ")", MapIssue.MessageType.Error);
                        }
                    }

                    WEATHER_SetRateRanges();
                }
                catch
                {
                    issue.Invoke("Error retrieving data from weather brackets (" + line  + ")", MapIssue.MessageType.Error);
                }
            }

            if (line.ToUpper().StartsWith("WINDX"))
            {
                string[] words = line.Split(' ');

                try
                {
                    SetHorizontalValues(float.Parse(words[1]), new Vector2().Parse(words[2], words[3]), float.Parse(words[4]), int.Parse(words[5]), int.Parse(words[6]), float.Parse(words[7]), float.Parse(words[8]));

                    try
                    {
                        RecalibrateHorizontalWind();
                    }
                    catch
                    {
                        issue.Invoke("Error calibrating horizontal wind (" + line + ")", MapIssue.MessageType.Error);
                    }
                }
                catch
                {
                    SetWindZero();
                    issue.Invoke("Error parsing horizontal wind (" + line + ")", MapIssue.MessageType.Error);
                }
            }
        }
        public override void SetDisplayVariables()
        {
            //Eventually, make a lot of these read/write-able!
            displayVariables.AppendLine("float <WindDirectionX> (" + windDirection.X + ")");
            displayVariables.AppendLine("Vector2 <SpeedRangeX> (" + horizontalWindSpeedRange.X + ", " + horizontalWindSpeedRange.Y + ")");
            displayVariables.AppendLine("float <WindPixelWidth> (" + horizontalWindWidth + ")");
            displayVariables.AppendLine("int <LengthMultiplierX> (" + horizontalLengthMultiplier + ")");
            displayVariables.AppendLine("float <SpeedMultiplierX> (" + horizontalSpeedMultiplier + ")");
            displayVariables.AppendLine("float <PixelMultiplierX> (" + horizontalWidthMultiplier + ")");
            displayVariables.AppendLine("int <WindTimeX> (" + xWindTime + ")");

            if (currentWeatherHolder != null)
            {
                displayVariables.AppendLine("int _WeatherTimeLeft_ (" + (currentWeatherHolder.WeatherTime / 1000) + "s)");
                displayVariables.AppendLine("int _WeatherLength_ (" + (currentWeatherHolder.WeatherLength / 1000) + "s)");
            }

            displayVariables.AppendLine("void RecalibrateWind()");
            displayVariables.AppendLine("void AddWeatherCategory(string name, int minimumLength, int maximumLength, int rate)");
            displayVariables.AppendLine("void RemoveWeatherCategory(string name, int minimumLength, int maximumLength, int rate)");
            displayVariables.AppendLine("void RandomWeather()");
            displayVariables.AppendLine("void SelectWeather([Unimplemented])");

            displayVariables.AppendLine("\n----- Current Weather -----");
            for (int i = 0; i < currentWeather.Count; i++)
            {
                displayVariables.AppendLine(currentWeather[i].WeatherName + " [Min: " + currentWeather[i].WeatherLengthRange.X + ", Max: " +
                                            currentWeather[i].WeatherLengthRange.Y + ", Rate: " + currentWeather[i].WeatherRate + ", IsActive: " + currentWeather[i].IsActive + "]");
            }
            displayVariables.AppendLine();

            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            try
            {
                if (line.ToUpper().StartsWith("WINDDIRECTIONX"))
                    windDirection.X = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("SPEEDRANGEX"))
                {
                    horizontalWindSpeedRange.X = float.Parse(words[1]);
                    horizontalWindSpeedRange.Y = float.Parse(words[2]);
                }
                if (line.ToUpper().StartsWith("WINDPIXELWIDTH"))
                    horizontalWindWidth = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("LENGTHMULTIPLIERX"))
                    horizontalLengthMultiplier = int.Parse(words[1]);
                if (line.ToUpper().StartsWith("SPEEDMULTIPLIERX"))
                    horizontalSpeedMultiplier = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("PIXELMULTIPLIERX"))
                    horizontalWidthMultiplier = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("WINDTIMEX"))
                    xWindTime = int.Parse(words[1]);

                if (line.ToUpper().StartsWith("SPEEDRANGEX"))
                {
                    horizontalWindSpeedRange.X = float.Parse(words[1]);
                    horizontalWindSpeedRange.Y = float.Parse(words[2]);
                }

                if (line.ToUpper().StartsWith("RECALIBRATEWIND"))
                {
                    RecalibrateHorizontalWind();
                    RecalibrateVerticalWind();
                }

                if (line.ToUpper().StartsWith("ADDWEATHERCATEGORY"))
                {
                    WEATHER_AddCategory(words[1], int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]));
                }
                if (line.ToUpper().StartsWith("REMOVEWEATHERCATEGORY"))
                {
                    WEATHER_RemoveCategory(words[1], int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]));
                }
                if (line.ToUpper().StartsWith("RANDOMWEATHER"))
                {
                    WEATHER_BeginFade();
                    currentWeatherHolder = WEATHER_Select();
                }
                if (line.ToUpper().StartsWith("SELECTWEATHER"))
                {
                }
            }
            catch
            {
            }

            base.ParseEdit(line, words);
        }
        public override string MapOutputLine()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("WindX " + windDirection.X + " " + horizontalWindSpeedRange.X + " " + horizontalWindSpeedRange.Y + " " + horizontalWindWidth + " " + xWindTime + " " + horizontalLengthMultiplier + " " + horizontalSpeedMultiplier + " " + horizontalWidthMultiplier);

            string weathers = string.Empty;

            weathers += "[";

            for (int i = 0; i < currentWeather.Count; i++)
            {
                weathers += currentWeather[i].WeatherName + " " + currentWeather[i].WeatherLengthRange.X + " " + currentWeather[i].WeatherLengthRange.Y + " " + currentWeather[i].WeatherRate; //[Rain 10 30 50, Clear 200, Snow 10]

                if (i != currentWeather.Count - 1)
                    weathers += ", ";
            }

            weathers += "]";

            builder.AppendLine("Weather " + weathers);

            return builder.ToString();
        }
        public override void InitializeSuggestLine()
        {
            //float windDirection, Vector2 windSpeedRange, float windWidth, int windTransitionTime, int lengthMultiplier, float speedMultiplier, float widthMultiplier)
            //SetHorizontalValues(float.Parse(words[1]), new Vector2().Parse(words[2], words[3]), float.Parse(words[4]), int.Parse(words[5]), int.Parse(words[6]), float.Parse(words[7]), float.Parse(words[8]));
            objectType = AutoSuggestionObject.ObjectType.Objects;
            suggestLines.Add("WindX float WindDirection, Vector2 SpeedRangeX, float WindPixelWidth, int WindTimeX, int LengthMultiplierX, float SpeedMultiplierX, float PixelMultiplierX");
        }
    }
}
