using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Commands;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.NetProcessingUI;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;


namespace Animalopoly.Code
{
    class Program
    {
        const int PLAYER_COUNT = 4;
        const int INFO_VER = 2; // This needs to be incremented whenever the format of info.csv is updated
        // The following variables are public so that they can be accessed by commands
        public static Player[] players = new Player[PLAYER_COUNT];
        public static bool gameRunning;
        public static Grapher grapher;
        public static string currentGameName;
        public static int turnCount;
        public static DateTime startTime;
        public static bool animations = true;
        public static bool guiMode;
        static long GetTimestamp(DateTime time) // From: https://aske.wachs.dk/06/07/2021/c-conversion-between-unix-timestamps-and-datetime/
        {
            return ((DateTimeOffset)time).ToUnixTimeMilliseconds();
        }
        static void Main()
        {
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
                // Main game
                Fullscreen();
            }

            // Set name for current game
            startTime = DateTime.UtcNow;
            currentGameName = Convert.ToString(startTime).Replace("/", " ").Replace(":", "_");
            Directory.CreateDirectory($"../../../Save Files/{currentGameName}");
            string info = $"v{INFO_VER}\ncreatedDate,modifiedDate\n{GetTimestamp(startTime)},{GetTimestamp(startTime)}";
            File.WriteAllText($"../../../Save Files/{currentGameName}/info.csv", info);

            // Load players
            for (int i = 1; i <= PLAYER_COUNT; i++)
            {
                string? attemptedName = "";
                while (attemptedName == null || attemptedName.Length != 1 || Char.IsWhiteSpace(attemptedName[0]))
                {
                    Write($"[{colourNames[i - 1]}]Player {i}[white], choose your single-char name: ");
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
            while (gameRunning)
            {
                // Round
                for (int i = 0; i < players.Length; i++)
                {
                    Player player = players[i];
                    grapher.LogMoney(player, turnCount, player.GetMoney());
                    if (player.GetBankruptStatus() >= 2) // Change it since they didn't bankrupt this turn
                    {
                        player.SetBankruptStatus(3);
                    }
                    else if (player.GetSkip() == true)
                    {
                        WriteLine($"[{colourNames[i]}]{player.GetName()}[white]'s turn was skipped!");
                        player.SetSkip(false);
                    }
                    else if (player.GetBankruptWarning() == true && player.GetBankruptStatus() == 1) // Eliminate if bankrupt
                    {
                        player.SetBankruptStatus(2);
                        WriteLine($"[{colourNames[player.GetId()]}]{player.GetName()}[white] is bankrupt (-£{-player.GetMoney()}) and, therefore, eliminated!");
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
                        // Turn
                        WriteLine($"[{colourNames[i]}]{player.GetName()}[white]'s turn");

                        if (player.GetAILevel() == 0)
                        {
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
                        if (player.GetBankruptWarning() == true)
                        {
                            WriteLine($"You are currently £{-player.GetMoney()} in debt! If you're still in debt by the start of your next turn, you're out\n[tip]Your opponents may be willing to buy your animals. If you come to an agreement, use !trade to transfer ownership");
                            player.SetBankruptStatus(1); // Turn started since warning
                        }

                        locations[player.GetPos()].Land(ref player);
                    }
                    players[i] = player; // Update the stored player
                }
                // Check there are still remaining players
                int remainingCount = 0;
                foreach (Player player in players)
                {
                    if (player.GetBankruptStatus() < 2)
                    {
                        remainingCount += 1;
                    }
                }
                if (remainingCount <= 1)
                {
                    gameRunning = false;
                }
                turnCount++;
            }
            Player? winner = null;
            foreach (Player player in players)
            {
                if (player.GetBankruptStatus() < 2)
                {
                    winner = player; // There can only be one player left in
                }
            }
            if (winner == null) // Multiple players bankrupted on the last turn -- the winner is whomever is least bankrupt
            {
                Player[] recentlyBankrupted = (from player in players where player.GetBankruptStatus() == 2 select player).ToArray();
                int[] recentlyBankruptedMoneys = (from player in players where player.GetBankruptStatus() == 2 select player.GetMoney()).ToArray();
                winner = recentlyBankrupted[Array.IndexOf(recentlyBankruptedMoneys, recentlyBankruptedMoneys.Max())];
            }
            WriteLine($"[{colourNames[winner.GetId()]}]Player {winner.GetName()}[white] wins with £{winner.GetMoney()}!");
            grapher.GenerateGraph($"../../../Save files/{currentGameName}/Money graph.png");

            if (guiMode) // Net.Processing prevents standard exit message from appearing
            {
                WriteLine("You may now close the terminal window."); // Net.Processing automatically closes when the terminal is closed
            }
        }
    }
}
