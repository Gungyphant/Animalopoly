using static Animalopoly.PlayerClass;
using static Animalopoly.Writing;
namespace Animalopoly
{
    class CardClass
    {
        public class Card
        {
            public int reward;
            public string name;
            public string details;
            public Card(int reward, string name, string details)
            {
                this.reward = reward;
                this.name = name;
                this.details = details;
            }
            public void award(Player player)
            {
                Console.WriteLine(name);
                WriteLine(details);
                player.changeMoney(this.reward);
            }
        }
        public static (int, string, string)[] cards = new (int, string, string)[3] { // (reward, name, details)
                (100, "Business is booming!", "The extra customers meant you made an extra £100 in profit"),
                (-200, "Food prices are up!", "You had to spend an extra £200 to feed your animals"),
                (-1000, "Sued!", "Someone got hurt trying to see your animals, and they sued you for £1000!"),
            };
        public static Card getRandomCard((int, string, string)[] cards)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, cards.Length);
            (int, string, string) data = cards[index];
            return new Card(data.Item1, data.Item2, data.Item3);
        }
    }
}