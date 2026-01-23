using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.Saving;
using static Animalopoly.Code.Writing;

namespace Animalopoly.Code
{
    class Commands
    {
        static Dictionary<string, string> commandHelp = new Dictionary<string, string>()
        {
            { "help", "!help [string command]\nShows information about a command, or, if command is not provided, shows a list of commands" },
            { "save", "!save [string filename]\nSaves the current game. If no filename is provided, the name is the game's name, set with !name" },
            { "load", "!load [string filename]\nIf filename is provided, loads the game saved with that filename. Otherwise, load the most recent save" },
            { "graph", "!graph [string graphname]\nGenerates the money graph, in the savefile graphname if provided" },
            { "games", "!games\nLists all saved games and most recent modification" },
            //{ "name", "!name [string name]\nIf name is provided, sets the current game's name. Otherwise, returns the current game's name" }, // Need to make sure changing the name doesn't break things
            //{ "money", "!money set <int playerID> <int amount>\n!money add <int playerID> <int amount>\nAlters the amount of money a player has. To remove money, add a negative amount" },
            //{ "info", "!info <int playerID>\nShows information about a player" },
            //{ "anims", "!anims off\n!anims on\nToggles animations e.g. die rolling and other pauses. Default is on" },
            //{ "ai", "!ai <int playerID> <int AILevel>\nSets the AI level of a player" }
        };
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
                                else if (commandHelp.ContainsKey(parameters[0]))
                                {
                                    WriteLine(commandHelp[parameters[0]]);
                                }
                                else
                                {
                                    WriteLine($"[error]Unknown command {parameters[0]}");
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
                                    saveName = parameters[0].Replace("/", " ").Replace(":", "_").Replace("..", ".");
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
                                string saveFilePath = $"../../../Save Files/{saveName}/Gamestate.msg"; // .msg from MsgPack
                                Serialise(gameState, saveFilePath);
                                WriteLine($"[command output]Saved to {saveFilePath}");
                            }
                            break;
                        case "load": // Load a previous game state
                            if (parameters.Length > 1)
                            {
                                WriteLine("[error]!load only accepts zero or one parameters");
                            }
                            else
                            {
                                string saveName;
                                if (parameters.Length == 0)
                                {
                                    // Following code from https://stackoverflow.com/a/2941326
                                    DateTime lastHigh = new DateTime(1900, 1, 1);
                                    string highDir = "";
                                    foreach (string subdir in Directory.GetDirectories("../../../Save Files"))
                                    {
                                        DirectoryInfo fi1 = new DirectoryInfo(subdir);
                                        DateTime created = fi1.LastWriteTime;

                                        if (created > lastHigh)
                                        {
                                            highDir = subdir;
                                            lastHigh = created;
                                        }
                                    }
                                    if (highDir == "")
                                    {
                                        WriteLine("[error]!load was called with no parameters, however no saved games can be found");
                                        break;
                                    }
                                    saveName = highDir.Split(Path.DirectorySeparatorChar).Last();
                                }
                                else
                                {
                                    saveName = parameters[0];
                                }

                                string saveFilePath = $"../../../Save Files/{saveName}/Gamestate.msg";
                                GameState gamestate = Deserialise<GameState>(saveFilePath);
                                players = gamestate.players;
                                grapher = gamestate.grapher;
                                currentGameName = gamestate.currentGameName;
                                turnCount = gamestate.turnCount;
                                WriteLine($"[command output]Loaded save {saveName}");
                            }
                            break;
                        case "graph":
                            if (parameters.Length > 1)
                            {
                                WriteLine("[error]!graph only accepts one or zero parameters");
                            }
                            else
                            {
                                string graphName;
                                if (parameters.Length == 0)
                                {
                                    graphName = currentGameName;
                                }
                                else
                                {
                                    graphName = parameters[0];
                                }
                                grapher.GenerateGraph($"../../../Save files/{graphName}/Money graph.png");
                            }
                            break;
                        case "games":
                            if (parameters.Length > 0) 
                            {
                                WriteLine("[error]!games does not accept parameters");
                            }
                            string[] gameDirs = Directory.GetDirectories("../../../Save Files");
                            WriteLine("Game\tCreated\tSaved");
                            foreach (string gameDir in gameDirs)
                            {
                                //if (File.Exists())
                            }
                            break;
                        default:
                            WriteLine($"[error]Unknown command {command}");
                            break;
                    }

                    userInput = null; // Reset the read since passing on the command would count as input e.g. for GUI mode toggle
                }
            }
            while (userInput == null);

            return userInput;
        }
    }
}
