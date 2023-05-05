using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.LightEngine;
using System;

namespace Pilgrimage_Of_Embers.CombatEngine.Types.Shooters
{
    public class BowString
    {
        private Line stringA = new Line(), stringB = new Line(), stringShadowA = new Line(), stringShadowB = new Line();
        private Vector2 handleOrigin;

        private float stringDraw, stringDrawLerp = 0f;
        private float bowLength, handleDistance, minDraw, maxDraw;

        protected Texture2D pixel;
        protected Color stringColor = new Color(165, 154, 130, 255);

        public Vector2 StringDrawPosition { get { return stringA.locationA; } }
        public float StringMultiplier { get { return stringDrawLerp; } set { stringDrawLerp = MathHelper.Clamp(value, 0f, 1f); } }

        public float MinDraw { get { return minDraw; } }
        public float MaxDraw { get { return maxDraw; } }

        public BowString(Vector2 StringSlot1, Vector2 StringSlot2, Vector2 HandleOrigin, float HandleDistance, float MinimumDraw, float MaximumDraw, Texture2D Pixel)
        {
            bowLength = Vector2.Distance(StringSlot1, StringSlot2);
            handleOrigin = HandleOrigin;

            minDraw = MinimumDraw;
            maxDraw = MaximumDraw;
            handleDistance = HandleDistance;
            pixel = Pixel;
        }

        public void Update(GameTime gt, Vector2 center, Vector2 normalizedBowDir, float shadowOffset, float finalCombatHeight)
        {
            //The lerp for string A to B
            stringDrawLerp = MathHelper.Clamp(stringDrawLerp, 0f, 1f);
            //The string's current draw, from minDraw to maxDraw
            stringDraw = MathHelper.Lerp(minDraw, maxDraw, stringDrawLerp);

            //Cross vector of the current direction
            Vector2 bowCross = normalizedBowDir.Cross();

            //The center of the connecting string positions
            Vector2 stringCenter = center - (normalizedBowDir * (handleDistance * stringDraw));

            //Should be already normalized. Marked for erasing
            if (bowCross != Vector2.Zero)
                bowCross.Normalize();

            //Connecting position
            stringA.locationA = stringCenter;
            stringA.locationB = center - bowCross * (bowLength); //String end

            //Connecting position
            stringB.locationB = stringCenter; 
            stringB.locationA = center + bowCross * (bowLength - 4f); //String end
        }
        public void Draw(SpriteBatch sb, float finalDepth)
        {
            stringA.DrawLine(sb, pixel, stringColor, finalDepth, 1);
            stringB.DrawLine(sb, pixel, stringColor, finalDepth, 1);
        }

        public void PullString(GameTime gt, float weaponSpeed, float customSpeed)
        {
            stringDrawLerp += ((customSpeed * weaponSpeed) - Math.Abs(1.5f - stringDrawLerp)) * (float)gt.ElapsedGameTime.TotalSeconds;
        }
        public void ReleaseString(GameTime gt)
        {
            stringDrawLerp -= 20f * (float)gt.ElapsedGameTime.TotalSeconds;
        }
        public void ResetString()
        {
            stringDrawLerp = 0f;
        }

        public BowString Copy()
        {
            BowString copy = (BowString)this.MemberwiseClone();

            copy.stringA = new Line();
            copy.stringB = new Line();
            copy.ResetString();

            return copy;
        }
    }
}
