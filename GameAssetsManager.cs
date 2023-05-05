using System;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Entities;
using Pilgrimage_Of_Embers.ScreenEngine.RumorsNotes;
using Pilgrimage_Of_Embers.CombatEngine.Projectiles;
using Pilgrimage_Of_Embers.CombatEngine.AOE;
using Pilgrimage_Of_Embers.ScreenEngine.Souls;
using Pilgrimage_Of_Embers.ScreenEngine.Spellbook;
using Pilgrimage_Of_Embers.TileEngine.Objects.Soulgates;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.Entities.Factions;
using Pilgrimage_Of_Embers.AudioEngine;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.CombatEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Soulgate.Crafting;
using System.IO;

namespace Pilgrimage_Of_Embers
{
    public class GameAssetsManager
    {
        private Debugging.DebugManager debug;

        private static int loadCount = 0;

        public GameAssetsManager()
        {

        }
        public void SetReferences(Debugging.DebugManager debug) { this.debug = debug; }
        /// <summary>
        /// Make sure this is only called once per session!
        /// </summary>
        /// <param name="cm"></param>
        public void LoadAssets(ContentManager main, ContentManager map)
        {
            //If already loaded, and attempting to do so again ...
            if (loadCount > 1)
                Logger.AppendLine("WARNING: game assets have already been loaded. An invalid method call has been made." + loadCount.ToString() + "[GameAssetsManager.LoadAssets()]");

            //If not loaded ...
            if (loadCount == 0)
            {
                BaseCombat.LoadDebugTextures(main);

                ProjectileDatabase.LoadProjectiles(main); //Load all projectiles

                ItemDatabase.LoadItemData(main, map); //Load all inventory items

                SpellDatabase.LoadSpells(main); //Load all spells
                RumorDatabase.LoadRumors(); //Load all rumors
                SoulsDatabase.LoadSouls(main); //Load all souls

                FactionDatabase.LoadFactions(main); //Load all entity factions
                EntityDatabase.LoadEntities(main); //Load all entities
                StatusDatabase.LoadStatusEffects(main); //Load all statuses (buffs/debuffs)
                VisualDatabase.LoadVisuals(main, map); //Load all visual overlays on entities

                AOEDatabase.Load(main); //Load all area of effects

                MonumentDatabase.LoadSoulgates(main, map); //Load all soulgates
                MusicDatabase.Load(main);
            }
            loadCount++;
        }

        public void OutputCreationGuide(string fileName, StringBuilder extra)
        {
            if (!File.Exists(fileName)) //If the file doesn't exist, create it!
                File.Create(fileName);

            StringBuilder builder = new StringBuilder();

            builder.AppendLine(ItemDatabase.Output().ToString());
            builder.AppendLine(SpellDatabase.Output().ToString());
            builder.AppendLine(SoulsDatabase.Output().ToString());
            builder.AppendLine(RumorDatabase.Output().ToString());

            builder.AppendLine(EntityDatabase.Output().ToString());
            builder.AppendLine(StatusDatabase.Output().ToString());
            builder.AppendLine(VisualDatabase.Output().ToString());

            builder.AppendLine(MonumentDatabase.Output().ToString());
            builder.AppendLine(ProjectileDatabase.Output().ToString());

            builder.AppendLine(extra.ToString());

            File.WriteAllText(fileName, builder.ToString());
        }

        public void LoadMapContent(ContentManager mapContent)
        {
            VisualDatabase.Load(mapContent);
        }
    }
}
