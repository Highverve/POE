using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class GuardPoint : BaseGoal
    {
        private Vector2 guardPoint;
        private float maxDistance;

        public GuardPoint(string GoalName, BaseEntity Entity, Vector2 GuardPoint, DesireHolder Desire, float MaxDistance)
            : base(GoalName, Entity, Desire, 1)
        { guardPoint = GuardPoint; maxDistance = MaxDistance; }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            if (Vector2.Distance(entity.Position, guardPoint) >= maxDistance)
            {
                entity.SetMovementMotion(entity.steering.Arrive(guardPoint, true, maxDistance));
                entity.SENSES_SightDirection = entity.Position.Direction(guardPoint);
            }

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
