using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class Miscellaneous : BaseItem
    {
        //Similar to resource tab - except for things such as keys, etc.

        public Miscellaneous(Texture2D icon, int id, string name, string description, int MaxDurability, int MaximumQuantity, bool IsEssential, Requirements ItemRequirements, int SellPrice, string Type, string Subtype)
            : base(icon, id, name, description, MaximumQuantity, TabType.Miscellaneous, MaxDurability, IsEssential, ItemRequirements, SellPrice, Type, Subtype)
        {
            IsMultiStack = false;
        }

        public Miscellaneous(Texture2D icon, int id, string name, string description, int MaxDurability, int MaximumQuantity, bool IsEssential, Requirements ItemRequirements, int SellPrice, string Type, string Subtype, bool IsRestRepairsUnbroken, bool IsRestRepairsBroken)
            : base(icon, id, name, description, MaximumQuantity, TabType.Miscellaneous, MaxDurability, IsEssential, ItemRequirements, SellPrice, Type, Subtype)
        {
            IsMultiStack = false;
            this.IsRestRepairsUnbroken = IsRestRepairsUnbroken;
            this.IsRestRepairsBroken = IsRestRepairsBroken;
        }
    }
}
