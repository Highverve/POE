using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Concealment : BaseSkill
    {
        private float stealth, creatureMultiplier;

        public float Stealth { get { return stealth; } }
        public float CreatureMultiplier { get { return creatureMultiplier; } }

        public Concealment(int Level)
            : base(Level, "The veiling and transformation of a being. Governs the stats of Stealth and Creature Efficacy. [Unimplemented]")
        {

        }

        public override void AssignValues()
        {
            stealth = MathHelper.Clamp((float)Level / 125, .1f, 1f);
            creatureMultiplier = MathHelper.Clamp(.75f + ((float)Level / 200), .5f, 1.5f);

            base.AssignValues();
        }
    }
}
