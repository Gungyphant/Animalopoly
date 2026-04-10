using System.Runtime.InteropServices;

namespace Animalopoly.Code
{
    class Writing
    {
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
        { // Alternative to Console.Write that supports coloured text being written using colour codes e.g. [blue, red]
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
                    else if (newColourName == "prev" && prev_colours.Count > 0) // If [prev] is used with no prev to go back to, it's writted as-is
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
    }
}