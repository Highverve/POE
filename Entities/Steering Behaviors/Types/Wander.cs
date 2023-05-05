using System;
using Pilgrimage_Of_Embers.Entities.Entities;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Steering_Behaviors.Types
{
    public class Wander : BaseSteering
    {
        /// <summary>
        /// BROKEN! DO NOT USE YET! 
        /// </summary>

        Random random;
        Vector2 wanderTarget, toTarget;

        bool hasArrived;
        int time, waitTime = 3000, baseWaitTime = 3000;

        public Wander(BaseEntity Entity)
            : base(Entity)
        {
            random = new Random(Guid.NewGuid().GetHashCode());

            SetVariables();
            hasArrived = true;
        }

        public override void Update(GameTime gt)
        {
            if (hasArrived == true)
            {
                time += gt.ElapsedGameTime.Milliseconds;

                if (time >= waitTime)
                {
                    SetVariables();
                }
            }
            else
            {
                toTarget = wanderTarget - currentEntity.Position;
                float distance = toTarget.Length();

                float tweak = .1f;
                float speed = (distance) / (2 * tweak);
                currentEntity.Speed = speed;

                Vector2 desiredVelocity = toTarget / distance;
                currentEntity.SetMovementMotion(desiredVelocity);

                if (distance <= 10)
                {
                    hasArrived = true;
                }
            }

            base.Update(gt);
        }

        private void SetVariables()
        {
            waitTime = random.Next(0, 2000);
            waitTime += baseWaitTime;

            wanderTarget = new Vector2((float)random.Next(-100, 100), (float)random.Next(-100, 100));

            hasArrived = false;
            time = 0;
        }
    }
}
