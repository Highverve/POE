using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.CombatEngine.Projectiles;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class EquipAmmo : BaseGoal
    {
        public EquipAmmo(string GoalName, BaseEntity Entity, DesireHolder Desire)
            : base(GoalName, Entity, Desire)
        {
        }

        private List<BaseItem> ammos = new List<BaseItem>();
        private uint damage;

        public override void Initialize()
        {
            ammos.Clear();

            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            ammos = entity.STORAGE_ReturnItems("Ammo"); //Search storage for more ...

            Ammo ammo = null;

            for (int i = 0; i < ammos.Count; i++)
            {
                if (i == 0)
                    damage = ProjectileDatabase.Projectile(((Ammo)ammos[i]).ProjectileID).TotalDamage;

                if (ProjectileDatabase.Projectile(((Ammo)ammos[i]).ProjectileID).TotalDamage >= damage)
                    ammo = (Ammo)ammos[i];
            }

            if (ammo != null) //If the entity found more ammo in storage, equip it!
            {
                entity.EQUIPMENT_EquipAmmo(ammo, 1);
                state = GoalState.Completed;
            }
            else
                state = GoalState.Failed; //If not, this task should be terminated.

            base.Update(gt);
        }
        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
