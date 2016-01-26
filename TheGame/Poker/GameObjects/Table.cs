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
            this.LastRaise = 0;
            this.FoldedBots = 0;
            this.PlayersInTheGame = GlobalConstants.PlayersCount;
            this.IsRaising = false;
        }
        
        public int PokerCall { get; set; }
        
        public int BigBlind { get; set; }
        
        public int SmallBlind { get; set; }
        
        public int TurnCount { get; set; }

        public int Pot { get; set; }

        public int WinnersCount { get; set; }

        public int Rounds { get; set; }

        public int LastRaise { get; set; }

        public bool IsRaising { get; set; }

        /// <summary>
        /// Shous the numner of bots that are not in the game. 
        /// Initial Value = 0;
        /// </summary>
        public int FoldedBots { get; set; }

        /// <summary>
        /// Shous the number of the players still in the game;
        /// Initial Value = 6 ( GlobalConstants.PlayersCount )
        /// 
        /// used in CheckRaise method :   if (Table.Rounds == End && Table.PlayersInTheGame == 6);
        /// used in AllIn method->  #region FiveOrLessLeft: if (abc < 6 && abc > 1 && Table.Rounds >= End) 
        /// in Finish method is seted again on 4;
        /// </summary>
        public int PlayersInTheGame { get; set; }

        public int LastRaisedPlayerId { get; set; }
    }
}
