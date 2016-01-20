namespace Poker.GameObjects.Player
{
    using System.Drawing.Text;
    using System.Windows.Forms;

    public abstract class AbstractPlayer : IPlayer
    {
        protected  Panel cardsPanel;
        private const int InitialChips = 10000;

        protected AbstractPlayer( string name)
        {
            this.CardsPanel = new Panel();
            this.GameEnded = false;
            this.Chips = InitialChips;
            this.Folded = false;
            this.Call = 0;
            this.Raise = 0;
            this.PokerHandMultiplier = -1;
            this.Name = name;

        }

        public Panel CardsPanel
        {
            get { return this.cardsPanel; }
            private set
            {
                this.cardsPanel=value;
            }
        }

        public string Name { get; set; }

        public int Chips { get; set; }

        public bool Turn { get; set; }

        public bool GameEnded { get; set; }

        //съдържанието на полето се изписва в лейбъла - например raise 200
        public string Status { get; set; }

        public double PokerHandMultiplier { get; set; }

        public double CardPower { get; set; }

        public bool Folded { get; set; }

        public int Call { get; set; }

        public int Raise { get; set; }


    }
}
