using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pilgrimage_Of_Embers.ScreenEngine.Souls.Types;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.ScreenEngine.Souls
{
    public class SoulsDatabase
    {
        private static List<BaseSoul> souls = new List<BaseSoul>();
        public static List<BaseSoul> Souls { get { return souls; } }

        private const string directory = "Souls/";
        public static void LoadSouls(ContentManager cm)
        {
            souls.Add(new TestSoul(1, "Soul of Yitma", "Taken from Yitma the Beastling. This soul gives the user the ability to comprehend dialects of animals, monsters, and other such creatures.", cm.Load<Texture2D>(directory + "soul1"), cm.Load<Texture2D>(directory + "soul1Large"), 2000, 5000, 5));
            souls.Add(new TestSoul(2, "Slayer's Soul", "Taken from Redheart the Slayer. This soul grants the user a boost to strength, being most effective when fighting demons.", cm.Load<Texture2D>(directory + "soul2"), cm.Load<Texture2D>(directory + "soul2Large"), 2000, 5000, 5));
            souls.Add(new TestSoul(3, "Soul of an Old Necromancer", "Taken from Cecily the Blind Necromancer. This soul gives the user the ability to regenerate magical energy, a soul very useful for a necromancer.", cm.Load<Texture2D>(directory + "soul3"), cm.Load<Texture2D>(directory + "soul3Large"), 2000, 5000, 5));
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Souls (Total: " + souls.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            souls.OrderBy(x => x.ID);

            for (int i = 0; i < souls.Count; i++)
            {
                builder.AppendLine(souls[i].ID + " - " + souls[i].Name + " [Level Up: " + souls[i].CanLevelUp + ", Is Unlimited: " + souls[i].IsUnlimited + ", Max Charges: " + souls[i].MaxCharges + "]");
            }

            return builder;
        }
    }
}
