using static Animalopoly.Program;
using static Animalopoly.Writing;
using static Animalopoly.CardClass;

namespace Animalopoly
{
    class PlayerClass
    {
        public class Player
        {
            private char name;
            private int id;
            private int money;
            private bool bankruptWarning;
            private int cellId;
            private bool skipTurn;
            private int bankruptStatus; // 0: Normal, 1: Turn started since warning, 2: Bankrupt this turn, 3: Bankrupt before this turn

            public Player(char name, int id)
            {
                this.name = name;
                this.id = id;
                money = 3750;
                bankruptWarning = false;
                cellId = 0;
                skipTurn = false;
            }
            public void setSkip(bool newVal)
            {
                this.skipTurn = newVal;
            }
            public bool getSkip() 
            { 
                return skipTurn; 
            }
            public void setBankruptStatus(int newVal)
            {
                if (newVal < this.bankruptStatus)
                {
                    throw new Exception("Tried to decrease Bankruptcy status");
                }
                this.bankruptStatus = newVal;
            }
            public int getBankruptStatus()
            {
                return bankruptStatus;
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
                    WriteLine($"[{colourNames[id]}]{name}[white] is in danger of bankruptcy...");
                    bankruptWarning = true;
                }
                else if (money > 0 && bankruptWarning)
                {
                    WriteLine($"[{colourNames[id]}]{name}[white] is no longer in danger of bankruptcy (They have £{money})");
                    bankruptWarning = false;
                    this.bankruptStatus = 0;
                }
            }
            public bool getBankruptWarning()
            {
                return bankruptWarning;
            }
            public int getPos()
            {
                return cellId;
            }
            public void move(int cells)
            {
                cellId += cells;
                if (cellId >= 26)
                {
                    if (cellId == 26)
                    {
                        WriteLine($"[{colourNames[id]}]{name}[white] landed on Start and got £1000");
                        changeMoney(1000);
                    }
                    else
                    {
                        WriteLine($"[{colourNames[id]}]{name}[white] passed Start and got £500");
                        changeMoney(500);
                    }
                }
                cellId %= 26;
            }
            public void Roll()
            {
                Random rnd = new Random();
                int die1 = rnd.Next(1, 7);
                int die2 = rnd.Next(1, 7);
                for (int _ = 0; _ < 10; _++) // Show the dice 'rolling'
                {
                    Write($"{new string('\b', 5)}{rnd.Next(1, 7)} + {rnd.Next(1, 7)}");
                    Thread.Sleep(50);
                }
                WriteLine($"{new string('\b', 5)}{die1} + {die2} = {die1 + die2}");
                if (die1 == die2)
                {
                    getRandomCard(cards).award(this);
                    Thread.Sleep(100);
                }
                move(die1 + die2);
                //return die1 + die2;
            }
        }
    }
}