using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.AI.Goal.Type;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Types
{
    public class ArcherBehavior : CompositeGoal
    {
        private EquipAmmo equipAmmo;
        private DesireHolder equipAmmoDesire;

        private EquipWeapon equipWeapon;

        private DesireHolder primaryWeaponDesire, offhandWeaponDesire;
        private UseWeapon primaryWeapon, offhandWeapon;

        private DesireHolder usePotionDesire;
        private UseItem usePotion;


        public ArcherBehavior(string GoalName, BaseEntity Entity, DesireHolder Desire)
            : base(GoalName, Entity, Desire)
        {
        }

        public override void Initialize()
        {
            equipAmmoDesire = new DesireHolder(0);
            equipAmmo = new EquipAmmo("EquipAmmo", entity, equipAmmoDesire);

            equipWeapon = new EquipWeapon("EquipWeapon", entity, new DesireHolder(1000), "Shooter", "Arweblast", 1);

            primaryWeaponDesire = new DesireHolder(50);
            primaryWeapon = new UseWeapon("ShootArweblaster", entity, primaryWeaponDesire, true, 750f, CombatEngine.CombatMove.Basic);

            usePotionDesire = new DesireHolder(0);
            usePotion = new UseItem("UsePotion", entity, usePotionDesire, "Potion", "Healing", 1);

            base.Initialize();
        }

        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            equipAmmoDesire.Desire = (entity.EQUIPMENT_PrimaryAmmo() != null) ? 0 : 100;
            usePotionDesire.Desire = 100 * (entity.Skills.health.CurrentHP / entity.Skills.health.MaxHP);

            if (entity.EQUIPMENT_PrimaryAmmo() == null)
                AddSubgoal(equipAmmo); //Equip ammo

            if (entity.EQUIPMENT_PrimaryWeapon() == null)
                AddSubgoal(equipWeapon); //Equip crossbow

             AddSubgoal(usePotion);

            if (entity.enemyTarget != null && entity.enemyTarget.IsDead == false)
            {
                entity.SENSES_SightDirection = entity.Position.Direction(entity.enemyTarget.Position);
                AddSubgoal(primaryWeapon);
            }
            else
                state = GoalState.Completed;

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
