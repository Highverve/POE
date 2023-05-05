using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine;
using System.Diagnostics;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Performance;

namespace Pilgrimage_Of_Embers.Debugging
{
    public class Info
    {
        SpriteFont sf;
        Texture2D pixel;

        private static TimeSpan session = TimeSpan.Zero;
        DateTime time;

        public static string SessionText()
        {
            return string.Format("{0:hh\\:mm\\:ss}", session);
        }

        Vector2 baseLocation;
        public static Point currentTile = Point.Zero;

        private List<StringBuilder> customVariables = new List<StringBuilder>(20);
        public void SetVariable(string text, int index)
        {
            index -= 1;
            if (index < customVariables.Capacity)
            {
                customVariables[index].Clear();
                customVariables[index].Append((index + 1).ToString() + " - " + text.ToString());
            }
        }

        private TileMap tileMap;
        private ScreenManager screens;
        private Camera camera; private Vector2 screenRes;
        private PlayerEntity player;

        public bool IsActive = false;
        Controls controls = new Controls();

        //PerformanceCounter CPUCounter;

        public Info(SpriteFont SF, Texture2D Pixel, TileMap Map, ScreenManager Screens, Camera Camera, PlayerEntity Player)
        {
            sf = SF;
            pixel = Pixel;

            tileMap = Map;
            screens = Screens;
            camera = Camera;
            player = Player;

            baseLocation = new Vector2(10, 10);

            for (int i = 0; i < customVariables.Capacity; i++)
                customVariables.Add(new StringBuilder((i + 1).ToString() + " - "));

            screenRes = GameSettings.VectorResolution;

            //CPUCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        }

        int timer = 501;

        CallLimiter limitMemoryLog = new CallLimiter(45000);
        CallLimiter limitBuildString = new CallLimiter(100);

        float memoryCount;
        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenInfo))
                IsActive = !IsActive;

            time = DateTime.Now;
            session += gt.ElapsedGameTime;

            timer += gt.ElapsedGameTime.Milliseconds;

            if (timer >= 500)
            {
                //cpuCount = CPUCounter.NextValue();
                memoryCount = (float)((Process.GetCurrentProcess().WorkingSet64 / 1024) / 1024);

                timer = 0;
            }

            if (limitMemoryLog.IsCalling(gt))
                Logger.AppendLine("Current memory is " + memoryCount + "MB");
            if (limitBuildString.IsCalling(gt))
                BuildStrings();

            controls.UpdateLast();
        }

        StringBuilder combinedTime = new StringBuilder(), mapLine = new StringBuilder(), positionLine = new StringBuilder(), tileLine = new StringBuilder(), objectInfo = new StringBuilder();
        StringBuilder masterBuilder = new StringBuilder(), customBuilder = new StringBuilder();
        private void BuildStrings()
        {
            ClearStrings();

            combinedTime.Append("Session: " + string.Format("{0:d\\:hh\\:mm\\:ss}", session) + "  :  Current: " + time.ToString("hh:mm:ss tt : MMM d, ddd, yyyy"));
            mapLine.Append("MapName: " + tileMap.MapName + ".map  :  DisplayName: " + TileEngine.TileMap.DisplayName);

            positionLine.Append("Player[" + player.Position.X + ", " + player.Position.Y + "]  :  " + "Top Left[" + camera.Position.X + ", " + camera.Position.Y + "]");

            tileLine.Append("PlayerTile[" + player.Position.X / TileMap.TileWidth + ", " + player.Position.Y / TileMap.TileHeight + "]  :  " +
                            "TileMouse[" + (camera.ScreenToWorld(controls.MouseVector).X / TileMap.TileWidth) + ", " + (camera.ScreenToWorld(controls.MouseVector).Y / TileMap.TileHeight) + "]");
            objectInfo.Append("GameObjects: " + tileMap.GameObjectsInMap + "  :  Triggers: " + tileMap.TriggersInMap);

            masterBuilder.AppendLine(mapLine.ToString());
            masterBuilder.AppendLine(combinedTime.ToString());
            masterBuilder.AppendLine(positionLine.ToString());
            masterBuilder.AppendLine(tileLine.ToString());
            masterBuilder.AppendLine(objectInfo.ToString());
            masterBuilder.AppendLine("Current Particles: " + ParticleEngine.Particle.CurrentParticles + "   Recycled Particles: " + ParticleEngine.Particle.RecycledParticles);

            for (int i = 0; i < customVariables.Count; i++)
            {
                customBuilder.AppendLine(customVariables[i].ToString());
            }
        }
        private void ClearStrings()
        {
            combinedTime.Clear();
            mapLine.Clear();
            positionLine.Clear();
            tileLine.Clear();
            objectInfo.Clear();

            masterBuilder.Clear();
            customBuilder.Clear();
        }

        private Color inside = Color.Lerp(Color.Transparent, Color.Blue, .05f), outside = Color.Black;
        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {           
                SpriteBatchHelper.DrawBoxBordered(sb, pixel, new Rectangle((int)baseLocation.X, (int)baseLocation.Y, 500, 500), inside, outside);
                SpriteBatchHelper.DrawBoxBordered(sb, pixel, new Rectangle((int)baseLocation.X, (int)baseLocation.Y + 506, 500, 308), inside, outside);

                SpriteBatchHelper.DrawStringBordered(sb, sf, masterBuilder.ToString(), new Vector2(baseLocation.X + 8, baseLocation.Y + 8), Color.White, Color.Black);
                SpriteBatchHelper.DrawStringBordered(sb, sf, customBuilder.ToString(), new Vector2(baseLocation.X + 8, baseLocation.Y + 514), Color.White, Color.Black);
            }
        }
    }
}
