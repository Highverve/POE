using Pilgrimage_Of_Embers.Entities.Types.NPE;
using System;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class GlanceAround : BaseGoal
    {
        private Random random;
        private float minOffset, maxOffset, randomOffset;
        private int time, minTime, maxTime, randomTime;

        public GlanceAround(string GoalName, BaseEntity Entity, Desire.DesireHolder Desire,
                            float MinimumOffset, float MaximumOffset, int MinimumTime, int MaximumTime) : base(GoalName, Entity, Desire)
        {
            random = new Random(Guid.NewGuid().GetHashCode());

            minOffset = MinimumOffset;
            maxOffset = MaximumOffset;

            minTime = MinimumTime;
            maxTime = MaximumTime;

            randomTime = random.Next(minTime, maxTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gt)
        {
            time += gt.ElapsedGameTime.Milliseconds;

            if (time >= minTime)
            {
                randomOffset = random.NextFloat(minOffset, maxOffset);
                entity.SENSES_SightDirection += randomOffset;

                randomTime = random.Next(minTime, maxTime);
                time = 0;
            }

            base.Update(gt);
        }
    }
}
