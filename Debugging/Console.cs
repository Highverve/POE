using System;
using System.Collections.Generic;
using System.Text;
using Pilgrimage_Of_Embers.Helper_Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine;
using System.Windows.Forms;

namespace Pilgrimage_Of_Embers.Debugging
{
    public class Console
    {
        SpriteFont font;
        Texture2D pixel;

        Controls controls = new Controls();
        TextInput input = new TextInput();

        StringBuilder displayText, enteredText;

        List<string> outputText = new List<string>();
        List<string> errorText = new List<string>();
        List<string> storedText = new List<string>();

        int currentStoredText = -1;
        float outputLocationY = 0;

        public bool showConsole = false;

        int typerTime = 0;
        bool displayTyper = false;

        Rectangle scissorRect;
        RasterizerState rState = new RasterizerState()
        {
            ScissorTestEnable = true
        };

        int scrollValue, LocY;

        string startingOutput;

        enum CurrentWindow { Output, Error }
        CurrentWindow currentWindow = CurrentWindow.Output;

        bool isUnlocked = true;
        string unlockKey = "A";

        private WorldManager world;
        private ScreenManager screens;
        private TileMap tileMap;
        private PlayerEntity player;
        private Camera camera;

        public Console(SpriteFont Font, Texture2D Pixel, WorldManager World, ScreenManager Screens, TileMap Map, PlayerEntity Player, Camera Camera)
        {
            font = Font;
            pixel = Pixel;

            world = World;
            screens = Screens;
            tileMap = Map;
            player = Player;
            camera = Camera;

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            startingOutput = "'" + fvi.ProductName + "' RPG Engine : " + fvi.FileDescription + " [Version " + fvi.FileVersion + "]";
            outputText.Add(startingOutput);
            if (isUnlocked == false)
                outputText.Add("Unlock key required:");
            else
                outputText.Add("Type \"?\" or \"help\" for a short list of valid commands & examples. Good to go, traveller. Cheat away!");

            outputText.Add("");
            StringBuilder b = new StringBuilder();
        }

        public void Update(GameTime gt, ContentManager cm)
        {
            if (showConsole == true)
                input.UpdateInput(gt);

            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                enteredText = displayText;
                if (enteredText.Equals(unlockKey) && isUnlocked == false)
                {
                    isUnlocked = true;
                    outputText.Add("Type \"?\" or \"help\" for a short list of valid commands & examples. Good to go, traveller. Cheat away!");
                }

                if (isUnlocked == true)
                    EnterText(cm);
                else
                    EnterMinimalText(); //EnterMinimalText(cm); -- for those who shouldn't be in here!
            }
            else if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Delete))
                input.DeleteAllText();


            if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
            {
                if (currentWindow == CurrentWindow.Error)
                    currentWindow = CurrentWindow.Output;
                else if (currentWindow == CurrentWindow.Output)
                    currentWindow = CurrentWindow.Error;
            }

            if (controls.CurrentKey.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.C))
                {
                    input.text.Remove(input.text.Length - 1, 1);
                    Clipboard.SetText(input.text.ToString());
                }
                else if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.V))
                {
                    int previousLength = input.text.Length - 1;

                    input.text.Append(Clipboard.GetText());

                    if (input.text.Length > 1)
                        input.text.Remove(previousLength, 1); //Removes the shortcut key that was inputted, hah. (E.G., >: vpasted text here)
                    else
                        input.text.Remove(0, 1);
                }
            }

            CheckStoredText();

            typerTime += gt.ElapsedGameTime.Milliseconds;

            if (typerTime >= 500)
            {
                displayTyper = true;

                if (typerTime >= 1000)
                {
                    displayTyper = false;
                    typerTime = 0;
                }
            }

            if (currentWindow == CurrentWindow.Output)
                outputLocationY = (outputText.Count * -font.MeasureString("A").Y);
            else if (currentWindow == CurrentWindow.Error)
                outputLocationY = (errorText.Count * -font.MeasureString("A").Y);

            KeepOutputInBounds();

            if (outputText.Count > 500)
                outputText.RemoveRange(0, 250);
            if (errorText.Count > 500)
                errorText.RemoveRange(0, 250);

            controls.UpdateLast();
        }
        private void EnterText(ContentManager cm)
        {
            enteredText = displayText;

            if (currentWindow == CurrentWindow.Output)
            {
                if (!enteredText.Equals(null) || enteredText.Length > 0)
                {
                    storedText.Add(enteredText.ToString());
                    enteredText = ApplyScriptSyntax(enteredText);


                    //Console-related commands
                    ParseHelp(enteredText.ToString());
                    ParseConsole(enteredText.ToString());
                }
            }
            else if (currentWindow == CurrentWindow.Error)
            {
                storedText.Add(enteredText.ToString());
                enteredText = ApplyScriptSyntax(enteredText);

                ParseError(enteredText.ToString());
            }

            input.DeleteAllText();
            enteredText.Clear();
            currentStoredText = -1;
        }
        private void EnterMinimalText()
        {
            enteredText = displayText;

            if (currentWindow == CurrentWindow.Output)
            {
                if (!enteredText.Equals(null))
                {
                    storedText.Add(enteredText.ToString());
                    enteredText = ApplyScriptSyntax(enteredText);

                    ParseConsole(enteredText.ToString());
                }
            }
            else if (currentWindow == CurrentWindow.Error)
            {
                storedText.Add(enteredText.ToString());
                enteredText = ApplyScriptSyntax(enteredText);

                ParseError(enteredText.ToString());
            }

            input.DeleteAllText();
            enteredText.Clear();
            currentStoredText = -1;
        }
        private void CheckStoredText()
        {
            if (storedText.Count > 0)
            {
                if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    currentStoredText++;

                    if (currentStoredText >= storedText.Count) //Assign to -1 / bototm of list
                        currentStoredText = -1;
                    if (currentStoredText == -1)
                        input.DeleteAllText();

                    if (currentStoredText >= 0)
                    {
                        input.DeleteAllText();
                        input.text.Append(storedText[currentStoredText]);
                    }
                }
                else if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    if (currentStoredText >= -1)
                        currentStoredText--;

                    if (currentStoredText == -1) //Asign to blank value
                        input.DeleteAllText();
                    if (currentStoredText <= -2)
                        currentStoredText = storedText.Count - 1;

                    if (currentStoredText >= 0 && currentStoredText < storedText.Count)
                    {
                        input.DeleteAllText();
                        input.text.Append(storedText[currentStoredText]);
                    }
                }
            }
        }

        public void KeepOutputInBounds()
        {
            if (controls.CurrentMS.ScrollWheelValue < scrollValue)
            {
                LocY -= 15;
            }
            else if (controls.CurrentMS.ScrollWheelValue > scrollValue)
            {
                LocY += 15;
            }
            scrollValue = controls.CurrentMS.ScrollWheelValue;

            if (currentWindow == CurrentWindow.Output)
                LocY = (int)MathHelper.Clamp(LocY, 10, outputText.Count * font.MeasureString("A").Y - 8);
            else if (currentWindow == CurrentWindow.Error)
                LocY = (int)MathHelper.Clamp(LocY, 10, errorText.Count * font.MeasureString("A").Y - 8);
        }

        public void Draw(SpriteBatch sb)
        {
            displayText = input.text;

            DrawBoxes(sb);

            SpriteBatchHelper.DrawString(sb, font, ">:", new Vector2(54, GameSettings.WindowResolution.Y - 46), Vector2.Zero, Color.White, 1f);

            if (displayText != null)
                SpriteBatchHelper.DrawString(sb, font, displayText.ToString(), new Vector2(54 + font.MeasureString(">: ").X, GameSettings.WindowResolution.Y - 46), Vector2.Zero, Color.White, 1f);

            if (displayTyper == true)
                SpriteBatchHelper.DrawString(sb, font, "|", new Vector2((53 + font.MeasureString(">: ").X) + font.MeasureString(displayText).X, GameSettings.WindowResolution.Y - 46), Vector2.Zero, Color.White, 1f);
        }

        private void DrawBoxes(SpriteBatch sb)
        {
            SpriteBatchHelper.DrawBoxBordered(sb, pixel, new Rectangle(50, GameSettings.WindowResolution.Y - 50, GameSettings.WindowResolution.X - 100, 25), Color.Lerp(Color.Transparent, Color.Blue, .05f), Color.Lerp(Color.Transparent, Color.Black, .75f));
            SpriteBatchHelper.DrawBoxBordered(sb, pixel, new Rectangle(50, GameSettings.WindowResolution.Y - 465, GameSettings.WindowResolution.X - 100, 400), Color.Lerp(Color.Transparent, Color.Blue, .05f), Color.Lerp(Color.Transparent, Color.Black, .75f));
        }
        public void DrawOutput(SpriteBatch sb, GraphicsDevice g)
        {
            scissorRect = new Rectangle(50, GameSettings.WindowResolution.Y - 464, GameSettings.WindowResolution.X - 100, 399);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rState);

            g.ScissorRectangle = scissorRect;

            if (currentWindow == CurrentWindow.Output)
            {
                for (int i = 0; i < outputText.Count; i++)
                {
                    SpriteBatchHelper.DrawString(sb, font, outputText[i], new Vector2(54, (((GameSettings.WindowResolution.Y - 75) + outputLocationY) + (i * font.MeasureString("A").Y) + LocY)), Vector2.Zero, Color.Lerp(Color.Transparent, Color.White, 1f), 1f);
                }
            }

            if (currentWindow == CurrentWindow.Error)
            {
                for (int i = 0; i < errorText.Count; i++)
                {
                    SpriteBatchHelper.DrawString(sb, font, errorText[i], new Vector2(54, (((GameSettings.WindowResolution.Y - 75) + outputLocationY) + (i * font.MeasureString("A").Y) + LocY)), Vector2.Zero, Color.Lerp(Color.Transparent, Color.White, 1f), 1f);
                }
            }

            sb.End();
        }

        private void ParsePlayer(string text)
        {
            text = text.ToUpperInvariant();

            List<string> t = new List<string>();
            string[] spacing = text.Split(' ');

            foreach (string i in spacing)
            {
                if (!string.IsNullOrEmpty(i))
                    t.Add(i);
            }

            if (t[0].Equals("PLAYER")) //Player.
            {
                #region AddItem
                if (t[1].Equals("ITEM"))
                {
                    if (t[2].Equals("ADD"))
                    {
                        int id = 0;
                        int quantity = 0;

                        try
                        {
                            id = int.Parse(t[3]);
                            quantity = int.Parse(t[4]);

                            if (id < 0)
                                id = 0;
                            if (quantity < 1)
                                quantity = 1;
                        }
                        catch (Exception e)
                        {
                            outputText.Add(" ");
                            outputText.Add("Error Parsing Line: " + text);
                            outputText.Add(e.Message);
                        }

                        foreach (BaseItem b in ItemDatabase.Items)
                        {
                            if (b.ID == id)
                            {
                                outputText.Add("PLAYER: Added " + quantity + " of " + b.Name + " to \"" + b.tabType + "\" tab.");
                            }
                        }
                    }
                    else if (t[2].Equals("REMOVE"))
                    {
                        int id = 0;
                        int quantity = 0;

                        try
                        {
                            id = int.Parse(t[3]);
                            quantity = int.Parse(t[4]);

                            if (id < 0)
                                id = 0;
                            if (quantity < 1)
                                quantity = 1;
                        }
                        catch (Exception e)
                        {
                            outputText.Add(" ");
                            outputText.Add("Error Parsing Line: " + text);
                            outputText.Add(e.Message);
                        }

                        //screens.RemoveItem(id, quantity);

                        foreach (BaseItem b in ItemDatabase.Items)
                        {
                            if (b.ID == id)
                            {
                                outputText.Add("PLAYER: Removed " + quantity + " of " + b.Name + " to \"" + b.tabType + "\" tab.");
                            }
                        }
                    }
                }
                #endregion
                #region Player.Health
                if (t[1].Equals("SKILLS"))
                {

                }
                #endregion
            }
        }
        private void ParseMap(string text, ContentManager cm)
        {
        }
        private void ParseEntity(string text) { }
        private void ParseSettings(string text)
        {
            text = text.ToUpperInvariant();

            List<string> t = new List<string>();
            string[] spacing = text.Split(' ');

            foreach (string i in spacing)
            {
                if (!string.IsNullOrEmpty(i))
                    t.Add(i);
            }

            if (t[0].Equals("SETTINGS") || t[0].Equals("OPTIONS"))
            {
                if (t[1].Equals("SHOWPARTICLES"))
                {
                    GameSettings.ShowParticles = bool.Parse(t[2]);
                    outputText.Add("SETTINGS: set \"ShowParticles\" to " + GameSettings.ShowParticles);
                }
                else if (t[1].Equals("NOCLIP"))
                {
                    try { GameSettings.NoClip = bool.Parse(t[2]); }
                    catch (Exception e)
                    {
                        OutputToError("SETTINGS: Error parsing text: " + e.Message);
                    }

                    outputText.Add("SETTINGS: set \"NoClip\" to " + GameSettings.NoClip);
                }
            }
        }

        private void ParseHelp(string text)
        {
            text = text.ToUpperInvariant();

            List<string> t = new List<string>();
            string[] spacing = text.Split(' ');

            foreach (string i in spacing)
            {
                if (!string.IsNullOrEmpty(i))
                    t.Add(i);
            }

            if (t[0].Equals("?") || t[0].Equals("HELP"))
            {
                if (t.Count == 1)
                {
                    OutputDividerConsole();
                    outputText.Add("Valid opening commands: Player, Map, Entity, Settings/Options, Console. Type \"?\" plus an opening command for a list of commands.");
                    outputText.Add("Acceptable syntax: \",():\" are all replace with an empty value. However, \".\" is replaced with a space. (E.G., Player.Item Add (1, 5)");
                    outputText.Add("Use arrow keys to access command entries. 'Delete' key to clear command input line. Press 'Left Alt' to switch output windows.");
                    outputText.Add("Note: lines are split into words by spaces. Be sure to add in the proper amount of spacing!");
                }

                if (t.Count > 1)
                {
                    if (t[1].Equals("PLAYER"))
                    {
                        if (t.Count > 1)
                        {
                            OutputDividerConsole();
                            outputText.Add("Results for \"Player\":");
                            outputText.Add(" - Item Add/Remove/Check/CheckNumber");
                            outputText.Add(" - Skills Health/Agility/etc.");
                        }

                        if (t.Count > 2)
                        {
                            if (t[2].Equals("ITEM"))
                            {
                                OutputDividerConsole();
                                outputText.Add("Results for \"Player.Item\":");
                                outputText.Add(" - Add : (Add ID QUANTITY) Add item to player inventory");
                                outputText.Add(" - Remove : (Remove ID QUANTITY) Remove item from player inventory");
                            }
                        }
                    }
                    if (t[1].Equals("CONSOLE"))
                    {
                        OutputDividerConsole();
                        outputText.Add("Results for \"Console\":");
                        outputText.Add("");
                        outputText.Add(" - Clear All/Ouput/Stored : \"All\" clears both output and stored, \"Output\" clears text in window, \"Stored\" clears previous entries.");
                        outputText.Add(" -");
                        outputText.Add(" -");
                        outputText.Add("");
                    }
                }
            }
        }
        private void ParseConsole(string text)
        {
            text = text.ToUpperInvariant();

            List<string> t = new List<string>();
            string[] spacing = text.Split(' ');

            foreach (string i in spacing)
            {
                if (!string.IsNullOrEmpty(i))
                    t.Add(i);
            }

            if (t[0].Equals("CLEAR"))
            {
                if (t[1].Equals("ALL"))
                {
                    outputText.Clear();
                    errorText.Clear();
                    storedText.Clear();
                }
                else if (t[2].Equals("OUTPUT"))
                {
                    outputText.Clear();
                }
                else if (t[2].Equals("ERROR"))
                {
                    errorText.Clear();
                }
                else if (t[2].Equals("STORED"))
                {
                    storedText.Clear();
                }
            }
        }

        private void ParseError(string text)
        {
            text = text.ToUpperInvariant();

            List<string> t = new List<string>();
            string[] spacing = text.Split(' ');

            foreach (string i in spacing)
            {
                if (!string.IsNullOrEmpty(i))
                    t.Add(i);
            }

            if (t[0].Equals("?") || t[0].Equals("HELP") || t[0].Equals("H"))
            {
                OutputToError("Acceptable Error Window Commands: ");
                OutputToError("    ReturnLine lineNumber - returns the line of the current player map file, specified by lineNumber.");
                OutputToError("    Find keyword - returns all lines that contain the specified keyword.");

            }

            if (t[0].Equals("RETURNLINE"))
            {
                try { OutputToError("ReturnLine[" + t[1] + "]: " + tileMap.ReturnLine(int.Parse(t[1]))); }
                catch
                {
                    OutputToError("ERROR: " + t[1] + " is not a valid number.");
                }
            }

            if (t[0].Equals("FIND"))
            {
                try
                {
                    List<string> findValue = tileMap.Find(t[1]);

                    OutputToError("FIND[" + t[1] + "]: Found " + findValue.Count + " lines where \"" + t[1] + "\" occurs at least once.");

                    if (findValue.Count > 0)
                    {
                        for (int a = 0; a < findValue.Count; a++)
                            OutputToError("    " + (a + 1) + ". " + findValue[a]);
                    }
                }
                catch (Exception e)
                {
                    OutputToError("ERROR: " + e.Message);
                }
            }
        }

        private string ApplyScriptSyntax(string line)
        {
            line = line.Replace(",", "");
            line = line.Replace("(", "");
            line = line.Replace(")", "");
            line = line.Replace(":", "");
            line = line.Replace(".", " ");

            return line;
        }
        private StringBuilder ApplyScriptSyntax(StringBuilder line)
        {
            line = line.Replace(",", "");
            line = line.Replace("(", "");
            line = line.Replace(")", "");
            line = line.Replace(":", "");
            line = line.Replace(".", " ");

            return line;
        }

        public void OutputDividerConsole() { outputText.Add("_____________________________________________________________________________"); }
        public void OutputToConsole(string line) { outputText.Add(line); } //For use by other classes!

        public void OutputDividerError() { errorText.Add("--------------------------------------------------------------------------"); }
        public void OutputToError(string line) { line = line.Replace(Environment.NewLine, " "); errorText.Add(line); }

        public void OutputCommonMapError(string mapName, string type, int currentLine, Exception e) { errorText.Add(mapName + ": Error parsing " + type + " at line " + currentLine + ": " + e.Message); }
        public void OuputCommonMapWarning(string lineSnippet, int currentLine) { errorText.Add("WARNING: Unknown entry at line " + currentLine + ": " + lineSnippet); }
    }
}
