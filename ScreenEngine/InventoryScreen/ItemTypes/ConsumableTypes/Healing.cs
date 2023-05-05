using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.ConsumableTypes
{
    public class HealingPotion : Consumable
    {
        int healAmount = 10;

        public HealingPotion(Texture2D Icon, int ID, string Name, string Description, int MaxDurability, bool IsEssential, Requirements ItemRequirements, int SellPrice, int HealAmount)
            : base(Icon, ID, Name, Description, 99, MaxDurability, IsEssential, ItemRequirements, SellPrice, "Potion", "Restorative")
        {
            healAmount = HealAmount;

            buttonOneText = "Drink"; //Drink the potion. Normal stuff.
            buttonTwoText = "Sip"; //Sip a potion. Applies a regeneration effect to the player.
            buttonThreeText = "Chug"; //Chug a potion. Reduces time to drink by half, however ti is less effective. Used in emergency situations.
            buttonFourText = "Splash"; //Splash a potion of healing at an entity, entity heals! (useful for allies)

            type = "Potion";
            subType = "Healing";
        }

        public override void ButtonOne()
        {
            if (CurrentAmount > 0)
            {
                currentEntity.HEALTH_Restore(200, true);
                CurrentAmount--;
            }
        }
        public override void ButtonTwo()
        {
            if (CurrentAmount > 0)
            {
                currentEntity.STATUS_AddStatus(1, currentEntity.MapEntityID);
                CurrentAmount--;
            }
        }
        public override void ButtonThree()
        {
            if (CurrentAmount > 0)
            {
                CurrentDurability -= 1;

                if (CurrentDurability <= 0)
                {
                    CurrentAmount--;
                    CurrentDurability = MaxDurability;
                }
            }
        }
        public override void ButtonFour()
        {
            if (CurrentAmount > 0)
            {
                currentEntity.HEALTH_Damage(50, currentEntity.MapEntityID); //Suicide. Will remove soon.
                CurrentAmount--;
            }
        }

        public override void LoadData(string data)
        {
            string[] words = data.Split(' ');

            CurrentAmount = int.Parse(words[1]);
            base.LoadData(data);
        }

    }
}
