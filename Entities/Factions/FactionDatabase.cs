using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities.Factions
{
    public class FactionDatabase
    {
        static List<BaseFaction> factionsInDatabase = new List<BaseFaction>();
        public static List<BaseFaction> Factions { get { return factionsInDatabase; } }

        public static void LoadFactions(ContentManager main)
        {
            //Starting faction
            factionsInDatabase.Add(new BaseFaction(1, "Pilgrims of the Embers", main.Load<Texture2D>("Factions/pilgrimsOfEmbers"), BaseFaction.Morality.Good, new Color(155, 83, 5, 255), BaseFaction.Disposition.Neutral,
            (BaseFaction f) =>
            {
                f.AddFaction(2, BaseFaction.Disposition.Love);
                f.AddFaction(100, BaseFaction.Disposition.Despise);
            }));

            //New game+ faction
            factionsInDatabase.Add(new BaseFaction(2, "Friars of the Embers", null, BaseFaction.Morality.Righteous, Color.White, BaseFaction.Disposition.Like, (BaseFaction f) =>
            {
                f.AddFaction(1, BaseFaction.Disposition.Fond);
                f.AddFaction(100, BaseFaction.Disposition.Despise);
            }));

            //All enemies
            factionsInDatabase.Add(new BaseFaction(100, "The Flameless", null, BaseFaction.Morality.Evil, Color.White, BaseFaction.Disposition.Despise, (BaseFaction f) =>
            {
                f.AddFaction(1, BaseFaction.Disposition.Despise);
                f.AddFaction(2, BaseFaction.Disposition.Despise);
            }));

            ApplyDispositions();
        }
        private static void ApplyDispositions()
        {
            for (int i = 0; i < Factions.Count; i++)
                Factions[i].AddFactions();
        }

        public static BaseFaction GetFaction(int id)
        {
            for (int i = 0; i < Factions.Count; i++)
            {
                if (Factions[i].ID == id)
                {
                    return Factions[i];
                }
            }

            return null;
        }
    }
}
