using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    class Flank : BaseGoal
    {
        Vector2 position;
        public Vector2 Position { set { position = value; } }

        public Flank(string GoalName, BaseEntity Entity, DesireHolder Desire)
            : base(GoalName, Entity, Desire, 1)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            entity.SetMovementMotion(entity.steering.Flank(position, true));
            entity.SENSES_SightDirection = entity.Position.Direction(position);

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
