using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.MiscellaneousTypes
{
    public class NumberDice : Miscellaneous
    {
        public NumberDice(Texture2D Icon, int ID, string Name, string Description, int MaxQuantity, int MaxDurability, bool IsDisposable, Requirements ItemRequirements, int SellPrice, string Type, string Subtype)
            : base(Icon, ID, Name, Description, MaxQuantity, MaxDurability, IsDisposable, ItemRequirements, SellPrice, Type, Subtype)
        {
            buttonOneText = "Roll 6";
            buttonTwoText = "Roll 16";
            buttonThreeText = "Roll 50";
            buttonFourText = "Roll 100";
            buttonFiveText = "Roll 1,000";
            buttonSixText = "Roll 10,000";
        }

        public override void ButtonOne()
        {
            if (currentEntity.Skills.intelligence.Level >= ItemRequirements.IntelligenceLevel)
            {
                int value = Random.Next(1, 7);
                currentEntity.CAPTION_Queue("I rolled " + string.Format("{0:#,###0}", value) + ".");
            }
            else
                currentEntity.CAPTION_Queue("I can't count that high...");
        }
        public override void ButtonTwo()
        {
            if (currentEntity.Skills.intelligence.Level >= ItemRequirements.IntelligenceLevel)
            {
                int value = Random.Next(1, 17);
                currentEntity.CAPTION_Queue("I rolled " + string.Format("{0:#,###0}", value) + ".");
            }
            else
                currentEntity.CAPTION_Queue("I can't count that high...");
        }
        public override void ButtonThree()
        {
            if (currentEntity.Skills.intelligence.Level >= ItemRequirements.IntelligenceLevel)
            {
                int value = Random.Next(1, 51);
                currentEntity.CAPTION_Queue("I rolled " + string.Format("{0:#,###0}", value) + ".");
            }
            else
                currentEntity.CAPTION_Queue("I can't count that high...");
        }
        public override void ButtonFour()
        {
            if (currentEntity.Skills.intelligence.Level >= ItemRequirements.IntelligenceLevel)
            {
                int value = Random.Next(1, 101);
                currentEntity.CAPTION_Queue("I rolled " + string.Format("{0:#,###0}", value) + ".");
            }
            else
                currentEntity.CAPTION_Queue("I can't count that high...");
        }
        public override void ButtonFive()
        {
            if (currentEntity.Skills.intelligence.Level >= ItemRequirements.IntelligenceLevel)
            {
                int value = Random.Next(1, 1001);
                currentEntity.CAPTION_Queue("I rolled " + string.Format("{0:#,###0}", value) + ".");
            }
            else
                currentEntity.CAPTION_Queue("I can't count that high...");
        }
        public override void ButtonSix()
        {
            if (currentEntity.Skills.intelligence.Level >= ItemRequirements.IntelligenceLevel)
            {
                int value = Random.Next(1, 10001);
                currentEntity.CAPTION_Queue("I rolled " + string.Format("{0:#,###0}", value) + ".");
            }
            else
                currentEntity.CAPTION_Queue("I can't count that high...");
        }

    }
}
