using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.TileEngine.Objects.Colliders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pilgrimage_Of_Embers.TileEngine
{
    public class PathfindMap
    {
        private Dictionary<int, int[,]> maps;
        public Dictionary<int, int[,]> Maps { get { return maps; } }

        private List<BaseCollider> colliders;
        private Circle scanner;

        public PathfindMap() { }
        public PathfindMap(Dictionary<int, int[,]> Maps)
        {
            this.maps = Maps;
        }

        public void SetColliders(List<BaseCollider> totalColliders)
        {
            this.colliders = totalColliders;
        }
        public void ClearColliders()
        {
            colliders.Clear();
        }

        private int totalFloors = 1;
        private List<int> floors = new List<int>();
        private void CountFloors()
        {
            floors.Add(1);

            for (int i = 0; i < colliders.Count; i++)
            {
                if (!floors.Contains(colliders[i].CurrentFloor))
                {
                    floors.Add(colliders[i].CurrentFloor);
                }
            }

            floors.Sort(); //Should sort floors in order from lowest to highest.
            totalFloors = floors.Count;
        }
        private void AddFloors(Point mapSize)
        {
            maps = new Dictionary<int, int[,]>();

            for (int i = 0; i < floors.Count; i++)
                maps.Add(floors[i], new int[mapSize.X, mapSize.Y]);
        }
        private void ScanColliders(Point mapSize)
        {
            scanner = new Circle(Vector2.Zero, 30);
            for (int x = 0; x < mapSize.X; x++)
            {
                scanner.X = (x * Pathfinder.TileSize.X) + (Pathfinder.TileSize.X / 2);
                for (int y = 0; y < mapSize.Y; y++)
                {
                    scanner.Y = (y * Pathfinder.TileSize.Y) + (Pathfinder.TileSize.Y / 2);
                    for (int i = 0; i < colliders.Count; i++)
                    {
                        if (colliders[i].Intersects(scanner))
                        {
                            maps[colliders[i].CurrentFloor][x, y] = -1; //Set the current index to -1.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Map scanning is pretty much a developer tool. Unnecessary for regular use, so you won't have to care about efficiency (though it is very efficient so far).
        /// </summary>
        /// <param name="mapSize"></param>
        public void ScanMap(Point mapSize)
        {
            CountFloors();
            AddFloors(mapSize);
            ScanColliders(mapSize);
        }

        /// <summary>
        /// Output all of the pathfinding layers into a text file.
        /// 
        /// This should be called after ScanMap() AND all of the colliders have been placed (future reference for level designing).
        /// </summary>
        public StringBuilder OutputPathfindMaps()
        {
            StringBuilder builder = new StringBuilder(); //[Pathfind X] --- Where 'X' is the current floor.

            foreach (KeyValuePair<int, int[,]> map in maps)
            {
                builder.Append("[Pathfind " + map.Key +"]");
                builder.AppendLine();

                if (map.Value != null) //Dunno if this is necessary. Leave it for now just in case map is null
                {
                    for (int x = 0; x < map.Value.GetLength(0); x++)
                    {
                        for (int y = 0; y < map.Value.GetLength(1); y++)
                        {
                            int index = map.Value[y, x];
                            builder.Append(index.ToString() + " ");
                        }
                        builder.AppendLine();
                    }
                }
                builder.AppendLine("[/Pathfind]");
                builder.AppendLine();
            }

            return builder;
        }

        private Texture2D pixel;
        private bool isAssigned;

        Color wall = Color.Lerp(Color.Black, Color.Transparent, .25f);
        Color highCost = Color.Lerp(Color.Red, Color.Transparent, .25f);
        public void DrawDebug(SpriteBatch sb)
        {
            if (isAssigned == false)
                pixel = TextureHelper.CreatePixel(sb, ref isAssigned);

            foreach (KeyValuePair<int, int[,]> map in maps)
            {
                for (int x = 0; x < map.Value.GetLength(0); x++)
                {
                    for (int y = 0; y < map.Value.GetLength(1); y++)
                    {
                        if (map.Value[x, y] == -1)
                            sb.DrawBoxBordered(pixel, new Rectangle(x * Pathfinder.TileSize.X, y * Pathfinder.TileSize.Y, Pathfinder.TileSize.X, Pathfinder.TileSize.Y), wall, Color.Black, 1f, 1);
                        if (map.Value[x, y] > 0)
                            sb.DrawBoxBordered(pixel, new Rectangle(x * Pathfinder.TileSize.X, y * Pathfinder.TileSize.Y, Pathfinder.TileSize.X, Pathfinder.TileSize.Y), highCost, Color.Black, 1f, 1);
                    }
                }
            }
        }

        public void SaveAsVisualImage(SpriteBatch sb, string mapName)
        {
            if (isAssigned == false)
                pixel = TextureHelper.CreatePixel(sb, ref isAssigned);

            foreach (KeyValuePair<int, int[,]> map in maps)// int m = 0; m < maps.Count; m++)
            {
                Point size = new Point(map.Value.GetLength(1), map.Value.GetLength(0));

                Texture2D mapPNG = new Texture2D(sb.GraphicsDevice, size.X, size.Y);
                Color[] colorData = new Color[size.X * size.Y];

                for (int x = 0; x < mapPNG.Width; x++)
                {
                    for (int y = 0; y < mapPNG.Height; y++)
                    {
                        colorData[x + y * mapPNG.Width] = Color.White;

                        if (map.Value[x, y] == -1)
                            colorData[x + y * mapPNG.Width] = Color.Black;
                    }
                }

                mapPNG.SetData(colorData);

                DateTime date = DateTime.Now;

                Directory.CreateDirectory("../../../../../Editor/Layers/" + mapName + "/Images/");
                Stream stream = File.Create("../../../../../Editor/Layers/" + mapName + "/Images/" + mapName + " floor " + map.Key + ", " + date.ToString("MM-dd-yy H;mm;ss") + ".png");

                mapPNG.SaveAsPng(stream, size.X, size.Y);
                mapPNG.Dispose();
                stream.Dispose();
            }
        }
    }
}
