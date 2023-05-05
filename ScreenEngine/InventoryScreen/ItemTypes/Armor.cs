using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class Armor : BaseItem // Shields/Armor
    {
        public enum ArmorType
        {
            Head,
            Torso,
            Legs,
            Feet,
            Hands,
            Cape
        }
        private ArmorType armorType;
        public ArmorType SlotType { get { return armorType; } }

        public Armor(Texture2D Icon, int ID, string Name, string Description, int MaxDurability, bool IsEssential, ArmorType SlotType, ItemAttribute Attributes, Requirements Requirements, int SellPrice, string Type, string Subtype) //Default constructor - for unstackable weapons
            : base(Icon, ID, Name, Description, 1, TabType.Armor, MaxDurability, IsEssential, Requirements, SellPrice, Type, Subtype)
        {
            armorType = SlotType;
            itemAttributes = Attributes;

            buttonOneText = "Equip";

            IsMultiStack = true;
        }

        protected override string AttributeComparison(EntityEquipment equipment)
        {
            string value = string.Empty;
            BaseItem compareItem = equipment.GetArmorItem(armorType);

            if (compareItem != null)
            {
                value += CompareAttributeText("Physical Defense:", compareItem.PhysicalDefense(), PhysicalDefense(), true) + "\n";
                value += CompareAttributeText("Projectile Defense:", compareItem.ProjectileDefense(), ProjectileDefense(), true) + "\n";
                value += CompareAttributeText("Magic Defense:", compareItem.MagicDefense(), MagicDefense(), true) + "\n\n";
                value += CompareAttributeText("Weight:", compareItem.Weight(), Weight(), true) + "\n";
            }

            return value;
        }

        public override void RefreshAttributeText()
        {
            AttributeText = "Requirements: \n";
            if (ItemRequirements.HealthLevel > 0)
                AttributeText += " " + ItemRequirements.HealthLevel + " Health \n";
            if (ItemRequirements.DefenseLevel > 0)
                AttributeText += " " + ItemRequirements.DefenseLevel + " Defense \n";
            if (ItemRequirements.StrengthLevel > 0)
                AttributeText += " " + ItemRequirements.StrengthLevel + " Strength \n";
            if (ItemRequirements.ArcheryLevel > 0)
                AttributeText += " " + ItemRequirements.ArcheryLevel + " Archery \n";
            if (ItemRequirements.IntelligenceLevel > 0)
                AttributeText += " " + ItemRequirements.IntelligenceLevel + " Intelligence \n";
            if (ItemRequirements.MagicLevel > 0)
                AttributeText += " " + ItemRequirements.MagicLevel + " Magic";

            AttributeText += "Stats: \n [#" + ColorHelper.D_Blue.RGBToHex() + "]Physical Defense:[/#] " + PhysicalDefense() + " \n " +
                            "[#" + ColorHelper.D_Blue.RGBToHex() + "]Projectile Defense:[/#] " + ProjectileDefense() + " \n " +
                            "[#" + ColorHelper.D_Blue.RGBToHex() + "]Magical Defense:[/#] " + MagicDefense() + " \n\n ";

            if (itemAttributes.BaseHealthModifier != 0)
                AttributeText += "[#" + ColorHelper.D_Green.RGBToHex() + "]Health Bonus:[/#] " + itemAttributes.BaseHealthModifier + " \n ";
            if (itemAttributes.BaseStaminaModifier != 0)
                AttributeText += "[#" + ColorHelper.D_Green.RGBToHex() + "]Stamina Bonus:[/#] " + itemAttributes.BaseStaminaModifier + " \n ";
            if (itemAttributes.BaseMagicModifier != 0)
                AttributeText += "[#" + ColorHelper.D_Green.RGBToHex() + "]Magic Bonus:[/#] " + itemAttributes.BaseMagicModifier + " \n\n ";

            if (itemAttributes.BasePhysicalDamageModifier != 0)
                AttributeText += "[#" + ColorHelper.DD_Red.RGBToHex() + "]Physical Damage: [/#]" + itemAttributes.BasePhysicalDamageModifier + " \n ";
            if (itemAttributes.BaseProjectileDamageModifier != 0)
                AttributeText += "[#" + ColorHelper.DD_Red.RGBToHex() + "]Projectile Damage: [/#]" + itemAttributes.BaseProjectileDamageModifier + " \n ";
            if (itemAttributes.BaseSpellDamageModifier != 0)
                AttributeText += "[#" + ColorHelper.DD_Red.RGBToHex() + "]Spell Damage: [/#]" + itemAttributes.BaseSpellDamageModifier + " \n ";

            if (itemAttributes.BaseEffectResistance != 0)
                AttributeText += "\n [#" + ColorHelper.D_Orange.RGBToHex() + "]Effect Resistance: [/#]" + (EffectResistance() * 100) + "% \n ";
            if (itemAttributes.BaseEffectAmplifier != 0)
                AttributeText += "[#" + ColorHelper.D_Orange.RGBToHex() + "]Effect Amplifier: [/#]" + (EffectAmplifier() * 100) + "% \n\n ";

            if (itemAttributes.BaseAccuracyAmplifier != 0)
                AttributeText += "[#" + ColorHelper.DD_Purple.RGBToHex() + "]Accuracy: [/#]" + (itemAttributes.BaseAccuracyAmplifier * 100) + "% \n ";
            if (itemAttributes.BaseConcealmentAmplifier != 0)
                AttributeText += "[#" + ColorHelper.DD_Purple.RGBToHex() + "]Concealment: [/#]" + (itemAttributes.BaseConcealmentAmplifier * 100) + "% \n ";
            if (itemAttributes.BaseAwarenessAmplifier != 0)
                AttributeText += "[#" + ColorHelper.DD_Purple.RGBToHex() + "]Awareness: [/#]" + (itemAttributes.BaseAwarenessAmplifier * 100) + "% \n ";
            if (itemAttributes.BaseSpeedAmplifier != 0)
                AttributeText += "[#" + ColorHelper.DD_Purple.RGBToHex() + "]Movement Speed: [/#]" + (itemAttributes.BaseSpeedAmplifier * 100) + "% \n\n ";

            AttributeText += "[#" + ColorHelper.D_Orange.RGBToHex() + "]Weight: [/#]" + Weight();
        }

        public override void ButtonOne()
        {
            currentEntity.EQUIPMENT_EquipArmor(this);
            base.ButtonOne();
        }
    }
}