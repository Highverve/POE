using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pilgrimage_Of_Embers.Entities.Classes
{
    public class MemoryPacket
    {
        private string key, memory;
        private int lifeTime;
        private int priority;

        public string Key { get { return key; } }
        public string Memory { get { return memory; } }
        public int LifeTime { get { return lifeTime; } set { lifeTime = value; } }
        public int Priority { get { return priority; } }

        /// <summary>
        /// A memory packet for anything important
        /// </summary>
        /// <param name="Memory">The string of data to remember</param>
        /// <param name="LifeTime">The lifetime of the memory. If long-term memory packet, specify '-1' for infinite</param>
        /// <param name="Priority">The priority of the memory. This is used for organizing in the entity's memory data file, and shouldn't matter much for short term memory.</param>
        public MemoryPacket(string Key, string Memory, int LifeTime, int Priority)
        {
            key = Key;
            memory = Memory;
            lifeTime = LifeTime;
            priority = Priority;
        }

        public string SaveData()
        {
            return "\"" + priority.ToString() + "\" \"" + key + "\" \"" + memory + "\" \"" + lifeTime + "\""; //Lifetime data is always saved just in case a character needs to forget something after a specified time
        }

        public override bool Equals(object obj)
        {
            MemoryPacket packet = (MemoryPacket)obj;

            if (packet == null)
                return false;

            return packet.key == key;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ObjectMemory
    {
        private List<MemoryPacket> longTerm = new List<MemoryPacket>(); //Long term memory is used for storing values like player name, and is primarily used by character entities
        private List<MemoryPacket> shortTerm = new List<MemoryPacket>(); //Short term memory is useful for storing values like last seen enemy location, and is primarily used by monster entities

        public ObjectMemory() { }

        public void Update(GameTime gt)
        {
            for (int i = 0; i < longTerm.Count; i++)
            {
                if (longTerm[i].LifeTime != -1)
                {
                    longTerm[i].LifeTime -= gt.ElapsedGameTime.Milliseconds;

                    if (longTerm[i].LifeTime <= 0)
                       longTerm.RemoveAt(i);
                }
            }

            for (int i = 0; i < shortTerm.Count; i++)
            {
                shortTerm[i].LifeTime -= gt.ElapsedGameTime.Milliseconds;

                if (shortTerm[i].LifeTime <= 0)
                    shortTerm.RemoveAt(i);
            }
        }

        // [Methods]

        // Add to list
        public void SHORT_Add(MemoryPacket packet)
        {
            if (!shortTerm.Contains(packet))
                shortTerm.Insert(0, packet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="lifeTime">This is in milliseconds</param>
        /// <param name="priority"></param>
        public void SHORT_Add(string key, string memory, int lifeTime)
        {
            MemoryPacket packet = new MemoryPacket(key, memory, lifeTime, 1);

            if (!shortTerm.Contains(packet))
                shortTerm.Insert(0, packet);
        }

        public void LONG_Add(MemoryPacket packet)
        {
            if (!longTerm.Contains(packet))
                longTerm.Add(packet);

            longTerm.Sort((m1, m2) => m1.Priority.CompareTo(m2.Priority));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="lifeTime">This is in milliseconds</param>
        /// <param name="priority"></param>
        public void LONG_Add(string key, string memory, int lifeTime, int priority)
        {
            MemoryPacket packet = new MemoryPacket(key, memory, lifeTime, priority);

            if (!longTerm.Contains(packet))
                LONG_Add(packet);
        }

        // Get from list
        public string SHORT_FetchMemory(string keyword)
        {
            MemoryPacket packet = SHORT_FetchMemoryPacket(keyword);

            if (packet != null)
                return packet.Memory;
            else
                return string.Empty;
        }
        public MemoryPacket SHORT_FetchMemoryPacket(string keyword)
        {
            return shortTerm.FirstOrDefault(m => m.Key.ToUpper().StartsWith(keyword.ToUpper()));
        }

        public string LONG_FetchMemory(string keyword)
        {
            MemoryPacket packet = LONG_FetchMemoryPacket(keyword);

            if (packet != null)
                return packet.Memory;
            else
                return string.Empty;
        }
        public MemoryPacket LONG_FetchMemoryPacket(string keyword)
        {
            return longTerm.FirstOrDefault(m => m.Key.ToUpper().StartsWith(keyword.ToUpper()));
        }

        public bool SHORT_ContainsMemory(string keyword)
        {
            bool hasMemory = false;
            shortTerm.FirstOrDefault(s => hasMemory = s.Memory.ToUpper().StartsWith(keyword));
            return hasMemory;
        }
        public bool LONG_ContainsMemory(string keyword)
        {
            bool hasMemory = false;
            longTerm.FirstOrDefault(s => hasMemory = s.Memory.ToUpper().StartsWith(keyword));
            return hasMemory;
        }

        // Remove from longterm list
        public void LONG_Remove(MemoryPacket packet)
        {
            longTerm.Remove(packet);
        }
        public void LONG_Remove(string keyword)
        {
            longTerm.Remove(LONG_FetchMemoryPacket(keyword));
        }
        public void LONG_RemoveAll(string keyword)
        {
            for (int i = 0; i < longTerm.Count; i++)
            {
                if (longTerm[i].Memory.ToUpper().StartsWith(keyword.ToUpper()))
                    LONG_Remove(longTerm[i]);
            }
        }

        // Save longterm list
        public StringBuilder SaveData()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < longTerm.Count; i++)
                builder.AppendLine(longTerm[i].SaveData());

            return builder;
        }
        public void LoadData(List<string> data)
        {
            longTerm.Clear(); //Only called on save loading, so clear all previous data!

            for (int i = 0; i < data.Count; i++)
            {
                try
                {
                    int priority = 1, lifeTime = -1;

                    int.TryParse(data[i].FromWithin('"', 1), out priority);
                    int.TryParse(data[i].FromWithin('"', 4), out lifeTime);

                    LONG_Add(data[i].FromWithin('"', 2), data[i].FromWithin('"', 3), lifeTime, priority);
                }
                catch
                {
                    Logger.AppendLine("Error adding memory! The following line has not been added: " + data[i]);
                }
            }
        }
    }
}
