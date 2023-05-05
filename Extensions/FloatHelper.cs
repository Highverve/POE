using System;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public static class FloatHelper
    {
        public static float CurveAngle(this float angle, float from, float to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            Vector2 fromVector = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toVector = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 currentVector = Slerp(fromVector, toVector, step);

            return (float)Math.Atan2(currentVector.Y, currentVector.X);
        }

        public static Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            double theta = Math.Acos(Vector2.Dot(from, to));
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            return (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;
        }

        public static void MoveWithin(this float value1, float min, float max, float speed)
        {
            if (value1 > max)
                value1 -= speed;
            else if (value1 < min)
                value1 += speed;
        }

        public static bool IsClose(this float value1, float minDistance, float maxDistance)
        {
            return (value1 >= minDistance && value1 <= maxDistance);
        }

        /// <summary>
        /// Note: This is in radians
        /// </summary>
        /// <param name="value">Value expected in radians</param>
        /// <returns></returns>
        public static Vector2 ToVector2(this float value)
        {
            return new Vector2((float)Math.Cos(value), (float)Math.Sin(value));
        }
    }
}
