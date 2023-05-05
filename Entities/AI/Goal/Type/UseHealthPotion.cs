using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class UseHealthPotion : BaseGoal
    {
        public UseHealthPotion(string GoalName, BaseEntity Entity, DesireHolder Desire)
            : base(GoalName, Entity, Desire)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            entity.STORAGE_UseItemButton(entity.STORAGE_Search("Potion", "Healing"), 1);

            state = GoalState.Completed;

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
