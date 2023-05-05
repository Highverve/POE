using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters.Crossbows
{
    public class TestCrossbow : Crossbow
    {
        public TestCrossbow() : base(.05f, 30f, 10f, 100f) { }

        public override void Load(ContentManager main, ContentManager map)
        {
            bowTexture = main.Load<Texture2D>("Combat/Shooters/Crossbows/crossbow");

            SetPositions(new Vector2(39, 25), new Vector2(17, 11), new Vector2(52, 46), 105f, .5f, 0f);
            SetClickBools(false, true, true, true, true, true, true, true);

            minReach = -25f;
            maxReach = 30f;

            customDrawSpeed = 2.5f;

            base.Load(main, map);
        }

        public override void Basic(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(0f, 2000);
                isUpdateDirection = true;

                //Add shooting action
                shootingAction = new Action(() =>
                {
                    FireProjectile();
                    aftershotTime = 1;
                });
                bowState = BowState.Aiming;

                //If the character has not done this same action before
                if (LastAction != CombatMove.Basic)
                {
                    //Move handle to aiming position
                    AddAction(() =>
                    {
                        Adjust(gt, ref multiplier1, 3f);
                        multiplier1 = MathHelper.Clamp(multiplier1, 0f, 1f);
                    }, 0, 500);
                    AddAction(() => { handleAngle = MathHelper.SmoothStep(1.25f * dualWieldDirection, .25f * dualWieldDirection, multiplier1); }, 0, 500);

                    //Extend reach
                    AddAction(() =>
                    {
                        Adjust(gt, ref multiplier2, 3f);
                        multiplier2 = MathHelper.Clamp(multiplier2, 0f, 1f);
                    }, 0, 500);
                    AddAction(() => { reach = MathHelper.SmoothStep(-25f, 15f, multiplier2); }, 0, 600);
                } //Else, continue shooting after reloading
                else
                {
                    reach = 15f;
                    handleAngle = .25f * dualWieldDirection;
                }

                //Pause animation when crossbow position has been set.
                AddAction(() => { isMoveAnimPaused = true; }, 500, 550);
                
                //Release string when player has let go of MB
                AddAction(() => { bowState = BowState.Releasing; }, 550, 600);

                //Begin reloading
                AddAction(() => { bowState = BowState.Drawing; }, 1000, 2000);

                isAdded = true;
            }
        }
        public override void Power(GameTime gt)
        {
        }
        public override void Jump(GameTime gt)
        {
        }
        public override void Roll(GameTime gt)
        {
        }
        public override void Sneak(GameTime gt)
        {
            if (isAdded == false)
            {
                bowState = BowState.None;
                bowString.StringMultiplier = 0f;

                AssignVariables(-1.5f, 1000, 0f, .3f);

                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 300);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 300, maxCombatTime);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 5f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .25f); }, 300, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 300);

                //Moving smoothly
                AddAction(() => { reach = MathHelper.SmoothStep(20f, 30f, multiplier1); }, 0, maxCombatTime);
                AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (3.5f * dualWieldDirection), multiplier2); }, 0, maxCombatTime);
                
                //AddAction(() => { endAngle += 2f * (float)gt.ElapsedGameTime.TotalSeconds; }, 0, maxCombatTime);
                //AddAction(() => { handleAngle += 2f * (float)gt.ElapsedGameTime.TotalSeconds; }, 0, maxCombatTime);

                isAdded = true;
            }

            //shootingAction = new Action(() =>
            //{
            //    accuracyDifference = baseAccuracyDifference * .5f;
            //    FireProjectile();
            //});
        }
        public override void Sprint(GameTime gt)
        {
            shootingAction = new Action(() =>
            {
                FireProjectile();
            });
        }

        public override void Offhand(GameTime gt)
        {
        }
        public override void BehindShield(GameTime gt)
        {
        }
    }
}
