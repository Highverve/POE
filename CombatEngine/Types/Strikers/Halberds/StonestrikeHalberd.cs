using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using System;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Halberds
{
    public class StonestrikeHalberd : BaseStriker
    {
        private WeaponCollision spearEnd, axeEnd;

        public StonestrikeHalberd() : base(CombatSlotType.OneHand, 90f, 20f, 80f) { }

        public override void Load(ContentManager main, ContentManager map)
        {
            swordTexture = main.Load<Texture2D>(strikerDirectory + "Halberds/stonestrikeHalberd");
            baseScale = 2f;

            SetAnimationValues(new Point(64, 64), Point.Zero);
            SetClickBools(true, true, true, false, true, true, true, false);

            attributeNames = Entities.Entities.BaseEntity.ATTRIBUTE_CombatPhysicalMultipliers;

            base.Load(main, map);
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictCombat)
        {
            base.Update(gt, checkControls, restrictCombat);

            spearEnd.PositionA = strikerLine.PositionB;//CalculateRelativeVector(directionAngle + endAngle, stock.PositionA, stockLength, 0);
            spearEnd.PositionB = CalculateRelativeVector(directionAngle + endAngle, strikerLine.PositionB, 20, 0);

            Vector2 handleCross = (strikerLine.PositionA - strikerLine.PositionB).Cross();
            if (handleCross != Vector2.Zero)
                handleCross.Normalize();
            axeEnd.PositionA = strikerLine.PositionB - ((handleCross * 16) * SpriteEffectMultiplier());

            Vector2 bladeCross = (axeEnd.PositionA - strikerLine.PositionB).Cross();
            if (bladeCross != Vector2.Zero)
                bladeCross.Normalize();
            axeEnd.PositionB = axeEnd.PositionA + ((bladeCross * 24) * SpriteEffectMultiplier());
        }

        public override void Basic(GameTime gt)
        {
            if (isAdded == false)
            {
                if (comboSwing == 1)
                    BasicComboFirst(gt);
                else if (comboSwing == 2)
                    BasicComboSecond(gt);

                isAdded = true;
            }
        }
        private void BasicComboFirst(GameTime gt)
        {
            AssignVariables(-1.5f, 1000, 0f, .3f);
            TextureDirection = SpriteEffects.FlipHorizontally;
            entity.STAMINA_Damage(45f);

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

            //Motion controlling
            AddAction(() => { entity.ControlMotion((normalizedDirection.X * 50) * (multiplier2), (normalizedDirection.Y * 50) * (multiplier2), true); }, 200, 700);
            AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 300, maxCombatTime);
            AddAction(() => { entity.ResetMovement(); }, 300, maxCombatTime);
            AddAction(() => { entity.STATE_SetAction("Idle"); }, 0, maxCombatTime);

            //Damage range
            AddAction(() =>
            {
                spearEnd.IsActive = true;
                axeEnd.IsActive = true;
            }, 350, 650);
            AddAction(() =>
            {
                spearEnd.IsActive = false;
                axeEnd.IsActive = false;
            }, 650, 700);
        }
        private void BasicComboSecond(GameTime gt)
        {
            AssignVariables(1.5f, 1000, 0f, .3f);
            entity.STAMINA_Damage(40f);

            //Multi's
            AddAction(() => Adjust(gt, ref multiplier1, 4f), 0, 300);
            AddAction(() => Adjust(gt, ref multiplier1, -2f), 300, maxCombatTime);

            AddAction(() => { if (multiplier2 <= .8f) Adjust(gt, ref multiplier2, 5f); }, 300, maxCombatTime);
            AddAction(() => { if (multiplier2 > .8f && multiplier2 <= .9f) Adjust(gt, ref multiplier2, 2f); }, 300, maxCombatTime);
            AddAction(() => { if (multiplier2 > .9f) Adjust(gt, ref multiplier2, .25f); }, 300, maxCombatTime);
            AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 300);

            //Moving smoothly
            AddAction(() => { reach = MathHelper.SmoothStep(0f, 15f, multiplier1); }, 0, maxCombatTime);
            AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (-3.5f * dualWieldDirection), multiplier2); }, 0, maxCombatTime);

            //Motion controlling
            AddAction(() => { entity.ControlMotion((normalizedDirection.X * 50) * (multiplier2), (normalizedDirection.Y * 50) * (multiplier2), true); }, 200, 700);
            AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 300, maxCombatTime);
            AddAction(() => { entity.ResetMovement(); }, 300, maxCombatTime);
            AddAction(() => { entity.STATE_SetAction("Idle"); }, 0, maxCombatTime);

            //Damage range
            AddAction(() =>
            {
                spearEnd.IsActive = true;
                axeEnd.IsActive = true;
            }, 350, 650);
            AddAction(() =>
            {
                spearEnd.IsActive = false;
                axeEnd.IsActive = false;
            }, 650, 700);
        }
        public override void Power(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(-1.5f, 1200, .2f, .3f);
                TextureDirection = SpriteEffects.FlipHorizontally;
                entity.STAMINA_Damage(50f);

                //Reach
                AddAction(() => Adjust(gt, ref multiplier1, -1f), 0, 400);
                AddAction(() => Adjust(gt, ref multiplier1, 2f), 400, maxCombatTime);

                //Prepare for swinging (telegraph)
                AddAction(() => Adjust(gt, ref multiplier2, -1.5f), 0, 300);

                //Speed up swing
                AddAction(() => { Adjust(gt, ref multiplier2, .25f); }, 300, 400);
                AddAction(() => { Adjust(gt, ref multiplier2, .5f); }, 400, 500);
                AddAction(() => { Adjust(gt, ref multiplier2, 1f); }, 500, 700);
                AddAction(() => { Adjust(gt, ref multiplier2, 2.5f); }, 700, 1000);

                //Stop quickly
                AddAction(() => { Adjust(gt, ref multiplier2, 1f); }, 1000, 1100);
                AddAction(() => { Adjust(gt, ref multiplier2, .5f); }, 1100, 1150);
                AddAction(() => { Adjust(gt, ref multiplier2, .25f); }, 1150, 1200);

                //Moving smoothly
                AddAction(() => { reach = MathHelper.SmoothStep(0f, 45f, multiplier1); }, 0, maxCombatTime);
                AddAction(() => { directionAngle = MathHelper.SmoothStep(startingAngle, startingAngle + (7.5f * dualWieldDirection), multiplier2); }, 0, maxCombatTime);

                //Entity controlling
                AddAction(() => { entity.ControlMotion((normalizedDirection.X * 250) * (multiplier2), (normalizedDirection.Y * 250) * (multiplier2), true); }, 400, 850);
                AddAction(() => { entity.SENSES_CurrentDirection = directionAngle; }, 300, maxCombatTime);
                AddAction(() => { entity.ResetMovement(); }, 300, maxCombatTime);
                AddAction(() => { entity.STATE_SetAction("Idle"); }, 0, maxCombatTime);

                //Damage range
                AddAction(() =>
                {
                    spearEnd.IsActive = true;
                    axeEnd.IsActive = true;
                }, 650, 1000);
                AddAction(() =>
                {
                    spearEnd.IsActive = false;
                    axeEnd.IsActive = false;
                }, 1000, 1050);

                isAdded = true;
            }
        }

        public override void Sneak(GameTime gt)
        {
            if (isAdded == false)
            {
                AssignVariables(0, 650, 0f);
                AddAction(() => { endAngle = random.NextFloat(-.3f, 0f); }, 0, 50);
                TextureDirection = SpriteEffects.FlipHorizontally;

                //Bring weapon forward
                AddAction(() => { Adjust(gt, ref multiplier1, 1f); }, 0, 100);
                AddAction(() => { Adjust(gt, ref multiplier1, 4f); }, 100, 250);
                AddAction(() => { Adjust(gt, ref multiplier1, 1f); }, 250, 300);

                //Pause animation
                AddAction(() => { isMoveAnimPaused = true; }, 300, 350);
                isUpdateDirection = true;

                //Put weapon back
                AddAction(() => { Adjust(gt, ref multiplier1, -1f); }, 350, 400);
                AddAction(() => { Adjust(gt, ref multiplier1, -4f); }, 400, 550);
                AddAction(() => { Adjust(gt, ref multiplier1, -1f); }, 550, 600);

                AddAction(() => { reach = MathHelper.SmoothStep(0f, 30f, multiplier1); }, 0, maxCombatTime);

                //Handle animation
                AddAction(() => { handleAngle = MathHelper.SmoothStep(1.25f * dualWieldDirection, 1f * dualWieldDirection, multiplier1); }, 0, maxCombatTime);
                AddAction(() => { endAngle = MathHelper.SmoothStep(0f, -1.65f * dualWieldDirection, multiplier1); }, 0, maxCombatTime);

                //Block range
                AddAction(() =>
                {
                    spearEnd.IsBlocking = true;
                    axeEnd.IsBlocking = true;
                }, 300, 350);
                AddAction(() =>
                {
                    spearEnd.IsBlocking = false;
                    axeEnd.IsBlocking = false;
                }, 350, 400);

                isAdded = true;
            }
        }
        /*
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
                AddAction(() =>
                {
                    spearEnd.IsActive = true;
                    axeEnd.IsActive = true;
                }, 450, 650);
                AddAction(() =>
                {
                    spearEnd.IsActive = false;
                    axeEnd.IsActive = false;
                }, 650, 700);

                isAdded = true;
            }
        }*/
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
                AddAction(() => { entity.ControlMotion((normalizedDirection.X * 50) * (multiplier1), (normalizedDirection.Y * 50) * (multiplier1), true); }, 0, 900);
                AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 200, maxCombatTime);
                AddAction(() => { entity.MaxSpeed = 0f; }, 200, maxCombatTime);

                //Pause animation
                AddAction(() => { isMoveAnimPaused = true; }, 150, 200);
                AddAction(() => { isUpdateDirection = true; }, 0, 200);
                AddAction(() => { isUpdateDirection = false; }, 200, maxCombatTime);

                //Damage range
                AddAction(() =>
                {
                    spearEnd.IsActive = true;
                    axeEnd.IsActive = true;
                }, 250, 450);
                AddAction(() =>
                {
                    spearEnd.IsActive = false;
                    axeEnd.IsActive = false;
                }, 450, 500);

                isAdded = true;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawGenericStriker(sb, new Vector2(11, 55), swordTexture, scale);

            base.Draw(sb);
        }

        public override BaseCombat Copy()
        {
            StonestrikeHalberd copy = (StonestrikeHalberd)base.Copy();

            copy.strikerLine.DamageMultiplier = .3f;
            copy.spearEnd = new WeaponCollision("SpearEnd", 1.15f);
            copy.axeEnd = new WeaponCollision("AxeEnd");

            copy.lines.Add(copy.spearEnd);
            copy.lines.Add(copy.axeEnd);

            return copy;
        }
    }
}
