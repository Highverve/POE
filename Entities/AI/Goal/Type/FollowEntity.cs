using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class FollowEntity : BaseGoal
    {
        private BaseEntity leader;

        public FollowEntity(string GoalName, BaseEntity Entity, BaseEntity Leader, DesireHolder Desire, int FailTolerance = 3)
            : base(GoalName, Entity, Desire, FailTolerance)
        { leader = Leader; }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            entity.SetMovementMotion(entity.steering.Arrive(leader.Position, true, 125));
            entity.SENSES_SightDirection = entity.Position.Direction(leader.Position);

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
