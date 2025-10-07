using System.Runtime.InteropServices;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.Saving;

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
            { "error", ConsoleColor.DarkRed },
            { "command output", ConsoleColor.Gray },

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
            { "load", "!load [string filename]\nIf filename is provided, loads the game saved with that filename. Otherwise, load the most recent save" },
            { "games", "!games\nLists all saved games and most recent modification" },
            { "name", "!name [string name]\nSets the current game's name to. Otherwise, returns the current game's name" }, // Need to make sure changing the name doesn't break things
            { "money", "!money set <int playerID> <int amount>\n!money add <int playerID> <int amount>\nAlters the amount of money a player has. To remove money, add a negative amount" },
            { "info", "!info <int playerID>\nShows information about a player" },
            { "anims", "!anims off\n!anims on\nToggles animations e.g. die rolling and other pauses. Default is on" },
            { "ai", "!ai <int playerID> <int AILevel>\nSets the AI level of a player" }
        };
        class GameState
        {
            public Player[] players;
            public Grapher grapher;
            public string currentGameName;
            public int turnCount;
        }
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
                                WriteLine("[error]!help only accepts one or zero parameters");
                            }
                            else
                            {
                                if (parameters.Length == 0)
                                {
                                    foreach (string key in commandHelp.Keys)
                                    {
                                        WriteLine($"[command output]!{key}{new string(' ', 10 - key.Length)}{commandHelp[key]}\n");
                                    }
                                }
                                else
                                {
                                    WriteLine(commandHelp[parameters[0]]);
                                }
                            }
                            break;
                        case "save": // Save the current state of the game to a file
                            if (parameters.Length > 1)
                            {
                                WriteLine("[error]!save only accepts zero or one parameters");
                            }
                                else
                                {
                                string saveName;
                                if (parameters.Length == 0)
                                    {
                                    saveName = currentGameName;
                                    }
                                else
                                {
                                    saveName = parameters[0];
                                }
                                if (!gameRunning)
                                {
                                    Console.WriteLine("[error]Game is over, cannot save");
                            }
                                GameState gameState = new GameState();
                                gameState.players = players;
                                gameState.grapher = grapher;
                                gameState.currentGameName = currentGameName;
                                gameState.turnCount = turnCount;
                                string saveFilePath = $"../../../Save Files/{saveName}/Gamestate.msg";
                                Serialise(gameState, saveFilePath); // .msg from MsgPack
                                WriteLine($"[command output]Saved to {saveFilePath}");
                            };
                            break;

                    }

                    userInput = null; // Reset the read since passing on the command would count as input e.g. for GUI mode toggle
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