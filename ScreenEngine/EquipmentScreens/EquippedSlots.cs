using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.Souls.Types;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;

namespace Pilgrimage_Of_Embers.ScreenEngine.EquipmentScreens
{
    public class EquippedSlots
    {
        private Vector2 columnPosition;

        private Texture2D pixel, iconBG;
        private SpriteFont font;

        private ScreenManager screens;

        private BaseEntity controlledEntity;
        private EntityEquipment equipment;

        private Color whiteFade, redFade, goldFade, charcoalFade, darkGrayFade;

        public void SetReferences(ScreenManager screens)
        {
            this.screens = screens;
        }

        public void SetControlledEntity(BaseEntity controlledEntity, EntityEquipment equipment)
        {
            this.controlledEntity = controlledEntity;
            this.equipment = equipment;
        }

        public void Load(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");
            iconBG = cm.Load<Texture2D>("Interface/Global/iconBG");

            font = cm.Load<SpriteFont>("Fonts/regularOutlined");

            columnPosition = new Vector2(GameSettings.VectorResolution.X - 68, (GameSettings.VectorResolution.Y / 2) - (32 * 5)); //5 = number of slots
        }

        public void Update(GameTime gt)
        {

        }
        public void LerpHidden(GameTime gt, float fade)
        {
            whiteFade = Color.Lerp(Color.Transparent, Color.White, fade);
            redFade = Color.Lerp(Color.OrangeRed, Color.Transparent, .5f);
            goldFade = Color.Lerp(Color.Transparent, ColorHelper.UI_Gold, fade);
            charcoalFade = Color.Lerp(Color.Transparent, ColorHelper.Charcoal, fade);
            darkGrayFade = Color.Lerp(Color.Transparent, Color.DarkGray, fade);
        }

        private int spacing = 85;
        public void Draw(SpriteBatch sb)
        {
            DrawIconBG(sb, columnPosition);
            DrawIconBG(sb, columnPosition + new Vector2(0, spacing));
            DrawIconBG(sb, columnPosition + new Vector2(0, spacing * 2));
            DrawIconBG(sb, columnPosition + new Vector2(0, spacing * 3));
            DrawIconBG(sb, columnPosition + new Vector2(0, spacing * 4));

            DrawItems(sb);
        }
        private void DrawItems(SpriteBatch sb)
        {
            DrawWeaponIcon(sb, equipment.CurrentPrimary(), columnPosition);
            DrawWeaponIcon(sb, equipment.CurrentOffhand(), columnPosition + new Vector2(0, spacing));
            DrawAmmoIcon(sb, equipment.CurrentPrimaryAmmo(), columnPosition + new Vector2(0, spacing * 2));
            DrawSpellIcon(sb, equipment.CurrentSpell(), columnPosition + new Vector2(0, spacing * 3));
            DrawSoulIcon(sb, equipment.SelectedSoul(), columnPosition + new Vector2(0, spacing * 4));
        }
        private void DrawWeaponIcon(SpriteBatch sb, BaseItem item, Vector2 position)
        {
            if (item != null)
            {
                DrawBoxString(sb, item.Name, position - new Vector2(-64, 3));

                if (item.IsBroken() == false && (((Weapon)item).CombatWeapon.IsUsable == true))
                    sb.Draw(item.Icon, position, whiteFade);
                else if (((Weapon)item).CombatWeapon.IsUsable == false)
                    sb.Draw(item.Icon, position, darkGrayFade);
                else if (item.IsBroken() == true)
                    sb.Draw(item.Icon, position, redFade);

                if (item.CurrentAmount > 1)
                    sb.DrawString(font, item.CurrentAmount.ToString(), position + new Vector2(3, 3), goldFade);
                if (item.CurrentDurability < item.MaxDurability)
                    sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 4, (int)position.Y + (iconBG.Height - 5), (int)(56 * ((float)item.CurrentDurability / item.MaxDurability)), 1), goldFade, charcoalFade);
            }
        }
        private void DrawAmmoIcon(SpriteBatch sb, BaseItem item, Vector2 position)
        {
            if (item != null)
            {
                DrawBoxString(sb, item.Name, position - new Vector2(-64, 3));

                if (item.IsBroken() == false)
                    sb.Draw(item.Icon, position, whiteFade);
                else
                    sb.Draw(item.Icon, position, redFade);

                if (item.CurrentAmount > 1)
                    sb.DrawString(font, item.CurrentAmount.ToString(), position + new Vector2(3, 3), goldFade);
                if (item.CurrentDurability < item.MaxDurability)
                    sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 4, (int)position.Y + (iconBG.Height - 5), (int)(56 * ((float)item.CurrentDurability / item.MaxDurability)), 1), goldFade, charcoalFade);
            }
        }
        private void DrawSoulIcon(SpriteBatch sb, BaseSoul soul, Vector2 position)
        {
            if (soul != null)
            {
                DrawBoxString(sb, soul.Name, position - new Vector2(-64, 3));

                if (soul.SoulCharges > 0)
                    sb.Draw(soul.Icon, position, whiteFade);
                else
                    sb.Draw(soul.Icon, position, redFade);

                if (soul.SoulCharges > 1)
                    sb.DrawString(font, soul.SoulCharges.ToString(), position + new Vector2(3, 3), goldFade);
                else
                    sb.DrawString(font, soul.SoulCharges.ToString(), position + new Vector2(3, 3), redFade);

                if (soul.CurrentState == BaseSoul.SoulState.Effect)
                    sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 4, (int)position.Y + (iconBG.Height - 5), (int)(56 * ((float)soul.DelayTime / soul.EffectTime)), 1), goldFade, charcoalFade);
                else if (soul.CurrentState == BaseSoul.SoulState.Cooldown)
                    sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 4, (int)position.Y + (iconBG.Height - 5), (int)(56 * ((float)soul.DelayTime / soul.CooldownTime)), 1), redFade, charcoalFade);
            }
        }
        private void DrawSpellIcon(SpriteBatch sb, BaseSpell spell, Vector2 position)
        {
            if (spell != null)
            {
                DrawBoxString(sb, spell.Name, position - new Vector2(-64, 3));

                //if (spell.IsBroken() == false)
                    sb.Draw(spell.Icon, position, whiteFade);
                //else
                //    sb.Draw(spell.Icon, position, redColor);

                //if (item.CurrentAmount > 1)
                //    sb.DrawString(font, item.CurrentAmount.ToString(), position + new Vector2(3, 3), ColorHelper.goldFade);
                //if (item.CurrentDurability < item.MaxDurability)
                //    sb.DrawBoxBordered(pixel, new Rectangle((int)position.X + 4, (int)position.Y + (iconBG.Height - 5), (int)(56 * ((float)item.CurrentDurability / item.MaxDurability)), 1), ColorHelper.goldFade, ColorHelper.Charcoal);
            }
        }
        private void DrawBoxString(SpriteBatch sb, string text, Vector2 position)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Vector2 fontOrigin = font.MeasureString(text);
                sb.DrawString(font, text, position, fontOrigin, goldFade, 1f);

                sb.Draw(pixel, new Rectangle((int)(position.X - fontOrigin.X), (int)position.Y - 1, (int)fontOrigin.X, 1), darkGrayFade);
                sb.Draw(pixel, new Rectangle((int)(position.X - fontOrigin.X), (int)position.Y + 0, (int)fontOrigin.X, 1), charcoalFade);
            }
        }

        private void DrawIconBG(SpriteBatch sb, Vector2 position)
        {
            sb.Draw(iconBG, position, whiteFade);
        }

        public void ResetPosition()
        {
            columnPosition = new Vector2(GameSettings.VectorResolution.X - 68, (GameSettings.VectorResolution.Y / 2) - (32 * 5)); //5 = number of slots
        }
    }
}
