namespace Poker.GameObjects.Player
{
    using System.Drawing.Text;
    using System.Windows.Forms;

    public abstract class AbstractPlayer : IPlayer
    {
        private const int InitialChips = 10000;
        private Panel cardsPanel;

        protected AbstractPlayer(string name, Label label)
        {
            this.CardsPanel = new System.Windows.Forms.Panel();
            this.GameEnded = false;
            this.Chips = InitialChips;
            this.Folded = false;
            this.Call = 0;
            this.Raise = 0;
            this.PokerHandMultiplier = -1;
            this.Name = name;
            this.Label = label;
            this.Label = new System.Windows.Forms.Label();
        }

        public Panel CardsPanel
        {
            get
            {
                return this.cardsPanel;
            }

            private set
            {
                this.cardsPanel = value;
            }
        }

        public string Name { get; set; }

        public int Chips { get; set; }

        public bool Turn { get; set; }

        public bool GameEnded { get; set; }

        //// съдържанието на полето се изписва в лейбъла - например raise 200
        public string Status { get; set; }

        public double PokerHandMultiplier { get; set; }

        public double CardPower { get; set; }

        public bool Folded { get; set; }

        public int Call { get; set; }

        public int Raise { get; set; }

        public Label Label { get; set; }
    }
}
