using System;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Skills;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.ParticleEngine.EmitterTypes;

namespace Pilgrimage_Of_Embers.CombatEngine.Types
{
    public class BaseDeflector : BaseCombat
    {
        private float width;

        protected WeaponCollision deflector, deflectorSpike;

        protected Texture2D shieldTexture;
        protected Texture2D borderedRect;

        protected Point spriteDirection = new Point(); //For spritesheet positioning

        protected Vector2 shieldCenter;

        public void SetDirection(float angle)
        {
            if (angle > -157.5f && angle < -112.5f) //NW
                spriteDirection.X = 3;
            else if (angle > -112.5f && angle < -67.5f) //N
                spriteDirection.X = 4;
            else if (angle > -67.5f && angle < -22.5f) //NE
                spriteDirection.X = 5;
            else if (angle > -22.5f && angle < 22.5f) //E
                spriteDirection.X = 6;
            else if (angle > 22.5f && angle < 67.5f) //SE
                spriteDirection.X = 7;
            else if (angle > 67.5f && angle < 112.5f) //S
                spriteDirection.X = 0;
            else if (angle > 112.5f && angle < 157.5f) //SW
                spriteDirection.X = 1;
            else
                spriteDirection.X = 2;
        }

        public BaseDeflector(CombatSlotType SlotType, float Width, float WeaponDistance)
            : base(WeaponCategory.Deflector, SlotType, 30, WeaponDistance)
        {
            width = MathHelper.Clamp(Width, .05f, 1f);
            SetAnimationValues(new Point(64, 64), Point.Zero);
        }

        public override void Load(ContentManager main, ContentManager map)
        {
            borderedRect = main.Load<Texture2D>("Combat/Deflectors/borderedRect");

            base.Load(main, map);
        }

        private float shieldOffset, mouseDistanceOffset = .35f, mouseDistance;
        public float shieldCross;
        public override void Update(GameTime gt, bool checkControls, bool restrictControls)
        {
            //UpdateDepth();

            base.Update(gt, checkControls, restrictControls);

            if (offhand is BaseDeflector) //Dual-wielding shields...
                shieldOffset = MathHelper.Lerp(-.5f * dualWieldDirection, -1.55f * dualWieldDirection, Math.Abs(mouseDistanceOffset));
            else
                shieldOffset = MathHelper.Lerp(0f * dualWieldDirection, -1.55f * dualWieldDirection, Math.Abs(mouseDistanceOffset));

            deflector.PositionA = CalculateRelativeVector(directionAngle + handleAngle - width - shieldOffset, center, FinalDistance + reach, CombatHeight); //Circle.Rotate((directionAngle - width) - shieldOffset, shieldRadius, center);
            deflector.PositionB = CalculateRelativeVector(directionAngle + handleAngle + width - shieldOffset, center, FinalDistance + reach, CombatHeight);
            //deflector.PositionB = Circle.Rotate((directionAngle + width) - shieldOffset, shieldRadius, center);

            deflectorSpike.PositionB = CalculateRelativeVector(directionAngle + handleAngle - shieldOffset, center, FinalDistance + reach + 20f, CombatHeight);//center + (directionAngle - shieldOffset).ToVector2() * (shieldRadius * 1.35f);
            deflectorSpike.PositionA = CalculateRelativeVector(directionAngle + handleAngle - shieldOffset, center, FinalDistance + reach - 10f, CombatHeight);//center + (directionAngle - shieldOffset).ToVector2() * (shieldRadius * .5f);

            shieldCross = deflectorSpike.PositionA.Direction(deflectorSpike.PositionB);

            shieldCenter = deflectorSpike.PositionA;// = Circle.RotateOval(directionAngle - shieldOffset, (shieldRadius * .75f) + reach, center, new Vector2(1.25f, .8f)); //(center += ((directionAngle + MathHelper.ToRadians(angleOffset)).ToVector2() * (shieldRadius * .75f)));
            SetDirection(MathHelper.Clamp(MathHelper.ToDegrees(center.Direction(shieldCenter)), -180f, 180));

            //Spark positioning and direction.
            sparksPosition = deflectorSpike.PositionA;
            Vector2 sparkTempDir = deflectorSpike.PositionB - deflectorSpike.PositionA;

            if (sparkTempDir != Vector2.Zero)
                sparkTempDir.Normalize();

            sparksDirection = sparkTempDir;
        }
        protected override void CheckControls(GameTime gt)
        {
            base.CheckControls(gt);

            float safeZone = 200, maxRange = 200;

            mouseDistance = MathHelper.Clamp(Vector2.Distance(camera.ScreenToWorld(controls.MouseVector), center) - safeZone, 0, maxRange);
            mouseDistanceOffset = mouseDistance / maxRange;

            /* SafeZone = 200, MsDistToCenter = 325, MaxRange = 500 - SafeZone = 300

            distance = MSDistToCenter - SafeZone = 125
            lerp = distance / MaxRange = .25

            [MsDistToCenter = 450]
            distance = 450 - SafeZone = 250
            lerp = distance / MaxRange = .5
            */
        }
        protected override void UpdateDepth()
        {
            if (deflectorSpike.PositionB.Y > center.Y)
                finalDepth = baseDepth + .0001f;
            else
                finalDepth = baseDepth - .0001f;
        }

        /// <summary>
        /// Determines the deflector's direction. Used specifically for NPEs.
        /// </summary>
        /// <param name="distance">Between 0 and 1. Zero is by the side, one is in front.</param>
        public void SetDistanceOffset(float distance)
        {
            mouseDistanceOffset = MathHelper.Clamp(distance, 0, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            //if (CurrentAction != CombatMove.None)
            //    deflector.DrawLine(sb, borderedRect, shieldLineColor, 1f, 3);

            base.Draw(sb);
        }

        public override void DrawDebug(SpriteBatch sb)
        {
            deflector.DrawDebug(sb, pixel);
            deflectorSpike.DrawDebug(sb, pixel);

            base.DrawDebug(sb);
        }

        public bool IsDeflected(Line line)
        {
            return false;
            //return deflector.Intersects(line);// || deflectorSpike.Intersects(line);
        }
        public bool IsDeflected(BaseStriker striker)
        {
            //return deflector.Intersects(striker.StrikerLine) ||
            //       deflectorSpike.Intersects(striker.StrikerLine) ||
            //       striker.IsStrikerCollide(deflectorSpike.locationB);
            return false;
        }

        public override void DeflectOther(GameTime gt)
        {
            deflector.IsBlocking = true;
            deflectorSpike.IsBlocking = true;

            base.DeflectOther(gt);
        }

        public override BaseCombat Copy()
        {
            BaseDeflector copy = (BaseDeflector)base.Copy();

            copy.deflector = new WeaponCollision("ShieldLine");
            copy.deflectorSpike = new WeaponCollision("ShieldSpike");

            copy.lines.Add(copy.deflector);
            copy.lines.Add(copy.deflectorSpike);

            return copy;
        }
    }
}
