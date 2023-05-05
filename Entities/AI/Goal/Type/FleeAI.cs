using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class FleeAI : BaseGoal
    {
        public Vector2 FleeFrom { get; set; }

        public FleeAI(string GoalName, BaseEntity CurrentEntity, DesireHolder Desire, Vector2 FleeFrom )
            : base(GoalName, CurrentEntity, Desire, 1)
        {
            this.FleeFrom = FleeFrom;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            entity.STEERING_SetState(Steering_Behaviors.SteeringBehavior.MovementState.Flee);
            entity.STEERING_SetTargetPosition(FleeFrom);

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
