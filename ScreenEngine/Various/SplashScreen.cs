using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ScreenEngine.Various
{
    public class SplashScreen
    {
        private Texture2D logo;
        private SpriteFont font;

        private float lerp = 0f;
        private int pauseTime = 0, delay = 0;
        private bool isIncreasing = true;

        private ScreenManager screens;
        private WorldManager world;

        public SplashScreen(ScreenManager screens, WorldManager world)
        {
            this.screens = screens;
            this.world = world;
        }

        public void Load(ContentManager cm)
        {
            logo = cm.Load<Texture2D>("Interface/Various/logo");
            font = cm.Load<SpriteFont>("Fonts/titleFont");
        }

        public bool IsSplashComplete { get; private set; }

        private Controls controls = new Controls();

        public void Update(GameTime gt)
        {
            if (world.MAIN_IsActive == true)
            {
                /*
                if (lerp >= 1f)
                {
                    controls.UpdateCurrent();

                    Microsoft.Xna.Framework.Input.Keys key = controls.GetPressedKey();

                    if (key != Microsoft.Xna.Framework.Input.Keys.None)
                        isIncreasing = false;

                    controls.UpdateLast();
                }*/

                if (delay >= 1000)
                {
                    if (isIncreasing == true)
                        lerp += 2f * (float)gt.ElapsedGameTime.TotalSeconds;
                    else
                        lerp -= 2f * (float)gt.ElapsedGameTime.TotalSeconds;

                    lerp = MathHelper.Clamp(lerp, 0f, 1f);

                    if (lerp >= 1f)
                    {
                        pauseTime += gt.ElapsedGameTime.Milliseconds;

                        if (pauseTime >= 2000)
                        {
                            isIncreasing = false;
                            pauseTime = 0;
                        }
                    }
                }
                else
                    delay += gt.ElapsedGameTime.Milliseconds;

                if (isIncreasing == false && lerp <= 0f)
                {
                    IsSplashComplete = true;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (world.MAIN_IsActive == true)
            {
                sb.Draw(logo, GameSettings.VectorCenter + new Vector2(75, -100), Color.Lerp(Color.Transparent, Color.White, lerp), logo.Center(), 0f, .35f, 0f);
                sb.DrawString(font, "Enckling Games", GameSettings.VectorCenter + new Vector2(0, 150), "Enckling Games".LineCenter(font), Color.Lerp(Color.Transparent, Color.White, lerp), 1f);
            }
        }
    }
}
