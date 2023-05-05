using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook.Types;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Pilgrimage_Of_Embers.ScreenEngine.Spellbook
{
    public class SpellDatabase
    {
        private static List<BaseSpell> spells = new List<BaseSpell>();
        public static List<BaseSpell> Spells { get { return spells; } }

        private const string directory = "Spells/";
        public static void LoadSpells(ContentManager cm)
        {
            spells.Add(new BaseSpell(1, "Flame-newt's Whisper", "A flaming fire technique discovered by the old diviner Picarus. Though he possessed no destructive talents, he was taught this spell by a curious creature while pond scrying.", cm.Load<Texture2D>(directory + "Elemental/fireball"), BaseSpell.TabType.Destructive, 20,
                (s, c) =>
                {
                    if (c.CastCount == 1)
                    {
                        c.FireCustom(1, c.CastingPosition, c.MouseDirection - .05f, 0f, 1f, 1f, CombatEngine.BaseCombat.DecreaseProjectile.Spell, 1);
                        c.FireCustom(1, c.CastingPosition, c.MouseDirection, 0f, 1f, 1f, CombatEngine.BaseCombat.DecreaseProjectile.Spell, 2);
                        c.FireCustom(1, c.CastingPosition, c.MouseDirection + .05f, 0f, 1f, 1f, CombatEngine.BaseCombat.DecreaseProjectile.Spell, 3);

                        s.Charges -= 1;
                        c.CastCount++;
                    }
                },
                null, null, null, null, null, null, null, null));
        }
        public static void LoadSpellContent(ContentManager cm)
        {

        }

        public static BaseSpell Spell(int id)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i].ID == id)
                    return spells[i];
            }

            return null;
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Spells (Total: " + spells.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            spells.OrderBy(x => x.ID);

            for (int i = 0; i < spells.Count; i++)
            {
                builder.AppendLine(spells[i].ID + " - " + spells[i].Name);
            }

            return builder;
        }
    }
}
