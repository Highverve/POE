using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.LightEngine;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters
{
    public class Bow : BaseShooter
    {
        protected Texture2D bowTexture;

        public Bow(float AccuracyDifference, float WeaponDistance, float FrontLength)
            : base(CombatSlotType.TwoHand, AccuracyDifference, WeaponDistance, 15f, FrontLength)
        {
            customDrawSpeed = 2f;
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictControls)
        {
            base.Update(gt, checkControls, restrictControls);

            //stock.PositionA = CalculateRelativeVector(directionAngle + handleAngle, center, FinalDistance + reach, FinalCombatHeight);
            //stock.PositionB = CalculateRelativeVector(directionAngle + endAngle, stock.PositionA, stockLength, 0);

            stock.PositionA = CalculateRelativeVector(directionAngle + handleAngle, center, FinalDistance + reach, CombatHeight);
            stock.PositionB = CalculateRelativeVector(directionAngle + endAngle, stock.PositionA, stockLength, 0);

            //stock.PositionB = CalculateRelativeVector(directionAngle + endAngle, center, FinalDistance + (stockLength / 2) + reach, FinalCombatHeight);
            //stock.PositionA = CalculateRelativeVector(directionAngle + handleAngle, center, FinalDistance - (stockLength / 2) + reach, FinalCombatHeight);

            Vector2 cross = (stock.PositionA - stock.PositionB).Cross();
            if (cross != Vector2.Zero)
                cross.Normalize();

            bowLine.PositionA = stock.PositionB + (cross * (weaponLength / 2)); //Far end of the stock, right side
            bowLine.PositionB = stock.PositionB + -(cross * (weaponLength / 2)); //Far end of the stock, opposite side

            //bowLine.PositionB = CalculateRelativeVector(directionAngle + handleAngle + .8f, center, FinalDistance + reach + 30, FinalCombatHeight);
            //bowLine.PositionA = CalculateRelativeVector(directionAngle + handleAngle - .8f, center, FinalDistance + reach + 30, FinalCombatHeight);

            bowString.Update(gt, stock.PositionA, normalizedEnd, shadowOffset, CombatHeight);
        }

        //Aim, pull string, release string and shoot... repeat.
        protected override void CheckControls(GameTime gt)
        {
            base.CheckControls(gt);
        }

        private float shotSpeed;
        protected override void CheckEntityControls(GameTime gt)
        {
            if (CurrentAction != CombatMove.None) //If the current action is not nothing
            {
                if (assignShotSpeed == false) //Assign the shotSpeed required before releasing
                {
                    bowState = BowState.Drawing;
                    shotSpeed = random.NextFloat(.05f, .4f);

                    assignShotSpeed = true;
                }

                if (bowString.StringMultiplier < shotSpeed) //Release the bowstring when shotSpeed is reached
                    bowState = BowState.Releasing;

                if (bowState == BowState.Releasing)
                {
                    ReleaseString(gt);

                    //Wait time
                    aftershotTime += gt.ElapsedGameTime.Milliseconds;
                    if (aftershotTime > (500 / weaponSpeed))
                        ResetVariables();
                }
                if (bowState == BowState.Drawing)
                    PullString(gt); //Pull back the bowstring
            }
        }

        protected override void ResetVariables()
        {
            bowString.StringMultiplier = 0f;

            base.ResetVariables();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (scale > 0f)
            {
                sb.Draw(bowTexture, stock.PositionB, Color.White, handleOrigin, stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, scale, SpriteEffects.None, finalDepth);
                sb.Draw(bowTexture, CalculateShadow(stock.PositionB), WorldLight.ShadowColor, handleOrigin, stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, scale, SpriteEffects.None, finalDepth - .001f);

                bowString.Draw(sb, finalDepth - .00002f);
            }

            if (bowState == BowState.Drawing)
            {
                if (ammo != null && ammo.ProjectileTexture != null)
                {
                    sb.Draw(ammo.ProjectileTexture, bowString.StringDrawPosition, Color.White, new Vector2(0, ammo.ProjectileTexture.Height), stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 1f, SpriteEffects.None, finalDepth - .0005f);
                    sb.Draw(ammo.ProjectileTexture, CalculateShadow(bowString.StringDrawPosition), WorldLight.ShadowColor, new Vector2(0, ammo.ProjectileTexture.Height), stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 1f, SpriteEffects.None, finalDepth - .001f);
                }
            }

            base.Draw(sb);
        }
    }
}
