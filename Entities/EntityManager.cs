using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Entities.Types;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Entities.Types.NPE;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Debugging;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.Entities.Types.NPE.NPC;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.TileEngine.Objects.Colliders;
using Pilgrimage_Of_Embers.ParticleEngine;
using Pilgrimage_Of_Embers.TileEngine.Map.Editor;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.Entities
{
    public class EntityManager : GameObject
    {
        List<NonPlayerEntity> monsters = new List<NonPlayerEntity>();
        public void SetMonsters(List<BaseEntity> monsters) { this.monsters = monsters.Cast<NonPlayerEntity>().ToList(); AssignEntities(); }

        List<CharacterEntity> mapCharacters = new List<CharacterEntity>();
        private new PlayerEntity player;
        private BaseEntity playerControlled;
        public BaseEntity PlayerControlled { get { return playerControlled; } }
        //List bosses blah

        List<BaseEntity> totalEntities = new List<BaseEntity>();
        List<BaseEntity> totalEntitiesNoPlayer = new List<BaseEntity>();

        public List<NonPlayerEntity> Monsters { get { return monsters; } }
        public List<CharacterEntity> MapCharacters { get { return mapCharacters; } }
        public List<BaseEntity> TotalEntities { get { return totalEntities; } }
        public List<BaseEntity> TotalEntitiesNoPlayer { get { return totalEntitiesNoPlayer; } }

        private WorldManager worldManager;

        private ContentManager mainContent, mapContent;

        private Controls controls = new Controls();

        static int controlledByPlayer = 0;

        public EntityManager(ContentManager mainContent, ContentManager mapContent) : base(-6, 1, 0)
        {
            this.mainContent = mainContent;
            this.mapContent = mapContent;

            position = new Vector2(96, -64);
            isUseTileLocation = false;
        }
        public void SetReferences(Camera Camera, TileMap Map, DebugManager Debug, WorldManager WorldManager, ScreenManager Screens, CultureManager Culture, WeatherManager weather)
        {
            camera = Camera;
            map = Map;
            debug = Debug;
            worldManager = WorldManager;
            screens = Screens;
            culture = Culture;
            this.weather = weather;
        }

        public void AddMonster(int id, int objectID, int currentFloor, Point startTile, float lookDirection, bool isSaveable)
        {
            for (int i = 0; i < EntityDatabase.Monsters.Count; i++)
            {
                if (EntityDatabase.Monsters[i].ID == id)
                {
                    monsters.Add((NonPlayerEntity)EntityDatabase.Monsters[i].DeepCopy(map, camera));
                    monsters.Last().SetValues(objectID, currentFloor, startTile, lookDirection, isSaveable);
                    monsters.Last().SetReferences(camera, map, debug, worldManager, screens, this, culture, player, weather);

                    if (isSaveable == false)
                    {
                        monsters.Last().SetMapInfo(pathMap);
                        monsters.Last().totalColliders = colliders;
                        monsters.Last().Load(mainContent);
                    }
                }
            }

            AssignEntities();
        }
        public void AddMonster(NonPlayerEntity monster)
        {
            monsters.Add(monster);
            monster.Load(mainContent);
            monster.SetReferences(camera, map, debug, worldManager, screens, this, culture, player, weather);

            AssignEntities();
        }
        public void RemoveMonster(NonPlayerEntity monster)
        {
            monsters.Remove(monster);

            AssignEntities();
        }

        public NonPlayerEntity ParseMonster(string line, Action<string, MapIssue.MessageType> issue)
        {
            NonPlayerEntity npe = null;

            if (line.ToUpper().StartsWith("MONSTER"))
            {
                string[] words = line.Split(' ');

                int id = -1, objectID = -1, currentFloor = 1;
                Point startTile = Point.Zero;
                float lookDirection = 0;

                try { id = int.Parse(words[1]); }
                catch (Exception e) { issue.Invoke(e.Message + " (EntityManager)", MapIssue.MessageType.Error); }

                try { objectID = int.Parse(words[2]); }
                catch (Exception e) { issue.Invoke(e.Message + " (EntityManager)", MapIssue.MessageType.Error); }

                try { currentFloor = int.Parse(words[3]); }
                catch (Exception e) { issue.Invoke(e.Message + " (EntityManager)", MapIssue.MessageType.Error); }

                try { startTile = new Point().Parse(words[4], words[5]); }
                catch (Exception e) { issue.Invoke(e.Message + " (EntityManager)", MapIssue.MessageType.Error); }

                try { lookDirection = float.Parse(words[6]); }
                catch (Exception e) { issue.Invoke(e.Message + " (EntityManager)", MapIssue.MessageType.Error); }

                for (int i = 0; i < EntityDatabase.Monsters.Count; i++)
                {
                    if (EntityDatabase.Monsters[i].ID == id)
                    {
                        try
                        {
                            npe = (NonPlayerEntity)EntityDatabase.Monsters[i].DeepCopy(map, camera);
                            npe.SetValues(objectID, currentFloor, startTile, lookDirection, true);
                            npe.SetReferences(camera, map, debug, worldManager, screens, this, culture, player, weather);
                        }
                        catch (Exception e)
                        {
                            issue.Invoke(e.Message, MapIssue.MessageType.Error);
                        }

                        break;
                    }
                }

                if (npe == null && id != -1) //If the entity is still null after searching, indicate a warning!
                    issue.Invoke("No ID in EntityDatabase matches " + id.ToString() + "!", MapIssue.MessageType.Warning);
            }

            return npe;
        }

        public void AddNPC()
        {

        }
        public void AddBoss()
        {

        }

        public BaseEntity SelectEntity(string mapEntityID)
        {
            for (int i = 0; i < totalEntities.Count; i++)
            {
                if (totalEntities[i] is CharacterEntity)
                {
                    if (totalEntities[i].Name.ToUpper() == mapEntityID.ToUpper())
                        return totalEntities[i];
                }
                else if (totalEntities[i] is NonPlayerEntity)
                {
                    if (totalEntities[i].MapEntityID.ToUpper() == mapEntityID.ToUpper())
                        return totalEntities[i];
                }
            }

            return null;
        }
        private void SelectNPC(string characterName) { }
        private void SelectBoss(string bossName) { }
        public PlayerEntity SelectPlayer { get { return player; } }

        public void Load(ContentManager cm, TileMap tm)
        {
            for (int i = 0; i < monsters.Count; i++)
                monsters[i].Load(cm);

            for (int i = 0; i < mapCharacters.Count; i++)
            {
                mapCharacters[i].Load(cm);
                mapCharacters[i].SetReferences(player);
            }
        }
        public void LoadPlayer(ContentManager cm)
        {
            player = new PlayerEntity("Player", new AnimationState(cm.Load<Texture2D>("Entities/Creatures/rabbitTemplateAnimated"), "MainContent/States/rabbit.state"), new Skillset());//Player/TestChar/testChar3
            player.SetReferences(camera, map, debug, worldManager, screens, this, culture, player, weather);
            player.Load(cm);

            playerControlled = player;
        }
        private void AssignEntities() //Experimental code.
        {
            RefreshEntities();

            for (int i = 0; i < totalEntities.Count; i++) //Assign entity map id.
                totalEntities[i].AssignMapID(i);

            for (int i = 0; i < totalEntities.Count; i++) //Give reference of total map entity list to each entity for targeting.
                totalEntities[i].AddEntities(TotalEntities);
        }
        public void RefreshEntities()
        {
            totalEntities.Clear();
            
            totalEntities.Add(player);

            for (int i = 0; i < monsters.Count; i++)
                totalEntities.Add(monsters[i]); //Do you get the reference? Yes, you do.
            for (int i = 0; i < mapCharacters.Count; i++)
                totalEntities.Add(mapCharacters[i]); //Do you get the reference? Yes, you do.

            for (int i = 0; i < mapCharacters.Count; i++)
                totalEntitiesNoPlayer.Add(mapCharacters[i]);
            for (int i = 0; i < monsters.Count; i++)
                totalEntitiesNoPlayer.Add(monsters[i]);
        }

        private List<BaseCollider> colliders;
        private PathfindMap pathMap;

        public void SetMapData(List<BaseCollider> colliders, PathfindMap pathMap) //Add A* map here
        {
            this.colliders = colliders;
            this.pathMap = pathMap;

            RefreshMapData();
        }
        private void RefreshMapData()
        {
            for (int i = 0; i < totalEntities.Count; i++)
            {
                totalEntities[i].SetMapInfo(pathMap);
                totalEntities[i].totalColliders = colliders;
            }
        }

        public new void Update(GameTime gt)
        {
            controlledByPlayer = 0;

            for (int i = 0; i < totalEntities.Count; i++)
            {
                if (totalEntities[i].IsPlayerControlled == true)
                {
                    controlledByPlayer++;
                    playerControlled = totalEntities[i];
                }

                totalEntities[i].SetControlledEntity(playerControlled);

                totalEntities[i].Update(gt);

                //Entity-to-entity collision code
                for (int j = i + 1; j < totalEntities.Count; j++)
                {
                    if (totalEntities[i].EntityCircle.Intersects(totalEntities[j].EntityCircle))
                        totalEntities[j].PushEntity(totalEntities[i]);
                }

                if (totalEntities[i].IsDead && totalEntities[i].IsSavable == false)
                {
                    if (totalEntities[i] is NonPlayerEntity)
                        monsters.Remove((NonPlayerEntity)totalEntities[i]);

                    totalEntities.Remove(totalEntities[i]);
                }
            }

            //Ensure the proper amount of controlled entities is enforced (always one).
            if (controlledByPlayer != 1)
            {
                ClearControlledEntities();
                SetControlledEntity(player);

                debug.OutputConsole("ERROR: entities locked on by camera is invalid!: " + controlledByPlayer.ToString() + " total.");
                debug.OutputConsole("Attempted to fix. If this continues, look into possible reasons.");
            }

            controls.UpdateCurrent();

            controls.UpdateLast();
        }

        public Texture2D Pixel { private get; set; }
        public new void Draw(SpriteBatch sb)
        {
            sb.DrawBoxBordered(Pixel, new Rectangle((int)position.X - 3, (int)position.Y - 3, 6, 6), Color.Red, Color.White, 1f);

            player.Draw(sb);

            for (int i = 0; i < monsters.Count; i++)
                monsters[i].Draw(sb);
            for (int i = 0; i < mapCharacters.Count; i++)
                mapCharacters[i].Draw(sb);
        }
        public new void DrawAboveLight(SpriteBatch sb)
        {
            player.DrawUI(sb);

            for (int i = 0; i < monsters.Count; i++)
                monsters[i].DrawUI(sb);
            for (int i = 0; i < mapCharacters.Count; i++)
                mapCharacters[i].DrawUI(sb);
        }

        // [Encapsulate] NPE
        public StringBuilder MONSTER_SaveData(string tag)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(tag);

            for (int i = 0; i < monsters.Count; i++)
            {
                string line = monsters[i].SaveData().ToString();

                if (!string.IsNullOrEmpty(line))
                    builder.AppendLine(line);
            }

            builder.AppendLine(tag.Replace("[", "[/"));

            return builder;
        }
        public void MONSTER_LoadData(List<string> data)
        {
            for (int i = 0; i < monsters.Count; i++)
                monsters[i].LoadData(data, player.SoulgateRestCount);
        }

        // [Encapsulate] Character Entity
        public void CHARACTERS_AddToMap(string mapName)
        {
            for (int i = 0; i < EntityDatabase.Characters.Count; i++)
            {
                if (EntityDatabase.Characters[i].MapLocation.ToUpper().Equals(mapName.ToUpper()))
                {
                    mapCharacters.Add(EntityDatabase.Characters[i]);
                    mapCharacters.LastOrDefault().SetReferences(camera, map, debug, worldManager, screens, this, culture, player, weather);
                }
            }
        }
        public void CHARACTER_SwitchMaps(int id, int currentFloor, string mapName, Point tileLocation, float lookDirection)
        {
            for (int i = 0; i < EntityDatabase.Characters.Count; i++)
            {
                if (EntityDatabase.Characters[i].ID == id)
                {
                    EntityDatabase.Characters[i].MapLocation = mapName;
                    EntityDatabase.Characters[i].SetValues(-1, currentFloor, tileLocation, lookDirection, true);
                }
            }
        }
        public void CHARACTER_SetTile(int id, Point tileLocation)
        {
            for (int i = 0; i < EntityDatabase.Characters.Count; i++)
            {
                if (EntityDatabase.Characters[i].ID == id)
                {
                    EntityDatabase.Characters[i].SetTile(tileLocation);
                }
            }
        }
        public void CHARACTER_SaveAll(string playerName)
        {
            string directory = "Saves/" + playerName + "/Characters/";
            for (int i = 0; i < EntityDatabase.Characters.Count; i++)
            {
                EntityDatabase.Characters[i].WriteCharacterData(directory);
            }
        }
        public void CHARACTER_LoadAll(string playerName)
        {
            string directory = "Saves/" + playerName + "/Characters/";
            for (int i = 0; i < EntityDatabase.Characters.Count; i++)
            {
                EntityDatabase.Characters[i].ReadCharacterData(directory);
            }
        }
        public bool CHARACTER_IsClickingUI()
        {
            for (int i = 0; i < mapCharacters.Count; i++)
            {
                if (mapCharacters[i].IsClickingUI() == true)
                    return true;
            }

            return false;
        }
        public bool CHARACTER_IsUIOpen()
        {
            for (int i = 0; i < mapCharacters.Count; i++)
            {
                if (mapCharacters[i].IsUIOpen() == true)
                    return true;
            }

            return false;
        }
        public void CHARACTER_ForceCloseUIs()
        {
            for (int i = 0; i < mapCharacters.Count; i++)
                mapCharacters[i].ForceCloseUIs();
        }

        public void ReviveAll()
        {
            for (int i = 0; i < totalEntities.Count; i++)
                totalEntities[i].Revive();
        }
        public void ReviveMonsters(bool isSetTile)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].Revive();

                if (isSetTile == true)
                    monsters[i].SetTile(monsters[i].StartTile, false);
            }
        }

        // Switch Controls
        public void SetControlledEntity(BaseEntity entity)
        {
            ClearControlledEntities();

            if (entity != null)
            {
                entity.SetPlayerControlled(true);
            }
        }
        public void ClearControlledEntities()
        {
            for (int i = 0; i < totalEntities.Count; i++)
                totalEntities[i].SetPlayerControlled(false);
        }

        public void ClearAll()
        {
            totalEntities.Clear();
            monsters.Clear();
            mapCharacters.Clear();
        }


        //GameObject overriding

        public override void SetDisplayVariables()
        {
            displayVariables.AppendLine("int _EntityCount_ (" + totalEntities.Count + ")");
            displayVariables.AppendLine("int _MonsterCount_ (" + monsters.Count + ")");
            displayVariables.AppendLine("int _MonsterIDCount_(int id) [Format: MonsterIDCount(id)]");
            displayVariables.AppendLine("int _CharacterCount_ (" + mapCharacters.Count + ")");

            displayVariables.AppendLine("void SetNPCTile(int id, Point tileLocation)");
            displayVariables.AppendLine("void SetNPCMap(int id, int currentFloor, string mapName, Point tileLocation, float lookDirection)");
            displayVariables.AppendLine("void SpawnMonster(int id, int gameObjectID, int currentFloor, Point tileLocation, float lookDirection");
            displayVariables.AppendLine("void ReviveMonsters(bool isSetTile)");
            displayVariables.AppendLine("void EditMonsterByID(int id, string \"Edit\")");
            displayVariables.AppendLine("void EditMonsterObjectID(int id, string \"Edit\")");
            displayVariables.AppendLine("");

            base.SetDisplayVariables();
        }
        public override void ParseEdit(string line, string[] words)
        {
            try
            {
                if (line.ToUpper().StartsWith("SPAWNMONSTER"))
                    AddMonster(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), new Point().Parse(words[4], words[5]), float.Parse(words[6]), false);
                if (line.ToUpper().StartsWith("REVIVEMONSTERS"))
                    ReviveMonsters(bool.Parse(words[2]));

                if (line.ToUpper().StartsWith("EDITMONSTERBYID"))
                {
                    int id = int.Parse(words[1]);
                    string edit = line.FirstLastWithin("\"");
                    string[] editWords = edit.Split(' ');

                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (monsters[i].ID == id)
                            monsters[i].ParseEdit(edit, editWords);
                    }
                }

                if (line.ToUpper().StartsWith("EDITMONSTERBYOBJECTID"))
                {
                    int id = int.Parse(words[1]);
                    string edit = line.FirstLastWithin("\"");
                    string[] editWords = edit.Split(' ');

                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (((GameObject)monsters[i]).ID == id)
                            monsters[i].ParseEdit(edit, editWords);
                    }
                }

            }
            catch
            {
            }

            base.ParseEdit(line, words);
        }
        public override string RetrieveVariable(string name)
        {
            if (name.ToUpper().StartsWith("ENTITYCOUNT"))
                return totalEntities.Count.ToString();
            if (name.ToUpper().StartsWith("MONSTERCOUNT"))
                return monsters.Count.ToString();
            if (name.ToUpper().StartsWith("CHARACTERCOUNT"))
                return mapCharacters.Count.ToString();

            if (name.ToUpper().StartsWith("MONSTERIDCOUNT("))
            {
                int id = int.Parse(name.FromWithin('(', ')', 1));
                return monsters.Count(n => n.ID == id).ToString();
            }

            return base.RetrieveVariable(name);
        }

        public override string MapOutputLine()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].IsSavable == true) //Monster was added by map creator, so don't output to .map files.
                {
                    builder.AppendLine(monsters[i].MapOutputLine());
                }
            }

            return builder.ToString();
        }
        public override void InitializeSuggestLine()
        {
            objectType = AutoSuggestionObject.ObjectType.Objects;
            //suggestLines.Add("WindX float WindDirection, Vector2 SpeedRangeX, float WindPixelWidth, int WindTimeX, int LengthMultiplierX, float SpeedMultiplierX, float PixelMultiplierX");
        }
    }
}
