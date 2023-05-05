using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.ScreenEngine.Notification;
using Pilgrimage_Of_Embers.Debugging;

namespace Pilgrimage_Of_Embers.ScreenEngine.RumorsNotes
{
    public class RumorsInterface
    {
        //To-Do: Make sidebar open based on gt.ElapsedTime.TotalSeconds;
        private List<Rumor> rumors = new List<Rumor>();

        public enum RumorCompleteStatus { Completed, NotCompleted, NotAdded }

        private SpriteFont font, headerFont;
        private Texture2D bg, paneBG, paneRight, rumorBar, rumorBarSelect, status, statusActive, statusResolved, statusDismissed, newIcon;

        private Point currentFrame = Point.Zero, sheetSize = new Point(8, 1), frameSize = new Point(9, 9);
        BasicAnimation anim = new BasicAnimation();

        Controls controls = new Controls();

        private Vector2 mouseDragOffset;
        Vector2 screenPosition, offset;
        float positionY = 200;
        int longBounds, paneSizeIncrease;
        int PaneSizeIncrease { get { return paneSizeIncrease; } set { paneSizeIncrease = (int)MathHelper.Clamp(value, 0, 288); } } //288 is paneBG.Width

        Vector2 Offset { get { return offset; } set { offset = new Vector2(MathHelper.Clamp(value.X, 0, GameSettings.WindowResolution.X - 670), MathHelper.Clamp(value.Y, 0, GameSettings.WindowResolution.Y - 520)); } }

        Rectangle scissorRect, dragArea, uiRect;
        RasterizerState rState = new RasterizerState() { ScissorTestEnable = true };
        RasterizerState rStateOff = new RasterizerState() { ScissorTestEnable = false };

        private NotificationManager notification;
        private DebugManager debug;
        private ScreenManager screens;

        private Color resolvedColor = Color.LightGreen, dismissedColor = new Color(145, 62, 62, 255);

        private Texture2D windowButton, windowButtonHover, hintIcon, hideIcon;
        private Rectangle hintRect, hideRect;
        private bool isHintHover, isHideHover;

        /*
           

            //Update

            //Draw

        */

        public bool IsActive { get; set; }

        public void SetReferences(NotificationManager notification, DebugManager debug, ScreenManager screens)
        {
            this.notification = notification;
            this.debug = debug;
            this.screens = screens;
        }

        public void AddRumor(int id, bool bypassNotification = false)
        {
            for (int i = 0; i < RumorDatabase.Rumors.Count; i++)
            {
                if (RumorDatabase.Rumors[i].ID == id && !rumors.Contains(RumorDatabase.Rumors[i]))
                {
                    rumors.Add(RumorDatabase.Rumors[i]);
                    UpdateHeader();

                    if (bypassNotification == false)
                        notification.AddNotification(NotificationManager.IconType.Rumors, "Added \"" + rumors[i].FullHeader +"\"");
                }
            }
        }
        public void ResolveRumor(int id)
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (rumors[i].ID == id)
                {
                    rumors[i].State = Rumor.RumorState.Resolved;
                    notification.AddNotification(NotificationManager.IconType.Rumors, "Resolved \"" + rumors[i].FullHeader + "\"");
                }
            }
        }
        public void DismissRumor(int id)
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (rumors[i].ID == id)
                {
                    rumors[i].State = Rumor.RumorState.Dismissed;
                    notification.AddNotification(NotificationManager.IconType.Rumors, "Dismissed \"" + rumors[i].FullHeader + "\"");
                }
            }
        }
        public Rumor.RumorState GetRumorState(int id)
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (rumors[i].ID == id)
                {
                    return rumors[i].State;
                }
            }
            return Rumor.RumorState.Active;
        }    
        public RumorCompleteStatus IsRumorCompleted(int id)
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (rumors[i].ID == id)
                {
                    if (rumors[i].IsCompleted == true) return RumorCompleteStatus.Completed;
                    else return RumorCompleteStatus.NotCompleted;
                }
            }
            return RumorCompleteStatus.NotAdded;
        }
        public bool HasRumor(int id)
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (rumors[i].ID == id)
                {
                    return true;
                }
            }

            return false;
        }
        
        string directory = "Interface/Rumors/";
        public void Load(ContentManager cm)
        {
            font = cm.Load<SpriteFont>("Fonts/regularOutlined");
            headerFont = cm.Load<SpriteFont>("Fonts/boldOutlined");

            bg = cm.Load<Texture2D>(directory + "rumorBackground");
            rumorBar = cm.Load<Texture2D>(directory + "rumorBar");
            rumorBarSelect = cm.Load<Texture2D>(directory + "rumorBarSelect");
            status = cm.Load<Texture2D>(directory + "status"); //The status background
            statusActive = cm.Load<Texture2D>(directory + "statusActive");
            statusResolved = cm.Load<Texture2D>(directory + "statusComplete");
            statusDismissed = cm.Load<Texture2D>(directory + "statusIncomplete");
            newIcon = cm.Load<Texture2D>("Interface/Shared/newIcon");

            paneBG = cm.Load<Texture2D>(directory + "paneMiddle");
            paneRight = cm.Load<Texture2D>(directory + "paneRight");

            windowButton = cm.Load<Texture2D>("Interface/Global/windowButton");
            windowButtonHover = cm.Load<Texture2D>("Interface/Global/windowButtonHover");

            hintIcon = cm.Load<Texture2D>("Interface/Global/hintIcon");
            hideIcon = cm.Load<Texture2D>("Interface/Global/hideIcon");

            offset = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - (bg.Height / 2));
            mouseDragOffset = new Vector2(bg.Width / 2, 12);

            //AddRumor(1);
            //AddRumor(2);
            //ResolveRumor(1);
        }

        private string hints = "Rumor Tips:\n\n" +
                                "The plain-colored are unsolved rumors, green are resolved rumors,\n" +
                                "and red are dismissed rumors. Some rumors are true, others may not be.";

        public void Update(GameTime gt)
        {
            screenPosition = new Vector2(offset.X, offset.Y);

            uiRect = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, bg.Width + PaneSizeIncrease, bg.Height);
            dragArea = new Rectangle((int)screenPosition.X + 94, (int)screenPosition.Y, 166, 20);

            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(controls.CurrentControls.OpenRumors))
            {
                IsActive = !IsActive;
                Logger.AppendLine("Opened rumors UI");
            }

            if (IsActive == true)
            {
                currentFrame = anim.FramePosition(gt, 55, sheetSize, true);

                if (isDragging == false)
                    CheckRumorSelect();

                ChangePaneSize(gt);

                //Window Buttons
                hintRect = new Rectangle((int)screenPosition.X + 256, (int)screenPosition.Y, windowButton.Width - 20, windowButton.Height);
                hideRect = new Rectangle((int)screenPosition.X + 283, (int)screenPosition.Y, windowButton.Width - 20, windowButton.Height);

                if (hintRect.Contains(controls.MousePosition))
                {
                    isHintHover = true;

                    ToolTip.RequestStringAssign(hints);

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                    }
                }
                else
                    isHintHover = false;

                if (hideRect.Contains(controls.MousePosition))
                {
                    isHideHover = true;
                    ToolTip.RequestStringAssign("Hide Rumors");

                    if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                    {
                        screens.PlaySound("Button Click 1");
                        IsActive = false;
                    }
                }
                else
                    isHideHover = false;
            }

            controls.UpdateLast();
        }
        private void CheckRumorSelect()
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (controls.IsClickedOnce(Controls.MouseButton.LeftClick))
                {
                    if (rumors[i].SelectRectangle.Contains(controls.MousePosition))
                    {
                        rumors[i].IsSelected = true;
                        screens.PlaySound("Item Select");

                        if (rumors[i].IsNew == true)
                            RumorNewStatus(i);

                        UpdateHeader();
                    }
                    else
                        rumors[i].IsSelected = false;
                }
            }
        }
        private bool IsRumorSelected()
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (rumors[i].IsSelected == true)
                    return true;
            }

            return false;
        }
        private int SelectedRumor()
        {
            for (int i = 0; i < rumors.Count; i++)
            {
                if (rumors[i].IsSelected == true)
                    return i;
            }
            return -1;
        }

        public StringBuilder SaveData(string tag)
        {
            StringBuilder temp = new StringBuilder();

            temp.AppendLine(tag);

            for (int i = 0; i < rumors.Count; i++)
                temp.AppendLine(rumors[i].SaveData().ToString());

            temp.AppendLine(tag.Replace("[", "[/"));

            return temp;
        }
        public void LoadData(List<string> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                try
                {
                    string[] idSplit = data[i].Split(' ');
                    int id = int.Parse(idSplit[0]);
                    AddRumor(id, true);
                    rumors.Last().LoadData(data[i]);
                }
                catch (Exception e)
                {
                    debug.OutputError("ERROR: Bad data line(RumorData): " + e.Message);
                }
            }
        }
        public void ResetRumors()
        {
            rumors.Clear();
        }

        int scrollValue = 0;
        private void ScrollRumors()
        {
            if (controls.CurrentMS.ScrollWheelValue < scrollValue)
            {
                positionY -= 8;
            }
            else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
            {
                positionY += 8;
            }
            scrollValue = controls.CurrentMS.ScrollWheelValue;
        }
        private void CheckBounds()
        {
            longBounds = -((rumors.Count * (rumorBar.Height + 2)) - (bg.Height - 4));

            if (positionY >= 48)
                positionY = 48;
            else if (positionY <= longBounds)
                positionY = longBounds;
        }
        private bool isDragging = false;
        private void DragWindow()
        {
            if (dragArea.Contains(controls.MousePosition))
                screens.SetCursorState(Cursor.CursorState.Moving);

            if (dragArea.Contains(controls.MousePosition) && controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                isDragging = true;
            else if (controls.CurrentMS.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                isDragging = false;

            if (isDragging == true)
            {
                Offset = controls.MouseVector - mouseDragOffset;
                screens.SetCursorState(Cursor.CursorState.Move);
            }
        }

        private void RumorNewStatus(int index)
        {
            rumors[index].IsNew = false;
        }

        StringBuilder header = new StringBuilder("Rumors");

        private void UpdateHeader()
        {
            if (rumors.Count > 0)
            {
                int totalNew = 0;

                for (int i = 0; i < rumors.Count; i++)
                {
                    if (rumors[i].IsNew == true)
                        totalNew++;
                }

                header.Clear();

                if (totalNew > 0)
                    header.Append("Rumors(" + totalNew + " new)");
                else
                    header.Append("Rumors");
            }
        }

        Color headerBarColor;
        Color defaultColor = Color.LightGray;
        Color goldColor = new Color(182, 191, 137, 255);

        public void Draw(SpriteBatch sb)
        {
            if (IsActive == true)
            {
                controls.UpdateLast();
                controls.UpdateCurrent();

                DragWindow();

                if (rumors.Count >= 19)
                    ScrollRumors();

                CheckBounds();

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                sb.Draw(bg, screenPosition, Color.White, Vector2.Zero, 0f, 1f);
                sb.DrawString(headerFont, header.ToString(), new Vector2(screenPosition.X + (bg.Width / 2), screenPosition.Y + 12), header.ToString().LineCenter(headerFont), goldColor, 1f);

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

                DrawPane(sb);

                sb.End();


                scissorRect = new Rectangle((int)screenPosition.X, (int)screenPosition.Y + 46, bg.Width, bg.Height - 51);

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, rState);
                sb.GraphicsDevice.ScissorRectangle = scissorRect;

                for (int i = 0; i < rumors.Count; i++)
                {
                    rumors[i].SelectRectangle = new Rectangle((int)screenPosition.X + 17, (int)screenPosition.Y + (i * (rumorBar.Height + 2)) + (int)positionY, rumorBar.Width, rumorBar.Height);

                    if (rumors[i].IsSelected == true)
                        sb.Draw(rumorBarSelect, new Vector2(rumors[i].SelectRectangle.X, rumors[i].SelectRectangle.Y), Color.White, Vector2.Zero, 0f, 1f);
                    else
                        sb.Draw(rumorBar, new Vector2(rumors[i].SelectRectangle.X, rumors[i].SelectRectangle.Y), Color.White, Vector2.Zero, 0f, 1f);

                    CheckColorState(i);

                    sb.DrawString(font, rumors[i].Header, new Vector2(rumors[i].SelectRectangle.X + 6, rumors[i].SelectRectangle.Y + 5), Vector2.Zero, headerBarColor, 1f);

                    if (rumors[i].IsNew == true)
                        sb.Draw(newIcon, new Vector2(rumors[i].SelectRectangle.X + 1, rumors[i].SelectRectangle.Y + 1), new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

                    sb.Draw(status, new Vector2(rumors[i].SelectRectangle.X + (rumorBar.Width - status.Width - 4), rumors[i].SelectRectangle.Y + 4), Color.White, Vector2.Zero, 0f, 1f);

                    if (rumors[i].State == Rumor.RumorState.Resolved)
                        sb.Draw(statusResolved, new Vector2(rumors[i].SelectRectangle.X + (rumorBar.Width - status.Width - 4), rumors[i].SelectRectangle.Y + 4), Color.White, Vector2.Zero, 0f, 1f);
                    else if (rumors[i].State == Rumor.RumorState.Dismissed)
                        sb.Draw(statusDismissed, new Vector2(rumors[i].SelectRectangle.X + (rumorBar.Width - status.Width - 4), rumors[i].SelectRectangle.Y + 4), Color.White, Vector2.Zero, 0f, 1f);
                    else if (rumors[i].State == Rumor.RumorState.Active)
                        sb.Draw(statusActive, new Vector2(rumors[i].SelectRectangle.X + (rumorBar.Width - status.Width - 4), rumors[i].SelectRectangle.Y + 4), Color.White, Vector2.Zero, 0f, 1f);
                }

                sb.GraphicsDevice.RasterizerState = rStateOff;
                sb.End();
            }
        }

        float textColorIntensity = 0f;
        int index = 0;
        Color textColorInside, bodyColorInside, textColorOutside, iconColor;
        private void DrawPane(SpriteBatch sb)
        {
            sb.Draw(paneBG, new Vector2(screenPosition.X + bg.Width - 10, screenPosition.Y + (bg.Height / 7)), new Rectangle(0, 0, PaneSizeIncrease, paneBG.Height), Color.White);
            sb.Draw(paneRight, new Vector2((screenPosition.X + bg.Width - 10) + PaneSizeIncrease, screenPosition.Y + (bg.Height / 7) + 1), Color.White);

            if (IsRumorSelected() == true)
            {
                index = SelectedRumor();

                CheckColorState(index);

                sb.DrawString(headerFont, rumors[index].FullHeader.WrapText(headerFont, 280), new Vector2(screenPosition.X + bg.Width, screenPosition.Y + (bg.Height / 7) + 10), Vector2.Zero, textColorInside, 1f);

                if (rumors[index].State == Rumor.RumorState.Active)
                    sb.DrawString(font, rumors[index].Body.WrapText(font, 280), new Vector2(screenPosition.X + bg.Width, screenPosition.Y + (bg.Height / 4) + 23), Vector2.Zero, bodyColorInside, 1f);
                else if (rumors[index].State == Rumor.RumorState.Resolved)
                    sb.DrawString(font, rumors[index].ResolvedBody.WrapText(font, 280), new Vector2(screenPosition.X + bg.Width, screenPosition.Y + (bg.Height / 4) + 23), Vector2.Zero, bodyColorInside, 1f);
                else if (rumors[index].State == Rumor.RumorState.Dismissed)
                    sb.DrawString(font, rumors[index].DismissedBody.WrapText(font, 280), new Vector2(screenPosition.X + bg.Width, screenPosition.Y + (bg.Height / 4) + 23), Vector2.Zero, bodyColorInside, 1f);

                sb.DrawString(headerFont, rumors[index].State.ToString(), new Vector2(screenPosition.X + bg.Width + (PaneSizeIncrease / 2) - 10, screenPosition.Y + paneBG.Height + 50), rumors[index].State.ToString().LineCenter(headerFont), textColorInside, 1f);

                if (rumors[index].State == Rumor.RumorState.Resolved)
                    sb.Draw(statusResolved, new Vector2(screenPosition.X + bg.Width + (PaneSizeIncrease / 2) - headerFont.MeasureString(rumors[index].State.ToString()).X + 10, screenPosition.Y + paneBG.Height + 50), iconColor, statusResolved.Center(), 0f, 1f);
                else if (rumors[index].State == Rumor.RumorState.Dismissed)
                    sb.Draw(statusDismissed, new Vector2(screenPosition.X + bg.Width + (PaneSizeIncrease / 2) - headerFont.MeasureString(rumors[index].State.ToString()).X + 12, screenPosition.Y + paneBG.Height + 50), iconColor, statusDismissed.Center(), 0f, 1f);
                else if (rumors[index].State == Rumor.RumorState.Active)
                    sb.Draw(statusActive, new Vector2(screenPosition.X + bg.Width + (PaneSizeIncrease / 2) - headerFont.MeasureString(rumors[index].State.ToString()).X + 2, screenPosition.Y + paneBG.Height + 50), iconColor, statusActive.Center(), 0f, 1f);
            }

            textColorIntensity = MathHelper.Clamp(textColorIntensity, 0f, 1f);

            textColorInside = Color.Lerp(Color.Transparent, headerBarColor, textColorIntensity);
            textColorOutside = Color.Lerp(Color.Transparent, Color.Black, textColorIntensity - .2f);
            bodyColorInside = Color.Lerp(Color.Transparent, Color.LightGray, textColorIntensity);
            iconColor = Color.Lerp(Color.Transparent, Color.White, textColorIntensity);
        }
        private void ChangePaneSize(GameTime gt)
        {
            if (IsRumorSelected() == true)
            {
                PaneSizeIncrease += (int)(1600 * (float)gt.ElapsedGameTime.TotalSeconds);

                if (PaneSizeIncrease >= 250)
                    textColorIntensity += 25f * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                PaneSizeIncrease -= (int)(1200 * (float)gt.ElapsedGameTime.TotalSeconds);
                textColorIntensity -= 25f * (float)gt.ElapsedGameTime.TotalSeconds;
            }
        }
        private void CheckColorState(int index)
        {
            if (rumors[index].State == Rumor.RumorState.Active)
                headerBarColor = defaultColor;
            else if (rumors[index].State == Rumor.RumorState.Resolved)
                headerBarColor = resolvedColor;
            else
                headerBarColor = dismissedColor;
        }

        public bool IsClickingUI()
        {
            if (IsActive == true)
                return isDragging == true || uiRect.Contains(controls.MousePosition);
            else
                return false;
        }
        public void ResetPosition()
        {
            offset = new Vector2(GameSettings.VectorCenter.X - (bg.Width / 2), GameSettings.VectorCenter.Y - bg.Height / 2);
        }
    }
}
