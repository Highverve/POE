using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Performance
{
    public class CallLimiter
    {
        private int currentTime, maxTime;
        public CallLimiter(int Time) { this.maxTime = Time; }

        /// <summary>
        /// Use this by itself, or only use IsCalling() combined with Countdown(GameTime gt.
        /// </summary>
        /// <param name="gt"></param>
        public bool IsCalling(GameTime gt)
        {
            if (currentTime > maxTime)
            {
                currentTime = 0;
                return true;
            }
            else
                currentTime += gt.ElapsedGameTime.Milliseconds;

            return false;
        }

        /// <summary>
        /// Use this in combination with Countdown(GameTime gt), or only use IsCalling(GameTime gt).
        /// </summary>
        /// <param name="gt"></param>
        public bool IsCalling() { return currentTime >= maxTime; }
        /// <summary>
        /// Use this in combination with IsCalling(), or only use IsCalling(GameTime gt).
        /// </summary>
        /// <param name="gt"></param>
        public void Countdown(GameTime gt)
        {
            if (currentTime >= maxTime)
                currentTime = 0;

            currentTime += gt.ElapsedGameTime.Milliseconds;
        }

        public void ChangeTime(int time) { maxTime = time; }
    }
}
