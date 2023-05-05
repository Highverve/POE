using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.IO;

namespace Pilgrimage_Of_Embers.ScreenEngine.Options
{
    public class ControlKeys
    {
        private Keys moveUp, moveDown, moveLeft, moveRight, activate, sprint, sneak, quickslot, jump, roll;
        private Keys takeScreenshot, typeMessage, loopQSLeft, loopQSRight, buttonTargetRight, buttonTargetLeft;
        private Keys useSelectedSoul, scrollSouls, scrollSpells, swapPrimary, swapOffhand, swapAmmo, swapWeaponUse;
        private Keys openInventory, openMagic, openSouls, openRumors, openStats, openSettings, pause;
        private Keys command0, command1, command2, command3, command4, command5, command6, command7, command8, command9;

        //Unmodifiable keys go here
        private Keys selectQS1, selectQS2, selectQS3, selectQS4, selectQS5, selectQS6, selectQS7, selectQS8, selectQS9, selectQS10;
        private Keys openTileEditor, openHelperScreen, openGameObjectEditor, openInfo, openConsole, debug, outputMap;

        #region Character movement controls
        public Keys MoveUp { get { return moveUp; } }
        public Keys MoveDown { get { return moveDown; } }
        public Keys MoveLeft { get { return moveLeft; } }
        public Keys MoveRight { get { return moveRight; } }

        public Keys Activate { get { return activate; } }
        public Keys Quickslot { get { return quickslot; } }
        public Keys Sprint { get { return sprint; } }
        public Keys Sneak { get { return sneak; } }
        public Keys Jump { get { return jump; } }
        public Keys Roll { get { return roll; } }
        #endregion

        #region Miscellaneous character controls
        public Keys TakeScreenshot { get { return takeScreenshot; } }
        public Keys TypeMessage { get { return typeMessage; } }

        public Keys LoopQSLeft { get { return loopQSLeft; } }
        public Keys LoopQSRight { get { return loopQSRight; } }
        public Keys ButtonTargetRight { get { return buttonTargetRight; } }
        public Keys ButtonTargetLeft { get { return buttonTargetLeft; } }
        #endregion

        #region Combat-related controls
        public Keys SwapWeaponUse { get { return swapWeaponUse; } }
        public Keys UseSelectedSoul { get { return useSelectedSoul; } }
        public Keys ScrollSouls { get { return scrollSouls; } }
        public Keys ScrollSpells { get { return scrollSpells; } }

        public Keys SwapPrimary { get { return swapPrimary; } }
        public Keys SwapOffhand { get { return swapOffhand; } }
        public Keys SwapAmmo { get { return swapAmmo; } }
        #endregion

        #region Shortcut controls
        public Keys OpenInventory { get { return openInventory; } }
        public Keys OpenMagic { get { return openMagic; } }
        public Keys OpenSouls { get { return openSouls; } }
        public Keys OpenRumors { get { return openRumors; } }
        public Keys OpenStats { get { return openStats; } }
        public Keys OpenSettings { get { return openSettings; } }
        public Keys Pause { get { return pause; } }
        #endregion

        #region Companion commands
        public Keys Command0 { get { return command0; } }
        public Keys Command1 { get { return command1; } }
        public Keys Command2 { get { return command2; } }
        public Keys Command3 { get { return command3; } }
        public Keys Command4 { get { return command4; } }
        public Keys Command5 { get { return command5; } }
        public Keys Command6 { get { return command6; } }
        public Keys Command7 { get { return command7; } }
        public Keys Command8 { get { return command8; } }
        public Keys Command9 { get { return command9; } }
        #endregion

        #region Unmodifiable controls
        public Keys SelectQS1 { get { return selectQS1; } }
        public Keys SelectQS2 { get { return selectQS2; } }
        public Keys SelectQS3 { get { return selectQS3; } }
        public Keys SelectQS4 { get { return selectQS4; } }
        public Keys SelectQS5 { get { return selectQS5; } }
        public Keys SelectQS6 { get { return selectQS6; } }
        public Keys SelectQS7 { get { return selectQS7; } }
        public Keys SelectQS8 { get { return selectQS8; } }
        public Keys SelectQS9 { get { return selectQS9; } }
        public Keys SelectQS10 { get { return selectQS10; } }

        public Keys OpenTileEditor { get { return openTileEditor; } }
        public Keys OpenHelperScreen { get { return openHelperScreen; } }
        public Keys OpenGameObjectEditor { get { return openGameObjectEditor; } }
        public Keys OpenInfo { get { return openInfo; } }
        public Keys OpenConsole { get { return openConsole; } }
        public Keys Debug { get { return debug; } }
        public Keys OutputMap { get { return outputMap; } }
        #endregion

        public ControlKeys()
        {
            SetDefaultControls();

            ReadData(GameSettings.FileName);
            LoadCustomControls();
        }

        public enum KeyEnum
        {
            MoveUp, MoveDown, MoveLeft, MoveRight, Activate, Quickslot, Sprint, Sneak, Jump, Roll,
            TakeScreenshot, TypeMessage, LoopQSLeft, LoopQSRight, ButtonTargetRight, ButtonTargetLeft,
            SwapWeaponUse, UseSelectedSoul, ScrollSouls, ScrollSpells, SwapPrimary, SwapOffhand, SwapAmmo,
            OpenInventory, OpenMagic, OpenSouls, OpenRumors, OpenStats, OpenSettings, Pause,
            Command0, Command1, Command2, Command3, Command4, Command5, Command6, Command7, Command8, Command9
        }
        public void SetControl(KeyEnum control, Keys key)
        {
            switch (control)
            {
                case KeyEnum.MoveUp: moveUp = key; break;
                case KeyEnum.MoveDown: moveDown = key; break;
                case KeyEnum.MoveLeft: moveLeft = key; break;
                case KeyEnum.MoveRight: moveRight = key; break;

                case KeyEnum.Activate: activate = key; break;
                case KeyEnum.Quickslot: quickslot = key; break;
                case KeyEnum.Sprint: sprint = key; break;
                case KeyEnum.Sneak: sneak = key; break;
                case KeyEnum.Jump: jump = key; break;

                case KeyEnum.TakeScreenshot: takeScreenshot = key; break;
                case KeyEnum.TypeMessage: typeMessage = key; break;

                case KeyEnum.LoopQSLeft: loopQSLeft = key; break;
                case KeyEnum.LoopQSRight: loopQSRight = key; break;
                case KeyEnum.ButtonTargetRight: buttonTargetRight = key; break;
                case KeyEnum.ButtonTargetLeft: buttonTargetLeft = key; break;

                case KeyEnum.SwapWeaponUse: swapWeaponUse = key; break;
                case KeyEnum.UseSelectedSoul: useSelectedSoul = key; break;
                case KeyEnum.ScrollSouls: scrollSouls = key; break;
                case KeyEnum.ScrollSpells: scrollSpells = key; break;
                case KeyEnum.SwapPrimary: swapPrimary = key; break;
                case KeyEnum.SwapOffhand: swapOffhand = key; break;
                case KeyEnum.SwapAmmo: swapAmmo = key; break;

                case KeyEnum.OpenInventory: openInventory = key; break;
                case KeyEnum.OpenMagic: openMagic = key; break;
                case KeyEnum.OpenSouls: openSouls = key; break;
                case KeyEnum.OpenRumors: openRumors = key; break;
                case KeyEnum.OpenStats: openStats = key; break;
                case KeyEnum.OpenSettings: openSettings = key; break;
                case KeyEnum.Pause: pause = key; break;

                case KeyEnum.Command0: command0 = key; break;
                case KeyEnum.Command1: command1 = key; break;
                case KeyEnum.Command2: command2 = key; break;
                case KeyEnum.Command3: command3 = key; break;
                case KeyEnum.Command4: command4 = key; break;
                case KeyEnum.Command5: command5 = key; break;
                case KeyEnum.Command6: command6 = key; break;
                case KeyEnum.Command7: command7 = key; break;
                case KeyEnum.Command8: command8 = key; break;
                case KeyEnum.Command9: command9 = key; break;
            }
        }
        public void SetDefaultControls()
        {
            moveUp = Keys.W;
            moveDown = Keys.S;
            moveLeft = Keys.A;
            moveRight = Keys.D;

            activate = Keys.E;
            quickslot = Keys.Q;
            sprint = Keys.LeftShift;
            sneak = Keys.LeftControl;
            jump = Keys.Space;
            roll = Keys.LeftAlt;

            typeMessage = Keys.Enter;
            takeScreenshot = Keys.F12;

            loopQSLeft = Keys.OemOpenBrackets;
            loopQSRight = Keys.OemCloseBrackets;
            buttonTargetLeft = Keys.G;
            buttonTargetRight = Keys.H;

            //Combat
            swapWeaponUse = Keys.Z;
            useSelectedSoul = Keys.X;
            scrollSouls = Keys.C;
            scrollSpells = Keys.V;

            swapPrimary = Keys.F;
            swapOffhand = Keys.CapsLock;
            swapAmmo = Keys.R;

            //UI Shortcuts
            openInventory = Keys.Tab;
            openMagic = Keys.M;
            openSouls = Keys.N;
            openRumors = Keys.B;
            openStats = Keys.OemComma;
            openSettings = Keys.OemPeriod;
            pause = Keys.P;

            command0 = Keys.NumPad0;
            command1 = Keys.NumPad1;
            command2 = Keys.NumPad2;
            command3 = Keys.NumPad3;
            command4 = Keys.NumPad4;
            command5 = Keys.NumPad5;
            command6 = Keys.NumPad6;
            command7 = Keys.NumPad7;
            command8 = Keys.NumPad8;
            command9 = Keys.NumPad9;

            selectQS1 = Keys.D1;
            selectQS2 = Keys.D2;
            selectQS3 = Keys.D3;
            selectQS4 = Keys.D4;
            selectQS5 = Keys.D5;
            selectQS6 = Keys.D6;
            selectQS7 = Keys.D7;
            selectQS8 = Keys.D8;
            selectQS9 = Keys.D9;
            selectQS10 = Keys.D0;

            openConsole = Keys.OemTilde;
            openTileEditor = Keys.F1;
            openGameObjectEditor = Keys.F2;
            openHelperScreen = Keys.F3;
            openInfo = Keys.F4;
            debug = Keys.F8;
            outputMap = Keys.F9;
        }
        public void SetControlsForTyping()
        {
            moveUp = Keys.None;
            moveDown = Keys.None;
            moveLeft = Keys.None;
            moveRight = Keys.None;

            activate = Keys.None;
            quickslot = Keys.None;
            sprint = Keys.None;
            sneak = Keys.None;
            jump = Keys.None;
            roll = Keys.None;

            typeMessage = Keys.Enter;
            takeScreenshot = Keys.None;

            loopQSLeft = Keys.None;
            loopQSRight = Keys.None;
            buttonTargetLeft = Keys.None;
            buttonTargetRight = Keys.None;

            //Combat
            swapWeaponUse = Keys.None;
            useSelectedSoul = Keys.None;
            scrollSouls = Keys.None;
            scrollSpells = Keys.None;

            swapPrimary = Keys.None;
            swapOffhand = Keys.None;
            swapAmmo = Keys.None;

            //UI Shortcuts
            openInventory = Keys.None;
            openMagic = Keys.None;
            openSouls = Keys.None;
            openRumors = Keys.None;
            openStats = Keys.None;
            openSettings = Keys.None;
            pause = Keys.None;

            command0 = Keys.None;
            command1 = Keys.None;
            command2 = Keys.None;
            command3 = Keys.None;
            command4 = Keys.None;
            command5 = Keys.None;
            command6 = Keys.None;
            command7 = Keys.None;
            command8 = Keys.None;
            command9 = Keys.None;

            selectQS1 = Keys.None;
            selectQS2 = Keys.None;
            selectQS3 = Keys.None;
            selectQS4 = Keys.None;
            selectQS5 = Keys.None;
            selectQS6 = Keys.None;
            selectQS7 = Keys.None;
            selectQS8 = Keys.None;
            selectQS9 = Keys.None;
            selectQS10 = Keys.None;
        }

        List<string> controlData = new List<string>();
        public void ReadData(string fileName)
        {
            controlData.Clear();

            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (!sr.EndOfStream)
                    {
                        controlData.Add(sr.ReadLine());
                    }
                }
            }
        }

        public StringBuilder SaveCustomControls()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("[Controls]");

            builder.AppendLine("Key " + KeyEnum.MoveUp.ToString() + " " + moveUp);
            builder.AppendLine("Key " + KeyEnum.MoveDown.ToString() + " " + moveDown);
            builder.AppendLine("Key " + KeyEnum.MoveLeft.ToString() + " " + moveLeft);
            builder.AppendLine("Key " + KeyEnum.MoveRight.ToString() + " " + moveRight);

            builder.AppendLine("Key " + KeyEnum.Activate.ToString() + " " + activate);
            builder.AppendLine("Key " + KeyEnum.Sprint.ToString() + " " + sprint);
            builder.AppendLine("Key " + KeyEnum.Sneak.ToString() + " " + sneak);
            builder.AppendLine("Key " + KeyEnum.Quickslot.ToString() + " " + quickslot);
            builder.AppendLine("Key " + KeyEnum.Jump.ToString() + " " + jump);
            builder.AppendLine("Key " + KeyEnum.Roll.ToString() + " " + roll);

            builder.AppendLine("Key " + KeyEnum.TakeScreenshot.ToString() + " " + takeScreenshot);
            builder.AppendLine("Key " + KeyEnum.TypeMessage.ToString() + " " + typeMessage);

            builder.AppendLine("Key " + KeyEnum.LoopQSLeft.ToString() + " " + loopQSLeft);
            builder.AppendLine("Key " + KeyEnum.LoopQSRight.ToString() + " " + loopQSRight);
            builder.AppendLine("Key " + KeyEnum.ButtonTargetLeft.ToString() + " " + buttonTargetRight);
            builder.AppendLine("Key " + KeyEnum.ButtonTargetRight.ToString() + " " + buttonTargetLeft);

            //Combat
            builder.AppendLine("Key " + KeyEnum.SwapWeaponUse.ToString() + " " + swapWeaponUse);
            builder.AppendLine("Key " + KeyEnum.UseSelectedSoul.ToString() + " " + useSelectedSoul);
            builder.AppendLine("Key " + KeyEnum.ScrollSouls.ToString() + " " + scrollSouls);
            builder.AppendLine("Key " + KeyEnum.ScrollSpells.ToString() + " " + scrollSpells);
            builder.AppendLine("Key " + KeyEnum.SwapPrimary.ToString() + " " + swapPrimary);
            builder.AppendLine("Key " + KeyEnum.SwapOffhand.ToString() + " " + swapOffhand);
            builder.AppendLine("Key " + KeyEnum.SwapAmmo.ToString() + " " + swapAmmo);

            builder.AppendLine("Key " + KeyEnum.OpenInventory.ToString() + " " + openInventory);
            builder.AppendLine("Key " + KeyEnum.OpenMagic.ToString() + " " + openMagic);
            builder.AppendLine("Key " + KeyEnum.OpenSouls.ToString() + " " + openSouls);
            builder.AppendLine("Key " + KeyEnum.OpenRumors.ToString() + " " + openRumors);
            builder.AppendLine("Key " + KeyEnum.OpenStats.ToString() + " " + openStats);
            builder.AppendLine("Key " + KeyEnum.OpenSettings.ToString() + " " + openSettings);
            builder.AppendLine("Key " + KeyEnum.Pause.ToString() + " " + pause);

            builder.AppendLine("Key " + KeyEnum.Command0.ToString() + " " + command0);
            builder.AppendLine("Key " + KeyEnum.Command1.ToString() + " " + command1);
            builder.AppendLine("Key " + KeyEnum.Command2.ToString() + " " + command2);
            builder.AppendLine("Key " + KeyEnum.Command3.ToString() + " " + command3);
            builder.AppendLine("Key " + KeyEnum.Command4.ToString() + " " + command4);
            builder.AppendLine("Key " + KeyEnum.Command5.ToString() + " " + command5);
            builder.AppendLine("Key " + KeyEnum.Command6.ToString() + " " + command6);
            builder.AppendLine("Key " + KeyEnum.Command7.ToString() + " " + command7);
            builder.AppendLine("Key " + KeyEnum.Command8.ToString() + " " + command8);
            builder.AppendLine("Key " + KeyEnum.Command9.ToString() + " " + command9);

            builder.AppendLine("[/Controls]");

            return builder;
        }

        public void LoadCustomControls()
        {
            if (controlData.Count > 0)
            {
                for (int i = 0; i < controlData.Count; i++)
                {
                    if (controlData[i].ToUpper().StartsWith("KEY"))
                    {
                        string[] words = controlData[i].Split(' ');

                        try
                        {
                            SetControl((KeyEnum)Enum.Parse(typeof(KeyEnum), words[1], true),
                                       (Keys)Enum.Parse(typeof(Keys), words[2], true));
                        }
                        catch
                        {
                            Logger.AppendLine("Error assigning '" + words[2] + "' to '" + words[1] + "' from settings.data. Ensure values are correct!");
                        }
                    }
                }
            }
        }
    }
    public class Controls
    {
        public KeyboardState CurrentKey, LastKey;
        public MouseState CurrentMS, LastMS;

        public Point MousePosition;
        public Vector2 MouseVector;

        private static ControlKeys currentControls = new ControlKeys();
        public ControlKeys CurrentControls { get { return currentControls; } }

        public static bool isTyping = false;

        public Controls() { }

        /// <summary>
        /// Update CurrentKey/CurrentMS to equate to Keyboard.GetState()/Mouse.GetState(). Add to the top of the Update method.
        /// </summary>
        public void UpdateCurrent()
        {
            CurrentKey = Keyboard.GetState();
            CurrentMS = Mouse.GetState();

            MousePosition = new Point(CurrentMS.X, CurrentMS.Y);
            MouseVector = new Vector2(MousePosition.X, MousePosition.Y);
        }
        /// <summary>
        /// Make the LastKey/LastMS equate to CurrentKey/CurrentMS. Add to the bottom of the Update() method.
        /// </summary>
        public void UpdateLast()
        {
            LastKey = CurrentKey;
            LastMS = CurrentMS;

            lastScrollValue = CurrentMS.ScrollWheelValue;
            scrollDirection = 0;
        }

        public bool IsKeyPressedOnce(Keys key)
        {
            if (CurrentKey.IsKeyDown(key) && LastKey.IsKeyUp(key))
                return true;
            else
                return false;
        }

        int time = 0;
        public bool IsKeyHeld(Keys key, int milliseconds, GameTime gt)
        {
            bool value = false;

            if (CurrentKey.IsKeyDown(key))
            {
                time += gt.ElapsedGameTime.Milliseconds;

                if (time >= milliseconds)
                    value = true;
            }
            else
            {
                time = 0;
                //Took out value = false; Line! Experimental!
            }

            return value;
        }

        public void KeyHold(Keys key, GameTime gt)
        {
            if (CurrentKey.IsKeyDown(key))
                keyHoldTime += gt.ElapsedGameTime.Milliseconds;

            if (LastKey.IsKeyUp(key))
                keyHoldTime = -1;
        }
        private int keyHoldTime;
        public int KeyHoldTime { get { return keyHoldTime; } }

        public Keys GetPressedKey()
        {
            Keys[] pressedKeys = CurrentKey.GetPressedKeys();
            if (pressedKeys.Length > 0)
                return pressedKeys[0]; //Return the first pressed key.
            else
                return Keys.None; //If there are no keys, return none.
        }

        public enum MouseButton
        {
            LeftClick,
            RightClick,
            MiddleClick,
            XButton1,
            XButton2
        }
        public bool IsClickedOnce(MouseButton button)
        {
            bool value = false;

            if (button == MouseButton.LeftClick)
            {
                if (CurrentMS.LeftButton == ButtonState.Pressed && LastMS.LeftButton == ButtonState.Released)
                    value = true;
                else
                    value = false;
            }
            else if (button == MouseButton.RightClick)
            {
                if (CurrentMS.RightButton == ButtonState.Pressed && LastMS.RightButton == ButtonState.Released)
                    value = true;
                else
                    value = false;
            }
            else if (button == MouseButton.MiddleClick)
            {
                if (CurrentMS.MiddleButton == ButtonState.Pressed && LastMS.MiddleButton == ButtonState.Released)
                    value = true;
                else
                    value = false;
            }

            return value;
        }

        int mouseClickTime = 0;
        int clickSpeed = 300;
        public bool IsDoubleClicked(GameTime gt, MouseButton button)
        {
            bool value = false;

            mouseClickTime += gt.ElapsedGameTime.Milliseconds;

            if (button == MouseButton.LeftClick)
            {
                if (CurrentMS.LeftButton == ButtonState.Pressed && LastMS.LeftButton == ButtonState.Released)
                {
                    if (mouseClickTime < clickSpeed)
                    {
                        value = true;
                        mouseClickTime = 0;
                    }
                    else
                        mouseClickTime = 0;
                }
            }
            else if (button == MouseButton.RightClick)
            {
                if (CurrentMS.RightButton == ButtonState.Pressed && LastMS.RightButton == ButtonState.Released)
                {
                    if (mouseClickTime < clickSpeed)
                    {
                        value = true;
                        mouseClickTime = 0;
                    }
                    else
                        mouseClickTime = 0;
                }
            }
            else if (button == MouseButton.MiddleClick)
            {
                if (CurrentMS.MiddleButton == ButtonState.Pressed && LastMS.MiddleButton == ButtonState.Released)
                {
                    if (mouseClickTime < clickSpeed)
                    {
                        value = true;
                        mouseClickTime = 0;
                    }
                    else
                        mouseClickTime = 0;
                }
            }

            return value;
        }

        int mouseHoldTime = 0;
        public bool IsMouseHeld(GameTime gt, MouseButton button, int milliseconds)
        {
            bool returnBool = false;

            if (button == MouseButton.LeftClick)
            {
                if (CurrentMS.LeftButton == ButtonState.Pressed)
                {
                    mouseHoldTime += gt.ElapsedGameTime.Milliseconds;

                    if (mouseHoldTime >= milliseconds)
                    {
                        returnBool = true;
                    }
                }
                else
                {
                    mouseHoldTime = 0;
                }
            }
            else if (button == MouseButton.RightClick)
            {
                if (CurrentMS.RightButton == ButtonState.Pressed)
                {
                    mouseHoldTime += gt.ElapsedGameTime.Milliseconds;

                    if (mouseHoldTime >= milliseconds)
                    {
                        returnBool = true;
                    }
                }
                else
                {
                    mouseHoldTime = 0;
                }
            }
            else if (button == MouseButton.MiddleClick)
            {
                if (CurrentMS.MiddleButton == ButtonState.Pressed)
                {
                    mouseHoldTime += gt.ElapsedGameTime.Milliseconds;

                    if (mouseHoldTime >= milliseconds)
                    {
                        returnBool = true;
                    }
                }
                else
                {
                    mouseHoldTime = 0;
                }
            }

            return returnBool;
        }
        public bool IsMouseHeld(GameTime gt, MouseButton button, int min, int max)
        {
            bool returnBool = false;

            if (button == MouseButton.LeftClick)
            {
                if (CurrentMS.LeftButton == ButtonState.Pressed)
                {
                    mouseHoldTime += gt.ElapsedGameTime.Milliseconds;

                    if (mouseHoldTime >= min && mouseHoldTime <= max)
                    {
                        returnBool = true;
                    }
                }
                else
                {
                    mouseHoldTime = 0;
                }
            }
            else if (button == MouseButton.RightClick)
            {
                if (CurrentMS.RightButton == ButtonState.Pressed)
                {
                    mouseHoldTime += gt.ElapsedGameTime.Milliseconds;

                    if (mouseHoldTime >= min && mouseHoldTime <= max)
                    {
                        returnBool = true;
                    }
                }
                else
                {
                    mouseHoldTime = 0;
                }
            }
            else if (button == MouseButton.MiddleClick)
            {
                if (CurrentMS.MiddleButton == ButtonState.Pressed)
                {
                    mouseHoldTime += gt.ElapsedGameTime.Milliseconds;

                    if (mouseHoldTime >= min && mouseHoldTime <= max)
                    {
                        returnBool = true;
                    }
                }
                else
                {
                    mouseHoldTime = 0;
                }
            }

            return returnBool;
        }
        public bool IsMouseHeld(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftClick: return CurrentMS.LeftButton == ButtonState.Pressed;
                case MouseButton.MiddleClick: return CurrentMS.MiddleButton == ButtonState.Pressed;
                case MouseButton.RightClick: return CurrentMS.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1: return CurrentMS.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2: return CurrentMS.XButton2 == ButtonState.Pressed;
            }

            return false;
        }

        public bool IsMouseDown(MouseButton button)
        {
            bool returnBool = false;

            if (button == MouseButton.LeftClick)
            {
                if (CurrentMS.LeftButton == ButtonState.Pressed)
                    returnBool = true;
            }
            else if (button == MouseButton.RightClick)
            {
                if (CurrentMS.RightButton == ButtonState.Pressed)
                    returnBool = true;
            }
            else if (button == MouseButton.MiddleClick)
            {
                if (CurrentMS.MiddleButton == ButtonState.Pressed)
                    returnBool = true;
            }
            else if (button == MouseButton.XButton1)
            {
                if (CurrentMS.XButton1 == ButtonState.Pressed)
                    returnBool = true;
            }
            else if (button == MouseButton.XButton2)
            {
                if (CurrentMS.XButton2 == ButtonState.Pressed)
                    returnBool = true;
            }

            return returnBool;
        }

        int lastScrollValue = 0, scrollDirection = 0;
        public int CheckScrollDirection()
        {
            if (CurrentMS.ScrollWheelValue < lastScrollValue)
                scrollDirection = -1;
            if (CurrentMS.ScrollWheelValue > lastScrollValue)
                scrollDirection = 1;

            return scrollDirection;
        }

        public void ResetControls()
        {
            currentControls.SetDefaultControls();
        }
        public void SetDefaultControls()
        {
            currentControls.LoadCustomControls();
        }
        public void SetControlsForTyping()
        {
            currentControls.SetControlsForTyping();
        }

        public bool IsMouseOnScreen()
        {
            return (MousePosition.X >= 0) && (MousePosition.Y >= 0) &&
                   (MousePosition.X < GameSettings.WindowResolution.X) &&
                   (MousePosition.Y < GameSettings.WindowResolution.Y);
        }

        public string KeyString(Keys key)
        {
            if (key == Keys.OemComma) return ",";
            else if (key == Keys.OemPeriod) return ".";
            else if (key == Keys.OemSemicolon) return ";";
            else if (key == Keys.OemQuotes) return "\"";
            else if (key == Keys.OemQuestion) return "?";
            else if (key == Keys.OemPlus) return "+";
            else if (key == Keys.OemMinus) return "-";
            else if (key == Keys.OemTilde) return "~";
            else if (key == Keys.OemSemicolon) return ";";
            else if (key == Keys.OemPipe) return "|";
            else if (key == Keys.OemCloseBrackets) return "}";
            else if (key == Keys.OemOpenBrackets) return "{";
            else
                return key.ToString();
        }
    }
}
