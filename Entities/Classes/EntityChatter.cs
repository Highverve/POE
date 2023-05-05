using Pilgrimage_Of_Embers.Helper_Classes;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pilgrimage_Of_Embers.Entities.Classes
{
    public class EntityChat
    {
        private List<Tuple<string, string>> lines = new List<Tuple<string, string>>();
        public List<Tuple<string, string>> Lines { get { return lines; } }

        Random random;

        public EntityChat(string entityName)
        {
            ReadChatterFile(entityName);
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        private const string directory = "MainContent/Dialogue/Chat/", ext = ".cht";
        public void ReadChatterFile(string entityName)
        {
            if (!string.IsNullOrEmpty(entityName))
            {
                if (File.Exists(directory + entityName + ext))
                {
                    using (StreamReader reader = new StreamReader(directory + entityName + ext))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine().Trim();

                            if (!line.StartsWith(@"//"))
                                ParseLine(line);
                        }
                    }
                }
            }
        }

        private void ParseLine(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                string key, content;

                key = line.FromWithin('"', 1);
                content = line.FromWithin('"', 2);

                lines.Add(Tuple.Create(key, content));
            }
        }

        public bool CheckKey(string key)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Item1.ToUpper().Equals(key.ToUpper()))
                    return true;
            }

            return false;
        }

        private List<string> totalResults = new List<string>();
        public string RetrieveRandom(string key)
        {
            totalResults.Clear();

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Item1.ToUpper().Equals(key.ToUpper()))
                    totalResults.Add(lines[i].Item2);
            }

            if (totalResults.Count > 0)
                return totalResults[random.Next(0, totalResults.Count)];
            else
                return string.Empty;
        }
    }
}
