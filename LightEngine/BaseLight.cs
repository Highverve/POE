using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.LightEngine
{
    public class BaseLight : GameObject
    {
        private string texturePath;
        private Texture2D texture;

        private Color color;
        protected float size;

        float angle;

        //Animation
        public Point frameSize = new Point(128, 128);
        public Point currentFrame = new Point(0, 0);
        Point sheetSize = new Point(4, 0);

        protected Random random;

        public float BaseSize { get { return baseSize; } set { baseSize = value; } }
        protected float baseSize;

        public BaseLight() : base(-1, -1, 0) { }
        public BaseLight(int ID, string TexturePath, Point TileLocation, Vector2 Offset, Color Color, float Size, float Angle) : base(ID, -1, 0)
        {
            texturePath = TexturePath;

            tileLocation = TileLocation;
            position = Offset;

            color = Color;
            size = Size;
            baseSize = size;

            angle = Angle;

            random = new Random((int)((tileLocation.X + position.X * tileLocation.Y + position.Y) + (color.R * color.G * color.B + color.A) * (Size + Angle)));

            ParseName = "Light";
        }

        public override void Load(ContentManager cm)
        {
            texture = cm.Load<Texture2D>(texturePath);
        }

        public virtual void UpdateLights(GameTime gt)
        {
            /* Move to AnimatedLight.cs
            if (lightType == LightType.Animated)
            {
                lastFrame += gt.ElapsedGameTime.Milliseconds;
                if (lastFrame > MPF)
                {
                    lastFrame -= MPF;
                    ++currentFrame.X;
                    if (currentFrame.X >= sheetSize.X)
                    {
                        currentFrame.X = 0;

                        ++currentFrame.Y;
                        if (currentFrame.Y >= sheetSize.Y)
                            currentFrame.Y = 0;
                    }
                }
            }
            */
        }

        public virtual void DrawLight(SpriteBatch sb)
        {
            if (isActivated == true) //Don't check for "IsOnScreen". Has a bad time with big lights.
            {
                sb.Draw(texture, Position, new Rectangle(0, 0, texture.Width, texture.Height), color, angle,
                        texture.Center(), size, SpriteEffects.None, 0f);

                 // Move to animation
                 //   sb.Draw(texture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), color, rotate,
                 //           new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);
            }
        }

        public BaseLight Parse(string parseName, string line, ContentManager cm, string mapName, int currentLine, Action<string, MapIssue.MessageType> issue)
        {
            string[] words = line.Split(' ');
            BaseLight obj = null;

            this.ParseName = parseName;

            if (CheckParse(line) == true)
            {
                try
                {
                    obj = new BaseLight(int.Parse(words[1]), words[2], new Point().Parse(words[3], words[4]), new Vector2().Parse(words[5], words[6]), new Color(byte.Parse(words[7]), byte.Parse(words[8]), byte.Parse(words[9]), byte.Parse(words[10])), float.Parse(words[11]), float.Parse(words[12]));
                    obj.texture.Name = words[2];
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
            displayVariables.AppendLine("float BaseSize (" + baseSize + ")");
            displayVariables.AppendLine("float Size (" + size + ")");
            displayVariables.AppendLine("float Angle (" + angle + ")");

            displayVariables.AppendLine("Color Color (" +  color.R + ", " + color.G + ", " + color.B + ", " + color.A + ")");

            displayVariables.AppendLine("void AdjustAngle(float value)");
            displayVariables.AppendLine("void AdjustSize(float value)");

            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            try
            {
                if (line.ToUpper().StartsWith("BASESIZE"))
                    baseSize = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("SIZE"))
                    size = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("ANGLE"))
                    angle = float.Parse(words[1]);

                if (line.ToUpper().StartsWith("COLOR"))
                    color = new Color(byte.Parse(words[1]), byte.Parse(words[2]), byte.Parse(words[3]), byte.Parse(words[4]));

                if (line.ToUpper().StartsWith("ADJUSTANGLE"))
                {
                    angle += float.Parse(words[1]);
                }
                if (line.ToUpper().StartsWith("ADJUSTSIZE"))
                {
                    size = MathHelper.Clamp(size + float.Parse(words[1]), 0f, baseSize);
                }
            }
            catch (Exception e)
            {
            }

            base.ParseEdit(line, words);
        }

        public override void InitializeSuggestLine()
        {
            objectType = TileEngine.Map.Editor.AutoSuggestionObject.ObjectType.Lights;
            suggestLines.Add("Light int ID, string TexturePath, Point TileLocation, Vector2 Offset, Color Color, float Size, float Angle");
        }

        public override string MapOutputLine()
        {
            return ParseName + " " + ID + " " + texture.Name + " " + tileLocation.X + " " + tileLocation.Y + " " + (int)position.X + " " + (int)position.Y + " " + color.R + " " + color.G + " " + color.B + " " + color.A + " " + baseSize + " " + angle;
        }

    }
}