using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrimage_Of_Embers.ScreenEngine.Main
{
    public class StartScreen
    {
        private SpriteFont font;

        private Color baseColor, lerpColor;
        private int delayTime = 0;
        private float fontLerp = .3f;
        private bool isLerpIncrease = false;

        private Controls controls = new Controls();

        public bool IsActive { get; set; }

        private ScreenManager screens;

        public StartScreen(ScreenManager Screens) { screens = Screens; }

        private float offset;
        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/titleFont");
            IsActive = true;

            baseColor = ColorHelper.UI_GlowingGold;
            offset = 150 * (GameSettings.VectorResolution.Y / 1080);

            lerpColor = Color.Lerp(Color.Transparent, baseColor, .3f);
        }

        public void Update(GameTime gt)
        {
            if (IsActive == true)
            {
                if (delayTime >= 5000)
                {
                    controls.UpdateCurrent();
                    CheckKeys();
                    controls.UpdateLast();

                    UpdateLerp(gt);
                }
                else
                    delayTime += gt.ElapsedGameTime.Milliseconds;
            }
        }
        private void CheckKeys()
        {
            Keys key = controls.GetPressedKey();

            if (key != Keys.None)
            {
                IsActive = false;
                delayTime = 0;

                screens.PlaySound("StartGame");
            }
        }
        private void UpdateLerp(GameTime gt)
        {
            if (isLerpIncrease == true)
                fontLerp += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
            else
                fontLerp -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;

            if (fontLerp >= 1f)
                isLerpIncrease = false;
            else if (fontLerp <= .3f)
                isLerpIncrease = true;

            lerpColor = Color.Lerp(Color.Transparent, baseColor, fontLerp);
        }

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                //sb.DrawString(font, "Press Any Key", GameSettings.VectorResolution - new Vector2(GameSettings.VectorCenter.X, 200),  lerpColor, 0f, "Press Any Key".LineCenter(font), 1f, SpriteEffects.None, 1f);
                sb.DrawStringBordered(font, "Press Any Key", GameSettings.VectorResolution - new Vector2(GameSettings.VectorCenter.X, offset), "Press Any Key".LineCenter(font), 0f, 1f, 1f, lerpColor, ColorHelper.Charcoal);
            }
        }
    }
}
