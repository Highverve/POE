namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class Percent
    {

        public int PercentageToInteger(float percentage)
        {
            return (int)percentage * 100;
        }
        public float ToFloat(int percentage)
        {
            return percentage / 100;
        }

        public static float PercentageFloat(int baseNumber, int subNumber)
        {
            return (float)(subNumber / baseNumber);
        }
        public static int PercentageInteger(int baseNumber, int subNumber)
        {
            return (subNumber / baseNumber) * 100;
        }
    }
}
