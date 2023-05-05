using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Extensions
{
    public static class PointHelper
    {
        public static int CompareTo(this Point a, Point b)
        {
            int sort = a.X.CompareTo(b.X); //Compare the X axis together

            if (sort == 0)
                sort = a.Y.CompareTo(b.Y);

            return sort;
        }
    }
}
