namespace Pilgrimage_Of_Embers.Skills
{
    public class ExperienceMultiplier
    {
        static int[] levelExperience = new int[100];

        static ExperienceMultiplier()
        {
            AssignLevelExperience();
        }

        private static void AssignLevelExperience()
        {
            /*
            for (int i = 1; i < levelExperience.Length + 1; i++)
            {
                if (i == 1)
                    levelExperience[i - 1] = MultiplyByLevel(i);
                else
                    levelExperience[i - 1] = levelExperience[i - 2] + MultiplyByLevel(i);
            }*/

            //This code should function the same as above, but not as messy.
            for (int i = 0; i < levelExperience.Length; i++)
            {
                if (i == 0) //Level 1
                    levelExperience[i] = MultiplyByLevel(i);
                else
                    levelExperience[i] = levelExperience[i - 1] + MultiplyByLevel(i); //Level experience equals last level experience + current level experience.
            }
        }

        /// <summary>
        /// Dispose of soon! Use LevelExperience(level) instead.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int MultiplyByLevel(int level)
        {
            int exp = 0;

            if (level < 10 && level > 0)
                exp += 200;
            else if (level < 20 && level >= 10)
                exp += 400;
            else if (level < 30 && level >= 20)
                exp += 600;
            else if (level < 40 && level >= 30)
                exp += 800;
            else if (level < 50 && level >= 40)
                exp += 1000;
            else if (level < 60 && level >= 50)
                exp += 1200;
            else if (level < 70 && level >= 60)
                exp += 1400;
            else if (level < 80 && level >= 70)
                exp += 1600;
            else if (level < 90 && level >= 80)
                exp += 1800;
            else if (level < 95 && level >= 90)
                exp += 3000;
            else if (level < 100 && level >= 95)
                exp += 6300;

            return exp;
        }

        public static int LevelExperience(int level)
        {
            return levelExperience[level];
        }
    }
}
