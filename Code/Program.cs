using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using static Animalopoly.Writing;
using static Animalopoly.PlayerClass;
using static Animalopoly.CardClass;
using static Animalopoly.TileClasses;
using static Animalopoly.CommandLineInterface;

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
            const int PLAYERCOUNT = 4;
            Player[] players = new Player[PLAYERCOUNT];
            for (int i = 1; i <= PLAYERCOUNT; i++) // Get player names
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
            while (players.Count() > 1) // The game ends when there's only one player left
            {
                // Round
                for (int i = 0; i < players.Length; i++)
                {
                    Player player = players[i];
                    if (player.getSkip() == true)
                    {
                        WriteLine($"[{colourNames[i]}]{player.getName()}[white]'s turn was skipped!");
                        player.setSkip(false);
                        players[i] = player;
                        continue;
                    }
                    // Turn
                    WriteLine($"[{colourNames[i]}]{player.getName()}[white]'s turn\nPress enter to roll");
                    Console.ReadLine();
                    player.Roll();
                    Thread.Sleep(700);
                    WriteBoard(players, locations);
                    locations[player.getPos()].Land(ref player);

                    players[i] = player;
                }

                // Eliminate bankrupt players
                foreach (Player eliminatedPlayer in (from player in players where player.getMoney() < 0 select player).ToArray())
                {
                    WriteLine($"[{colourNames[eliminatedPlayer.getId()]}]Player {eliminatedPlayer.getName()}[white] is bankrupt and, therefore, eliminated!");
                    foreach (Animal animal in locations)
                    {
                        if (animal.GetOwner() == eliminatedPlayer)
                        {
                            animal.ClearOwner();
                        }
                    }
                }
                players = (from player in players where player.getMoney() >= 0 select player).ToArray();
            }
            WriteLine($"[{colourNames[players[0].getId()]}]Player {players[0].getName()}[white] wins!");
        }
    }
}
