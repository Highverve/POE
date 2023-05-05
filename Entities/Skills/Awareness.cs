using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Awareness : BaseSkill
    {
        private float perception;
        public float Perception { get { return perception; } }

        public Awareness(int Level) : base(Level, "The physical, or spiritual, awareness of a being. Governs the Perception stat. [Unimplemented]")
        {
        }

        public override void AssignValues()
        {
            perception = MathHelper.Clamp(.25f + ((float)Level / 200), .1f, 1f);
            base.AssignValues();
        }
    }
}
