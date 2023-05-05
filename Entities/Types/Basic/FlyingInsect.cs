using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.TileEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.Entities.Types.Basic
{
    public class FlyingInsect : GameObject
    {
        private Texture2D texture;
        private int timePerFrame, frameWidth;
        private Point currentFrame, sheetsize;
        private BasicAnimation animation = new BasicAnimation();

        private Vector2 targetPosition, homePosition;
        private float targetAngle, maxSpeed, maxAngleAdjust, maxDistance, windMultiplier;
        private int posTimer = 0;

        private int heightTimer = 0, landDelay = 0;
        private bool isLandDelaySet = false;
        private float minHeight, maxHeight, targetHeight, maxHeightAdjust;

        private int maxHealth, itemID, quantity;

        enum State { Alive, Dead, Harvested }
        State state = State.Alive;

        public FlyingInsect() : base() { }
        public FlyingInsect(int ID, int CurrentFloor, float DepthOrigin, Vector2 HomePosition, Texture2D Texture, int TimePerFrame, int FrameWidth,
                            float MaxSpeed, float MaxDistance, float MaxAngleAdjust, float MinHeight, float MaxHeight, float MaxHeightAdjust, float WindMultiplier, int MaxHealth, int ItemID, int Quantity)
            : base(ID, CurrentFloor, DepthOrigin)
        {
            position = HomePosition;
            homePosition = HomePosition;

            texture = Texture;
            timePerFrame = TimePerFrame;
            frameWidth = FrameWidth;

            currentFrame = Point.Zero;
            sheetsize = new Point(texture.Width / frameWidth, 0);

            maxSpeed = MaxSpeed;
            maxDistance = MaxDistance;
            maxAngleAdjust = MaxAngleAdjust;

            minHeight = MinHeight;
            maxHeight = MaxHeight;

            maxHeightAdjust = MaxHeightAdjust;
            windMultiplier = WindMultiplier;
            maxHealth = MaxHealth;

            itemID = ItemID;
            quantity = Quantity;

            maxGravity = 100f;

            if (itemID != -1)
                IsObjectUsable = true;
            if (itemID != -1 || maxHealth != -1)
                isSaveType = true;

            isUseTileLocation = false;
        }

        public override void Load(ContentManager cm)
        {
            targetAngle = trueRandom.NextFloat(0, 6.2f);
            HeightOffset = trueRandom.NextFloat(minHeight, maxHeight);

            base.Load(cm);
        }
        public override void LoadMain(ContentManager main)
        {
            base.LoadMain(main);
        }

        public override void Update(GameTime gt)
        {
            if (IsOnScreen() == true)
            {
                UpdateDepth(depthOrigin);

                if (state == State.Alive)
                    CheckAlive(gt);
                else
                    isGravityEnabled = true;
            }
            base.Update(gt);
        }
        private void CheckAlive(GameTime gt)
        {
            isGravityEnabled = false;

            if (heightOffset < 0)
            {
                UpdateMovement(gt);
                UpdateAnimation(gt);
                UpdateHeight(gt);

                isLandDelaySet = false;
            }
            else
            {
                if (isLandDelaySet == false)
                {
                    landDelay = trueRandom.Next(1000, 10000);
                    isLandDelaySet = true;
                }

                if (landDelay > 0)
                    landDelay -= gt.ElapsedGameTime.Milliseconds;
                else
                {
                    targetHeight = trueRandom.NextFloat(-maxHeightAdjust * 2, -maxHeightAdjust);
                    heightTimer = -500;

                    heightOffset = -1;
                }
            }
        }
        private void UpdateMovement(GameTime gt)
        {
            //Calculate direction
            if (posTimer >= 50)
            {
                if (Vector2.Distance(position, homePosition) < maxDistance)
                {
                    targetAngle += trueRandom.NextFloat(-maxAngleAdjust, maxAngleAdjust);

                    posTimer = 0;
                }
                else
                {
                    float homeAngle = position.Direction(homePosition);

                    float antiClockwiseDistance = Math.Abs(180 - targetAngle) + Math.Abs(-180 - homeAngle);
                    float clockwiseDistance = homeAngle - targetAngle;

                    if (targetAngle > homeAngle + .1 || targetAngle < homeAngle - .1f)
                    {
                        if (clockwiseDistance < antiClockwiseDistance)
                            targetAngle += MathHelper.Clamp(Math.Abs(targetAngle - homeAngle) * 10, 0, (maxAngleAdjust * 10)) * (float)gt.ElapsedGameTime.TotalSeconds;
                        else
                            targetAngle -= MathHelper.Clamp(Math.Abs(targetAngle - homeAngle) * 10, 0, (maxAngleAdjust * 10)) * (float)gt.ElapsedGameTime.TotalSeconds;
                    }
                }

                if (targetAngle > MathHelper.Pi)
                    targetAngle -= MathHelper.TwoPi;
                if (targetAngle < -MathHelper.Pi)
                    targetAngle += MathHelper.TwoPi;
            }
            else
                posTimer += gt.ElapsedGameTime.Milliseconds;

            targetPosition = Circle.Rotate(targetAngle, maxSpeed, position);

            //Move
            position += (targetAngle.ToVector2() * maxSpeed) * (float)gt.ElapsedGameTime.TotalSeconds;
            SimulateWind(gt);
        }

        private void UpdateHeight(GameTime gt)
        {
            if (heightTimer >= 250)
            {
                targetHeight = trueRandom.NextFloat(-maxHeightAdjust, maxHeightAdjust);
                heightTimer = 0;
            }
            else
                heightTimer += gt.ElapsedGameTime.Milliseconds;

            HeightOffset += targetHeight * (float)gt.ElapsedGameTime.TotalSeconds;

            if (HeightOffset > minHeight)
                HeightOffset = minHeight;
            if (HeightOffset < maxHeight)
                HeightOffset = maxHeight;
        }
        private void UpdateAnimation(GameTime gt)
        {
            currentFrame = animation.FramePosition(gt, timePerFrame, sheetsize, false);
        }

        private float windSpeed;
        private void SimulateWind(GameTime gt)
        {
            if (weather.HasWindChanged == true)
                windSpeed = weather.RetrieveHorizontalWindSpeed(camera.WorldToScreen(Position).X) * windMultiplier;

            position.X -= windSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isVisible == true && state != State.Harvested)
            {
                sb.Draw(texture, Position, new Rectangle(currentFrame.X * frameWidth, 0, frameWidth, texture.Height), Color.White,
                        targetAngle + 1.5708f, new Vector2(frameWidth / 2, texture.Height / 2), 2f, SpriteEffects.None, Depth);

                sb.Draw(texture, Position + new Vector2(0, shadowHeightOffset), new Rectangle(currentFrame.X * frameWidth, 0, frameWidth, texture.Height),
                        WorldLight.ShadowColor, targetAngle + 1.5708f, new Vector2(frameWidth / 2, texture.Height / 2), 2f, SpriteEffects.None, Depth - .0001f);
            }

            base.Draw(sb);
        }
        public override void DrawUI(SpriteBatch sb)
        {
            base.DrawUI(sb);
        }

        public override string SaveData()
        {
            RefreshUniqueID(homePosition);
            return string.Join(" ", "FlyingInsect", uniqueID, (int)state);
        }
        public override void LoadData(string data)
        {
            try
            {
                if (data.ToUpper().StartsWith("FLYINGINSECT"))
                {
                    RefreshUniqueID(homePosition);
                    string[] words = data.Split(" ");
                    
                    if (uniqueID == words[1])
                    {
                        state = (State)Enum.Parse(typeof(State), words[2]);
                    }
                }
            }
            catch { }

            base.LoadData(data);
        }

        public override void SelectObject(GameTime gt)
        {
            if (state == State.Alive)
            {
                ArrowOffset = 24f;
                screens.ACTIVATEBOX_SetLines("Catch", "Flying Insect");

                base.SelectObject(gt);
            }
        }
        public override void UseObject(BaseEntity entity)
        {
            if (state == State.Alive && itemID != -1)
            {
                entity.STORAGE_AddItem(itemID, quantity);

                //If a NPE grabbed it, give the player a chance to get it back by killing it.
                if (!(entity is PlayerEntity))
                    entity.LOOT_AddItem(itemID, quantity, 100f);

                state = State.Harvested;
                IsObjectUsable = false;
            }

            base.UseObject(entity);
        }

        #region Parse-related
        protected override GameObject GetParseObject(string line, string[] words, ContentManager cm)
        {
            return new FlyingInsect(int.Parse(NextWord(words)), int.Parse(NextWord(words)), float.Parse(NextWord(words)), new Vector2(float.Parse(NextWord(words)),
                                    float.Parse(NextWord(words))), cm.Load<Texture2D>(NextWord(words)), int.Parse(NextWord(words)), int.Parse(NextWord(words)),
                                    float.Parse(NextWord(words)), float.Parse(NextWord(words)), float.Parse(NextWord(words)), float.Parse(NextWord(words)), float.Parse(NextWord(words)),
                                    float.Parse(NextWord(words)), float.Parse(NextWord(words)), int.Parse(NextWord(words)), int.Parse(NextWord(words)), int.Parse(NextWord(words)));
        }

        public override void SetDisplayVariables()
        {
            displayVariables.AppendLine("Vector2 <HomePosition> (" + homePosition.X + ", " + homePosition.Y + ")");
            displayVariables.AppendLine("int <TimePerFrame> (" + timePerFrame + ")");
            displayVariables.AppendLine("int <FrameWidth> (" + frameWidth + ")");
            displayVariables.AppendLine("float <MaxSpeed> (" + maxSpeed + ")");
            displayVariables.AppendLine("float <MaxDistance> (" + maxDistance + ")");
            displayVariables.AppendLine("float <MaxAngleAdjust> (" + maxAngleAdjust + ")");
            displayVariables.AppendLine("float <MinHeight> (" + minHeight + ")");
            displayVariables.AppendLine("float <MaxHeight> (" + maxHeight + ")");
            displayVariables.AppendLine("float <MaxHeightAdjust> (" + maxHeightAdjust + ")");
            displayVariables.AppendLine("float <WindMultiplier> (" + windMultiplier + ")");
            displayVariables.AppendLine("int <MaxHealth> (" + maxHealth + ")");
            displayVariables.AppendLine("int <ItemID> (" + itemID + ")");
            displayVariables.AppendLine("int <Quantity> (" + quantity + ")");

            displayVariables.AppendLine("void Revive()");
            displayVariables.AppendLine("void Kill()");

            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            try
            {
                if (line.ToUpper().StartsWith("HOMEPOSITION"))
                {
                    homePosition.X = float.Parse(words[1]);
                    homePosition.Y = float.Parse(words[2]);
                }
                if (line.ToUpper().StartsWith("TIMEPERFRAME"))
                    timePerFrame = int.Parse(words[1]);
                if (line.ToUpper().StartsWith("FRAMEWIDTH"))
                    frameWidth = int.Parse(words[1]);
                if (line.ToUpper().StartsWith("MAXSPEED"))
                    maxSpeed = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("MAXDISTANCE"))
                    maxDistance = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("MAXANGLEADJUST"))
                    maxAngleAdjust = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("MINHEIGHT"))
                    minHeight = float.Parse(words[1]);
                if (words[0].ToUpper().Equals("MAXHEIGHT"))
                    maxHeight = float.Parse(words[1]);
                if (words[0].ToUpper().Equals("MAXHEIGHTADJUST"))
                    maxHeightAdjust = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("WINDMULTIPLIER"))
                    windMultiplier = float.Parse(words[1]);
                if (line.ToUpper().StartsWith("MAXHEALTH"))
                    maxHealth = int.Parse(words[1]);
                if (line.ToUpper().StartsWith("ITEMID"))
                    itemID = int.Parse(words[1]);
                if (line.ToUpper().StartsWith("QUANTITY"))
                    quantity = int.Parse(words[1]);

                if (line.ToUpper().StartsWith("REVIVE"))
                    state = State.Alive;
                if (line.ToUpper().StartsWith("KILL"))
                    state = State.Dead;
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
            AddSuggest(TileEngine.Map.Editor.AutoSuggestionObject.ObjectType.Objects, "FlyingInsect int ID, int CurrentFloor, float DepthOrigin, Vector2 HomePosition, Texture2D Texture, int TimePerFrame, int FrameWidth, float MaxSpeed, float MaxDistance float MaxAngleAdjust, float MinHeight, float MaxHeight, float MaxHeightAdjust, float WindMultiplier, int MaxHealth, int ItemID, int Quantity");
            base.InitializeSuggestLine();
        }
        public override string MapOutputLine()
        {
            return string.Join(" ", ParseName, ID, CurrentFloor, depthOrigin, homePosition.X, homePosition.Y, texture.Name, timePerFrame, frameWidth, maxSpeed, maxDistance, maxAngleAdjust, minHeight, maxHeight, maxHeightAdjust, windMultiplier, maxHealth, itemID, quantity);
        }
        #endregion
    }
}
