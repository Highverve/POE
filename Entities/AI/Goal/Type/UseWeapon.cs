using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.CombatEngine;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class UseWeapon : BaseGoal
    {
        private CombatMove[] moves;
        private float range;
        private bool isPrimary = true;

        public UseWeapon(string GoalName, BaseEntity Entity, DesireHolder Desire, bool IsPrimary, float Range, params CombatEngine.CombatMove[] Moves)
            : base(GoalName, Entity, Desire)
        {
            this.isPrimary = IsPrimary;
            this.range = Range;
            this.moves = Moves;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
