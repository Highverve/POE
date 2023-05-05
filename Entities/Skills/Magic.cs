using System.Linq;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Wisdom : BaseSkill
    {
        int currentEnergy, maxEnergy;
        float negPotionEffect, posPotionEffect;

        public int CurrentEnergy { get { return currentEnergy; } set { currentEnergy = value; } }
        public int MaxEnergy { get { return maxEnergy; } }

        public float NegativePotionEffect { get { return negPotionEffect; } }
        public float PositivePotionEffect { get { return posPotionEffect; } }

        public float NegativePotionMultiplier { get; set; }
        public float PositivePotionMultiplier { get; set; }

        private Point[] spellSlotLevels = new Point[]
        {
            new Point(10, 0),
            new Point(15, 1),
            new Point(25, 2),
            new Point(35, 3),
            new Point(45, 4),
            new Point(60, 5),
            new Point(75, 6),
            new Point(1000, 7)
        };

        public int SpellSlots()
        {
            for (int i = 0; i < spellSlotLevels.Length; i++)
            {
                if (Level < spellSlotLevels[i].X)
                    return spellSlotLevels[i].Y;
            }

            return spellSlotLevels.Last().Y; //If none of the above are met, return the highest spell slot available. Just in case.
        }

        public Wisdom(int Level)
            : base(Level, "The mystical energy of a being. Governs the stats of Magic Points, Positive Brew Potency, Negative Brew Potency, and Spell Slots.")
        {
        }

        public override void AssignValues()
        {
            maxEnergy = MathHelper.Clamp((Level + skills.intelligence.Level) * 10, 20, 3000);
            currentEnergy = MaxEnergy;

            posPotionEffect = MathHelper.Clamp(.75f + ((float)Level / 200), .5f, 1.5f);
            negPotionEffect = MathHelper.Clamp(.75f + ((float)Level / 200), .5f, 1.5f);

            base.AssignValues();
        }
        public override void ResetMultipliers()
        {
            NegativePotionMultiplier = 1f;
            PositivePotionMultiplier = 1f;

            base.ResetMultipliers();
        }

        public void UseCharge(uint value)
        {
            currentEnergy -= (int)value;

            if (currentEnergy < 0)
                currentEnergy = 0;
        }
        public void RestoreCharge(uint value)
        {
            currentEnergy += (int)value;

            if (currentEnergy > MaxEnergy)
                currentEnergy = MaxEnergy;
        }
    }
}
