using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class Consumable : BaseItem
    {
        public Consumable(Texture2D Icon, int ID, string Name, string Description, int MaximumQuantity, int MaxDurability, bool IsEssential, Requirements ItemRequirements, int SellPrice, string Type, string Subtype)
            : base(Icon, ID, Name, Description, MaximumQuantity, TabType.Consumables, MaxDurability, IsEssential, ItemRequirements, SellPrice, Type, Subtype)
        {
            IsMultiStack = false;
        }

        /// <summary>
        /// When the player presses the "Use Quickslot" key, what button should be pressed?
        /// </summary>
        /// <param name="value">A value between 1 and 4.</param>
        public void SetQuickButton(int value)
        {
            value = (int)MathHelper.Clamp(value, 1, 4);

            buttonTarget = value;
        }
    }
}
