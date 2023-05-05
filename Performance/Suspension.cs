using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Performance
{
    public class Suspension
    {
        public enum SuspendState
        {
            None,
            Suspended,
            EndSuspend
        }
        SuspendState suspension = SuspendState.None;

        int timer = 0, maxTime;
        public int CurrentTime { get { return timer; } }
        public int MaxTime { get { return maxTime; } }

        public SuspendState SuspensionState { get { return suspension; } }

        public void Suspend(int timer)
        {
            maxTime = timer;
            suspension = SuspendState.Suspended;

            this.timer = 0;
        }

        public void ManageSuspension(GameTime gt)
        {
            if (suspension == SuspendState.Suspended)
            {
                timer += gt.ElapsedGameTime.Milliseconds;

                if (timer >= maxTime)
                    suspension = SuspendState.EndSuspend;
            }
            else if (suspension == SuspendState.EndSuspend)
            {
                timer = 0;
                maxTime = 0;
                suspension = SuspendState.None;
            }
        }
    }
}
