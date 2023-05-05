using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers
{
    public static class Parsing
    {
        public static Point Parse(this Point obj, string x, string y) { return new Point(int.Parse(x), int.Parse(y)); }
        public static Vector2 Parse(this Vector2 obj, string x, string y) { return new Vector2(float.Parse(x), float.Parse(y)); }
    }
}
