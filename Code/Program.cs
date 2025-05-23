using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using static Animalopoly.Writing;
using static Animalopoly.PlayerClass;
using static Animalopoly.CardClass;
using static Animalopoly.TileClasses;
using static Animalopoly.CommandLineInterface;
using static Animalopoly.Graphing;

namespace Animalopoly
{
    class Program
    {
        static void Main()
        {
            // Testing
            //WriteLine("This should be normal. [blue]This should be blue. [blue]a[red]b[green]c[white][yellow]d [white]and this should be normal again");


            Console.SetWindowSize(106, Console.LargestWindowHeight);
            #pragma warning disable CA1416 // Validate platform compatibility
            Console.SetWindowPosition(0, 0);
            #pragma warning restore CA1416 // Validate platform compatibility
            Console.OutputEncoding = Encoding.UTF8;

            // Main game

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
                    grapher.LogMoney(player, turnCount, player.getMoney());
                    if (player.getBankruptStatus() >= 2) // Change it since they didn't bankrupt this turn
                    {
                        player.setBankruptStatus(3);
                    }
                    else if (player.getSkip() == true)
                    {
                        WriteLine($"[{colourNames[i]}]{player.getName()}[white]'s turn was skipped!");
                        player.setSkip(false);
                    }
                    else if (player.getBankruptWarning() == true && player.getBankruptStatus() == 1) // Eliminate if bankrupt
                    {
                        player.setBankruptStatus(2);
                        WriteLine($"[{colourNames[player.getId()]}]{player.getName()}[white] is bankrupt (-£{-player.getMoney()}) and, therefore, eliminated!");
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
                        WriteLine($"[{colourNames[i]}]{player.getName()}[white]'s turn");

                        WriteLine("Press enter to roll");
                        Console.ReadLine();
                        player.Roll();
                        Thread.Sleep(700);

                        WriteBoard(players, locations);
                        if (player.getBankruptWarning() == true)
                        {
                            WriteLine($"You are currently £{-player.getMoney()} in debt! If you're still in debt by the start of your next turn, you're out");
                            player.setBankruptStatus(1); // Turn started since warning
                        }

                        locations[player.getPos()].Land(ref player);
                    }
                    players[i] = player; // Update the stored player
                }
                // Check there are still remaining players
                int remainingCount = 0;
                foreach (Player player in players)
                {
                    if (player.getBankruptStatus() < 2)
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
                if (player.getBankruptStatus() < 2)
                {
                    winner = player; // There can only be one player left in
                }
            }
            if (winner == null) // Multiple players bankrupted on the last turn -- the winner is whomever is least bankrupt
            {
                Player[] recentlyBankrupted = (from player in players where player.getBankruptStatus() == 2 select player).ToArray();
                int[] recentlyBankruptedMoneys = (from player in players where player.getBankruptStatus() == 2 select player.getMoney()).ToArray();
                winner = recentlyBankrupted[Array.IndexOf(recentlyBankruptedMoneys, recentlyBankruptedMoneys.Max())];
            }
            WriteLine($"[{colourNames[winner.getId()]}]Player {winner.getName()}[white] wins with £{winner.getMoney()}!");
            grapher.GenerateGraph($"../../../Save files/{currentGameName}/Money graph.png");
        }
    }
}
