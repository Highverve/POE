using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Entities;

namespace Pilgrimage_Of_Embers.Entities.Types.Player
{
    public class TypingInterface
    {
        private Texture2D end, middle;
        private SpriteFont font;

        Controls controls = new Controls();
        TextInput input = new TextInput();

        private bool isPlayerTyping = false;
        public bool IsPlayerTyping { get { return isPlayerTyping; } set { isPlayerTyping = value; } }

        private int barLength = 0;
        private int BarLength { get { return barLength; } set { barLength = (int)MathHelper.Clamp(value, 0f, 2000); } }

        private BaseEntity entity;
        private Vector2 offset;

        Random random;

        public TypingInterface()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
        }
        public void SetReferences(BaseEntity entity, Vector2 offset)
        {
            this.entity = entity;
            this.offset = offset;
        }

        private string directory = "Interface/Typing/";
        public void Load(ContentManager cm)
        {
            end = cm.Load<Texture2D>(directory + "end");
            middle = cm.Load<Texture2D>(directory + "middle");
            font = cm.Load<SpriteFont>("Fonts/boldOutlined");
        }
        public void Update(GameTime gt)
        {
            controls.UpdateLast();
            controls.UpdateCurrent();

            if (IsPlayerTyping == false && GameSettings.IsDebugging == false)
            {
                if (controls.IsKeyPressedOnce(controls.CurrentControls.TypeMessage))
                {
                    isPlayerTyping = true;
                    controls.SetControlsForTyping();
                }
            }
            else
            {
                input.UpdateInput(gt);

                if (input.text.Length == 0)
                {
                    if (controls.IsKeyPressedOnce(controls.CurrentControls.TypeMessage))
                    {
                        isPlayerTyping = false;
                        controls.SetDefaultControls();
                    }
                }
                else if (input.text.Length > 0)
                {
                    barLength = (int)font.MeasureString(input.ReturnText.ToString()).X / 2 + 5;

                    if (controls.IsKeyPressedOnce(controls.CurrentControls.TypeMessage))
                    {
                        string subMessage = input.ReturnText.ToString();

                        entity.CAPTION_SendImmediate(subMessage);
                        entity.SendMessage(entity, null, entity.Faction, 1000, 10, "Chat", "$" + subMessage, true, isRandomCooldown: false);

                        input.DeleteAllText();
                        isPlayerTyping = false;
                        controls.SetDefaultControls();
                        BarLength = 0;
                    }
                }
            }

            if (GameSettings.IsDebugging == true)
            {
                input.DeleteAllText();
                isPlayerTyping = false;
                BarLength = 0;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (IsPlayerTyping == true)
            {
                sb.Draw(end, offset - new Vector2(BarLength, 0), Color.White, new Vector2(end.Width, 0), 0f, 1f, SpriteEffects.None, .99f);
                sb.Draw(end, offset + new Vector2(BarLength, 0), Color.White, Vector2.Zero, 0f, 1f, SpriteEffects.FlipHorizontally, .99f);
                sb.Draw(middle, new Rectangle((int)offset.X - barLength, (int)offset.Y + 13, barLength * 2, middle.Height), Color.White);

                sb.DrawString(font, input.ReturnText.ToString(), offset + new Vector2(0, 5), input.ReturnText.ToString().LineCenter(font), Color.White, 1f);
            }
        }
    }
}
