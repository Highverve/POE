using System;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public static class VectorHelper
    {
        public static Point ToPoint(this Vector2 vector) { return new Point((int)vector.X, (int)vector.Y); }

        public static Vector2 Bezier(this Vector2 vector, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }

        public static Vector2 Cross(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }

        public static Vector2 Midpoint(this Vector2 vector1, Vector2 vector2)
        {
            return new Vector2((vector1.X + vector2.X) / 2, (vector1.Y + vector2.Y) / 2);
        }
        public static Vector2 Midpoint(this Vector2 vector1, Vector2 vector2, float maxLength)
        {
            return Vector2.Lerp(vector1, vector2, maxLength);
        }

        public static float Triangle(this Vector2 vector1, Vector2 vector2, Vector2 vector3)
        {
            return (float)(Math.Atan2(vector2.Y - vector1.Y, vector2.X - vector1.X) -
                    Math.Atan2(vector3.Y - vector1.Y, vector3.X - vector1.X));
        }

        public static bool IsInSight(this Vector2 positionA, Vector2 positionB, float coneDirection, float arcLength, float minLength, float maxLength)
        {
            float distance = Vector2.Distance(positionA, positionB);

            if (distance >= minLength && distance <= maxLength) //Is position in distance
            {
                float angle = (float)Math.Atan2(positionA.Y - positionB.Y, positionA.X - positionB.X);

                if (angle >= (coneDirection - (arcLength / 2)) &&
                    angle <= (coneDirection + (arcLength / 2))) //If angle is inside metaphorical cone
                {
                    return true;
                }
            }

            return false;
        }

        public static float Direction(this Vector2 positionA, Vector2 positionB)
        {
            return (float)Math.Atan2(positionB.Y - positionA.Y, positionB.X - positionA.X);
        }

        public static Vector2 Reflect(this Vector2 vector, Vector2 normal)
        {
            return vector - 2 * Vector2.Dot(vector, normal) * normal;
        }

        public static Vector2 Parse(this Vector2 value, string x, string y)
        {
            return new Vector2(float.Parse(x), float.Parse(y));
        }
    }
}
