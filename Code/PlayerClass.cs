using MsgPack.Serialization;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.Commands;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;
using static Animalopoly.Code.AI;

namespace Animalopoly.Code
{
    public class PlayerClass
    {
        private static int shownDie1;
        private static int shownDie2;
        public static (int, int) GetDice()
        {
            return (shownDie1, shownDie2);
        }
        public class Player
        {
            [MessagePackMember(0)]
            private readonly char name;

            [MessagePackMember(1)]
            private readonly int id;

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

            [MessagePackMember(7)]
            private int AILevel;

            public Player(char name, int id)
            {
                this.name = name;
                this.id = id;
                money = 3750;
                bankruptWarning = false;
                cellId = 0;
                skipTurn = false;
                AILevel = 0;
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
                //if (newVal < this.bankruptStatus)
                //{
                //    throw new Exception("Tried to decrease Bankruptcy status");
                //} // Don't remember why this was written so leaving it as a comment just in case
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
                return cellId % 26;
            }
            public int GetAILevel()
            {
                return AILevel;
            }
            public void SetAILevel(int AILevel)
            {
                if (AILevel > 4 || AILevel < 0)
                {
                    throw new Exception($"Invalid AI level {AILevel}");
                }
                else
                {
                    this.AILevel = AILevel;
                }
            }
            public void Move(int cells)
            {
                if (animations)
                {
                    for (int _ = 0; _ < cells; _++) // Animate piece movement
                    {
                        cellId++;
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    cellId += cells;
                }
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
                if (animations) // Show the dice 'rolling'
                {
                    for (int _ = 0; _ < 10; _++)
                    {
                        shownDie1 = rnd.Next(1, 7);
                        shownDie2 = rnd.Next(1, 7);
                        if (!guiMode)
                        {
                            Write($"{new string('\b', 5)}{shownDie1} + {shownDie2}");
                        }
                        Thread.Sleep(50);
                    }
                }
                if (guiMode)
                {
                    shownDie1 = die1;
                    shownDie2 = die2;
                }
                WriteLine($"{new string('\b', 5)}{die1} + {die2} = {die1 + die2}");
                if (die1 == die2)
                {
                    GetRandomCard(cards).Award(this);
                    if (animations)
                    {
                        Thread.Sleep(100);
                    }
                }
                Move(die1 + die2);
                //return die1 + die2;
            }
            public bool GetResponse(string question, Animal animal)
            {
                string? response;
                switch (this.AILevel)
                {
                    case 0:
                        switch (question)
                        {
                            case "buy":
                                WriteLine($"Nobody owns this animal. It's in the set {animal.GetSet()}. Do you want to buy it for £{animal.GetBuyCost()}? (you have £{this.GetMoney()}) (y/n)");
                                response = ReadLine();
                                return response.Equals("y", StringComparison.CurrentCultureIgnoreCase);
                            case "upgrade":
                                WriteLine($"You own this animal. Do you want to upgrade it for £{animal.GetBuyCost()}? (you have £{this.money}) (y/n)");
                                response = ReadLine();
                                return response.Equals("y", StringComparison.CurrentCultureIgnoreCase);
                            default:
                                throw new Exception($"Unknown question {question}");
                        }
                    case 1:
                        return Easy(question, animal, this);
                    case 2:
                        return Medium(question, animal, this);
                    case 3:
                        return Hard(question, animal, this);
                    case 4:
                        return Expert(question, animal, this);
                    default:
                        throw new Exception($"Invalid AI level {this.AILevel}");
                }
            }
        }
    }
}