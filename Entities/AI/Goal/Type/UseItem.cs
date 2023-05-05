using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.AI.Desire;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;

namespace Pilgrimage_Of_Embers.Entities.AI.Goal.Type
{
    class UseItem : BaseGoal
    {
        private string searchType, searchSubType;
        private int buttonIndex;

        public UseItem(string GoalName, BaseEntity Entity, DesireHolder Desire, string SearchType, string SearchSubType, int ButtonIndex)
            : base(GoalName, Entity, Desire)
        {
            searchType = SearchType;
            searchSubType = SearchSubType;
            buttonIndex = ButtonIndex;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gt)
        {
            ActivateIfInactive();

            BaseItem item = null;

            if (string.IsNullOrEmpty(searchSubType))
                item = entity.STORAGE_SearchForItem(searchType); //Search storage for more ...
            else
                item = entity.STORAGE_SearchForItem(searchType, searchSubType);

            if (item != null) //If the entity found more ammo in storage, equip it!
            {
                entity.STORAGE_UseItemButton(item.ID, buttonIndex);
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
