using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public struct Point4
    {
        private int a, b, c, d;

        public int A
        {
            set { a = value; }
            get { return a; }
        }
        public int B
        {
            set { b = value; }
            get { return b; }
        }
        public int C
        {
            set { c = value; }
            get { return c; }
        }
        public int D
        {
            set { d = value; }
            get { return d; }
        }

        public Point4(int A, int B, int C, int D)
        {
            a = A;
            b = B;
            c = C;
            d = D;
        }

        public Point FirstTwo() { return new Point(a, b); }
        public Point LastTwo() { return new Point(c, d); }

        public static Point4 operator +(Point4 point1, Point4 point2)
        {
            return new Point4(point1.A + point2.A,
                              point1.B + point2.B,
                              point1.C + point2.C,
                              point1.D + point2.D);
        }
        public static Point4 operator +(Point4 point1, int amount)
        {
            return new Point4(point1.A + amount,
                              point1.B + amount,
                              point1.C + amount,
                              point1.D + amount);
        }


        public static bool operator ==(Point4 point1, Point4 point2)
        {
            return ((point1.A == point2.A) && (point1.B == point2.B) &&
                    (point1.D == point2.C) && (point1.D == point2.D));
        }
        public static bool operator !=(Point4 point1, Point4 point2)
        {
            return !(point1 == point2);
        }

        public override bool Equals(object obj)
        {
            Point4 point = (Point4)obj;

            return ((point.A == A) && (point.B == B) && (point.C == C) && (point.D == D));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + A + ", " + B + ", " + C + ", " + D + ")";
        }

        private static Point4 zero = new Point4(0, 0, 0, 0);
        public static Point4 Zero { get { return zero; } }

        public bool IsNegative()
        {
            return (a < 0 && b < 0 && c < 0 && d < 0);
        }
    }
}
