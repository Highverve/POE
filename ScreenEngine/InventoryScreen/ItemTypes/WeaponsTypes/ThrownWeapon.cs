using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.CombatEngine.Types.Shooters;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.WeaponsTypes
{
    public class ThrownWeapon : Weapon
    {
        int projectileID;
        private Texture2D projectileTexture;

        public int ProjectileID { get { return projectileID; } }
        public Texture2D ProjectileTexture { get { return projectileTexture; } }

        public ThrownWeapon(Texture2D Icon, int ID, string Name, string Description, int MaxAmount, bool IsEssential, ThrownItem Weapon,
                            Requirements Requirements, int MaxDurability, float AttackSpeed, int PhysicalDamage, int ProjectileDamage, float BlockingPercentage, int ProjectileID, Texture2D ProjectileTexture, int SellPrice, string Type, string Subtype)
            : base(Icon, ID, Name, Description, MaxAmount, IsEssential, Weapon, Skills.ShooterType.ThrownItem, Requirements, MaxDurability, AttackSpeed, PhysicalDamage, ProjectileDamage, BlockingPercentage, SellPrice, Type, Subtype)
        {
            projectileID = ProjectileID;
            projectileTexture = ProjectileTexture;

            buttonOneText = "Throw Item";
            buttonTwoText = "Equip Primary";
            buttonThreeText = "Equip Offhand";
        }

        public override void ButtonOne()
        {
            CurrentAmount--;
            currentEntity.ThrowProjectile(projectileID, Random.NextFloat(.85f, 1f), Random.NextFloat(.85f, 1f), .05f, ProjectileDamage);
            currentEntity.SUSPEND_Action(1000);
        }
        public override void ButtonTwo()
        {
            if (combatWeapon != null)
                currentEntity.EQUIPMENT_EquipWeapon(this, currentEntity.EQUIPMENT_PrimaryWeaponIndex);
            base.ButtonOne();
        }
        public override void ButtonThree()
        {
            if (combatWeapon != null)
                currentEntity.EQUIPMENT_EquipWeapon(this, currentEntity.EQUIPMENT_OffhandWeaponIndex);
            base.ButtonTwo();
        }

    }
}
