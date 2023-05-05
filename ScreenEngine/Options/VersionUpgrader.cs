using Pilgrimage_Of_Embers.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ScreenEngine.Options
{
    public static class VersionUpgrader
    {
        //When parse variables from game objects, player, etc. are changed ...
        //Add the current version with the specific action to each parse line.

        public static void UpgradeGameObject(int majorVersion, int weeklyUpdate, int minorRevision, ref string line)
        {
            //Example: Test -1 1 0 500 500 false
            //Run if the file version equals the specified version.
            if (majorVersion == 0 && weeklyUpdate == 45 && minorRevision == 0)
            {
                line = Replace(line, "WavyFlora", "grassTuft", "Other/grassTuft");
                line = Replace(line, "WavyFlora", "leafTuft", "Other/leafTuft");

                minorRevision = 1; //Increment to the next weekly update that needs to be tested...
            }
        }

        private static string UpgradePlayerSave(string version, string line)
        {
            string value = line;
            /*if (version == "0.0.0")
            {
                //Same example as above ...
            }*/
            return line;
        }
        /// <summary>
        /// This isn't nearly as important as UpgradeGameObject(...).
        /// </summary>
        /// <param name="version"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string UpgradeMapSave(string version, string line)
        {
            string value = line;
            /*if (version == "0.0.0")
            {
                //Same example as above ...
            }*/
            return line;
        }
        private static string UpgradeWorldSave(string version, string line)
        {
            string value = line;
            /*if (version == "0.0.0")
            {
                //Same example as above ...
            }*/
            return line;
        }

        private static string InsertVariable(string line, string parseName, string character, int characterNumber, string insertValue)
        {
            string value = line;

            if (value.ToUpper().StartsWith(parseName.ToUpper()))
            {
                int index = value.IndexOfNth(character, characterNumber);
                value = value.Insert(index, insertValue + character);
            }

            return value;
        }
        private static string Replace(string line, string parseName, string oldValue, string newValue)
        {
            string value = line;

            if (value.ToUpper().StartsWith(parseName.ToUpper()))
                value = value.Replace(oldValue, newValue);

            return value;
        }
        private static string DeleteVariable(string line, string parseName, string character, int characterNumber)
        {
            string value = line;

            if (value.ToUpper().StartsWith(parseName.ToUpper()))
            {
                //Find index of variable
                int startIndex = value.IndexOfNth(character, characterNumber);
                int endIndex = 0;

                //If the index is less than or equal to the total characters, get end index length
                if (characterNumber + 1 <= value.TotalCharacters(character))
                    endIndex = value.IndexOfNth(character, characterNumber + 1) - startIndex;
                else
                    endIndex = value.Length - startIndex;

                //If the startIndex and endIndex is not negative (that is, they have real values), remove the variable
                if (startIndex >= 0 && endIndex >= 0)
                    value = value.Remove(startIndex, endIndex);
            }

            return value;
        }
        private static string ChangeVariable(string line, string parseName, string character, int characterNumber, string replaceVariable)
        {
            string value = line;

            if (value.ToUpper().StartsWith(parseName.ToUpper()))
            {
                //Find index of variable
                int startIndex = value.IndexOfNth(character, characterNumber);
                int endIndex = 0;

                //If the index is less than or equal to the total characters, get end index length
                if (characterNumber + 1 <= value.TotalCharacters(character))
                    endIndex = value.IndexOfNth(character, characterNumber + 1) - startIndex;
                else
                    endIndex = value.Length - startIndex;

                //If the startIndex and endIndex is not negative (that is, they have real values), remove the variable
                if (startIndex >= 0 && endIndex >= 0)
                    value = value.Remove(startIndex, endIndex);

                //Insert the new variable in place of the deleted variable
                value = value.Insert(startIndex + 1, replaceVariable + character);
            }

            return value;
        }
        private static string AppendLine(string line, string parseName, string newLine)
        {
            string value = line;

            if (value.ToUpper().StartsWith(parseName.ToUpper()))
            {
                value += Environment.NewLine + newLine;
            }

            return value;
        }
    }
}
