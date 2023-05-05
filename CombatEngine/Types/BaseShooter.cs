using System;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.CombatEngine.Types.Shooters;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.WeaponsTypes;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.CombatEngine.Types
{
    public class BaseShooter : BaseCombat
    {
        protected WeaponCollision stock = new WeaponCollision("Stock"), bowLine = new WeaponCollision("BowLine");

        protected Line stringShadowA = new Line(), stringShadowB = new Line(); //bow string lines and bow
        protected Vector2 handleOrigin; //Used for positioning the strings on the bow
        protected float accuracyDifference, baseAccuracyDifference, stockLength, customDrawSpeed;

        protected BowString bowString;

        protected int aftershotTime = 0; //The wait time after shooting.

        public BaseShooter(CombatSlotType SlotType, float BaseAccuracyDifference, float WeaponDistance, float StockLength, float FrontLength)
            : base(Skills.WeaponCategory.Shooter, SlotType, FrontLength, WeaponDistance)
        {
            baseAccuracyDifference = BaseAccuracyDifference;
            accuracyDifference = baseAccuracyDifference;
            stockLength = StockLength;

            baseScale = 2f;
            customDrawSpeed = 1f;
        }
        public void SetPositions(Vector2 HandleOrigin, Vector2 StringSlotA, Vector2 StringSlotB, float HandleDistance, float MaximumDraw, float StartingStringMultiplier = 1f)
        {
            handleOrigin = HandleOrigin;
            bowString = new BowString(StringSlotA, StringSlotB, handleOrigin, HandleDistance, 0f, MaximumDraw, pixel);
            bowString.StringMultiplier = StartingStringMultiplier;
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictControls)
        {
            base.Update(gt, checkControls, restrictControls);

            if (this is Bow)
            {
                if (ammo != null)
                {
                    if (ammo.ammo == Ammo.AmmoType.Bolt)
                        ammo = null; //Remove bow-and-bolt combinations
                }
            }

            UpdateBehavior(gt);
        }
        protected virtual void UpdateBehavior(GameTime gt)
        {
            switch (bowState)
            {
                case BowState.Drawing: PullString(gt, customDrawSpeed); break;
                case BowState.Releasing:
                {
                    ReleaseString(gt);

                    //Wait time
                    aftershotTime += gt.ElapsedGameTime.Milliseconds;
                    if (aftershotTime > (500 / weaponSpeed))
                        ResetCombatState();

                } break;
            }
        }

        protected override void UpdateDepth()
        {
            if (stock.PositionB.Y > center.Y)
                finalDepth = baseDepth + .0001f;
            else
                finalDepth = baseDepth - .0001f;
        }
        public enum BowState { Drawing, Releasing, Aiming, None }
        protected BowState bowState = BowState.None;

        protected void PullString(GameTime gt, float customSpeed = 2f)
        {
            bowString.PullString(gt, weaponSpeed, customSpeed);
        }
        protected void ReleaseString(GameTime gt)
        {
            //Fire projectile
            if (shootingAction != null && aftershotTime == 0)
                shootingAction.Invoke();

            //Release string
            bowString.ReleaseString(gt);
        }

        protected int GetTotalDamage()
        {
            int returnValue = 0;

            if (weapon != null)
                returnValue += weapon.ProjectileDamage;

            returnValue += ArcheryDamage();

            return returnValue;
        }
        protected float AccuracyOffset()
        {
            return random.NextFloat((-accuracyDifference) * dualWieldDirection, (accuracyDifference * .5f) * dualWieldDirection) * entity.ARCHERY_Accuracy();
        }
        public void FireProjectile()
        {
            float accuracyOffset = AccuracyOffset();
            float speed = MathHelper.Clamp(bowString.StringMultiplier, .25f, 1f);
            float damage = MathHelper.Clamp(bowString.StringMultiplier, .25f, 1f);

            if (ammo != null)
            {
                tileMap.AddProjectile(ammo.ProjectileID, bowString.StringDrawPosition, directionAngle + accuracyOffset, speed, damage, currentFloor, entity.MapEntityID, GetTotalDamage());
                ammo.CurrentAmount--;
            }
        }

        protected Action shootingAction;

        protected bool assignShotSpeed = false;
        protected override void ResetVariables()
        {
            bowState = BowState.None;
            accuracyDifference = baseAccuracyDifference;
            shootingAction = null;
            aftershotTime = 0;
            assignShotSpeed = false;

            base.ResetVariables();
        }

        public override void ForceStop()
        {
            CombatHeight = 0f;

            aftershotTime = 0;
            bowState = BowState.None;
            shootingAction = null;
            accuracyDifference = baseAccuracyDifference;
            assignShotSpeed = false;

            //bowString.ResetString();

            isForceStopped = true;
            ResetCombatState();
            ClearActions();

            base.ForceStop();
        }

        public override BaseCombat Copy()
        {
            BaseShooter copy = (BaseShooter)base.Copy();

            copy.stock = new WeaponCollision("Stock");
            copy.bowLine = new WeaponCollision("BowLine");

            copy.lines.Add(copy.stock);
            copy.lines.Add(copy.bowLine);

            if (bowString != null)
                copy.bowString = bowString.Copy();

            return copy;
        }
    }
}
