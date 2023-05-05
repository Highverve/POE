using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters
{
    public class TestBow : Bow
    {
        public TestBow() : base(.05f, 60f, 130f) { }

        public override void Load(ContentManager main, ContentManager map)
        {
            bowTexture = main.Load<Texture2D>("Combat/Shooters/stringlesss");
            SetPositions(new Vector2(37, 26), new Vector2(2, 2), new Vector2(61, 61), 90f, .6f);
            SetClickBools(false, true, true, true, true, true, true, true);

            base.Load(main, map);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void Basic(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(0f, 750);
                isUpdateDirection = true;

                //Add shooting action
                shootingAction = new Action(() =>
                {
                    if (ShotCount == 1)
                    {
                        FireProjectile();
                        aftershotTime = (int)(750 * WeaponSpeed);
                        ShotCount++;
                    }
                });
                bowState = BowState.Drawing;

                //Pause animation when bow drawing has been set.
                AddAction(() => { isMoveAnimPaused = true; }, 0, 150);

                //Release string when player has let go of MB
                AddAction(() => { bowState = BowState.Releasing; }, 150, 200);
                AddAction(() => { bowState = BowState.None; }, 500, maxCombatTime);

                isAdded = true;
            }
        }
        public override void Power(GameTime gt)
        {
            shootingAction = new Action(() =>
            {
                accuracyDifference = baseAccuracyDifference * 2f;
                FireProjectile();
                FireProjectile();
            });
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
                AssignVariables(-1.5f, 1000, 0f, .3f);

                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 300);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 300, maxCombatTime);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 5f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .25f); }, 300, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 300);

                //Moving smoothly
                AddAction(() => { reach = MathHelper.SmoothStep(0f, 15f, multiplier1); }, 0, maxCombatTime);
                AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (3.5f * dualWieldDirection), multiplier2); }, 0, maxCombatTime);

                //Animation
                AddAction(() => SetCurrentFrame(0, 1), 0, 50);
                AddAction(() => SetCurrentFrame(1, 1), 50, 100);
                AddAction(() => SetCurrentFrame(2, 1), 100, 150);
                AddAction(() => SetCurrentFrame(3, 1), 150, 200); //Glowing fully

                AddAction(() => SetCurrentFrame(2, 1), 650, 700);
                AddAction(() => SetCurrentFrame(1, 1), 700, 750);
                AddAction(() => SetCurrentFrame(0, 1), 750, 800);

                isAdded = true;
            }
            /*
            shootingAction = new Action(() =>
            {
                accuracyDifference = baseAccuracyDifference * .5f;
                FireProjectile();
            });*/
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
