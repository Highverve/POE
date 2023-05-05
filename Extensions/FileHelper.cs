using System;
using System.Collections.Generic;
using System.IO;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class FileHelper
    {
        public static List<String> RetrieveFiles(string path, string keyword, SearchOption option, string eliminater)
        {
            List<String> files = new List<String>();

            foreach (string f in Directory.GetFiles(path, keyword, option))
            {
                string temp = f.Replace("\\", "/");
                temp = temp.Replace("../", "");
                temp = temp.Replace(eliminater, "");
                files.Add(temp);
            }

            return files;
        }

        public static List<String> RetrieveFiles(string path, string keyword, SearchOption option)
        {
            List<String> files = new List<String>();

            foreach (string f in Directory.GetFiles(path, keyword, option))
            {
                string temp = f.Replace("\\", "/");
                temp = temp.Replace("../", "");
                files.Add(temp);
            }

            return files;
        }

        public static List<String> RetrieveFiles(string path, string keyword)
        {
            List<String> files = new List<String>();

            foreach (string f in Directory.GetFiles(path, keyword))
            {
                string temp = f.Replace("\\", "/");
                temp = temp.Replace("../", "");
                files.Add(temp);
            }

            return files;
        }

        public static List<String> RetrieveFiles(string path)
        {
            List<String> files = new List<String>();

            foreach (string f in Directory.GetFiles(path))
            {
                string temp = f.Replace("\\", "/");
                temp = temp.Replace("../", "");
                files.Add(temp);
            }

            return files;
        }
    }
}
