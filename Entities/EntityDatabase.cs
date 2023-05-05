using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.Entities.NPC;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.Entities.Factions;
using Pilgrimage_Of_Embers.Entities.AI.Agents.Type;
using Pilgrimage_Of_Embers.Entities.Types.NPE.NPC;

namespace Pilgrimage_Of_Embers.Entities.Types
{
    public class EntityDatabase
    {
        private static List<NonPlayerEntity> monsters = new List<NonPlayerEntity>();
        public static List<NonPlayerEntity> Monsters { get { return monsters; } }

        private static List<CharacterEntity> characters = new List<CharacterEntity>();
        public static List<CharacterEntity> Characters { get { return characters; } }

        //private static List<BaseBoss> BossesInDatabase = new List<BaseBoss>();
        //public static List<BaseBoss> Bosses { get { return BossesInDatabase; } }

        public static void LoadEntities(ContentManager cm)
        {
            LoadMonsters(cm);
            LoadCharacters(cm);
            LoadBosses(cm);
        }

        private static void LoadMonsters(ContentManager cm)
        {
            monsters.Add(new NonPlayerEntity(1, "Rabbit", new AnimationState(cm.Load<Texture2D>("Entities/Creatures/rabbitTemplateAnimated"), "MainContent/States/rabbit.state"),
                         new Skillset("MainContent/States/rabbit.state"), new ObjectAttributes(), new EntityLoot(null, 5, -1), new EntityStorage("MainContent/States/rabbit.state"),
                         FactionDatabase.GetFaction(1), new ObjectSenses("MainContent/States/rabbit.state"), new EntityKin("WoodlandCreature", "Rabbit"), 16f, -8, 5, 30, 35, new VegetableAgent()));
        }
        private static void LoadCharacters(ContentManager cm)
        {
            characters.Add(new CharacterEntity(1, "Collier the Fuel Merchant", new AnimationState(cm.Load<Texture2D>("Entities/Player/TestChar/testChar3"), new Point(64, 64), 65, 4),
                           new Skillset(99, 19, 25, 23, 16, 12, 14, 8, 18, 21, 10, 6), new ObjectAttributes(null, true),
                           new EntityLoot(new List<ItemDrop>() { new ItemDrop(1, 5, 40, 50) }, 100, -1),
                           new EntityStorage(new List<Point>() { new Point(50, 1), new Point(2, 15), new Point(100, 10) }),
                           FactionDatabase.GetFaction(1),
                           new ObjectSenses(500f, 500f, 65f, 800f, .0045f, .075f, .1f),
                           new EntityKin("Human"), 20f, -40f, 5, 112, 70, new VegetableAgent(),
                           new Merchant(new List<Point>() { new Point(7001, 1), new Point(7002, 1), new Point(7003, 1), new Point(2, 10) },
                                        new List<Point>() { },
                                        new List<int>() { 600, 601, 650, 602, 700 },
                                        new List<Point>() { new Point(700, 15),new Point(6000, 10), new Point(6001, 7), new Point(6002, 5), new Point(6003, 4), new Point(6007, 2), new Point(6008, 5), new Point(50, 1), new Point(51, 1), new Point(52, 1), new Point(53, 1), new Point(54, 1)  },
                                        .85f, .7f, 1.15f),
                           "MainMenu", new Point(14, 12)));
        }
        private static void LoadBosses(ContentManager cm)
        {

        }

        private CharacterEntity CharacterFromID(int id)
        {
            CharacterEntity entity = null;

            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].ID == id)
                    entity = characters[i];
            }

            return entity;
        }

        public static StringBuilder Output()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("------------------------------------------------------------");
            builder.AppendLine("    Entities (Total: " + (monsters.Count + characters.Count) + ")");
            builder.AppendLine("------------------------------------------------------------");

            builder.AppendLine();
            builder.AppendLine("Monsters:");

            monsters.OrderBy(x => x.ID);

            for (int i = 0; i < monsters.Count; i++)
            {
                builder.AppendLine(monsters[i].ID + " - " + monsters[i].Name + " [Health: " + monsters[i].Skills.health.MaxHP + ", Faction ID: " + monsters[i].Faction.ID + "]");
            }

            builder.AppendLine();
            builder.AppendLine("Characters:");

            characters.OrderBy(x => x.ID);

            for (int i = 0; i < characters.Count; i++)
            {
                builder.AppendLine(characters[i].ID + " - " + characters[i].Name + " [Health: " + characters[i].Skills.health.MaxHP +
                                   ", Faction ID: " + characters[i].Faction.ID + ", Merchant: " + characters[i].MERCHANT_IsMerchant() +
                                   ", Starting Map/Tile: " + characters[i].MapLocation + ", x" + characters[i].StartTile.X + " y" + characters[i].StartTile.Y + "]");
            }

            return builder;
        }
    }
}
