using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Entities
{
    public class ObjectAttributes
    {
        private Dictionary<string, float> nameMultiplier;

        public ObjectAttributes(Dictionary<string, float> NameMultiplier = null, bool isAddDefaults = true)
        {
            if (NameMultiplier == null)
                nameMultiplier = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            else
                nameMultiplier = new Dictionary<string, float>(NameMultiplier, StringComparer.OrdinalIgnoreCase);

            if (isAddDefaults == true)
                SetDefaults();
        }
        private void SetDefaults()
        {
            //Default attribute stuff goes here... E.G., "FireWeakness" = 1.25f.
        }

        public void SetMultiplier(string name, float value)
        {
            if (nameMultiplier.ContainsKey(name))
                nameMultiplier[name] = value;
            else
                nameMultiplier.Add(name, value);
        }
        public void AdjustMultiplier(string name, float value)
        {
            if (nameMultiplier.ContainsKey(name))
                nameMultiplier[name] += value;
            else
                nameMultiplier.Add(name, value);
        }
        public float GetMultiplier(string name, float nullDefault = 1f)
        {
            if (nameMultiplier.ContainsKey(name))
                return nameMultiplier[name];

            return nullDefault;
        }

        public ObjectAttributes Copy()
        {
            return new ObjectAttributes(this.nameMultiplier, false);
        }
    }
}
