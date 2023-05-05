using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    public class EquipWeapon : BaseGoal
    {
        private string searchType, searchSubType;
        private int weaponIndex;

        public EquipWeapon(string GoalName, BaseEntity Entity, DesireHolder Desire, string SearchType, string SearchSubType, int WeaponIndex)
            : base(GoalName, Entity, Desire)
        {
            searchType = SearchType;
            searchSubType = SearchSubType;
            weaponIndex = WeaponIndex;
        }

        private List<BaseItem> weapons = new List<BaseItem>();
        private int damage;

        public override void Initialize()
        {
            weapons.Clear();

            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            if (string.IsNullOrEmpty(searchSubType))
                weapons = entity.STORAGE_ReturnItems(searchType); //Search storage for more ...
            else
                weapons = entity.STORAGE_ReturnItems(searchType, searchSubType);

            Weapon weapon = null;

            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].IsBroken() == false)
                {
                    if (i == 0)
                        damage = ((Weapon)weapons[i]).PhysicalDamage;

                    if (((Weapon)weapons[i]).PhysicalDamage >= damage)
                        weapon = (Weapon)weapons[i];
                }
            }

            if (weapon != null)
            {
                entity.EQUIPMENT_EquipWeapon(weapon, weaponIndex);
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
