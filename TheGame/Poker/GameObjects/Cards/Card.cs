namespace Poker.GameObjects.Cards
{
    public class Card
    {
        // the enumeration of the art from 2 -> 14
        public int CardRang { get; set; }

        public CardSuit Suit { get; set; }

        public int CardNumeration { get; set; }
    }
}