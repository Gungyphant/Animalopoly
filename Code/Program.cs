using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Commands;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.NetProcessingUI;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Saving;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;


namespace Animalopoly.Code
{
    class Program
    {
        const int PLAYER_COUNT = 4;
        const int INFO_VER = 2; // This needs to be incremented whenever the format of info.csv is updated
        // The following variables are public so that they can be accessed by commands, particularly saving; where possible, they are private set so don't have the typical drawbacks of global variables
        public static Player[] players = new Player[PLAYER_COUNT];
        public static bool gameRunning { get; private set; }
        public static Grapher grapher;
        public static SafeFilePath currentGameName;
        public static int turnCount;
        public static bool guiMode { get; private set; }
        public static DateTime startTime { get; private set; }
        static long DateTimeToTimestamp(DateTime time) // From: https://aske.wachs.dk/06/07/2021/c-conversion-between-unix-timestamps-and-datetime/
        {
            return ((DateTimeOffset)time).ToUnixTimeMilliseconds();
        }
        static void Main()
        {
            // Testing:
            //Console.WriteLine("Testing:"); // Console.WriteLine rather than WriteLine since InitWriting hasn't been called yet
            //Console.WriteLine($"13: {Testing.Thirteen()}");

            // Run any code that other files need for setup
            InitWriting();

            WriteLine("Do you want to enable GUI mode? (y/n)");
            guiMode = ReadLine().ToLower() == "y";
            WriteLine("[tip]Use !help to view a list of commands");
            if (guiMode)
            {
                new NetProcessingUI().Start(false);
                Thread.Sleep(1000); // Size throws an error if the game begins too soon after Net.Processing begins
            }
            else
            {
                // CLI mode
                Fullscreen();
            }

            // Set name for current game
            startTime = DateTime.UtcNow;
            currentGameName = new SafeFilePath(Convert.ToString(startTime).Replace("/", " ").Replace(":", "_"));
            Directory.CreateDirectory($"../../../Save Files/{currentGameName}");
            string info = $"v{INFO_VER}\ncreatedDate,modifiedDate\n{DateTimeToTimestamp(startTime)},{DateTimeToTimestamp(startTime)}";
            File.WriteAllText($"../../../Save Files/{currentGameName}/info.csv", info);

            // Load players
            for (int i = 1; i <= PLAYER_COUNT; i++)
            {
                string? attemptedName = "";
                while (attemptedName is null || attemptedName.Length != 1 || Char.IsWhiteSpace(attemptedName[0]))
                {
                    Write($"[{colourNames[i - 1]}]Player {i}[prev], choose your single-char name: ");
                    attemptedName = ReadLine();
                }
                players[i - 1] = new Player(attemptedName[0], i - 1);
            }
            if (!guiMode)
            {
                WriteBoard(players, locations);
            }

            // Initialise grapher
            grapher = new Grapher();

            // Main loop
            gameRunning = true;
            turnCount = 0;
            int turnsSinceActivity = 0;
            while (gameRunning)
            {
                // Round
                for (int i = 0; i < players.Length; i++)
                {
                    Player player = players[i];
                    grapher.LogMoney(player, turnCount, player.GetMoney());
                    if (player.GetBankruptStatus() == BankruptcyStatus.RecentlyBankrupt) // Change it since they bankrupted last turn
                    {
                        player.SetBankruptStatus(BankruptcyStatus.NonrecentlyBankrupt);
                    }
                    else if (player.GetSkip() == true)
                    {
                        WriteLine($"[{colourNames[i]}]{player.GetName()}[white]'s turn was skipped!");
                        player.SetSkip(false);
                    }
                    else if (player.GetDebtWarning() == true && player.GetBankruptStatus() == BankruptcyStatus.Warned) // Eliminate if bankrupt
                    {
                        player.SetBankruptStatus(BankruptcyStatus.RecentlyBankrupt);
                        WriteLine($"[{colourNames[player.GetId()]}]{player.GetName()}[white] is bankrupt ({FormatBalance(player.GetMoney())}) and, therefore, eliminated!");
                        turnsSinceActivity = 0;
                        foreach (Tile tile in locations)
                        {
                            if (tile is Animal animal && animal.GetOwner() == player)
                            {
                                animal.ClearOwner();
                            }
                        }
                    }
                    else
                    {
                        player.SetBankruptStatus(BankruptcyStatus.Normal); // They aren't in danger of bankruptcy
                        // Turn
                        WriteLine($"[{colourNames[i]}]{player.GetName()}[white]'s turn");

                        if (player.IsAI())
                        {
                            turnsSinceActivity = 0;
                            WriteLine("Press enter to roll");
                            ReadLine();
                        }
                        player.Roll();
                        if (animations)
                        {
                            Thread.Sleep(700);
                        }

                        if (!guiMode)
                        {
                            WriteBoard(players, locations);
                        }
                        if (player.GetDebtWarning() == true)
                        {
                            WriteLine($"You are currently {FormatBalance(-player.GetMoney(), true)} in debt! If you're still in debt by the start of your next turn, you're out\n[tip]Your opponents may be willing to buy your animals. If you come to an agreement, use !trade to transfer ownership");
                            player.SetBankruptStatus(BankruptcyStatus.Warned);
                        }

                        locations[player.GetPos()].Land(ref player);
                    }
                    players[i] = player; // Update the stored player
                }
                // Check there are still remaining players
                int remainingCount = 0;
                foreach (Player player in players)
                {
                    if (player.GetBankruptStatus() == BankruptcyStatus.Normal || player.GetBankruptStatus() == BankruptcyStatus.Warned)
                    {
                        remainingCount += 1;
                    }
                }
                if (remainingCount <= 1)
                {
                    gameRunning = false;
                }
                // Safety check: a game with only AI players can theoretically go on forever without letting the user input; this gives the user the chance to run commands
                if (turnsSinceActivity >= 50)
                {
                    WriteLine("It's been 50 turns since a human took a turn or anyone got eliminated. Do you want to run any commands?");
                    ReadLine();
                    turnsSinceActivity = 0;
                }
                turnCount++;
                turnsSinceActivity++;
            }
            Player? winner = null;
            foreach (Player player in players)
            {
                if (player.GetBankruptStatus() == BankruptcyStatus.Normal || player.GetBankruptStatus() == BankruptcyStatus.Warned)
                {
                    winner = player; // There can only be one player left in
                    break;
                }
            }
            if (winner is null) // Multiple players bankrupted on the last turn -- the winner is whomever is least bankrupt
            {
                Player[] recentlyBankrupted = (from player in players where player.GetBankruptStatus() == BankruptcyStatus.RecentlyBankrupt select player).ToArray();
                int[] recentlyBankruptedMoneys = (from player in players where player.GetBankruptStatus() == BankruptcyStatus.RecentlyBankrupt select player.GetMoney()).ToArray();
                winner = recentlyBankrupted[Array.IndexOf(recentlyBankruptedMoneys, recentlyBankruptedMoneys.Max())];
            }
            WriteLine($"[{colourNames[winner.GetId()]}]Player {winner.GetName()}[white] wins with {FormatBalance(winner.GetMoney())}!");
            grapher.GenerateGraph($"../../../Save files/{currentGameName}/Money graph.png");

            // Let the user run commands if they want, e.g. custom-res graph
            WriteLine("You may now close the terminal window."); // Net.Processing automatically closes when the terminal is closed
            while (true)
            {
                ReadLine();
            }
        }
    }
}
