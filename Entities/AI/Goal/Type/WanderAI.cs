using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class WanderAI : BaseGoal
    {
        public WanderAI(string GoalName, BaseEntity Entity, DesireHolder Desire, int FailTolerance = 3)
            : base(GoalName, Entity, Desire, FailTolerance)
        { }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            //entity.Motion += entity.steering.Wander(gt, 300f, 200, 1500, 2500);
            state = GoalState.Completed;

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
