using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public enum WeaponCategory
    {
        None,
        Striker,
        Deflector,
        Shooter,
        Caster,
    }

    public enum StrikerType
    {
        None,
        Dagger,
        Shortsword,
        Longsword,
        Spear,
        Hammer,
        Axe,
        Greatsword,
        Halberd,
        Rapier
    }

    public enum DeflectorType
    {
        None,
        Roundshield,
        Armguard,
        Squareshield,
        Kiteshield,
        Greatshield
    }

    public enum ShooterType
    {
        None,
        Shortbow,
        Longbow,
        Crossbow,
        Arweblast,
        ThrownItem,
        ThrownFlask
    }

    public enum CasterType
    {
        None,
        Shortstaff,
        Longstaff,
        Book
    }

    public class Strength : BaseSkill
    {
        private float meleeDamage;
        private float archeryDamage;

        public float MeleeDamage { get { return (int)(meleeDamage * PhysicalDamageMultiplier); } }
        public float ArcheryDamage { get { return archeryDamage; } }

        public float PhysicalDamageMultiplier { get; set; }
        public float ProjectileDamageMultiplier { get; set; }

        public Strength(int Level) : base(Level, "The strength of a being. Governs the Physical and Projectile Damage stats.") { }
        public override void AssignValues()
        {
            archeryDamage = (int)MathHelper.Clamp((float)Level / 4, 5, 65);
            meleeDamage = (int)MathHelper.Clamp((float)Level / 8, 0, 65);

            base.AssignValues();
        }
        public override void ResetMultipliers()
        {
            PhysicalDamageMultiplier = 1f;
            ProjectileDamageMultiplier = 1f;

            base.ResetMultipliers();
        }

        public float GetPhysicalDamage(int weaponDamage)
        {
            return (weaponDamage + meleeDamage);
        }
        public float GetProjectileDamage(int weaponDamage)
        {
            return (weaponDamage + archeryDamage);
        }
    }
}
