using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pilgrimage_Of_Embers.ScreenEngine.Soulgate.Crafting
{
    public class ItemHolder
    {
        private int id, quantity;
        private string name;
        private Texture2D icon;

        public int ID { get { return id; } }
        public int Quantity { get { return quantity; } }
        public string Name { get { return name; } }
        public Texture2D Icon { get { return icon; } }

        public Rectangle Rect { get; set; }

        public ItemHolder(int ID, int Quantity, string Name, Texture2D Icon)
        {
            id = ID;
            quantity = Quantity;
            name = Name;
            icon = Icon;
        }
    }
}
