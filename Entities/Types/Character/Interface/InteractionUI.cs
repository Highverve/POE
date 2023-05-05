using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pilgrimage_Of_Embers.Entities.Entities;
using Pilgrimage_Of_Embers.Entities.NPC;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Entities.Types.NPE.Character.Interface
{
    class InteractButton
    {
        Rectangle buttonRect;
        Texture2D buttonTexture, clickTexture, hoverTexture;

        BasicAnimation animation = new BasicAnimation();

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }

        public InteractButton(Texture2D ButtonTexture, Texture2D ClickTexture, Texture2D HoverTexture)
        {
            buttonTexture = ButtonTexture;
            clickTexture = ClickTexture;
            hoverTexture = HoverTexture;

            buttonRect = new Rectangle(0, 0, buttonTexture.Width, buttonTexture.Height);
        }

        public bool IsButtonPressed { get; private set; }
        public void Update(GameTime gt, Controls controls)
        {
            Update(gt, controls, controls.MouseVector);
        }
        public void Update(GameTime gt, Controls controls, Vector2 mousePosition)
        {
            IsButtonPressed = false;

            buttonRect.X = (int)(Position.X - Origin.X);
            buttonRect.Y = (int)(Position.Y - Origin.Y);

            if (buttonRect.Contains(mousePosition))
            {
                IsHover = true;

                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    IsLeftClicked = true;
                else
                    IsLeftClicked = false;

                if (controls.IsClickedOnce(Controls.MouseButton.RightClick))
                    IsRightClicked = true;
                else
                    IsRightClicked = false;

                if (controls.IsClickedOnce(Controls.MouseButton.MiddleClick))
                    IsMiddleClicked = true;
                else
                    IsMiddleClicked = false;

                if (controls.CurrentMS.LeftButton == ButtonState.Pressed ||
                    controls.CurrentMS.RightButton == ButtonState.Pressed ||
                    controls.CurrentMS.MiddleButton == ButtonState.Pressed)
                {
                    IsButtonPressed = true;
                }
            }
            else
            {
                IsHover = false;

                IsLeftClicked = false;
                IsRightClicked = false;
                IsMiddleClicked = false;
            }
        }

        public void DrawButton(SpriteBatch sb, Color color, float scale)
        {
            if (IsButtonPressed == true)
                sb.Draw(clickTexture, Position, color, Origin, 0f, scale, 1f);
            else if (IsHover == true)
                sb.Draw(hoverTexture, Position, color, Origin, 0f, scale, 1f);
            else
                sb.Draw(buttonTexture, Position, color, Origin, 0f, scale, 1f);
        }
        public void DrawButtonIdle(SpriteBatch sb, Color color)
        {
            sb.Draw(buttonTexture, buttonRect, color);
        }

        public bool IsHover { set; get; }
        public bool IsLeftClicked { set; get; }
        public bool IsRightClicked { set; get; }
        public bool IsMiddleClicked { set; get; }

        public void ResetButtonStates()
        {
            IsHover = false;
            IsLeftClicked = false;
            IsRightClicked = false;
            IsMiddleClicked = false;
        }
    }

    public class InteractionUI
    {
        private Vector2 position;

        private Texture2D bg, button, buttonSelect, talkIcon, buyIcon, barterIcon;
        private InteractButton talkButton, buyButton, barterButton, makeCompanionButton, compFollowButton, compStayButton, compLeaveMeButton;

        private float baseCircleRadius, circleRadius, buttonSpacing, baseSpacingOffset, spacingOffset, scale, animationLerp, slowAnimLerp, bgAngle;

        private Controls controls = new Controls();
        private ScreenManager screens;
        private BaseEntity entity, controlledEntity;
        private Camera camera;
        private ConversationManager conversing;

        private bool isActive = false;
        public bool IsActive { get { return isActive; } set { isActive = value; } }

        public InteractionUI() { }

        public void SetReferences(ScreenManager screens, BaseEntity entity, Camera camera, ConversationManager conversing)
        {
            this.screens = screens;
            this.entity = entity;
            this.camera = camera;
            this.conversing = conversing;
        }

        public void Load(ContentManager cm)
        {
            bg = cm.Load<Texture2D>("Interface/Character/selectionBackground");

            button = cm.Load<Texture2D>("Interface/Global/mediumButtonBG");
            buttonSelect = cm.Load<Texture2D>("Interface/Global/mediumButtonBGSelect");

            talkIcon = cm.Load<Texture2D>("Interface/Character/Icons/talk");
            buyIcon = cm.Load<Texture2D>("Interface/Character/Icons/buy");
            barterIcon = cm.Load<Texture2D>("Interface/Character/Icons/barter");


            talkButton = new InteractButton(button, buttonSelect, buttonSelect);
            buyButton = new InteractButton(button, buttonSelect, buttonSelect);
            barterButton = new InteractButton(button, buttonSelect, buttonSelect);

            makeCompanionButton = new InteractButton(button, buttonSelect, buttonSelect);
            compFollowButton = new InteractButton(button, buttonSelect, buttonSelect);
            compStayButton = new InteractButton(button, buttonSelect, buttonSelect);
            compLeaveMeButton = new InteractButton(button, buttonSelect, buttonSelect);

            talkButton.Origin = button.Center();
            buyButton.Origin = button.Center();
            barterButton.Origin = button.Center();
            makeCompanionButton.Origin = button.Center();
            compFollowButton.Origin = button.Center();
            compStayButton.Origin = button.Center();
            compLeaveMeButton.Origin = button.Center();


            baseCircleRadius = 85f;
            circleRadius = baseCircleRadius;

            buttonSpacing = .9f;
        }
        private void RecalculateButtonOffsets()
        {
            baseSpacingOffset = MathHelper.ToRadians(-90f) - (float)(lastButtonIndex - 1) * (buttonSpacing * .5f); ; //-147.25f
        }

        public void Update(GameTime gt, Vector2 position)
        {
            this.position = position;

            controls.UpdateCurrent();

            UpdateLerp(gt);

            if (buttonIndex == lastButtonIndex)
                RecalculateButtonOffsets();

            UpdateButtons(gt);

            controls.UpdateLast();
        }

        int buttonIndex = 0, lastButtonIndex = -1;
        private void UpdateButtons(GameTime gt)
        {
            if (isActive == true)
            {
                lastButtonIndex = buttonIndex;
                buttonIndex = 0;

                talkButton.Position = Circle.Rotate(spacingOffset + (buttonIndex * buttonSpacing), circleRadius, position);
                talkButton.Update(gt, controls);
                buttonIndex++;

                if (entity.MERCHANT_IsMerchant())
                {
                    buyButton.Position = Circle.Rotate(spacingOffset + (buttonIndex * buttonSpacing), circleRadius, position);
                    buyButton.Update(gt, controls);
                    buttonIndex++;

                    barterButton.Position = Circle.Rotate(spacingOffset + (buttonIndex * buttonSpacing), circleRadius, position);
                    barterButton.Update(gt, controls);
                    buttonIndex++;
                }
                else
                {
                    buyButton.ResetButtonStates();
                    barterButton.ResetButtonStates();
                }

                if (entity.CanMakeCompanion == true && entity.HasCompanionLeader == false)
                {
                    makeCompanionButton.Position = Circle.Rotate(spacingOffset + (buttonIndex * buttonSpacing), circleRadius, position);
                    makeCompanionButton.Update(gt, controls);
                    buttonIndex++;
                }
                else
                {
                    makeCompanionButton.ResetButtonStates();
                }

                if (entity.IsCompanionLeader(controlledEntity) == true)
                {
                    compFollowButton.Position = Circle.Rotate(spacingOffset + (buttonIndex * buttonSpacing), circleRadius, position);
                    compFollowButton.Update(gt, controls);
                    buttonIndex++;

                    compStayButton.Position = Circle.Rotate(spacingOffset + (buttonIndex * buttonSpacing), circleRadius, position);
                    compStayButton.Update(gt, controls);
                    buttonIndex++;

                    compLeaveMeButton.Position = Circle.Rotate(spacingOffset + (buttonIndex * buttonSpacing), circleRadius, position);
                    compLeaveMeButton.Update(gt, controls);
                    buttonIndex++;
                }
                else
                {
                    compFollowButton.ResetButtonStates();
                    compStayButton.ResetButtonStates();
                    compLeaveMeButton.ResetButtonStates();
                }

                if (talkButton.IsHover == true)
                    ToolTip.RequestStringAssign("Converse");
                if (buyButton.IsHover == true)
                    ToolTip.RequestStringAssign("Purchase");
                if (barterButton.IsHover == true)
                    ToolTip.RequestStringAssign("Barter");
                if (makeCompanionButton.IsHover == true)
                    ToolTip.RequestStringAssign("Make Follower");

                if (compFollowButton.IsHover == true)
                    ToolTip.RequestStringAssign("Follow Me");
                if (compStayButton.IsHover == true)
                    ToolTip.RequestStringAssign("Stay Here");
                if (compLeaveMeButton.IsHover == true)
                    ToolTip.RequestStringAssign("Part Ways");

                if (talkButton.IsLeftClicked == true)
                {
                    conversing.IsActive = true;

                    screens.BARTERING_End();
                    screens.PURCHASING_End();

                    isActive = false;
                }

                if (barterButton.IsLeftClicked == true)
                {
                    if (screens.BARTERING_IsActive() == false)
                    {
                        screens.BARTERING_SetMerchant(entity);
                        screens.BARTERING_Begin();
                    }
                    else if (screens.BARTERING_IsActive() == true)
                        screens.BARTERING_End();

                    //if (screens.PURCHASING_IsPurchasing())
                    //    screens.PURCHASING_End();

                    isActive = false;
                }

                if (buyButton.IsLeftClicked == true)
                {
                    screens.PURCHASING_SetMerchant(entity);
                    screens.PURCHASING_Begin();

                    if (screens.BARTERING_IsActive() == true)
                    {
                        screens.BARTERING_End();
                    }

                    isActive = false;
                }

                if (makeCompanionButton.IsLeftClicked == true)
                {
                    entity.MakeCompanion(controlledEntity);
                }

                if (compFollowButton.IsLeftClicked == true)
                    controlledEntity.SendMessage(new MessageHolder(controlledEntity, null, controlledEntity.Faction, "Companions", "Follow", 750, 100));

                if (compStayButton.IsLeftClicked == true)
                    controlledEntity.SendMessage(new MessageHolder(controlledEntity, null, controlledEntity.Faction, "Companions", "Guard", 750, 100));

                if (compLeaveMeButton.IsLeftClicked == true)
                {
                    entity.UnmakeCompanion();
                }
            }
        }
        private void UpdateLerp(GameTime gt)
        {
            bgAngle += .1f * (float)gt.ElapsedGameTime.TotalSeconds;

            if (isActive == true)
            {
                animationLerp += 5f * (float)gt.ElapsedGameTime.TotalSeconds;
                slowAnimLerp += 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                animationLerp -= 5f * (float)gt.ElapsedGameTime.TotalSeconds;
                slowAnimLerp -= 2f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            animationLerp = MathHelper.Clamp(animationLerp, 0f, 1f);
            slowAnimLerp = MathHelper.Clamp(slowAnimLerp, .5f, 1f);

            scale = MathHelper.SmoothStep(0f, 1f, animationLerp);
            fadeColor = Color.Lerp(Color.Transparent, new Color(255, 255, 255, 210), animationLerp);
            spacingOffset = MathHelper.SmoothStep(baseSpacingOffset + 3f, baseSpacingOffset, slowAnimLerp);

            circleRadius = MathHelper.SmoothStep(0f, baseCircleRadius, animationLerp);
        }

        private Color halfWhite = Color.Lerp(Color.White, Color.Transparent, .35f);
        private Color halfBlack = Color.Lerp(Color.Black, Color.Transparent, .35f);
        private Color fadeColor = Color.White;
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(bg, position, Color.Lerp(Color.Transparent, halfWhite, animationLerp), bg.Center(), bgAngle, scale - .1f, SpriteEffects.None, 0f);
            sb.Draw(bg, position, Color.Lerp(Color.Transparent, halfBlack, animationLerp), bg.Center(), -bgAngle, scale, SpriteEffects.None, 0f);

            DrawButtonIcon(sb, talkIcon, talkButton);

            if (entity.MERCHANT_IsMerchant())
            {
                DrawButtonIcon(sb, buyIcon, buyButton);
                DrawButtonIcon(sb, barterIcon, barterButton);
            }

            if (entity.CanMakeCompanion == true && entity.HasCompanionLeader == false)
                DrawButtonIcon(sb, null, makeCompanionButton);

            if (entity.IsCompanionLeader(controlledEntity) == true)
            {
                DrawButtonIcon(sb, null, compFollowButton);
                DrawButtonIcon(sb, null, compStayButton);
                DrawButtonIcon(sb, null, compLeaveMeButton);
            }
        }
        private void DrawButtonIcon(SpriteBatch sb, Texture2D icon, InteractButton button)
        {
            button.DrawButton(sb, fadeColor, scale);

            if (icon != null)
                sb.Draw(icon, button.Position, fadeColor, icon.Center(), 0f, scale, SpriteEffects.None, 0f);
        }

        public void DrawShadow(SpriteBatch sb, float depth)
        {
            sb.Draw(bg, position + new Vector2(0, 25), Color.Lerp(Color.Transparent, WorldLight.ShadowColor, animationLerp), bg.Center(), bgAngle, scale, SpriteEffects.None, depth);
            //sb.Draw(bg, position, Color.Black, bg.Center(), -bgAngle, scale, SpriteEffects.None, depth);
        }

        public void SetControlledEntity(BaseEntity entity)
        {
            this.controlledEntity = entity;
        }

        public bool IsClickingUI()
        {
            if (isActive == true)
                return Vector2.Distance(controls.MouseVector, position) <= 200;
            else
                return false;
        }
    }
}
