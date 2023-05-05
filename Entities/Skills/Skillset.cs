using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pilgrimage_Of_Embers.Skills
{
    public class Skillset
    {
        public Health health;
        public Endurance endurance;
        public Agility agility;
        public Resistance resistance;
        public Strength strength;
        public Archery archery;
        public Wisdom wisdom;
        public Intelligence intelligence;

        public Trapping trapping;
        public Awareness awareness;
        public Concealment concealment;
        public Looting looting;

        int experience;
        public int ExperiencePoints { get { return experience; } set { experience = MathHelper.Clamp(value, 0, int.MaxValue); } }
        public float ExperienceMultiplier { get; set; }

        /// <summary>
        /// Sets all levels to default of 1. Use this only for testing purposes.
        /// </summary>
        public Skillset() : this (1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)
        {
        }
        public Skillset(int HealthLevel, int EnduranceLevel, int ResistanceLevel, int StrengthLevel,
                        int AgilityLevel, int ArcheryLevel, int IntelligenceLevel, int WisdomLevel,
                        int TrappingLevel, int AwarenessLevel, int ConcealmentLevel, int LootingLevel)
        {
            health = new Health(HealthLevel);
            endurance = new Endurance(EnduranceLevel);
            agility = new Agility(AgilityLevel);

            strength = new Strength(StrengthLevel);
            resistance = new Resistance(ResistanceLevel);
            archery = new Archery(ArcheryLevel);
            intelligence = new Intelligence(IntelligenceLevel);
            wisdom = new Wisdom(WisdomLevel);

            trapping = new Trapping(TrappingLevel);
            awareness = new Awareness(AwarenessLevel);
            concealment = new Concealment(ConcealmentLevel);
            looting = new Looting(LootingLevel);

            ExperienceMultiplier = 1f;
        }
        public Skillset(string StateFilePath)
        {
            stateFilePath = StateFilePath;
            LoadFromFile(stateFilePath);
        }

        public void SetReferences(Entities.ObjectAttributes attributes)
        {
            health.SetReferences(this, attributes);
            endurance.SetReferences(this, attributes);
            agility.SetReferences(this, attributes);

            strength.SetReferences(this, attributes);
            resistance.SetReferences(this, attributes);
            archery.SetReferences(this, attributes);
            wisdom.SetReferences(this, attributes);
            intelligence.SetReferences(this, attributes);

            trapping.SetReferences(this, attributes);
            awareness.SetReferences(this, attributes);
            concealment.SetReferences(this, attributes);
            looting.SetReferences(this, attributes);
        }

        public void UpdateSkillset(GameTime gt)
        {
            UpdateDefense(gt);
            ResetMultipliers();
        }
        private void ResetMultipliers()
        {
            health.ResetMultipliers();
            endurance.ResetMultipliers();
            resistance.ResetMultipliers();

            strength.ResetMultipliers();
            agility.ResetMultipliers();
            archery.ResetMultipliers();

            wisdom.ResetMultipliers();
            intelligence.ResetMultipliers();

            trapping.ResetMultipliers();
            awareness.ResetMultipliers();
            concealment.ResetMultipliers();
            looting.ResetMultipliers();

            ExperienceMultiplier = 1f;
        }

        private int stunTime = 0;
        private float previousStun;
        private bool isStunPause;
        private void UpdateDefense(GameTime gt)
        {
            if (previousStun != resistance.CurrentStun)
            {
                isStunPause = true; //Entity has been hit, redo stun time;

                stunTime = 0;
            }

            if (isStunPause == true)
            {
                stunTime += gt.ElapsedGameTime.Milliseconds;

                if (stunTime > 1000)
                {
                    stunTime = 0;
                    isStunPause = false;
                }
            }
            else
            {
                resistance.CurrentStun -= 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            previousStun = resistance.CurrentStun;
        }

        public int CombinedLevels
        {
            get
            {
                return health.Level +
                  endurance.Level +
                  resistance.Level +
                  strength.Level +
                  agility.Level +
                  archery.Level +
                  intelligence.Level +
                  wisdom.Level +
                  trapping.Level +
                  awareness.Level +
                  concealment.Level +
                  looting.Level;
            }
        }
        public int AverageXPToLevel
        {
            get
            {
                return (health.Experience +
                       endurance.Experience +
                       resistance.Experience +
                       strength.Experience +
                       agility.Experience +
                       archery.Experience +
                       intelligence.Experience +
                       wisdom.Experience +
                       trapping.Experience +
                       awareness.Experience +
                       concealment.Experience +
                       looting.Experience) / 12;
            }
        }

        public bool CompareGreaterThan(BaseSkill skill)
        {
            if (ExperiencePoints >= skill.Experience)
                return true;

            return false;
        }
        public void RemoveEXP(BaseSkill skill)
        {
            ExperiencePoints -= skill.Experience;
        }
        public void RemoveEXP(int amount) { ExperiencePoints -= amount; }
        public void AddEXP(int amount) { ExperiencePoints += (int)(amount * ExperienceMultiplier); }

        public void RestoreSkills()
        {
            health.CurrentHP = health.MaxHP;
            wisdom.CurrentEnergy = wisdom.MaxEnergy;
            endurance.CurrentStamina = endurance.MaxStamina;
        }

        private string stateFilePath;
        public void ReloadState()
        {
            if (!string.IsNullOrEmpty(stateFilePath))
                LoadFromFile(stateFilePath);
        }
        public void LoadFromFile(string file)
        {
            if (File.Exists(file))
            {
                string[] lines = File.ReadAllLines(file);
                ParseFromFile(lines.ToList());
            }
        }
        public void ParseFromFile(List<string> data)
        {
            bool isReading = false;

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToUpper().StartsWith("[SKILLS]"))
                {
                    isReading = false;
                    break;
                }
                if (data[i].ToUpper().StartsWith("[/SKILLS]"))
                    isReading = true;

                if (isReading == true)
                {
                    if (data[i].ToUpper().StartsWith("HEALTH"))
                    {
                        string[] words = data[i].Split(' ');
                        health.SetEntityLevel(int.Parse(words[1]));
                        health.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("ENDURANCE"))
                    {
                        string[] words = data[i].Split(' ');
                        endurance.SetEntityLevel(int.Parse(words[1]));
                        endurance.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("RESISTANCE"))
                    {
                        string[] words = data[i].Split(' ');
                        resistance.SetEntityLevel(int.Parse(words[1]));
                        resistance.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("STRENGTH"))
                    {
                        string[] words = data[i].Split(' ');
                        strength.SetEntityLevel(int.Parse(words[1]));
                        strength.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("AGILITY"))
                    {
                        string[] words = data[i].Split(' ');
                        agility.SetEntityLevel(int.Parse(words[1]));
                        agility.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("ARCHERY"))
                    {
                        string[] words = data[i].Split(' ');
                        archery.SetEntityLevel(int.Parse(words[1]));
                        archery.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("INTELLIGENCE"))
                    {
                        string[] words = data[i].Split(' ');
                        intelligence.SetEntityLevel(int.Parse(words[1]));
                        intelligence.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("WISDOM"))
                    {
                        string[] words = data[i].Split(' ');
                        wisdom.SetEntityLevel(int.Parse(words[1]));
                        wisdom.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("TRAPPING"))
                    {
                        string[] words = data[i].Split(' ');
                        trapping.SetEntityLevel(int.Parse(words[1]));
                        trapping.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("AWARENESS"))
                    {
                        string[] words = data[i].Split(' ');
                        awareness.SetEntityLevel(int.Parse(words[1]));
                        awareness.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("CONCEALMENT"))
                    {
                        string[] words = data[i].Split(' ');
                        concealment.SetEntityLevel(int.Parse(words[1]));
                        concealment.AssignValues();
                    }
                    if (data[i].ToUpper().StartsWith("LOOTING"))
                    {
                        string[] words = data[i].Split(' ');
                        looting.SetEntityLevel(int.Parse(words[1]));
                        looting.AssignValues();
                    }
                }
            }
        }

        public Skillset Copy()
        {
            Skillset copy = (Skillset)MemberwiseClone();

            copy.health = (Health)health.Copy();
            copy.endurance = (Endurance)endurance.Copy();
            copy.agility = (Agility)agility.Copy();
            copy.resistance = (Resistance)resistance.Copy();
            copy.strength = (Strength)strength.Copy();
            copy.archery = (Archery)archery.Copy();
            copy.wisdom = (Wisdom)wisdom.Copy();
            copy.intelligence = (Intelligence)intelligence.Copy();

            copy.trapping = (Trapping)trapping.Copy();
            copy.awareness = (Awareness)awareness.Copy();
            copy.concealment = (Concealment)concealment.Copy();
            copy.looting = (Looting)looting.Copy();

            return copy;
        }
    }
}
