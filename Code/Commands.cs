using System.Linq.Expressions;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.Saving;
using static Animalopoly.Code.Writing;
using static Animalopoly.Code.TileClasses;

namespace Animalopoly.Code
{
    class Commands
    {
        public static bool cheats = false;
        static Dictionary<string, string> commandHelp = new Dictionary<string, string>()
        {
            { "help", "!help [string command]\nShows information about a command, or, if command is not provided, shows a list of commands" },
            { "save", "!save [string filename]\nSaves the current game. If [variable]filename[prev] is not provided, the name is the game's " +
                "name, set with !name" },
            { "load", "!load [string filename]\nIf [variable]filename[prev] is provided, loads the game saved with that filename. Otherwise, " +
                "load the most recent save" },
            { "graph", "!graph [string graphname]\nGenerates the money graph, in the savefile [variable]graphname[prev] if provided, otherwise " +
                "in the current save file" },
            { "games", "!games\nLists all saved games and their most recent save" },
            { "name", "!name [string name]\nIf [variable]name[prev] is provided, sets the current game's name. Otherwise, returns the current " +
                "game's name. To set a name containing spaces, put [variable]name[prev] in quotes" }, // Need to make sure changing the name doesn't break things
            { "cheats", "!cheats\n!cheats on\nQueries or enables cheat commands. Cheats cannot be disabled" },
            { "money", "!money set <int player ID> <int amount>\n!money add <int playerID> <int amount>\nAlters the amount of money a player " +
                "has. To remove money, add a negative amount. Cheat" },
            { "info", "!info player <int player ID> [*parameters]\nShows information about a player. If [variable]parameters[prev] are provided, " +
                "specific information will be given in more detail\nValid parameters:\nn name\tPlayer name\nm money\tPlayer's current money\n" +
                "p properties\tPlayer's current properties\nl location\tPlayer's current tile\ns skipped\tIf the player's turn will be " +
                "skipped" }, // TODO: !info for tiles
            //{ "anims", "!anims off\n!anims on\nToggles animations e.g. die rolling and other pauses. Default is on" },
            //{ "ai", "!ai <int player ID> <int AI level>\nSets the AI level of a player. [variable]AI level[prev] should be one of:\n 0 - no " +
                //"AI\n 1 - easy AI\n 2 - medium AI\n 3 - hard AI\n4 - expert AI" },
            //{ "trade", "!trade <int recipientID> <int money sent> <csv animals sent> [csv animals recieved]\nTrades with another player. " +
            //    "The trade is initiated by the current player; trades should only be made with the recipient's permission. The recipient " +
            //    "recieves $[variable]money sent[prev] and the [variable]animals sent[prev], and in return the initiator recieves the " +
            //    "[variable]animals received[prev]. If [variable]money sent[prev] is negative, the initiator recieves money instead. " +
            //    "[variable]animals sent[prev] and [variable]animals received[prev] should be comma-separated lists. Cheat if trading" +
            //    "with an AI player\ne.g. [command]!trade 1 1500 2,3,7 10[prev] would cause the current player to give Player 1 $1500, " +
            //    "the Sparrow, the Hedgehog, and the Bat in return for the Brown Bear" },
        };
        public static string? ReadLine() // ReadLine can only return null if a command set abort to true to exit early
        {
            string? userInput;
            bool abort = false;
            do
            {
                userInput = Console.ReadLine();
                if (userInput == null)
                {
                    continue;
                }
                if (userInput.Length > 1 && userInput[0] == '!') // Command has been entered
                {
                    userInput = userInput.Trim();
                    string command;
                    string[] parameters;
                    if (userInput.Contains(" "))
                    {
                        command = userInput[1..userInput.IndexOf(" ")];
                        string parameter_section = userInput[(userInput.IndexOf(" ") + 1)..];
                        string[] splitPhrases = parameter_section.Split("\"", StringSplitOptions.RemoveEmptyEntries);

                        bool isString = parameter_section[0] == '"';
                        List<string> parameterList = new List<string>(); // Uses a List rather than an array since the number of parameters is unknown
                        foreach (string phrase in splitPhrases)
                        {
                            if (isString)
                            {
                                parameterList.Add(phrase);
                            }
                            else
                            {
                                parameterList.AddRange(phrase.Split(" "));
                            }
                        }
                        parameters = parameterList.ToArray();
                    }
                    else
                    {
                        command = userInput[1..];
                        parameters = [];
                    }
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
                                        WriteLine($"[command output]!{key}:");
                                        foreach (string line in commandHelp[key].Split("\n"))
                                        {
                                            WriteLine($"{new string(' ', 10 - key.Length)}[command output]{line}");
                                        }
                                        WriteLine();
                                    }
                                }
                                else if (commandHelp.ContainsKey(parameters[0]))
                                {
                                    WriteLine($"[command output]{commandHelp[parameters[0]]}");
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
                                    saveName = parameters[0];
                                }
                                saveName = saveName.Replace("/", " ").Replace(":", "_").Replace("..", "."); // Manual replacements
                                foreach (char badChar in Path.GetInvalidFileNameChars()) // Automatic replacements of everything else
                                {
                                    saveName = saveName.Replace(badChar, '-');
                                }
                                if (!gameRunning)
                                {
                                    WriteLine("[error]Game is over, cannot save");
                                }
                                GameState gameState = new GameState();
                                gameState.players = players;
                                gameState.grapher = grapher;
                                gameState.currentGameName = currentGameName;
                                gameState.turnCount = turnCount;
                                gameState.cheats = cheats;
                                string saveFilePath = $"../../../Save Files/{saveName}/Gamestate.msg"; // .msg from MessagePack
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
                                        DateTime created = fi1.LastWriteTime; // TODO: get from info.csv

                                        if (File.Exists($"{subdir}/Gamestate.msg") && created > lastHigh)
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
                                try
                                {
                                    GameState gamestate = Deserialise<GameState>(saveFilePath);
                                    players = gamestate.players;
                                    grapher = gamestate.grapher;
                                    currentGameName = gamestate.currentGameName;
                                    turnCount = gamestate.turnCount;
                                    WriteLine($"[command output]Loaded save {saveName}");
                                }
                                catch (FileNotFoundException e)
                                {
                                    WriteLine($"[error]Deserialise raised {e.Message}");
                                }
                            }
                            abort = true;
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
                        case "name":
                            if (parameters.Length == 0)
                            {
                                WriteLine($"[command output]{currentGameName}");
                            }
                            else if (parameters.Length == 1)
                            {
                                currentGameName = parameters[0];
                            }
                            else
                            {
                                WriteLine("[error]!name only accepts 0 or 1 parameters");
                            }
                            break;
                        case "cheats":
                            if (parameters.Length == 0)
                            {
                                WriteLine($"Cheats are {(cheats ? "on" : "off")}");
                            }
                            else if (parameters.Length == 1)
                            {
                                if (parameters[0] == "on")
                                {
                                    cheats = true;
                                    WriteLine("Cheats are now enabled for this save");
                                }
                                else
                                {
                                    WriteLine("[error]Cheats cannot be disabled");
                                }
                            }
                            else
                            {
                                WriteLine("[error]!cheats only accepts one parameter");
                            }
                            break;
                        case "money":
                            if (!cheats)
                            {
                                WriteLine("[error]!money is a cheat, and cheats are disabled");
                            }
                            else
                            {
                                if (parameters.Length == 3)
                                {
                                    int targetID;
                                    try
                                    {
                                        targetID = Convert.ToInt16(parameters[1]);
                                    }
                                    catch
                                    {
                                        WriteLine("[error]Invalid second parameter to !money");
                                        break;
                                    }
                                    Player target = players[targetID];

                                    int money;
                                    try
                                    {
                                        money = Convert.ToInt32(parameters[2]);
                                    }
                                    catch
                                    {
                                        WriteLine("[error]Invalid third parameter to !money");
                                        break;
                                    }
                                    switch (parameters[0])
                                    {
                                        case "set":
                                            target.ChangeMoney(money - target.GetMoney());
                                            break;
                                        case "add":
                                            target.ChangeMoney(money);
                                            break;
                                        default:
                                            WriteLine($"[error] Unknown first parameter to !money '{parameters[0]}'");
                                            break;
                                    }
                                }
                                else
                                {
                                    WriteLine("[error]!money only accepts two parameters");
                                }
                            }
                            break;
                        case "info":
                            if (parameters.Length >= 2)
                            {
                                switch (parameters[0])
                                {
                                    case "player":
                                        int targetID;
                                        try
                                        {
                                            targetID = Convert.ToInt16(parameters[1]);
                                        }
                                        catch
                                        {
                                            WriteLine("Invalid player ID");
                                            break;
                                        }
                                        Player target = players[targetID];
                                        if (parameters.Length == 2)
                                        {
                                            WriteLine($"Player {targetID} [{colourNames[targetID]}]{target.GetName()}[prev] with ${target.GetMoney()}");
                                        }
                                        else
                                        {
                                            string[] args = parameters[2..];
                                            WriteLine($"Player {targetID}");
                                            if (args.Contains("n") || args.Contains("name"))
                                            {
                                                WriteLine($" [{colourNames[targetID]}]{target.GetName()}[prev]");
                                            }
                                            if (args.Contains("m") || args.Contains("money"))
                                            {
                                                WriteLine($" With ${target.GetMoney()}");
                                            }
                                            if (args.Contains("p") || args.Contains("properties"))
                                            {
                                                WriteLine($" The properties: {String.Join(", ", (from animal in locations where animal.GetOwner() == target select animal.GetName()))}");
                                            }
                                            if (args.Contains("l") || args.Contains("location"))
                                            {
                                                WriteLine($" At square {locations[target.GetPos()].GetFormattedName()}");
                                            }
                                            if (args.Contains("s") || args.Contains("skipped"))
                                            {
                                                WriteLine($" Next turn will {(target.GetSkip() ? "" : "not")} be skipped");
                                            }
                                        }
                                        break;
                                    case "animal":
                                        // TODO: this
                                        break;
                                    default:
                                        WriteLine($"[error]Unknown first parameter for !info '{parameters[0]}'");
                                        break;
                                }
                            }
                            else
                            {
                                WriteLine("[error]!info requires 2+ parameters");
                            }
                            break;
                        default:
                            WriteLine($"[error]Unknown command '{command}'");
                            break;
                    }

                    userInput = null; // Reset the read since passing on the command would count as input e.g. for GUI mode toggle
                }
            }
            while (!abort && userInput == null);

            return userInput;
        }
    }
}
