using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Swords
{
    public class TarnishedShortsword : BaseStriker
    {
        public TarnishedShortsword() : base(CombatSlotType.OneHand, 74f, 0f, 40f) { }

        public override void Load(ContentManager main, ContentManager map)
        {
            swordTexture = main.Load<Texture2D>(strikerDirectory + "Swords/tarnishedShortsword");
            baseScale = 2f;

            SetAnimationValues(new Point(64, 64), Point.Zero);
            SetClickBools(true, true, true, false, true, true, true, false);

            attributeNames = Entities.Entities.BaseEntity.ATTRIBUTE_CombatPhysicalMultipliers;

            base.Load(main, map);
        }

        public override void Basic(GameTime gt)
        {
            if (isAdded == false)
            {
                switch(comboSwing)
                {
                    case 1: BasicComboFirst(gt); break;
                    case 2: BasicComboSecond(gt); break;
                }

                isAdded = true;
            }
        }
        private void BasicComboFirst(GameTime gt)
        {
            AssignVariables(-1.5f, 600, 0f, .3f);
            entity.STAMINA_Damage(25f);

            //Multi's
            AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 200);
            AddAction(() => Adjust(gt, ref multiplier1, -2f), 200, maxCombatTime);

            AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 6f); }, 200, maxCombatTime);
            AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2.5f); }, 200, maxCombatTime);
            AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .5f); }, 200, maxCombatTime);
            AddAction(() => Adjust(gt, ref multiplier2, -2f), 0, 200);

            //Moving smoothly
            AddAction(() => { reach = MathHelper.SmoothStep(0f, 25f, multiplier1); }, 0, maxCombatTime);
            AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (2 * dualWieldDirection), multiplier2); }, 0, maxCombatTime);
            AddAction(() => { endAngle = MathHelper.SmoothStep(-.5f * dualWieldDirection, 1f * dualWieldDirection, multiplier2); }, 0, maxCombatTime);

            //Motion controlling
            AddAction(() => { entity.ControlMotion((normalizedDirection.X * 20) * (multiplier1), (normalizedDirection.Y * 20) * (multiplier1), true); }, 200, 450);
            AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 200, maxCombatTime);
            AddAction(() => { entity.ResetMovement(); }, 200, maxCombatTime);
            AddAction(() => { entity.STATE_SetAction("Idle"); }, 0, maxCombatTime);

            //Damage range
            AddAction(() => { strikerLine.IsActive = true; }, 250, 550);
            AddAction(() => { strikerLine.IsActive = false; }, 550, 600);
        }
        private void BasicComboSecond(GameTime gt)
        {
            AssignVariables(1.5f, 500, .2f, .3f);
            entity.STAMINA_Damage(30f);

            //Multi's
            AddAction(() => Adjust(gt, ref multiplier1, -2f), 0, 100);
            AddAction(() => Adjust(gt, ref multiplier1, 4f), 100, 450);

            AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 6f); }, 100, maxCombatTime);
            AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2.5f); }, 100, maxCombatTime);
            AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .5f); }, 100, maxCombatTime);
            AddAction(() => Adjust(gt, ref multiplier2, -2f), 0, 100);

            //Moving smoothly
            AddAction(() => { reach = MathHelper.SmoothStep(0f, 45f, multiplier2); }, 0, maxCombatTime);
            AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (-1.75f * dualWieldDirection), multiplier2); }, 0, maxCombatTime);
            AddAction(() => { endAngle = MathHelper.SmoothStep(-.5f, 1.25f, multiplier2); }, 0, maxCombatTime);

            //Motion controlling
            AddAction(() => { entity.ControlMotion((normalizedDirection.X * 20) * (multiplier1), (normalizedDirection.Y * 20) * (multiplier1), true); }, 100, 450);
            AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 100, maxCombatTime);
            AddAction(() => { entity.ResetMovement(); }, 100, maxCombatTime);
            AddAction(() => { entity.STATE_SetAction("Idle"); }, 0, maxCombatTime);

            //Damage range
            AddAction(() => { strikerLine.IsActive = true; }, 150, 450);
            AddAction(() => { strikerLine.IsActive = false; }, 450, 500);
        }

        public override void Sneak(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(-1.5f, 900, 0f, .3f);
                entity.STAMINA_Damage(35f);

                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, -1.5f), 0, 200);
                AddAction(() => Adjust(gt, ref multiplier1, 4f), 200, 650);
                AddAction(() => Adjust(gt, ref multiplier1, -2.5f), 800, 900);

                AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 7f); }, 200, 550);
                AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2.5f); }, 200, 550);
                AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .5f); }, 200, 550);
                AddAction(() => Adjust(gt, ref multiplier2, -2f), 0, 200);

                AddAction(() => IncreaseSwingInteger(), 550, 600);
                AddAction(() => Adjust(gt, ref multiplier2, -3.5f), 500, 600);
                AddAction(() => Adjust(gt, ref multiplier2, -7f), 600, 800);
                AddAction(() => Adjust(gt, ref multiplier2, -3.5f), 800, 850);
                AddAction(() => Adjust(gt, ref multiplier2, -.5f), 850, 900);

                //Moving smoothly
                AddAction(() => { reach = MathHelper.SmoothStep(0f, 15f, multiplier1); }, 0, maxCombatTime);
                AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (2f * dualWieldDirection), multiplier2); }, 0, maxCombatTime);
                AddAction(() => { endAngle = MathHelper.SmoothStep(-.5f * dualWieldDirection, 1f * dualWieldDirection, multiplier2); }, 0, maxCombatTime);

                //Motion controlling
                AddAction(() => { entity.ControlMotion((normalizedDirection.X * 20) * (multiplier2), (normalizedDirection.Y * 20) * (multiplier2), true); }, 200, 450);
                AddAction(() => { entity.ControlMotion((normalizedDirection.X * 25) * (multiplier2), (normalizedDirection.Y * 25) * (multiplier2), true); }, 550, 800);
                AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 200, maxCombatTime);
                AddAction(() => { entity.ResetMovement(); }, 200, maxCombatTime);
                AddAction(() => { entity.STATE_SetAction("Idle"); }, 0, maxCombatTime);

                //Damage range
                AddAction(() => { strikerLine.IsActive = true; }, 250, 550);
                AddAction(() => { strikerLine.IsActive = false; }, 500, 550);
                AddAction(() => { strikerLine.IsActive = true; }, 550, 850);
                AddAction(() => { strikerLine.IsActive = false; }, 850, 900);

                isAdded = true;
            }
        }
        public override void Sprint(GameTime gt)
        {

        }
        public override void Jump(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(0f, 1000, 0f, .3f);
                AddAction(() =>
                {
                    startingAngle = -6f;
                    if (entity != null)
                    {
                        if (entity.Gravity > 0)
                            entity.Gravity = -.5f;
                        else
                            entity.Gravity -= .5f;

                        entity.FinalMotion *= .5f;
                    }
                }, 0, 20);
                TextureDirection = SpriteEffects.FlipHorizontally;

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
                AddAction(() => { strikerLine.IsActive = true; }, 450, 650);
                AddAction(() => { strikerLine.IsActive = false; }, 650, 700);

                isAdded = true;
            }
        }
        public override void BehindShield(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(0, 900, .2f);
                AddAction(() => { endAngle = random.NextFloat(-.3f, 0f); }, 0, 50);
                float randHandle = random.NextFloat(.5f, 1f);

                //Reach anmimation
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 0, 200);
                AddAction(() => Adjust(gt, ref multiplier1, 1f), 200, 250);
                AddAction(() => Adjust(gt, ref multiplier1, 8f), 250, 400);
                AddAction(() => Adjust(gt, ref multiplier1, 2f), 400, 450);
                AddAction(() => Adjust(gt, ref multiplier1, -3f), 450, 850);
                AddAction(() => Adjust(gt, ref multiplier1, -1f), 850, 900);

                AddAction(() => { reach = MathHelper.SmoothStep(0f, 100f, multiplier1); }, 0, maxCombatTime);
                AddAction(() => { CombatHeight = MathHelper.SmoothStep(0, 32f * randHandle, multiplier1); }, 0, maxCombatTime);

                //Handle animation
                AddAction(() => { handleAngle = MathHelper.SmoothStep(1.25f * dualWieldDirection * randHandle, .2f * dualWieldDirection, multiplier1); }, 0, maxCombatTime);

                //Motion controlling
                AddAction(() => { entity.ControlMotion((normalizedDirection.X * 50) * (multiplier1 - .2f), (normalizedDirection.Y * 50) * (multiplier1 - .2f), true); }, 0, maxCombatTime);

                //Pause animation
                AddAction(() => { isMoveAnimPaused = true; }, 150, 200);
                isUpdateDirection = true;

                //Damage range
                AddAction(() => { strikerLine.IsActive = true; }, 250, 450);
                AddAction(() => { strikerLine.IsActive = false; }, 450, 500);

                isAdded = true;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawGenericStriker(sb, new Vector2(6, 60), swordTexture, scale);

            base.Draw(sb);
        }

        public override BaseCombat Copy()
        {
            TarnishedShortsword copy = (TarnishedShortsword)base.Copy();

            copy.blur = new VFX.MotionBlur(4, Color.White);

            return copy;
        }
    }
}
