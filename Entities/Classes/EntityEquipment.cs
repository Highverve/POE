using System.Collections.Generic;
using System.Text;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Souls.Types;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.Souls;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.CombatEngine;
using Pilgrimage_Of_Embers.Performance;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;

namespace Pilgrimage_Of_Embers.Entities
{
    public class EntityEquipment
    {
        List<EquipSlot> quickSlots = new List<EquipSlot>(10) { new EquipSlot(), new EquipSlot(), new EquipSlot(), new EquipSlot(), new EquipSlot(),
                                                               new EquipSlot(), new EquipSlot(), new EquipSlot(), new EquipSlot(), new EquipSlot() };
        public List<EquipSlot> QuickSlots { get { return quickSlots; } }

        public enum QSlotSelect
        {
            One = 0,
            Two = 1,
            Three = 2,
            Four = 3,
            Five = 4,
            Six = 5,
            Seven = 6,
            Eight = 7,
            Nine = 8,
            Ten = 9
        }
        public QSlotSelect selectedQuickSlot = QSlotSelect.One;
        int numberSlot = 0;
        public int NumberSlot { get { return numberSlot; } }

        private CallLimiter limitEmptyCheck = new CallLimiter(200);

        //Weapon variables
        public enum WeaponSlot { One, Two, Three }
        WeaponSlot primaryWeaponIndex = WeaponSlot.One, offhandWeaponIndex = WeaponSlot.One;

        EquipSlot weaponOne = new EquipSlot(), weaponTwo = new EquipSlot(), weaponThree = new EquipSlot(); //Swords, staves, bows, flasks, etc.
        EquipSlot offhandOne = new EquipSlot(), offhandTwo = new EquipSlot(), offhandThree = new EquipSlot(); //Shields, books, orbs, etc or for dual wielding!
        public EquipSlot WeaponOne { get { return weaponOne; } }
        public EquipSlot WeaponTwo { get { return weaponTwo; } }
        public EquipSlot WeaponThree { get { return weaponThree; } }
        public EquipSlot OffhandOne { get { return offhandOne; } }
        public EquipSlot OffhandTwo { get { return offhandTwo; } }
        public EquipSlot OffhandThree { get { return offhandThree; } }
        public int PrimaryWeaponIndex { get { return (int)primaryWeaponIndex + 1; } }
        public int OffhandWeaponIndex { get { return (int)offhandWeaponIndex + 1; } }

        //Armor variables
        EquipSlot headSlot = new EquipSlot(), torsoSlot = new EquipSlot(), legsSlot = new EquipSlot(), feetSlot = new EquipSlot(), handsSlot = new EquipSlot(), capeSlot = new EquipSlot();
        public EquipSlot HeadSlot { get { return headSlot; } }
        public EquipSlot TorsoSlot { get { return torsoSlot; } }
        public EquipSlot LegsSlot { get { return legsSlot; } }
        public EquipSlot FeetSlot { get { return feetSlot; } }
        public EquipSlot HandsSlot { get { return handsSlot; } }
        public EquipSlot CapeSlot { get { return capeSlot; } }

        //Ammo variables
        EquipSlot primaryAmmo1 = new EquipSlot(), primaryAmmo2 = new EquipSlot(), primaryAmmo3 = new EquipSlot(), primaryAmmo4 = new EquipSlot();
        public EquipSlot PrimaryAmmo1 { get { return primaryAmmo1; } }
        public EquipSlot PrimaryAmmo2 { get { return primaryAmmo2; } }
        public EquipSlot SecondaryAmmo1 { get { return primaryAmmo3; } }
        public EquipSlot SecondaryAmmo2 { get { return primaryAmmo4; } }
        private int ammoIndex = 1;

        //Jewellery variables
        EquipSlot neckSlot = new EquipSlot(), ring1Slot = new EquipSlot(), ring2Slot = new EquipSlot(), ring3Slot = new EquipSlot(), ring4Slot = new EquipSlot();
        public EquipSlot NeckSlot { get { return neckSlot; } }
        public EquipSlot Ring1Slot { get { return ring1Slot; } }
        public EquipSlot Ring2Slot { get { return ring2Slot; } }
        public EquipSlot Ring3Slot { get { return ring3Slot; } }
        public EquipSlot Ring4Slot { get { return ring4Slot; } }

        //Spell variables
        SpellSlot spellOne = new SpellSlot(), spellTwo = new SpellSlot(), spellThree = new SpellSlot(),
                  spellFour = new SpellSlot(), spellFive = new SpellSlot(), spellSix = new SpellSlot(), spellSeven = new SpellSlot();
        public SpellSlot SpellOne { get { return spellOne; } }
        public SpellSlot SpellTwo { get { return spellTwo; } }
        public SpellSlot SpellThree { get { return spellThree; } }
        public SpellSlot SpellFour { get { return spellFour; } }
        public SpellSlot SpellFive { get { return spellFive; } }
        public SpellSlot SpellSix { get { return spellSix; } }
        public SpellSlot SpellSeven { get { return spellSeven; } }
        private int spellIndex = 1;
        public int MaximumSpellSlots() { return currentEntity.MAGIC_SpellSlots(); }

        //Soul variables
        public enum SoulSlot { One, Two, Three, Four }
        private SoulSlot currentSoulSlot = SoulSlot.One;

        BaseSoul soulOne, soulTwo, soulThree, soulFour;
        public BaseSoul SoulOne { get { return soulOne; } }
        public BaseSoul SoulTwo { get { return soulTwo; } }
        public BaseSoul SoulThree { get { return soulThree; } }
        public BaseSoul SoulFour { get { return soulFour; } }

        public BaseSoul SelectedSoul()
        {
            switch (currentSoulSlot)
            {
                case SoulSlot.One: return soulOne;
                case SoulSlot.Two: return soulTwo;
                case SoulSlot.Three: return soulThree;
                case SoulSlot.Four: return soulFour;
            }

            return null; //This will never return null. Just sayin', VS...
        }

        private ScreenManager screens;
        private EntityStorage storage;
        private BaseEntity currentEntity;

        public EntityEquipment() { }
        public void SetReferences(ScreenManager Screens, EntityStorage Storage, BaseEntity CurrentEntity)
        {
            this.screens = Screens;
            this.storage = Storage;
            this.currentEntity = CurrentEntity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot">The weapon to be referenced.</param>
        /// <param name="index">A value between one and six. First three are primary hand, last three are offhand.</param>
        public void EquipWeapon(int index, Weapon slot)
        {
            if (slot != null)
            {
                index = MathHelper.Clamp(index, 1, 6);

                VerifyWeapons(slot.UniqueID); //If the player is trying to equip two of the same unique ID weapon, remove the earlier assigned one.

                if (index == 1) { weaponOne.item = slot; slot.CombatWeapon.SetButtons(Controls.MouseButton.LeftClick, Controls.MouseButton.RightClick); } //Left-hand mouse button assignment for dual-wielding compatability.
                else if (index == 2) { weaponTwo.item = slot; slot.CombatWeapon.SetButtons(Controls.MouseButton.LeftClick, Controls.MouseButton.RightClick); }
                else if (index == 3) { weaponThree.item = slot; slot.CombatWeapon.SetButtons(Controls.MouseButton.LeftClick, Controls.MouseButton.RightClick); }
                else if (index == 4) { offhandOne.item = slot; slot.CombatWeapon.SetButtons(Controls.MouseButton.RightClick, Controls.MouseButton.LeftClick); } //Right hand mouse button assignment for dual-wielding compatability.
                else if (index == 5) { offhandTwo.item = slot; slot.CombatWeapon.SetButtons(Controls.MouseButton.RightClick, Controls.MouseButton.LeftClick); }
                else if (index == 6) { offhandThree.item = slot; slot.CombatWeapon.SetButtons(Controls.MouseButton.RightClick, Controls.MouseButton.LeftClick); }
            }
        }
        private void VerifyWeapons(string uniqueID)
        {
            if (IsWeaponEquipped(uniqueID)) //If the weapon slots contain the ID, it's true!
            {
                CheckRemove(uniqueID, weaponOne); //If the slot is not null and equals the id, remove it!
                CheckRemove(uniqueID, weaponTwo);
                CheckRemove(uniqueID, weaponThree);
                CheckRemove(uniqueID, offhandOne);
                CheckRemove(uniqueID, offhandTwo);
                CheckRemove(uniqueID, offhandThree);
            }
        }
        private void CheckWeaponDuplicationEquip(int id)
        {
            if (IsWeaponEquipped(id))
            {
                if (weaponOne.item != null)
                {
                    if (weaponOne.item.ID == id)
                    {
                        weaponOne.item = null;
                    }
                }
            }
        }

        public void EquipArmor(Armor armor)
        {
            switch (armor.SlotType)
            {
                case Armor.ArmorType.Head:  headSlot.item = armor;  break;
                case Armor.ArmorType.Torso: torsoSlot.item = armor; break;
                case Armor.ArmorType.Legs: legsSlot.item = armor; break;
                case Armor.ArmorType.Feet: feetSlot.item = armor; break;
                case Armor.ArmorType.Hands: handsSlot.item = armor; break;
                case Armor.ArmorType.Cape: capeSlot.item = armor; break;
            }
        }
        public void EquipAmmo(int index, Ammo ammo)
        {
            index = (int)MathHelper.Clamp(index, 1, 4);

            if (index == 1) { primaryAmmo1.item = ammo; }
            else if (index == 2) { primaryAmmo2.item = ammo; }
            else if (index == 3) { primaryAmmo3.item = ammo; }
            else if (index == 4) { primaryAmmo4.item = ammo; }
        }
        public void EquipJewellery(int index, Jewellery jewellery)
        {
            index = MathHelper.Clamp(index, 1, 5);

            VerifyJewellery(jewellery.UniqueID);

            if (jewellery.JewellerySlot == Jewellery.JewelleryType.Ring)
            {
                if (index == 1) { ring1Slot.item = jewellery; }
                else if (index == 2) { ring2Slot.item = jewellery; }
                else if (index == 3) { ring3Slot.item = jewellery; }
                else if (index == 4) { ring4Slot.item = jewellery; }
            }

            if (jewellery.JewellerySlot == Jewellery.JewelleryType.Amulet)
            {
                if (index == 5) { neckSlot.item = jewellery; }
            }
        }
        private void VerifyJewellery(string uniqueID)
        {
            if (IsJewelleryEquipped(uniqueID)) //If the weapon slots contain the ID, it's true!
            {
                CheckRemove(uniqueID, ring1Slot); //If the slot is not null and equals the id, remove it!
                CheckRemove(uniqueID, ring2Slot);
                CheckRemove(uniqueID, ring3Slot);
                CheckRemove(uniqueID, ring4Slot);
                CheckRemove(uniqueID, neckSlot);
            }
        }
        private void CheckJewelleryDuplicateEquip(int id)
        {
            if (IsWeaponEquipped(id))
            {
                if (weaponOne.item != null)
                {
                    if (weaponOne.item.ID == id)
                    {
                        weaponOne.item = null;
                    }
                }
            }
        }

        /// <summary>
        /// For loading data only!
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <param name="iconID"></param>
        public void EquipQuickSlot(int index, BaseItem item, BaseItem iconID)
        {
            index = MathHelper.Clamp(index, 0, 9);

            quickSlots[index].item = item;
            quickSlots[index].ID = iconID.ID;
            quickSlots[index].Icon = iconID.Icon;
        }
        public void EquipQuickSlot(int index, BaseItem item)
        {
            index = MathHelper.Clamp(index, 0, 9);

            quickSlots[index].item = item;
            quickSlots[index].ID = item.ID;
            quickSlots[index].Icon = item.Icon;
        }
        public void EquipMagicSlot()
        {
        }

        // [Methods] Consumable
        public void SelectQuickSlot(int index)
        {
            selectedQuickSlot = (QSlotSelect)index;
            numberSlot = index;
        }
        public void AdjustQuickSlot(int direction) //For gamepads and their ilk!
        {
            numberSlot += direction;
            
            //Loop back around
            if (numberSlot > 9)
                numberSlot = 0;
            else if (numberSlot < 0)
                numberSlot = 9;

            selectedQuickSlot = (QSlotSelect)numberSlot;
        }
        public void ChangeButtonTarget(int direction)
        {
            direction = (int)MathHelper.Clamp(direction, -1, 1);
            AdjustButtonTarget(numberSlot, direction);
        }
        private void AdjustButtonTarget(int index, int value) //For easy management of button targets using keys.
        {
            if (quickSlots[index].item != null)
            {
                quickSlots[index].item.buttonTarget += value;

                if (quickSlots[index].item.buttonTarget < 1)
                    quickSlots[index].item.buttonTarget = quickSlots[index].item.MaxButtonTargets;
                else if (quickSlots[index].item.buttonTarget > quickSlots[index].item.MaxButtonTargets)
                {
                    if (quickSlots[index].item.MaxButtonTargets != 0)
                        quickSlots[index].item.buttonTarget = 1;
                    else
                        quickSlots[index].item.buttonTarget = 0;
                }
            }
        }

        public void UseQuickSlotItem()
        {
            UseQuickSlotItem(numberSlot);
        }
        public void UseQuickSlotItem(int index)
        {
            if (quickSlots[index].item != null && quickSlots[index].item.CurrentAmount > 0) //Double insurance for quick slot usage (currentAmount > 0)
            {
                if (quickSlots[index].item.buttonTarget == 1)
                    quickSlots[index].item.ButtonOne();
                else if (quickSlots[index].item.buttonTarget == 2)
                    quickSlots[index].item.ButtonTwo();
                else if (quickSlots[index].item.buttonTarget == 3)
                    quickSlots[index].item.ButtonThree();
                else if (quickSlots[index].item.buttonTarget == 4)
                    quickSlots[index].item.ButtonFour();
            }
        }

        // [Methods] Weapon
        public Weapon CurrentPrimary()
        {
            if (primaryWeaponIndex == WeaponSlot.One)
                return (Weapon)weaponOne.item;
            if (primaryWeaponIndex == WeaponSlot.Two)
                return (Weapon)weaponTwo.item;
            if (primaryWeaponIndex == WeaponSlot.Three)
                return (Weapon)weaponThree.item;

            return (Weapon)weaponOne.item;
        }
        public Weapon CurrentOffhand()
        {
            if (offhandWeaponIndex == WeaponSlot.One)
                return (Weapon)offhandOne.item;
            if (offhandWeaponIndex == WeaponSlot.Two)
                return (Weapon)offhandTwo.item;
            if (offhandWeaponIndex == WeaponSlot.Three)
                return (Weapon)offhandThree.item;

            return (Weapon)offhandOne.item;
        }

        public BaseCombat CurrentPrimaryWeapon()
        {
            if (CurrentPrimary() != null)
            {
                if (CurrentPrimary().CombatWeapon != null)
                    return CurrentPrimary().CombatWeapon;
            }

            return null;
        }
        public BaseCombat CurrentOffhandWeapon()
        {
            if (CurrentOffhand() != null)
            {
                if (CurrentOffhand().CombatWeapon != null)
                    return CurrentOffhand().CombatWeapon;
            }

            return null;
        }

        public void SwitchPrimary(WeaponSlot slot)
        {
            primaryWeaponIndex = slot;
        }
        public void SwitchOffhand(WeaponSlot slot)
        {
            offhandWeaponIndex = slot;
        }
        public void AdjustPrimary(int direction)
        {
            int index = (int)primaryWeaponIndex;

            index += direction;

            if (index > 2)
                index = 0;
            else if (index < 0)
                index = 2;

            primaryWeaponIndex = (WeaponSlot)index;
        }
        public void AdjustOffhand(int direction)
        {
            int index = (int)offhandWeaponIndex;

            index += direction;

            if (index > 2)
                index = 0;
            else if (index < 0)
                index = 2;

            offhandWeaponIndex = (WeaponSlot)index;
        }

        // [Methods] Armor
        private enum AttributeType { PhysicalDefense, ProjectileDefense, MagicDefense, Weight, EffectResistance, EffectAmplifier }
        private float GetDefenseValue(EquipSlot slot, AttributeType type)
        {
            if (slot != null)
            {
                if (slot.item != null)
                {
                    switch (type)
                    {
                        case AttributeType.PhysicalDefense: return slot.item.PhysicalDefense();
                        case AttributeType.ProjectileDefense: return slot.item.ProjectileDefense();
                        case AttributeType.MagicDefense: return slot.item.MagicDefense();
                        case AttributeType.Weight: return slot.item.Weight();
                        case AttributeType.EffectResistance: return slot.item.EffectResistance();
                        case AttributeType.EffectAmplifier: return slot.item.EffectAmplifier();
                    }
                }
            }

            return 0;
        }
        private float RetrieveAllAttributeValues(AttributeType type)
        {
            return GetDefenseValue(headSlot, type) + GetDefenseValue(torsoSlot, type) + GetDefenseValue(legsSlot, type) +
                   GetDefenseValue(feetSlot, type) + GetDefenseValue(handsSlot, type) + GetDefenseValue(capeSlot, type) +
                   GetDefenseValue(ring1Slot, type) + GetDefenseValue(ring2Slot, type) + GetDefenseValue(ring3Slot, type) +
                   GetDefenseValue(ring4Slot, type) + GetDefenseValue(neckSlot, type);
        }

        public ItemAttribute GetSlotAttribute(Armor.ArmorType type)
        {
            switch (type)
            {
                case Armor.ArmorType.Head: if (headSlot.item != null) { return headSlot.item.Attributes; } break;
                case Armor.ArmorType.Torso: if (torsoSlot.item != null) { return torsoSlot.item.Attributes; } break;
                case Armor.ArmorType.Legs: if (legsSlot.item != null) { return legsSlot.item.Attributes; } break;
                case Armor.ArmorType.Feet: if (feetSlot.item != null) { return feetSlot.item.Attributes; } break;
                case Armor.ArmorType.Hands: if (handsSlot.item != null) { return handsSlot.item.Attributes; } break;
                case Armor.ArmorType.Cape: if (capeSlot.item != null) { return capeSlot.item.Attributes; } break;
            }

            return null;
        }
        public BaseItem GetArmorItem(Armor.ArmorType type)
        {
            switch (type)
            {
                case Armor.ArmorType.Head: if (headSlot.item != null) { return headSlot.item; } break;
                case Armor.ArmorType.Torso: if (torsoSlot.item != null) { return torsoSlot.item; } break;
                case Armor.ArmorType.Legs: if (legsSlot.item != null) { return legsSlot.item; } break;
                case Armor.ArmorType.Feet: if (feetSlot.item != null) { return feetSlot.item; } break;
                case Armor.ArmorType.Hands: if (handsSlot.item != null) { return handsSlot.item; } break;
                case Armor.ArmorType.Cape: if (capeSlot.item != null) { return capeSlot.item; } break;
            }

            return null;
        }

        private int actionIteration = 0;
        private CallLimiter armorStatLimit = new CallLimiter(1000), actionLimit = new CallLimiter(1000);
        public void UpdateAttributes(GameTime gt)
        {
            if (armorStatLimit.IsCalling(gt))
                CalculateArmorStats();

            if (actionLimit.IsCalling(gt))
            {
                CallArmorAction(headSlot);
                CallArmorAction(torsoSlot);
                CallArmorAction(legsSlot);
                CallArmorAction(feetSlot);
                CallArmorAction(handsSlot);
                CallArmorAction(capeSlot);

                CallJewelleryAction(ring1Slot);
                CallJewelleryAction(ring2Slot);
                CallJewelleryAction(ring3Slot);
                CallJewelleryAction(ring4Slot);
                CallJewelleryAction(neckSlot);
            }
        }
        private void CalculateArmorStats()
        {
            currentEntity.ATTRIBUTE_SetMultiplier(BaseEntity.ATTRIBUTE_EquipmentPhysicalDefense, RetrieveAllAttributeValues(AttributeType.PhysicalDefense));
            currentEntity.ATTRIBUTE_SetMultiplier(BaseEntity.ATTRIBUTE_EquipmentProjectileDefense, RetrieveAllAttributeValues(AttributeType.ProjectileDefense));
            currentEntity.ATTRIBUTE_SetMultiplier(BaseEntity.ATTRIBUTE_EquipmentMagicalDefense, RetrieveAllAttributeValues(AttributeType.MagicDefense));

            currentEntity.ATTRIBUTE_SetMultiplier(BaseEntity.ATTRIBUTE_EquipmentEffectAmplifier, RetrieveAllAttributeValues(AttributeType.EffectAmplifier));
            currentEntity.ATTRIBUTE_SetMultiplier(BaseEntity.ATTRIBUTE_EquipmentEffectResistance, RetrieveAllAttributeValues(AttributeType.EffectResistance));
            currentEntity.ATTRIBUTE_SetMultiplier(BaseEntity.ATTRIBUTE_EquipmentWeight, RetrieveAllAttributeValues(AttributeType.Weight));
        }
        private void CallArmorAction(EquipSlot slot)
        {
            if (slot != null && slot.item != null && slot.item is Armor)
            {
                if (((Armor)slot.item).Attributes != null)
                {
                    if (((Armor)slot.item).Attributes.Action != null)
                    {
                        ((Armor)slot.item).Attributes.Action.Invoke(currentEntity, actionIteration);
                    }
                }
            }
        }

        private void CallJewelleryAction(EquipSlot slot)
        {
            if (slot != null && slot.item != null && slot.item is Jewellery)
            {
                if (((Jewellery)slot.item).Attributes != null)
                {
                    if (((Jewellery)slot.item).Attributes.Action != null)
                    {
                        ((Jewellery)slot.item).Attributes.Action.Invoke(currentEntity, actionIteration);
                    }
                }
            }
        }

        // [Methods] Ammo
        public Ammo CurrentPrimaryAmmo()
        {
            switch (ammoIndex)
            {
                case 1: return (Ammo)primaryAmmo1.item;
                case 2: return (Ammo)primaryAmmo2.item;
                case 3: return (Ammo)primaryAmmo3.item;
                case 4: return (Ammo)primaryAmmo4.item;
            }

            return (Ammo)primaryAmmo1.item; //If outside of bounds, return primary ammo 1 (default)
        }
        public void AdjustAmmo(int direction)
        {
            ammoIndex += direction;

            if (ammoIndex > 4)
                ammoIndex = 1;
            else if (ammoIndex < 1)
                ammoIndex = 4;
        }

        // [Methods] Spells
        public BaseSpell CurrentSpell()
        {
            switch (spellIndex)
            {
                case 1: return spellOne.spell;
                case 2: return spellTwo.spell;
                case 3: return spellThree.spell;
                case 4: return spellFour.spell;
                case 5: return spellFive.spell;
                case 6: return spellSix.spell;
                case 7: return spellSeven.spell;
            }

            return spellOne.spell; //If outside of bounds, return primary ammo 1 (default)
        }
        public void AdjustSpells(int direction)
        {
            spellIndex += direction;

            if (spellIndex > MaximumSpellSlots())
                spellIndex = 1;
            else if (spellIndex < 1)
                spellIndex = MaximumSpellSlots();
        }
        public void EquipSpell(BaseSpell spell, int index)
        {
            if (spell != null)
            {
                switch(index)
                {
                    case 1: spellOne.spell = spell; break;
                    case 2: spellTwo.spell = spell; break;
                    case 3: spellThree.spell = spell; break;
                    case 4: spellFour.spell = spell; break;
                    case 5: spellFive.spell = spell; break;
                    case 6: spellSix.spell = spell; break;
                    case 7: spellSeven.spell = spell; break;
                }
            }
        }
        public void EquipAvailableSpell(BaseSpell spell)
        {
            if (spellOne.spell == null)
                EquipSpell(spell, 1);
            else
            {
                if (spellTwo.spell == null)
                    EquipSpell(spell, 2);
                else
                {
                    if (spellThree.spell == null)
                        EquipSpell(spell, 3);
                    else
                    {
                        if (spellFour.spell == null)
                            EquipSpell(spell, 4);
                        else
                        {
                            if (spellFive.spell == null)
                                EquipSpell(spell, 5);
                            else
                            {
                                if (spellSix.spell == null)
                                    EquipSpell(spell, 6);
                                else
                                {
                                    if (spellSeven.spell == null)
                                        EquipSpell(spell, 7);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void UnequipSpell(BaseSpell spell)
        {
            if (spellOne.spell != null && spellOne.spell == spell)
                spellOne.spell = null;
            if (spellTwo.spell != null && spellTwo.spell == spell)
                spellTwo.spell = null;
            if (spellThree.spell != null && spellThree.spell == spell)
                spellThree.spell = null;
            if (spellFour.spell != null && spellFour.spell == spell)
                spellFour.spell = null;
            if (spellFive.spell != null && spellFive.spell == spell)
                spellFive.spell = null;
            if (spellSix.spell != null && spellSix.spell == spell)
                spellSix.spell = null;
            if (spellSeven.spell != null && spellSeven.spell == spell)
                spellSeven.spell = null;
        }

        // [Methods] Souls
        public void EquipSoul(BaseSoul soul, SoulSlot slot)
        {
            switch (slot)
            {
                case SoulSlot.One: soulOne = soul; break;
                case SoulSlot.Two: soulTwo = soul; break;
                case SoulSlot.Three: soulThree = soul; break;
                case SoulSlot.Four: soulFour = soul; break;
            }
        }
        public void EquipSoul(int soulID, SoulSlot slot, TileMap map, BaseEntity entity, ScreenManager screens)
        {
            for (int i = 0; i < SoulsDatabase.Souls.Count; i++)
            {
                if (SoulsDatabase.Souls[i].ID == soulID)
                {
                    switch (slot)
                    {
                        case SoulSlot.One: soulOne = SoulsDatabase.Souls[i].DeepCopy(map, entity, screens); break;
                        case SoulSlot.Two: soulTwo = SoulsDatabase.Souls[i].DeepCopy(map, entity, screens); break;
                        case SoulSlot.Three: soulThree = SoulsDatabase.Souls[i].DeepCopy(map, entity, screens); break;
                        case SoulSlot.Four: soulFour = SoulsDatabase.Souls[i].DeepCopy(map, entity, screens); break;
                    }
                }
            }
        }
        public void ActivateSoul(SoulSlot slot)
        {
            switch (slot)
            {
                case SoulSlot.One: soulOne.ActivateSoul(); break;
                case SoulSlot.Two: soulTwo.ActivateSoul(); break;
                case SoulSlot.Three: soulThree.ActivateSoul(); break;
                case SoulSlot.Four: soulFour.ActivateSoul(); break;
            }
        }
        public void ActivateSelectedSoul()
        {
            switch (currentSoulSlot)
            {
                case SoulSlot.One: if (soulOne != null) soulOne.ActivateSoul(); break;
                case SoulSlot.Two: if (soulTwo != null) soulTwo.ActivateSoul(); break;
                case SoulSlot.Three: if (soulThree != null) soulThree.ActivateSoul(); break;
                case SoulSlot.Four: if (soulFour != null) soulFour.ActivateSoul(); break;
            }
        }
        public void ScrollSouls(int direction = 1)
        {
            int value = (int)currentSoulSlot + direction;

            if (value > 3)
                value = 0;
            else if (value < 0)
                value = 3;

            currentSoulSlot = (SoulSlot)value;
        }
        public void ForceCharge(SoulSlot slot)
        {
            switch (slot)
            {
                case SoulSlot.One: soulOne.ForceCharge(); break;
                case SoulSlot.Two: soulTwo.ForceCharge(); break;
                case SoulSlot.Three: soulThree.ForceCharge(); break;
                case SoulSlot.Four: soulFour.ForceCharge(); break;
            }
        }

        public void RemoveEquip(int id)
        {
            for (int i = 0; i < quickSlots.Count; i++)
                CheckRemove(id, quickSlots[i]);

            CheckRemove(id, weaponOne);
            CheckRemove(id, weaponTwo);
            CheckRemove(id, weaponThree);
            CheckRemove(id, offhandOne);
            CheckRemove(id, offhandTwo);
            CheckRemove(id, offhandThree);

            CheckRemove(id, headSlot);
            CheckRemove(id, torsoSlot);
            CheckRemove(id, legsSlot);
            CheckRemove(id, feetSlot);
            CheckRemove(id, handsSlot);
            CheckRemove(id, capeSlot);

            CheckRemove(id, primaryAmmo1);
            CheckRemove(id, primaryAmmo2);
            CheckRemove(id, primaryAmmo3);
            CheckRemove(id, primaryAmmo4);

            CheckRemove(id, neckSlot);
            CheckRemove(id, ring1Slot);
            CheckRemove(id, ring2Slot);
            CheckRemove(id, ring3Slot);
            CheckRemove(id, ring4Slot);
        }
        private void CheckRemove(int id, EquipSlot slot)
        {
            if (slot.item != null)
            {
                if (slot.item.ID == id)
                {
                    slot.item = null;
                    slot.ID = -1;
                }
            }
        }
        private void CheckRemove(string uniqueID, EquipSlot slot)
        {
            if (slot.item != null)
            {
                if (slot.item.UniqueID == uniqueID)
                    slot.item = null;
            }
        }

        // [Methods] Equipment Checking

        public bool IsItemEquipped(int id)
        {
            bool isEquipped = false;

            //Weapons
            IsItemEquippedCheck(weaponOne, id, ref isEquipped);
            IsItemEquippedCheck(weaponTwo, id, ref isEquipped);
            IsItemEquippedCheck(weaponThree, id, ref isEquipped);
            IsItemEquippedCheck(offhandOne, id, ref isEquipped);
            IsItemEquippedCheck(offhandTwo, id, ref isEquipped);
            IsItemEquippedCheck(offhandThree, id, ref isEquipped);


            //Armor
            IsItemEquippedCheck(headSlot, id, ref isEquipped);
            IsItemEquippedCheck(torsoSlot, id, ref isEquipped);
            IsItemEquippedCheck(legsSlot, id, ref isEquipped);
            IsItemEquippedCheck(feetSlot, id, ref isEquipped);
            IsItemEquippedCheck(handsSlot, id, ref isEquipped);
            IsItemEquippedCheck(capeSlot, id, ref isEquipped);


            //Ammo
            IsItemEquippedCheck(primaryAmmo1, id, ref isEquipped);
            IsItemEquippedCheck(primaryAmmo2, id, ref isEquipped);
            IsItemEquippedCheck(primaryAmmo3, id, ref isEquipped);
            IsItemEquippedCheck(primaryAmmo4, id, ref isEquipped);


            //Jewellery
            IsItemEquippedCheck(neckSlot, id, ref isEquipped);
            IsItemEquippedCheck(ring1Slot, id, ref isEquipped);
            IsItemEquippedCheck(ring2Slot, id, ref isEquipped);
            IsItemEquippedCheck(ring3Slot, id, ref isEquipped);
            IsItemEquippedCheck(ring4Slot, id, ref isEquipped);

            return isEquipped;
        }
        private void IsItemEquippedCheck(EquipSlot item, int id, ref bool isEquipped)
        {
            if (item.item != null)
            {
                if (item.item.ID == id)
                    isEquipped = true;
            }
        }

        public bool IsWeaponEquipped(int id)
        {
            if (IsPrimaryWeaponEquipped(id))
                return true;
            if (IsOffhandWeaponEquipped(id) == true)
                return true;

            return false;
        }
        public bool IsWeaponEquipped(string uniqueID)
        {
            if (IsPrimaryWeaponEquipped(uniqueID))
                return true;
            if (IsOffhandWeaponEquipped(uniqueID) == true)
                return true;

            return false;
        }
        public bool IsPrimaryWeaponEquipped(int id)
        {
            if (weaponOne.item != null) { if (weaponOne.item.ID == id) return true; }
            if (weaponTwo.item != null) { if (weaponTwo.item.ID == id) return true; }
            if (weaponThree.item != null) { if (weaponThree.item.ID == id) return true; }

            return false;
        }
        public bool IsPrimaryWeaponEquipped(string uniqueID)
        {
            if (weaponOne.item != null) { if (weaponOne.item.UniqueID == uniqueID) return true; }
            if (weaponTwo.item != null) { if (weaponTwo.item.UniqueID == uniqueID) return true; }
            if (weaponThree.item != null) { if (weaponThree.item.UniqueID == uniqueID) return true; }

            return false;
        }

        public bool IsOffhandWeaponEquipped(int id)
        {
            if (offhandOne.item != null) { if (offhandOne.item.ID == id) return true; }
            if (offhandTwo.item != null) { if (offhandTwo.item.ID == id) return true; }
            if (offhandThree.item != null) { if (offhandThree.item.ID == id) return true; }

            return false;
        }
        public bool IsOffhandWeaponEquipped(string uniqueID)
        {
            if (offhandOne.item != null) { if (offhandOne.item.UniqueID == uniqueID) return true; }
            if (offhandTwo.item != null) { if (offhandTwo.item.UniqueID == uniqueID) return true; }
            if (offhandThree.item != null) { if (offhandThree.item.UniqueID == uniqueID) return true; }

            return false;
        }

        public bool IsQuickslotEquipped(int id)
        {
            bool isEquipped = false;

            for (int i = 0; i < quickSlots.Count; i++)
            {
                if (quickSlots[i].item != null)
                {
                    if (quickSlots[i].item.ID == id)
                        isEquipped = true;
                }
            }

            return isEquipped;
        }

        public bool IsSoulEquipped(int id)
        {
            bool isEquipped = false;

            if (soulOne != null) { if (soulOne.ID == id) isEquipped = true; }
            if (soulTwo != null) { if (soulTwo.ID == id) isEquipped = true; }
            if (soulThree != null) { if (soulThree.ID == id) isEquipped = true; }
            if (soulFour != null) { if (soulFour.ID == id) isEquipped = true; }

            return isEquipped;
        }

        public bool IsJewelleryEquipped(int id)
        {
            if (ring1Slot.item != null) { if (ring1Slot.ID == id) return true; } 
            if (ring2Slot.item != null) { if (ring2Slot.ID == id) return true; }
            if (ring3Slot.item != null) { if (ring3Slot.ID == id) return true; }
            if (ring4Slot.item != null) { if (ring4Slot.ID == id) return true; }
            if (neckSlot.item != null) { if (neckSlot.ID == id) return true; }

            return false;
        }
        public bool IsJewelleryEquipped(string uniqueID)
        {
            if (ring1Slot.item != null) { if (ring1Slot.item.UniqueID == uniqueID) return true; }
            if (ring2Slot.item != null) { if (ring2Slot.item.UniqueID == uniqueID) return true; }
            if (ring3Slot.item != null) { if (ring3Slot.item.UniqueID == uniqueID) return true; }
            if (ring4Slot.item != null) { if (ring4Slot.item.UniqueID == uniqueID) return true; }
            if (neckSlot.item != null) { if (neckSlot.item.UniqueID == uniqueID) return true; }

            return false;
        }

        public void Update(GameTime gt)
        {
            if (soulOne != null)
                soulOne.Update(gt);
            if (soulTwo != null)
                soulTwo.Update(gt);
            if (soulThree != null)
                soulThree.Update(gt);
            if (soulFour != null)
                soulFour.Update(gt);

            if (limitEmptyCheck.IsCalling(gt) == true)
                EnsureAmount();

            UpdateAttributes(gt);
        }
        private void EnsureAmount()
        {
            CheckItemAmountEmpty(primaryAmmo1);
            CheckItemAmountEmpty(primaryAmmo2);
            CheckItemAmountEmpty(primaryAmmo3);
            CheckItemAmountEmpty(primaryAmmo4);

            CheckItemAmountEmpty(weaponOne);
            CheckItemAmountEmpty(weaponTwo);
            CheckItemAmountEmpty(weaponThree);
            CheckItemAmountEmpty(offhandOne);
            CheckItemAmountEmpty(offhandTwo);
            CheckItemAmountEmpty(offhandThree);
        }
        private void CheckItemAmountEmpty(EquipSlot slot)
        {
            if (slot.item != null)
            {
                if (slot.item.CurrentAmount <= 0)
                    slot.item = null;
            }
        }

        public void UpdateShortcuts(GameTime gt, Controls controls)
        {
            //Weapon switching
            if (currentEntity.SUSPENSION_Action == Suspension.SuspendState.None)
            {
                if (controls.IsKeyPressedOnce(controls.CurrentControls.SwapPrimary))
                {
                    AdjustPrimary(1);

                    //Auto-increase if an empty weapon slot is found
                    if (CurrentPrimaryWeapon() == null)
                        AdjustPrimary(1);

                    if (CurrentPrimaryWeapon() == null)
                        AdjustPrimary(1);

                    if (CurrentPrimaryWeapon() != null)
                        CurrentPrimaryWeapon().ResetCombatState();

                    screens.HUD_ResetEquipLerp();
                    currentEntity.SUSPEND_Action(250);
                }
                if (controls.IsKeyPressedOnce(controls.CurrentControls.SwapOffhand))
                {
                    AdjustOffhand(1);

                    //Auto-increase if an empty weapon slot is found
                    if (CurrentOffhandWeapon() == null)
                        AdjustOffhand(1);

                    if (CurrentOffhandWeapon() == null)
                        AdjustOffhand(1);

                    if (CurrentOffhandWeapon() != null)
                        CurrentOffhandWeapon().ResetCombatState();

                    screens.HUD_ResetEquipLerp();
                    currentEntity.SUSPEND_Action(250);
                }

                //Ammo switching
                if (controls.IsKeyPressedOnce(controls.CurrentControls.SwapAmmo))
                {
                    AdjustAmmo(1);

                    //Auto-increase if an empty ammo slot is found
                    if (CurrentPrimaryAmmo() == null)
                        AdjustAmmo(1);
                    if (CurrentPrimaryAmmo() == null)
                        AdjustAmmo(1);
                    if (CurrentPrimaryAmmo() == null)
                        AdjustAmmo(1);
                    if (CurrentPrimaryAmmo() == null)
                        AdjustAmmo(1);

                    screens.HUD_ResetEquipLerp();
                    currentEntity.SUSPEND_Action(250);
                }

                //Soul using and switching
                if (controls.IsKeyPressedOnce(controls.CurrentControls.UseSelectedSoul))
                {
                    ActivateSelectedSoul();

                    screens.HUD_ResetEquipLerp();
                }
                if (controls.IsKeyPressedOnce(controls.CurrentControls.ScrollSouls))
                {
                    ScrollSouls();

                    screens.HUD_ResetEquipLerp();
                    currentEntity.SUSPEND_Action(250);
                }

                //Spell switching
                if (controls.IsKeyPressedOnce(controls.CurrentControls.ScrollSpells))
                {
                    AdjustSpells(1);

                    for (int i = 0; i < MaximumSpellSlots() - 1; i++)
                    {
                        if (CurrentSpell() == null)
                            AdjustSpells(1);
                        else
                            break;
                    }

                    screens.HUD_ResetEquipLerp();
                    currentEntity.SUSPEND_Action(250);
                }
            }
        }

        // Saving
        public StringBuilder SaveData()
        {
            StringBuilder builder = new StringBuilder();

            for (int a = 0; a < quickSlots.Count; a++)
                if (quickSlots[a].item != null || (quickSlots[a].ID != -1 && quickSlots[a].Icon != null)) builder.AppendLine(QuickSlotSaveAssist(a));

            if (weaponOne.item != null) builder.AppendLine("EquipWeapon 1 " + weaponOne.item.ID);
            if (weaponTwo.item != null) builder.AppendLine("EquipWeapon 2 " + weaponTwo.item.ID);
            if (weaponThree.item != null) builder.AppendLine("EquipWeapon 3 " + weaponThree.item.ID);
            if (offhandOne.item != null) builder.AppendLine("EquipWeapon 4 " + offhandOne.item.ID);
            if (offhandTwo.item != null) builder.AppendLine("EquipWeapon 5 " + offhandTwo.item.ID);
            if (offhandThree.item != null) builder.AppendLine("EquipWeapon 6 " + offhandThree.item.ID);

            if (headSlot.item != null) builder.AppendLine("EquipArmor " + headSlot.item.ID);
            if (torsoSlot.item != null) builder.AppendLine("EquipArmor " + torsoSlot.item.ID);
            if (legsSlot.item != null) builder.AppendLine("EquipArmor " + legsSlot.item.ID);
            if (feetSlot.item != null) builder.AppendLine("EquipArmor " + feetSlot.item.ID);
            if (handsSlot.item != null) builder.AppendLine("EquipArmor " + handsSlot.item.ID);
            if (capeSlot.item != null) builder.AppendLine("EquipArmor " + capeSlot.item.ID);

            if (primaryAmmo1.item != null) builder.AppendLine("EquipAmmo 1 " + primaryAmmo1.item.ID);
            if (primaryAmmo2.item != null) builder.AppendLine("EquipAmmo 2 " + primaryAmmo2.item.ID);
            if (primaryAmmo3.item != null) builder.AppendLine("EquipAmmo 3 " + primaryAmmo3.item.ID);
            if (primaryAmmo4.item != null) builder.AppendLine("EquipAmmo 4 " + primaryAmmo4.item.ID);

            if (ring1Slot.item != null) builder.AppendLine("EquipJewellery 1 " + ring1Slot.item.ID);
            if (ring2Slot.item != null) builder.AppendLine("EquipJewellery 2 " + ring2Slot.item.ID);
            if (ring3Slot.item != null) builder.AppendLine("EquipJewellery 3 " + ring3Slot.item.ID);
            if (ring4Slot.item != null) builder.AppendLine("EquipJewellery 4 " + ring4Slot.item.ID);
            if (neckSlot.item != null) builder.AppendLine("EquipJewellery 5 " + neckSlot.item.ID);

            if (spellOne.spell != null) builder.AppendLine("EquipSpell 1 " + spellOne.spell.ID);
            if (spellTwo.spell != null) builder.AppendLine("EquipSpell 2 " + spellTwo.spell.ID);
            if (spellThree.spell != null) builder.AppendLine("EquipSpell 3 " + spellThree.spell.ID);
            if (spellFour.spell != null) builder.AppendLine("EquipSpell 4 " + spellFour.spell.ID);
            if (spellFive.spell != null) builder.AppendLine("EquipSpell 5 " + spellFive.spell.ID);
            if (spellSix.spell != null) builder.AppendLine("EquipSpell 6 " + spellSix.spell.ID);
            if (spellSeven.spell != null) builder.AppendLine("EquipSpell 7 " + spellSeven.spell.ID);

            if (soulOne != null) builder.AppendLine("EquipSoul 1 " + soulOne.ID);
            if (soulTwo != null) builder.AppendLine("EquipSoul 2 " + soulTwo.ID);
            if (soulThree != null) builder.AppendLine("EquipSoul 3 " + soulThree.ID);
            if (soulFour != null) builder.AppendLine("EquipSoul 4 " + soulFour.ID);

            return builder;
        }
        private string QuickSlotSaveAssist(int index) { return "Quickslot " + index + " " + quickSlots[index].ID;  }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string[] words = data[i].Split(' ');

                if (words[0].ToUpper().Equals("QUICKSLOT"))
                {
                    int index = int.Parse(words[1]);
                    int id = int.Parse(words[2]);

                    EquipQuickSlot(index, storage.GetItem(id), ItemDatabase.Item(id));
                }
                if (words[0].ToUpper().Equals("EQUIPWEAPON"))
                {
                    int index = int.Parse(words[1]);

                    Weapon weapon = (Weapon)storage.GetItem(int.Parse(words[2]));

                    if (weapon != null)
                        EquipWeapon(index, weapon);
                }

                if (words[0].ToUpper().Equals("EQUIPARMOR"))
                    EquipArmor((Armor)storage.GetItem(int.Parse(words[1])));

                if (words[0].ToUpper().Equals("EQUIPAMMO"))
                {
                    int index = int.Parse(words[1]);
                    EquipAmmo(index, (Ammo)storage.GetItem(int.Parse(words[2])));
                }

                if (words[0].ToUpper().Equals("EQUIPJEWELLERY"))
                {
                    int index = int.Parse(words[1]);
                    EquipJewellery(index, (Jewellery)storage.GetItem(int.Parse(words[2])));
                }

                if (words[0].ToUpper().Equals("EQUIPSPELL"))
                {
                    int index = int.Parse(words[1]);

                    switch (index)
                    {
                        case 1: EquipSpell(storage.SpellByID(int.Parse(words[2])), 1); break;
                        case 2: EquipSpell(storage.SpellByID(int.Parse(words[2])), 2); break;
                        case 3: EquipSpell(storage.SpellByID(int.Parse(words[2])), 3); break;
                        case 4: EquipSpell(storage.SpellByID(int.Parse(words[2])), 4); break;
                        case 5: EquipSpell(storage.SpellByID(int.Parse(words[2])), 5); break;
                        case 6: EquipSpell(storage.SpellByID(int.Parse(words[2])), 6); break;
                        case 7: EquipSpell(storage.SpellByID(int.Parse(words[2])), 7); break;
                    }
                }

                if (words[0].ToUpper().Equals("EQUIPSOUL"))
                {
                    int index = int.Parse(words[1]);

                    switch (index)
                    {
                        case 1: soulOne = screens.SOULS_Retrieve(int.Parse(words[2])); break;
                        case 2: soulTwo = screens.SOULS_Retrieve(int.Parse(words[2])); break;
                        case 3: soulThree = screens.SOULS_Retrieve(int.Parse(words[2])); break;
                        case 4: soulFour = screens.SOULS_Retrieve(int.Parse(words[2])); break;
                    }
                }
            }

            CalculateArmorStats();
        }

        public EntityEquipment Copy()
        {
            EntityEquipment copy = (EntityEquipment)MemberwiseClone();

            copy.weaponOne = new EquipSlot();
            copy.weaponTwo = new EquipSlot();
            copy.weaponThree = new EquipSlot();
            copy.offhandOne = new EquipSlot();
            copy.offhandTwo = new EquipSlot();
            copy.offhandThree = new EquipSlot();

            copy.headSlot = new EquipSlot();
            copy.torsoSlot = new EquipSlot();
            copy.legsSlot = new EquipSlot();
            copy.handsSlot = new EquipSlot();
            copy.feetSlot = new EquipSlot();
            copy.capeSlot = new EquipSlot();

            copy.primaryAmmo1 = new EquipSlot();
            copy.primaryAmmo2 = new EquipSlot();
            copy.primaryAmmo3 = new EquipSlot();
            copy.primaryAmmo4 = new EquipSlot();

            copy.ring1Slot = new EquipSlot();
            copy.ring2Slot = new EquipSlot();
            copy.ring3Slot = new EquipSlot();
            copy.ring4Slot = new EquipSlot();
            copy.neckSlot = new EquipSlot();

            return copy;
        }
    }
}
