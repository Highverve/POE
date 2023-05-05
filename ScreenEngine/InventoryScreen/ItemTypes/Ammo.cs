using System;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.CombatEngine.Projectiles;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes
{
    public class Ammo : BaseItem
    {
        public enum AmmoType { Arrow, Bolt }
        AmmoType ammoType;
        public AmmoType ammo { get { return ammoType; } }

        private int projectileID;
        public int ProjectileID { get { return projectileID; } }

        private Texture2D projectileTexture;
        public Texture2D ProjectileTexture { get { return projectileTexture; } }

        public Ammo(Texture2D icon, int id, string name, string description, int MaxDurability, int ProjectileID, Texture2D ProjectileTexture, bool IsEssential, Requirements ItemRequirements, AmmoType Ammo, int SellPrice, string Type, string Subtype)
            : base(icon, id, name, description, 999, TabType.Ammo, MaxDurability, IsEssential, ItemRequirements, SellPrice, Type, Subtype)
        {
            buttonOneText = "Equip Primary";
            buttonTwoText = "Equip Secondary";

            projectileID = ProjectileID;
            projectileTexture = ProjectileTexture;

            ammoType = Ammo;

            IsMultiStack = false;

            AttributeText = "  [#" + ColorHelper.DD_Red.RGBToHex() + "]Damage: [/#]" + ProjectileDatabase.Projectile(projectileID).BaseDamage +
            " \n [#" + ColorHelper.DD_Green.RGBToHex() + "]Speed: [/#]" + ProjectileDatabase.Projectile(projectileID).BaseSpeed;
        }
    }
}
