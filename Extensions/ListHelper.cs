using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Extensions
{
    public static class ListHelper
    {
        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

    }
}
