using MsgPack.Serialization;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.Writing;

namespace Animalopoly.Code
{
    public class PlayerClass
    {
        public class Player
        {
            [MessagePackMember(0)]
            private char name;

            [MessagePackMember(1)]
            private int id;

            [MessagePackMember(2)]
            private int money;

            [MessagePackMember(3)]
            private bool bankruptWarning;

            [MessagePackMember(4)]
            private int cellId;

            [MessagePackMember(5)]
            private bool skipTurn;

            [MessagePackMember(6)]
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
            public void SetSkip(bool newVal)
            {
                this.skipTurn = newVal;
            }
            public bool GetSkip() 
            { 
                return skipTurn; 
            }
            public void SetBankruptStatus(int newVal)
            {
                if (newVal < this.bankruptStatus)
                {
                    throw new Exception("Tried to decrease Bankruptcy status");
                }
                this.bankruptStatus = newVal;
            }
            public int GetBankruptStatus()
            {
                return bankruptStatus;
            }
            public char GetName()
            {
                return name;
            }
            public int GetId()
            {
                return id;
            }
            public int GetMoney()
            {
                return money;
            }
            public void ChangeMoney(int change)
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
            public bool GetBankruptWarning()
            {
                return bankruptWarning;
            }
            public int GetPos()
            {
                return cellId;
            }
            public void Move(int cells)
            {
                cellId += cells;
                if (cellId >= 26)
                {
                    if (cellId == 26)
                    {
                        WriteLine($"[{colourNames[id]}]{name}[white] landed on Start and got £1000");
                        ChangeMoney(1000);
                    }
                    else
                    {
                        WriteLine($"[{colourNames[id]}]{name}[white] passed Start and got £500");
                        ChangeMoney(500);
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
                    GetRandomCard(cards).Award(this);
                    Thread.Sleep(100);
                }
                Move(die1 + die2);
                //return die1 + die2;
            }
        }
    }
}