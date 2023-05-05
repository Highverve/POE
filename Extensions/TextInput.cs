using System.Collections.Generic;
using System.Text;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class TextInput
    {
        Controls controls = new Controls();

        Keys LeftModifier, RightModifier, CapsLock, BackSpace;
        List<Keys> AcceptedLetters = new List<Keys>();
        List<Keys> AcceptedSymbols = new List<Keys>();

        public StringBuilder text = new StringBuilder();

        int time = 0;

        public enum KeyType { All, LettersOnly, DigitsOnly, DigitsPeriodOnly, }
        private KeyType keyType = KeyType.All;

        public TextInput(KeyType Type = KeyType.All)
        {
            LeftModifier = Keys.LeftShift;
            RightModifier = Keys.RightShift;

            CapsLock = Keys.CapsLock;
            BackSpace = Keys.Back;

            keyType = Type;
            AcceptKeys();
        }

        public void ResetAcceptedKeys(KeyType keyType)
        {
            this.keyType = keyType;

            AcceptedLetters.Clear();
            AcceptedSymbols.Clear();

            AcceptKeys();
        }

        private void AcceptKeys()
        {
            if (keyType == KeyType.LettersOnly || keyType == KeyType.All)
            {
                AddToLetters(Keys.A, Keys.B, Keys.C,
                       Keys.D, Keys.E, Keys.F,
                       Keys.G, Keys.H, Keys.I, 
                       Keys.J, Keys.K, Keys.L, 
                       Keys.M, Keys.N, Keys.O,
                       Keys.P, Keys.Q, Keys.R, Keys.S,
                       Keys.T, Keys.U, Keys.V, 
                       Keys.W, Keys.X, Keys.Y, Keys.Z);
            }

            if (keyType == KeyType.DigitsOnly || keyType == KeyType.All || keyType == KeyType.DigitsPeriodOnly)
            {
                AddToSymbols(Keys.D1, Keys.D2, Keys.D3,
                             Keys.D4, Keys.D5, Keys.D6,
                             Keys.D7, Keys.D8, Keys.D9, Keys.D0);
            }

            if (keyType == KeyType.All)
            {
                AddToSymbols(Keys.OemPeriod, Keys.OemComma, Keys.OemQuestion,
                             Keys.OemQuotes, Keys.OemSemicolon, Keys.OemOpenBrackets,
                             Keys.OemCloseBrackets, Keys.OemPlus, Keys.OemMinus, Keys.OemPipe);
            }

            if (keyType == KeyType.DigitsPeriodOnly)
                AddToSymbols(Keys.OemPeriod, Keys.OemMinus);

            AddToSymbols(Keys.Space);
        }

        private void AddToLetters(params Keys[] keys)
        {
            for (int k = 0; k < keys.Length; k++)
            {
                AcceptedLetters.Add(keys[k]);
            }
        }
        private void AddToSymbols(params Keys[] keys)
        {
            for (int k = 0; k < keys.Length; k++)
            {
                AcceptedSymbols.Add(keys[k]);
            }
        }

        public void UpdateInput(GameTime gt)
        {
            controls.UpdateCurrent();

            CheckLetterInput();
            CheckSymbolInput();

            if (controls.IsKeyPressedOnce(BackSpace))
                Backspace();
            
            if (controls.IsKeyHeld(BackSpace, 350, gt))
            {
                time += gt.ElapsedGameTime.Milliseconds;

                if (time > 50)
                {
                    Backspace();
                    time = 0;
                }
            }

            controls.UpdateLast();
        }

        private void CheckLetterInput()
        {
            foreach (Keys key in AcceptedLetters)
            {
                if (controls.CurrentKey.IsKeyDown(LeftModifier) ||
                    controls.CurrentKey.IsKeyDown(RightModifier) ||
                    controls.CurrentKey.IsKeyDown(CapsLock))
                {
                    if (controls.IsKeyPressedOnce(key))
                        AddUpperCase(key);
                }
                else
                {
                    if (controls.IsKeyPressedOnce(key))
                        AddLowerCase(key);
                }
            }
        }
        private void CheckSymbolInput()
        {
            foreach (Keys key in AcceptedSymbols)
            {
                if (controls.CurrentKey.IsKeyDown(LeftModifier) ||
                    controls.CurrentKey.IsKeyDown(RightModifier) ||
                    controls.CurrentKey.IsKeyDown(CapsLock))
                {
                    if (controls.IsKeyPressedOnce(key))
                        AddUpperSymbol(key);
                }
                else
                {
                    if (controls.IsKeyPressedOnce(key))
                        AddLowerSymbol(key);
                }
            }
        }

        private void AddLowerCase(Keys key)
        {
            string temp = "" + key;
            temp = temp.ToLowerInvariant();

            text.Append(temp);
        }
        private void AddUpperCase(Keys key)
        {
            text.Append(key);
        }

        private void AddLowerSymbol(Keys key)
        {
            switch (key)
            {
                case Keys.D1: text.Append("1"); break;
                case Keys.D2: text.Append("2"); break;
                case Keys.D3: text.Append("3"); break;
                case Keys.D4: text.Append("4"); break;
                case Keys.D5: text.Append("5"); break;
                case Keys.D6: text.Append("6"); break;
                case Keys.D7: text.Append("7"); break;
                case Keys.D8: text.Append("8"); break;
                case Keys.D9: text.Append("9"); break;
                case Keys.D0: text.Append("0"); break;
                case Keys.OemPeriod: text.Append("."); break;
                case Keys.OemComma: text.Append(","); break;
                case Keys.OemQuestion: text.Append("/"); break;
                case Keys.OemQuotes: text.Append("'"); break;
                case Keys.OemSemicolon: text.Append(";"); break;
                case Keys.OemOpenBrackets: text.Append("["); break;
                case Keys.OemCloseBrackets: text.Append("]"); break;
                case Keys.OemPlus: text.Append("="); break;
                case Keys.OemMinus: text.Append("-"); break;
                case Keys.OemPipe: text.Append("\\"); break;
                case Keys.Space: text.Append(" "); break;
            }
        }
        private void AddUpperSymbol(Keys key)
        {
            switch (key)
            {
                case Keys.D1: text.Append("!"); break;
                case Keys.D2: text.Append("@"); break;
                case Keys.D3: text.Append("#"); break;
                case Keys.D4: text.Append("$"); break;
                case Keys.D5: text.Append("%"); break;
                case Keys.D6: text.Append("^"); break;
                case Keys.D7: text.Append("&"); break;
                case Keys.D8: text.Append("*"); break;
                case Keys.D9: text.Append("("); break;
                case Keys.D0: text.Append(")"); break;
                case Keys.OemPeriod: text.Append(">"); break;
                case Keys.OemComma: text.Append("<"); break;
                case Keys.OemQuestion: text.Append("?"); break;
                case Keys.OemQuotes: text.Append("\""); break;
                case Keys.OemSemicolon: text.Append(":"); break;
                case Keys.OemOpenBrackets: text.Append("{"); break;
                case Keys.OemCloseBrackets: text.Append("}"); break;
                case Keys.OemPlus: text.Append("+"); break;
                case Keys.OemMinus: text.Append("_"); break;
                case Keys.OemPipe: text.Append("|"); break;
                case Keys.Space: text.Append(" "); break;
            }
        }

        private void Backspace()
        {
            if (text.Length > 0)
                text = text.Remove(text.Length - 1, 1);
        }

        public StringBuilder ReturnText { get { return text; } }

        public void DeleteAllText()
        {
            text.Clear();
        }
    }
}
