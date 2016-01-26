namespace Poker.GameObjects.Cards
{
    using UI;

    public class PepsterCard
    {
        public PepsterCard()
        {
            this.CardBackImageUri = "../Resources/Cards/Back.png";
        }

        public int Id { get; set; }

        public int DealtPosition { get; set; }
        
        public CardSuit Suit { get; set; }

        public string CardFrontImageUri { get; set; }

        public string CardBackImageUri { get; private set; }
        
        /// <summary>
        /// If is TRUE the face of the card must be shown
        /// </summary>
        public bool IsVisible { get; set; }
    }
}
