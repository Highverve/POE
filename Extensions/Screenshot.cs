using System;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class Screenshot
    {
        public static bool isPictureTaken = true;
        public static bool IsPictureTaken() { return isPictureTaken; }

        public static void TakePicture(Texture2D renderTarget, string folder, string fileName)
        {
            isPictureTaken = false;

            Stream stream;
            DateTime dateTime = DateTime.Now;
            string name = fileName;//dateTime.ToString("MM-dd-yy_H;mm;ss") + ".png";

            if (File.Exists(folder))
                stream = File.Create(folder + name);
            else
            {
                Directory.CreateDirectory(folder);
                stream = File.Create(folder + name);
            }

            renderTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
            
            stream.Dispose();

            isPictureTaken = true;
        }
        public static void TakeRawPicture(GraphicsDevice g)
        {
            isPictureTaken = false;

            Point resolution = GameSettings.WindowResolution;

            int[] backBuffer = new int[resolution.X * resolution.Y];
            //g.GetBackBufferData(backBuffer);

            Texture2D texture = new Texture2D(g, resolution.X, resolution.Y);
            texture.SetData(backBuffer);

            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            string folder = @"\Screenshots\";

            Stream stream;
            DateTime dateTime = DateTime.Now;
            string name = dateTime.ToString("MM-dd-yy_H;mm;ss") + ".png";

            if (File.Exists(path + folder)) 
                stream = File.Create(path + folder + name);
            else
            {
                Directory.CreateDirectory(path + folder);
                stream = File.Create(path + folder + name);
            }

            texture.SaveAsPng(stream, resolution.X, resolution.Y);

            stream.Dispose();
            texture.Dispose();

            isPictureTaken = true;
        }
    }
}
