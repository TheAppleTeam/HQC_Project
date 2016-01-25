namespace Poker.GameObjects.Player
{
    using UI;

    public abstract class AbstractPlayer : IPlayer
    {
        private const int InitialChips = 10000;
        private readonly int id;
        private int chips;
        // private Panel cardsPanel;
        
        protected AbstractPlayer(int id, string name, int firstCardNumeration, int secondCardNumeration)
        {
        // this.CardsPanel = new Panel();
        // this.Label = label;
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

     //   public Panel CardsPanel
        //{
        //    get
        //    {
        //        return this.cardsPanel;
        //    }

        //    private set
        //    {
        //        this.cardsPanel = value;
        //    }
        //}

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
                if (value <= 0)
                {
                    this.chips = 0;
                }
                else
                {
                    this.chips = value;    
                }
            }
        }

        public bool Turn { get; set; }

        public bool GameEnded { get; set; }

        //// съдържанието на полето се изписва в лейбъла - например raise 200
        public string Status { get; set; }

        public double PokerHandMultiplier { get; set; }

        public double CardPower { get; set; }

        public bool Folded { get; set; }

        public int Call { get; set; }

        public int Raise { get; set; }

   //     public Label Label { get; set; }

        public int FirstCardPosition { get; set; }

        public int SecondCardPosition { get; set; }
    }
}
