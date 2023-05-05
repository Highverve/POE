using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Endurance : BaseSkill
    {
        int maxStamina;
        float currentStamina, regenMultiplier = 1.0f;

        public float CurrentStamina { get { return currentStamina; } set { currentStamina = value; } }
        public int MaxStamina { get { return maxStamina; } }

        public float RegenerationMultiplier { get { return regenMultiplier; } }

        public bool IsEmpty
        {
            get
            {
                if (currentStamina < 1f)
                    return true;
                else
                    return false;
            }
        }

        public Endurance(int Level) : base(Level, "The physical endurance of a being. Governs the Endurance Points stat.") { }

        public override void AssignValues()
        {
            maxStamina = (int)MathHelper.Clamp(100 + (Level), 50, 300);
            currentStamina = MathHelper.Clamp(MaxStamina, 0.0f, MaxStamina);
            regenMultiplier = MathHelper.Clamp(30f * (.5f + ((float)Level / 200)), 10f, 50f);

            base.AssignValues();
        }

        public void DamageStamina(float value) //For actions that use stamina - running, jumping, fighting
        {
            currentStamina -= value;

            if (CurrentStamina < 0)
                currentStamina = 0;
        }
        public void RestoreStamina(float value)
        {
            currentStamina += value;

            if (currentStamina >= MaxStamina)
                currentStamina = MaxStamina;
        }
    }
}
