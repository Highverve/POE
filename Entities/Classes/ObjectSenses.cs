using System;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pilgrimage_Of_Embers.Entities
{
    public class ObjectSenses
    {
        Vector2 position;

        float voiceRadius,
              listenRadius,
              noiseRadius,
              maxNoiseRadius; //Sound sense fields

        float currentDirection,
              sightDirection,
              sightWidth,
              sightPeriph,
              maxSightLength,
              sightLength; //Visual sense fields

        float currentRotationSpeed, slowRotationSpeed, mediumRotationSpeed, fastRotationSpeed;

        public enum RotateSpeed { Slow, Medium, Fast }
        RotateSpeed speed = RotateSpeed.Slow;
        public RotateSpeed TurnSpeed { get { return speed; } set { speed = value; } }

        public Vector2 Position { set { position = value; } }

        public float VoiceRadius { get { return voiceRadius; } }
        public float ListenRadius { get { return listenRadius; } }
        public float NoiseRadius { get { return noiseRadius; } set { noiseRadius = value; } }

        public float SightWidth { get { return sightWidth; } }

        public float SightLength { get { return sightLength; } set { sightLength = MathHelper.Clamp(value, 0f, maxSightLength); } }
        public float MaxSightLength { get { return maxSightLength; } }

        public float SightDirection { get { return sightDirection; } set { sightDirection = value; } }
        public float CurrentDirection { get { return currentDirection; } set { currentDirection = value; } }

        #region Rendering Variables

        Texture2D pixel;
        Texture2D lookArrow;
        SpriteFont tinyFont;

        Color sight = Color.Lerp(Color.White, Color.Transparent, .25f);
        Color periph = Color.Lerp(Color.Orange, Color.Transparent, .5f);
        Color directionColor = Color.Lerp(Color.Orange, Color.Transparent, .5f);
        Color pointerColor = Color.Lerp(Color.White, Color.Transparent, .25f);

        Line sightLeft, sightRight, periphLeft, periphRight, direction;

        private float depth;
        public float Depth { set { depth = value; } }

        #endregion

        /// <summary>
        /// Voice, hearing, & sight senses for the entity
        /// </summary>
        /// <param name="VoiceRadius">Determines how far an entity's voice can travel. Used for entity message sending and sound effects.</param>
        /// <param name="ListenRadius">Determines how far the entity can hear other entities. Used for detecting other entities.</param>
        /// <param name="NoiseRadius">Determines how noisy an entity is when performing actions. Used in combination with ListenRadius.</param>
        /// <param name="SightWidth">Determines how wide an entity's sight range is. Measured in degrees.</param>
        /// <param name="SightLength">Determines how far an entity's sight can see. Measured in pixels.</param>
        /// <param name="SlowRotationSpeed">How fast the entity can turn, when TurnSpeed is set to "Slow". .0025 - .005</param>
        /// <param name="MedRotationSpeed">How fast the entity can turn, when TurnSpeed is set to "Medium". .005 - .0075</param>
        /// <param name="FastRotationSpeed">How fast the entity can turn, when TurnSpeed is set to "Fast". .0075 - .01</param>
        public ObjectSenses(float VoiceRadius, float ListenRadius, float SightWidth, float SightLength, float SlowRotationSpeed, float MedRotationSpeed, float FastRotationSpeed)
        {
            voiceRadius = VoiceRadius;
            listenRadius = ListenRadius;

            sightWidth = MathHelper.ToRadians(SightWidth);
            sightPeriph = MathHelper.ToRadians(SightWidth + (SightWidth / 4));

            sightLength = SightLength;
            maxSightLength = SightLength;

            sightLeft = new Line(Vector2.Zero, Vector2.Zero); periphLeft = new Line(Vector2.Zero, Vector2.Zero);
            sightRight = new Line(Vector2.Zero, Vector2.Zero); periphRight = new Line(Vector2.Zero, Vector2.Zero);
            direction = new Line(Vector2.Zero, Vector2.Zero);

            slowRotationSpeed = SlowRotationSpeed;
            mediumRotationSpeed = MedRotationSpeed;
            fastRotationSpeed = FastRotationSpeed;
        }
        public ObjectSenses(string StateFilePath)
        {
            stateFilePath = StateFilePath;

            sightLeft = new Line(Vector2.Zero, Vector2.Zero); periphLeft = new Line(Vector2.Zero, Vector2.Zero);
            sightRight = new Line(Vector2.Zero, Vector2.Zero); periphRight = new Line(Vector2.Zero, Vector2.Zero);
            direction = new Line(Vector2.Zero, Vector2.Zero);

            LoadFromFile(stateFilePath);
        }

        public void Load(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");
            tinyFont = cm.Load<SpriteFont>("Fonts/tinyFont");
            lookArrow = cm.Load<Texture2D>("Interface/Senses/pointer");
        }

        public enum SightType { None, Peripheral, Spotted }
        /// <summary>
        /// This is done so in degrees.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public SightType IsInSight(Vector2 position) //Experimental, however should would fine.
        {
            if (Vector2.Distance(position, this.position) <= sightLength) //Is the position within sight length?
            {
                float angle = (float)Math.Atan2(position.Y - this.position.Y, position.X - this.position.X);

                if (angle >= (currentDirection - sightWidth) &&
                    angle <= (currentDirection + sightWidth)) //If angle is inside metaphorical cone
                {
                    return SightType.Spotted;
                }
                else if (angle >= (currentDirection - sightPeriph) &&
                         angle <= (currentDirection + sightPeriph))
                {
                    return SightType.Peripheral;
                }
            }

            return SightType.None;
        }
        public bool IsHearingRange(Vector2 position)
        {
            if (Vector2.Distance(position, this.position) <= listenRadius)
                return true;

            return false;
        }

        private void UpdateRotationSpeed()
        {
            if (TurnSpeed == RotateSpeed.Slow)
                currentRotationSpeed = slowRotationSpeed;
            if (TurnSpeed == RotateSpeed.Medium)
                currentRotationSpeed = mediumRotationSpeed;
            if (TurnSpeed == RotateSpeed.Fast)
                currentRotationSpeed = fastRotationSpeed;
        }

        private float left, right, leftP, rightP, dir;
        public void Draw(SpriteBatch sb)
        {
            UpdateRotationSpeed();

            currentDirection = currentDirection.CurveAngle(currentDirection, sightDirection, currentRotationSpeed);

            if (float.IsNaN(currentDirection))
                currentDirection = 0f;

            //sb.Draw(lookArrow, new Vector2((float)Math.Cos(currentDirection) * pointerSpacing + position.X, (float)Math.Sin(currentDirection) * pointerSpacing + position.Y), pointerColor, lookArrow.Center(), currentDirection, 1f, depth - .0001f);

            if (GameSettings.IsDebugging == true)
            {
                sightLeft.locationA = position;
                sightRight.locationA = position;

                direction.locationA = position;
                periphLeft.locationA = position;
                periphRight.locationA = position;

                left = currentDirection - sightWidth;
                leftP = currentDirection - sightPeriph;
                right = currentDirection + sightWidth;
                rightP = currentDirection + sightPeriph;
                dir = currentDirection;

                sightLeft.locationB = new Vector2((float)Math.Cos(left) * 32 + position.X, (float)Math.Sin(left) * 32 + position.Y);
                sightRight.locationB = new Vector2((float)Math.Cos(right) * 32 + position.X, (float)Math.Sin(right) * 32 + position.Y);
                direction.locationB = new Vector2((float)Math.Cos(dir) * 32 + position.X, (float)Math.Sin(dir) * 32 + position.Y);

                sightLeft.DrawLine(sb, pixel, sight, 1f, 2);
                sightRight.DrawLine(sb, pixel, sight, 1f, 2);
                direction.DrawLine(sb, pixel, directionColor, 1f, 2);

                sb.DrawStringBordered(tinyFont, sightLength.ToString(), direction.locationB, tinyFont.MeasureString(sightLength.ToString()),
                                      direction.locationA.Direction(direction.locationB), 1f, .99f, Color.White, Color.Black);

                periphLeft.locationB = new Vector2((float)Math.Cos(leftP) * 24 + position.X, (float)Math.Sin(leftP) * 24 + position.Y);
                periphRight.locationB = new Vector2((float)Math.Cos(rightP) * 24 + position.X, (float)Math.Sin(rightP) * 24 + position.Y);

                periphLeft.DrawLine(sb, pixel, periph, 1);
                periphRight.DrawLine(sb, pixel, periph, 1);
            }
        }

        private string stateFilePath;
        public void ReloadState()
        {
            if (!string.IsNullOrEmpty(stateFilePath))
                LoadFromFile(stateFilePath);
        }
        public void LoadFromFile(string file)
        {
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                ParseFromFile(lines.ToList());
            }
        }
        public void ParseFromFile(List<string> data)
        {
            bool isReading = false;

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToUpper().StartsWith("[SENSES]"))
                {
                    isReading = false;
                    break;
                }
                if (data[i].ToUpper().StartsWith("[/SENSES]"))
                    isReading = true;

                if (isReading == true)
                {
                    if (data[i].ToUpper().StartsWith("VOICERADIUS"))
                    {
                        string[] words = data[i].Split(' ');
                        voiceRadius = float.Parse(words[1]);
                    }
                    if (data[i].ToUpper().StartsWith("LISTENRADIUS"))
                    {
                        string[] words = data[i].Split(' ');
                        listenRadius = float.Parse(words[1]);
                    }
                    if (data[i].ToUpper().StartsWith("SIGHTWIDTH"))
                    {
                        string[] words = data[i].Split(' ');
                        sightWidth = MathHelper.ToRadians(float.Parse(words[1]));
                        sightPeriph = MathHelper.ToRadians(SightWidth + (SightWidth / 4));
                    }
                    if (data[i].ToUpper().StartsWith("SIGHTLENGTH"))
                    {
                        string[] words = data[i].Split(' ');
                        sightLength = float.Parse(words[1]);
                        maxSightLength = sightLength;
                    }
                    if (data[i].ToUpper().StartsWith("ROTATION"))
                    {
                        string[] words = data[i].Split(' ');
                        slowRotationSpeed = float.Parse(words[1]);
                        mediumRotationSpeed = float.Parse(words[2]);
                        fastRotationSpeed = float.Parse(words[3]);
                    }
                }
            }
        }

        public ObjectSenses Copy()
        {
            ObjectSenses copy = (ObjectSenses)this.MemberwiseClone();

            sightLeft = new Line();
            sightRight = new Line();
            periphLeft = new Line();
            periphRight = new Line();
            direction = new Line();

            return copy;
        }
    }
}
