using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class TextBlock
    {
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public SpriteFont Font { get; set; }

        public TextBlock(string Text, Vector2 Position, Color Color)
        {
            this.Text = Text;
            this.Position = Position;
            this.Color = Color;
            this.Font = null;
        }
        public TextBlock(string Text, Vector2 Position, Color Color, SpriteFont Font)
        {
            this.Text = Text;
            this.Position = Position;
            this.Color = Color;
            this.Font = Font;
        }
    }

    public static class Strings
    {
        private static string RGBAToHex(Color color)
        {
            return color.R.ToString("X2") +
                   color.G.ToString("X2") +
                   color.B.ToString("X2") +
                   color.A.ToString("X2");
        }
        private static Color HexToRGBA(string hex)
        {
            hex = hex.Replace("#", ""); //Remove hashtag at beginning if it has one

            if (hex.Length % 2 != 0)
                return Color.Transparent; //If the hex code is invalid (odd number of characters), return transparent.

            string r = (hex[0] + hex[1]).ToString();
            string g = (hex[2] + hex[3]).ToString();
            string b = (hex[4] + hex[5]).ToString();
            string a = (hex[6] + hex[7]).ToString();

            return new Color(byte.Parse(r, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(g, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(b, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(a, System.Globalization.NumberStyles.HexNumber));
        }

        public static string RGBToHex(this Color color)
        {
            return color.R.ToString("X2") +
                   color.G.ToString("X2") +
                   color.B.ToString("X2");
        }
        public static Color HexToRGB(string hex)
        {
            hex = hex.Replace("#", ""); //Remove hashtag at beginning if it has one

            if (hex.Length % 2 != 0)
                return Color.Transparent; //If the hex code is invalid (odd number of characters), return transparent.

            string r = string.Format("{0}{1}", hex[0], hex[1]);
            string g = string.Format("{0}{1}", hex[2], hex[3]);
            string b = string.Format("{0}{1}", hex[4], hex[5]);

            return new Color(byte.Parse(r, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(g, System.Globalization.NumberStyles.HexNumber),
                             byte.Parse(b, System.Globalization.NumberStyles.HexNumber),
                             255);
        }

        private enum FormatTextState { NewLine, StartTag, EndTag, None }
        public static List<TextBlock> WrapFormatText(this string text, SpriteFont font, Color defaultColor, float maxWidth)
        {
            List<TextBlock> blocks = new List<TextBlock>();
            StringBuilder builder = new StringBuilder();

            //Example format: Hello, [@255 255 255 255]Bob[/@]! My [#FFFFFF]name[/#] is [B] Bob[/B]
            string[] words = Regex.Split(text, @"(?<=[ ])");//text.Split(' ');

            float currentLength = 0;// space = font.MeasureString(" ").X;
            Vector2 position = Vector2.Zero;
            Color currentColor = defaultColor;
            FormatTextState state = FormatTextState.None;

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("\n"))
                    state = FormatTextState.NewLine;
                else if (words[i].ToUpper().Contains("[") && !words[i].ToUpper().Contains("[/"))
                    state = FormatTextState.StartTag;
                else if (words[i].ToUpper().Contains("[/"))
                    state = FormatTextState.EndTag;
                else
                    state = FormatTextState.None;

                switch (state)
                {
                    case FormatTextState.NewLine:
                    {
                        //Get total new lines
                        int totalNL = words[i].TotalCharacters("\n");

                        //If the word starts with \n... 
                        if (words[i].StartsWith("\n")) //Cut off the block there.
                        {
                            blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                            builder.Clear();
                        }
                        else if (words[i].EndsWith("\n")) //If it ends with it, add the word then cut it off.
                        {
                            blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                            builder.Clear();

                            words[i] = string.Empty;
                        }

                        position.X = 0; //Set position.X back to 0

                        //Add to position.Y with total new lines
                        for (int j = 0; j < totalNL; j++)
                            position.Y += font.LineSpacing;

                        words[i] = words[i].Replace("\n", "");

                        //If the current word is not empty, 
                        if (!string.IsNullOrEmpty(words[i]))
                        {
                            if (!string.IsNullOrEmpty(words[i]))
                                builder.Append(words[i] + " ");
                        }

                        currentLength = 0;
                    }
                    break;

                    case FormatTextState.StartTag:
                    {
                        string tagContent = words[i].FromWithin('[', ']', 1);

                        //If the word starts with a tag ...
                        if (words[i].ToUpper().StartsWith("["))
                        {
                            //Add the current text
                            blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                            position.X += font.MeasureString(builder.ToString()).X;

                            builder.Clear();

                            //Get new color from tag
                            if (tagContent.StartsWith("#"))
                                currentColor = HexToRGB(tagContent);

                            //Replace tag contents with empty value
                            words[i] = words[i].Replace("[" + tagContent + "]", "");

                            //Append the current word to the next text block
                            if (!string.IsNullOrEmpty(words[i]))
                                builder.Append(words[i]);
                        }
                        else //Else, the word ends with a tag
                        {
                            //Replace tag contents with empty value
                            words[i] = words[i].Replace("[" + tagContent + "]", "");

                            //Append the current word to the current text block
                            if (!string.IsNullOrEmpty(words[i]))
                                builder.Append(words[i]);

                            //Add the current text
                            blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                            position.X += font.MeasureString(builder.ToString()).X;

                            builder.Clear();

                            //Get new color from tag
                            if (tagContent.StartsWith("#"))
                                currentColor = HexToRGB(tagContent);
                        }
                    }
                    break;

                    case FormatTextState.EndTag:
                    {
                        string tagContent = words[i].FromWithin('[', ']', 1);

                        if (words[i].ToUpper().StartsWith("[")) //If the tag is at the front of the word ...
                        {
                            //Add the current block with the current color.
                            blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                            position.X += font.MeasureString(builder.ToString()).X;
                            builder.Clear();

                            //Replace tag contents with empty value
                            words[i] = words[i].Replace("[" + tagContent + "]", "");

                            //Append the current word to the next text block
                            if (!string.IsNullOrEmpty(words[i]))
                                builder.Append(words[i]);
                        }
                        else //Else, the tag is definitely at the back!
                        {
                            //Replace tag contents with empty value
                            words[i] = words[i].Replace("[" + tagContent + "]", "");

                            //Append the current word to the current text block
                            if (!string.IsNullOrEmpty(words[i]))
                                builder.Append(words[i]);

                            //Add the current text block with the current color
                            blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                            position.X += font.MeasureString(builder.ToString()).X;
                            builder.Clear();
                        }

                        //Reset color to default
                        currentColor = defaultColor;
                    }
                    break;

                    case FormatTextState.None:
                    {
                        builder.Append(words[i]);
                    }
                    break;
                }

                float nextWordLength = 0;

                if (i != words.Length - 1)
                    nextWordLength = font.MeasureString(words[i + 1].ToString()).X;

                currentLength += font.MeasureString(words[i].ToString()).X;

                if (currentLength + nextWordLength >= maxWidth)
                {
                    blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                    builder.Clear();

                    position.Y += font.LineSpacing;
                    position.X = 0;

                    currentLength = 0;
                }

                if (i == words.Length - 1 && builder.Length > 0)
                {
                    blocks.Add(new TextBlock(builder.ToString(), position, currentColor, font));
                    builder.Clear();
                }
            }

            return blocks;
        }
        public static string WrapText(this string text, SpriteFont spriteFont, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            for (int i = 0; i < words.Length; i++)
            {
                Vector2 size = spriteFont.MeasureString(words[i]);

                if (words[i].Contains("%%%")) //If word contains a NewLine(\n)
                {
                    words[i] = words[i].Replace("%%%", "");
                    sb.Append("\n\n\n" + words[i] + " "); //Append word + space, and assign lineWidth to size.X + spaceWidth;
                    lineWidth = size.X + spaceWidth;
                }
                else if (words[i].Contains("%%")) //If word contains a NewLine(\n)
                {
                    words[i] = words[i].Replace("%%", "");
                    sb.Append("\n\n" + words[i] + " "); //Append word + space, and assign lineWidth to size.X + spaceWidth;
                    lineWidth = size.X + spaceWidth;
                }
                else if (words[i].Contains("%")) //If word contains a NewLine(\n)
                {
                    words[i] = words[i].Replace("%", "");
                    sb.Append("\n" + words[i] + " "); //Append word + space, and assign lineWidth to size.X + spaceWidth;
                    lineWidth = size.X + spaceWidth;
                }

                else if (lineWidth + size.X < maxLineWidth) //If lineWidth + current word size is less than maxLineWidth
                {
                    sb.Append(words[i] + " "); //Append word + space, and increase lineWidth by size.X + spaceWidth;
                    lineWidth += size.X + spaceWidth;
                }
                else if (lineWidth + size.X > maxLineWidth)
                {
                    sb.Append("\n" + words[i] + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }

        public static List<string> SplitLines(this string text, SpriteFont spriteFont, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            List<string> lines = new List<string>();

            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            string tempText = "";

            for (int i = 0; i < words.Length; i++)
            {
                Vector2 size = spriteFont.MeasureString(words[i]);
                
                if (lineWidth + size.X < maxLineWidth - (maxLineWidth * .1f)) //If lineWidth + current word size is less than maxLineWidth
                {
                    tempText += (words[i] + " "); //Append word + space, and increase lineWidth by size.X + spaceWidth;
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    lines.Add(tempText + words[i] + " ");
                    tempText = string.Empty;
                    lineWidth = size.X + spaceWidth;
                }

                if (i == words.Length - 1)
                {
                    lines.Add(tempText);
                    tempText = string.Empty;
                }
            }

            return lines;
        }

        public static bool IsDigitsOnly(this string value)
        {
            foreach (char c in value)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        public static bool IsDigitsPeriodOnly(this string value)
        {
            foreach (char c in value)
            {
                if (c < '0' || c > '9')
                {
                    if (c != '.')
                        return false;
                }
            }

            return true;
        }
        public static string CommaSeparation(this int value)
        {
            return string.Format("{0:n0}", value);
        }
        public static Vector2 LineCenter(this string value, SpriteFont font)
        {
            return new Vector2((int)font.MeasureString(value).X / 2, (int)font.MeasureString(value).Y / 2);
        }
        public static Vector2 StringCenter(this string value, SpriteFont font)
        {
            int newLineCount = value.Count(f => f == '\n'); //Get the total new lines in the string

            string[] temp = value.Split('\n'); //Split the strings
            int average = 0;

            for (int i = 0; i < temp.Length; i++)
                average += (int)font.MeasureString(temp[i]).X; //Add each line length to the average

            average /= temp.Length; //Get the average by dividing by total

            int valueY = newLineCount * font.LineSpacing;

            return new Vector2(average / 2, valueY / 2);
        }

        public static int LineCount(this string value, SpriteFont font)
        {
            return (int)(font.MeasureString(value).Y / font.LineSpacing);
        }

        public static string[] Split(this string value, string word) { return value.Split(new string[] { word }, StringSplitOptions.None); }

        public static int TotalCharacters(this string value, string characters)
        {
            int total = value.Length, totalInChar = characters.Length;
            int newTotal = value.Replace(characters, "").Length;

            return (total - newTotal) / totalInChar;
        }
        public static int TotalCharacters(this string value, char character) { return value.Count(f => f == character); }

        public static int IndexOfNth(this string value, string word, int count)
        {
            int s = -1;

            for (int i = 0; i < count; i++)
            {
                s = value.IndexOf(word, s + 1);

                if (s == -1) break;
            }

            return s;
        }
        public static int IndexOfNth(this string value, char word, int count)
        {
            int s = -1;

            for (int i = 0; i < count; i++)
            {
                s = value.IndexOf(word, s + 1);

                if (s == -1) break;
            }

            return s;
        }

        public static string FromWithin(this string value, char character, int quoteSet)
        {
            int quotePosA, quotePosB;

            int totalSets = TotalCharacters(value, character) / 2; //Get total sets. (total characters / 2)

            quotePosA = IndexOfNth(value, character, (quoteSet * 2) - 1) + 1;
            quotePosB = IndexOfNth(value, character, quoteSet * 2) - quotePosA;

            if (quotePosA != -1 && quotePosB != -1)
                return value.Substring(quotePosA, quotePosB);
            else
                return string.Empty;
        }
        public static string FromWithin(this string value, char startChar, char endChar, int quoteSet)
        {
            int quotePosA, quotePosB;

            int totalSets = (TotalCharacters(value, startChar) + TotalCharacters(value, endChar)) / 2; //Get total sets. (total characters / 2)

            quotePosA = IndexOfNth(value, startChar, (quoteSet)) + 1;
            quotePosB = IndexOfNth(value, endChar, quoteSet) - quotePosA;

            if (quotePosA != -1 && quotePosB != -1)
                return value.Substring(quotePosA, quotePosB);
            else
                return string.Empty;
        }
        public static string FromWithin(this string value, string character, int quoteSet)
        {
            int quotePosA = IndexOfNth(value, character, (quoteSet * 2) - 1) + 1;
            int quotePosB = IndexOfNth(value, character, quoteSet * 2) - quotePosA;

            if (quotePosA != -1 && quotePosB != -1)
                return value.Substring(quotePosA, quotePosB);
            else
                return string.Empty;
        }
        public static string FromWithin(this string value, string startChar, string endChar, int quoteSet)
        {
            int quotePosA, quotePosB;

            int totalSets = (TotalCharacters(value, startChar) + TotalCharacters(value, endChar)) / 2; //Get total sets. (total characters / 2)

            quotePosA = IndexOfNth(value, startChar, (quoteSet * 2) - 1) + 1;
            quotePosB = IndexOfNth(value, endChar, quoteSet * 2) - quotePosA;

            if (quotePosA < 0 || quotePosB < 0)
            {
                quotePosA = 0;
                quotePosB = 0;
            }

            if (quotePosA != -1 && quotePosB != -1)
                return value.Substring(quotePosA, quotePosB);
            else
                return string.Empty;
        }

        public static string FirstLastWithin(this string value, string character)
        {
            int quotePosA = value.IndexOf(character) + 1;
            int quotePosB = value.LastIndexOf(character) - quotePosA;

            if (quotePosA != -1 && quotePosB != -1)
                return value.Substring(quotePosA, quotePosB);
            else
                return string.Empty;
        }

        public static Point WithinIndex(this string value, string character, int quoteSet)
        {
            Point within = new Point();

            within.X = IndexOfNth(value, character, (quoteSet * 2) - 1) + 1;
            within.Y = IndexOfNth(value, character, (quoteSet * 2)) - within.X;

            return within;
        }
        public static Point WithinIndex(this string value, char character, int quoteSet)
        {
            Point within = new Point();

            within.X = IndexOfNth(value, character, (quoteSet * 2) - 1) + 1;
            within.Y = IndexOfNth(value, character, (quoteSet * 2)) - within.X;

            return within;
        }
        public static Point WithinIndex(this string value, string starChar, string endChar, int quoteSet)
        {
            Point within = new Point();

            within.X = IndexOfNth(value, starChar, (quoteSet * 2) - 1) + 1;
            within.Y = IndexOfNth(value, endChar, (quoteSet * 2)) - within.X;

            return within;
        }
        public static Point WithinIndex(this string value, char startChar, char endChar, int quoteSet)
        {
            Point within = new Point();

            within.X = IndexOfNth(value, startChar, (quoteSet * 2) - 1) + 1;
            within.Y = IndexOfNth(value, endChar, (quoteSet * 2)) - within.X;

            return within;
        }


        public static bool IsWithin(this string value, char character, int quoteSet, int index)
        {
            bool isWithin = false;

            if (value.ToUpper().TotalCharacters(character) > 1) //If there are two or more characters in the string
            {
                Point range = WithinIndex(value, character, quoteSet);

                if (index >= range.X && index <= range.Y)
                    return true;
            }
            else
                isWithin = false;

            return isWithin;
        }

        public static string Random(Random random, params string[] stringArray)
        {
            return stringArray[random.Next(0, stringArray.Length)];
        }

        public static string NumberEnding(int number)
        {
            if (number == 0) return string.Empty;
            if (number == 11 || number == 12 || number == 13) return "th";

            string numberString = number.ToString();
            string lastDigit = "" + numberString[numberString.Length - 1];

            int final = int.Parse(lastDigit);

            if (final == 1)
                return "st";
            if (final == 2)
                return "nd";
            if (final == 3)
                return "rd";
            
            return "th";
        }

        public static string ApplyIdentifierSyntax(this string value)
        {
            string returnValue = value;

            if (returnValue.StartsWith("//") || returnValue.StartsWith("/"))
                returnValue = string.Empty;

            int colonCount = returnValue.TotalCharacters(':');
            int spaceCount = returnValue.TotalCharacters(' ');

            for (int i = 1; i <= colonCount; i++)
            {
                int colonIndex = returnValue.IndexOfNth(":", i);
                int previousSpaceIndex = 0, spaceIndex = 0;

                for (int j = 1; j <= spaceCount; j++)
                {
                    spaceIndex = returnValue.IndexOfNth(" ", j);

                    if (spaceIndex > colonIndex || spaceIndex == -1)
                    {
                        spaceIndex = previousSpaceIndex;
                        break;
                    }
                    previousSpaceIndex = spaceIndex;
                }

                returnValue = returnValue.Remove(spaceIndex, (colonIndex - spaceIndex));
                returnValue = returnValue.Insert(spaceIndex, " ");
            }

            returnValue = returnValue.Replace(":", ""); //Temporary fix!

            return returnValue;
        }
        public static string InjectRandoms(this string value, Random random, string tagInt = "RINT:", string tagFloat = "RFLOAT:", string tagString = "RSTRING:", string tagPickInt = "PINT:", string tagGibberish = "GIBBERISH:", string tagGibberize = "GIBBERIZE:")
        {
            string returnValue = value;

            string[] words = returnValue.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                string temp = words[i];

                //E.G, rint:5-20  --  random integer injection
                if (words[i].ToUpper().StartsWith(tagInt))
                {
                    words[i] = words[i].ToUpper().Replace(tagInt, "");
                    string[] sub = words[i].Split('-');

                    int minNumber = int.Parse(sub[0]), maxNumber = int.Parse(sub[1]);
                    words[i] = random.Next(minNumber, maxNumber).ToString();
                }

                //E.G, rfloat:5.5-20.5  --  random float injection
                if (words[i].ToUpper().StartsWith(tagFloat))
                {
                    string[] variables = words[i].GrabVariables(tagFloat, '-');

                    float minNumber = float.Parse(variables[0]), maxNumber = float.Parse(variables[1]);
                    words[i] = random.NextFloat(minNumber, maxNumber).ToString();
                }

                //E.G, gibberish:50,1,10  --  random word injection, structured
                if (words[i].ToUpper().StartsWith(tagGibberish))
                {
                    string[] variables = words[i].GrabVariables(tagGibberish, ',');

                    int length = int.Parse(variables[0]), minWordLength = int.Parse(variables[1]), maxWordLength = int.Parse(variables[2]);
                    words[i] = "\"" + RandomLettersStructured(random, length, minWordLength, maxWordLength) + "\"";
                }

                if (!string.IsNullOrEmpty(temp))
                    returnValue = returnValue.Replace(temp, words[i]);
            }

            int totalStringR = value.ToUpper().TotalCharacters(tagString.ToUpper()); //Get the total occurences of "rstring:"

            //E.G, rstring:[Hello there! -- Hey! -- Yes, you...]  -  random string injection
            for (int i = 1; i < totalStringR + 1; i++)
            {
                if (value.ToUpper().Contains(tagString))
                {
                    string within = value.FromWithin('[', ']', i);
                    string[] variables = within.Split(" -- ");

                    string rand = Random(random, variables);
                    returnValue = returnValue.Replace("[" + within + "]", rand); //Replaces everything from '[' to ']' with the chosen string
                } //Result is rstring:"Yes, you..."
            }

            int totalPInt = value.ToUpper().TotalCharacters(tagPickInt.ToUpper()); //Get the total occurences of "pInt:"

            //E.G, pint:[4,8,15,16,23,42]  -  random number selection
            for (int i = 1; i < totalPInt + 1; i++)
            {
                if (value.ToUpper().Contains(tagPickInt))
                {
                    string within = value.FromWithin('[', ']', i);
                    string[] variables = within.Split(",");

                    string rand = Random(random, variables);
                    returnValue = returnValue.Replace("[" + within + "]", rand); //Replaces everything from '[' to ']' with the chosen string
                } //Result is pint:8
            }

            return returnValue;
        }
        private static string[] GrabVariables(this string line, string tag, char separator)
        {
            line = line.ToUpper().Replace(tag, ""); //Remove tag (E.G, "rint:"
            return line.Split(separator);
        }

        private const string consonants = "bcdfghjklmnpqrstvwxyz";
        private const string vowels = "aeiou";
        public static char GetVowel(Random random) { return vowels[random.Next(0, vowels.Length)]; }
        public static char GetConsonant(Random random) { return consonants[random.Next(0, consonants.Length)]; }
        public static char GetLetter(Random random) { return (char)('a' + random.Next(0, 26)); }
        public static char GetCharacter(Random random, string customSymbols) { return customSymbols[random.Next(0, customSymbols.Length)]; }

        public static bool IsVowel(char letter)
        {
            for (int i = 0; i < vowels.Length; i++)
            {
                if (Char.ToUpper(vowels[i]).Equals(Char.ToUpper(letter)))
                    return true;
            }

            return false;
        }
        public static bool IsConsonant(char letter)
        {
            for (int i = 0; i < consonants.Length; i++)
            {
                if (Char.ToUpper(consonants[i]).Equals(Char.ToUpper(letter)))
                    return true;
            }

            return false;
        }

        public static string RandomLetters(Random random, int length, int minWordLength = 5)
        {
            string words = string.Empty;
            int spaceIndex = 0;

            for (int i = 0; i < length; i++)
            {
                words += GetLetter(random);

                if ((words.Length - spaceIndex) > minWordLength) //Gets the length of the current word
                {
                    if (random.Next(0, 100) > 75) //25% chance of adding a space
                    {
                        spaceIndex = words.Length;
                        words += ' '; //Add a space into the sentence!
                    }
                }
            }

            return words;
        }
        public static string RandomLettersStructured(Random random, int length, int minWordLength = 3, int maxWordLength = 12)
        {
            string words = string.Empty;
            int spaceIndex = 0;

            for (int i = 0; i < length; i++)
            {
                int currentWordLength = random.Next(minWordLength, maxWordLength);

                if (string.IsNullOrEmpty(words) || words.Last() == ' ') //If the last letter is nothing or a space
                    words += GetLetter(random);
                if (IsConsonant(words.Last())) //If the last letter is a consonant, get a vowel!
                {
                    words += GetVowel(random);

                    if (Char.ToUpper(words.Last()).Equals('T'))
                    {
                        if (random.Next(0, 100) < 60)
                        {
                            words += 'h';
                        }
                    }

                    if (Char.ToUpper(words.Last()).Equals('X'))
                    {
                        if (random.Next(0, 100) < 75) //75% chance to remove the 'X'
                        {
                            words = words.TrimEnd('X');
                            words += GetVowel(random);
                        }
                    }

                    if (Char.ToUpper(words.Last()).Equals('Z'))
                    {
                        if (random.Next(0, 100) < 85) //85% chance to remove the 'X'
                        {
                            words = words.TrimEnd('Z');
                            words += GetVowel(random);
                        }
                    }

                    if (Char.ToUpper(words.Last()).Equals('Y'))
                    {
                        if (random.Next(0, 100) < 50) //50% chance to remove the 'X'
                        {
                            words = words.TrimEnd('Y');
                            words += GetVowel(random);
                        }
                    }

                    if (Char.ToUpper(words.Last()).Equals('Q'))
                    {
                        if (random.Next(0, 100) < 85) //85% chance to remove the 'X'
                        {
                            words = words.TrimEnd('Q');
                            words += GetVowel(random);
                        }
                    }

                    if (Char.ToUpper(words.Last()).Equals('J'))
                    {
                        if (random.Next(0, 100) < 50) //50% chance to remove the 'X'
                        {
                            words = words.TrimEnd('X');
                            words += GetVowel(random);
                        }
                    }

                    if (random.Next(0, 100) > 85)
                        words += GetVowel(random); //15% chance to produce double random vowels
                }
                if (IsVowel(words.Last())) //If the last letter is a consonant, get a vowel!
                {
                    words += GetConsonant(random);

                    if (random.Next(0, 100) > 85)
                        words += GetConsonant(random); //15% chance to produce double random consonants
                }

                if ((words.Length - spaceIndex) > currentWordLength) //Gets the length of the current word
                {
                    spaceIndex = words.Length;
                    words += ' ';
                }
            }

            return words;
        }
        public static string Gibberize(string text, Random random)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            char[] values = text.ToCharArray();

            for (int i = 0; i < values.Length; i++)
            {
                if (IsVowel(values[i]))
                    values[i] = GetVowel(random);
                if (IsConsonant(values[i]))
                    values[i] = GetConsonant(random);
            }

            return new string(values);
        }
        public static string Gibberize(string text, Random random, string customVowels, string customConsonants)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            char[] values = text.ToCharArray();

            for (int i = 0; i < values.Length; i++)
            {
                if (IsConsonant(values[i]))
                    values[i] = GetCharacter(random, customConsonants);
                if (IsVowel(values[i]))
                    values[i] = GetCharacter(random, customVowels);
            }

            return new string(values);
        }

        public static string RandomSymbols(Random random, int length, string symbols)
        {
            string returnValue = string.Empty;

            for (int i = 0; i < length; i++)
                returnValue += symbols[random.Next(0, symbols.Length)];

            return returnValue;
        }

        public static string TypeName(this Type type)
        {
            if (type == typeof(int)) return "int";
            if (type == typeof(uint)) return "uint";

            if (type == typeof(short)) return "short";
            if (type == typeof(ushort)) return "ushort";

            if (type == typeof(byte)) return "byte";
            if (type == typeof(sbyte)) return "sbyte";

            if (type == typeof(bool)) return "bool";

            if (type == typeof(long)) return "long";
            if (type == typeof(ulong)) return "ulong";

            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";

            if (type == typeof(string)) return "string";
            if (type == typeof(char)) return "char";

            if (type.IsGenericType)
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => TypeName(x)).ToArray()) + ">";

            return type.Name;
        }
        public static string CapitalizeFirst(this string value)
        {
            if (!string.IsNullOrEmpty(value))
                if (value.Length > 0)
                    return char.ToUpper(value[0]) + value.Substring(1);

            return value;
        }
        public static string AppendVariables(params object[] value)
        {
            return "(" + string.Join(", ", value) + ")";
        }

        public static object Parse(this string source, Type ttype, Func<object> onFailure = null)
        {
            object parsed;

            try
            {
                parsed = Convert.ChangeType(source, ttype);
            }
            catch (Exception)
            {
                if (onFailure == null)
                {
                    throw;
                }

                return onFailure();
            }

            return parsed;
        }
        public static TType Parse<TType>(this string source, Func<TType> onFailure = null)
        {
            return (TType)source.Parse(typeof(TType), onFailure != null ? (Func<object>)(() => onFailure()) : null);
        }
    }
}
