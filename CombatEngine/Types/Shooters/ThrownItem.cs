using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.WeaponsTypes;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters
{
    public class ThrownItem : BaseStriker
    {
        protected new ThrownWeapon weapon;
        protected float accuracyDifference;
        protected bool drawWeapon = true;

        public ThrownItem(float ItemLength, float WeaponDistance, float AccuracyDifference, float MaxReach)
            : base(CombatSlotType.OneHand, ItemLength, WeaponDistance, MaxReach)
        {
            accuracyDifference = AccuracyDifference;
        }

        public override void Load(ContentManager main, ContentManager map)
        {
            base.Load(main, map);
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictCombat)
        {
            if (mouseButton == ScreenEngine.Options.Controls.MouseButton.LeftClick)
                weapon = (ThrownWeapon)entity.EQUIPMENT_PrimaryWeapon();
            else if (mouseButton == ScreenEngine.Options.Controls.MouseButton.RightClick)
                weapon = (ThrownWeapon)entity.EQUIPMENT_OffhandWeapon();

            base.Update(gt, checkControls, restrictCombat);
        }

        protected override void ResetVariables()
        {
            drawWeapon = true;
            base.ResetVariables();
        }
        public override void ForceStop()
        {
            base.ForceStop();
        }

        //Sling, sling, sling...
        protected override void CheckControls(GameTime gt)
        {
            /*if (IsActionless == true)
            {
                if (controls.IsClickedOnce(mouseButton))
                    RequestCombatMove(CombatMove.Basic);
                if (controls.IsClickedOnce(antiButton))
                    RequestCombatMove(CombatMove.Power);
            }

            if (isSneaking && controls.IsClickedOnce(mouseButton))
                RequestCombatMove(CombatMove.Sneak);*/

            base.CheckControls(gt);
        }
        protected override void CheckEntityControls(GameTime gt)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
            if (weapon != null)
                DrawGenericStriker(sb, new Vector2(0, weapon.ProjectileTexture.Height), weapon.ProjectileTexture, scale);

            base.Draw(sb);
        }
    }
}
