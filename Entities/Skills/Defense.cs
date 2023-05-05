using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Resistance : BaseSkill
    {
        private float physicalDefense, projectileDefense, magicalDefense; // This is not shield blocking, but percentage of damage blocked on entity hit

        private float currentStun = 0f, maxStun;
        public float CurrentStun { get { return currentStun; } set { currentStun = MathHelper.Clamp(value, 0f, maxStun); } } //If this goes above maxStun, the entity is stunned for a very short time (Think "Dark Souls player gets hit" short).
        public float MaxStun { get { return maxStun; } }

        public bool IsStunned { get { return currentStun > maxStun; } }
        public bool IsRecovered { get { return currentStun < (maxStun / 2); } }

        public Resistance(int Level)
            : base(Level, "Protection from all threats. Governs the stats of Physical Protection, Projectile Protection, Magical Protection, and Stun Protection.")
        {
        }

        public override void AssignValues()
        {
            physicalDefense = 1 - ((float)Level / 500);
            projectileDefense = 1 - ((float)Level / 500);
            magicalDefense = 1 - ((float)Level / 500);

            maxStun = 25 + (25 * ((float)Level / 100));

            attributes.SetMultiplier(BaseEntity.ATTRIBUTE_SkillsPhysicalDefense, physicalDefense);
            attributes.SetMultiplier(BaseEntity.ATTRIBUTE_SkillsProjectileDefense, projectileDefense);
            attributes.SetMultiplier(BaseEntity.ATTRIBUTE_SkillsMagicalDefense, magicalDefense);

            base.AssignValues();
        }
        public override void ResetMultipliers()
        {
            attributes.SetMultiplier(BaseEntity.ATTRIBUTE_SkillsPhysicalDefenseMultiplier, 1f);
            attributes.SetMultiplier(BaseEntity.ATTRIBUTE_SkillsProjectileDefenseMultiplier, 1f);
            attributes.SetMultiplier(BaseEntity.ATTRIBUTE_SkillsMagicalDefenseMultiplier, 1f);

            base.ResetMultipliers();
        }
    }
}
