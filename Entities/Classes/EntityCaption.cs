using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Helper_Classes;
using System.Text.RegularExpressions;

namespace Pilgrimage_Of_Embers.Entities
{
    /// <summary>
    /// Words that appear above an entity's head.
    /// </summary>
    public class EntityCaption
    {
        /* To-Do: Add in the language being worked on for creatures/monsters? Use custom symbols.
         * 
         * 
         */

        bool getNewCaption = true, isSentCaption = false;
        /// <summary>
        /// If this is an NPC, allow the player to hear them always. If not, then 
        /// </summary>

        public enum DisplayState { Always, OnSight } //If the player sees text, he could easily know where the enemies are at!
        DisplayState state { get; set; }

        const int baseMilliseconds = 2000; //Time spacing between the current message and the next message.
        int randomTime, messageTime, currentTime;

        string currentMessage;
        public string CurrentMessage { get { return currentMessage; } }

        Random random;
        SpriteFont font;

        bool isPixelAssigned = false;
        Texture2D pixel;

        Vector2 offset;

        List<string> queue = new List<string>();

        public EntityCaption(Vector2 Offset)
        {
            random = new Random(Guid.NewGuid().GetHashCode());
            offset = Offset;
        }

        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/boldOutlined");
        }

        public void Update(GameTime gt)
        {
            CaptionTime(gt);

            if (isSentCaption == false)
            {
                if (queue.Count == 0) //If the queued text is empty, pick a message
                {
                }
                else if (getNewCaption == true)
                {
                    SendCaption(queue.FirstOrDefault(), false);
                    queue.RemoveAt(0); //Once sent, dispose of first message.
                }
            }
        }
        private void CaptionTime(GameTime gt)
        {
            if (getNewCaption == false)
            {
                currentTime += gt.ElapsedGameTime.Milliseconds;

                if (currentTime >= messageTime)
                {
                    currentMessage = string.Empty;
                }

                if (currentTime >= randomTime)
                {
                    isSentCaption = false;
                    getNewCaption = true;
                }
            }
        }
        private void ResetValues()
        {
            messageTime = 0;
            randomTime = 0;
            currentTime = 0;
        }

        public void Reset()
        {
            currentMessage = string.Empty;
            ResetValues();
        }

        /// <summary>
        /// Send a new caption, interrupting the current caption
        /// </summary>
        /// <param name="captionText"></param>
        public void SendCaptionImmediate(string captionText, bool isInfiniteTime)
        {
            if (!string.IsNullOrEmpty(captionText)) //Prevents sending empty messages
            {
                ResetValues();

                isSentCaption = true;

                    if (currentMessage != captionText) //Prevents sending the exact same message
                        currentMessage = Regex.Unescape(captionText);

                if (isInfiniteTime == false)
                    SetTimesNoRandom();
                else
                    SetTimesInfinite();

                getNewCaption = false;
            }
        }
        public void SendCaptionDelayed(string captionText, bool isInfiniteTime)
        {
            SendCaptionImmediate("          ", false);
            QueueCaption(captionText);

            if (isInfiniteTime == true)
                SetTimesInfinite();
        }
        private void SendCaption(string captionText, bool randomTime)
        {
            ResetValues();

            isSentCaption = true;

            if (!string.IsNullOrEmpty(captionText))
                currentMessage = Regex.Unescape(captionText);
            else
                currentMessage = "WARNING: Empty caption sent.";

            if (randomTime == false)
                SetTimesNoRandom();
            else
                SetTimes();

            getNewCaption = false;
        }
        public void QueueCaption(string captionText)
        {
            if (!string.IsNullOrEmpty(captionText))
            {
                if (queue.LastOrDefault() != captionText)
                    queue.Add(currentMessage = Regex.Unescape(captionText));
            }
        }

        private void SetTimes()
        {
            messageTime += (currentMessage.Length * 100) + baseMilliseconds;

            randomTime = random.Next(2500, 10000); //between 2.5s and 10s
            randomTime += messageTime;
        }
        private void SetTimesNoRandom()
        {
            messageTime += (currentMessage.Length * 100) + baseMilliseconds;
            randomTime = messageTime;
        }
        private void SetTimesInfinite()
        {
            messageTime = 1000000000; //No one is going to wait this long, so it's infinite.
            randomTime = messageTime;
        }

        private Color insideBox = Color.Lerp(Color.Transparent, ColorHelper.Charcoal, 1f);

        public void Draw(SpriteBatch sb, Vector2 location)
        {
            if (isPixelAssigned == false)
                pixel = TextureHelper.CreatePixel(sb, ref isPixelAssigned);

            if (!string.IsNullOrEmpty(currentMessage))
            {
                sb.DrawBoxBordered(pixel, new Rectangle((int)(location.X + offset.X) - (int)(currentMessage.LineCenter(font).X + 4),
                                                        (int)(location.Y + offset.Y) - (int)(currentMessage.LineCenter(font).Y + 4),
                                                        (int)font.MeasureString(currentMessage).X + 10, (int)font.MeasureString(currentMessage).Y + 8),
                                                        insideBox, Color.Black, 1f - .0002f);

                sb.DrawBoxBordered(pixel, new Rectangle((int)(location.X + offset.X) - (int)(currentMessage.LineCenter(font).X + 2),
                                        (int)(location.Y + offset.Y) - (int)(currentMessage.LineCenter(font).Y + 2),
                                        (int)font.MeasureString(currentMessage).X + 6, (int)font.MeasureString(currentMessage).Y + 4),
                                        insideBox, ColorHelper.Charcoal, 1f - .00015f);

                sb.DrawString(font, currentMessage, location + offset, currentMessage.LineCenter(font), ColorHelper.D_Orange, 1f, 1f - .0001f);
            }
        }

        public bool IsTalking
        {
            get { return !string.IsNullOrEmpty(currentMessage); }
        }
    }
}
