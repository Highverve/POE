using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Skills;
using System;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Entities.Types.Player
{
    public class StartInfo
    {
        List<Tuple<string, Skillset, Point[], string>> classes = new List<Tuple<string, Skillset, Point[], string>>();
        public List<Tuple<string, Skillset, Point[], string>> Classes { get { return classes; } }
        public Tuple<string, Skillset, Point[], string> RetrieveClass(string name)
        {
            for (int i = 0; i < classes.Count; i++)
            {
                if (classes[i].Item1.ToUpper().Equals(name.ToUpper()))
                    return classes[i];
            }

            return null;
        }

        List<Tuple<string, string, int>> pathways = new List<Tuple<string, string, int>>();
        public List<Tuple<string, string, int>> Pathways { get { return pathways; } }
        public Tuple<string, string, int> RetrievePathway(string name)
        {
            for (int i = 0; i < pathways.Count; i++)
            {
                if (pathways[i].Item1.ToUpper().Equals(name.ToUpper()))
                    return pathways[i];
            }

            return null;
        }

        List<Tuple<string, string, string, Point>> birthplaces = new List<Tuple<string, string, string, Point>>();
        public List<Tuple<string, string, string, Point>> Birthplaces { get { return birthplaces; } }
        public Tuple<string, string, string, Point> RetrieveBirthplace(string name)
        {
            for (int i = 0; i < birthplaces.Count; i++)
            {
                if (birthplaces[i].Item1.ToUpper().Equals(name.ToUpper()))
                    return birthplaces[i];
            }

            return null;
        }

        public StartInfo()
        {
            InitializeClasses();
            InitializePathways();
            InitializeBirthplaces();
        }

        private void InitializeClasses()
        {
            classes.Add(Tuple.Create("Swordsman", new Skillset(13, 17, 10, 9, 14, 6, 2, 4, 2, 7, 11, 5), new Point[] { new Point(50, 1) }, "Wielder of swords, they prefer endurance \nand agility over defense and \nstrength."));
            classes.Add(Tuple.Create("Spearguard", new Skillset(14, 13, 16, 9, 11, 4, 5, 3, 3, 14, 7, 1), new Point[] { }, "A spearguard of the east, they prefer \ndefense over strength and have a \ngood amount of health."));
            classes.Add(Tuple.Create("Warrior", new Skillset(13, 12, 9, 17, 12, 3, 4, 5, 5, 7, 2, 11), new Point[] { }, "A mighty warrior, they prefer strength over \ndefense and have a moderate amount \nof health."));

            classes.Add(Tuple.Create("Archer", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Farshooter", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Flaskmaster", new Skillset(), new Point[] { }, ""));

            classes.Add(Tuple.Create("Arcanist", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Sorcerer", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Battlemage", new Skillset(), new Point[] { }, ""));

            classes.Add(Tuple.Create("Inhabitant", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Vigorian", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Cenobite", new Skillset(), new Point[] { }, ""));

            classes.Add(Tuple.Create("Concealar", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Stalker", new Skillset(), new Point[] { }, ""));
            classes.Add(Tuple.Create("Lootmaster", new Skillset(), new Point[] { }, ""));

            classes.Add(Tuple.Create("Barbarian", new Skillset(), new Point[] { }, ""));
        }
        private void InitializePathways()
        {
            pathways.Add(Tuple.Create("Defender's Oath", "You were a shieldmaster's squire. (+10% shield blocking, +10% damage reduction)", 1));
            pathways.Add(Tuple.Create("Piercing Sight", "You were once a deer hunter. (+10% archery accuracy, start with extra arrows.", 2));
            pathways.Add(Tuple.Create("Caster's Wit", "You were once a Spellmaster's apprentice. (+10% magical damage, start with extra spells)", 3));
            pathways.Add(Tuple.Create("Locksmith's Key", "You were once a locksmith. (Start with a few keys)", 4));

            pathways.Add(Tuple.Create("Observant", "You see what most do not. (+10% XP gain, +10% Awareness)", 5));
            pathways.Add(Tuple.Create("Scavenger", "You find what most others pass over. (+10% looting)", 6));
            pathways.Add(Tuple.Create("Ghost", "You are the unseen. (+25% concealment, -10% around those who bear the \"Observant\" pathway)", 7));
            pathways.Add(Tuple.Create("Hermit", "You have lived an agile life in the mountains. (-25% stamina usage, -10% defense)", 8));
        }
        private void InitializeBirthplaces()
        {
            birthplaces.Add(Tuple.Create("Forest of the Fairies", "Born to a travelling family, you were lost along the way.", "SmallForest.map", new Point(32, 32)));
        }

        private void LoadCustomClasses(string file)
        {
            //ClassName HP End Def Str Agi Arc Int Mag Trap Aware Con Loot [ID Quantity, ...] "Description"
        }
        private void LoadCustomPathways(string file)
        {
            //ID "PathwayName" "Description"
        }
        private void LoadCustomBirthplaces(string file)
        {
            //tileX tileY "BirthplaceName" "Description" "MapName"
        }
    }
}
