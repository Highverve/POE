using System;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Skills;

namespace Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes.Helper
{
    public class Requirements
    {
        int strengthLevel = 0;
        public int StrengthLevel { get { return strengthLevel; } set { strengthLevel = (int)MathHelper.Clamp(value, 0, 100); } }

        int defenseLevel = 0;
        public int DefenseLevel { get { return defenseLevel; } set { defenseLevel = (int)MathHelper.Clamp(value, 0, 100); } }

        int intelligenceLevel = 0, magicLevel = 0;
        public int IntelligenceLevel { get { return intelligenceLevel; } set { intelligenceLevel = (int)MathHelper.Clamp(value, 0, 100); } }
        public int MagicLevel { get { return magicLevel; } set { magicLevel = (int)MathHelper.Clamp(value, 0, 100); } }

        int archeryLevel = 0;
        public int ArcheryLevel { get { return archeryLevel; } set { archeryLevel = (int)MathHelper.Clamp(value, 0, 100); } }

        int healthLevel = 0;
        public int HealthLevel { get { return healthLevel; } set { healthLevel = (int)MathHelper.Clamp(value, 0, 100); } }

        public Requirements(int StrengthLevel = 0, int DefenseLevel = 0, int IntelligenceLevel = 0, int MagicLevel = 0, int ArcheryLevel = 0, int HealthLevel = 0)
        {
            this.StrengthLevel = StrengthLevel;
            this.DefenseLevel = DefenseLevel;
            this.IntelligenceLevel = IntelligenceLevel;
            this.MagicLevel = MagicLevel;
            this.ArcheryLevel = ArcheryLevel;
            this.HealthLevel = HealthLevel;
        }

        public bool HasRequirements(Skillset skills)
        {
            if (skills.strength.Level < strengthLevel)
                return false;
            if (skills.resistance.Level < defenseLevel)
                return false;
            if (skills.intelligence.Level < intelligenceLevel)
                return false;
            if (skills.wisdom.Level < magicLevel)
                return false;
            if (skills.archery.Level < archeryLevel)
                return false;
            if (skills.health.Level < healthLevel)
                return false;

            return true;
        }

        /// <summary>
        /// Get the average percentage of required levels. Use this to determine the effectiveness of the item!
        /// </summary>
        /// <param name="skills"></param>
        /// <returns></returns>
        public float RequirementPercentage(Skillset skills, int percentageDivide) //req 20 str 12
        {
            float totalPct = 0f;
            int divideCount = 0;

            totalPct += SkillPercent(skills.health.Level, healthLevel, ref divideCount);
            totalPct += SkillPercent(skills.strength.Level, strengthLevel, ref divideCount);
            totalPct += SkillPercent(skills.resistance.Level, defenseLevel, ref divideCount);
            totalPct += SkillPercent(skills.intelligence.Level, intelligenceLevel, ref divideCount);
            totalPct += SkillPercent(skills.wisdom.Level, magicLevel, ref divideCount);
            totalPct += SkillPercent(skills.archery.Level, archeryLevel, ref divideCount);

            if (divideCount > 0)
            {
                totalPct /= divideCount;
                totalPct -= (totalPct / percentageDivide);
            }
            else
                totalPct = 1f;

            return totalPct;
        }
        private float SkillPercent(int currentLevel, int reqLevel, ref int divideCount)
        {
            if (reqLevel > 0)
            {
                float pct = (float)currentLevel / (float)reqLevel;

                if (pct < 1f)
                {
                    divideCount++;
                    return pct;
                }
            }

            return 0f;
        }

        public string RequirementsToString(Skillset skills)
        {
            string temp = "";

            if (skills.health.Level < healthLevel)
                temp += "Health: " + healthLevel.ToString() + Environment.NewLine;
            if (skills.resistance.Level < defenseLevel)
                temp += "Defense: " + DefenseLevel.ToString() + Environment.NewLine;
            if (skills.strength.Level < strengthLevel)
                temp += "Strength: " + strengthLevel.ToString() + Environment.NewLine;
            if (skills.archery.Level < archeryLevel)
                temp += "Archery: " + archeryLevel.ToString() + Environment.NewLine;
            if (skills.intelligence.Level < intelligenceLevel)
                temp += "Intelligence: " + intelligenceLevel.ToString() + Environment.NewLine;
            if (skills.wisdom.Level < magicLevel)
                temp += "Magic: " + magicLevel.ToString() + Environment.NewLine;

            return temp;
        }
    }
}
