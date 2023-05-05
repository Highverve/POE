using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class Resource : BaseItem
    {
        public Resource(Texture2D Icon, int ID, string Name, string Description, int MaximumQuantity, int MaxDurability, bool IsEssential, Requirements ItemRequirements, int SellPrice, string Type, string Subtype)
            : base(Icon, ID, Name, Description, MaximumQuantity, TabType.Resources, MaxDurability, IsEssential, ItemRequirements, SellPrice, Type, Subtype) { IsMultiStack = false; }
    }
}
