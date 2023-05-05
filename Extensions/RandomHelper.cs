using System;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public static class RandomHelper
    {
        public static float NextFloat(this Random random, double minimum, double maximum)
        {
            return (float)(random.NextDouble() * (maximum - minimum) + minimum);
        }
    }
}
