using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Content;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public class ScreenEffects
    {
        public enum TransitionType { None, Fade, Swipe }
        TransitionType transitionType = TransitionType.None;

        private bool isBeginning = true;
        private float lerp = 0f, incrementSpeed = 1f, decrementSpeed = 2f;
        private int pauseTime = 500, currentTime;
        Color color, assignedColor;
        Texture2D pixel;

        ShaderType blur;
        WorldManager world;

        public bool IsFaded { get; set; }
        public float FadeLerp { get { return lerp; } set { lerp = value; } }

        private Controls testControls = new Controls();

        public ScreenEffects(WorldManager World)
        {
            world = World;
        }

        public void Load(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("rect");
        }

        public void BeginTransition(TransitionType type, Color color, int pauseTime, float incrementSpeed, float decrementSpeed)
        {
            //Prevent from getting interrupted!
            if (transitionType == TransitionType.None)
            {
                ResetVariables();

                transitionType = type;
                assignedColor = color;
                this.pauseTime = pauseTime;
                this.incrementSpeed = incrementSpeed;
                this.decrementSpeed = decrementSpeed;
            }
        }
        private void ResetVariables()
        {
            transitionType = TransitionType.None;

            color = Color.Transparent;
            assignedColor = Color.Transparent;

            lerp = 0f;
            pauseTime = 500;

            isBeginning = true;
            IsFaded = false;

            world.ActivateShader("Blur");
            blur = world.GetShader("Blur");
        }

        public void Update(GameTime gt)
        {
            lerp = MathHelper.Clamp(lerp, 0f, 1f);
            //pauseTime = (int)MathHelper.Clamp(pauseTime, 0, 2500);

            switch (transitionType)
            {
                case TransitionType.Fade: FadeScreen(gt); break;
            }

            UpdateWidescreen(gt);
        }
        private void FadeScreen(GameTime gt)
        {
            color = Color.Lerp(Color.Transparent, assignedColor, lerp);

            if (isBeginning == true)
            {
                lerp += incrementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
                IsFaded = false;

                if (lerp >= 1f)
                {
                    currentTime += gt.ElapsedGameTime.Milliseconds;
                    IsFaded = true;

                    if (currentTime >= pauseTime)
                    {
                        currentTime = 0;
                        isBeginning = false;
                    }
                }
            }
            else
            {
                lerp -= decrementSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
                IsFaded = false;

                if (lerp <= 0f)
                {
                    transitionType = TransitionType.None;
                    world.DeactivateShader("Blur");
                }
            }

            blur.Effect.Parameters["currentOffset"].SetValue(MathHelper.Clamp(lerp * .00025f, 0f, 1f));
        }

        public void ForceEnd()
        {
            currentTime = pauseTime;
        }


        private float widescreenLerp, widescreenHeight;
        private Color widescreenColor;

        public bool IsWidescreen { get; private set; }
        public void StartWidescreen()
        {
            if (GameSettings.IsUseWidescreen == true)
                IsWidescreen = true;
            else
                IsWidescreen = false;
        }
        public void EndWidescreen()
        {
            IsWidescreen = false;
        }
        public void UpdateWidescreen(GameTime gt)
        {
            if (IsWidescreen == true)
            {
                if (GameSettings.IsUseWidescreen == false)
                    IsWidescreen = false;

                widescreenLerp += 4f * (float)gt.ElapsedGameTime.TotalSeconds;

                if (widescreenLerp > 1f)
                    widescreenLerp = 1f;
            }
            else
            {
                widescreenLerp -= 4f * (float)gt.ElapsedGameTime.TotalSeconds;

                if (widescreenLerp < 0f)
                    widescreenLerp = 0f;
            }

            if (widescreenLerp > 0 && widescreenLerp < 1)
            {
                widescreenHeight = MathHelper.SmoothStep(0, 75, widescreenLerp);
                widescreenColor = Color.Lerp(Color.Transparent, Color.Black, widescreenLerp);
            }

            testControls.UpdateCurrent();

            if (testControls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.D4))
                StartWidescreen();
            if (testControls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.D5))
                EndWidescreen();

            testControls.UpdateLast();
        }

        public void Draw(SpriteBatch sb)
        {
            if (transitionType != TransitionType.None)
                sb.Draw(pixel, new Rectangle(0, 0, GameSettings.WindowResolution.X, GameSettings.WindowResolution.Y), color);

            if (widescreenLerp > 0)
            {
                sb.Draw(pixel, new Rectangle(0, 0, GameSettings.WindowResolution.X, (int)widescreenHeight), widescreenColor);
                sb.Draw(pixel, new Rectangle(0, GameSettings.WindowResolution.Y - (int)widescreenHeight, GameSettings.WindowResolution.X, (int)widescreenHeight), widescreenColor);
            }
        }
    }
}
