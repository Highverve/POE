using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.MiscellaneousTypes
{
    public class NumberHolder : Miscellaneous
    {
        public int Number { get; set; }
        private string numberName;
        private Color nameColor;

        public NumberHolder(Texture2D Icon, int ID, string Name, string Description, int MaxQuantity, int MaxDurability, bool IsDisposable, Requirements ItemRequirements, int SellPrice, string Type, string Subtype, string NumberName, Color NameColor)
            : base(Icon, ID, Name, Description, MaxQuantity, MaxDurability, IsDisposable, ItemRequirements, SellPrice, Type, Subtype)
        {
            numberName = NumberName;
            nameColor = NameColor;
        }

        public override void Load(ContentManager main, ContentManager map)
        {
            IsMultiStack = true;
            base.Load(main, map);
        }

        public override void RefreshAttributeText()
        {
            AttributeText = string.Empty;
            AttributeText += "[#" + nameColor.RGBToHex() + "]" + numberName + ": [/#]" + Number.ToString();
        }

        public override string SaveData()
        {
            return base.SaveData() + " " + Number;
        }
        public override void LoadData(string data)
        {
            string[] words = data.Split(' ');

            try
            {
                Number = int.Parse(words[6]);
            }
            catch
            {
                Logger.AppendLine("Error loading item data (" + data + "). Ensure the data saved is in the expected format and has not been corrupted!");
            }

            base.LoadData(data);
        }
    }
}
