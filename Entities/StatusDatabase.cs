using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pilgrimage_Of_Embers.Entities.Status_Effects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Entities;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Entities
{
    public class StatusDatabase
    {
        static List<BaseStatus> statuses = new List<BaseStatus>();
        public static List<BaseStatus> Statuses { get { return statuses; } }

        private static Debugging.DebugManager debug;
        public static Debugging.DebugManager Debug { set { debug = value; } }

        private const string iconDirectory = "Interface/HUD/Statuses/";

        public static void LoadStatusEffects(ContentManager cm)
        {
            statuses.Add(new BaseStatus(1, cm.Load<Texture2D>("Bonuses/regenerate3"), "Sunlight's Touch", 20000, 1000, false, false, 1, (BaseEntity e, BaseStatus s) => { e.HEALTH_Restore(3); e.VISUAL_AddVisual(1); }, (BaseEntity e, BaseStatus s) => { e.VISUAL_BeginStop(1); }, false, false, true));
            statuses.Add(new BaseStatus(2, cm.Load<Texture2D>("Bonuses/Burning"), "Burning", 20000, 500, true, true, 5, (BaseEntity e, BaseStatus s) =>
            {
                s.EffectTimeMultiplier = e.Skills.wisdom.NegativePotionMultiplier;
                e.HEALTH_Damage(1, e.MapEntityID);
            }, null, false, false, true));
            statuses.Add(new BaseStatus(3, cm.Load<Texture2D>("Bonuses/poison"), "Poisoned", 60000, 2000, true, true, 3, (BaseEntity e, BaseStatus s) => { e.HEALTH_Damage(2, e.MapEntityID, Color.Lerp(Color.Green, Color.White, .25f)); }, null, false, false, true));

            //Force, Guardian, Stilling (accuracy), and Celerity (speed)

            //Tonic of Force
            statuses.Add(new BaseStatus(10, cm.Load<Texture2D>(iconDirectory + "physUp"), "Force", 60000, 1, true, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.Skills.strength.PhysicalDamageMultiplier += .25f;
            }, null, false, false, true));

            //Spearguard's Stimulant
            statuses.Add(new BaseStatus(11, cm.Load<Texture2D>(iconDirectory + "physDefUp"), "Guardian", 60000, 1, true, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.ATTRIBUTE_AdjustMultiplier(BaseEntity.ATTRIBUTE_SkillsPhysicalDefenseMultiplier, .25f);
            }, null, false, false, true));

            //Stilling Concoction
            statuses.Add(new BaseStatus(12, cm.Load<Texture2D>(iconDirectory + "accUp"), "Stilling", 120000, 1, true, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.Skills.archery.AccuracyMultiplier -= .25f;
            }, null, false, false, true));

            //Drop of Celerity
            statuses.Add(new BaseStatus(13, cm.Load<Texture2D>(iconDirectory + "speedUp"), "Celerity", 120000, 1, true, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.Skills.agility.MovementMultiplier += .20f;
                e.Skills.agility.AttackMultiplier += .10f;
            }, null, false, false, true));

            //Wanderer's Bandstrot
            statuses.Add(new BaseStatus(50, cm.Load<Texture2D>("Bonuses/regenerate3"), "Defense Relative Weight", 5000, 0, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                float increase = e.EQUIPMENT_ArmorWeight() / 750;
                e.ATTRIBUTE_AdjustMultiplier(BaseEntity.ATTRIBUTE_SkillsPhysicalDefenseMultiplier, increase);
                e.ATTRIBUTE_AdjustMultiplier(BaseEntity.ATTRIBUTE_SkillsProjectileDefenseMultiplier, increase);
                e.ATTRIBUTE_AdjustMultiplier(BaseEntity.ATTRIBUTE_SkillsMagicalDefenseMultiplier, increase);
            }, null, false, true, false));
            //Tongues of Foreigners
            statuses.Add(new BaseStatus(51, cm.Load<Texture2D>("Bonuses/regenerate3"), "Uncommon Languages", 5000, 0, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
            }, null, false, true, false));
            //Chain of Easing
            statuses.Add(new BaseStatus(52, cm.Load<Texture2D>("Bonuses/regenerate3"), "Eased Burden", 5000, 0, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.Skills.agility.ArmorWeightMultiplier -= .3f;
            }, null, false, true, false));

            //Ring of Renewal
            statuses.Add(new BaseStatus(99, cm.Load<Texture2D>("Bonuses/regenerate3"), "Regenerative Stamina", 1000, 100, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.STAMINA_Restore(1f);
            }, null, false, true, false));

            //Ring of the Flameful
            statuses.Add(new BaseStatus(100, cm.Load<Texture2D>("Bonuses/regenerate3"), "Heightened Embers", 1000, 1, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.SKILL_EmberMultiplier += .2f;
            }, null, false, true, false));

            //Spellcaster's Patron
            statuses.Add(new BaseStatus(101, cm.Load<Texture2D>("Bonuses/regenerate3"), "Regenerative Magic", 1000, 1000, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.MAGIC_Restore(1);
            }, null, false, true, false));

            //Band of Nature

            //Wooden Ring

            //Ring of Hastening
            statuses.Add(new BaseStatus(104, cm.Load<Texture2D>("Bonuses/regenerate3"), "Heightened Flurry", 1000, 1, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.Skills.agility.AttackMultiplier += .15f;
            }, null, false, true, false));

            //Farsighting Ring
            statuses.Add(new BaseStatus(105, cm.Load<Texture2D>("Bonuses/regenerate3"), "Heightened Sight", 1000, 1, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                
            }, null, false, true, false));

            //Resistance Armyll
            statuses.Add(new BaseStatus(106, cm.Load<Texture2D>("Bonuses/regenerate3"), "Heightened Resistance", 1000, 1, false, false, 1, (BaseEntity e, BaseStatus s) =>
            {
                e.Skills.wisdom.NegativePotionMultiplier += .15f;
                e.ATTRIBUTE_AdjustMultiplier(BaseEntity.ATTRIBUTE_SkillsMagicalDefenseMultiplier, .25f);
            }, null, false, true, false));

            Load(cm);
            CheckConflictingIDs();
        }

        private static void Load(ContentManager cm)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                statuses[i].Load(cm);
            }
        }
        private static void CheckConflictingIDs()
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                for (int j = 0; j < statuses.Count; j++)
                {
                    if (j != i) //Excluding checking self
                    {
                        if (statuses[i].ID == statuses[j].ID)
                        {
                            Logger.AppendLine("WARNING: " + statuses[i].Name + " and " + statuses[j].Name + " both have the same ID value of " + statuses[i].ID);
                        }
                    }
                }
            }
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Status Effects (Total: " + statuses.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            statuses.OrderBy(x => x.ID);

            for (int i = 0; i < statuses.Count; i++)
            {
                builder.AppendLine(statuses[i].ID + " - " + statuses[i].Name + " [Stack Type: " + statuses[i].stackType.ToString() + ", " +
                                   statuses[i].LifeTime + "ms, Effect Time Intensity: " + statuses[i].EffectTimeIntensity + "ms, Is Infinite: " +
                                   statuses[i].IsInfinite + ", Is Invisible: " + statuses[i].IsInvisible + ", Is Cured By Rest: " + statuses[i].IsRestCure + "]");
            }

            return builder;
        }
    }
}
