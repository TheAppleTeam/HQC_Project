namespace GameObjects.Players
{
    public abstract class AbstractPlayer
    {
        public int Chips { get; set; }

        public bool Turn { get; set; }

        public bool GameEnded { get; set; }

        //съдържанието на полето се изписва в лейбъла - например raise 200
        public string Status { get; set; }

        public double PokerHandMultiplier { get; set; }

        public double CardPower { get; set; }

    }
}
