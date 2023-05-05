using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Health : BaseSkill
    {
        int currentHP, maxHP, bonusHP;

        public int CurrentHP { get { return currentHP; } set { currentHP = value; } }
        public int MaxHP { get { return maxHP + bonusHP; } }

        public bool IsDead
        {
            get
            {
                if (currentHP <= 0)
                    return true;
                else
                    return false;
            }
        }

        public Health(int Level) : base(Level, "The corporal vitality of a being. Governs the Health Points stat.")
        {
        }

        public override void AssignValues()
        {
            currentHP = maxHP;
            maxHP = MathHelper.Clamp((Level * 10), 10, 2500);

            base.AssignValues();
        }

        public void Damage(uint value)
        {
            currentHP -= (int)value;

            if (currentHP <= 0)
                currentHP = 0;
        }
        public void Restore(uint value)
        {
            currentHP += (int)value;

            if (currentHP >= MaxHP)
                currentHP = MaxHP;
        }

        public void ChangeBonusHP(int value)
        {
            bonusHP = (int)MathHelper.Clamp(value, 0, 1500);
            maxHP = (int)MathHelper.Clamp((Level * 10) + bonusHP, 10, 2500);
        }
    }
}
