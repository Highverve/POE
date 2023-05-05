using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pilgrimage_Of_Embers.ScreenEngine.RumorsNotes
{
    public class RumorDatabase
    {
        private static List<Rumor> rumors = new List<Rumor>();
        public static List<Rumor> Rumors { get { return rumors; } }

        private static Debugging.DebugManager debug;
        public static Debugging.DebugManager Debug { set { debug = value; } }

        public static void LoadRumors()
        {
            rumors.Add(new Rumor(1, "Island Mage Rumor", "He tells me he is a master mage, but I have my doubts. I should ask him more about magic.",
                                    "Well, it turns out he can cast a spell or two. Though he's not a master mage.",
                                    "As I suspected, he was just a self-important old island merchant."));

            CheckConflictingIDs();
        }
        private static void CheckConflictingIDs()
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                for (int j = i + 1; j < rumors.Count; j++)
                {
                    if (rumors[i].ID == rumors[j].ID)
                        debug.OutputError("WARNING: conflicting ID found: " + rumors[i].ID);
                }
            }
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Rumors (Total: " + rumors.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            rumors.OrderBy(x => x.ID);

            for (int i = 0; i < rumors.Count; i++)
            {
                builder.AppendLine(rumors[i].ID + " - " + rumors[i].FullHeader);
            }

            return builder;
        }
    }
}
