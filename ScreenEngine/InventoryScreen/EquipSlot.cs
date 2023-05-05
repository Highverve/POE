using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen
{
    public class EquipSlot
    {
        public BaseItem item { get; set; }

        public Rectangle slotRect { get; set; }
        public Rectangle deleteRect { get; set; }

        public int ID { get; set; }
        public Microsoft.Xna.Framework.Graphics.Texture2D Icon { get; set; }
    }

    public class SpellSlot
    {
        public BaseSpell spell;

        public Rectangle slotRect { get; set; }
        public Rectangle deleteRect { get; set; }
    }
}
