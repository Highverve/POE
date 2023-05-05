using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities
{
    public class ItemDrop
    {
        int id, minQuantity, maxQuantity;
        public int ID { get { return id; } }
        public void IncreaseQuantity(int amount) { minQuantity += amount; maxQuantity += amount; }
        public int MinQuantity { get { return minQuantity; } }
        public int MaxQuantity { get { return maxQuantity; } }

        public bool IsDisposable { get; set; }

        float rarity;
        public float Rarity { get { return rarity; } }

        /// <summary>
        /// For use by DropTable only
        /// </summary>
        /// <param name="ID">The ID of the item to add to the drop table. Look at Inventory.ItemDatabase for reference.</param>
        /// <param name="Quantity">How much of this item should there be? Between 1 and 99.</param>
        /// <param name="Rarity">This is a percentage! Any value below or above will be set to the closest correct value (either 0 or 100)</param>
        public ItemDrop(int ID, int Quantity, float RarityPercentage)
        {
            id = ID;

            minQuantity = maxQuantity = Quantity;

            rarity = MathHelper.Clamp(RarityPercentage, 0f, 100f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">The ID of the item to add to the drop table. Look at Inventory.ItemDatabase for reference.</param>
        /// <param name="MinQuantity">Minumum amount for random quantity.</param>
        /// <param name="MaxQuantity">Maximum amount for random quantity.</param>
        /// <param name="RarityPercentage">This is a percentage! Any value below or above will be set to the closest correct value (either 0 or 100)</param>
        public ItemDrop(int ID, int MinQuantity, int MaxQuantity, float RarityPercentage)
        {
            id = ID;

            maxQuantity = (int)MathHelper.Clamp(MaxQuantity, 1, 998);
            minQuantity = (int)MathHelper.Clamp(MinQuantity, 1, MaxQuantity);

            rarity = MathHelper.Clamp(RarityPercentage, 0f, 100f);
        }
    }
}
