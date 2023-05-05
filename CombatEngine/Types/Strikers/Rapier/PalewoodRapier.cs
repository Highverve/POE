using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Strikers.Rapier
{
    class PalewoodRapier : BaseStriker
    {
        private WeaponCollision spearEnd;

        public PalewoodRapier() : base(CombatSlotType.OneHand, 55f, 25f, 40f) { }

        public override void Load(ContentManager main, ContentManager map)
        {
            swordTexture = main.Load<Texture2D>(strikerDirectory + "Rapiers/palewoodRapier");
            baseScale = 2f;

            SetAnimationValues(new Point(64, 64), Point.Zero);
            SetClickBools(false, true, true, true, true, false, true, true);

            attributeNames = Entities.Entities.BaseEntity.ATTRIBUTE_CombatPhysicalMultipliers;
            baseWeaponSpeed = 1.15f;

            base.Load(main, map);
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictCombat)
        {
            base.Update(gt, checkControls, restrictCombat);

            spearEnd.PositionA = strikerLine.PositionB;
            spearEnd.PositionB = CalculateRelativeVector(directionAngle + endAngle, strikerLine.PositionB, 24, 0);
        }

        public override void Basic(GameTime gt)
        {
            if (isAdded == false)
            {
                switch (comboSwing)
                {
                    case 1: BasicComboFirst(gt); break;
                    case 2: BasicComboSecond(gt); break;
                    case 3: BasicComboThird(gt); break;
                }

                isAdded = true;
            }
        }
        private void BasicComboFirst(GameTime gt)
        {
            AssignVariables(0, 900, .2f);
            AddAction(() => { endAngle = random.NextFloat(-.3f, 0f); }, 0, 50);
            float randHandle = random.NextFloat(.5f, 1f);
            entity.STAMINA_Damage(25f);

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
            AddAction(() => { entity.ControlMotion((normalizedDirection.X * 20) * (multiplier1), (normalizedDirection.Y * 20) * (multiplier1), true); }, 0, 900);
            AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 200, maxCombatTime);
            AddAction(() => { entity.MaxSpeed = 0f; }, 200, maxCombatTime);

            //Pause animation
            AddAction(() => { isMoveAnimPaused = true; }, 200, 250);
            AddAction(() => { isUpdateDirection = true; }, 0, 250);
            AddAction(() => { isUpdateDirection = false; }, 250, maxCombatTime);

            //Damage range
            AddAction(() =>
            {
                spearEnd.IsActive = true;
            }, 250, 450);
            AddAction(() =>
            {
                spearEnd.IsActive = false;
            }, 450, 500);
        }
        private void BasicComboSecond(GameTime gt)
        {
            AssignVariables(0, 750, 0f);
            AddAction(() => { endAngle = random.NextFloat(-.3f, 0f); }, 0, 50);
            float randHandle = random.NextFloat(.5f, 1f);
            entity.STAMINA_Damage(20f);

            //Reach anmimation
            AddAction(() => Adjust(gt, ref multiplier1, 1f), 0, 50);
            AddAction(() => Adjust(gt, ref multiplier1, 9f), 50, 150);
            AddAction(() => Adjust(gt, ref multiplier1, 2f), 150, 250);
            AddAction(() => Adjust(gt, ref multiplier1, -3.25f), 250, 650);
            AddAction(() => Adjust(gt, ref multiplier1, -1f), 650, 750);

            AddAction(() => { reach = MathHelper.SmoothStep(0f, 100f, multiplier1); }, 0, maxCombatTime);
            AddAction(() => { CombatHeight = MathHelper.SmoothStep(0, 32f * randHandle, multiplier1); }, 0, maxCombatTime);

            //Handle animation
            AddAction(() => { handleAngle = MathHelper.SmoothStep(1.25f * dualWieldDirection * randHandle, .2f * dualWieldDirection, multiplier1); }, 0, maxCombatTime);

            //Motion controlling
            AddAction(() => { entity.ControlMotion((normalizedDirection.X * 20) * (multiplier1), (normalizedDirection.Y * 20) * (multiplier1), true); }, 0, 900);
            AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 200, maxCombatTime);
            AddAction(() => { entity.MaxSpeed = 0f; }, 200, maxCombatTime);

            //Pause animation
            AddAction(() => { isMoveAnimPaused = true; }, 0, 50);
            AddAction(() => { isUpdateDirection = true; }, 0, 50);
            AddAction(() => { isUpdateDirection = false; }, 50, maxCombatTime);

            //Damage range
            AddAction(() =>
            {
                spearEnd.IsActive = true;
                spearEnd.DamageMultiplier = spearEnd.BaseDamageMultiplier * 1.1f;
            }, 0, 200);
            AddAction(() =>
            {
                spearEnd.IsActive = false;
            }, 450, 500);
        }
        private void BasicComboThird(GameTime gt)
        {
            AssignVariables(0, 600, .2f);
            AddAction(() => { endAngle = random.NextFloat(-.3f, 0f); }, 0, 50);
            float randHandle = random.NextFloat(.5f, 1f);
            entity.STAMINA_Damage(25f);

            //Reach anmimation
            AddAction(() => Adjust(gt, ref multiplier1, 1f), 0, 50);
            AddAction(() => Adjust(gt, ref multiplier1, 9.5f), 50, 150);
            AddAction(() => Adjust(gt, ref multiplier1, 2f), 150, 200);
            AddAction(() => Adjust(gt, ref multiplier1, -4.5f), 200, 500);
            AddAction(() => Adjust(gt, ref multiplier1, -1f), 500, 600);

            AddAction(() => { reach = MathHelper.SmoothStep(0f, 100f, multiplier1); }, 0, maxCombatTime);
            AddAction(() => { CombatHeight = MathHelper.SmoothStep(0, 32f * randHandle, multiplier1); }, 0, maxCombatTime);

            //Handle animation
            AddAction(() => { handleAngle = MathHelper.SmoothStep(1.25f * dualWieldDirection * randHandle, .2f * dualWieldDirection, multiplier1); }, 0, maxCombatTime);

            //Motion controlling
            AddAction(() => { entity.ControlMotion((normalizedDirection.X * 20) * (multiplier1), (normalizedDirection.Y * 20) * (multiplier1), true); }, 0, 900);
            AddAction(() => { entity.SENSES_SightDirection = directionAngle; }, 200, maxCombatTime);
            AddAction(() => { entity.MaxSpeed = 0f; }, 200, maxCombatTime);

            //Pause animation
            AddAction(() => { isMoveAnimPaused = true; }, 0, 50);
            AddAction(() => { isUpdateDirection = true; }, 0, 50);
            AddAction(() => { isUpdateDirection = false; }, 50, maxCombatTime);

            //Damage range
            AddAction(() =>
            {
                spearEnd.IsActive = true;
                spearEnd.DamageMultiplier = spearEnd.BaseDamageMultiplier * 1.25f;
            }, 0, 200);
            AddAction(() =>
            {
                spearEnd.IsActive = false;
            }, 200, 250);

            AddAction(() =>
            {
                spearEnd.IsActive = true;
                spearEnd.DamageMultiplier = spearEnd.BaseDamageMultiplier * 1.15f;
            }, 250, 550);
            AddAction(() =>
            {
                spearEnd.IsActive = false;
            }, 550, 600);
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

        public override void Draw(SpriteBatch sb)
        {
            DrawGenericStriker(sb, new Vector2(9, 56), swordTexture, scale);

            base.Draw(sb);
        }

        public override BaseCombat Copy()
        {
            PalewoodRapier copy = (PalewoodRapier)base.Copy();

            copy.strikerLine.DamageMultiplier = .3f;

            copy.spearEnd = new WeaponCollision("SpearEnd", 1f);
            copy.lines.Add(copy.spearEnd);

            return copy;
        }
    }
}
