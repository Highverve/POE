using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.Steering_Behaviors
{
    public class BaseSteering
    {
        protected Vector2 direction, side;
        public Vector2 Direction { get { return direction; } }
        public Vector2 Side { get { return side; } }

        float maxRotationSpeed;

        protected BaseEntity currentEntity, targetEntity;

        protected Vector2 targetPosition;
        public Vector2 TargetPosition { set { targetPosition = value; } }

        public BaseSteering(BaseEntity Entity)
        {
            currentEntity = Entity;
        }

        public virtual void Update(GameTime gt)
        {
            if (currentEntity.MovementMotion != Vector2.Zero)
            {
                direction = Vector2.Normalize(currentEntity.MovementMotion);
                side = VectorHelper.Cross(direction);
            }
        }
    }
}
