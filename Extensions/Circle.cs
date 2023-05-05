using System;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class Circle
    {
        private Vector2 position;
        public Vector2 Position { get { return position; } set { position = value; } }

        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }

        public float radius { get; set; }

        public Circle() : this(Vector2.Zero, 0f) { }
        public Circle(float Radius) : this (Vector2.Zero, Radius) { }
        public Circle(Vector2 Position, float Radius)
        {
            this.Position = Position;
            radius = Radius;
        }

        public bool Intersects(Vector2 position, float radius)
        {
            float distance = Vector2.Distance(Position, position);

            return (this.radius + radius) > distance;
        }
        public bool Intersects(Circle Circle)
        {
            float distance = Vector2.Distance(Position, Circle.Position);

            return (radius + Circle.radius) > distance;
        }
        public bool Intersects(Rectangle rectangle) //Experimental, test when possible!
        {
            // Find the closest point to the circle within the rectangle
            float closestX = MathHelper.Clamp(Position.X, rectangle.Left, rectangle.Right);
            float closestY = MathHelper.Clamp(Position.Y, rectangle.Top, rectangle.Bottom);

            // Calculate the distance between the circle's center and this closest point
            float distanceX = Position.X - closestX;
            float distanceY = Position.Y - closestY;

            // If the distance is less than the circle's radius, an intersection occurs
            float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
            return distanceSquared < (radius * radius);
        }

        public enum LineIntersection { NoIntersection, Intersection, Tangent } //Tangent = line intersection with two points on circle
        public LineIntersection IntersectsInfinite(Line line)
        {
            Vector2 locationA, locationB;

            locationA = line.locationA;
            locationB = line.locationB;

            locationA.X -= Position.X;
            locationA.Y -= Position.Y;
            locationB.X -= Position.X;
            locationB.Y -= Position.Y;

            float distanceFromX = locationA.X - locationB.X;
            float distanceFromY = locationA.Y - locationB.Y;

            float distanceFromRadius = (distanceFromX * distanceFromY) +
                                       (distanceFromY * distanceFromY);

            float distance = locationA.X * locationB.Y - locationB.X * locationA.Y;
         
            float di = (radius * radius) *
                       (distanceFromRadius * distanceFromRadius) -
                       (distance * distance);

            if (di > 0) return LineIntersection.Intersection;
            else if (di == 0) return LineIntersection.Tangent;
            else return LineIntersection.NoIntersection;
        }
        public bool IntersectsSegment(Line line)
        {
            // First up, let's normalise our vectors so the circle is on the origin
            Vector2 normA = line.locationA - Position;
            Vector2 normB = line.locationB - Position;

            Vector2 d = normB - normA;

            // Want to solve as a quadratic equation, need 'a','b','c' components
            float aa = Vector2.Dot(d, d);
            float bb = 2 * (Vector2.Dot(normA, d));
            float cc = Vector2.Dot(normA, normA) - (radius * radius);

            // Get determinant to see if LINE intersects
            double deter = Math.Pow(bb, 2.0) - 4 * aa * cc;
            if (deter > 0)
            {
                // Get t values (solve equation) to see if LINE SEGMENT intersects
                double t = (-bb - Math.Sqrt(deter)) / (2 * aa);
                double t2 = (-bb + Math.Sqrt(deter)) / (2 * aa);
                bool match = false;

                if (0.0 <= t && t <= 1.0)
                {
                    // Interpolate to get collision point
                    Vector2 collisionPoint = Position + Vector2.Lerp(normA, normB, (float)t);
                    match = true;
                }
                if (0.0 <= t2 && t2 <= 1.0)
                {
                    Vector2 collisionPoint2 = Position + Vector2.Lerp(normA, normB, (float)t2);
                    match = true;
                }
                return match;
            }
            else
                return false;
        }

        public bool Contains(Vector2 position)
        {
            return (Vector2.Distance(position, this.Position) < radius);
        }
        /// <summary>
        /// Probably inefficient. Rarely use this, instead go for the Intersects() methods. EXPERIMENTAL
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool Contains(Rectangle rectangle)
        {
            return (Contains(new Vector2(rectangle.X, rectangle.Y)) &&
                    Contains(new Vector2(rectangle.X + rectangle.Width, rectangle.Y)) &&
                    Contains(new Vector2(rectangle.X, rectangle.Y + rectangle.Height)) &&
                    Contains(new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height)));
        }

        public void Repel(Circle Circle, float RepelStrength)
        {
            Vector2 pushDirection;

            pushDirection = Vector2.Subtract(Circle.Position, this.Position);

            Circle.Position += (pushDirection * RepelStrength);
        }
        public Vector2 Repel(Vector2 position, float RepelStrength)
        {
            Vector2 pushDirection = position - this.Position;

            if (pushDirection != Vector2.Zero)
                pushDirection.Normalize();

            return (pushDirection * RepelStrength);
        }

        public void Attract(Circle Circle, float RepelStrength)
        {
            Vector2 pushDirection;

            pushDirection = Vector2.Subtract(Circle.Position, this.Position);

            Circle.Position -= (pushDirection * RepelStrength);
        }

        public void PushOut(ref Vector2 position)
        {
            float depthPush = Vector2.Distance(this.Position, position) - radius;
            Vector2 direction = position - this.Position;

            if (direction != Vector2.Zero)
                direction.Normalize();

            position -= (direction * depthPush);
        }
        public Vector2 PushOut(Vector2 position)
        {
            float depthPush = Vector2.Distance(this.Position, position) - radius;
            Vector2 direction = position - this.Position;

            if (direction != Vector2.Zero)
                direction.Normalize();

            return (position -= (direction * depthPush));
        }
        public void PushOut(Circle circle)
        {
            float depthPush = Vector2.Distance(this.Position, circle.Position) - (this.radius + circle.radius);
            Vector2 direction = circle.Position - this.Position;

            if (direction != Vector2.Zero)
                direction.Normalize();

            circle.Position -= (direction * depthPush);
        }
        public Vector2 PushOutCircle(Vector2 position, float radius)
        {
            float depthPush = Vector2.Distance(this.Position, position) - (this.radius + radius);
            Vector2 direction = position - this.Position;

            if (direction != Vector2.Zero)
                direction.Normalize();

            return (direction * depthPush);
        }
        public Vector2 InvertedPushCircle(Vector2 circlePosition, float circleRadius)
        {
            float depthPush = Vector2.Distance(this.Position, circlePosition) - (this.radius + circleRadius);
            Vector2 direction = circlePosition - this.Position;

            if (direction != Vector2.Zero)
                direction.Normalize();

            return (direction * depthPush);
        }

        public float Distance(Vector2 position)
        {
            return Vector2.Distance(Position, position);
        }
        public float Distance(Circle circle)
        {
            return (Vector2.Distance(circle.position, this.position) - (circle.radius + this.radius));
        }
        public float Distance(Vector2 position, float radius)
        {
            return (Vector2.Distance(position, this.position) - (radius + this.radius));
        }

        public Vector2 VectorDistance(Vector2 position, float radius)
        {
            return new Vector2((Math.Abs(position.X - this.position.X)) - (radius + this.radius),
                               (Math.Abs(position.Y - this.position.Y)) - (radius + this.radius));
        }
        public Vector2 VectorDistance(Circle circle)
        {
            return new Vector2((Math.Abs(circle.position.X - this.position.X)) - (circle.radius + this.radius),
                               (Math.Abs(circle.position.Y - this.position.Y)) - (circle.radius + this.radius));
        }

        public Vector2 Rotate(float angle)
        {
            return new Vector2((float)(radius * Math.Cos(angle)), (float)(radius * Math.Sin(angle))) + position;
        }
        public Vector2 RotateOval(float angle, Vector2 ovalOffset)
        {
            return new Vector2((float)((radius * ovalOffset.X) * Math.Cos(angle)),
                               (float)((radius * ovalOffset.Y) * Math.Sin(angle))) + position;
        }

        public static Vector2 Rotate(float angle, float distance, Vector2 center)
        {
            return new Vector2((float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle))) + center;
        }
        public static Vector2 RotateOval(float angle, float distance, Vector2 center, Vector2 ovalOffset) //Rotate objects around a circle
        {
            return new Vector2((float)((distance * ovalOffset.X) * Math.Cos(angle)),
                               (float)((distance * ovalOffset.Y) * Math.Sin(angle))) + center;
        }

        public static Circle Empty
        {
            get { return new Circle(Vector2.Zero, 0f); }
        }
    }
}
