using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.ScreenEngine.Various
{
    public class GameExit
    {
        private SelectionBox box;
        private ScreenManager screens;
        private WorldManager world;

        public bool IsActive { get { return box.IsActive; } }
        private bool isExiting = false;

        private Controls controls = new Controls();

        public void ResetPosition()
        {
            box.ResetPosition();
        }

        public GameExit()
        {
            box = new SelectionBox("Quit Game?", "All unsaved data will be saved upon exiting. Are you sure you want to stop playing?", "Exit to Main Menu", "Exit to Desktop", "Cancel");
        }
        public void SetReferences(WorldManager world, ScreenManager screens)
        {
            this.world = world;
            this.screens = screens;
        }

        public void Load(ContentManager cm)
        {
            box.Load(cm);
        }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            box.Update(gt);

            if (box.CurrentSelection == 0)
            {
                box.IsActive = false;
                isExiting = true;
            }
            if (box.CurrentSelection == 1)
            {
                box.IsActive = false;
                world.QuitGame();
            }
            if (box.CurrentSelection == 2)
                box.IsActive = false;

            if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                screens.CloseAllUIs();

                box.IsActive = !box.IsActive;
            }

            if (isExiting == true)
            {
                screens.EFFECTS_BeginTransition(ScreenEffects.TransitionType.Fade, Color.Black, 1000, 2f, 1f);

                if (screens.EFFECTS_IsTransitionFaded == true)
                {
                    world.GoToMainMenu();

                    isExiting = false;
                }
            }

            controls.UpdateLast();
        }

        public void Draw(SpriteBatch sb)
        {
            box.Draw(sb);
        }
    }
}
