using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.LightEngine;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters
{
    public class Arweblast : BaseShooter
    {
        protected Texture2D bowTexture;

        public Arweblast(float AccuracyDifference, float WeaponDistance, float StockLength, float FrontLength)
            : base(CombatSlotType.TwoHand, AccuracyDifference, WeaponDistance, StockLength, FrontLength)
        {

        }

        protected override void ResetVariables()
        {
            isReloaded = false;

            base.ResetVariables();
        }
        public override void ForceStop()
        {
            isForceStopped = true;
            ResetCombatState();
            ClearActions();

            base.ForceStop();
        }

        //Aim, shoot, pull string and reload... repeat.
        private bool isReloaded = true;
        protected override void CheckControls(GameTime gt)
        {
            if (isReloaded == true) //Shoot the bolt
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

                if (IsActionless == true)
                {
                    if (controls.IsClickedOnce(mouseButton))
                        RequestCombatMove(CombatMove.Basic);
                    if (controls.IsClickedOnce(antiButton))
                        RequestCombatMove(CombatMove.Power);
                }

                if (isSneaking && controls.IsClickedOnce(mouseButton))
                    RequestCombatMove(CombatMove.Sneak);
            }
            else if (isReloaded == false) //Reload the bow
            {
                PullString(gt, 2f);

                if (bowString.StringMultiplier <= 0f) //If the string has been pulled all the way back ...
                {
                    bowState = BowState.None; //... it is reloaded
                    ResetCombatState();
                    isReloaded = true;
                }
            }
        }

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
            if (CurrentAction != CombatMove.None || isReloaded == false)
            {
                sb.Draw(bowTexture, stock.Line.locationB, Color.White, handleOrigin, stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 2f, SpriteEffects.None, finalDepth);
                bowString.Draw(sb, finalDepth + .00001f);

                //Shadow
                sb.Draw(bowTexture, stock.Line.locationB - new Vector2(0, shadowOffset - CombatHeight), WorldLight.ShadowColor, handleOrigin, stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 2f, SpriteEffects.None, finalDepth - .0001f);
            }

            //if ((bowState == BowState.Aiming || bowState == BowState.Drawing) && bowString.StringMultiplier < 1f ) //If the player is aiming or pulling their bow, draw ammo
            if ((CurrentAction != CombatMove.None || isReloaded == false) && bowString.StringMultiplier < 1f)
            {
                if (ammo != null && ammo.ProjectileTexture != null)
                {
                    sb.Draw(ammo.ProjectileTexture, bowString.StringDrawPosition, Color.White, new Vector2(0, ammo.ProjectileTexture.Height), stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 1f, SpriteEffects.None, finalDepth + .0001f);
                    sb.Draw(ammo.ProjectileTexture, bowString.StringDrawPosition + new Vector2(0, CombatHeight), WorldLight.ShadowColor, new Vector2(0, ammo.ProjectileTexture.Height), stock.Line.locationA.Direction(stock.Line.locationB) + angleOffset, 1f, SpriteEffects.None, finalDepth - .0002f);
                }
            }

            base.Draw(sb);
        }
    }
}
