using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.CombatEngine.Projectiles.Types.Physical;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.CombatEngine.Projectiles.Types.Physical.Arrows;

namespace Pilgrimage_Of_Embers.CombatEngine.Projectiles
{
    public static class ProjectileDatabase
    {
        private static List<BaseProjectile> projectiles = new List<BaseProjectile>();
        public static List<BaseProjectile> Projectiles { get { return projectiles; } }

        public static BaseProjectile Projectile(int id)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].ID == id)
                    return projectiles[i];
            }

            return null;
        }

        private const string ProjDir = "Combat/Projectiles/Physical/";
        public static void LoadProjectiles(ContentManager cm)
        {
            projectiles.Add(new PhysicalProjectile(1, "Steel Knife", 48f, cm.Load<Texture2D>(ProjDir + "Knives/SteelKnife"), 1200f, 10, 700));

            //Arrows 0-100
            projectiles.Add(new PhysicalProjectile(1, "Spearstone Arrow", 85f, cm.Load<Texture2D>(ProjDir + "Arrows/spearstoneArrow"), 1300f, 7, 3000));
            projectiles.Add(new PhysicalProjectile(2, "Iron Arrow", 85f, cm.Load<Texture2D>(ProjDir + "Arrows/ironArrow"), 1500f, 12, 3002));
            projectiles.Add(new PhysicalProjectile(3, "Glass Arrow", 85f, cm.Load<Texture2D>(ProjDir + "Arrows/glassArrow"), 1600f, 9, 3004));
            projectiles.Add(new StatusProjectile(4, "Poison Arrow", 85f, cm.Load<Texture2D>(ProjDir + "Arrows/poisonArrow"), 1500f, 9, 3006, 3));
            projectiles.Add(new FireArrow(5, "Flaming Arrow", 85f, cm.Load<Texture2D>(ProjDir + "Arrows/flameArrow"), 1500f, 8, 3008));

            //Bolts 100-200
            projectiles.Add(new PhysicalProjectile(101, "Spearstone Bolt", 60f, cm.Load<Texture2D>(ProjDir + "Bolts/spearstoneBolt"), 1800f, 9, 3001));
            projectiles.Add(new PhysicalProjectile(102, "Iron Bolt", 60f, cm.Load<Texture2D>(ProjDir + "Bolts/ironBolt"), 2000f, 14, 3003));
            projectiles.Add(new PhysicalProjectile(103, "Glass Bolt", 60f, cm.Load<Texture2D>(ProjDir + "Bolts/glassBolt"), 2100f, 11, 3005));
            projectiles.Add(new StatusProjectile(104, "Poison Bolt", 60f, cm.Load<Texture2D>(ProjDir + "Bolts/poisonBolt"), 2000f, 11, 3007, 3));
            projectiles.Add(new FireArrow(105, "Flaming Bolt", 60f, cm.Load<Texture2D>(ProjDir + "Bolts/flameBolt"), 2000f, 11, 3009));
            
            //Thrown physical 200-300

            //Flasks 300-400

            //Spells 500-1000


            LoadProjectileContent(cm);
            CheckConflictingIDs();
        }
        private static void LoadProjectileContent(ContentManager cm)
        {
            for (int i = 0; i < projectiles.Count; i++)
                projectiles[i].Load(cm);
        }
        private static void CheckConflictingIDs()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = i + 1; j < projectiles.Count; j++)
                {
                    if (projectiles[i].ID == projectiles[j].ID)
                    {
                        
                    }
                }
            }
        }

        public static Texture2D GetProjectileTexture(int id)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].ID == id)
                {
                    return projectiles[i].ProjectileTexture;
                }
            }
            return null;
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Projectiles (Total: " + projectiles.Count + ")");
            builder.AppendLine("------------------------------------------------------------");

            projectiles.OrderBy(x => x.ID);

            for (int i = 0; i < projectiles.Count; i++)
            {
                builder.AppendLine(projectiles[i].ID + " - " + projectiles[i].Name);
            }

            return builder;
        }
    }
}
