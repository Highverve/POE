using Pilgrimage_Of_Embers.Helper_Classes;
using System.Collections.Generic;
using System.Text;

namespace Pilgrimage_Of_Embers.Entities
{
    public class ObjectStat
    {
        public int Integer { get; set; }
        public float Float { get; set; }
        public string String { get; set; }

        public ObjectStat(int Integer) : this(Integer, 0f, string.Empty) { }
        public ObjectStat(float Float) : this(0, Float, string.Empty) { }
        public ObjectStat(string String) : this(0, 0f, String) { }

        private ObjectStat(int Integer, float Float, string String)
        {
            this.Integer = Integer;
            this.Float = Float;
            this.String = String;
        }
    }
    public class ObjectStatistics
    {
        private Dictionary<string, ObjectStat> nameStat;

        public ObjectStatistics(Dictionary<string, ObjectStat> NameStats = null, bool isAddDefaults = true)
        {
            if (NameStats == null)
                nameStat = new Dictionary<string, ObjectStat>(System.StringComparer.OrdinalIgnoreCase);
            else
                nameStat = new Dictionary<string, ObjectStat>(NameStats, System.StringComparer.OrdinalIgnoreCase);

            if (isAddDefaults == true)
                SetDefaults();
        }
        private void SetDefaults()
        {
            //Default stats go here.
            nameStat.Add("total_deaths", new ObjectStat(0)); //Hook these guys up sometime.
            nameStat.Add("monuments_activated", new ObjectStat(0));
            nameStat.Add("beings_slain", new ObjectStat(0));

            nameStat.Add("embers_collected", new ObjectStat(0));
            nameStat.Add("most_embers_held", new ObjectStat(0));
            nameStat.Add("most_embers_lost", new ObjectStat(0));
        }

        public void SetStat(string name, int value)
        {
            if (nameStat.ContainsKey(name))
                nameStat[name].Integer = value;
            else
                nameStat.Add(name, new ObjectStat(value));
        }
        public void SetStat(string name, float value)
        {
            if (nameStat.ContainsKey(name))
                nameStat[name].Float = value;
            else
                nameStat.Add(name, new ObjectStat(value));
        }
        public void SetStat(string name, string value)
        {
            if (nameStat.ContainsKey(name))
                nameStat[name].String = value;
            else
                nameStat.Add(name, new ObjectStat(value));
        }

        public void AdjustStat(string name, int value)
        {
            if (nameStat.ContainsKey(name))
                nameStat[name].Integer += value;
            else
                nameStat.Add(name, new ObjectStat(value));
        }
        public void AdjustStat(string name, float value)
        {
            if (nameStat.ContainsKey(name))
                nameStat[name].Float += value;
            else
                nameStat.Add(name, new ObjectStat(value));
        }
        public void AdjustStat(string name, string value)
        {
            if (nameStat.ContainsKey(name))
                nameStat[name].String += value;
            else
                nameStat.Add(name, new ObjectStat(value));
        }

        public int GetStatInteger(string name)
        {
            if (nameStat.ContainsKey(name))
                return nameStat[name].Integer;

            return 0;
        }
        public float GetStatFloat(string name)
        {
            if (nameStat.ContainsKey(name))
                return nameStat[name].Float;

            return 0;
        }
        public string GetStatString(string name)
        {
            if (nameStat.ContainsKey(name))
                return nameStat[name].String;

            return string.Empty;
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            List<string> keyList = new List<string>(nameStat.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                if (nameStat[keyList[i]].Integer != 0)
                    builder.AppendLine("[INT] \"" + keyList[i] + "\" \"" + nameStat[keyList[i]].Integer + "\"");
                if (nameStat[keyList[i]].Float != 0)
                    builder.AppendLine("[FLOAT] \"" + keyList[i] + "\" \"" + nameStat[keyList[i]].Float + "\"");
                if (!string.IsNullOrEmpty(nameStat[keyList[i]].String))
                    builder.AppendLine("[STRING] \"" + keyList[i] + "\" \"" + nameStat[keyList[i]].String + "\"");
            }

            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                string tag = data[i].FromWithin('[', ']', 1);
                string name = data[i].FromWithin('"', 1);
                string value = data[i].FromWithin('"', 2);

                if (tag.ToUpper().Equals("INT"))
                    SetStat(name, int.Parse(value));
                if (tag.ToUpper().Equals("FLOAT"))
                    SetStat(name, float.Parse(value));
                if (tag.ToUpper().Equals("STRING"))
                    SetStat(name, value);
            }
        }

        public ObjectStatistics Copy()
        {
            return new ObjectStatistics(this.nameStat, false);
        }
    }
}
