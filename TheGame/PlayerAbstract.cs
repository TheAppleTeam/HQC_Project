namespace GameObjects.GamePlayers
{
    using System;

    protected abstract class PlayerAbstract
    {
        private int playerChips = InitialChips;

        public PlayerAbstract()
        {
            
        }

        public int  Chips { get; set; }
        public Type PokerHandMultiplier { get; set; }
    }
}

