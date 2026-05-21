using MsgPack.Serialization;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.Commands;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;
using static Animalopoly.Code.AI;
using System.Runtime.Serialization;

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
        public enum BankruptcyStatus
        {
            Normal,
            Warned,
            RecentlyBankrupt,
            NonrecentlyBankrupt,
        }
        public class Player
        {
            [MessagePackMember(0)]
            private readonly char piece;

            [MessagePackMember(1)]
            private readonly int id;

            [MessagePackMember(2)]
            private int money;

            [MessagePackMember(3)]
            private bool debtWarning;

            [MessagePackMember(4)]
            private int cellId;

            [MessagePackMember(5)]
            private bool skipTurn;

            [MessagePackMember(6)]
            private BankruptcyStatus bankruptStatus;

            [MessagePackMember(7)]
            private int AILevel;

            //[MessagePackMember(8)] // Cannot be serialised; see FixInconsistencies()
            public Func<string, Animal, Player, bool> GetResponse;

            [MessagePackMember(8)]
            private readonly string name;

            static Random rnd = new Random();

            public Player(char piece, int id, string name)
            {
                this.piece = piece;
                this.id = id;
                this.name = name;
                money = 3750;
                debtWarning = false;
                cellId = 0;
                skipTurn = false;
                AILevel = 0;
                GetResponse = AI.Human;
            }

            public void SetSkip(bool newVal)
            {
                this.skipTurn = newVal;
            }
            public bool GetSkip() 
            { 
                return skipTurn; 
            }
            public void SetBankruptStatus(BankruptcyStatus newVal)
            {
                //if (newVal < this.bankruptStatus)
                //{
                //    throw new Exception("Tried to decrease Bankruptcy status");
                //} // Don't remember why this was written so leaving it as a comment just in case
                this.bankruptStatus = newVal;
            }
            public BankruptcyStatus GetBankruptStatus()
            {
                return bankruptStatus;
            }
            public char GetPiece()
            {
                return piece;
            }
            public int GetId()
            {
                return id;
            }
            public string GetName()
            {
                return name;
            }
            public int GetMoney()
            {
                return money;
            }
            public void ChangeMoney(int change)
            {
                money += change;
                if (money < 0 && !debtWarning)
                {
                    WriteLine($"[{colourNames[id]}]{piece}[white] is in danger of bankruptcy...");
                    debtWarning = true;
                }
                else if (money > 0 && debtWarning)
                {
                    WriteLine($"[{colourNames[id]}]{piece}[white] is no longer in danger of bankruptcy (They have {FormatBalance(money)})");
                    debtWarning = false;
                    this.bankruptStatus = BankruptcyStatus.Normal;
                }
            }
            public bool GetDebtWarning()
            {
                return debtWarning;
            }
            public int GetPos()
            {
                return cellId % 26;
            }
            //public int GetAILevel()
            //{
            //    return AILevel;
            //}
            public bool IsAI()
            {
                return AILevel == 0;
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
                this.GetResponse = new Func<string, Animal, Player, bool>[] { AI.Human, AI.Easy, AI.Medium, AI.Hard, AI.Expert } [AILevel];
            }
            [OnDeserialized]
            private void FixInconsistencies() // this.GetResponse cannot be serialised, as it is a delegate, so when the player is deserialised, it must fix its GetResponse via this
            {
                this.SetAILevel(this.AILevel);
            }
            public void Move(int cells)
            { // Makes the player move cells spaces along the board; doesn't 'land' on the destination
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
                if (cellId > 26)
                {
                    WriteLine($"[{colourNames[id]}]{piece}[white] passed Start and got {FormatMoneyChange(500, true)}");
                    ChangeMoney(500);
                }
                cellId %= 26;
            }
            public void Roll()
            { // Rolls the dice and then moves the resulting amount
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
            //public bool GetResponse(string question, Animal animal)
            //{ // If the player is a human, prints the relevant text and asks what they want to do. If the player is an AI, calls the relevant function to determine what to do
            //    string? response;
            //    switch (this.AILevel)
            //    {
            //        case 0:
            //            switch (question)
            //            {
            //                case "buy":
            //                    WriteLine($"Nobody owns this animal. It's in the set {animal.GetSet()}. Do you want to buy it for {FormatMoney(animal.GetBuyCost())}? (you have {FormatBalance(this.GetMoney())}) (y/n)");
            //                    response = ReadLine();
            //                    return response.Equals("y", StringComparison.CurrentCultureIgnoreCase);
            //                case "upgrade":
            //                    WriteLine($"You own this animal. Do you want to upgrade it for {FormatMoney(animal.GetBuyCost())}? (you have {FormatBalance(this.money)}) (y/n)");
            //                    response = ReadLine();
            //                    return response.Equals("y", StringComparison.CurrentCultureIgnoreCase);
            //                default:
            //                    throw new Exception($"Unknown question {question}");
            //            }
            //        case 1:
            //            return Easy(question, animal, this);
            //        case 2:
            //            return Medium(question, animal, this);
            //        case 3:
            //            return Hard(question, animal, this);
            //        case 4:
            //            return Expert(question, animal, this);
            //        default:
            //            throw new Exception($"Invalid AI level {this.AILevel}");
            //    }
            //}
        }
    }
}