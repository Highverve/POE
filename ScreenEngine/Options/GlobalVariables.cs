using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers
{
    public static class GlobalVariables
    {
        private static float gameSpeed = 1f;
        /// <summary>
        /// For slow motion stuff and what not.
        /// </summary>
        public static float GameSpeedMultiplier { get { return gameSpeed; } set { gameSpeed = MathHelper.Clamp(value, .1f, 2f); } }
    }
}
