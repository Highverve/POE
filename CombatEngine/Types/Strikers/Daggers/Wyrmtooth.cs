using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Daggers
{
    public class Wyrmtooth : BaseStriker
    {
        public Wyrmtooth() : base(CombatSlotType.OneHand, 32f, 10f, 50f) { }

        public override void Load(ContentManager main, ContentManager map)
        {
            swordTexture = main.Load<Texture2D>(strikerDirectory + "Daggers/wyrmtoothDagger");
            baseScale = 2f;

            SetAnimationValues(new Point(64, 64), new Point(4, 1));
            SetClickBools(true, true, true, false, false, true, true, true);

            weaponSpeed = 1.5f;

            base.Load(main, map);
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictCombat)
        {

            base.Update(gt, checkControls, restrictCombat);
        }

        public override void Basic(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(-1.5f, 1000, 0f, .3f);
                TextureDirection = SpriteEffects.FlipHorizontally;
                entity.SUSPEND_Action(maxCombatTime);
                entity.SUSPEND_Movement(maxCombatTime);

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

                //Damage range
                AddAction(() =>
                {
                    strikerLine.IsActive = true;
                }, 450, 650);
                AddAction(() =>
                {
                    strikerLine.IsActive = false;
                }, 650, 700);

                isAdded = true;
            }
        }

        public override void Power(GameTime gt)
        {
            AssignVariables(2.1f, 100, 0f, .2f);

            if (isAdded == false)
            {
                //AddAction(() => { }, 0, 800);


                isAdded = true;
            }
        }

        public override void Jump(GameTime gt)
        {
        }
        public override void Roll(GameTime gt)
        {
        }

        private int sneakTimer = 0;
        public override void Sneak(GameTime gt)
        {
            AssignVariables(2.1f, -1, 0f, .2f);

            isUpdateDirection = true;

            if (sneakTimer >= 200)
            {
                FireCustom(1, strikerLine.PositionB, mouseDirection, -.05f, 1f, 1f, DecreaseProjectile.None, 1);

                ResetVariables();
                sneakTimer = 0;
            }
            else
                sneakTimer += gt.ElapsedGameTime.Milliseconds;

            //combatTime = 0;
        }
        public override void Sprint(GameTime gt)
        {
            AssignVariables(-1.5f, 1000, 0f, .3f);

            if (isAdded == false)
            {
                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 300);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 300, maxCombatTime);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 5f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2f); }, 300, maxCombatTime);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .25f); }, 300, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 300);

                //Moving smoothly
                AddAction(() => MoveSmoothly(gt, ref reach, 0f, 15f, multiplier1), 0, maxCombatTime);
                AddAction(() => MoveSmoothly(gt, ref directionAngle, startingAngle, startingAngle + (3.5f * dualWieldDirection), multiplier2), 0, maxCombatTime);

                //Pause Animation
                AddAction(() => { isMoveAnimPaused = true; }, 500, 520);

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
        }

        public override void Offhand(GameTime gt)
        {
        }
        public override void BehindShield(GameTime gt)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawGenericStriker(sb, new Vector2(11, 52), swordTexture, scale);

            base.Draw(sb);
        }
    }
}
