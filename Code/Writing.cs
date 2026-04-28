using static Animalopoly.Code.Program;
using System.Runtime.InteropServices;

namespace Animalopoly.Code
{
    class Writing
    {
        public const string LOCALE = "jp-JP";
        public static string LOCALE_CURRENCY;
        public static string LOCALE_MONEYSIGN;
        public static bool LOCALE_AT_START;
        public static int LOCALE_SCALE_FACTOR;
        public static void InitWriting()
        { // Setup necessary for functions in Writing to work
            // Enable ANSI codes -- Original code from https://stackoverflow.com/a/43078669
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll")]
            static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32.dll")]
            static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
            var handle = GetStdHandle(-11);
            GetConsoleMode(handle, out uint mode);
            mode |= 4;
            SetConsoleMode(handle, mode);

            LOCALE_CURRENCY = knownCurrencies[LOCALE];
            LOCALE_MONEYSIGN = knownMoneySigns[LOCALE_CURRENCY];
            LOCALE_AT_START = knownAtStarts[LOCALE_CURRENCY];
            LOCALE_SCALE_FACTOR = knownScaleFactors[LOCALE_CURRENCY];
        }
        static readonly Dictionary<string, ConsoleColor> colourNameLookup = new Dictionary<string, ConsoleColor>()
        {
            // Player colours:
            { "blue", ConsoleColor.Blue },
            { "red", ConsoleColor.Red },
            { "green", ConsoleColor.Green },
            { "yellow", ConsoleColor.Yellow },

            // Additional colours:
            { "white", ConsoleColor.White },
            { "null", ConsoleColor.White },
            { "grey", ConsoleColor.Gray },
            { "dark grey", ConsoleColor.DarkGray },
            { "dark yellow", ConsoleColor.DarkYellow },

            // Special colours:
            { "error", ConsoleColor.DarkRed },
            { "command output", ConsoleColor.Gray },
            { "command", ConsoleColor.DarkGray }, // For example commands, rather than command names
            { "variable", ConsoleColor.DarkGray },
            //{ "prev", previously used colour } // Not a ConsoleColor, but does work the same way as the others, so it's listed here
            { "tip", ConsoleColor.Gray },

        };
        //const string ANSI_RESET = 
        public static void Write(string text) // TODO: Rewrite to use regex regexr.com/8li29
        { // Alternative to Console.Write that supports coloured text being written using colour codes e.g. [blue], [red]
            ConsoleColor colour = ConsoleColor.White;
            Stack<ConsoleColor> prev_colours = new Stack<ConsoleColor>();
            string currentANSIFormatting = "";
            string textCache = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (
                    c == '[' 
                    && (i < 4 || text[i - 1] != '\u001b')  // Prevent ANSI escape sequences (for underline) from getting treated as colour codes
                    )
                {
                    // Clear cache
                    WriteColour(textCache, colour);
                    textCache = "";

                    string newColourName = "";
                    i++;
                    c = text[i];
                    while (c != ']')
                    {
                        newColourName += c;
                        i++;
                        c = text[i];
                    }
                    if (colourNameLookup.ContainsKey(newColourName))
                    {
                        prev_colours.Push(colour);
                        colour = colourNameLookup[newColourName];
                    }
                    else if (newColourName == "prev" && prev_colours.Count > 0) // If [prev] is used with no prev to go back to, it's written as-is
                    {
                        colour = prev_colours.Pop();
                    }
                    else // Just regular text in [] e.g. [foo]
                    {
                        textCache += $"{currentANSIFormatting}[{newColourName}]";
                    }
                }
                else if (c == '\u001b')
                {
                    string ANSICode = "\u001b";
                    i++;
                    c = text[i];
                    while (c != 'm')
                    {
                        ANSICode += c;
                        i++;
                        c = text[i];
                    }
                    ANSICode += "m";
                    currentANSIFormatting = ANSICode;
                }
                else
                {
                    textCache += $"{currentANSIFormatting}{c}";
                }
            }
            WriteColour(textCache, colour);
        }
        public static void WriteLine(string text)
        { // Alternative to WriteLine allowing colour codes
            Write(text);
            Console.WriteLine(); // Using Console.WriteLine rather than appending an Environment.NewLine to make sure no functionality is lost
        }
        public static void WriteLine()
        { // Only exists to completely avoid Console.WriteLine()
            Console.WriteLine();
        }
        private static void WriteColour(string string_to_write, ConsoleColor colour) // Should only be used in Write() and Writeline()
        { // Writes an entire string in a certain colour and then resets it
            Console.ForegroundColor = colour;
            Console.Write(string_to_write);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void WriteColour(char char_to_write, ConsoleColor colour) // Should only be used in Write() and Writeline()
        { // Overload to allow WriteColour of chars
            WriteColour(Convert.ToString(char_to_write), colour);
        }
        public static string Underline(string string_to_underline) // Original code from https://stackoverflow.com/a/43078669
        { // Returns a string that, when printed, looks like string_to_underline with an underline
            return $"\x1B[4m{string_to_underline}\x1B[24m";
        }
        public static ConsoleColor[] colours = new ConsoleColor[4]{
            ConsoleColor.Blue,
            ConsoleColor.Green,
            ConsoleColor.Red,
            ConsoleColor.Yellow,
        };
        public static string[] colourNames = new string[4]
        {
            "blue",
            "green",
            "red",
            "yellow",
        };

        static readonly Dictionary<string, string> knownCurrencies = new Dictionary<string, string>()
        {
            { "en-GB", "GBP" },
            { "en-US", "USD" },
            { "jp-JP", "JPY" },
        };
        static readonly Dictionary<string, string> knownMoneySigns = new Dictionary<string, string>()
        {
            { "GBP", "£" },
            { "USD", "$" },
            { "JPY", "¥" },
        };
        static readonly Dictionary<string, bool> knownAtStarts = new Dictionary<string, bool>()
        {
            { "GBP", true },
            { "USD", true },
            { "JPY", true },
        };
        static readonly Dictionary<string, int> knownScaleFactors = new Dictionary<string, int>()
        {
            { "GBP", 1 },     // by definition
            { "USD", 1 },     // 1.36
            { "JPY", 216 },   // 215.87
        };
        private static string FormatMoney(int money, string moneySign, bool atStart, int scaleFactor) // Completely customisable
        {
            int scaledMoney = money * scaleFactor;
            string result = "";
            if (scaledMoney < 0)
            {
                result += "-";
                scaledMoney *= -1;
            }
            if (atStart)
            {
                result += moneySign;
            }
            result += Convert.ToString(scaledMoney);
            if (!atStart)
            {
                result += moneySign;
            }
            return result;
        }
        private static string FormatMoney(int money, string locale) // Any locale
        {
            string currency = knownCurrencies[locale];
            string moneySign = knownMoneySigns[currency];
            bool atStart = knownAtStarts[currency];
            int scaleFactor = knownScaleFactors[currency];
            return FormatMoney(money, moneySign, atStart, scaleFactor);
        }
        public static string FormatMoney(int money) // Current locale
        {
            return FormatMoney(money, LOCALE);
        }
        public static string FormatMoneyChange(int money_delta, bool gaining_good)
        {
            if (gaining_good == money_delta > 0)
            {
                return $"[green]{FormatMoney(money_delta)}[prev]";
            }
            else
            {
                return $"[red]{FormatMoney(money_delta)}[prev]";
            }
        }
        public static string FormatBalance(int balance, bool debt = false)
        {
            if (balance > 0 != debt)
            {
                return $"[white]{FormatMoney(balance)}[prev]";
            }
            else
            {
                return $"[red]{FormatMoney(balance)}[prev]";
            }
        }
    }
}