using System;
using System.Collections.Generic;
using System.Linq;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.TileEngine.Objects.ContainerTypes;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities
{
    public class EntityLoot
    {
        Random random;

        public int experiencePoints, soulID;

        private List<ItemDrop> dropTable = new List<ItemDrop>();

        private List<BaseItem> items = new List<BaseItem>();
        public List<BaseItem> Items { get { return items; } }

        private ScreenManager screens;
        private TileMap map;
        private BaseEntity entity;
        private Debugging.DebugManager debug;
        private Camera camera;

        public EntityLoot(List<ItemDrop> Table, int ExperiencePoints, int SoulID)
        {
            experiencePoints = ExperiencePoints;
            soulID = SoulID;

            if (Table != null)
                dropTable = Table;
            else
                dropTable = new List<ItemDrop>();

            random = new Random(Guid.NewGuid().GetHashCode());
        }
        public void SetReferences(ScreenManager screens, TileMap map, BaseEntity entity, Debugging.DebugManager debug, Camera camera)
        {
            this.screens = screens;
            this.map = map;
            this.entity = entity;
            this.debug = debug;
            this.camera = camera;
        }

        public void GetRandomChance(ItemDrop item)
        {
            float r = (float)random.NextDouble() * 100f;

            if (r < item.Rarity)
            {
                if (ItemDatabase.ContainsItem(item.ID))
                {
                    items.Add(ItemDatabase.Item(item.ID).Copy(screens, map, entity, camera)); //Yay, item added!
                    items.Last().CurrentAmount = random.Next(item.MinQuantity, item.MaxQuantity + 1);
                }
                else
                {
                    debug.OutputError("Warning: Item ID(" + item.ID + ") does not exist! Did not add to " + entity.Name + "'s drop table!");
                }
            }
        }

        public bool ContainsItems()
        {
            return items.Count > 0; //returns false if the drop table does not contain items!
        }

        public void DropLoot(int currentFloor, string containerTexture, Vector2 entityPosition)
        {
            for (int i = 0; i < dropTable.Count; i++)
                GetRandomChance(dropTable[i]);

            if (ContainsItems())
            {
                if (items.Count > 1) //If there is more than one item...
                    map.AddContainer(new MultiItem(currentFloor, 12f, entity.Name + "'s Items", containerTexture, items, entityPosition));
                else
                    map.AddContainer(new SingleItem(-1, currentFloor, 0f, items[0].ID, items[0].CurrentAmount, entityPosition, false));
            }

            RemoveLateAddedItems();
        }
        public void AddItem(int id, int quantity, float rarityPct)
        {
            bool containsItem = false;

            for (int i = 0; i < dropTable.Count; i++)
            {
                if (dropTable[i].ID == id)
                {
                    containsItem = true;
                    dropTable[i].IncreaseQuantity(quantity);
                    dropTable[i].IsDisposable = true;
                }
            }

            if (containsItem == false)
            {
                dropTable.Add(new ItemDrop(id, quantity, rarityPct));
                dropTable.Last().IsDisposable = true;
            }
        }
        private void RemoveLateAddedItems()
        {
            for (int i = 0; i < dropTable.Count; i++)
            {
                if (dropTable[i].IsDisposable == true)
                    dropTable.Remove(dropTable[i]);
            }
        }

        public EntityLoot Copy(ScreenManager screens, TileMap map, BaseEntity entity, Debugging.DebugManager debug)
        {
            EntityLoot copy = (EntityLoot)this.MemberwiseClone();

            copy.SetReferences(screens, map, entity, debug, camera);
            copy.random = new Random(Guid.NewGuid().GetHashCode());

            copy.dropTable = dropTable;
            copy.items = new List<BaseItem>();

            return copy;
        }
    }
}
