using static Animalopoly.Code.Commands;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Writing;

namespace Animalopoly.Code
{
    public class TileClasses
    {
        public abstract class Tile
        {
            protected string name;
            public Tile(string name)
            {
                this.name = name;
            }
            public string GetName()
            {
                return name;
            }
            public abstract void Land(ref Player player); // Events that occur when landing on the tile
            public abstract string GetFormattedName(); // How the name should be printed
        }

        public class Set
        {
            private string name;// Shown on card & GUI board
            private string smallName;  // Shown on CLI board, and card if set is too long
            private string hexColour;
            public Set(string name, string smallName, string hexColour)
            {
                this.name = name;
                this.smallName = smallName;
                this.hexColour = hexColour;
            }
            public string GetName()
            {
                return this.name;
            }
            public string GetSmallName()
            {
                return this.smallName;
            }
            public string GetHexColour()
            {
                return this.hexColour;
            }
            public List<Animal> GetAnimalsInSet()
            {
                List<Animal> animalsInSet = new List<Animal>();
                foreach (Tile tile in locations)
                {
                    if (tile is Animal animal && animal.GetSet() == this)
                    {
                        animalsInSet.Add(animal);
                    }
                }
                return animalsInSet;
            }
            public int GetNumberOfAnimalsInSetWithOwner(Player? owner)
            {
                int result = 0;
                if (owner is null)
                {
                    result = 1;
                }
                else
                {
                    foreach (Animal animal in GetAnimalsInSet())
                    {
                        if (animal.GetOwner() == owner)
                        {
                            result++;
                        }
                    }
                }
                return result;
            }
        }
        public class Animal : Tile
        {
            protected int level;
            protected int[] stopCosts;
            protected int buyCost;
            protected Player? owner;
            protected Set set;
            //public Animal(string name, int[] stopCosts, int buyCost, string set, string smallSet, string setColour) : base(name) // Fully verbose constructor; allows for custom combinations of smallSet, set, and setColour that are not one of the standard sets
            //{
            //    this.name = name;
            //    this.level = 0;
            //    this.stopCosts = stopCosts;
            //    this.buyCost = buyCost;
            //    this.owner = default(Player);
            //    this.set = set;
            //    this.smallSet = smallSet;
            //    this.setColour = setColour;
            //}
            public Animal(string name, int[] stopCosts, int buyCost, Set set) : base(name)
            {
                this.name = name;
                this.level = 0;
                this.stopCosts = stopCosts;
                this.buyCost = buyCost;
                this.owner = default(Player);
                this.set = set;
            }
            public Set GetSet()
            {
                return set;
            }
            public string GetSetName()
            {
                return set.GetName();
            }
            public string GetSmallSet()
            {
                return set.GetSmallName();
            }
            public string GetSetColour()
            {
                return set.GetHexColour();
            }
            private int GetSetMultiplier()
            {
                // GetSetMultiplier() == Math.Pow(2, (GetNumberOfAnimalsInSetWithSameOwner() - 1)) currently, but this is hardcoded to make it easier to change
                int numberOfAnimalsInSet = this.set.GetNumberOfAnimalsInSetWithOwner(this.owner);
                int result = numberOfAnimalsInSet switch
                {
                    1 => 1,
                    2 => 2,
                    3 => 4,
                    _ => throw new Exception($"{numberOfAnimalsInSet} animals in one set"),
                };
                return result;
            }
            private int GetStopCostAtLevel(int level)
            {
                int result = stopCosts[level - 1]; // -1 as level is 1-indexed
                result *= GetSetMultiplier();
                return result;
            }
            public int GetStopCost() // Returns the current Stop Cost or, if level == 0, the next stop cost
            {
                if (level == 0)
                {
                    return GetStopCostAtLevel(1);
                }
                else
                {
                    return GetStopCostAtLevel(level);
                }
            }
            public int GetNextStopCost()
            {
                if (level + 1 >= stopCosts.Length)
                {
                    return GetStopCost();
                }
                else
                {
                    return GetStopCostAtLevel(level + 1);
                }
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
            public void ClearOwner() // This function is necessary as owner in SetOwner cannot be nullable, maybe because it doesn't want a reference to null and it's ref Player?
            {
                this.owner = null;
            }
            public string GetCard(bool upgrading = false)
            { // Gets the info card for the animal
                string levelString = $"Lvl {level}";

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
                    card += $"│ │     {(i + 1 == level ? "[white]" : (upgrading && (i + 1 == level + 1) ? "[yellow]" : "[grey]"))}{i + 1}: {FormatMoney(this.stopCosts[i])}[prev]{new string(' ', 8 - FormatMoney(this.stopCosts[i]).Length)}│ │\n";
                }
                card += $"│ └────────────────┘ │\n";
                card += $"│ ┌────────────────┐ │\n";
                card += $"│ │  Cost: {FormatMoney(this.buyCost)}{new string(' ', 8 - FormatMoney(this.buyCost).Length)}│ │\n";
                card += $"│ └────────────────┘ │\n";
                card += $"│ ┌────────────────┐ │\n";
                if (Convert.ToString(this.set).Length <= 8)
                {
                    card += $"│ │   Set: {this.set.GetName()}{new string(' ', 8 - Convert.ToString(this.set.GetName()).Length)}│ │\n";
                }
                else
                {
                    card += $"│ │   Set: {this.set.GetSmallName()}{new string(' ', 8 - Convert.ToString(this.set.GetSmallName()).Length)}│ │\n";
                }
                if (this.GetSetMultiplier() != 1)
                {
                    card += $"│ │  Mult: [dark yellow]x{this.GetSetMultiplier()}[prev]      │ │\n";
                }
                card += $"│ └────────────────┘ │\n";
                card += $"│ ┌────────────────┐ │\n";
                if (this.owner != null)
                {
                    if (this.owner.GetName().Length <= 8)
                    {
                        card += $"│ │ Owner: [{colourNames[this.owner.GetId()]}]{this.owner.GetName()}[white]{new string(' ', 8 - Convert.ToString(this.owner.GetName()).Length)}│ │\n";
                    }
                    else
                    {
                        card += $"│ │ Owner: [{colourNames[this.owner.GetId()]}]{this.owner.GetPiece()}[white]{new string(' ', 8 - Convert.ToString(this.owner.GetPiece()).Length)}│ │\n";
                    }
                }
                else
                {
                    card += $"│ │ Owner: None{new string(' ', 8 - "None".Length)}│ │\n";
                }
                card += $"│ └────────────────┘ │\n";
                card += $"└────────────────────┘";

                return card;
            }
            public override void Land(ref Player player)
            {
                if (this.owner is null)
                {
                    WriteLine(this.GetCard(this.level == 0));
                    if (player.GetResponse("buy", this, player))
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
                    if ((this.level + 1) <= this.stopCosts.Length)
                    {
                        WriteLine(this.GetCard(true));
                        if (player.GetResponse("upgrade", this, player))
                        {
                            player.ChangeMoney(-1 * this.buyCost);
                            this.level++;
                        }
                    }
                    else
                    {
                        WriteLine(this.GetCard(false));
                        if (this.owner.IsAI())
                        {
                            WriteLine($"You own this animal. You can't upgrade it any more");
                        }
                    }
                }
                else
                {
                    WriteLine(this.GetCard(false));
                    int animalsInSet = this.set.GetNumberOfAnimalsInSetWithOwner(this.owner);
                    WriteLine($"[{colourNames[this.owner.GetId()]}]{this.owner.GetName()}[prev] owns this animal. ");
                    if (this.owner.IsAI())
                    {
                        if (animalsInSet <= 1)
                        {
                            WriteLine($"You have to pay them a fee of {FormatMoneyChange(this.GetStopCost(), false)} (you now have {FormatBalance(player.GetMoney() - this.GetStopCost())})");
                        }
                        else
                        {
                            WriteLine($"They have {animalsInSet} animals from that set, so you have to pay them a fee of {FormatMoneyChange(this.GetStopCost(), false)} (you now have {FormatBalance(player.GetMoney() - this.GetStopCost())})");
                        }
                    }
                    player.ChangeMoney(-1 * this.GetStopCost());
                    this.owner.ChangeMoney(this.GetStopCost());
                }
            }
            //public void Upgrade()
            //{
            //    this.level++;
            //}
            public override string GetFormattedName()
            {
                if (this.owner != null)
                {
                    return $"[{colourNames[this.owner.GetId()]}]{this.name}[prev]";
                }
                else
                {
                    return this.name;
                }
            }
        }

        public class Start : Tile
        {
            public Start() : base("Start") { }
            public override string GetFormattedName()
            {
                return this.GetName();
            }
            public override void Land(ref Player player)
            {
                WriteLine($"[{colourNames[player.GetId()]}]{player.GetName()}[white] landed on Start and got {FormatMoneyChange(1000, true)}");
                player.ChangeMoney(1000);
                if (animations)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public class Miss : Tile
        {
            public Miss() : base("Miss a turn") { }
            public override string GetFormattedName()
            {
                return this.GetName();
            }
            public override void Land(ref Player player)
            {
                player.SetSkip(true);
                WriteLine("Miss a turn!");
            }
        }

        static Set common = new Set("Common", "CO", "#BFBFBF");
        static Set rare = new Set("Rare", "RA", "#89EF8B");
        static Set wild = new Set("Wild", "WI", "#439143");
        static Set least_concern = new Set("Least Concern", "LC", "#006666");
        static Set near_threatened = new Set("Near-threatened", "NT", "#9ACD9A");
        static Set vulnerable = new Set("Vulnerable", "VU", "#D9C771");
        static Set endangered = new Set("Endangered", "EN", "#E4C0A5");
        static Set crtically_endangered = new Set("Critically Endangered", "CR", "#E4A5A5");
        static Set fictional = new Set("Fictional", "FI", "#A46ACF");

        public static Tile[] locations = new Tile[26] // Prices copied from regular Monopoly, but scaled by the fact that you get £500 instead of £200, and rounded to the nearest £5
        {
            new Start(),
            // Common in UK
            new Animal("Squirrel", [25, 75, 225, 400], 125, common),
            new Animal("Sparrow", [50, 150, 450, 800], 125, common),
            // Rarer in UK
            new Animal("Hedgehog", [75, 225, 675, 1200], 125, rare),
            new Animal("Fox", [75, 225, 675, 1200], 125, rare),
            new Animal("Badger", [100, 250, 750, 1125], 125, rare),
            // UK wild animals
            new Animal("Deer", [125, 375, 1125, 1560], 250, wild),
            new Animal("Bat", [125, 375, 1125, 1560], 250, wild),
            new Animal("Wildcat", [150, 450, 1250, 1750], 250, wild),
            // Least concern
            new Animal("Arctic fox", [175, 500, 1375, 1875], 250, least_concern),
            new Animal("Brown bear", [175, 500, 1375, 1875], 250, least_concern),
            new Animal("Kangaroo", [200, 550, 1500, 2000], 250, least_concern),
            // Near-threatened
            new Animal("Jaguar", [225, 625, 1750, 2190], 375, near_threatened),
            new Miss(),
            new Animal("White rhino", [225, 625, 1750, 2190], 375, near_threatened),
            new Animal("Bison", [250, 750, 1875, 2310], 375, near_threatened),
            // Vulnerable
            new Animal("Cheetah", [275, 825, 2000, 2440], 375, vulnerable),
            new Animal("Lion", [275, 825, 2000, 2440], 375, vulnerable),
            new Animal("Polar Bear", [300, 900, 2125, 2560], 375, vulnerable),
            // Endangered
            new Animal("Elephant", [325, 975, 2250, 2750], 500, endangered),
            new Animal("Tiger", [375, 975, 2250, 2750], 500, endangered),
            new Animal("Chimpanzee", [375, 1125, 2500, 3000], 500, endangered),
            // Critically endangered
            new Animal("Black Rhino", [440, 1250, 2750, 3250], 500, crtically_endangered),
            new Animal("Orangutan", [500, 1500, 3500, 4250], 500, crtically_endangered),
            // Fictional (not in base Monopoly)
            new Animal("Dragon", [560, 1750, 4250, 5250], 1000, fictional),
            new Animal("Unicorn", [625, 2000, 5000, 6250], 1000, fictional),
        };
    }
}