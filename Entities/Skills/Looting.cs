using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Looting : BaseSkill
    {
        private float luck, pickpocketing;
        public float Luck { get { return luck; } }
        public float Pickpocketing { get { return pickpocketing; } }

        public Looting(int Level) : base(Level, "[Unimplemented]")
        {
        }

        public override void AssignValues()
        {
            luck = MathHelper.Clamp(.75f + ((float)Level / 400), .25f, 1.5f);
            pickpocketing = MathHelper.Clamp(.75f, + ((float)Level / 200), 1.25f);
             
            base.AssignValues();
        }
    }
}
