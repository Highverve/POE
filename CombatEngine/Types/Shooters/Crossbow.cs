using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Debugging;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.LightEngine;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters
{
    public class Crossbow : BaseShooter
    {
        protected Texture2D bowTexture;

        public Crossbow(float BaseAccuracyDifference, float WeaponDistance, float StockLength, float FrontLength)
            : base(CombatSlotType.OneHand, BaseAccuracyDifference, WeaponDistance, StockLength, FrontLength)
        {
            customDrawSpeed = 2f;
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictControls)
        {
            base.Update(gt, checkControls, restrictControls);

            stock.PositionA = CalculateRelativeVector(directionAngle + handleAngle, center, FinalDistance + reach, CombatHeight);
            stock.PositionB = CalculateRelativeVector(directionAngle + endAngle, stock.PositionA, stockLength, 0);

            Vector2 cross = (stock.PositionA - stock.PositionB).Cross();
            if (cross != Vector2.Zero)
                cross.Normalize();

            bowLine.PositionA = stock.PositionB + (cross * (weaponLength / 2)); //Far end of the stock, right side
            bowLine.PositionB = stock.PositionB + -(cross * (weaponLength / 2)); //Far end of the stock, opposite side

            bowString.Update(gt, stock.PositionA, normalizedEnd, shadowOffset, CombatHeight);
        }
        protected override void UpdateBehavior(GameTime gt)
        {
            switch (bowState)
            {
                case BowState.Drawing: PullString(gt, customDrawSpeed); break;
                case BowState.Releasing: { ReleaseString(gt); } break;
            }
        }
        protected override void CheckControls(GameTime gt)
        {
            base.CheckControls(gt);
        }

        protected override void ResetVariables()
        {
            bowString.StringMultiplier = 1f;

            base.ResetVariables();
        }
        public override void ForceStop()
        {
            isForceStopped = true;
            ResetCombatState();
            ClearActions();

            base.ForceStop();
        }

        //Marked for removal
        private bool isReloaded = true;
        protected override void CheckEntityControls(GameTime gt)
        {
            if (isReloaded == true)
            {
                bowState = BowState.Aiming;

                if (CurrentAction != CombatMove.None)
                {
                    ReleaseString(gt);

                    //Wait time
                    aftershotTime += gt.ElapsedGameTime.Milliseconds;
                    if (aftershotTime > (500 / weaponSpeed))
                        ResetVariables();
                }
            }
            else if (isReloaded == false)
            {
                PullString(gt, 2f);

                if (bowString.StringMultiplier <= 0f) //If the string has been pulled all the way back ...
                {
                    bowState = BowState.None; //... it is reloaded
                    isReloaded = true;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (scale > 0)
            {
                sb.Draw(bowTexture, stock.PositionB, Color.White, handleOrigin, stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, scale, SpriteEffects.None, finalDepth);
                sb.Draw(bowTexture, CalculateShadow(stock.PositionB), WorldLight.ShadowColor, handleOrigin, stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, scale, SpriteEffects.None, finalDepth - .001f);
            }

            if (scale >= baseScale)
                bowString.Draw(sb, finalDepth - .00002f);

            if ((bowState == BowState.Drawing || bowState == BowState.Aiming) && scale > 0f)
            {
                if (ammo != null && ammo.ProjectileTexture != null)
                {
                    sb.Draw(ammo.ProjectileTexture, bowString.StringDrawPosition, Color.White, new Vector2(0, ammo.ProjectileTexture.Height), stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 1f, SpriteEffects.None, finalDepth + .001f);
                    sb.Draw(ammo.ProjectileTexture, CalculateShadow(bowString.StringDrawPosition), WorldLight.ShadowColor, new Vector2(0, ammo.ProjectileTexture.Height), stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 1f, SpriteEffects.None, finalDepth - .001f);
                }
            }

            base.Draw(sb);
        }
    }
}
