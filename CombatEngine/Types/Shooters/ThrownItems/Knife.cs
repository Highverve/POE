using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters.ThrownItems
{
    public class Knife : ThrownItem
    {
        public Knife() : base(32f, 15f, .05f, 50f)
        {
            weaponSpeed = 1.25f;

            baseScale = 1f;
            SetAnimationValues(new Point(64, 64), Point.Zero);
        }

        public override void Basic(GameTime gt)
        {
            AssignVariables(-1.5f, 850, 0f, .3f);

            if (isAdded == false)
            {
                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 200);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 200, maxCombatTime);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 4f); }, 200, maxCombatTime);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2f); }, 200, maxCombatTime);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .25f); }, 200, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 200);

                //Moving smoothly
                AddAction(() => MoveSmoothly(gt, ref reach, 0f, 50f, multiplier1), 0, maxCombatTime);
                AddAction(() => MoveSmoothly(gt, ref directionAngle, startingAngle, startingAngle + (1.5f * dualWieldDirection), multiplier2), 0, maxCombatTime);

                AddAction(() => MoveEnd(gt, -5f * dualWieldDirection), 0, 300);
                AddAction(() => MoveEnd(gt, 8f * dualWieldDirection), 300, maxCombatTime);

                AddAction(() =>
                {
                    FireCustom(weapon.ProjectileID, strikerLine.Line.locationA, mouseDirection, .05f, 1f, 1f, DecreaseProjectile.Weapon, 1);
                }, 350, maxCombatTime);

                //AddAction(() => { this.drawWeapon = false; weapon.CurrentDurability = weapon.MaxDurability; }, 350, maxCombatTime);
                AddAction(() => { entity.SUSPEND_Action(maxCombatTime); }, 0, maxCombatTime);

                isAdded = true;
            }
        }
        public override void Power(GameTime gt)
        {
            AssignVariables(2.1f, 800, 0f, .2f);

            if (isAdded == false)
            {
                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 3f), 0, 300);
                AddAction(() => Adjust(gt, ref multiplier1, -.75f), 300, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -.5f), 0, 200);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 3f); }, 200, maxCombatTime);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 1f); }, 200, maxCombatTime);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .5f); }, 200, maxCombatTime);

                //Height
                AddAction(() => MoveStrikerHeight(gt, -50f), 0, 300);
                AddAction(() => MoveStrikerHeight(gt, 100f), 300, 600);

                AddAction(() => FireCustom(weapon.ProjectileID, strikerLine.Line.locationA, mouseDirection, .025f, 1f, 1f, DecreaseProjectile.Weapon, 1), 360, maxCombatTime);
                AddAction(() => FireCustom(weapon.ProjectileID, strikerLine.Line.locationA, mouseDirection, .025f, 1f, 1f, DecreaseProjectile.Weapon, 2), 360, maxCombatTime);
                AddAction(() => FireCustom(weapon.ProjectileID, strikerLine.Line.locationA, mouseDirection, .025f, 1f, 1f, DecreaseProjectile.Weapon, 3), 360, maxCombatTime);

                AddAction(() => { drawWeapon = false; weapon.CurrentDurability = weapon.MaxDurability; }, 350, maxCombatTime);

                //Reach and length
                AddAction(() => MoveSmoothly(gt, ref reach, 0f, 15f, multiplier1), 0, maxCombatTime);
                AddAction(() => MoveSmoothly(gt, ref directionAngle, startingAngle, startingAngle - (4f * dualWieldDirection), multiplier2), 0, maxCombatTime);

                isAdded = true;
            }
        }
        public override void Jump(GameTime gt)
        {
        }
        public override void Roll(GameTime gt)
        {
        }
        public override void Sneak(GameTime gt)
        {
            AssignVariables(-1.5f, 850, 0f, .3f);

            if (isAdded == false)
            {
                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 200);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 200, maxCombatTime);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 4f); }, 200, maxCombatTime);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2f); }, 200, maxCombatTime);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .25f); }, 200, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 200);

                //Moving smoothly
                AddAction(() => MoveSmoothly(gt, ref reach, 0f, 50f, multiplier1), 0, maxCombatTime);
                AddAction(() => MoveSmoothly(gt, ref directionAngle, startingAngle, startingAngle + (3.5f * dualWieldDirection), multiplier2), 0, maxCombatTime);

                //AddAction(() => FireCustom(weapon, .025f, 1f, 1f, 1), 360, maxCombatTime);

                //AddAction(() => { this.drawWeapon = false; weapon.CurrentDurability = weapon.MaxDurability; }, 350, maxCombatTime);
                AddAction(() => { entity.SUSPEND_Action(maxCombatTime); }, 0, maxCombatTime);

                isAdded = true;
            }
        }
        public override void Sprint(GameTime gt)
        {
        }

        public override void Offhand(GameTime gt)
        {
        }
        public override void BehindShield(GameTime gt)
        {
        }
    }
}
