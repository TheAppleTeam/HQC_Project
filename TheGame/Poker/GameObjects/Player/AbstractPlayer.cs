namespace Poker.GameObjects.Player
{
    using UI;

    public abstract class AbstractPlayer : IPlayer
    {
        private const int InitialChips = 10000;
        private readonly int id;
        private int chips;

        protected AbstractPlayer(int id, string name, int firstCardNumeration, int secondCardNumeration)
        {
            this.id = id;
            this.GameEnded = false;
            this.Chips = InitialChips;
            this.Folded = false;
            this.Call = 0;
            this.Raise = 0;
            this.PokerHandMultiplier = -1;
            this.Name = name;
                      
            this.FirstCardPosition = firstCardNumeration;
            this.SecondCardPosition = secondCardNumeration;
        }

        public int Id 
        {
            get { return this.id; }
        }
        
        public string Name { get; set; }

        public int Chips
        {
            get
            {
                return this.chips;
            }

            set
            {
                this.chips = value <= 0 ? 0 : value;
            }
        }

        /// <summary>
        /// Shows if is Players Turn
        /// </summary>
        public bool Turn { get; set; }

        public bool GameEnded { get; set; }

        /// <summary>
        /// Is uset to set value of Player's Lable 
        /// </summary>
        public string Status { get; set; }

        public int PokerHandMultiplier { get; set; }

        public double CardPower { get; set; }

        /// <summary>
        /// Shows if Playr is Folded
        /// </summary>
        public bool Folded { get; set; }
        
        /// <summary>
        /// Shows the value of Player's Call
        /// </summary>
        public int Call { get; set; }
        
        /// <summary>
        /// Shows the value of Player'sRase
        /// </summary>
        public int Raise { get; set; }

        public int FirstCardPosition { get; set; }

        public int SecondCardPosition { get; set; }
    }
}
