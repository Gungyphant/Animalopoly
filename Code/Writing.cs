using System.Runtime.InteropServices;

namespace Animalopoly.Code
{
    class Writing
    {
        public static void InitWriting()
        {
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
        static Dictionary<string, ConsoleColor> knownConsoleColours = new Dictionary<string, ConsoleColor>()
        {
            { "blue", ConsoleColor.Blue },
            { "red", ConsoleColor.Red },
            { "green", ConsoleColor.Green },
            { "yellow", ConsoleColor.Yellow },
            { "white", ConsoleColor.White },
            { "null", ConsoleColor.White },
            { "grey", ConsoleColor.Gray },
            { "dark grey", ConsoleColor.DarkGray },
        };
        //const string ANSI_RESET = 
        public static void Write(string text)
        {
            // Allows for writing text containing (case-sensitive) colour codes e.g. [blue], [red]. [white] or [null] resets to normal
            ConsoleColor colour = ConsoleColor.White;
            string CurrentANSIFormatting = "";
            string textCache = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '[' && (i < 4 || text[i - 1] != '\u001b')) // Second part is to prevent ANSI escape sequences (for underline) from getting treated as colour codes
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
                    if (knownConsoleColours.ContainsKey(newColourName))
                    {
                        colour = knownConsoleColours[newColourName];
                    }
                    else // Just regular text in [] e.g. [foo]
                    {
                        textCache += $"{CurrentANSIFormatting}[{newColourName}]";
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
                    CurrentANSIFormatting = ANSICode;
                }
                else
                {
                    textCache += $"{CurrentANSIFormatting}{c}";
                }
            }
            WriteColour(textCache, colour);
        }
        public static void WriteLine(string text)
        {
            Write(text);
            Console.WriteLine();
        }
        private static void WriteColour(char c, ConsoleColor color) // Should only be used in Write() and Writeline()
        {
            WriteColour(Convert.ToString(c), color);
        }
        private static void WriteColour(string s, ConsoleColor color) // Should only be used in Write() and Writeline()
        {
            Console.ForegroundColor = color;
            Console.Write(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static Dictionary<string, string> commandHelp = new Dictionary<string, string>()
        {
            { "help", "!help [string command]\nShows information about a command, or, if command is not provided, shows a list of commands" },
            { "save", "!save [string filename]\nSaves the current game. If no filename is provided, the name is the game's name, set with !name" },
            { "load", "!load [string filename]\nIf filename is provided, loads the game saved with that filename. Otherwise, load the most unnamed save" },
            { "name", "!name <string name>\nSets the current game's name to name. Changing name mid-game is not recommended" },
            { "money", "!money set <int playerID> <int amount>\n!money add <int playerID> <int amount>\nAlter the amount of money a player has. To remove money, add a negative amount" },
            { "info", "!info <int playerID>\nShow information about a player" },
        };
        static string ReadLine()
        public static string ReadLine()
        {
            string? userInput;
            do
            {
                userInput = Console.ReadLine();
                if (userInput == null)
                {
                    continue;
                }
                if (userInput.Length > 1 && userInput[0] == '!') // Command has been entered
                {
                    string command = userInput[1..].Split(" ")[0];
                    string[] parameters = userInput[1..].Split(" ")[1..];
                    switch (command)
                    {
                        case "help": // Get help about a command
                            if (parameters.Length > 1)
                            {
                                WriteLine("!help only accepts one or zero parameters");
                            }
                            else
                            {
                                if (parameters.Length == 0)
                                {
                                    WriteLine(commandHelp[parameters[0]]);
                                }
                                else
                                {
                                    foreach (string key in commandHelp.Keys)
                                    {
                                        WriteLine($"!{key}\t{commandHelp[key]}");
                                    }
                                }
                            }
                            break;
                        //case "save": // Save the current state of the game to a file

                    }

                    userInput = null;
                }
            }
            while (userInput == null);
            return userInput;
        }
        public static string Underline(string s) // Original code from https://stackoverflow.com/a/43078669
        {
            return $"\x1B[4m{s}\x1B[24m";
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