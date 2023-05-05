using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.CombatEngine.AOE.Types;

namespace Pilgrimage_Of_Embers.CombatEngine.AOE
{
    public static class AOEDatabase
    {
        private static List<BaseAOE> areaOfEffects = new List<BaseAOE>();
        public static List<BaseAOE> AreaOfEffects { get { return areaOfEffects; } }

        public static void Load(ContentManager cm)
        {
            areaOfEffects.Add(new TestAOE(1, "ExplosionTest"));

            LoadAOEContent(cm);
        }
        private static void LoadAOEContent(ContentManager cm)
        {
            for (int i = 0; i < areaOfEffects.Count; i++)
            {
                areaOfEffects[i].Load(cm);
            }
        }
    }
}
