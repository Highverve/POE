using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.LightEngine.Types
{
    public class FlickerLight : BaseLight
    {
        private float flickerDifference;

        public FlickerLight(int ID, string TexturePath, Point TileLocation, Vector2 Offset, Color Color, float Size, float Angle, float FlickerSizeDifference)
            : base(ID, TexturePath, TileLocation, Offset, Color, Size, Angle)
        {
            flickerDifference = FlickerSizeDifference;
        }

        int flickerTime = 0, maxTime = 0; float multiplier = 0f, lastSize;
        public override void UpdateLights(GameTime gt)
        {
            if (maxTime <= 0)
                maxTime = random.Next(25, 100);

            flickerTime += gt.ElapsedGameTime.Milliseconds;

            if (flickerTime >= maxTime)
            {
                multiplier = random.NextFloat(-flickerDifference, flickerDifference);
                lastSize = size;

                flickerTime = 0;
                maxTime = 0;
            }

            if (maxTime > 0) //&& flickerTime > 0)
            {
                size = MathHelper.Lerp(lastSize,
                                       baseSize + multiplier,
                                       MathHelper.Clamp(((float)flickerTime / (float)maxTime), 0f, 1f));
            }

            base.UpdateLights(gt);
        }
    }
}
