using static Animalopoly.Writing;
using static Animalopoly.PlayerClass;
using static Animalopoly.CardClass;
using static Animalopoly.Program;

namespace Animalopoly
{
    class TileClasses
    {
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
                card += $"│ │{new string(' ', levelSpaceCount)}{levelString}{new string(' ', Convert.ToInt32((16 - levelString.Length) / 2.0 + 0.5))}│ │\n";
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
                    else if (this.owner.getId() == player.getId() && (this.level + 1) < this.stopCosts.Length)
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
                        player.changeMoney(-1 * this.stopCosts[this.level]);
                        this.owner.changeMoney(this.stopCosts[this.level]);
                    }
                }
            }
            //public void Upgrade()
            //{
            //    this.level++;
            //}
            public string GetAnimalText()
            {
                if (this.owner != null)
                {
                    return $"[{colourNames[this.owner.getId()]}]{this.name}[white]";
                }
                else
                {
                    return this.name;
                }
            }
        }
        //public class Tile
        //{
        //    private int location;
        //    private string name;
        //    private Animal animal;
        //}
        public static Animal[] locations = new Animal[26] // Prices copied from regular Monopoly, but scaled by the fact that you get £500 instead of £200
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
    }
}