using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public static class TextureHelper
    {
        public static Texture2D CreatePixel(SpriteBatch sb, ref bool isAssigned)
        {
            Texture2D texture = new Texture2D(sb.GraphicsDevice, 1, 1);

            Color[] data = new Color[1] { Color.White };
            texture.SetData(data);
            isAssigned = true;

            return texture;
        }
        public static Texture2D CreatePixel(GraphicsDevice g)
        {
            Texture2D texture = new Texture2D(g, 1, 1);

            Color[] data = new Color[1] { Color.White };
            texture.SetData(data);

            return texture;
        }

        public static Vector2 Center(this Texture2D texture)
        {
            return new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public static Color[,] TextureTo2DArray(this Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];

            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * texture.Width];
                }
            }

            return colors2D;
        }

        /// <summary>
        /// Returns a 2D array of the "Red" value, from 0 to 255
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int[,] RedTo2DArray(this Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];

            texture.GetData(colors1D);

            int [,] array = new int[texture.Width, texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    array[x, y] = colors1D[x + y * texture.Width].R;
                }
            }

            return array;
        }
        /// <summary>
        /// Returns a 2D array of the "Green" value, from 0 to 255
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int[,] GreenTo2DArray(this Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];

            texture.GetData(colors1D);

            int[,] array = new int[texture.Width, texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    array[x, y] = colors1D[x + y * texture.Width].G;
                }
            }

            return array;
        }
        /// <summary>
        /// Returns a 2D array of the "Blue" value, from 0 to 255
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int[,] BlueTo2DArray(this Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];

            texture.GetData(colors1D);

            int[,] array = new int[texture.Width, texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    array[x, y] = colors1D[x + y * texture.Width].B;
                }
            }

            return array;
        }
        /// <summary>
        /// Returns a 2D array of the "Alpha" value, from 0 to 255
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int[,] AlphaTo2DArray(this Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];

            texture.GetData(colors1D);

            int[,] array = new int[texture.Width, texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    array[x, y] = colors1D[x + y * texture.Width].A;
                }
            }

            return array;
        }

        /// <summary>
        /// Returns the y position of the top-most pixel.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int TopPixel(this Texture2D texture)
        {
            int value = 0;

            Color[] colors1D = new Color[texture.Width * texture.Height];

            texture.GetData(colors1D);

            for (int i = 0; i < colors1D.Length; i++) //Reverse-for loop.
            {
                if (colors1D[i] != Color.Transparent) //If the color is not transparent ...
                {
                    value = i / texture.Height; //Return the first result that is not transparent, divided by texture height to get the pixel's Y location.
                    break;
                }
            }

            return value;
        }
        /// <summary>
        /// Returns the y position of the bottom-most pixel.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int BottomPixel(this Texture2D texture)
        {
            int value = 0;

            Color[] colors1D = new Color[texture.Width * texture.Height];

            texture.GetData(colors1D);

            for (int i = colors1D.Length - 1; i > 0; --i) //Reverse-for loop.
            {
                if (colors1D[i] != Color.Transparent) //If the color is not transparent ...
                {
                    value = i / texture.Height; //Return the first result that is not transparent, divided by texture height to get the pixel's Y location.
                    break;
                }
            }

            return value;
        }
        /// <summary>
        /// Returns the x position of the right-most pixel.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int RightPixel(this Texture2D texture)
        {
            return 0;
        }
        /// <summary>
        /// Returns the x position of the left-most pixel.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static int LeftPixel(this Texture2D texture)
        {
            return 0;
        }

        public static Color SelectColor(this Texture2D texture, int xPosition)
        {
            xPosition = (int)MathHelper.Clamp(xPosition, 0, texture.Width);

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[xPosition];
        }
        public static Color SelectColor(this Texture2D texture, Point location)
        {
            location = new Point((int)MathHelper.Clamp(location.X, 0, texture.Width - 1), (int)MathHelper.Clamp(location.Y, 0, texture.Height - 1));

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[(texture.Width * location.Y) - (location.X - texture.Width)];
        }

        public static int SelectRedColor(this Texture2D texture, int xPosition)
        {
            xPosition = (int)MathHelper.Clamp(xPosition, 0, texture.Width);

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[xPosition].R;
        }
        public static int SelectRedColor(this Texture2D texture, Point location)
        {
            location = new Point((int)MathHelper.Clamp(location.X, 0, texture.Width - 1), (int)MathHelper.Clamp(location.Y, 0, texture.Height - 1));

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[(texture.Width * location.Y) - (location.X - texture.Width)].R;
        }

        public static int SelectGreenColor(this Texture2D texture, int xPosition)
        {
            xPosition = (int)MathHelper.Clamp(xPosition, 0, texture.Width);

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[xPosition].G;
        }
        public static int SelectGreenColor(this Texture2D texture, Point location)
        {
            location = new Point((int)MathHelper.Clamp(location.X, 0, texture.Width - 1), (int)MathHelper.Clamp(location.Y, 0, texture.Height - 1));

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[(texture.Width * location.Y) - (location.X - texture.Width)].G;
        }

        public static int SelectBlueColor(this Texture2D texture, int xPosition)
        {
            xPosition = (int)MathHelper.Clamp(xPosition, 0, texture.Width);

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[xPosition].B;
        }
        public static int SelectBlueColor(this Texture2D texture, Point location)
        {
            location = new Point((int)MathHelper.Clamp(location.X, 0, texture.Width - 1), (int)MathHelper.Clamp(location.Y, 0, texture.Height - 1));

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[(texture.Width * location.Y) - (location.X - texture.Width)].B;
        }

        public static int SelectAlphaColor(this Texture2D texture, int xPosition)
        {
            xPosition = (int)MathHelper.Clamp(xPosition, 0, texture.Width);

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[xPosition].A;
        }
        public static int SelectAlphaColor(this Texture2D texture, Point location)
        {
            location = new Point((int)MathHelper.Clamp(location.X, 0, texture.Width - 1), (int)MathHelper.Clamp(location.Y, 0, texture.Height - 1));

            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            return colors1D[(texture.Width * location.Y) - (location.X - texture.Width)].A;
        }

        public static Texture2D Crop(this Texture2D texture, GraphicsDevice graphics, Rectangle clipping)
        {
            Texture2D imageFile = new Texture2D(graphics, clipping.Width, clipping.Height);
            Color[] data = new Color[clipping.Width * clipping.Height];
            texture.GetData(0, clipping, data, 0, data.Length);
            imageFile.SetData(data);

            return imageFile;
        }

        public static Texture2D ChangeColor(this Texture2D texture, Color color, bool changeAlpha, bool softenEdges)
        {
            Color[,] colors = texture.TextureTo2DArray();
            Color[] colors1D = new Color[texture.Width * texture.Height];

            for (int y = 0; y < texture.Height - 1; y++)
            {
                for (int x = 0; x < texture.Width - 1; x++)
                {
                    bool hasSoftened = false;
                    if (softenEdges == true)
                    {
                        if (x > 0 && x < texture.Width - 1 && y > 0 && y < texture.Height - 1)
                        {
                            int totalNeighbours = TotalNeighbours(colors[x, y - 1].A, colors[x + 1, y].A, colors[x, y + 1].A, colors[x - 1, y].A);
                            if (totalNeighbours > 1 && totalNeighbours < 4)
                            {
                                colors[x, y].R = (byte)(color.R / 2); colors[x, y].G = (byte)(color.G / 2); colors[x, y].B = (byte)(color.B / 2);

                                colors[x, y].A = (byte)(color.A / 2);//(byte)(color.A * TotalNeighbours(colors[x, y - 1].A, colors[x + 1, y].A, colors[x, y + 1].A, colors[x - 1, y].A));

                                colors1D[y * texture.Height + x] = colors[x, y];

                                hasSoftened = true;
                            }
                        }
                        else
                        {
                            hasSoftened = false;
                        }
                    }

                    if (hasSoftened == false)
                    {
                        if (colors[x, y].A > 0)
                        {
                            colors[x, y].R = color.R; colors[x, y].G = color.G; colors[x, y].B = color.B;

                            if (changeAlpha == true)
                                colors[x, y].A = color.A;

                            colors1D[y * texture.Height + x] = colors[x, y];
                        }
                        else
                        {
                            colors1D[y * texture.Height + x] = Color.Transparent;
                        }
                    }
                }
            }
            texture.SetData<Color>(colors1D);

            return texture;
        }
        public static Texture2D ChangeColor(this Texture2D texture, Color color)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].A > 0)
                {
                    data[i] = color;
                }
            }

            texture.SetData(data);

            return texture;
        }
        public static bool HasNeighbours(int north, int east, int south, int west)
        {
            return (north != 0 || east != 0 || south != 0 || west != 0);
        }
        public static int TotalNeighbours(int north, int east, int south, int west)
        {
            int value = 0;

            if (north != 0)
                value++;
            if (east != 0)
                value++;
            if (south != 0)
                value++;
            if (west != 0)
                value++;

            return value;
        }

        public static List<Point> TextureOutline(this Texture2D texture)
        {
            List<Point> locations = new List<Point>();

            Color[,] colors2D = TextureTo2DArray(texture);

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    int totalAlpha = 0;

                    totalAlpha += colors2D[Math.Min(x + 1, texture.Width - 1), y].A;
                    totalAlpha += colors2D[Math.Max(x - 1, 0), y].A;
                    totalAlpha += colors2D[x, Math.Min(y + 1, texture.Height - 1)].A;
                    totalAlpha += colors2D[x, Math.Max(y - 1, 0)].A;

                    if (totalAlpha > 0)
                        locations.Add(new Point(x, y));
                }
            }

            return locations;
        }
        public static List<Point> Test(this Texture2D texture)
        {
            List<Point> locations = new List<Point>();

            Color[,] colors2D = TextureTo2DArray(texture);

            Point current = new Point(0, 0);
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    int totalAlpha = 0;

                    totalAlpha += colors2D[Math.Min(x + 1, texture.Width - 1), y].A;
                    totalAlpha += colors2D[Math.Max(x - 1, 0), y].A;
                    totalAlpha += colors2D[x, Math.Min(y + 1, texture.Height - 1)].A;
                    totalAlpha += colors2D[x, Math.Max(y - 1, 0)].A;

                    if (totalAlpha > 0)
                    {

                    }
                }
            }

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    int totalAlpha = 0;

                    totalAlpha += colors2D[Math.Min(x + 1, texture.Width - 1), y].A;
                    totalAlpha += colors2D[Math.Max(x - 1, 0), y].A;
                    totalAlpha += colors2D[x, Math.Min(y + 1, texture.Height - 1)].A;
                    totalAlpha += colors2D[x, Math.Max(y - 1, 0)].A;

                    if (totalAlpha > 0)
                        locations.Add(new Point(x, y));
                }
            }

            return locations;
        }

        public static Texture2D ChangeColor(this Texture2D texture, Color min, Color max, Color color, bool changeAlpha)
        {
            Color[,] colors = texture.TextureTo2DArray();
            Color[] colors1D = new Color[texture.Width * texture.Height];

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    if (colors[x, y].IsColorInRange(min, max) == true)
                    {
                        colors[x, y].R = color.R; colors[x, y].G = color.G; colors[x, y].B = color.B;

                        if (changeAlpha == true)
                            colors[x, y].A = color.A;

                        colors1D[y * texture.Height + x] = colors[x, y];
                    }
                }
            }
            Texture2D temp = texture;
            temp.SetData<Color>(colors1D);

            return temp;
        }

        public static Texture2D CreateShadow(this Texture2D inputTexture, SpriteBatch sb, float intensity)
        {
            Color[] colors = new Color[inputTexture.Width * inputTexture.Height];
            inputTexture.GetData<Color>(colors);

            Texture2D temp = new Texture2D(sb.GraphicsDevice, inputTexture.Width, inputTexture.Height);
            temp.SetData<Color>(colors);
            temp = temp.ChangeColor(new Color(0, 0, 0, intensity));

            return temp;
        }

        public static Texture2D CreateShadow(this Texture2D inputTexture, SpriteBatch sb, Color color, float intensity)
        {
            Color[] colors = new Color[inputTexture.Width * inputTexture.Height];
            inputTexture.GetData<Color>(colors);

            Texture2D temp = new Texture2D(sb.GraphicsDevice, inputTexture.Width, inputTexture.Height);
            temp.SetData<Color>(colors);
            temp = temp.ChangeColor(Color.Lerp(Color.Transparent, color, intensity), true, false);

            return temp;
        }

        public static Texture2D Copy(this Texture2D inputTexture, SpriteBatch sb)
        {
            Color[] colors = new Color[inputTexture.Width * inputTexture.Height];
            inputTexture.GetData<Color>(colors);

            Texture2D temp = new Texture2D(sb.GraphicsDevice, inputTexture.Width, inputTexture.Height);
            temp.SetData<Color>(colors);

            return temp;
        }
    }
}
