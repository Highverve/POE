using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class ArriveAI : BaseGoal
    {
        public ArriveAI(string GoalName, BaseEntity Entity, DesireHolder Desire, int FailTolerance = 3)
            : base(GoalName, Entity, Desire, FailTolerance)
        { }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            //if (entity.enemyTarget != null)
            //    entity.Motion += entity.steering.Seek();

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
