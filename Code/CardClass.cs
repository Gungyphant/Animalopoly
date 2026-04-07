using static Animalopoly.Code.PlayerClass;
using static Animalopoly.Code.Writing;
namespace Animalopoly.Code
{
    class CardClass
    {
        public class Card
        {
            private readonly int reward;
            private readonly string name;
            private readonly string details;
            public Card(int reward, string name, string details)
            {
                this.reward = reward;
                this.name = name;
                this.details = details;
            }
            public void Award(Player player)
            {
                WriteLine(name);
                WriteLine(details);
                player.ChangeMoney(this.reward);
            }
        }
        public static (int, string, string)[] cards = new (int, string, string)[3] { // (reward, name, details)
            (100, "Business is booming!", "The extra customers meant you made an extra £100 in profit"),
            (-200, "Food spoiled!", "You had to spend an extra £200 to replace it"),
            (-1000, "Sued!", "Someone got hurt trying to see your animals, and they sued you for £1000!"),
        };
        public static Card GetRandomCard((int, string, string)[] cards)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, cards.Length);
            (int, string, string) data = cards[index];
            return new Card(data.Item1, data.Item2, data.Item3);
        }
    }
}