using System;
using Pilgrimage_Of_Embers.TileEngine;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.ScreenEngine
{
    public class BaseScreen
    {
        public Vector2 Position { get; protected set; }

        protected Rectangle dragArea, clickRect;

        public bool IsActive { get; set; }

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        /*
            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            //Update

            hintRect = new Rectangle((int)screenPosition.X + 393, (int)screenPosition.Y, windowButton.Width - 20, windowButton.Height);
            hideRect = new Rectangle((int)screenPosition.X + 420, (int)screenPosition.Y, windowButton.Width - 20, windowButton.Height);

            if (hintRect.Contains(controls.MousePosition))
            {
                isHintHover = true;

                ToolTip.RequestStringAssign("Inventory Tips:\n\n" +
                        "Items can be dragged by left-clicking and dragging the cursor\n" +
                        "while over the item. With dragged item in hand, you can: \n\n" +
                        "1. Combine them with other items by right-clicking\n" +
                        "2. Gift them to other characters\n" +
                        "3. Use them in other UIs like the Stonehold or Bartering.\n\n" +
                        "Right-click an item then left-click a separate item to combine them.\n" +
                        "This enables combining items across different tabs.");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                }
            }
            else
                isHintHover = false;

            if (hideRect.Contains(controls.MousePosition))
            {
                isHideHover = true;
                ToolTip.RequestStringAssign("Hide Inventory");

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    this.isActive = false;
                }
            }
            else
                isHideHover = false;

            //Draw
            if (isHintHover == true)
                sb.Draw(windowButtonHover, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);
            else
                sb.Draw(windowButton, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);

            if (isHideHover == true)
                sb.Draw(windowButtonHover, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);
            else
                sb.Draw(windowButton, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);

            sb.Draw(hintIcon, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);
            sb.Draw(hideIcon, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);
        */

        protected Controls controls = new Controls();

        protected ScreenManager screens;
        protected TileMap tileMap;
        protected BaseEntity controlledEntity;
        protected Camera camera;

        public BaseScreen() { }
        public void SetReferences(ScreenManager screens, TileMap tileMap, BaseEntity controlledEntity, Camera camera)
        {
            this.screens = screens;
            this.tileMap = tileMap;
            this.controlledEntity = controlledEntity;
            this.camera = camera;
        }
        public virtual void SetControlledEntity(BaseEntity controlledEntity)
        {
            this.controlledEntity = controlledEntity;
        }

        public virtual void Load(ContentManager cm)
        {
            LoadWindowButtons(cm);
        }
        protected virtual void LoadWindowButtons(ContentManager cm)
        {
            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");
        }

        public virtual void Update(GameTime gt)
        {
            controls.UpdateLast();
            controls.UpdateCurrent();
        }
        protected virtual void UpdateWindowButtons(GameTime gt, int offset, string hintHover, string hideHover)
        {
            hintRect = new Rectangle((int)Position.X + offset, (int)Position.Y, windowButton.Width - 20, windowButton.Height);
            hideRect = new Rectangle((int)Position.X + offset + 27, (int)Position.Y, windowButton.Width - 20, windowButton.Height);

            if (hintRect.Contains(controls.MousePosition))
            {
                isHintHover = true;

                ToolTip.RequestStringAssign(hintHover);
            }
            else
                isHintHover = false;

            if (hideRect.Contains(controls.MousePosition))
            {
                isHideHover = true;
                ToolTip.RequestStringAssign(hideHover);

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    screens.PlaySound("Button Click 1");
                    IsActive = false;
                }
            }
            else
                isHideHover = false;
        }

        protected void SmoothScroll(GameTime gt, float scrollSpeed, float maxScrollSpeed, float scrollSlowdown, float clampSpeed,
                                  ref float scrollPosition, ref float scrollValue, ref float scrollVelocity, float longBounds, Rectangle container)
        {
            if (container.Contains(controls.MousePosition))
            {
                if (controls.CurrentMS.ScrollWheelValue < scrollValue)
                    scrollVelocity -= scrollSpeed;
                else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
                    scrollVelocity += scrollSpeed;
            }

            scrollValue = controls.CurrentMS.ScrollWheelValue;

            //Smooth scrolling code
            scrollPosition += scrollVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            scrollVelocity = MathHelper.Clamp(scrollVelocity, -maxScrollSpeed, maxScrollSpeed);

            if (scrollVelocity > clampSpeed)
                scrollVelocity -= scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity < -clampSpeed)
                scrollVelocity += scrollSlowdown * (float)gt.ElapsedGameTime.TotalSeconds;
            else if (scrollVelocity >= -clampSpeed && scrollVelocity < clampSpeed)
                scrollVelocity = 0f;

            if (longBounds >= 0f)
                longBounds = 0f;

            if (scrollPosition > 0f)
                scrollVelocity = 0f;
            else if (scrollPosition < longBounds)
                scrollVelocity = 0f;

            scrollPosition = MathHelper.Clamp(scrollPosition, longBounds, 0f);
        }

        protected bool isDragging = false;
        protected Vector2 mouseDragOffset;
        protected void CheckDrag(Rectangle rect)
        {
            dragArea = rect;

            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                Position = controls.MouseVector - mouseDragOffset;
                screens.SetCursorState(Cursor.CursorState.Move);
            }
        }

        RasterizerState scissorOn = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState scissorOff = new RasterizerState() { ScissorTestEnable = false };

        public virtual void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                DrawWindowButtons(sb);
            }
        }
        public virtual void DrawScissored(SpriteBatch sb)
        {

        }

        protected void DrawInside(SpriteBatch sb, Rectangle scissor, Action contents)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, scissorOn);
            sb.GraphicsDevice.ScissorRectangle = scissor;

            contents.Invoke();

            sb.GraphicsDevice.RasterizerState = scissorOff;
            sb.End();
        }

        protected virtual void DrawWindowButtons(SpriteBatch sb)
        {
            if (isHintHover == true)
                sb.Draw(windowButtonHover, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);
            else
                sb.Draw(windowButton, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);

            if (isHideHover == true)
                sb.Draw(windowButtonHover, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);
            else
                sb.Draw(windowButton, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);

            sb.Draw(hintIcon, new Vector2(hintRect.X - 10, hintRect.Y), Color.White);
            sb.Draw(hideIcon, new Vector2(hideRect.X - 10, hideRect.Y), Color.White);
        }

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return clickRect.Contains(controls.MousePosition) || hintRect.Contains(controls.MousePosition) || hideRect.Contains(controls.MousePosition) || isDragging;
            else
                return false;
        }
    }
}
