using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class GoToPoint : BaseGoal
    {
        private Vector2 guardPoint;
        public Vector2 Position { set { guardPoint = value; } }

        private float maxDistance;

        public GoToPoint(string GoalName, NonPlayerEntity Entity, Vector2 GuardPoint, DesireHolder Desire, float MaxDistance)
            : base(GoalName, Entity, Desire, 1)
        { guardPoint = GuardPoint; maxDistance = MaxDistance; }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            entity.SetMovementMotion(entity.steering.Arrive(guardPoint, safeDistance:16));
            entity.SENSES_SightDirection = entity.Position.Direction(guardPoint);

            if (Vector2.Distance(entity.Position, guardPoint) < maxDistance)
            {
                state = GoalState.Completed;
                entity.STATE_SetAction("Idle");
            }

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
