using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Archery : BaseSkill
    {
        private float accuracy;
        public float Accuracy { get { return accuracy * AccuracyMultiplier; } }

        public float AccuracyMultiplier { get; set; }

        public Archery(int Level) : base(Level, "The eyesight and steadiness of a being. Governs the Accuracy stat.") { }
        public override void AssignValues()
        {
            accuracy = MathHelper.Clamp(1f - ((float)Level / 150), .1f, 1.5f);
            base.AssignValues();
        }
        public override void ResetMultipliers()
        {
            AccuracyMultiplier = 1f;

            base.ResetMultipliers();
        }
    }
}
