using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.AI.Desire
{
    public class DesireHolder
    {
        private float desire = 0;
        public float Desire { get { return desire; } set { desire = value; } }

        public DesireHolder(float Desire)
        {
            desire = Desire;
        }
    }

    public class DesireManager
    {
        private BaseEntity entity;

        public DesireManager(BaseEntity Entity)
        {
            entity = Entity;
        }

        public int RestoreHealth(int maxDesire)
        {
            return (entity.Skills.health.CurrentHP / entity.Skills.health.MaxHP) * maxDesire;
        }
        public int FleeEnemies(int maxDesire, float safeDistance)
        {
            return (int)(Vector2.Distance(entity.enemyTarget.Position, entity.Position) / safeDistance) * maxDesire;
        }
    }
}
