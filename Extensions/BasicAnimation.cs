using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Global.Interfaces;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class BasicAnimation : IUnload
    {
        private int time = 0;
        private Point position;

        public int framesPassed = 0;

        protected bool isUnloading = false;
        bool isUnloaded = false;
        public virtual void Unload()
        {
            isUnloading = true;

            time = 0;
            position = Point.Zero;
            framesPassed = 0;

            isUnloaded = true;
        }
        public bool IsUnloading()
        {
            return isUnloading;
        }
        public bool IsUnloaded()
        {
            return isUnloaded;
        }

        public int FramePositionX(GameTime gt, int frameSpeed, int width, bool isReversed)
        {
            time += gt.ElapsedGameTime.Milliseconds;

            if (isReversed == false) //Increase frames like normal
            {
                if (time >= frameSpeed)
                {
                    position.X++;
                    framesPassed++;

                    if (position.X >= width)
                        position.X = 0;

                    time = 0;
                }
            }
            else
            {
                if (time >= frameSpeed)
                {
                    position.X--;
                    framesPassed--;

                    if (position.X < 0)
                        position.X = width - 1;

                    time = 0;
                }
            }

            return position.X;
        }

        public Point FramePosition(GameTime gt, int timeBetweenFrames, Point sheetSize, bool increaseFrames)
        {
            time += gt.ElapsedGameTime.Milliseconds;

            if (increaseFrames == true) //Increase frames like normal
            {
                if (time >= timeBetweenFrames)
                {
                    position.X++;
                    framesPassed++;

                    if (position.X >= sheetSize.X)
                    {
                        position.X = 0;
                        position.Y++;

                        if (position.Y > sheetSize.Y - 1)
                        {
                            position.Y = 0;
                            framesPassed = 0;
                        }
                    }

                    time = 0;
                }
            }
            else if (increaseFrames == false) //Decrease frames
            {
                if (time >= timeBetweenFrames)
                {
                    position.X--;
                    framesPassed--;

                    if (position.X < 0)
                    {
                        position.X = sheetSize.X - 1;
                        position.Y--;

                        if (position.Y < 0)
                        {
                            position.Y = sheetSize.Y - 1;
                            framesPassed = 0;
                        }
                    }

                    if (position.X < 0)
                    {
                        if (sheetSize.Y > 1)
                        {
                            --position.Y;
                            position.X = sheetSize.X;
                        }

                        position.X = sheetSize.X;
                    }

                    time = 0;
                }
            }

            return position;
        }
        public Point FramePosition(GameTime gt, ref int currentTime, ref Point position, ref int framesPassed,
                                   int timeBetweenFrames, Point sheetSize, bool increaseFrames = true)
        {
            currentTime += gt.ElapsedGameTime.Milliseconds;

            if (increaseFrames == true) //Increase frames like normal
            {
                if (currentTime >= timeBetweenFrames)
                {
                    ++position.X;
                    ++framesPassed;

                    if (position.X >= sheetSize.X)
                    {
                        position.X = 0;
                        if (sheetSize.Y > 1)
                        {
                            position.Y++;
                            position.X = 0;
                        }
                    }

                    time = 0;
                }
            }
            else if (increaseFrames == false) //Decrease frames
            {
                if (currentTime >= timeBetweenFrames)
                {
                    --position.X;
                    ++framesPassed;

                    if (position.X < 0)
                    {
                        if (sheetSize.Y > 1)
                        {
                            --position.Y;
                            position.X = sheetSize.X;
                        }

                        position.X = sheetSize.X;
                    }

                    time = 0;
                }
            }

            return position;
        }
    }
}
