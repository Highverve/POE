using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.ConsumableTypes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;
using Pilgrimage_Of_Embers.CombatEngine.Types.StrikerTypes.Longswords;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.CombatEngine.Types.DeflectorTypes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.MiscellaneousTypes;
using Pilgrimage_Of_Embers.CombatEngine.Types.Shooters;
using Pilgrimage_Of_Embers.CombatEngine.Projectiles;
using Microsoft.Xna.Framework.Audio;
using Pilgrimage_Of_Embers.AudioEngine;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.WeaponsTypes;
using Pilgrimage_Of_Embers.CombatEngine.Types.Shooters.ThrownItems;
using Pilgrimage_Of_Embers.CombatEngine.Types.Shooters.Crossbows;
using Pilgrimage_Of_Embers.CombatEngine.Types.Casters.Longstaves;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Daggers;
using Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Halberds;
using Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Swords;
using Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Spears;
using Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Rapier;
using Pilgrimage_Of_Embers.CombatEngine.Types.Deflectors;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen
{
    public class ItemDatabase
    {
        static List<BaseItem> items = new List<BaseItem>();
        public static List<BaseItem> Items { get { return items; } }

        private static Dictionary<int, int> idIndex = new Dictionary<int, int>();

        private const string IconDir = "Items/Icons/";
        private const string ConsumableDir = "Consumable/";
        private const string WeaponDir = "Weapons/";
        private const string ArmorDir = "Armor/";
        private const string AmmoDir = "Ammo/";
        private const string JewelleryDir = "Jewellery/";
        private const string ResourceDir = "Resources/";
        private const string MiscDir = "Misc/";
        
        /* How to add items to this database:
         * 
         * ItemsInDatabase.Add(new *ItemType*(c.Load<Texture2D>(IconDir + *ItemType*Dir + "*IconName*"), *Item ID*, "*Item Name*", "*Item Description*",... Additional ItemType Parameters));
         * 
         * Breakdown:
         * 
         * ItemTypes:  Consumable, Weapon, Armor, Ammo, Jewellery, Resource, Miscelleneous
         * Icon Directory: A fixed variable which leads to the icon directory (I.E, "Content/Interface/ItemIcons/")
         * *ItemType*Dir: A fixed varibale which leads to the ItemType's folder (I.E, "Consumable/")
         * *IconName*: A different variable which points to the icon's name (I.E, "Health Potion")
         * *Item ID*: A variable used to assign an item an ID. This is for adding items to the player's inventory. (I.E., TargetHandler.player.inventory.AddItemByID(*ID*, *Quantity*))
         * *Item Name*: Name is for display purposes/Inventory.AddItemByName(*Name*, *Quantity*)
         */

        public static List<BaseItem> CustomItems(ContentManager cm)
        {
            List<BaseItem> modItems = new List<BaseItem>();

            return modItems;
        }

        public static void LoadItemData(ContentManager main, ContentManager map)
        {
            LoadConsumables(main);
            LoadWeapons(main);
            LoadArmor(main);
            LoadAmmo(main);
            LoadJewellery(main);
            LoadResources(main);
            LoadMiscellaneous(main);

            AddTo(new HealingPotion(main.Load<Texture2D>(IconDir + ConsumableDir + "vigorousElixer"), 2, "Elixer of the Vigorous", "A core elixer bottled at a soulgate. Restores the user's health by quite a lot. Blah Blah Blah Blah Blah Blah Blah Blah Blah [#B2A68B]Blah Blah Blah...? [/#]Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah Blah.", 5, false, new Requirements(), -1, 300));

            //AddTo(new Miscellaneous(main.Load<Texture2D>(IconDir + MiscDir + "Games/TwoDiceB"), 1000, "Bone Dice", "An old cube carved from bone with notches etched into it. It has no real purpose other than as a simple passtime.", 100, 1, true, new Requirements(IntelligenceLevel: 10), 50, "Game", null));

            LoadItemContent(main, map);
            IterateDictionary();
        }
        private static void LoadItemContent(ContentManager main, ContentManager map)
        {
            for (int i = 0; i < items.Count; i++)
                items[i].Load(main, map);
        }
        private static void IterateDictionary()
        {
            for (int i = 0; i < items.Count; i++)
                idIndex.Add(items[i].ID, i);
        }

        //0-1000
        private static void LoadConsumables(ContentManager cm)
        {
            #region Essences
            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "inertEssence"), 10, "Inert Essence", "", 20, 1, true, new Requirements(), -1, "Essence", null));
            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "sunlightEssence"), 11, "Sunlight Essence", "", 20, 5, true, new Requirements(), -1, "Essence", "Restorative"), "Restore", "Regenerate", null, null, null, null,
                (BaseItem a) =>
                {
                    uint value = (uint)(200 * ((float)a.CurrentDurability / a.MaxDurability));
                    a.CurrentEntity.HEALTH_Restore(value);

                    a.CurrentAmount--;
                    a.CurrentEntity.STORAGE_AddItem(10, 1);

                    a.CurrentEntity.SUSPEND_Action(1000);
                }, (BaseItem a) =>
                {
                    //Add regenerative code here.
                    a.CurrentEntity.SUSPEND_Action(500);
                    a.CurrentDurability--;

                    if (a.CurrentDurability <= 0)
                    {
                        a.CurrentAmount--;
                        a.CurrentEntity.STORAGE_AddItem(10, 1);

                        a.CurrentDurability = a.MaxDurability;
                    }

                    a.CurrentEntity.STATUS_AddStatus(1, a.CurrentEntity.MapEntityID);
                }, null, null, null, null);

            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "moonlightEssence"), 12, "Moonlight Essence", "", 20, 5, true, new Requirements(), -1, "Essence", "Magical"), "Restore", "Regenerate", null, null, null, null,
                (BaseItem a) =>
                {
                    uint value = (uint)(400 * ((float)a.CurrentDurability / a.MaxDurability));
                    a.CurrentEntity.MAGIC_Restore(value);

                    a.CurrentAmount--;
                    a.CurrentEntity.STORAGE_AddItem(10, 1);

                    a.CurrentEntity.SUSPEND_Action(1000);

                }, (BaseItem a) =>
                {
                    //Add regenerative code here.
                    a.CurrentEntity.SUSPEND_Action(500);
                    a.CurrentDurability--;

                    if (a.CurrentDurability <= 0)
                    {
                        a.CurrentAmount--;
                        a.CurrentEntity.STORAGE_AddItem(10, 1);

                        a.CurrentDurability = a.MaxDurability;
                    }

                }, null, null, null, null);

            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "loamstoneEssence"), 13, "Loamstone Essence", "", 20, 5, true, new Requirements(), -1, "Essence", "Invigorative"), "Restore", "Regenerate", null, null, null, null,
                (BaseItem a) =>
                {
                    uint value = (uint)(50 * ((float)a.CurrentDurability / a.MaxDurability));
                    a.CurrentEntity.STAMINA_Restore(value);

                    a.CurrentAmount--;
                    a.CurrentEntity.STORAGE_AddItem(10, 1);

                    a.CurrentEntity.SUSPEND_Action(1000);

                }, (BaseItem a) =>
                {
                    //Add regenerative code here.
                    a.CurrentEntity.SUSPEND_Action(500);
                    a.CurrentDurability--;

                    if (a.CurrentDurability <= 0)
                    {
                        a.CurrentAmount--;
                        a.CurrentEntity.STORAGE_AddItem(10, 1);

                        a.CurrentDurability = a.MaxDurability;
                    }

                }, null, null, null, null);
            #endregion

            #region Potions
            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "Potions/tonicOfForce"), 50, "Tonic of Force", "", 99, 5, false, new Requirements(), 500, "Potion", "Heightening"), "Drink", string.Empty, null, null, null, null,
                (BaseItem a) =>
                {
                    if (a.CurrentAmount > 0)
                    {
                        a.CurrentEntity.STATUS_AddStatus(10, "ForceTonic");
                        a.CurrentAmount--;
                    }
                }, null, null, null, null, null, null);
            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "Potions/spearguardStimulant"), 51, "Spearguard's Stimulant", "", 99, 5, false, new Requirements(), 500, "Potion", "Heightening"), "Drink", string.Empty, null, null, null, null,
                (BaseItem a) =>
                {
                    if (a.CurrentAmount > 0)
                    {
                        a.CurrentEntity.STATUS_AddStatus(11, "GuardianStimulant");
                        a.CurrentAmount--;
                    }
                }, null, null, null, null, null, null);
            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "Potions/stillingConcoction"), 52, "Stilling Concoction", "", 99, 5, false, new Requirements(), 500, "Potion", "Heightening"), "Drink", string.Empty, null, null, null, null,
                (BaseItem a) =>
                {
                    if (a.CurrentAmount > 0)
                    {
                        a.CurrentEntity.STATUS_AddStatus(12, "StillingConcoction");
                        a.CurrentAmount--;
                    }
                }, null, null, null, null, null, null);
            AddTo(new Consumable(cm.Load<Texture2D>(IconDir + ConsumableDir + "Potions/dropOfCelerity"), 53, "Drop of Celerity", "", 99, 5, false, new Requirements(), 500, "Potion", "Heightening"), "Drink", string.Empty, null, null, null, null,
                (BaseItem a) =>
                {
                    if (a.CurrentAmount > 0)
                    {
                        a.CurrentEntity.STATUS_AddStatus(13, "CelerityPotion");
                        a.CurrentAmount--;
                    }
                }, null, null, null, null, null, null);
            #endregion
        }

        //1000-2000
        private static void LoadWeapons(ContentManager cm)
        {
            LoadStrikers(cm);
            LoadDeflectors(cm);
            LoadShooters(cm);
            LoadCasters(cm);
        }
        private static void LoadStrikers(ContentManager cm)
        {
            //1000s
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "testSword"), 1000, "Old-Moss Sword", "A longsword forged from steel with a green tint.", 1, false, new TestSword(), StrikerType.Longsword, new Requirements(10), 100, 1f, 50, 0, 0f, 400, "Melee", "Longsword"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Strikers/Daggers/wyrmtoothDagger"), 1003, "Wyrmtooth", "", 1, false, new Wyrmtooth(), StrikerType.Dagger, new Requirements(), 250, 1f, 15, 0, .85f, 5000, "Striker", "Dagger"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Strikers/oldKnightsLongsword"), 1004, "Old Knight's Longsword", "", 1, false, new Wyrmtooth(), StrikerType.Longsword, new Requirements(), 300, 1f, 10, 0, .9f, 7500, "Striker", "Longsword"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Strikers/tarnishedShortsword"), 1005, "Tarnished Shortsword", "", 1, false, new TarnishedShortsword(), StrikerType.Shortsword, new Requirements(), 300, 1f, 7, 0, .9f, 7500, "Striker", "Shortsword"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Strikers/primitivesSpear"), 1006, "Primitive Spear", "", 1, false, new PrimitiveSpear(), StrikerType.Spear, new Requirements(), 300, 1f, 18, 0, .9f, 7500, "Striker", "Spear"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Strikers/sacrificialDagger"), 1007, "Sacrificial Dagger", "", 1, false, new Wyrmtooth(), StrikerType.Longsword, new Requirements(), 300, 1f, 10, 0, .9f, 7500, "Striker", "Dagger"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Strikers/stonestrikeSpearaxe"), 1008, "Stonestrike Halberd", "", 1, false, new StonestrikeHalberd(), StrikerType.Halberd, new Requirements(), 500, 1f, 35, 0, .9f, 7500, "Striker", "Halberd"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Strikers/palewoodRapier"), 1009, "Palewood Rapier", "", 1, false, new PalewoodRapier(), StrikerType.Longsword, new Requirements(), 300, 1f, 15, 0, .9f, 7500, "Striker", "Rapier"));
        }
        private static void LoadDeflectors(ContentManager cm)
        {
            //1200s
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "testSword"), 1200, "Test Shield", "", 1, false, new TestShield(), DeflectorType.Roundshield, new Requirements(10), 100, 1f, 10, 0, .95f, 300, "Defense", "Shield"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Blockers/oldKnightsGreatshield"), 1201, "Old Knight's Greatshield", "", 1, false, new TestShield(), DeflectorType.Greatshield, new Requirements(), 200, 1f, 15, 0, .99f, 6000, "Blocker", "Greatshield"));
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Blockers/villagekeepersShield"), 1202, "Villagekeeper's Shield", "", 1, false, new VillagekeepersShield(), DeflectorType.Greatshield, new Requirements(), 200, 1f, 15, 0, .99f, 6000, "Blocker", "Shield"));
        }
        private static void LoadShooters(ContentManager cm)
        {
            //1400s
            items.Add(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Shooters/Longbows/keenLongbow"), 1400, "Longbow Of The Keen", "", 1, false, new TestBow(), ShooterType.Longbow, new Requirements(), 100, 1f, 10, 30, 0f, 500, "Archery", "Longbow"));
            AddTo(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Shooters/Crossbows/beastfellersArweblast"), 1401, "Beastfeller's Arweblast", "", 1, false, new TestArweblast(), ShooterType.Arweblast, new Requirements(), 150, 1f, 15, 50, 0f, 500, "Archery", "Arweblast"));
            AddTo(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Shooters/Crossbows/wallguardsCrossbow"), 1402, "Wallguard's Crossbow", "", 1, false, new TestCrossbow(), ShooterType.Crossbow, new Requirements(), 125, 1f, 10, 40, 0f, 500, "Archery", "Crossbow"));

            //Thrown
            AddTo(new ThrownWeapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Thrown/ironTippedStake"), 1500, "Iron-tipped Stake", "", 99, false, new Knife(), new Requirements(), 15, 1f, 10, 15, 0f, 1, ProjectileDatabase.GetProjectileTexture(1), 30, "Archery", "Stake"));
            AddTo(new ThrownWeapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Thrown/ironFrancisca"), 1501, "Iron Francisca", "", 99, false, new Knife(), new Requirements(), 15, 1f, 10, 15, 0f, 1, ProjectileDatabase.GetProjectileTexture(1), 30, "Archery", "Axe"));
            AddTo(new ThrownWeapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Thrown/easternSkullpiercer"), 1502, "Eastern Skullpiercer", "", 99, false, new Knife(), new Requirements(), 15, 1f, 10, 15, 0f, 1, ProjectileDatabase.GetProjectileTexture(1), 30, "Archery", "Javelin"));
            AddTo(new ThrownWeapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Thrown/SteelKnife"), 1403, "Steel Knife", "", 99, false, new Knife(), new Requirements(), 15, 1f, 10, 15, 0f, 1, ProjectileDatabase.GetProjectileTexture(1), 30, "Archery", "Knife"));

            //Flasks
            AddTo(new ThrownWeapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Thrown/poisonousFlask"), 1503, "Poisonous Flask", "", 99, false, new Knife(), new Requirements(), 15, 1f, 10, 15, 0f, 1, ProjectileDatabase.GetProjectileTexture(1), 30, "Archery", "Flask"));
            AddTo(new ThrownWeapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Thrown/explosiveFlask"), 1504, "Explosive Flask", "", 99, false, new Knife(), new Requirements(), 15, 1f, 10, 15, 0f, 1, ProjectileDatabase.GetProjectileTexture(1), 30, "Archery", "Flask"));
        }
        private static void LoadCasters(ContentManager cm)
        {
            //1600s
            AddTo(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Casters/castersShortstaff"), 1600, "Caster's Shortstaff", "", 1, false, new TestLongstaff(), CasterType.Shortstaff, new Requirements(MagicLevel: 5), 200, 1f, 10, 30, 0f, 750, "Magic", "Shortstaff"));
            AddTo(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Casters/lightworkersQuarterstaff"), 1601, "Lightworker's Quarterstaff", "", 1, false, new TestLongstaff(), CasterType.Shortstaff, new Requirements(MagicLevel: 5), 200, 1f, 10, 30, 0f, 750, "Magic", "Quarterstaff"));
            AddTo(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Casters/warmagesLongstaff"), 1602, "Warmage's Longstaff", "", 1, false, new TestLongstaff(), CasterType.Longstaff, new Requirements(MagicLevel: 5), 200, 1f, 10, 30, 0f, 750, "Magic", "Longstaff"));

            AddTo(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Casters/ofBlessingsAndSundries"), 1603, "Of Blessings & Sundries", "", 1, false, new TestLongstaff(), CasterType.Book, new Requirements(MagicLevel: 5), 200, 1f, 10, 30, 0f, 750, "Magic", "Book"));
            AddTo(new Weapon(cm.Load<Texture2D>(IconDir + WeaponDir + "Casters/grimoireOfChants"), 1604, "Grimoire Of Chants", "", 1, false, new TestLongstaff(), CasterType.Book, new Requirements(MagicLevel: 5), 200, 1f, 10, 30, 0f, 750, "Magic", "Book"));
        }

        //2000s
        private static void LoadArmor(ContentManager cm)
        {
            //Test armor
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "IronHelm"), 101, "Iron Helm", "A helmet of iron.", 100, false, Armor.ArmorType.Head, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            items.Add(new Armor(cm.Load<Texture2D>(IconDir + MiscDir + "hiddenItem"), 102, "Iron Chestplate", "Blah.", 700, false, Armor.ArmorType.Torso, new ItemAttribute(1.15f, 1.1f, 1.1f, 25f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null), new Requirements(StrengthLevel: 5, DefenseLevel: 15, IntelligenceLevel: 2), 250, "Torso", "Iron"));
            items.Add(new Armor(cm.Load<Texture2D>(IconDir + MiscDir + "hiddenItem"), 103, "Iron Platelegs", "Blah.", 600, false, Armor.ArmorType.Legs, new ItemAttribute(40, 50, 25, 43.5f, null), new Requirements(StrengthLevel: 5), 200, "Legs", "Iron"));
            items.Add(new Armor(cm.Load<Texture2D>(IconDir + MiscDir + "hiddenItem"), 104, "Iron Plateboots", "Blah.", 800, false, Armor.ArmorType.Feet, new ItemAttribute(20, 15, 10, 13.25f, null), new Requirements(StrengthLevel: 5), 150, "Feet", "Iron"));
            items.Add(new Armor(cm.Load<Texture2D>(IconDir + MiscDir + "hiddenItem"), 105, "Iron Gauntlets", "Blah.", 800, false, Armor.ArmorType.Hands, new ItemAttribute(25, 20, 15, 7.65f, null), new Requirements(StrengthLevel: 5), 175, "Hands", "Iron"));
            items.Add(new Armor(cm.Load<Texture2D>(IconDir + MiscDir + "hiddenItem"), 106, "Enchanted Cape", "Blah.", 400, false, Armor.ArmorType.Cape, new ItemAttribute(5, 10, 15, -50.35f, null), new Requirements(StrengthLevel: 20), 225, "Cape", ""));
            items.Add(new Armor(cm.Load<Texture2D>(IconDir + MiscDir + "hiddenItem"), 107, "Steel Chestplate", "Blah.", 700, false, Armor.ArmorType.Torso, new ItemAttribute(55, 85, 35, 60, 0, 0, 0, 0, 0, 0, 0, 0, 0f, 0f, 0f, .1f, null), new Requirements(StrengthLevel: 5, DefenseLevel: 15, IntelligenceLevel: 2), 250, "Torso", "Steel"));

            //Light
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "ironSallet"), 2000, "Iron Sallet", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Head, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "chainmailTunic"), 2001, "Chainmail Tunic", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Torso, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "chainmailCuisse"), 2002, "Chainmail Cuisse", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Legs, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "ironBoots"), 2003, "Iron Boots", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Feet, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "ironGauntlets"), 2004, "Iron Gauntlets", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Hands, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));

            //Heavy
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "oldKnightsHelm"), 2005, "Old Knight Helm", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Head, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "oldKnightsChestplate"), 2006, "Old Knight Chestplate", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Torso, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "oldKnightsPlatedLegs"), 2007, "Old Knight Platelegs", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Legs, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "oldKnightsSabatons"), 2008, "Old Knight Sabatons", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Feet, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "oldKnightsVambraces"), 2009, "Old Knight Vambraces", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Hands, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));

            //Caster
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "castersHood"), 2010, "Caster Hood", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Head, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "castersPlatedRobes"), 2011, "Caster Platerobes", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Torso, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Torso", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "castersLegs"), 2012, "Caster Legs", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Legs, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "castersFootgear"), 2013, "Caster Footgear", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Hands, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
            AddTo(new Armor(cm.Load<Texture2D>(IconDir + ArmorDir + "castersArmguards"), 2014, "Caster Armguards", "An iron helm worn by an old knight whose name is no longer known.", 100, false, Armor.ArmorType.Feet, new ItemAttribute(), new Requirements(StrengthLevel: 5), 150, "Head", "Iron"));
        }

        //3000s
        private static void LoadAmmo(ContentManager cm)
        {
            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "spearstoneArrow"), 3000, "Spearstone Arrow", "", 10, 1, ProjectileDatabase.GetProjectileTexture(1), false, new Requirements(), Ammo.AmmoType.Arrow, 50, "Arrow", ""));
            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "spearstoneBolt"), 3001, "Spearstone Bolt", "", 10, 101, ProjectileDatabase.GetProjectileTexture(101), false, new Requirements(), Ammo.AmmoType.Bolt, 50, "Bolt", ""));

            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "ironArrow"), 3002, "Iron Arrow", "", 10, 2, ProjectileDatabase.GetProjectileTexture(2), false, new Requirements(), Ammo.AmmoType.Arrow, 50, "Arrow", ""));
            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "ironBolt"), 3003, "Iron Bolt", "", 10, 102, ProjectileDatabase.GetProjectileTexture(102), false, new Requirements(), Ammo.AmmoType.Bolt, 50, "Bolt", ""));

            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "glassArrow"), 3004, "Glass Arrow", "", 10, 3, ProjectileDatabase.GetProjectileTexture(3), false, new Requirements(), Ammo.AmmoType.Arrow, 50, "Arrow", ""));
            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "glassBolt"), 3005, "Glass Bolt", "", 10, 103, ProjectileDatabase.GetProjectileTexture(103), false, new Requirements(), Ammo.AmmoType.Bolt, 50, "Bolt", ""));

            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "poisonArrow"), 3006, "Poison Arrow", "", 10, 4, ProjectileDatabase.GetProjectileTexture(4), false, new Requirements(), Ammo.AmmoType.Arrow, 50, "Arrow", ""));
            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "poisonBolt"), 3007, "Poison Bolt", "", 10, 104, ProjectileDatabase.GetProjectileTexture(104), false, new Requirements(), Ammo.AmmoType.Bolt, 50, "Bolt", ""));

            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "flameArrow"), 3008, "Flame Arrow", "", 10, 5, ProjectileDatabase.GetProjectileTexture(5), false, new Requirements(), Ammo.AmmoType.Arrow, 50, "Arrow", ""));
            AddTo(new Ammo(cm.Load<Texture2D>(IconDir + AmmoDir + "flameBolt"), 3009, "Flame Bolt", "", 10, 105, ProjectileDatabase.GetProjectileTexture(105), false, new Requirements(), Ammo.AmmoType.Bolt, 50, "Bolt", ""));
        }
        //3500s
        private static void LoadJewellery(ContentManager cm)
        {
            //3500-3750 - Rings
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Rings/ringOfRenewal"), 3500, "Ring Of Renewal", "", Jewellery.JewelleryType.Ring, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(99, string.Empty); }), 200, "Ring", "Regenerative"));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Rings/flamefulRing"), 3501, "Ring Of The Flameful", "", Jewellery.JewelleryType.Ring, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(100, string.Empty); }), 200, "Ring", "Heightening"));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Rings/spellcastersPatron"), 3502, "Spellcaster's Patron", "", Jewellery.JewelleryType.Ring, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(101, string.Empty); }), 200, "Ring", "Regenerative"));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Rings/bandOfNature"), 3503, "Band Of Leaves", "", Jewellery.JewelleryType.Ring, 100, false, new Requirements(), new ItemAttribute((e, i) => { }), 200, "Ring", ""));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Rings/conserversRing"), 3504, "Conserver's Ring", "", Jewellery.JewelleryType.Ring, 100, false, new Requirements(), new ItemAttribute((e, i) => { }), 200, "Ring", "Conservation"));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Rings/ringOfHastening"), 3505, "Ring Of Hastening", "", Jewellery.JewelleryType.Ring, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(104, string.Empty); }), 200, "Ring", "Speed"));
            //Missing ring
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Rings/resistanceArmyll"), 3507, "Resistance Armyll", "", Jewellery.JewelleryType.Ring, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(106, string.Empty); }), 200, "Ring", "Defensive"));

            //3750-4000 - Necklaces
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Necklaces/wanderersBandstrot"), 3750, "Knight's Bandstrot", "", Jewellery.JewelleryType.Amulet, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(50, string.Empty); }), 200, "Necklace", "Teleportation"));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Necklaces/tonguesOfForeigners"), 3751, "Tongues Of Foreigners", "", Jewellery.JewelleryType.Amulet, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(51, string.Empty); }), 200, "Necklace", "Languages"));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Necklaces/chainOfEasing"), 3752, "Chain Of Easing", "", Jewellery.JewelleryType.Amulet, 100, false, new Requirements(), new ItemAttribute((e, i) => { e.STATUS_AddStatus(52, string.Empty); }), 200, "Necklace", "Weight"));
            AddTo(new Jewellery(cm.Load<Texture2D>(IconDir + JewelleryDir + "Necklaces/casualsNecklace"), 4000, "Casual's Necklace", "", Jewellery.JewelleryType.Amulet, 100, false, new Requirements(), new ItemAttribute((e, i) => { if (e.Skills.health.CurrentHP < e.Skills.health.MaxHP) { e.HEALTH_Restore((uint)(e.Skills.health.MaxHP - e.Skills.health.CurrentHP), false); } }), 200, "Necklace", "Invincibility"));
        }
        //4000-5000s
        private static void LoadResources(ContentManager cm)
        {
            //Brewing - Primary Ingredients
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Flora/brewersFoot"), 4700, "Brewer's Foot", "", 99, 1, false, new Requirements(), 50, "Ingredient", "Herb"), null, null);

            //Brewing - Secondary Ingredients
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Flora/risingDawn"), 4800, "Rising Dawn", "", 99, 1, false, new Requirements(), 50, "Ingredient", "Herb"), null, null);
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Flora/bonecapMushroom"), 4801, "Bonecap Mushroom", "", 99, 1, false, new Requirements(), 50, "Ingredient", "Fungi"), null, null);
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Flora/saintsHerb"), 4802, "Saint's Herb", "", 99, 1, false, new Requirements(), 50, "Ingredient", "Herb"), null, null);
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Flora/greenClover"), 4803, "Green Clover", "", 99, 1, false, new Requirements(), 50, "Ingredient", "Herb"), null, null);

            //Wood
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Wood/palewoodLog"), 4901, "Palewood Log", "", 99, 5, false, new Requirements(), 3, "Wood", "Light"), string.Empty, null, null, null, null, null, null, null, null, null, null, null, null);

            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Wood/stillwoodLog"), 4906, "Stillwood Log", "", 99, 8, false, new Requirements(), 3, "Wood", "Hard"), string.Empty, null, null, null, null, null, null, null, null, null, null, null, null);

            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Wood/storgewoodLog"), 4910, "Storgewood Log", "", 99, 10, false, new Requirements(), 3, "Wood", "Hard"), string.Empty, null, null, null, null, null, null, null, null, null, null, null, null);

            //Stones
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Stone/commonStone"), 4950, "Common Stone", "", 99, 10, false, new Requirements(), 3, "Stone", ""));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Stone/spearstone"), 4951, "Spearstone", "", 99, 12, false, new Requirements(), 10, "Stone", ""));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Stone/riverrock"), 4952, "Riverrock", "", 99, 8, false, new Requirements(), 7, "Stone", ""));

            //Ores
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/ironOre"), 5000, "Iron Ore", "Ore of the iron.", 99, 100, false, new Requirements(), 100, "Ore", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/silverOre"), 5001, "Silver Ore", "Ore of the iron.", 99, 50, false, new Requirements(), 400, "Ore", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/goldOre"), 5002, "Gold Ore", "Ore of the iron.", 99, 50, false, new Requirements(), 500, "Ore", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/soliteOre"), 5008, "Solite Ore", "Solite ore.", 99, 100, false, new Requirements(), 2000, "Ore", null));

            //Metals
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/ironIngot"), 5020, "Iron Ingot", "Ore of the iron.", 99, 100, false, new Requirements(), 125, "Metal", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/steelIngot"), 5021, "Steel Ingot", "Ore of the iron.", 99, 100, false, new Requirements(), 450, "Metal", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/silverIngot"), 5022, "Silver Ingot", "Ore of the iron.", 99, 100, false, new Requirements(), 550, "Metal", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Metals/goldIngot"), 5023, "Gold Ingot", "Solite ore.", 99, 100, false, new Requirements(), 2100, "Metal", null));

            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Fuel/bituminousCoal"), 5050, "Bituminous Coal", "The standard blacksmith's coal.", 99, 7, false, new Requirements(), 50, "Fuel", ""));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Fuel/anthraciteCoal"), 5051, "Anthracite Coal", "A denser fuel with a higher temperature than bituminous coal.", 99, 9, false, new Requirements(), 50, "Fuel", ""));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Fuel/bituminousCoal"), 5052, "Fauxlight Coal", "An imitation fuel made from Anthracite Coal and a certain magical chant. It burns just as hot, but thrice as fast.", 99, 5, false, new Requirements(), 50, "Fuel", ""));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Fuel/daylightCoal"), 5053, "Daylight Coal", "A rare coal, said to burn bright even in daylight.", 99, 7, false, new Requirements(), 50, "Fuel", ""));

            AddTo(new Resource(cm.Load<Texture2D>(IconDir + MiscDir + "mendersMucilage"), 5500, "Mender's Mucilage", "A sticky substance that is made by combining reduced tree sap with enchanted materials. This will gradually transform into the metallic material this substance touches, making it useful for reforgers and blacksmiths.", 1, 99, false, new Requirements(), 250, "Adhesive", "Repair"));

            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "clearLiniment"), 6000, "Clear Liniment", "", 99, 1, false, new Requirements(), 25, null, null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "sapphireDust"), 6001, "Crushed Sapphire", "", 99, 1, false, new Requirements(), 53, "Powder", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "rubyDust"), 6002, "Crushed Ruby", "", 99, 1, false, new Requirements(), 115, "Powder", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "emeraldDust"), 6003, "Crushed Emerald", "", 99, 1, false, new Requirements(), 155, "Powder", null));

            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "Gems/onyx"), 6007, "Onyx", "", 99, 1, false, new Requirements(), 200, "Gemstone", null), (BaseItem a, BaseItem b) =>
            {
                if (b.ID == 7003) //If the item in use is the pestle and mortar ...
                {
                    a.CurrentAmount--;
                    a.CurrentEntity.STORAGE_AddItem(6008, 1, false, true);

                    SoundEffect2D s = a.GetSFX(0, 1, 2, 3, 4);
                    a.Map.AddSound(s, ref a.CurrentEntity.refPosition);
                    a.CurrentEntity.SUSPEND_Action(s.Duration);

                    a.doesCombineHaveResults = true;
                }
            }, "Crush", "", "", "", "", "", (BaseItem i) =>
            {
                if (i.CurrentEntity.STORAGE_Check(7003) == true) //If the entity has a pestle and mortar, ...
                    i.CombineItem(i.CurrentEntity.STORAGE_GetItem(7003));
                else
                {
                    i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Tool required: Ornate Grinder"); //Temporary, add in a dialog pop-up box when ready.
                    i.Screens.PlaySound("Invalid");
                }
            }, null, null, null, null, null, new SoundEffect2D(cm.Load<SoundEffect>("Audio/Effects/Items/Misc/Tools/pestleMortarCrush2"), 200f), new SoundEffect2D(cm.Load<SoundEffect>("Audio/Effects/Items/Misc/Tools/pestleMortarCrush3"), 200f),
               new SoundEffect2D(cm.Load<SoundEffect>("Audio/Effects/Items/Misc/Tools/pestleMortarCrush4"), 200f), new SoundEffect2D(cm.Load<SoundEffect>("Audio/Effects/Items/Misc/Tools/pestleMortarCrush5"), 200f),
               new SoundEffect2D(cm.Load<SoundEffect>("Audio/Effects/Items/Misc/Tools/pestleMortarCrush6"), 200f));

            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "onyxDust"), 6008, "Onyx Dust", "", 99, 1, false, new Requirements(), 210, null, null));
            //AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + ""), 6000, "", "", 1, false, new Requirements()));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "cenobiumCube"), 6350, "Cenobium Cube", "", 99, 1, false, new Requirements(), 150, "Metal", null));
            AddTo(new Resource(cm.Load<Texture2D>(IconDir + ResourceDir + "cenobiumCubeBlessed"), 6351, "Blessed Cenobium Cube", "", 99, 1, false, new Requirements(), 500, "Metal", "Enchanted"));
        }
        //7000s
        private static void LoadMiscellaneous(ContentManager cm)
        {
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "oldToken"), 1, "Outworn Token", "It's a fragile token, crumbles at the touch. A reminder of a different time, one of prosperity and peace.", 50, 99, false, new Requirements(IntelligenceLevel: 1), 1, "Memoir", null));
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "Tools/graveyardShovelB"), 7000, "Graveyard Spade", "", 250, 1, false, new Requirements(10), 300, "Tool", "Shovel"));
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "Tools/axe"), 7001, "Treefeller's Axe", "", 250, 1, false, new Requirements(15), 300, "Tool", "Axe"));
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "Tools/pickaxe"), 7002, "Oreseeker's Pickaxe", "", 250, 1, false, new Requirements(20), 350, "Tool", "Pickaxe"));
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "Tools/pestleMortar"), 7003, "Ornate Grinder", "", 500, 1, false, new Requirements(5), 250, "Tool", "Grinder"));
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "Tools/shriekhorn"), 7004, "Shriekhorn", "", 5, 1, false, new Requirements(), 10000, "Instrument", "Enchanted", true, true), "Soft Blow", "Shriek", null, null, null, null,
                (i) =>
                {
                    if (i.CurrentDurability > 0)
                    {
                        i.Map.ENTITIES_ResurrectAll(true);
                        i.CurrentDurability--;

                        i.CurrentEntity.SUSPEND_Action(1000);
                        i.CurrentEntity.SUSPEND_Movement(1000);
                    }
                },
                (i) =>
                {
                    if (i.CurrentDurability >= 2)
                    {
                        i.Map.ENTITIES_ResurrectAll(true);
                        i.CurrentDurability -= 3;

                        if (i.CurrentEntity is PlayerEntity)
                            ((PlayerEntity)i.CurrentEntity).IncrementRestCount();

                        i.CurrentEntity.SUSPEND_Action(1000);
                        i.CurrentEntity.SUSPEND_Movement(1000);
                    }
                }, null, null, null, null, null);
            AddTo(new Binocular(cm.Load<Texture2D>(IconDir + MiscDir + "Tools/magnifyingGlass"), 7005, "Magnifying Glass", "", 10, 1, false, new Requirements(), 1500, "Tool", "Observing"), "Observe", "Inspect", null, null, null, null,
                (i) =>
                {
                    i.Camera.SetCameraState(Camera.CameraState.Cinematic);
                    i.Camera.SmoothZoom(2f, 1f, true, 0);
                    ((Binocular)i).IsLooking = true;
                    ((Binocular)i).Distance = 120;

                    i.CurrentEntity.SUSPEND_Action(500);
                    i.CurrentEntity.SUSPEND_Movement(500);
                },
                (i) =>
                {
                    i.Camera.SetCameraState(Camera.CameraState.Cinematic);
                    i.Camera.SmoothZoom(4f, 1f, true, 0);
                    ((Binocular)i).Distance = 80;
                    ((Binocular)i).IsLooking = true;

                    i.CurrentEntity.SUSPEND_Action(500);
                    i.CurrentEntity.SUSPEND_Movement(500);
                }, null, null, null, null, null);
            AddTo(new Binocular(cm.Load<Texture2D>(IconDir + MiscDir + "Tools/realmfarersGlass"), 7006, "Realmfarer's Glass", "", 10, 1, false, new Requirements(), 1500, "Tool", "Looking"), "Peer", "Landgaze", null, null, null, null,
                (i) =>
                {
                    i.Camera.SetCameraState(Camera.CameraState.Cinematic);
                    i.Camera.SmoothZoom(1f, 1f, true, 0);
                    ((Binocular)i).IsLooking = true;
                    ((Binocular)i).IsMovementLocked = true;
                    ((Binocular)i).Distance = 700;
                },
                (i) =>
                {
                    if (i.CurrentDurability > 0)
                    {
                        i.Camera.SetCameraState(Camera.CameraState.Cinematic);
                        i.Camera.SmoothZoom(.75f, .25f, true, 0);
                        ((Binocular)i).Distance = 0;
                        ((Binocular)i).IsMovementLocked = true;
                        ((Binocular)i).IsLooking = true;

                        i.CurrentDurability--;
                    }
                    else
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Item is out of charge");
                }, null, null, null, null, null);
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "glassBottle2"), 7050, "Glass Bottle", "", 3, 99, false, new Requirements(), 25, "Container", "Empty"), "Fill", null, null, null, null, null,
                (i) =>
                {
                    if (i.CurrentAmount > 0)
                    {
                        if (i.CurrentEntity.STORAGE_IsMax(7051) == false)
                        {
                            i.CurrentEntity.STORAGE_AddItem(7051, 1, false, true);
                            i.CurrentAmount--;
                        }
                        else
                            i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "You can't carry anymore.");
                    }
                }, null, null, null, null, null, null);
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "filledBottle"), 7051, "Filled Bottle", "", 3, 99, false, new Requirements(), 25, "Container", "Water"), "Empty", null, null, null, null, null,
                (i) =>
                {
                    if (i.CurrentAmount > 0)
                    {
                        if (i.CurrentEntity.STORAGE_IsMax(7050) == false)
                        {
                            i.CurrentEntity.STORAGE_AddItem(7050, 1, false, true);
                            i.CurrentAmount--;
                        }
                        else
                            i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "You can't carry anymore.");
                    }
                }, null, null, null, null, null, null);
            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "ruinedBrew"), 7052, "Ruined Brew", "", 3, 99, false, new Requirements(), 25, "Container", "Miasmic"), "Empty", "Drink", null, null, null, null,
                (i) =>
                {
                    if (i.CurrentEntity.STORAGE_IsMax(7050) == false)
                    {
                        i.CurrentEntity.STORAGE_AddItem(7050, 1, false, true);
                        i.CurrentAmount--;
                    }
                    else
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "You can't carry anymore.");
                },
                (i) =>
                {
                    //To-do: fill these IDs with negative effects.
                    int[] ids = new int[] { 0, 0, 0, };
                    int statusID = i.Random.Next(0, ids.Length);

                    //i.CurrentEntity.STATUS_AddStatus(statusID, "Disease");
                    //i.CurrentAmount--;
                }, null, null, null, null, null);

            AddTo(new Miscellaneous(cm.Load<Texture2D>(IconDir + MiscDir + "familiarFragment"), 7499, "Familiar Fragment", "", 1, 99, false, new Requirements(), 500, "Warping", ""), "Return", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                (i) =>
                {
                    if (i.CurrentEntity.SUSPENSION_Action == Performance.Suspension.SuspendState.None)
                    {
                        if (i.CurrentEntity is PlayerEntity)
                        {
                            ((PlayerEntity)i.CurrentEntity).SOULGATE_BeginTeleporting();
                            i.CurrentAmount--;
                        }
                    }
                }, null, null, null, null, null, null, null);
            AddTo(new NumberHolder(cm.Load<Texture2D>(IconDir + MiscDir + "emberVessel"), 7500, "Ember Vessel", "A vessel for keeping gathered embers.", 1, 1, false, new Requirements(), 5000, "Container", "Embers", "Embers", ColorHelper.D_Orange), "Stow Embers", "Stow Half", "Stow Some", "Stow Few", "Smash Bank", string.Empty,
                (i) =>
                {
                    if (i.CurrentEntity.SKILL_Embers() > 0)
                    {
                        ((NumberHolder)i).Number += i.CurrentEntity.SKILL_TakeEmbers(i.CurrentEntity.SKILL_Embers());
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Stowed All Embers");
                    }
                    else
                    {
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "No Embers To Stow");
                    }
                },
                (i) =>
                {
                    if (i.CurrentEntity.SKILL_Embers() > 0)
                    {
                        ((NumberHolder)i).Number += i.CurrentEntity.SKILL_TakeEmbers((int)(i.CurrentEntity.SKILL_Embers() * .5f));
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Stowed Half Of Embers");
                    }
                    else
                    {
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "No Embers To Stow");
                    }
                },
                (i) =>
                {
                    if (i.CurrentEntity.SKILL_Embers() > 0)
                    {
                        ((NumberHolder)i).Number += i.CurrentEntity.SKILL_TakeEmbers((int)(i.CurrentEntity.SKILL_Embers() * .25f));
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Stowed Some Embers");
                    }
                    else
                    {
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "No Embers To Stow");
                    }
                },
                (i) =>
                {
                    if (i.CurrentEntity.SKILL_Embers() > 0)
                    {
                        ((NumberHolder)i).Number += i.CurrentEntity.SKILL_TakeEmbers((int)(i.CurrentEntity.SKILL_Embers() * .1f));
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Stowed A Few Embers");
                    }
                    else
                    {
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "No Embers To Stow");
                    }
                },
                (i) =>
                {
                    if (((NumberHolder)i).Number > 0)
                    {
                        i.CurrentEntity.SKILL_AddEmbers(((NumberHolder)i).Number);
                        i.CurrentAmount -= 1;
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Retrieved Embers");
                    }
                    else
                    {
                        i.Screens.NOTIFICATION_Add(Notification.NotificationManager.IconType.Inventory, "Vessel Is Empty");
                    }
                }, null, null);
        }

        // [Methods]
        private static void AddTo(BaseItem item)
        {
            items.Add(item);
        }
        private static void AddTo(BaseItem item, Action<BaseItem, BaseItem> useOnItem, params SoundEffect2D[] sfx)
        {
            item.CombineItemAction = useOnItem; //Doesn't matter if it's null or not. They do their own checking in the BaseItem class.

            if (sfx != null)
            {
                for (int i = 0; i < sfx.Length; i++)
                item.AddSFX(sfx[i]);
            }

            items.Add(item);
        }
        private static void AddTo(BaseItem item, string buttonOneText, string buttonTwoText, string buttonThreeText,
                                                string buttonFourText, string buttonFiveText, string buttonSixText,
                                                Action<BaseItem> buttonOneAction, Action<BaseItem> buttonTwoAction, Action<BaseItem> buttonThreeAction,
                                                Action<BaseItem> buttonFourAction, Action<BaseItem> buttonFiveAction, Action<BaseItem> buttonSixAction, params SoundEffect2D[] sfx)
        {
            item.SetButtonStrings(buttonOneText, buttonTwoText, buttonThreeText, buttonFourText, buttonFiveText, buttonSixText);
            item.ButtonOneAction = buttonOneAction;
            item.ButtonTwoAction = buttonTwoAction;
            item.ButtonThreeAction = buttonThreeAction;
            item.ButtonFourAction = buttonFourAction;
            item.ButtonFiveAction = buttonFiveAction;
            item.ButtonSixAction = buttonSixAction;

            if (sfx != null)
            {
                for (int i = 0; i < sfx.Length; i++)
                {
                    if (sfx[i] != null)
                        item.AddSFX(sfx[i]);
                }
            }

            items.Add(item);
        }
        private static void AddTo(BaseItem item, Action<BaseItem, BaseItem> useOnItem, string buttonOneText, string buttonTwoText, string buttonThreeText,
                                        string buttonFourText, string buttonFiveText, string buttonSixText,
                                        Action<BaseItem> buttonOneAction, Action<BaseItem> buttonTwoAction, Action<BaseItem> buttonThreeAction,
                                        Action<BaseItem> buttonFourAction, Action<BaseItem> buttonFiveAction, Action<BaseItem> buttonSixAction, params SoundEffect2D[] sfx)
        {
            item.CombineItemAction = useOnItem;

            item.SetButtonStrings(buttonOneText, buttonTwoText, buttonThreeText, buttonFourText, buttonFiveText, buttonSixText);
            item.ButtonOneAction = buttonOneAction;
            item.ButtonTwoAction = buttonTwoAction;
            item.ButtonThreeAction = buttonThreeAction;
            item.ButtonFourAction = buttonFourAction;
            item.ButtonFiveAction = buttonFiveAction;
            item.ButtonSixAction = buttonSixAction;

            for (int i = 0; i < sfx.Length; i++)
                item.AddSFX(sfx[i]);

            items.Add(item);
        }

        private static Tuple<int, List<Point>> SetTuple(int reinforcementValue, params Point[] idQuantity)
        {
            return new Tuple<int, List<Point>>(reinforcementValue, idQuantity.ToList());
        }

        public static BaseItem Item(int id)
        {
            /*
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                    return items[i];
            }*/

            if (idIndex.ContainsKey(id))
                return items[idIndex[id]];

            return null;
        }
        public static bool ContainsItem(int id)
        {
            return idIndex.ContainsKey(id);
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Items (Total: " + items.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            items.OrderBy(x => x.ID);

            for (int i = 0; i < items.Count; i++)
            {
                builder.AppendLine(items[i].ID + " - " + items[i].Name + " [" + items[i].GetType().Name + "]");
            }

            return builder;
        }
    }
}
