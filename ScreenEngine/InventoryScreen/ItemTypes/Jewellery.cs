using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class Jewellery : BaseItem
    {
        public enum JewelleryType
        {
            Ring,
            Amulet
        }
        JewelleryType jewellerySlot;
        public JewelleryType JewellerySlot { get { return jewellerySlot; } }

        private ItemAttribute attributes;
        public ItemAttribute Attributes { get { return attributes; } }

        public Jewellery(Texture2D Icon, int ID, string Name, string Description, JewelleryType JewelleryType, int MaxDurability, bool IsEssential, Requirements ItemRequirements, ItemAttribute Attributes, int SellPrice, string Type, string Subtype)
            : base(Icon, ID, Name, Description, 1, TabType.Jewellery, MaxDurability, IsEssential, ItemRequirements, SellPrice, Type, Subtype)
        {
            jewellerySlot = JewelleryType;
            attributes = Attributes;

            IsMultiStack = true;
        }
    }
}
