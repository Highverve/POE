using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers.ScreenEngine.Options
{
    public class Cursor
    {
        private Controls controls = new Controls();
        private BasicAnimation animation = new BasicAnimation(), moveAnimation = new BasicAnimation();

        public enum CursorState { Idle, Precision, Activate, Text, Move, Moving, Select }
        public CursorState State { get; set; }

        private Point activateFrame = Point.Zero;
        private Point moveFrame = Point.Zero;

        private Texture2D idle, precision, activate, text, move, moving, select;

        public Cursor() { State = CursorState.Idle; }

        public void Load(ContentManager cm)
        {
            idle = cm.Load<Texture2D>("Interface/Cursor/idle");
            precision = cm.Load<Texture2D>("Interface/Cursor/precision");
            activate = cm.Load<Texture2D>("Interface/Cursor/activate");
            text = cm.Load<Texture2D>("Interface/Cursor/text");
            move = cm.Load<Texture2D>("Interface/Cursor/move");
            moving = cm.Load<Texture2D>("Interface/Cursor/moveAnim");
            select = cm.Load<Texture2D>("Interface/Cursor/select");
        }

        public void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            activateFrame = animation.FramePosition(gt, 30, new Point(8, 1), true);
            moveFrame = moveAnimation.FramePosition(gt, 30, new Point(8, 1), true);

            controls.UpdateLast();
        }

        public void Draw(SpriteBatch sb)
        {
            if (GameSettings.DisplayCursor == true)
            {
                switch (State)
                {
                    case CursorState.Idle: sb.Draw(idle, controls.MouseVector, Color.White, new Vector2(12, 12), 0f, 1f); break;
                    case CursorState.Precision: sb.Draw(precision, controls.MouseVector, Color.White, new Vector2(12, 12), 0f, 1f); break;
                    case CursorState.Activate: sb.Draw(activate, controls.MouseVector, new Rectangle(activateFrame.X * 24, activateFrame.Y * 24, 24, 24), Color.White, 0f, new Vector2(12, 12), 1f, SpriteEffects.None, 1f); break;
                    case CursorState.Text: sb.Draw(text, controls.MouseVector, Color.White, new Vector2(11, 12), 0f, 1f); break;
                    case CursorState.Move: sb.Draw(move, controls.MouseVector, Color.White, new Vector2(12, 12), 0f, 1f); break;
                    case CursorState.Moving: sb.Draw(moving, controls.MouseVector, new Rectangle(moveFrame.X * 24, moveFrame.Y * 24, 24, 24), Color.White, 0f, new Vector2(12, 12), 1f, SpriteEffects.None, 1f); break;
                    case CursorState.Select: sb.Draw(select, controls.MouseVector, Color.White, new Vector2(2, 21), 0f, 1f); break;
                }
            }

            State = CursorState.Idle;
        }
    }
}
