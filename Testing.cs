using static Animalopoly.Code.CardClass;
using static Animalopoly.Code.CommandLineInterface;
using static Animalopoly.Code.Commands;
using static Animalopoly.Code.Graphing;
using static Animalopoly.Code.NetProcessingUI;
using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.TileClasses;
using static Animalopoly.Code.Writing;
using static Animalopoly.Code.Program;

namespace Animalopoly
{
    public class Testing
    {
        public static bool Thirteen()
        {
            // Roll 10_000_000 times. Is the returned distribution ~= the expected distribution?
            Dictionary<int, int> counts = new Dictionary<int, int>
            {
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
                { 5, 0 },
                { 6, 0 },
                { 7, 0 },
                { 8, 0 },
                { 9, 0 },
                { 10, 0 },
                { 11, 0 },
                { 12, 0 },
            };
            int observationCount = 10_000_000;
            for (int i = 0; i < observationCount; i++)
            {
                // The relevant section of Player.Roll:
                Random rnd = new Random();
                int die1 = rnd.Next(1, 7);
                int die2 = rnd.Next(1, 7);

                int result = die1 + die2;
                counts[result]++;
            }
            for (int i = 2; i <= 12; i++)
            {
                double observed_percentage = counts[i] * 100.0 / observationCount;
                double expected_percentage = (i < 7 ? i - 1 : 13 - i) * 100 / 36.0;
                double variation = observed_percentage / expected_percentage - 1;
                //Console.WriteLine($"{i} {observed_percentage}% {expected_percentage}% {variation * 100}%");
                if (Math.Abs(variation) > 0.01)
                {
                    return false;
                }
            }
            return true;
        }
    }
}