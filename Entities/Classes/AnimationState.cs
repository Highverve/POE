using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.LightEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pilgrimage_Of_Embers.Entities
{
    public class AnimationState
    {
        /* To-do:
         * - Read animation variables from a text file (maybe...)
         * - Update all Point4 locations when ready
         * 
         */

        #region Animation Fields

        Texture2D spriteSheet;
        Point currentFrame, startFrame, endFrame, frameSize;
        int baseTimePerFrame, bonusTimePerFrame, timer, pauseTime;
        float scale = 2f;

        public Texture2D AnimationSheet { get { return spriteSheet; } }
        public Point CurrentFrame { get { return currentFrame; } }
        public Point FrameSize { get { return frameSize; } }
        public int BaseTimePerFrame { get { return baseTimePerFrame; } }
        public int TimePerFrame() { return baseTimePerFrame + bonusTimePerFrame;  }
        public float Scale { get { return scale; } }

        public enum AnimateState { Animate, Paused, PausedTime }
        AnimateState animationState = AnimateState.Animate;
        public AnimateState CurrentState { get { return animationState; } }

        #endregion

        #region Look/Movement Enums
        public enum Direction { Up = 1, Down = 2, Left = 3, Right = 4 }

        Direction lookDirection = Direction.Down;
        Direction moveDirection;

        public Direction Look { get { return lookDirection; } set { lookDirection = value; } }
        public Direction Move { get { return moveDirection; } }

        #endregion

        #region Action Fields

        List<ActionState> actionStates = new List<ActionState>();
        ActionState currentAction = null, lastAction;
        
        public ActionState CurrentAction { get { return currentAction; } }
        public ActionState LastAction { get { return lastAction; } }

        #endregion

        //Action states will vary from player, to monster, to NPC, etc.
        //
        //List of recommended actions:
        //Idle, Walk, Sprint, Sneak, SneakIdle,
        //Climb, ClimbIdle, Swim, SwimIdle,
        //Activate, Consume
        //Jump, Midair, Lift, LifeMovement,
        //LeftHand, RightHand, DualHand, Dodge
        //Dead, Teleport

        readonly Point4 DeadDown = new Point4(8, 30, 15, 30);
        public Rectangle DeathTexture { get { return new Rectangle(DeadDown.C * frameSize.X, DeadDown.D * frameSize.Y, frameSize.X, frameSize.Y); } }

        public AnimationState(Texture2D SpriteSheet, string StateFilePath)
        {
            spriteSheet = SpriteSheet;

            stateFilePath = StateFilePath;
            LoadFromFile(stateFilePath);
        }
        public AnimationState(Texture2D SpriteSheet, Point FrameSize, int BaseTimePerFrame, float Scale = 2f)
        {
            spriteSheet = SpriteSheet;
            frameSize = FrameSize;

            baseTimePerFrame = BaseTimePerFrame;
            scale = Scale;
        }

        #region Updating Methods
        public void Update(GameTime gt)
        {
            lastAction = currentAction;

            UpdateActionState();
            UpdateAnimationState(gt);
        }
        private void UpdateAnimationState(GameTime gt)
        {
            if (animationState == AnimateState.Animate)
                UpdateFramePosition(gt, TimePerFrame());
            else if (animationState == AnimateState.PausedTime)
            {
                pauseTime -= gt.ElapsedGameTime.Milliseconds;

                if (pauseTime <= 0)
                {
                    pauseTime = 0;
                    animationState = AnimateState.Animate;
                }
            }
        }

        private void UpdateFramePosition(GameTime gt, int timeBetweenFrames)
        {
            timer += gt.ElapsedGameTime.Milliseconds;

            currentFrame.X = MathHelper.Clamp(currentFrame.X, startFrame.X, endFrame.X);
            currentFrame.Y = MathHelper.Clamp(currentFrame.Y, startFrame.Y, endFrame.Y);

            if (IsAnimationReversed == true) //Reversed animation
            {
                if (timer >= timeBetweenFrames)
                {
                    --currentFrame.X;

                    if (currentFrame.X < startFrame.X)
                    {
                        currentFrame.X = endFrame.X - 1;

                        if (currentFrame.Y < startFrame.Y)
                        {
                            currentFrame.Y = endFrame.Y - 1;
                        }
                    }

                    timer = 0;
                }
            }
            else //Animate forward (default)
            {
                if (timer >= timeBetweenFrames)
                {
                    ++currentFrame.X;

                    if (currentFrame.X > endFrame.X)
                    {
                        currentFrame.X = startFrame.X;
                        if (currentFrame.Y > endFrame.Y)
                        {
                            currentFrame.Y++;
                            currentFrame.X = startFrame.X;
                        }
                    }

                    timer = 0;
                }
            }
        }
        private void SetFrameValues(Point4 value)
        {
            startFrame = value.FirstTwo();
            endFrame = value.LastTwo();
        }
        #endregion

        public void DrawSprite(SpriteBatch sb, Vector2 location, Vector2 offset, Color entityColor, float depth, float shadowOffset)
        {
            sb.Draw(spriteSheet, new Vector2(location.X + offset.X, location.Y + offset.Y),
                                 new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), entityColor, 0f,
                                 new Vector2(frameSize.X / 2, frameSize.Y / 2), scale, SpriteEffects.None, depth);

            sb.Draw(spriteSheet, new Vector2(location.X + offset.X, (location.Y + offset.Y) + shadowOffset),
                                 new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), WorldLight.ShadowColor, 0f,
                                 new Vector2(frameSize.X / 2, frameSize.Y / 2), scale, SpriteEffects.FlipVertically, depth - .0001f);
        }

        #region Animation Methods

        public void SetTimePerFrame(int milliseconds)
        {
            baseTimePerFrame = MathHelper.Clamp(milliseconds, 16, 1000);
        }
        public void ResetCurrentValues()
        {
            currentFrame = new Point(0, 0);
            timer = 0;
        }

        public void PlayAnimation()
        {
            animationState = AnimateState.Animate;
        }
        public void PauseAnimation(uint pauseTime)
        {
            animationState = AnimateState.PausedTime;
            this.pauseTime = (int)pauseTime;
        }
        public void PauseAnimation()
        {
            animationState = AnimateState.Paused;
        }

        public bool IsAnimationReversed { get; set; }
        public bool IsAnimationFinished
        {
            get
            {
                return ((currentFrame.X == endFrame.X) &&
                        (currentFrame.Y == endFrame.Y));
            }
        }

        #endregion

        #region Look/Movement Methods

        public void SetLookDirection(float angle)
        {
            if (angle > -135 && angle < -45)
                lookDirection = Direction.Up;
            else if (angle > -45 && angle < 45)
                lookDirection = Direction.Right;
            else if (angle > 45 && angle < 135)
                lookDirection = Direction.Down;
            else
                lookDirection = Direction.Left;
        }
        public void SetMoveDirection(Vector2 Velocity)
        {
            if (Velocity.Y < 0)
                moveDirection = Direction.Up;
            else if (Velocity.Y > 0)
                moveDirection = Direction.Down;

            if (Velocity.X < 0)
                moveDirection = Direction.Left;
            else if (Velocity.X > 0)
                moveDirection = Direction.Right;
        }

        public void SetLookDirection(Direction direction)
        {
            lookDirection = direction;
        }
        public void SetMoveDirection(Direction direction)
        {
            moveDirection = direction;
        }

        public bool IsPolarOpposite()
        {
            if (lookDirection == Direction.Down && moveDirection == Direction.Up)
                return true;
            if (lookDirection == Direction.Up && moveDirection == Direction.Down)
                return true;
            if (lookDirection == Direction.Left && moveDirection == Direction.Right)
                return true;
            if (lookDirection == Direction.Right && moveDirection == Direction.Left)
                return true;

            return false;
        }

        #endregion

        #region Action Methods

        public void SetAction(string actionName)
        {
            for (int i = 0; i < actionStates.Count; i++)
            {
                if (actionStates[i].ActionName.ToUpper().Equals(actionName.ToUpper()))
                    currentAction = actionStates[i];
            }
        }
        public bool IsPerforming(string actionName)
        {
            if (currentAction != null && currentAction.ActionName.ToUpper().Equals(actionName.ToUpper()))
                return true;

            return false;
        }

        public void ModifyActionPoint(string actionName, Direction direction, Point4 newPoint)
        {
            for (int i = 0; i < actionStates.Count; i++)
            {
                if (actionStates[i].ActionName.ToUpper().Equals(actionName.ToUpper()))
                    actionStates[i].SetByLook(direction, newPoint);
            }
        }
        public void UpdateActionState()
        {
            if (currentAction != null)
            {
                //Set the frame to animate to and from (start to end)
                Point4 temp = currentAction.GetByLook(lookDirection);
                startFrame = temp.FirstTwo();
                endFrame = temp.LastTwo();

                //Set bonus time per frame
                bonusTimePerFrame = currentAction.BonusTimePerFrame;

                //Set if the animation should be reversed. Typically applied to walking animations, where "backwards walking" is expected.
                IsAnimationReversed = false;
                if (currentAction.IsReversible && IsPolarOpposite())
                    IsAnimationReversed = true;
            }
            else
                SetDefaultAction();
        }
        public void SetDefaultAction()
        {
            if (actionStates != null && actionStates.Count > 0)
                SetAction("Idle"); //Every animation needs to have an idle action
        }

        #endregion

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
            bool isReadingAnimation = false;

            for (int i = 0; i < data.Count; i++)
            {
                //Parse header
                if (data[i].ToUpper().StartsWith("[ANIMATION"))
                {
                    isReadingAnimation = true;

                    string[] words = data[i].Split(' ');

                    try
                    {
                        frameSize.X = int.Parse(words[1]);
                        frameSize.Y = int.Parse(words[2]);

                        baseTimePerFrame = int.Parse(words[3]);
                        scale = float.Parse(words[4]);
                    }
                    catch (System.Exception e)
                    {
                        Logger.AppendLine("Error parsing AnimationState's standard variables." + e.Message);
                    }
                }

                //Parse actions
                if (isReadingAnimation && !string.IsNullOrEmpty(data[i]))
                {
                    try
                    {
                        string name = data[i].FromWithin("\"", 1);
                        int bonusTime = int.Parse(data[i].FromWithin("\"", 2));
                        bool isReversible = bool.Parse(data[i].FromWithin("\"", 3));

                        Point4 up = new Point4(-1, -1, -1, -1), down = new Point4(-1, -1, -1, -1),
                               left = new Point4(-1, -1, -1, -1), right = new Point4(-1, -1, -1, -1);

                        string[] words = data[i].FromWithin("\"", 4).Split(' ');
                        up.A = int.Parse(words[0]); up.B = int.Parse(words[1]);
                        up.C = int.Parse(words[2]); up.D = int.Parse(words[3]);

                        words = data[i].FromWithin("\"", 5).Split(' ');
                        down.A = int.Parse(words[0]); down.B = int.Parse(words[1]);
                        down.C = int.Parse(words[2]); down.D = int.Parse(words[3]);

                        words = data[i].FromWithin("\"", 6).Split(' ');
                        left.A = int.Parse(words[0]); left.B = int.Parse(words[1]);
                        left.C = int.Parse(words[2]); left.D = int.Parse(words[3]);

                        words = data[i].FromWithin("\"", 7).Split(' ');
                        right.A = int.Parse(words[0]); right.B = int.Parse(words[1]);
                        right.C = int.Parse(words[2]); right.D = int.Parse(words[3]);

                        actionStates.Add(new ActionState(name, bonusTime, isReversible, up, down, left, right));
                    }
                    catch (System.Exception e)
                    {
                        Logger.AppendLine("Error parsing one of AnimationState's action variables." + e.Message);
                    }
                }

                if (data[i].ToUpper().StartsWith("[/ANIMATION]"))
                {
                    isReadingAnimation = false;
                    break;
                }
            }
        }

        public AnimationState Copy()
        {
            AnimationState copy = (AnimationState)MemberwiseClone();

            copy.currentAction = null;
            copy.lastAction = null;
            copy.actionStates = actionStates.ToList();

            return copy;
        }
    }
    public class ActionState
    {
        string actionName;
        int timePerFrame;
        bool isReversible;
        Point4 up, down, left, right;

        public string ActionName { get { return actionName; } }
        public int BonusTimePerFrame { get { return timePerFrame; } }
        public bool IsReversible { get { return isReversible; } }

        public ActionState(string ActionName, int BonusTimePerFrame, bool IsReversible, Point4 Up, Point4 Down, Point4 Left, Point4 Right)
        {
            actionName = ActionName;
            timePerFrame = BonusTimePerFrame;
            isReversible = IsReversible;

            if (Up != null) up = Up;
            else up = new Point4(-1, -1, -1, -1);

            if (Down != null) down = Down;
            else down = new Point4(-1, -1, -1, -1);

            if (Left != null) left = Left;
            else left = new Point4(-1, -1, -1, -1);

            if (Right != null) right = Right;
            else right = new Point4(-1, -1, -1, -1);
        }

        /// <summary>
        /// Animations that are not multi-directional would include death animations, possibly teleporation animations, etc.
        /// </summary>
        /// <returns></returns>
        public bool IsMultiDirectional()
        {
            return !up.IsNegative() && !down.IsNegative() &&
                   !left.IsNegative() && !right.IsNegative();
        }

        public void SetByLook(AnimationState.Direction direction, Point4 newPoint)
        {
            if (direction == AnimationState.Direction.Up)
                up = newPoint;
            if (direction == AnimationState.Direction.Down)
                down = newPoint;
            if (direction == AnimationState.Direction.Left)
                left = newPoint;
            if (direction == AnimationState.Direction.Right)
                right = newPoint;
        }

        public Point4 GetByLook(AnimationState.Direction direction)
        {
            if (direction == AnimationState.Direction.Up)
                return up;
            if (direction == AnimationState.Direction.Down)
                return down;
            if (direction == AnimationState.Direction.Left)
                return left;
            if (direction == AnimationState.Direction.Right)
                return right;

            return GetByNegativePrioritized();
        }
        public Point4 GetByNegativePrioritized()
        {
            if (!down.IsNegative())
                return down;
            if (!up.IsNegative())
                return up;
            if (!left.IsNegative())
                return left;
            if (!right.IsNegative())
                return right;

            return Point4.Zero;
        }
    }
}
