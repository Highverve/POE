using System;
using System.Collections.Generic;
using System.Text;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.ScreenEngine;
using System.IO;
using Pilgrimage_Of_Embers.Debugging;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.SaveTypes
{
    public class DataManager
    {
        private ScreenManager screens;
        private PlayerEntity playerEntity;
        private DebugManager debug;
        private TileMap maps;
        private CultureManager culture;

        private const string baseDirectory = "Saves/";
        private const string playerExt = ".data", mapExt = ".data", worldExt = ".data";

        private const string metaTag = "[META]";

        // Player tag variables (these will be used for saving in the player file)
        private const string playerTag = "[PLAYER]";
        private const string storageTag = "[STORAGE]";
        private const string statisticsTag = "[STATISTICS]";
        private const string soulTag = "[SOULS]";
        private const string rumorsTag = "[RUMORS]";
        private const string spellTag = "[SPELLBOOK]";
        private const string recipeTag = "[RECIPES]";
        private const string stoneholdTag = "[STONEHOLD]";
        private const string smelterTag = "[SMELTING]";
        private const string brewingTag = "[BREWING]";
        private const string imbueTag = "[IMBUE]";

        // Map tag variables
        private const string triggerTag = "[TRIGGERS]";
        private const string containerTag = "[CONTAINERS]";
        private const string floraTag = "[FLORA]";
        private const string entitiesTag = "[ENTITIES]";
        private const string variousTag = "[VARIOUS]";

        // World tag variables
        private const string calendarTag = "[CALENDAR]";
        private const string soulgateTag = "[SOULGATES]";

        private List<string> linesOfPlayerData = new List<string>();
        private List<string> linesOfMapData = new List<string>();
        private List<string> linesOfWorldData = new List<string>();

        Cryption cryption = new Cryption("CSharpAsuna1518", "IDoNTEveNKnOwWhATThIsIS", "29e20cba55a4b5f0b3e8f4cfe03bf975", 1020, "74MA54743B33TL35");
        bool applyCryption = false;

        public void SetReferences(TileMap Maps, ScreenManager screens, PlayerEntity playerEntity, DebugManager debug, CultureManager culture)
        {
            this.maps = Maps;
            this.screens = screens;
            this.playerEntity = playerEntity;
            this.debug = debug;
            this.culture = culture;
        }

        // --- Player Data Management ---

        List<string> playerMetaLines = new List<string>();
        List<string> soulLines = new List<string>();
        List<string> rumorLines = new List<string>();
        List<string> playerLines = new List<string>();
        List<string> statisticsLines = new List<string>();
        List<string> itemLines = new List<string>();
        List<string> spellLines = new List<string>();
        List<string> recipeLines = new List<string>();
        List<string> stoneholdLines = new List<string>();
        List<string> smeltingLines = new List<string>();
        List<string> brewingLines = new List<string>();
        List<string> imbueLines = new List<string>();

        bool isReadingPlayerMeta = false;
        bool isReadingSouls = false;
        bool isReadingRumors = false;
        bool isReadingPlayer = false;
        bool isReadingStats = false;
        bool isReadingItems = false;
        bool isReadingSpells = false;
        bool isReadingRecipes = false;
        bool isReadingStonehold = false;
        bool isReadingSmelting = false;
        bool isReadingBrewing = false;
        bool isReadingImbue = false;

        public void ReadPlayerData(string playerName)
        {
            string fileDir = baseDirectory + playerName + "/" + playerName + playerExt;

            if (File.Exists(fileDir)) //Validate file
            {
                if (applyCryption == true)
                {
                    string decryptText = cryption.Decrypt(File.ReadAllText(fileDir));

                    string[] splitLines = decryptText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    for (int i = 0; i < splitLines.Length; i++)
                        linesOfPlayerData.Add(splitLines[i]);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(fileDir))
                    {
                        while (!reader.EndOfStream)
                        {
                            linesOfPlayerData.Add(reader.ReadLine());
                        }
                    }
                }
            }

            LoadPlayerData();
        }
        private void LoadPlayerData()
        {
            for (int i = 0; i < linesOfPlayerData.Count; i++)
            {
                IsLineDataTag(linesOfPlayerData, i, soulTag, ref isReadingSouls);
                IsLineDataTag(linesOfPlayerData, i, rumorsTag, ref isReadingRumors);
                IsLineDataTag(linesOfPlayerData, i, playerTag, ref isReadingPlayer);
                IsLineDataTag(linesOfPlayerData, i, statisticsTag, ref isReadingStats);
                IsLineDataTag(linesOfPlayerData, i, storageTag, ref isReadingItems);
                IsLineDataTag(linesOfPlayerData, i, spellTag, ref isReadingSpells);
                IsLineDataTag(linesOfPlayerData, i, recipeTag, ref isReadingRecipes);
                IsLineDataTag(linesOfPlayerData, i, stoneholdTag, ref isReadingStonehold);
                IsLineDataTag(linesOfPlayerData, i, smelterTag, ref isReadingSmelting);
                IsLineDataTag(linesOfPlayerData, i, brewingTag, ref isReadingBrewing);
                IsLineDataTag(linesOfPlayerData, i, imbueTag, ref isReadingImbue);

                AddLine(linesOfPlayerData, i, isReadingPlayerMeta, metaTag, ref playerMetaLines);
                AddLine(linesOfPlayerData, i, isReadingSouls, soulTag, ref soulLines);
                AddLine(linesOfPlayerData, i, isReadingRumors, rumorsTag, ref rumorLines);
                AddLine(linesOfPlayerData, i, isReadingPlayer, playerTag, ref playerLines);
                AddLine(linesOfPlayerData, i, isReadingStats, statisticsTag, ref statisticsLines);
                AddLine(linesOfPlayerData, i, isReadingItems, storageTag, ref itemLines);
                AddLine(linesOfPlayerData, i, isReadingSpells, spellTag, ref spellLines);
                AddLine(linesOfPlayerData, i, isReadingRecipes, recipeTag, ref recipeLines);
                AddLine(linesOfPlayerData, i, isReadingStonehold, stoneholdTag, ref stoneholdLines);
                AddLine(linesOfPlayerData, i, isReadingSmelting, smelterTag, ref smeltingLines);
                AddLine(linesOfPlayerData, i, isReadingBrewing, brewingTag, ref brewingLines);
                AddLine(linesOfPlayerData, i, isReadingImbue, imbueTag, ref imbueLines);
            }

            screens.LoadRumorData(rumorLines);
            screens.OFFERINGS_LoadData(recipeLines);
            screens.STONEHOLD_LoadData(stoneholdLines);
            screens.SMELTER_LoadData(smeltingLines);
            screens.BREWING_LoadData(brewingLines);
            screens.IMBUE_LoadData(imbueLines);

            playerEntity.LoadData(playerLines);
            playerEntity.LoadStorageData(itemLines);
            playerEntity.STATISTICS_LoadData(statisticsLines);

            ResetPlayerLoadingVariables();
        }
        private void ResetPlayerLoadingVariables()
        {
            linesOfPlayerData.Clear();

            soulLines.Clear();
            playerLines.Clear();
            itemLines.Clear();

            rumorLines.Clear();
            spellLines.Clear();
            recipeLines.Clear();
            stoneholdLines.Clear();
            smeltingLines.Clear();
            brewingLines.Clear();
            imbueLines.Clear();

            statisticsLines.Clear();

            isReadingSouls = false;
            isReadingRumors = false;
            isReadingPlayer = false;
            isReadingStats = false;
            isReadingItems = false;
            isReadingSpells = false;
            isReadingRecipes = false;
            isReadingStonehold = false;
            isReadingSmelting = false;
            isReadingBrewing = false;
            isReadingImbue = false;
        }
        public void WritePlayerData(string playerName)
        {
            Directory.CreateDirectory(baseDirectory + playerName + "/");
            string fileDir = baseDirectory + playerName + "/" + playerName + playerExt;

            if (!File.Exists(fileDir)) //If the file doesn't exist, create it!
                File.Create(fileDir);

            string encryption;
            StringBuilder totalData = new StringBuilder();

            totalData.AppendLine(metaTag);
            totalData.AppendLine("Version \"" + GameInfo.Version() + "\"");
            totalData.AppendLine(metaTag.Replace("[", "[/"));
            totalData.AppendLine();

            totalData.AppendLine(playerEntity.SaveData(playerTag).ToString());

            totalData.AppendLine(playerEntity.SaveStorageData(storageTag).ToString());
            totalData.AppendLine(screens.SaveRumorData(rumorsTag).ToString());

            totalData.AppendLine(screens.OFFERINGS_SaveData(recipeTag).ToString());
            totalData.AppendLine(screens.STONEHOLD_SaveData(stoneholdTag).ToString());
            totalData.AppendLine(screens.SMELTER_SaveData(smelterTag).ToString());
            totalData.AppendLine(screens.BREWING_SaveData(brewingTag).ToString());
            totalData.AppendLine(screens.IMBUE_SaveData(imbueTag).ToString());

            totalData.AppendLine(playerEntity.STATISTICS_SaveData(statisticsTag));

            if (applyCryption == true)
                encryption = cryption.Encrypt(totalData.ToString());
            else
                encryption = totalData.ToString();

            try
            {
                File.WriteAllText(fileDir, encryption);
            }
            catch(Exception e)
            {
                Logger.AppendLine("Error saving player data: " + e.Message);
            }
        }

        // --- Map Data Management ---

        List<string> mapMetaLines = new List<string>();
        List<string> triggerLines = new List<string>();
        List<string> containerLines = new List<string>();
        List<string> floraLines = new List<string>();
        List<string> entityLines = new List<string>();
        List<string> variousLines = new List<string>();

        bool isReadingTriggers = false;
        bool isReadingContainers = false;
        bool isReadingFlora = false;
        bool isReadingEntity = false;
        bool isReadingVarious = false;

        public void ReadMapData(string playerName, string mapName)
        {
            string mapFileDir = baseDirectory + playerName + "/Maps/" + mapName + mapExt;
            if (File.Exists(mapFileDir)) //Check to ensure the file is real.
            {
                if (applyCryption == true)
                {
                    string decryptText = cryption.Decrypt(File.ReadAllText(mapFileDir));

                    string[] splitLines = decryptText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    for (int i = 0; i < splitLines.Length; i++)
                        linesOfMapData.Add(splitLines[i]);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(mapFileDir))
                    {
                        while (!reader.EndOfStream)
                        {
                            linesOfMapData.Add(reader.ReadLine());
                        }
                    }
                }
            }

            LoadMapData();
        }
        private void LoadMapData()
        {
            for (int i = 0; i < linesOfMapData.Count; i++)
            {
                IsLineDataTag(linesOfMapData, i, triggerTag, ref isReadingTriggers);
                AddLine(linesOfMapData, i, isReadingTriggers, triggerTag, ref triggerLines);

                IsLineDataTag(linesOfMapData, i, containerTag, ref isReadingContainers);
                AddLine(linesOfMapData, i, isReadingContainers, containerTag, ref containerLines);

                IsLineDataTag(linesOfMapData, i, floraTag, ref isReadingFlora);
                AddLine(linesOfMapData, i, isReadingFlora, floraTag, ref floraLines);

                IsLineDataTag(linesOfMapData, i, entitiesTag, ref isReadingEntity);
                AddLine(linesOfMapData, i, isReadingEntity, entitiesTag, ref entityLines);

                IsLineDataTag(linesOfMapData, i, variousTag, ref isReadingVarious);
                AddLine(linesOfMapData, i, isReadingVarious, variousTag, ref variousLines);
            }

            maps.LoadTriggerData(triggerLines);
            maps.LoadContainerData(containerLines);
            maps.LoadFloraData(floraLines);
            maps.LoadEntitiesData(entityLines);
            maps.LoadVariousObjectData(variousLines);

            ResetPlayerLoadingVariables();
        }
        private void ResetMapLoadingVariables()
        {
            linesOfMapData.Clear();

            triggerLines.Clear();
            containerLines.Clear();
            floraLines.Clear();
            entityLines.Clear();
            variousLines.Clear();

            isReadingTriggers = false;
            isReadingContainers = false;
            isReadingFlora = false;
            isReadingEntity = false;
            isReadingVarious = false;
        }
        public void WriteMapData(string playerName, string mapName)
        {
            string mapFileDir = baseDirectory + playerName + "/Maps/" + mapName + mapExt;

            Directory.CreateDirectory(baseDirectory + playerName + "/Maps/");

            if (!File.Exists(mapFileDir)) //If the file doesn't exist, create it!
                File.Create(mapFileDir);

            string encryption;
            StringBuilder totalData = new StringBuilder();

            totalData.AppendLine("[Meta]");
            totalData.AppendLine("Version \"" + GameInfo.Version() + "\"");
            totalData.AppendLine("[/Meta]");
            totalData.AppendLine();

            totalData.AppendLine(maps.SaveData(triggerTag, containerTag, floraTag, entitiesTag, variousTag).ToString());

            if (applyCryption == true)
                encryption = cryption.Encrypt(totalData.ToString());
            else
                encryption = totalData.ToString();

            try
            {
                File.WriteAllText(mapFileDir, encryption);
            }
            catch(Exception e)
            {
                debug.OutputError("Error saving map data: " + e.Message);
            }
        }

        // --- World Data Management ---


        List<string> calendarLines = new List<string>();
        List<string> soulgateLines = new List<string>();
        bool isReadingCalendar = false;
        bool isReadingSoulgates = false;

        public void ReadWorldData(string playerName)
        {
            string worldFileDir = baseDirectory + playerName + "/World" + worldExt;
            if (File.Exists(worldFileDir)) //Check to ensure the file is real.
            {
                if (applyCryption == true)
                {
                    string decryptText = cryption.Decrypt(File.ReadAllText(worldFileDir));

                    string[] splitLines = decryptText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    for (int i = 0; i < splitLines.Length; i++)
                        linesOfWorldData.Add(splitLines[i]);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(worldFileDir))
                    {
                        while (!reader.EndOfStream)
                        {
                            linesOfWorldData.Add(reader.ReadLine());
                        }
                    }
                }
            }

            LoadWorldData();
        }
        private void LoadWorldData()
        {
            for (int i = 0; i < linesOfWorldData.Count; i++)
            {
                IsLineDataTag(linesOfWorldData, i, calendarTag, ref isReadingCalendar);
                IsLineDataTag(linesOfWorldData, i, soulgateTag, ref isReadingSoulgates);

                AddLine(linesOfWorldData, i, isReadingCalendar, calendarTag, ref calendarLines);
                AddLine(linesOfWorldData, i, isReadingSoulgates, soulgateTag, ref soulgateLines);
            }

            culture.LoadData(calendarLines);
            maps.LoadSoulgateData(soulgateLines);

            ResetWorldLoadingVariables();
        }
        private void ResetWorldLoadingVariables()
        {
            linesOfWorldData.Clear();

            calendarLines.Clear();
            soulgateLines.Clear();

            isReadingCalendar = false;
            isReadingSoulgates = false;
        }
        public void WriteWorldData(string playerName)
        {
            string worldFileDir = baseDirectory + playerName + "/World" + worldExt;

            Directory.CreateDirectory(baseDirectory + playerName + "/Maps/");

            if (!File.Exists(worldFileDir)) //If the file doesn't exist, create it!
                File.Create(worldFileDir);

            string encryption;
            StringBuilder totalData = new StringBuilder();

            totalData.AppendLine("[Meta]");
            totalData.AppendLine("Version \"" + GameInfo.Version() + "\"");
            totalData.AppendLine("[/Meta]");
            totalData.AppendLine();

            totalData.AppendLine(culture.SaveData(calendarTag).ToString());
            totalData.AppendLine(maps.SaveSoulgateData(soulgateTag).ToString());

            if (applyCryption == true)
                encryption = cryption.Encrypt(totalData.ToString());
            else
                encryption = totalData.ToString();

            try
            {
                File.WriteAllText(worldFileDir, encryption);
            }
            catch(Exception e)
            {
                Logger.AppendLine("Error saving world data: " + e.Message);
            }
        }

        // --- Helping Methods ---

        private void IsLineDataTag(List<string> strings, int i, string tag, ref bool returnValue)
        {
            if (strings[i].ToUpper().Contains(tag.ToUpper()))
                returnValue = true;
            else if (strings[i].ToUpper().Contains(tag.Replace("[", "[/")))
                returnValue = false;
        }
        private void AddLine(List<string> strings, int i, bool isReadingTag, string tag, ref List<string> tagLines)
        {
            if (isReadingTag == true)
            {
                if (strings[i].ToUpper().Contains(tag.ToUpper()))
                    strings[i] = "";

                if (!string.IsNullOrEmpty(strings[i]))
                    tagLines.Add(strings[i]);
            }
        }
    }
}
