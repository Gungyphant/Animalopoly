// V2 additions:
//  Named animals

// V3 additions:
//  Coloured names
//  Coloured tiles based on owner

// V4 additions:
//  Made all references to a player (in main loop) use coloured names
//  Added a pause after rolling, after getting card, and after passing/landing on start
//  Show new balance after landing on an owned animal
//  Fixed a bug where players with 0 money would be silently eliminated

// V5 additions:
//  Bankrupted players now have their animals returned to the bank
//  Wrote a new function to allow easier printing of coloured text


// Potential features & known bugs:
//  Setting font size
//  More cards
//  Eliminate players a full loop after they first go negative, otherwise p1 has an advantage


using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using static Animalopoly.Program;

namespace Animalopoly
{
    class Program
    {
        static Dictionary<string, ConsoleColor> knownColours = new Dictionary<string, ConsoleColor>()
        {
            { "blue", ConsoleColor.Blue },
            { "red", ConsoleColor.Red },
            { "green", ConsoleColor.Green },
            { "yellow", ConsoleColor.Yellow },
            { "white", ConsoleColor.White },
            { "null", ConsoleColor.White },
        };
        //const string ANSI_RESET = 
        static void Write(string text)
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
                    if (knownColours.ContainsKey(newColourName))
                    {
                        colour = knownColours[newColourName];
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
        static void WriteLine(string text)
        {
            Write(text);
            Console.WriteLine();
        }
        static void Roll(ref Player player)
        {
            Random rnd = new Random();
            int die1 = rnd.Next(1, 7);
            int die2 = rnd.Next(1, 7);
            for (int _ = 0; _ < 10; _++) // Show the dice 'rolling'
            {
                Console.Write($"{new String('\b', 5)}{rnd.Next(1, 7)} + {rnd.Next(1, 7)}");
                Thread.Sleep(50);
            }
            WriteLine($"{new String('\b', 5)}{die1} + {die2} = {die1 + die2}");
            if (die1 == die2)
            {
                getRandomCard(cards).award(ref player);
                Thread.Sleep(100);
            }
            player.move(die1 + die2);
            //return die1 + die2;
        }
        public class Player
        {
            private char name;
            private int id;
            private int money;
            private bool bankruptWarning;
            private int cellId;
            public bool skipTurn;

            public Player(char name, int id)
            {
                this.name = name;
                this.id = id;
                this.money = 3750;
                this.bankruptWarning = false;
                this.cellId = 0;
                this.skipTurn = false;
            }
            public char getName()
            {
                return name;
            }
            public int getId()
            {
                return id;
            }
            public int getMoney()
            {
                return money;
            }
            public void changeMoney(int change)
            {
                money += change;
                if (money < 0 && !bankruptWarning)
                {
                    WriteLine($"[{colourNames[this.id]}]{this.name}[white] is in danger of bankruptcy...");
                    bankruptWarning = true;
                }
                else if (money > 0 && bankruptWarning)
                {
                    WriteLine($"[{colourNames[this.id]}]{this.name}[white] is no longer in danger of bankruptcy");
                    bankruptWarning = false;
                }
            }
            public int getPos()
            {
                return cellId;
            }
            public void move(int cells)
            {
                this.cellId += cells;
                if (cellId >= 26)
                {
                    if (cellId == 26)
                    {
                        WriteLine($"[{colourNames[this.id]}]{this.name}[white] landed on Start and got £1000");
                        this.changeMoney(1000);
                    }
                    else
                    {
                        WriteLine($"[{colourNames[this.id]}]{this.name}[white] passed Start and got £500");
                        this.changeMoney(500);
                    }
                }
                this.cellId %= 26;
            }
        }
        public class Card
        {
            public int reward;
            public string name;
            public string details;
            public Card(int reward, string name, string details)
            {
                this.reward = reward;
                this.name = name;
                this.details = details;
            }
            public void award(ref Player player)
            {
                Console.WriteLine(name);
                WriteLine(details);
                player.changeMoney(this.reward);
            }
        }
        public static (int, string, string)[] cards = new (int, string, string)[3] { // (reward, name, details)
                (100, "Business is booming!", "The extra customers meant you made an extra £100 in profit"),
                (-200, "Food prices are up!", "You had to spend an extra £200 to feed your animals"),
                (-1000, "Sued!", "Someone got hurt trying to see your animals, and they sued you for £1000!"),
            };
        static Card getRandomCard((int, string, string)[] cards)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, cards.Length);
            (int, string, string) data = cards[index];
            return new Card(data.Item1, data.Item2, data.Item3);
        }
        public class Animal
        {
            protected string name;
            protected int level;
            protected int[] stopCosts;
            protected int buyCost;
            protected Player? owner;
            public Animal(string name, int[] stopCosts, int buyCost)
            {
                this.name = name;
                this.level = 0;
                this.stopCosts = stopCosts;
                this.buyCost = buyCost;
                this.owner = default(Player);
            }
            public string GetName()
            {
                return name;
            }
            public int GetStopCost()
            {
                return stopCosts[level];
            }
            public int GetBuyCost()
            {
                return buyCost;
            }
            public Player? GetOwner()
            {
                return this.owner;
            }
            public void SetOwner(ref Player owner)
            {
                this.owner = owner;
            }
            public void ClearOwner() // This function is necessary as SetOwner(null) is invalid, probably because it doesn't want a reference to null and it's ref Player?
            {
                this.owner = default(Player);
            }
            public string GetCard()
            {
                string levelString = $"Lvl {level + 1}";

                int nameSpaceCount = (16 - this.name.Length) / 2;
                int levelSpaceCount = (16 - levelString.Length) / 2;

                string card = "";
                card += $"┌────────────────────┐\n";
                card += $"│ ┌────────────────┐ │\n";
                card += $"│ │{new string(' ', nameSpaceCount)}{Underline(name)}{new string(' ', (int)Math.Floor((16 - this.name.Length) / 2.0 + 0.5))}│ │\n";
                card += $"│ │{new string(' ', levelSpaceCount)}{levelString}{new string(' ', Convert.ToInt32((16 - levelString.Length)/2.0 + 0.5))}│ │\n";
                card += $"│ └────────────────┘ │\n";
                card += $"│ ┌────────────────┐ │\n";
                card += $"│ │   {Underline("Stop Costs")}   │ │\n";
                for (int i = 0; i < this.stopCosts.Length; i++)
                {
                    card += $"│ │     {i + 1}: £{this.stopCosts[i]}{new string(' ', 7 - (Convert.ToString(this.stopCosts[i]).Length))}│ │\n";
                }
                card += $"│ └────────────────┘ │\n";
                card += $"│ ┌────────────────┐ │\n";
                card += $"│ │  Cost: £{this.buyCost}{new string(' ', 7 - Convert.ToString(this.buyCost).Length)}│ │\n";
                card += $"│ └────────────────┘ │\n";
                card += $"│ ┌────────────────┐ │\n";
                if (this.owner != null)
                {
                    card += $"│ │ Owner: [{colourNames[this.owner.getId()]}]{this.owner.getName()}[white]{new string(' ', 8 - Convert.ToString(this.owner.getName()).Length)}│ │\n";
                }
                else
                {
                    card += $"│ │ Owner: None{new string(' ', 8 - "None".Length)}│ │\n";
                }
                card += $"│ └────────────────┘ │\n";
                card += $"└────────────────────┘";

                return card;
            }
            public void Land(ref Player player)
            {
                if (this.name == "Start")
                {
                    Thread.Sleep(100);
                }
                else if (this.name == "Miss a turn")
                {
                    WriteLine("Miss a turn!");
                }
                else
                {
                    WriteLine(this.GetCard());
                    if (this.owner == null)
                    {
                        WriteLine($"Nobody owns this animal. Do you want to buy it for £{this.buyCost}? (you have £{player.getMoney()}) (y/n)");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            player.changeMoney(-1 * this.buyCost);
                            this.owner = player;
                            this.level++;
                        }
                    }
                    else if (this.owner.getId() == player.getId())
                    {
                        WriteLine($"You own this animal. Do you want to upgrade it for £{this.buyCost}? (you have £{player.getMoney()}) (y/n)");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            player.changeMoney(-1 * this.buyCost);
                            this.level++;
                        }
                    }
                    else
                    {
                        WriteLine($"[{colourNames[this.owner.getId()]}]{this.owner.getName()}[white] owns this animal. You have to pay them a fee of £{this.stopCosts[this.level]} (you now have £{player.getMoney() - this.stopCosts[this.level]})");
                        player.changeMoney(-1 * this.buyCost); // TODO: this doesn't edit the actual player's money
                        this.owner.changeMoney(this.buyCost);
                    }
                }
            }
            //public void Upgrade()
            //{
            //    this.level++;
            //}
        }
        //public class Tile
        //{
        //    private int location;
        //    private string name;
        //    private Animal animal;
        //}

        static string Underline(string s)
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll")]
            static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32.dll")]
            static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

            var handle = GetStdHandle(-11);
            uint mode;
            GetConsoleMode(handle, out mode);
            mode |= 4;
            SetConsoleMode(handle, mode);
            return $"\x1B[4m{s}\x1B[24m";
        }

        static void WriteColour(char c, ConsoleColor color) // Should only be used in Write() and Writeline()
        {
            WriteColour(Convert.ToString(c), color);
        }
        static void WriteColour(string s, ConsoleColor color) // Should only be used in Write() and Writeline()
        {
            Console.ForegroundColor = color;
            Console.Write(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static string GetAnimalText(Animal animal)
        {
            if (animal.GetOwner() != null)
            {
                return $"[{colourNames[animal.GetOwner().getId()]}]{animal.GetName()}[white]";
            }
            else
            {
                return animal.GetName();
            }
        }

        const int TILEWIDTH = 12;
        const int TILEHEIGHT = 5;
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
        static void WriteBoard(Player[] players, Animal[] animals)
        {
            string boardString = "";
            boardString += ("┌");
            for (int i = 0; i < 8; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┬");
            }
            boardString += "\b┐" + "\n";

            // First tile-row
            // First row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players[0].getPos() == i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].getPos() == i)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            // Top half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2)/2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Name row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[i].GetName().Length)/2.0))));
                boardString += GetAnimalText(animals[i]);
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[i].GetName().Length)/2.0))));
            }
            boardString += "│" + "\n";
            // Bottom half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players.Length > 2 && players[2].getPos() == i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players.Length > 3 && players[3].getPos() == i)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            boardString += ("├");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┼");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┴");
            }
            boardString += ("\b┼");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┤");

            for (int row = 0; row < 5; row++)
            {
                boardString += "\n";
                // Top player row
                // Left
                boardString += ("│");
                if (players[0].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += ("│");
                // Middle
                boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                // Right
                boardString += ("│");
                if (players[0].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players[1].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += "│" + "\n";
                // Middle lines
                // Top half
                for (int line = 0; line < Math.Floor((double)(TILEHEIGHT - 2) / 2); line++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += ("│");
                    boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += "│" + "\n";
                }
                // Name line
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[25 - row].GetName().Length) / 2.0))));
                boardString += GetAnimalText(animals[25 - row]);
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[25 - row].GetName().Length) / 2.0))));
                boardString += ("│");

                boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));

                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[8 + row].GetName().Length) / 2.0))));
                boardString += GetAnimalText(animals[8 + row]);
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[8 + row].GetName().Length) / 2.0))));
                boardString += "│" + "\n";
                // Bottom half
                for (int line = 0; line < Math.Floor((double)(TILEHEIGHT - 2) / 2); line++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += ("│");
                    boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                    boardString += "│" + "\n";
                }
                // Bottom row
                // Left
                boardString += ("│");
                if (players.Length > 2 && players[2].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players.Length > 3 && players[3].getPos() == 25 - row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";

                }
                else
                {
                    boardString += (" ");
                }
                boardString += ("│");
                // Middle
                boardString += (new string(' ', 6 * (TILEWIDTH + 1) - 1));
                // Right
                boardString += ("│");
                if (players.Length > 2 && players[2].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players.Length > 3 && players[3].getPos() == 8 + row)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += "│" + "\n";

                boardString += ("├");
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┤");
                for (int i = 0; i < 6; i++)
                {
                    boardString += (new string(' ', TILEWIDTH + 1));
                }
                boardString += ("\b├");
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┤");
            }
            // Bottom row
            boardString += (new string('\b', 110));
            boardString += ("├");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┼");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┬");
            }
            boardString += ("\b┼");
            boardString += (new string('─', TILEWIDTH));
            boardString += "┤" + "\n";

            // Final row
            // First line
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players.Length > 0 && players[0].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[0]}]{players[0].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players.Length > 1 && players[1].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[1]}]{players[1].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            // Top half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Name row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                boardString += (new string(' ', (int)Math.Floor((float)((TILEWIDTH - animals[20 - i].GetName().Length) / 2.0))));
                boardString += GetAnimalText(animals[20 - i]);
                boardString += (new string(' ', (int)Math.Ceiling((float)((TILEWIDTH - animals[20 - i].GetName().Length) / 2.0))));
            }
            boardString += "│" + "\n";
            // Bottom half
            for (int row = 0; row < Math.Floor((double)(TILEHEIGHT - 2) / 2); row++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardString += ("│");
                    boardString += (new string(' ', TILEWIDTH));
                }
                boardString += "│" + "\n";
            }
            // Bottom row
            for (int i = 0; i < 8; i++)
            {
                boardString += ("│");
                if (players.Length > 2 && players[2].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[2]}]{players[2].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
                boardString += (new string(' ', TILEWIDTH - 2));
                if (players.Length > 3 && players[3].getPos() == 20 - i)
                {
                    boardString += $"[{colourNames[3]}]{players[3].getName()}[white]";
                }
                else
                {
                    boardString += (" ");
                }
            }
            boardString += "│" + "\n";

            boardString += ("└");
            boardString += (new string('─', TILEWIDTH));
            boardString += ("┴");
            for (int i = 0; i < 6; i++)
            {
                boardString += (new string('─', TILEWIDTH));
                boardString += ("┴");
            }
            boardString += ("\b┴");
            boardString += (new string('─', TILEWIDTH));
            boardString += "┘" + "\n";

            Write(boardString);
        }
        static void Main()
        {
            // Testing
            //WriteLine("This should be normal. [blue]This should be blue. [blue]a[red]b[green]c[white][yellow]d [white]and this should be normal again");


            Console.SetWindowSize(106, Console.LargestWindowHeight);
            Console.SetWindowPosition(0, 0);
            Console.OutputEncoding = Encoding.UTF8;

            // Main game
            const int PLAYERCOUNT = 4;
            Player[] players = new Player[PLAYERCOUNT];
            Animal[] locations = new Animal[26] // Prices copied from regular Monopoly, but scaled by the fact that you get £500 instead of £200
            {
                new Animal("Start", [], 0),
                // Common in UK
                new Animal("Squirrel", [25, 75, 225, 400], 125),
                new Animal("Sparrow", [50, 150, 450, 800], 125),
                // Rarer in UK
                new Animal("Hedgehog", [75, 225, 675, 1200], 125),
                new Animal("Fox", [75, 225, 675, 1200], 125),
                new Animal("Badger", [100, 250, 750, (int)(450 * 2.5)], 125),
                // UK wild animals
                new Animal("Deer", [125, 375, (int)(450 * 2.5), (int)(625 * 2.5)], 250),
                new Animal("Bat", [125, 375, (int)(450 * 2.5), (int)(625 * 2.5)], 250),
                new Animal("Wildcat", [150, 450, 1250, 1750], 250),
                // Least concern
                new Animal("Arctic fox", [175, 500, (int)(550 * 2.5), 1875], 250),
                new Animal("Brown bear", [175, 500, (int)(550 * 2.5), 1875], 250),
                new Animal("Kangaroo", [200, 550, 1500, 2000], 250),
                // Near-threatened
                new Animal("Jaguar", [225, 625, 1750, (int)(875 * 2.5)], 375),
                new Animal("Miss a turn", [], 0),
                new Animal("White rhino", [225, 625, 1750, (int)(875 * 2.5)], 375),
                new Animal("Bison", [250, 750, 1875, (int)(925 * 2.5)], 375),
                // Vulnerable
                new Animal("Cheetah", [275, 825, 2000, (int)(975 * 2.5)], 375),
                new Animal("Lion", [275, 825, 2000, (int)(975 * 2.5)], 375),
                new Animal("Polar Bear", [300, 900, (int)(850 * 2.5), (int)(1025 * 2.5)], 375),
                // Endangered
                new Animal("Elephant", [325, (int)(390 * 2.5), 2250, (int)(1100 * 2.5)], 500),
                new Animal("Tiger", [325, (int)(390 * 2.5), 2250, (int)(1100 * 2.5)], 500),
                new Animal("Chimpanzee", [375, 1125, 2500, 3000], 500),
                // Critically endangered
                new Animal("Black Rhino", [(int)(175 * 2.5), 1250, 2750, 3250], 500),
                new Animal("Orangutan", [500, 1500, 3500, 4250], 500),
                // Fictional (not in base Monopoly)
                new Animal("Dragon", [(int)(225 * 2.5), 1750, 4250, 5250], 1000),
                new Animal("Unicorn", [(int)(250 * 2.5), 2000, 5000, 6250], 1000),
            };
            for (int i = 1; i <= PLAYERCOUNT; i++) // Get player names
            {
                string attemptedName = "";
                while (attemptedName.Length != 1)
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
                    if (player.skipTurn)
                    {
                        WriteLine($"[{colourNames[i]}]{player.getName()}[white]'s turn was skipped!");
                        player.skipTurn = false;
                        players[i] = player;
                        continue;
                    }
                    // Turn
                    WriteLine($"[{colourNames[i]}]{player.getName()}[white]'s turn\nPress enter to roll");
                    Console.ReadLine();
                    Roll(ref player);
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
