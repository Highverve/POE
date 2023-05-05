using System;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public static class ColorHelper
    {
        public static Color Random(Random random, float alpha)
        {
            return new Color(random.NextFloat(0f, 1f), random.NextFloat(0f, 1f), random.NextFloat(0f, 1f), alpha);
        }
        public static Color Random(Random random, Color a, Color b, byte opacity = 0)
        {
            byte red, green, blue, alpha;

            if (a.R < b.R) red = (byte)random.Next(a.R, b.R);
            else red = (byte)random.Next(b.R, a.R);

            if (a.G < b.G) green = (byte)random.Next(a.G, b.G);
            else green = (byte)random.Next(b.G, a.G);

            if (a.B < b.B) blue = (byte)random.Next(a.B, b.B);
            else blue = (byte)random.Next(b.B, a.B);

            if (a.A < b.A) alpha = (byte)random.Next(a.A, b.A);
            else alpha = (byte)random.Next(b.A, a.A);

            if (opacity == 0)
                return new Color(red, green, blue, alpha);
            else
                return new Color(red, green, blue, opacity);
        }

        public static bool IsColorInRange(this Color color, Color min, Color max)
        {
            return ((color.R >= min.R) && (color.R <= max.R)) &&
                   ((color.G >= min.G) && (color.G <= max.G)) &&
                   ((color.B >= min.B) && (color.B <= max.B)) &&
                   ((color.A >= min.A) && (color.A <= max.A));
        }

        public static Color Change(this Color color, byte direction)
        {
            return new Color((byte)MathHelper.Clamp(color.R + direction, 0, 255),
                             (byte)MathHelper.Clamp(color.G + direction, 0, 255),
                             (byte)MathHelper.Clamp(color.B + direction, 0, 255),
                             (byte)MathHelper.Clamp(color.A + direction, 0, 255));
        }
        public static byte Change(this byte value, int direction)
        {
            return (byte)MathHelper.Clamp(value + direction, 0, 255);
        }

        private static Color glowGoldUI = Color.Lerp(new Color(165, 174, 124, 255), Color.Gold, .5f);
        private static Color lightGoldUI = Color.Lerp(new Color(165, 174, 124, 255), Color.Gold, .25f);
        private static Color goldUI = new Color(165, 174, 124, 255);
        private static Color darkGoldUI = new Color(72, 76, 55, 255);
        private static Color moneyUI = Color.Lerp(Color.LightGreen, Color.Green, .5f);

        private static Color charcoal = new Color(25, 25, 25, 255);
        private static Color darkGray = new Color(64, 64, 64, 255);

        //Desaturated
        private static Color desatBlue = new Color(95, 111, 191, 255); //= new Color(, , , 255)
        private static Color desatPurple = new Color(164, 95, 191, 255);
        private static Color desatRed = new Color(191, 95, 95, 255);
        private static Color desatOrange = new Color(191, 148, 95, 255);
        private static Color desatYellow = new Color(191, 175, 95, 255);
        private static Color desatGreen = new Color(95, 191, 95, 255);
        private static Color desatCyan = new Color(95, 191, 95, 255);

        public static Color D_Blue { get { return desatBlue; } }
        public static Color D_Purple { get { return desatPurple; } }
        public static Color D_Red { get { return desatRed; } }
        public static Color D_Orange { get { return desatOrange; } }
        public static Color D_Yellow { get { return desatYellow; } }
        public static Color D_Green { get { return desatGreen; } }
        public static Color D_Cyan { get { return desatCyan; } }

        //Desaturated and Dark
        private static Color desatDarkBlue = new Color(63, 63, 127, 255);
        private static Color desatDarkPurple = new Color(109, 63, 127, 255);
        private static Color desatDarkRed = new Color(127, 63, 63, 255);
        private static Color desatDarkOrange = new Color(127, 95, 63, 255);
        private static Color desatDarkYellow = new Color(127, 127, 63, 255);
        private static Color desatDarkGreen = new Color(63, 127, 63, 255);
        private static Color desatDarkCyan = new Color(63, 127, 127, 255);

        public static Color DD_Blue { get { return desatDarkBlue; } }
        public static Color DD_Purple { get { return desatDarkPurple; } }
        public static Color DD_Red { get { return desatDarkRed; } }
        public static Color DD_Orange { get { return desatDarkOrange; } }
        public static Color DD_Yellow { get { return desatDarkYellow; } }
        public static Color DD_Green { get { return desatDarkGreen; } }
        public static Color DD_Cyan { get { return desatDarkCyan; } }

        public static Color UI_GlowingGold { get { return glowGoldUI; } }
        public static Color UI_LightGold { get { return lightGoldUI; } }
        public static Color UI_Gold { get { return goldUI; } }
        public static Color UI_DarkerGold { get { return darkGoldUI; } }
        public static Color UI_Money { get { return moneyUI; } }

        /// <summary>
        /// 25, 25, 25, 255
        /// </summary>
        public static Color Charcoal { get { return charcoal; } }
        public static Color DarkGray { get { return darkGray; } }

        private static string RGBAToHex(Color color)
        {
            return color.R.ToString("X2") +
                   color.G.ToString("X2") +
                   color.B.ToString("X2") +
                   color.A.ToString("X2");
        }
        private static Color HexToRGBA(string hex)
        {
            hex = hex.Replace("#", ""); //Remove hashtag at beginning if it has one

            if (hex.Length % 2 != 0)
                return Color.Transparent; //If the hex code is invalid (odd number of characters), return transparent.

            string r = (hex[0] + hex[1]).ToString();
            string g = (hex[2] + hex[3]).ToString();
            string b = (hex[4] + hex[5]).ToString();
            string a = (hex[6] + hex[7]).ToString();

            return new Color(byte.Parse(r, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(g, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(b, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(a, System.Globalization.NumberStyles.HexNumber));
        }

        public static string RGBToHex(Color color)
        {
            return color.R.ToString("X2") +
                   color.G.ToString("X2") +
                   color.B.ToString("X2");
        }
        public static Color HexToRGB(string hex)
        {
            hex = hex.Replace("#", ""); //Remove hashtag at beginning if it has one

            if (hex.Length % 2 != 0)
                return Color.Transparent; //If the hex code is invalid (odd number of characters), return transparent.

            string r = (hex[0] + hex[1]).ToString();
            string g = (hex[2] + hex[3]).ToString();
            string b = (hex[4] + hex[5]).ToString();

            return new Color(byte.Parse(r, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(g, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(b, System.Globalization.NumberStyles.HexNumber),
                             255);
        }
    }
}
