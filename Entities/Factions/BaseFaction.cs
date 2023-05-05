using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Entities.Factions
{
    public class BaseFaction
    {
        private int id;
        private string name;
        private Texture2D icon;
        private Color color;

        public enum Morality
        {
            Evil,
            Impure,
            Bad,

            Neutral,

            Good,
            Pure,
            Righteous
        }
        Morality morality = Morality.Neutral;

        public enum Disposition
        {
            Despise = -3,
            Tense = -2,
            Dislike = -1,

            Neutral = 0,

            Like = 1,
            Fond = 2,
            Love = 3,
        }
        public Dictionary<int, Disposition> factionDispositions = new Dictionary<int,Disposition>();
        private Disposition defaultDisposition;

        public int ID { get { return id; } }
        public string Name { get { return name; } }
        public Texture2D Icon { get { return icon; } }
        public Color Color { get { return color; } }

        private Action<BaseFaction> action;

        public BaseFaction(int ID, string Name, Texture2D Icon, Morality Morality, Color Color, Disposition Default, Action<BaseFaction> PostAction)
        {
            icon = Icon;
            name = Name;
            id = ID;

            morality = Morality;
            color = Color;
            defaultDisposition = Default;

            action = PostAction;
        }

        public virtual void AddFactions()
        {
            AddFaction(id, Disposition.Love);

            action.Invoke(this);
        }

        public void AddFaction(int id, Disposition disposition)
        {
            for (int i = 0; i < FactionDatabase.Factions.Count; i++)
            {
                if (FactionDatabase.Factions[i].ID == id)
                    factionDispositions.Add(id, disposition);
            }
        }
        public void RemoveFaction(int id)
        {
            factionDispositions.Remove(id);
        }
        public Disposition GetDisposition(int id) //untested code!
        {
            Disposition result;
            return factionDispositions.TryGetValue(id, out result) ? result : defaultDisposition; //Decide if neutral is the best option to default to!
        }

        public override string ToString()
        {
            return id.ToString() + " - " + name + " : " + morality.ToString();
        }
    }
}
