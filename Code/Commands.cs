using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.Saving;
using static Animalopoly.Code.Writing;
using static Animalopoly.Code.TileClasses;

namespace Animalopoly.Code
{
    class Commands
    {
        public static bool animations { get; private set; } = true;
        public static bool cheats { get; private set; } = false;
        static readonly Dictionary<string, string> commandHelp = new Dictionary<string, string>()
        {
            // command help should fit this regex: regexr.com/8lgn5
            { "help", "!help [string command]\nShows information about a command, or, if command is not provided, shows a list of commands\n" +
                "Parameters in [square brackets] are optional, and parameters in <angle brackets> are mandatory" },
            { "save", "!save [string filename]\nSaves the current game. If [variable]filename[prev] is not provided, the name is the game's " +
                "name, set with !name" },
            { "load", "!load [string filename]\nIf [variable]filename[prev] is provided, loads the game saved with that filename. Otherwise, " +
                "load the most recent save" },
            { "graph", "!graph [string graph name] [<int width> <int height>]\n!graph [<int width> <int height>]\nGenerates the money graph, in the " +
                "savefile [variable]graph name[prev] if provided, otherwise in the current save file. If provided, [variable]width[prev] and " +
                "[variable]height[prev] are the dimensions of the generated image" },
            { "games", "!games\nLists all saved games and their most recent save" },
            { "name", "!name [string name]\nIf [variable]name[prev] is provided, sets the current game's name. Otherwise, returns the current " +
                "game's name. To set a name containing spaces, put [variable]name[prev] in quotes" }, // Need to make sure changing the name doesn't break things
            { "cheats", "!cheats\n!cheats on\nQueries or enables cheat commands. Cheats cannot be disabled once they have enabled" },
            { "money", "!money set <int player ID> <int amount>\n!money add <int playerID> <int amount>\nAlters the amount of money a player " +
                "has. To remove money, add a negative amount. Cheat" },
            { "info", "!info player <int player ID> [*parameters]\nShows information about a player. If [variable]parameters[prev] are provided, " +
                "specific information will be given in more detail\nValid parameters:\nn name\tPlayer name\nm money\tPlayer's current money\n" +
                "p properties\tPlayer's current properties\nl location\tPlayer's current tile\ns skipped\tIf the player's turn will be " +
                "skipped\n!info animal <int animal ID>\nShows the card for the animal [variable]animal ID[prev]" },
            { "anims", "!anims off\n!anims on\nToggles animations e.g. die rolling and other pauses. Default is on" },
            { "ai", "!ai <int player ID> <int AI level>\nSets the AI level of a player. [variable]AI level[prev] should be one of:\n 0 - no " +
                "AI\n 1 - easy AI\n 2 - medium AI\n 3 - hard AI\n 4 - expert AI" },
            { "trade", "!trade <int senderID> <int recipientID> <int money sent> <csv animals sent> [csv animals recieved]\nTrades with another " +
                $"player. Trades should only be made with the recipient and the sender's permission. The recipient recieves {LOCALE_MONEYSIGN}[variable]money " +
                "sent[prev] and the [variable]animals sent[prev], and in return the sender recieves the [variable]animals received[prev], if " +
                "present. If [variable]money sent[prev] is negative, the sender recieves money instead. [variable]animals sent[prev] and " +
                "[variable]animals received[prev] should be comma-separated lists. Cheat if an AI player is involved in the trade\ne.g. " +
                $"[command]!trade 2 1 1500 2,3,7 10[prev] would cause the Player 2 to give Player 1 {LOCALE_MONEYSIGN}1500, the Sparrow, the Hedgehog, and the Bat " +
                "in return for the Brown Bear" }, // TODO: money transfer
            //{ "setowner", "!setowner <int animal ID> [int new owner ID]\nSets the owner of animal #[variable]animal ID[prev] to be player " +
            //    "#[variable]new owner ID[prev], or, if none is provided, to have no owner" }
        };
        private static (int?, Player?) ParsePlayerID(string playerIDText)
        { // Converts playerIDText to an int and returns the processed (0-indexed) ID and the player for the (1-indexed) ID provided. If the ID is invalid, null will be returned for the output(s) that could not be determined
            int targetID;
            Player target;
            try
            {
                targetID = Convert.ToInt16(playerIDText) - 1;
                target = players[targetID];
            }
            catch
            {
                WriteLine($"[error]Invalid player ID '{playerIDText}'");
                return (null, null);
            }
            if (target is null)
            {
                WriteLine($"[error]Player {targetID} has not been named yet, please wait");
                return (targetID, null);
            }
            return (targetID, target);
        }
        private static (int?, Animal?) ParseAnimalID(string animalIDText)
        { // Similar to ParsePlayerID but for Animals, and with an additional check that the Tile is an Animal
            int targetID;
            Tile target;
            try
            {
                targetID = Convert.ToInt16(animalIDText);
                target = locations[targetID];
            }
            catch
            {
                WriteLine($"[error]Invalid animal ID '{animalIDText}'");
                return (null, null);
            }
            if (target is not Animal animalTarget)
            {
                WriteLine($"[error]The tile '{target.GetFormattedName()}' is not an animal");
                return (targetID, null);
            }
            return (targetID, animalTarget);
        }
        private static int? ParseInt(string intText)
        {
            int result;
            try
            {
                result = Convert.ToInt32(intText);
            }
            catch
            {
                WriteLine($"[error]Invalid int '{intText}'");
                return null;
            }
            return result;
        }
        private static int? ParseNonNegativeInt(string intText)
        {
            int? result = ParseInt(intText);
            if (result is null)
            {
                return null;
            }
            if (result < 0)
            {
                WriteLine($"[error]Unexpected negative int '{result}'");
                return null;
            }
            return result;
        }
        private static int? ParseMoney(string moneyText, bool isNonNegative = true)
        {
            int? unscaledResult;
            if (isNonNegative)
            {
                unscaledResult = ParseNonNegativeInt(moneyText);
            }
            else
            {
                unscaledResult = ParseInt(moneyText);
            }
            if (unscaledResult is null)
            {
                return null;
            }
            if (unscaledResult % LOCALE_SCALE_FACTOR != 0)
            {
                WriteLine($"[error]Money must be a multiple of {LOCALE_SCALE_FACTOR}");
                return null;
            }
            return unscaledResult / LOCALE_SCALE_FACTOR;
        }
        public static string CleanSaveName(string saveName)
        {
            foreach (char badChar in Path.GetInvalidFileNameChars())
            {
                saveName = saveName.Replace(badChar, '-');
            }
            return saveName;
        }
        public static string ReadLine()
        {
            string? userInput;
            bool abort = false;
            do
            {
                userInput = Console.ReadLine();
                
                if (userInput is null)
                {
                    continue;
                }
                if (userInput.Length > 1 && userInput[0] == '!') // Command has been entered
                {
                    userInput = userInput.Trim(); // Remove all trailing whitespace
                    string command;
                    string[] parameters;
                    if (userInput.Contains(' '))
                    {
                        command = userInput[1..userInput.IndexOf(' ')];
                        string parameterSection = userInput[(userInput.IndexOf(' ') + 1)..];
                        string[] splitPhrases = parameterSection.Split("\"", StringSplitOptions.RemoveEmptyEntries);

                        bool isQuotedParameter = parameterSection[0] == '"';
                        List<string> parameterList = new List<string>(); // Uses a List rather than an array since the number of parameters is unknown
                        foreach (string phrase in splitPhrases)
                        {
                            if (isQuotedParameter)
                            {
                                parameterList.Add(phrase);
                            }
                            else
                            {
                                // if phrase is not a quoted parameter, it is a list of space-separated parameters
                                parameterList.AddRange(phrase.Split(" ", StringSplitOptions.RemoveEmptyEntries)); 
                            }
                            isQuotedParameter = !isQuotedParameter;
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
                                            WriteLine($"    {(line[0] == '!' ? "" : " ")}[command output]{line}"); // one-line if to put an extra space for descriptions to make them obviously different from command signatures
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
                                saveName = saveName.Replace("/", " ").Replace(":", "_"); // Manual replacements
                                saveName = CleanSaveName(saveName); // Automatic replacements of everything else
                                if (!gameRunning)
                                {
                                    WriteLine("[error]Game is over, cannot save");
                                }
                                GameState gameState = new GameState(players, grapher, currentGameName, turnCount, cheats);
                                string saveFilePath = $"../../../Save Files/{saveName}/Gamestate.msg"; // .msg from MessagePack
                                try
                                {
                                    Serialise(gameState, saveFilePath);
                                }
                                catch
                                {
                                    WriteLine($"[error]Invalid saveFilePath '{saveFilePath}'");
                                    break;
                                }
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
                                    saveName = CleanSaveName(parameters[0]);
                                }

                                string saveFilePath = $"../../../Save Files/{saveName}/Gamestate.msg";
                                try
                                {
                                    GameState gamestate = Deserialise<GameState>(saveFilePath);
                                    players = gamestate.GetPlayers();
                                    grapher = gamestate.GetGrapher();
                                    currentGameName = gamestate.GetCurrentGameName();
                                    turnCount = gamestate.GetTurnCount();
                                    WriteLine($"[command output]Loaded save {saveName}");
                                }
                                catch (Exception e)
                                {
                                    WriteLine($"[error]Deserialise raised {e.Message}");
                                }
                            }
                            abort = true;
                            break;
                        case "graph":
                            string graphName;
                            switch (parameters.Length)
                            {
                                case 0: // !graph
                                    graphName = currentGameName;
                                    grapher.GenerateGraph($"../../../Save files/{graphName}/Money graph.png");
                                    break;
                                case 1: // !graph graph_name
                                    graphName = CleanSaveName(parameters[0]);
                                    grapher.GenerateGraph($"../../../Save files/{graphName}/Money graph.png");
                                    break;
                                case 2: // !graph width height
                                    graphName = currentGameName;
                                    if (ParseNonNegativeInt(parameters[0]) is not int width)
                                    {
                                        break;
                                    }
                                    if (ParseNonNegativeInt(parameters[1]) is not int height)
                                    {
                                        break;
                                    }
                                    grapher.GenerateGraph($"../../../Save files/{graphName}/Money graph.png", width, height);
                                    break;
                                case 3: // !graph graph_name width height
                                    graphName = CleanSaveName(parameters[0]);
                                    if (ParseNonNegativeInt(parameters[1]) is not int width2) // can't be called width since it's in the same scope as the previous, and there's no way to do a combined compare-and-assign to a previously declared variable
                                    {
                                        break;
                                    }
                                    if (ParseNonNegativeInt(parameters[2]) is not int height2)
                                    {
                                        break;
                                    }
                                    grapher.GenerateGraph($"../../../Save files/{graphName}/Money graph.png", width2, height2);
                                    break;
                                default:
                                    WriteLine("[error]!graph only accepts up to 3 parameters");
                                    break;
                            }
                            break;
                        case "games": // TODO: implement
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

                                    if (ParseMoney(parameters[2], false) is not int money)
                                    {
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
                                            WriteLine($"[command output]Player {targetID + 1} [{colourNames[(int)targetID]}]{target.GetName()}[prev] with {FormatBalance(target.GetMoney())}");
                                        }
                                        else
                                        {
                                            string[] args = parameters[2..];
                                            WriteLine($"[command output]Player {targetID + 1}");
                                            if (args.Contains("n") || args.Contains("name"))
                                            {
                                                WriteLine($" [{colourNames[(int)(targetID)]}]{target.GetName()}[prev]");
                                            }
                                            if (args.Contains("m") || args.Contains("money"))
                                            {
                                                WriteLine($" [command output]With {FormatBalance(target.GetMoney())}");
                                            }
                                            if (args.Contains("p") || args.Contains("properties"))
                                            {
                                                WriteLine($" [command output]With the properties: {String.Join(", ", 
                                                    locations
                                                    .OfType<Animal>() // Non-Animal Tiles have no owner
                                                    .Where(animal => animal.GetOwner() == target)
                                                    .Select(animal => animal.GetName())
                                                )}");
                                            }
                                            if (args.Contains("l") || args.Contains("location"))
                                            {
                                                WriteLine($" [command output]At square {locations[target.GetPos()].GetFormattedName()}");
                                            }
                                            if (args.Contains("s") || args.Contains("skipped"))
                                            {
                                                WriteLine($" [command output]Next turn will {(target.GetSkip() ? "" : "not ")}be skipped");
                                            }
                                        }
                                        break;
                                    case "animal":
                                        (int? targetAnimalID, Animal? targetAnimal) = ParseAnimalID(parameters[1]);
                                        if (targetAnimalID is null || targetAnimal is null)
                                        {
                                            break;
                                        }
                                        WriteLine(targetAnimal.GetCard());
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
                                        WriteLine($"[error]Unknown parameter for !anims '{parameters[0]}'");
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
                                if (ParseMoney(parameters[2], false) is not int moneySent)
                                {
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
                                string[] splitAnimalsSentParameter = parameters[3].Split(",");
                                Animal[] animalsSent = new Animal[splitAnimalsSentParameter.Length];
                                bool failed = false;
                                int i = 0;
                                foreach (string animalIDstr in splitAnimalsSentParameter)
                                {
                                    (int? animalID, Animal? animal) = ParseAnimalID(animalIDstr);
                                    if (animalID is null || animal is null)
                                    {
                                        failed = true;
                                        break;
                                    }
                                    if (animal.GetOwner() != sender)
                                    {
                                        WriteLine($"[error]Sender does not own {animal.GetFormattedName()}");
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
                                    string[] splitAnimalsRecievedParameter = parameters[4].Split(",");
                                    animalsRecieved = new Animal[splitAnimalsRecievedParameter.Length];
                                    failed = false;
                                    i = 0;
                                    foreach (string animalIDstr in splitAnimalsRecievedParameter)
                                    {
                                        (int? animalID, Animal? animal) = ParseAnimalID(animalIDstr);
                                        if (animalID is null || animal is null)
                                        {
                                            failed = true;
                                            break;
                                        }
                                        if (animal.GetOwner() != recipient)
                                        {
                                            WriteLine($"[error]Recipient does not own {animal.GetFormattedName()}");
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
            while (!abort && userInput is null);

            if (userInput is null)
            {
                throw new Exception("Abort called");
            }

            return userInput;
        }
    }
}
