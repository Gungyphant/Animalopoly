using static Animalopoly.Code.Writing;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.Program;

namespace Animalopoly.Code
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
            protected string set; // Shown on card
            protected string smallSet; // Shown on board
            public Animal(string name, int[] stopCosts, int buyCost, string set, string smallSet)
            {
                this.name = name;
                this.level = 0;
                this.stopCosts = stopCosts;
                this.buyCost = buyCost;
                this.owner = default(Player);
                this.set = set;
                this.smallSet = smallSet;
            }
            public string GetName()
            {
                return name;
            }
            public string GetSet()
            {
                return set;
            }
            public string GetSmallSet()
            {
                return smallSet;
            }
            public int GetAnimalsInSet()
            {
                int animalsInSet = 0;
                foreach (Animal animal in locations)
                {
                    if (animal.GetSet() == this.set && animal.GetOwner() == this.owner)
                    {
                        animalsInSet++;
                    }
                }
                return animalsInSet;
            }
            public int GetStopCost()
            {
                int result = stopCosts[level];
                int animalsInSet = GetAnimalsInSet();
                result *= animalsInSet switch
                {
                    1 => 1,
                    2 => 2,
                    3 => 4,
                    _ => throw new Exception($"{animalsInSet} animals in one set"),
                };
                return result;
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
                //card += $"│ ┌────────────────┐ │\n";
                //card += $"│ │   Set: {this.set}{new string(' ', 7 - Convert.ToString(this.set).Length)}│ │\n";
                //card += $"│ └────────────────┘ │\n";
                card += $"│ ┌────────────────┐ │\n";
                if (this.owner != null)
                {
                    card += $"│ │ Owner: [{colourNames[this.owner.GetId()]}]{this.owner.GetName()}[white]{new string(' ', 8 - Convert.ToString(this.owner.GetName()).Length)}│ │\n";
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
                    player.SetSkip(true);
                    WriteLine("Miss a turn!");
                }
                else // Animal
                {
                    WriteLine(this.GetCard());
                    if (this.owner == null)
                    {
                        WriteLine($"Nobody owns this animal. It's in the set {this.set}. Do you want to buy it for £{this.buyCost}? (you have £{player.GetMoney()}) (y/n)");
                        string? response = Console.ReadLine();
                        if (response != null && response.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                        {
                            player.ChangeMoney(-1 * this.buyCost);
                            this.owner = player;
                            if (this.level == 0)  // Animals of bankrupted players should stay at their current levels
                            {
                                this.level = 1;
                            }
                        }
                    }
                    else if (this.owner.GetId() == player.GetId())
                    {
                        if ((this.level + 1) < this.stopCosts.Length)
                        {
                            WriteLine($"You own this animal. Do you want to upgrade it for £{this.buyCost}? (you have £{player.GetMoney()}) (y/n)");
                            string? response = Console.ReadLine();
                            if (response != null && response.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                            {
                                player.ChangeMoney(-1 * this.buyCost);
                                this.level++;
                            }
                        }
                        else
                        {
                            WriteLine($"You own this animal. You can't upgrade it any more");
                        }
                    }
                    else
                    {
                        int animalsInSet = GetAnimalsInSet();
                        Write($"[{colourNames[this.owner.GetId()]}]{this.owner.GetName()}[white] owns this animal. ");
                        if (animalsInSet <= 1)
                        {
                            WriteLine($"You have to pay them a fee of £{this.GetStopCost()} (you now have £{player.GetMoney() - this.GetStopCost()})");
                        }
                        else
                        {
                            WriteLine($"They have {animalsInSet} animals from that set, so you have to pay them a fee of £{this.GetStopCost()} (you now have £{player.GetMoney() - this.GetStopCost()})");
                        }
                        player.ChangeMoney(-1 * this.GetStopCost());
                        this.owner.ChangeMoney(this.GetStopCost());
                    }
                }
            }
            //public void Upgrade()
            //{
            //    this.level++;
            //}
            public string GetFormattedName()
            {
                if (this.owner != null)
                {
                    return $"[{colourNames[this.owner.GetId()]}]{this.name}[white]";
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
        public static Animal[] locations = new Animal[26] // Prices copied from regular Monopoly, but scaled by the fact that you get £500 instead of £200, and rounded to the nearest £5
        {
            new Animal("Start", [], 0, "", ""),
            // Common in UK
            new Animal("Squirrel", [25, 75, 225, 400], 125, "Common", "CO"),
            new Animal("Sparrow", [50, 150, 450, 800], 125, "Common", "CO"),
            // Rarer in UK
            new Animal("Hedgehog", [75, 225, 675, 1200], 125, "Rare", "RA"),
            new Animal("Fox", [75, 225, 675, 1200], 125, "Rare", "RA"),
            new Animal("Badger", [100, 250, 750, 1125], 125, "Rare", "RA"),
            // UK wild animals
            new Animal("Deer", [125, 375, 1125, 1560], 250, "Wild", "WI"),
            new Animal("Bat", [125, 375, 1125, 1560], 250, "Wild", "WI"),
            new Animal("Wildcat", [150, 450, 1250, 1750], 250, "Wild", "WI"),
            // Least concern
            new Animal("Arctic fox", [175, 500, 1375, 1875], 250, "Least concern", "LC"),
            new Animal("Brown bear", [175, 500, 1375, 1875], 250, "Least concern", "LC"),
            new Animal("Kangaroo", [200, 550, 1500, 2000], 250, "Least concern", "LC"),
            // Near-threatened
            new Animal("Jaguar", [225, 625, 1750, 2190], 375, "Near-threatened", "NT"),
            new Animal("Miss a turn", [], 0, "", ""),
            new Animal("White rhino", [225, 625, 1750, 2190], 375, "Near-threatened", "NT"),
            new Animal("Bison", [250, 750, 1875, 2310], 375, "Near-threatened", "NT"),
            // Vulnerable
            new Animal("Cheetah", [275, 825, 2000, 2440], 375, "Vulnerable", "VU"),
            new Animal("Lion", [275, 825, 2000, 2440], 375, "Vulnerable", "VU"),
            new Animal("Polar Bear", [300, 900, 2125, 2560], 375, "Vulnerable", "VU"),
            // Endangered
            new Animal("Elephant", [325, 975, 2250, 2750], 500, "Endangered", "EN"),
            new Animal("Tiger", [375, 975, 2250, 2750], 500, "Endangered", "EN"),
            new Animal("Chimpanzee", [375, 1125, 2500, 3000], 500, "Endangered", "EN"),
            // Critically endangered
            new Animal("Black Rhino", [440, 1250, 2750, 3250], 500, "Critically Endangered", "CR"),
            new Animal("Orangutan", [500, 1500, 3500, 4250], 500, "Critically Endangered", "CR"),
            // Fictional (not in base Monopoly)
            new Animal("Dragon", [560, 1750, 4250, 5250], 1000, "Fictional", "FI"),
            new Animal("Unicorn", [625, 2000, 5000, 6250], 1000, "Fictional", "FI"),
        };
    }
}