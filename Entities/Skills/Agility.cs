using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Agility : BaseSkill
    {
        float strafeSpeed, sneakSpeed, walkSpeed, sprintSpeed, climbSpeed, swimSpeed, dodgeSpeed, dodgeDistance, attackSpeed;

        public float jumpHeight; //Default is 64 pixels, half a block. Max is 196 pixels, 1.5 blocks. (Without items/boosts)
        public float baseJumpHeight = 3f;

        public float StrafeSpeed { get { return strafeSpeed * MovementMultiplier; } }
        public float SneakSpeed { get { return sneakSpeed * MovementMultiplier; } }
        public float WalkSpeed { get { return walkSpeed * MovementMultiplier; } }
        public float SprintSpeed { get { return sprintSpeed * MovementMultiplier; } }

        public float ClimbSpeed { get { return climbSpeed * MovementMultiplier; } }
        public float SwimSpeed { get { return swimSpeed * MovementMultiplier; } }

        public float DodgeSpeed { get { return dodgeSpeed * MovementMultiplier; } }
        public float DodgeDistance { get { return dodgeDistance; } }

        public float AttackSpeed { get { return attackSpeed * AttackMultiplier; } }

        public float MovementMultiplier { get; set; }
        public float AttackMultiplier { get; set; }
        public float ArmorWeightMultiplier { get; set; }

        public Agility(int Level) : base(Level, "Speed and effieciency of actions. Governs the Attack Speed stat and various movement speeds.") { }

        public override void AssignValues()
        {
            climbSpeed = MathHelper.Clamp(65f + ((float)Level / 5), 0.5f, 200f);
            swimSpeed = MathHelper.Clamp(100f + ((float)Level / 4), .05f, 200f);

            strafeSpeed = 110f + ((float)Level / 3); //Not currently in use

            sneakSpeed = 75f + (Level * .5f); //75 to 125
            walkSpeed = 150f + (Level * .75f); //150 to 225
            sprintSpeed = 250f + Level; //250 to 350

            dodgeSpeed = 400f + (Level * 1.5f); //400f to 550f
            dodgeDistance = 200f + (Level); //200f to 300f

            attackSpeed = MathHelper.Clamp(.75f + ((float)Level / 125), .5f, 2f);

            jumpHeight = MathHelper.Clamp(baseJumpHeight + (Level * .025f), 2f, 7f);

            base.AssignValues();
        }
        public override void ResetMultipliers()
        {
            MovementMultiplier = 1f;
            AttackMultiplier = 1f;
            ArmorWeightMultiplier = 1f;

            base.ResetMultipliers();
        }
    }
}
