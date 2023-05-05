using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Types.NPE;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class FollowPath : BaseGoal
    {
        public Point Goal { get; set; }
        private bool removeLastTile;

        public FollowPath(string GoalName, BaseEntity Entity, Point Goal, bool IsStopOneTileEarly, DesireHolder Desire)
            : base(GoalName, Entity, Desire, 1)
        {
            this.Goal = Goal;
            removeLastTile = IsStopOneTileEarly;
        }

        public override void Initialize()
        {
            entity.STEERING_SetState(Steering_Behaviors.SteeringBehavior.MovementState.Pathfind);

            entity.STEERING_ResetPathfind();
            entity.STEERING_SetTargetTile(Goal);

            base.Initialize();
        }
        private Point lastTile = Point.Zero;
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            if (entity.IsPathFailed() == true || entity.STEERING_State() == Steering_Behaviors.SteeringBehavior.MovementState.None)
            {
                //state = GoalState.Failed;
                entity.STEERING_ResetPathfind();
            }

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
