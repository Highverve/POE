using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers
{
    public class GameManager : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        WorldManager world;

        Controls controls = new Controls();

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreparingDeviceSettings += ((s, e) => e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents); //Windows only?

            Content.RootDirectory = "../MainContent";

            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(-10, 0);
            form.Size = new System.Drawing.Size(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 40);


            Logger.Initialize();

            Logger.AppendLine("----------------------------------------\nPilgrimage of Embers Logger File\n----------------------------------------", false);
            Logger.AppendLine("Version: " + GameInfo.Version(), false);

            Logger.AppendLine("", false);

            /*
                int width = Window.ClientBounds.Width;
                int height = Window.ClientBounds.Height;

                int displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                int displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                int windowWidth = graphics.GraphicsDevice.Viewport.Bounds.Width;
                int windowHeight = graphics.GraphicsDevice.Viewport.Bounds.Height;
            */

            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.IsFullScreen = GameSettings.IsFullScreen;

            this.IsMouseVisible = false;

            //TargetElapsedTime = TimeSpan.FromMilliseconds(16.666); //16.666 milliseconds, or 60(actually 60.00024000096) FPS.

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            world = new WorldManager(this, graphics);
            //Components.Add(world);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GameSettings.AssignResolution(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            world.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
            world.UnloadWorldContent();
        }

        protected override void Update(GameTime gameTime)
        {
            world.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            world.DrawTargets(spriteBatch);
            world.DrawAboveLight(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
