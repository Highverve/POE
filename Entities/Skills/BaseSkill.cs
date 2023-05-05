using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Skills
{
    public class BaseSkill
    {
        public const int MaxPlayerLevel = 100, MaxEntityLevel = 999;

        private int level, experience = 0;
        private string description;

        public int Level { get { return level; } }
        public int Experience { get { return experience; } }
        public string Description { get { return description; } }

        protected Skillset skills;
        protected Entities.ObjectAttributes attributes;

        public BaseSkill(int Level, string Description)
        {
            level = Level;
            description = Description;
        }
        public void SetReferences(Skillset skills, Entities.ObjectAttributes attributes)
        {
            this.skills = skills;
            this.attributes = attributes;

            AssignValues();
        }
        public virtual void AssignValues()
        {
            if (level < 100)
                experience = ExperienceMultiplier.LevelExperience(level);
            else if (level == 100)
                experience = 0;
        }
        public virtual void ResetMultipliers()
        {
        }

        public void FortifyLevel()
        {
            if (level < MaxPlayerLevel)
            {
                level++;
                AssignValues();
            }
        }

        public void SetPlayerLevel(int value)
        {
            level = (int)MathHelper.Clamp(value, 1, MaxPlayerLevel);
            AssignValues();
        }
        public void SetEntityLevel(int value)
        {
            level = (int)MathHelper.Clamp(value, 1, MaxEntityLevel);
            AssignValues();
        }

        public virtual BaseSkill Copy()
        {
            BaseSkill copy = (BaseSkill)MemberwiseClone();

            return copy;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
