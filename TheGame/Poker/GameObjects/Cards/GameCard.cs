namespace Poker.GameObjects.Cards
{
    using UI;

    public class GameCard
    {
        public GameCard()
        {
            this.CardBackImageUri = "../Resources/Cards/Back.png";
        }

        public int Id { get; set; }

        public int DealtPosition { get; set; }
        
        public CardSuit Suit { get; set; }

        public string CardFrontImageUri { get; set; }

        public string CardBackImageUri { get; private set; }

        public int Rank { get; set; }
        
        /// <summary>
        /// If is TRUE the face of the card must be shown
        /// </summary>
        public bool IsVisible { get; set; }

        public bool IsPresentOnTable { get; set; }
    }
}
