using System;

namespace Pilgrimage_Of_Embers
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)

        {
            using (GameManager game = new GameManager())
            {
                game.Run();
            }
        }
    }
#endif
}

