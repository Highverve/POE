using Pilgrimage_Of_Embers.Debugging;
using System;
using System.IO;

namespace Pilgrimage_Of_Embers
{
    public static class Logger
    {
        public const string fileName = "log.txt";

        private static object appendLock = new object();
        public static void AppendLine(string text, bool timeStamp = true)
        {
            lock(appendLock)
            {
                if (timeStamp == true)
                    File.AppendAllText(fileName, Info.SessionText() + " -- " + text + Environment.NewLine);
                else
                    File.AppendAllText(fileName, text + Environment.NewLine);
            }
        }

        private static TimeSpan Session;
        public static void SetSession(ref TimeSpan session) { Session = session; }

        /// <summary>
        /// Erases the old log file and creates a new one. Only should be called on in the constructor.
        /// </summary>
        public static void Initialize()
        {
            if (File.Exists(Logger.fileName))
                File.Delete(Logger.fileName);
        }
    }
}
