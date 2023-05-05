using System;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.WeaponsTypes;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.VFX;

namespace Pilgrimage_Of_Embers.CombatEngine.Types
{
    public class CombatAction
    {
        public int minTime { get; private set;}
        public int maxTime { get; private set; }
        public Action action { get; private set; }

        public CombatAction(Action Action, int MinTime, int MaxTime)
        {
            action = Action;
            minTime = MinTime;
            maxTime = MaxTime;
        }
    }

    public class BaseStriker : BaseCombat
    {
        protected WeaponCollision strikerLine;

        protected Texture2D swordTexture;
        protected const string strikerDirectory = "Combat/Strikers/";

        public BaseStriker(CombatSlotType SlotType, float WeaponLength, float WeaponDistance, float MaxReach)
            : base(WeaponCategory.Striker, SlotType, WeaponLength, WeaponDistance)
        {
            maxReach = MaxReach;
            strikerLine = new WeaponCollision("SwordLine");
        }

        public override void Load(ContentManager main, ContentManager map)
        {
            base.Load(main, map);
        }

        public override void Update(GameTime gt, bool checkControls, bool restrictCombat)
        {
            base.Update(gt, checkControls, restrictCombat);

            UpdateLinePositioning(gt);
        }
        protected override void UpdateDepth()
        {
            if (strikerLine.Line.locationA.Y > center.Y)
                finalDepth = baseDepth + .0001f;
            else
                finalDepth = baseDepth - .0001f;
        }

        protected int tickCount = 0;
        private void UpdateLinePositioning(GameTime gt)
        {
            lastPositionA = strikerLine.Line.locationA;
            lastPositionB = strikerLine.Line.locationB;

            strikerLine.PositionA = handlePosition;
            strikerLine.PositionB = endPosition;
        }

        protected override void ResetVariables()
        {
            tickCount = 0;

            base.ResetVariables();
        }
        public override void ForceStop()
        {
            base.ForceStop();
        }
        protected void MoveSmoothly(GameTime gt, ref float value, float start, float end, float speed)
        {
            value = MathHelper.SmoothStep(start, end, speed);
        }

        protected void DrawGenericStriker(SpriteBatch sb, Vector2 swordOrigin, Texture2D swordTexture, float scale)
        {
            Vector2 origin = swordOrigin;
            float angleAdjustment = angleOffset;

            //Flipped texture direction
            if (TextureDirection == SpriteEffects.FlipHorizontally)
            {
                origin = new Vector2(swordTexture.Width - swordOrigin.X, swordOrigin.Y);
                angleAdjustment = angleOffset * 3f;
            }

            //Sword
            sb.Draw(swordTexture, strikerLine.PositionA, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                Color.White, strikerLine.PositionA.Direction(strikerLine.PositionB) + angleAdjustment, origin, scale, TextureDirection, finalDepth);

            //Shadow
            sb.Draw(swordTexture, CalculateShadow(strikerLine.PositionA), new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                    WorldLight.ShadowColor, strikerLine.PositionA.Direction(strikerLine.PositionB) + angleAdjustment, origin, scale, TextureDirection, finalDepth - .001f);

            //Motion blurring
            blur.Draw(sb, swordTexture, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), angleAdjustment, origin, scale, TextureDirection, finalDepth, strikerLine.PositionA, strikerLine.PositionB);
        }

        public override BaseCombat Copy()
        {
            BaseStriker copy = (BaseStriker)base.Copy();

            copy.strikerLine = new WeaponCollision("SwordLine");
            copy.lines.Add(copy.strikerLine);

            return copy;
        }
    }
}
