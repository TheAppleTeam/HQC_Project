namespace Poker.GameObjects
{
    public class Table
    {
        private double type;

        private bool intsadded;
        private bool changed;
        public bool raising = false;


        public Table()
        {
            this.BigBlind = GlobalConstants.InitialBigBlind;
            this.SmallBlind = GlobalConstants.InitialSmallBlind;
            this.PokerCall = GlobalConstants.InitialSmallBlind;
            this.TurnCount = 0;
            this.Rounds = 0;
            this.WinnersCount = 0;
        }
        
        public int PokerCall { get; set; }
        
        public int BigBlind { get; set; }
        
        public int SmallBlind { get; set; }
        
        public int TurnCount { get; set; }

        public int Pot { get; set; }

        public int WinnersCount { get; set; }

        public int Rounds { get; set; }

        public int LastRaise { get; set; }
    }
}
