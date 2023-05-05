using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Intelligence : BaseSkill
    {
        float magicDamage; //Percentage of magical damage of magic damage dealt, not including spell/equiment bonus damage.
        public float MagicDamage { get { return magicDamage; } }

        public Intelligence(int Level) : base(Level, "The wit and knowledge of a being. Governs the stats of Magic Points and Magical Damage.") { }

        public override void AssignValues()
        {
            magicDamage = MathHelper.Clamp((float)Level / 2, 0, 75);

            base.AssignValues();
        }
    }
}
