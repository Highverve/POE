using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;
using Pilgrimage_Of_Embers.CombatEngine;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.CombatEngine.Types;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class Weapon : BaseItem
    {
        WeaponCategory weaponType;
        StrikerType strikerType;
        DeflectorType deflectorType;
        ShooterType shooterType;
        CasterType casterType;

        protected BaseCombat combatWeapon;

        float attackSpeed, blockingPct;
        int physicalDamage, projectileDamage;

        public WeaponCategory WeaponType { get { return weaponType; } }
        public StrikerType StrikerType { get { return strikerType; } }
        public DeflectorType DeflectorType { get { return deflectorType; } }
        public ShooterType ShooterType { get { return shooterType; } }
        public CasterType CasterType { get { return casterType; } }

        public float AttackSpeed { get { return attackSpeed; } }

        public int PhysicalDamage
        {
            get
            {
                float calc = physicalDamage * CalculateReinforcePct();
                calc *= CurrentReinforcement;
                return (int)((physicalDamage + calc) * DurabilityMultiplier);
            }
        }
        public int ProjectileDamage
        {
            get
            {
                float calc = projectileDamage * CalculateReinforcePct();
                calc *= CurrentReinforcement;
                return (int)((projectileDamage + calc) * DurabilityMultiplier);
            }
        }
        public float BlockingPercentage
        {
            get
            {
                float calc = blockingPct * CalculateReinforcePct();
                calc *= CurrentReinforcement;
                return MathHelper.Clamp(((blockingPct + calc) * DurabilityMultiplier), 0f, 1f);
            }
        }

        public BaseCombat CombatWeapon { get { return combatWeapon; } }

        //Melee: Dagger, Shortsword, Longsword, Spear, BattleHammer, BattleAxe, Halberd, Rapier
        //Shields: Roundshield, Armguard, Squareshield, Kiteshield, Greatshield
        //Archery: Dart, Knife, Axe, Shortbow, Longbow, Crossbow
        //Magic: Shortstaff, Longstaff, Book

        /// <summary>
        /// Striker!
        /// </summary>
        /// <param name="Icon"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="MaxAmount"></param>
        /// <param name="IsEssential"></param>
        /// <param name="CombatWeapon"></param>
        /// <param name="StrikerType"></param>
        /// <param name="Requirements"></param>
        /// <param name="AttackSpeed"></param>
        /// <param name="MaxDurability"></param>
        /// <param name="DamageBonus"></param>
        /// <param name="Type"></param>
        /// <param name="SubType"></param>
        public Weapon(Texture2D Icon, int ID, string Name, string Description, int MaxAmount, bool IsEssential,
                      BaseStriker CombatWeapon, StrikerType StrikerType, Requirements Requirements, int MaxDurability,
                      float AttackSpeed, int PhysicalDamage, int ProjectileDamage, float BlockingPercentage, int SellPrice, string Type, string Subtype) : this
            (Icon, ID, Name, Description, MaxAmount, IsEssential, CombatWeapon, WeaponCategory.Striker, StrikerType,
            DeflectorType.None, ShooterType.None, CasterType.None, Requirements, MaxDurability, AttackSpeed, PhysicalDamage,
            ProjectileDamage, BlockingPercentage, SellPrice, Type, Subtype)
        {
        }

        /// <summary>
        /// Deflector!
        /// </summary>
        /// <param name="Icon"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="MaxAmount"></param>
        /// <param name="IsEssential"></param>
        /// <param name="CombatWeapon"></param>
        /// <param name="DeflectorType"></param>
        /// <param name="Requirements"></param>
        /// <param name="AttackSpeed"></param>
        /// <param name="MaxDurability"></param>
        /// <param name="DamageBonus"></param>
        /// <param name="Type"></param>
        /// <param name="SubType"></param>
        public Weapon(Texture2D Icon, int ID, string Name, string Description, int MaxAmount, bool IsEssential,
                      BaseDeflector CombatWeapon, DeflectorType DeflectorType, Requirements Requirements, int MaxDurability, float AttackSpeed,
                      int PhysicalDamage, int ProjectileDamage, float BlockingPercentage, int SellPrice, string Type, string Subtype) : this
                (Icon, ID, Name, Description, MaxAmount, IsEssential, CombatWeapon, WeaponCategory.Deflector, StrikerType.None,
                DeflectorType, ShooterType.None, CasterType.None, Requirements, MaxDurability, AttackSpeed, PhysicalDamage,
            ProjectileDamage, BlockingPercentage, SellPrice, Type, Subtype)
        {
        }

        /// <summary>
        /// Shooter!
        /// </summary>
        /// <param name="Icon"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="MaxAmount"></param>
        /// <param name="IsEssential"></param>
        /// <param name="CombatWeapon"></param>
        /// <param name="ShooterType"></param>
        /// <param name="Requirements"></param>
        /// <param name="AttackSpeed"></param>
        /// <param name="MaxDurability"></param>
        /// <param name="DamageBonus"></param>
        /// <param name="Type"></param>
        /// <param name="SubType"></param>
        public Weapon(Texture2D Icon, int ID, string Name, string Description, int MaxAmount, bool IsEssential,
                      BaseCombat CombatWeapon, ShooterType ShooterType, Requirements Requirements, int MaxDurability, float AttackSpeed,
                      int PhysicalDamage, int ProjectileDamage, float BlockingPercentage, int SellPrice, string Type, string Subtype) : this
                (Icon, ID, Name, Description, MaxAmount, IsEssential, CombatWeapon, WeaponCategory.Shooter, StrikerType.None,
                DeflectorType.None, ShooterType, CasterType.None, Requirements, MaxDurability, AttackSpeed, PhysicalDamage,
            ProjectileDamage, BlockingPercentage, SellPrice, Type, Subtype)
        {
        }

        /// <summary>
        /// Caster!
        /// </summary>
        /// <param name="Icon"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="MaxAmount"></param>
        /// <param name="IsEssential"></param>
        /// <param name="CombatWeapon"></param>
        /// <param name="CasterType"></param>
        /// <param name="Requirements"></param>
        /// <param name="AttackSpeed"></param>
        /// <param name="MaxDurability"></param>
        /// <param name="DamageBonus"></param>
        /// <param name="Type"></param>
        /// <param name="SubType"></param>
        public Weapon(Texture2D Icon, int ID, string Name, string Description, int MaxAmount, bool IsEssential,
                      BaseCombat CombatWeapon, CasterType CasterType, Requirements Requirements, int MaxDurability, float AttackSpeed,
                      int PhysicalDamage, int ProjectileDamage, float BlockingPercentage, int SellPrice, string Type, string Subtype) : this
                (Icon, ID, Name, Description, MaxAmount, IsEssential, CombatWeapon, WeaponCategory.Caster, StrikerType.None,
                DeflectorType.None, ShooterType.None, CasterType, Requirements, MaxDurability, AttackSpeed, PhysicalDamage,
            ProjectileDamage, BlockingPercentage, SellPrice, Type, Subtype)
        {
        }

        protected Weapon(Texture2D Icon, int ID, string Name, string Description, int MaxAmount, bool IsEssential,
                      BaseCombat CombatWeapon, WeaponCategory WeaponCategory, StrikerType StrikerType, DeflectorType DeflectorType,
                      ShooterType ShooterType, CasterType CasterType, Requirements Requirements, int MaxDurability, float AttackSpeed,
                      int PhysicalDamage, int ProjectileDamage, float BlockingPercentage, int SellPrice, string Type, string SubType)
            : base(Icon, ID, Name, Description, MaxAmount, TabType.Weapons, MaxDurability, IsEssential, Requirements, SellPrice, Type, SubType)
        {
            combatWeapon = CombatWeapon;

            weaponType = WeaponCategory;
            strikerType = StrikerType;
            deflectorType = DeflectorType;
            shooterType = ShooterType;
            casterType = CasterType;

            attackSpeed = AttackSpeed;
            maxDurability = MaxDurability;
            currentDurability = maxDurability;

            physicalDamage = PhysicalDamage;
            projectileDamage = ProjectileDamage;
            blockingPct = BlockingPercentage;

            buttonOneText = "Equip Primary";
            buttonTwoText = "Equip Offhand";

            //If the max amount is one, the player can have duplicate stacks
            //If the max amount is not one, then the player may only hold one item stack.
            IsMultiStack = MaxAmount == 1;
        }

        public override void Load(ContentManager main, ContentManager map)
        {
            if (combatWeapon != null)
                combatWeapon.Load(main, map);

            base.Load(main, map);
        }

        public override void ButtonOne()
        {
            if (combatWeapon != null)
                currentEntity.EQUIPMENT_EquipWeapon(this, currentEntity.EQUIPMENT_PrimaryWeaponIndex);
            base.ButtonOne();
        }
        public override void ButtonTwo()
        {
            if (combatWeapon != null)
                currentEntity.EQUIPMENT_EquipWeapon(this, currentEntity.EQUIPMENT_OffhandWeaponIndex);
            base.ButtonTwo();
        }
        public override void ButtonThree()
        {
            base.ButtonTwo();
        }
        public override void ButtonFour()
        {
            base.ButtonTwo();
        }
        public override void ButtonFive()
        {
            base.ButtonTwo();
        }
        public override void ButtonSix()
        {
            base.ButtonTwo();
        }

        public override BaseItem Copy(ScreenManager screens, TileMap tileMap, BaseEntity currentEntity, Camera camera)
        {
            Weapon copy = (Weapon)base.Copy(screens, tileMap, currentEntity, camera);

            if (combatWeapon != null)
            {
                copy.combatWeapon = combatWeapon.Copy();
                copy.combatWeapon.SetReferences(camera, tileMap, currentEntity);
            }

            return copy;
        }
    }
}