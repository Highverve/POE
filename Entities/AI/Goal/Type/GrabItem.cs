using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    class GrabItem : BaseGoal
    {
        Vector2 itemPosition;

        public GrabItem(string GoalName, BaseEntity Entity, DesireHolder Desire, Vector2 ItemPosition)
            : base(GoalName, Entity, Desire, 1)
        { this.itemPosition = ItemPosition; }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            if (Vector2.Distance(entity.Position, itemPosition) <= 64)
            {
                entity.ActivateObject(); //If the item is close, activate object to grab it!
                state = GoalState.Completed;
            }

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
