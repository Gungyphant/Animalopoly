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
            { "anims", "!anims off\n!anims on\nToggles animations e.g. die rolling and other pauses. Default is on" },
            { "ai", "!ai <int player ID> <int AI level>\nSets the AI level of a player. [variable]AI level[prev] should be one of:\n 0 - no " +
                "AI\n 1 - easy AI\n 2 - medium AI\n 3 - hard AI\n 4 - expert AI" },
            { "trade", "!trade <int senderID> <int recipientID> <int money sent> <csv animals sent> [csv animals recieved]\nTrades with another " +
                "player. Trades should only be made with the recipient and the sender's permission. The recipient recieves £[variable]money " +
                "sent[prev] and the [variable]animals sent[prev], and in return the sender recieves the [variable]animals received[prev], if " +
                "present. If [variable]money sent[prev] is negative, the sender recieves money instead. [variable]animals sent[prev] and " +
                "[variable]animals received[prev] should be comma-separated lists. Cheat if an AI player is involved in the trade\ne.g. " +
                "[command]!trade 2 1 1500 2,3,7 10[prev] would cause the Player 2 to give Player 1 $1500, the Sparrow, the Hedgehog, and the Bat " +
                "in return for the Brown Bear" }, // TODO: money transfer
        };
        private static (int?, Player?) ParsePlayerID(string playerIDText)
        {
            int targetID;
            Player target;
            try
            {
                targetID = Convert.ToInt16(playerIDText) - 1;
                target = players[targetID];
            }
            catch
            {
                WriteLine("[error]Invalid player ID");
                return (null, null);
            }
            if (target is null)
            {
                WriteLine("[error]That player has not been named yet, please wait");
                return (targetID, null);
            }
            return (targetID, target);
        }
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
                    if (userInput.Contains(' '))
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
                                    (int? targetID, Player? target) = ParsePlayerID(parameters[1]);
                                    if  (targetID is null || target is null)
                                    {
                                        break;
                                    }

                                    int money;
                                    try
                                    {
                                        money = Convert.ToInt32(parameters[2]);
                                    }
                                    catch
                                    {
                                        WriteLine("[error]Invalid third parameter");
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
                                        (int? targetID, Player? target) = ParsePlayerID(parameters[1]);
                                        if (targetID is null || target is null)
                                        {
                                            break;
                                        }
                                        if (parameters.Length == 2)
                                        {
                                            WriteLine($"Player {targetID + 1} [{colourNames[(int)(targetID)]}]{target.GetName()}[prev] with £{target.GetMoney()}");
                                        }
                                        else
                                        {
                                            string[] args = parameters[2..];
                                            WriteLine($"Player {targetID}");
                                            if (args.Contains("n") || args.Contains("name"))
                                            {
                                                WriteLine($" [{colourNames[(int)(targetID - 1)]}]{target.GetName()}[prev]");
                                            }
                                            if (args.Contains("m") || args.Contains("money"))
                                            {
                                                WriteLine($" With £{target.GetMoney()}");
                                            }
                                            if (args.Contains("p") || args.Contains("properties"))
                                            {
                                                WriteLine($" The properties: {String.Join(", ", 
                                                    locations
                                                    .OfType<Animal>() // Non-Animal Tiles have no owner
                                                    .Where(animal => animal.GetOwner() == target)
                                                    .Select(
                                                        (animal, index) => $"{index} {animal.GetName()}"
                                                        )
                                                    )}");
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
                        case "anims":
                            if (parameters.Length == 1)
                            {
                                switch (parameters[0])
                                {
                                    case "on":
                                        animations = true;
                                        break;
                                    case "off":
                                        animations = false;
                                        break;
                                    default:
                                        WriteLine($"[error]Unknown parameter '{parameters[0]}'");
                                        break;
                                }
                            }
                            else
                            {
                                WriteLine("[error]!anims takes exactly one parameter");
                            }
                            break;
                        case "ai":
                            if (parameters.Length == 2)
                            {
                                (int? targetID, Player? target) = ParsePlayerID(parameters[0]);
                                if (targetID is null || target is null)
                                {
                                    break;
                                }
                                int AILevel;
                                try
                                {
                                    AILevel = Convert.ToInt16(parameters[1]);
                                    target.SetAILevel(AILevel);
                                }
                                catch
                                {
                                    WriteLine("[error]Invalid second parameter");
                                    break;
                                }
                            }
                            else
                            {
                                WriteLine("[error]!ai takes 2 parameters");
                            }
                            break;
                        case "trade":
                            if (parameters.Length == 4 || parameters.Length == 5)
                            {
                                (int? senderID, Player? sender) = ParsePlayerID(parameters[0]);
                                if (senderID is null || sender is null)
                                {
                                    break;
                                }
                                (int? recipientID, Player? recipient) = ParsePlayerID(parameters[1]);
                                if (recipientID is null || recipient is null)
                                {
                                    break;
                                }
                                int moneySent;
                                try
                                {
                                    moneySent = Convert.ToInt32(parameters[2]);
                                }
                                catch
                                {
                                    WriteLine("[error]Invalid [variable]money sent[prev]");
                                    break;
                                }
                                if (moneySent > 0)
                                {
                                    if (moneySent > sender.GetMoney())
                                    {
                                        WriteLine("[error]The sender does not have enough money for the trade");
                                        break;
                                    }
                                }
                                else
                                {
                                    if (-moneySent > recipient.GetMoney())
                                    {
                                        WriteLine("[error]The recipient does not have enough money for the trade");
                                        break;
                                    }
                                }
                                if (!cheats && (recipient.GetAILevel() > 0 || sender.GetAILevel() > 0))
                                {
                                    WriteLine("[error]!trade is a cheat when trading with AIs, and cheats are disabled");
                                    break;
                                }
                                Animal[] animalsSent = new Animal[parameters[3].Split(",").Length];
                                bool failed = false;
                                int i = 0;
                                foreach (string animalIDstr in parameters[3].Split(","))
                                {
                                    int animalID;
                                    Tile tile;
                                    try
                                    {
                                        animalID = Convert.ToInt32(animalIDstr);
                                        tile = locations[animalID];
                                    }
                                    catch
                                    {
                                        WriteLine($"[error]Invalid animal ID '{animalIDstr}'");
                                        failed = true;
                                        break;
                                    }
                                    if (tile is not Animal animal)
                                    {
                                        WriteLine($"[error]{tile.GetFormattedName()} is not an animal and cannot be owned");
                                        failed = true;
                                        break;
                                    }
                                    if (animal.GetOwner() != sender)
                                    {
                                        WriteLine($"[error]Sender does not own {tile.GetFormattedName()}");
                                        failed = true;
                                        break;
                                    }
                                    animalsSent[i] = animal;
                                    i++;
                                }
                                if (failed)
                                {
                                    break;
                                }
                                Animal[] animalsRecieved;
                                if (parameters.Length == 5)
                                {
                                    animalsRecieved = new Animal[parameters[4].Split(",").Length];
                                    failed = false;
                                    i = 0;
                                    foreach (string animalIDstr in parameters[4].Split(","))
                                    {
                                        int animalID;
                                        Tile tile;
                                        try
                                        {
                                            animalID = Convert.ToInt32(animalIDstr);
                                            tile = locations[animalID];
                                        }
                                        catch
                                        {
                                            WriteLine($"[error]Invalid animal ID '{animalIDstr}'");
                                            failed = true;
                                            break;
                                        }
                                        if (tile is not Animal animal)
                                        {
                                            WriteLine($"[error]{tile.GetFormattedName()} is not an animal and cannot be owned");
                                            failed = true;
                                            break;
                                        }
                                        if (animal.GetOwner() != recipient)
                                        {
                                            WriteLine($"[error]Recipient does not own {tile.GetFormattedName()}");
                                            failed = true;
                                            break;
                                        }
                                        animalsRecieved[i] = animal;
                                        i++;
                                    }
                                }
                                else
                                {
                                    animalsRecieved = [];
                                }
                                sender.ChangeMoney(-moneySent);
                                recipient.ChangeMoney(moneySent);
                                foreach (Animal givenAnimal in animalsSent)
                                {
                                    givenAnimal.SetOwner(ref recipient);
                                }
                                foreach (Animal takenAnimal in animalsRecieved)
                                {
                                    takenAnimal.SetOwner(ref sender);
                                }
                            }
                            else
                            {
                                WriteLine("[error]!trade takes 3 or 4 parameters");
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
