using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class DelayAI : BaseGoal
    {
        private int time = 0;

        public DelayAI(string GoalName, BaseEntity Entity, DesireHolder Desire, int Time, int FailTolerance = 3)
            : base(GoalName, Entity, Desire, FailTolerance)
        { time = Time; }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            time -= gt.ElapsedGameTime.Milliseconds;

            if (time <= 0)
                state = GoalState.Completed;

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
