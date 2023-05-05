using System;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.CombatEngine.Types
{
    public class BaseCaster : BaseCombat
    {
        protected Texture2D texture;

        protected WeaponCollision staffLine;
        protected float facingDir;
        protected Vector2 origin;

        public BaseCaster(CombatSlotType SlotType, float CasterDistance, float CasterLength, float MaxReach)
            : base(Skills.WeaponCategory.Caster, SlotType, CasterLength, 30)
        {
            maxReach = MaxReach;
            baseScale = 2f;
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictCombat)
        {
            base.Update(gt, checkControls, restrictCombat);

            staffLine.PositionA = handlePosition;
            staffLine.PositionB = endPosition;

            castingPosition = staffLine.PositionA;
        }
        protected override void UpdateDepth()
        {
            if (staffLine.PositionA.Y > center.Y)
                finalDepth = baseDepth + .0001f;
            else
                finalDepth = baseDepth - .0001f;

            base.UpdateDepth();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, staffLine.PositionA, Color.White, origin, staffLine.PositionA.Direction(staffLine.PositionB) + angleOffset, scale, SpriteEffects.None, finalDepth);
        }

        public override BaseCombat Copy()
        {
            BaseCaster copy = (BaseCaster)base.Copy();

            copy.staffLine = new WeaponCollision("StaffLine");
            copy.lines.Add(copy.staffLine);

            copy.lines[0].Line.locationA = new Vector2(1000, 1000);
            copy.lines[0].Line.locationB = new Vector2(1050, 1050);

            return copy;
        }
    }
}
