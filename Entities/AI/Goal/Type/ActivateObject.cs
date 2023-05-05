using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class ActivateObject : BaseGoal
    {
        public ActivateObject(string GoalName, BaseEntity Entity, DesireHolder Desire)
            : base(GoalName, Entity, Desire, 1)
        { }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            entity.ActivateObject();
            state = GoalState.Completed;

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
