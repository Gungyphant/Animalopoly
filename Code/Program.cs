using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;


namespace Animalopoly.Code
{
    class Program
    {
        static void Main()
        {
            // Run any code that other files need for setup
            InitWriting();

            // Main game
            Fullscreen();

            // Set name for current game
            string currentGameName = Convert.ToString(DateTime.Now).Replace("/", " ").Replace(":", "_");

            // Load players
            const int PLAYERCOUNT = 4;
            Player[] players = new Player[PLAYERCOUNT];
            for (int i = 1; i <= PLAYERCOUNT; i++)
            {
                string? attemptedName = "";
                while (attemptedName == null || attemptedName.Length != 1)
                {
                    Write($"[{colourNames[i - 1]}]Player {i}[white], choose your single-char name: ");
                    attemptedName = Console.ReadLine();
                }
                players[i - 1] = new Player(attemptedName[0], i - 1);
            }
            WriteBoard(players, locations);

            // Initialise grapher
            Grapher grapher = new Grapher();

            // Main loop
            bool gameRunning = true;
            int turnCount = 0;
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
                        foreach (Animal animal in locations)
                        {
                            if (animal.GetOwner() == player)
                            {
                                animal.ClearOwner();
                            }
                        }
                    }
                    else
                    {
                        // Turn
                        WriteLine($"[{colourNames[i]}]{player.GetName()}[white]'s turn");

                        WriteLine("Press enter to roll");
                        Console.ReadLine();
                        player.Roll();
                        Thread.Sleep(700);

                        WriteBoard(players, locations);
                        if (player.GetBankruptWarning() == true)
                        {
                            WriteLine($"You are currently £{-player.GetMoney()} in debt! If you're still in debt by the start of your next turn, you're out");
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
        }
    }
}
