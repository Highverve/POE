using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Performance;

namespace Pilgrimage_Of_Embers.Debugging
{
    public class FPSCounter
    {
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        SpriteFont sf;


        CallLimiter limitLogFPS = new CallLimiter(45000);
        private List<int> FPSAVG = new List<int>(); //FPS average

        public int FrameRate { get { return frameRate; } }

        public static int GlobalFrameRate { get; private set; }

        public FPSCounter(SpriteFont SF) { sf = SF; }

        public void Update(GameTime gt)
        {
            elapsedTime += gt.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;

                FPSAVG.Add(frameRate);
                GlobalFrameRate = frameRate;
            }

            if (limitLogFPS.IsCalling(gt))
            {
                float avg = 0;
                for (int i = 0; i < FPSAVG.Count; i++)
                    avg += FPSAVG[i];

                avg /= FPSAVG.Count;
                Logger.AppendLine("Average FPS is " + (int)avg + "");

                if (FPSAVG.Count >= 300)
                    FPSAVG.Clear();
            }
        }

        public void Draw(SpriteBatch sb)
        {
            frameCounter++;

            string fps = string.Format("{0}", frameRate);

            if (frameRate >= 50) SpriteBatchHelper.DrawStringBordered(sb, sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 4, 4), Color.Lerp(Color.White, Color.Transparent, .25f), Color.Lerp(Color.Black, Color.Transparent, .7f));
            if (frameRate >= 30 && frameRate < 50) SpriteBatchHelper.DrawStringBordered(sb, sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 4, 4), Color.Lerp(Color.Yellow, Color.Transparent, .25f), Color.Lerp(Color.Black, Color.Transparent, .7f));
            if (frameRate >= 10 && frameRate < 30) SpriteBatchHelper.DrawStringBordered(sb, sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 4, 4), Color.Lerp(Color.Orange, Color.Transparent, .25f), Color.Lerp(Color.Black, Color.Transparent, .7f));
            if (frameRate >= 0 && frameRate < 10) SpriteBatchHelper.DrawStringBordered(sb, sf, fps, new Vector2(GameSettings.WindowResolution.X - sf.MeasureString(fps).X - 4, 4), Color.Lerp(Color.Red, Color.Transparent, .25f), Color.Lerp(Color.Black, Color.Transparent, .7f));
        }
    }
}
