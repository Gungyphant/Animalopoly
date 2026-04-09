using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Program;
using static Animalopoly.Code.TileClasses;

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
            int moneyAfterBuying = player.GetMoney() - animal.GetBuyCost();

            int mostExpensiveStopCost = 0;
            foreach (Tile tileToCheck in locations)
            {
                if (
                    tileToCheck is Animal animalToCheck // Check it's an Animal and convert it if it is
                    && animalToCheck.GetOwner() is not null // Check it's owned
                    && animalToCheck.GetStopCost() > mostExpensiveStopCost // Is it more expensive?
                    )
                {
                    mostExpensiveStopCost = animalToCheck.GetStopCost();
                }
            }

            int mostCostlyCardCost = 0;
            foreach ((int, string, string) card in cards)
            {
                int cardCost = -card.Item1;
                if (cardCost > mostCostlyCardCost)
                {
                    mostCostlyCardCost = cardCost;
                }
            }

            int maximumTurnSpending = mostExpensiveStopCost + mostCostlyCardCost;
            return (moneyAfterBuying - maximumTurnSpending) > 0;
        }
        private const double HARD_TURN_THRESHOLD = 20; // Abritrary value
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
                throw new Exception($"Unknown question '{question}'");
            }
            double landsToEarnBack = (double)cost / gainPerStop;
            // When there are P players, there are (P - 1) other players. If it is assumed they are in random positions around the board, each of them
            //  has a 1/26 chance of landing on this property on their turn, so there are an expected (P - 1)/26 lands per turn
            double landsPerTurn = (players.Length - 1) / 26.0;
            double turnsToEarnBack = landsToEarnBack / landsPerTurn;
            return (turnsToEarnBack <= HARD_TURN_THRESHOLD);
        }
        public static bool Expert(string question, Animal animal, Player player)
        {
            // Simulate future rounds to maximise the probabilty of winning; need to implement
            return false;
        }
    }
}
