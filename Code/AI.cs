using System.ComponentModel;
using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Commands;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.NetProcessingUI;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;

namespace Animalopoly.Code
{
    class AI
    {
        public static bool Easy(string question, Animal animal, Player player)
        {
            // Always buy/upgrade
            return true;
        }
        public static bool Medium(string question, Animal animal, Player player)
        {
            // Only buy/upgrade if, after doing so, it is impossible to bankrupt next turn
            int mostExpensiveStopCost = 0;
            foreach (Tile tile in locations)
            {
                if (tile is Animal animal_to_check && animal_to_check.GetOwner() is not null && animal_to_check.GetStopCost() > mostExpensiveStopCost)
                {
                    mostExpensiveStopCost = animal_to_check.GetStopCost();
                }
            }

            int mostCostlyCardCost = 0;
            foreach ((int, string, string) card in cards)
            {
                if (-card.Item1 > mostCostlyCardCost)
                {
                    mostCostlyCardCost = -card.Item1;
                }
            }

            return (player.GetMoney() - animal.GetBuyCost() - mostExpensiveStopCost - mostCostlyCardCost) > 0;
        }
        private static double HARD_TURNTHRESHOLD = 20; // Abritrary value
        public static bool Hard(string question, Animal animal, Player player)
        {
            // Buy/upgrade if the charge/cost is above a certain threshold, and Medium
            if (!Medium(question, animal, player))
            {
                return false;
            }
            int cost = animal.GetBuyCost();
            int gainPerStop;
            if (question == "buy")
            {
                gainPerStop = animal.GetStopCost();
            }
            else if (question == "upgrade")
            {
                gainPerStop = animal.GetNextStopCost() - animal.GetStopCost();
            }
            else
            {
                throw new Exception(question);
            }
            double landsToEarnBack = (double)cost / gainPerStop;
            // When there are P players, there are (P - 1) other players. If it is assumed they are in random positions around the board, each of them
            //  has a 1/26 chance of landing on this property on their turn, so there are an expected (P - 1)/26 lands per turn
            double landsPerTurn = (players.Length - 1) / 26.0;
            double turnsToEarnBack = landsToEarnBack / landsPerTurn;
            return (turnsToEarnBack <= HARD_TURNTHRESHOLD);
        }
        public static bool Expert(string question, Animal animal, Player player)
        {
            // Simulate future rounds to maximise the probabilty of winning
            return false;
        }
    }
}
