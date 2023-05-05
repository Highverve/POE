using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Casters.Longstaves
{
    public class TestLongstaff : BaseCaster
    {
        public TestLongstaff() : base(CombatSlotType.TwoHand, 65f, 100f, 70f)
        {
        }

        public override void Load(ContentManager main, ContentManager map)
        {
            texture = main.Load<Texture2D>("Items/Icons/Weapons/Staves/testLongstaff");
            origin = new Vector2(19, 44);

            base.Load(main, map);
        }

        public override void Basic(GameTime gt)
        {
            AssignVariables(-1.5f, 850, 0f, 0f);

            if (isAdded == false)
            {
                AddAction(() => { spell.CastSpell(this, CurrentAction); }, 400, 500); //FireCustom(1, strikerLine.locationB, mouseDirection, 1f, 1f, 1);
                AddAction(() => { entity.SUSPEND_Action(maxCombatTime); }, 0, maxCombatTime);

                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 2f), 0, 400);
                AddAction(() => Adjust(gt, ref multiplier2, 2f), 0, 400);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 500, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -2f), 500, maxCombatTime);

                //Moving smoothly
                AddAction(() => { reach = MathHelper.SmoothStep(0f, 15f, multiplier1); }, 0, 400);
                AddAction(() => { endAngle = MathHelper.SmoothStep(MathHelper.ToRadians(-90f), MathHelper.ToRadians(-90f + (5f * facingDir)), multiplier2); }, 0, 400);

                isAdded = true;
            }
        }

        public override void Jump(GameTime gt)
        {
            AssignVariables(-1.5f, 850, 0f, .3f);

            if (isAdded == false)
            {
                AddAction(() => { spell.CastSpell(this, CurrentAction); }, 400, 500); //FireCustom(1, strikerLine.locationB, mouseDirection, 1f, 1f, 1);
                AddAction(() => { entity.SUSPEND_Action(maxCombatTime); }, 0, maxCombatTime);

                //Multi's
                AddAction(() => Adjust(gt, ref multiplier1, 2f), 0, 400);
                AddAction(() => Adjust(gt, ref multiplier2, 2f), 0, 400);
                AddAction(() => Adjust(gt, ref multiplier1, -2f), 500, maxCombatTime);
                AddAction(() => Adjust(gt, ref multiplier2, -2f), 500, maxCombatTime);

                //Moving smoothly
                AddAction(() => { reach = MathHelper.SmoothStep(0f, 15f, multiplier1); }, 0, 400);
                AddAction(() => { endAngle = MathHelper.SmoothStep(MathHelper.ToRadians(-90f), MathHelper.ToRadians(-85f), multiplier2); }, 0, 400);

                isAdded = true;
            }
        }

        public override void Sneak(GameTime gt)
        {
            AssignVariables(0f, 750, 0f, .3f);

            if (isAdded == false)
            {
                isAdded = true;
            }
        }

        public override void Power(GameTime gt)
        {

            AssignVariables(0f, 200);

            if (isAdded == false)
            {
                isAdded = true;
            }
            base.Power(gt);
        }
    }
}
