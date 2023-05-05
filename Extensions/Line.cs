using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class Line
    {
        public Vector2 locationA, locationB;

        public Line()
            : this(Vector2.Zero, Vector2.Zero)
        {

        }
        public Line(Vector2 LocationA, Vector2 LocationB)
        {
            locationA = LocationA;
            locationB = LocationB;
        }

        public void UpdateLocationA(Vector2 location) { locationA = location; }
        public void UpdateLocationB(Vector2 location) { locationB = location; }

        public bool Intersects(Line line) //Used for almost all things combat (sword swinging, blocking, arrows, magic, etc.)
        {
            Vector2 b = locationB - locationA;
            Vector2 d = line.locationB - line.locationA;

            float linesParallel = b.X * d.Y - b.Y * d.X;

            if (linesParallel == 0)
                return false;

            Vector2 c = line.locationA - locationA;

            float t = (c.X * d.Y - c.Y * d.X) / linesParallel;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / linesParallel;
            if (u < 0 || u > 1)
                return false;

            return true;
        }
        public bool Intersects(Vector2 positionA, Vector2 positionB)
        {
            Vector2 b = locationB - locationA;
            Vector2 d = positionB - positionA;

            float linesParallel = b.X * d.Y - b.Y * d.X;

            if (linesParallel == 0)
                return false;

            Vector2 c = positionA - locationA;

            float t = (c.X * d.Y - c.Y * d.X) / linesParallel;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / linesParallel;
            if (u < 0 || u > 1)
                return false;

            return true;
        }
        public bool Intersects(Circle circle)
        {
            // First up, let's normalise our vectors so the circle is on the origin
            Vector2 normA = locationA - circle.Position;
            Vector2 normB = locationB - circle.Position;

            Vector2 d = normB - normA;

            // Want to solve as a quadratic equation, need 'a','b','c' components
            float aa = Vector2.Dot(d, d);
            float bb = 2 * (Vector2.Dot(normA, d));
            float cc = Vector2.Dot(normA, normA) - (circle.radius * circle.radius);

            // Get determinant to see if LINE intersects
            double deter = (bb * bb) - 4 * aa * cc; //Math.Pow(bb, 2.0)
            if (deter > 0)
            {
                // Get t values (solve equation) to see if LINE SEGMENT intersects
                double t = (-bb - Math.Sqrt(deter)) / (2 * aa);
                double t2 = (-bb + Math.Sqrt(deter)) / (2 * aa);
                bool match = false;

                if (0.0 <= t && t <= 1.0)
                {
                    // Interpolate to get collision point
                    Vector2 collisionPoint = circle.Position + Vector2.Lerp(normA, normB, (float)t);
                    match = true;
                }
                if (0.0 <= t2 && t2 <= 1.0)
                {
                    Vector2 collisionPoint2 = circle.Position + Vector2.Lerp(normA, normB, (float)t2);
                    match = true;
                }
                return match;
            }
            else
                return false;
        }
        public bool Intersects(Rectangle rectangle)
        {
            return Intersects(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y)) ||
                   Intersects(new Vector2(rectangle.X + rectangle.Width, rectangle.Y), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height)) ||
                   Intersects(new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), new Vector2(rectangle.X, rectangle.Y + rectangle.Height)) ||
                   Intersects(new Vector2(rectangle.X, rectangle.Y + rectangle.Height), new Vector2(rectangle.X, rectangle.Y)) ||
                   (rectangle.Contains(locationA) && rectangle.Contains(locationB));
        }

        public Vector2 ClosestPoint(Vector2 position)
        {
            Vector2 AP = position - locationA;       //Vector from A to P   
            Vector2 AB = locationB - locationA;       //Vector from A to B  

            float ABsq = AB.LengthSquared();     //Magnitude of AB vector (it's length squared)     
            float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float distance = ABAPproduct / ABsq; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                return locationA;

            }
            else if (distance > 1)
            {
                return locationB;
            }
            else
            {
                return locationA + AB * distance;
            }

            /*
            Vector2 AToP = position - locationA;
            Vector2 AToB = locationB - locationA;

            float AToBSq = (AToB.X * AToB.X) + (AToB.Y * AToB.Y);

            float AToPDotAToB = (AToP.X * AToB.X) + (AToP.Y * AToB.Y);

            float T = AToPDotAToB / AToBSq;

            return new Vector2(locationA.X + AToB.X * T, locationA.Y + AToB.Y * T);*/
        }

        public Vector2 PushOut(Circle circle)
        {
            Vector2 closestPoint = ClosestPoint(circle.Position);
            float depthPush = Vector2.Distance(circle.Position, closestPoint) - circle.radius;
            Vector2 direction = circle.Position - closestPoint;

            return (direction * depthPush);
        }

        public Vector2 PushOut(Vector2 position, float radius)
        {
            Vector2 closestPoint = ClosestPoint(position);
            float depthPush = Vector2.Distance(position, closestPoint) - radius;

            Vector2 direction = position - closestPoint;

            if (direction != Vector2.Zero)
                direction.Normalize();

            return (direction * depthPush);
        }

        public float Length()
        {
            return (locationA.X - locationA.Y) * (locationA.X - locationA.Y) +
                   (locationB.X - locationB.Y) * (locationB.X - locationB.Y);
        }

        public void DrawLine(SpriteBatch sb, Texture2D rect, Color color, float depth = 1f, int width = 1)
        {
            Rectangle r = new Rectangle((int)locationA.X, (int)locationA.Y, (int)(locationB - locationA).Length() + width, width);

            Vector2 v = Vector2.Normalize(locationA - locationB);

            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));

            if (locationA.Y > locationB.Y) angle = MathHelper.TwoPi - angle;

            sb.Draw(rect, r, null, color, angle, Vector2.Zero, SpriteEffects.None, depth);
        }
        public void DrawLine(SpriteBatch sb, Texture2D rect, Vector2 offset, Color color, float depth = 1f, int width = 1)
        {
            Rectangle r = new Rectangle((int)locationA.X + (int)offset.X, (int)locationA.Y + (int)offset.Y, (int)(locationB - locationA).Length() + width, width);

            Vector2 v = Vector2.Normalize(locationA - locationB);

            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));

            if (locationA.Y > locationB.Y) angle = MathHelper.TwoPi - angle;

            sb.Draw(rect, r, null, color, angle, Vector2.Zero, SpriteEffects.None, depth);
        }
        public static void DrawLine(SpriteBatch spriteBatch, Texture2D rect, Vector2 begin, Vector2 end, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X,
                                        (int)begin.Y,
                                        (int)(end - begin).Length() + width,
                                        width);

            Vector2 v = Vector2.Normalize(begin - end);

            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));

            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;

            spriteBatch.Draw(rect, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
