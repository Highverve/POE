namespace Pilgrimage_Of_Embers.ScreenEngine.Options
{
    public static class GameInfo
    {
        /*  Recommended version names:

            1. Ashen (a.k.a, pre-alpha)
            2. Kindling (a.k.a, first release to public)
            3. Flames
            4. Embers
            5. ...
        */

        public const string VersionName = "Ashen";
        public const int MajorVersion = 0;
        public const int WeeklyUpdate = 46;
        public const int MinorRevision = 0;

        public static string Version() { return VersionName + " " + MajorVersion + "." + WeeklyUpdate + "." + MinorRevision; }
    }
}
