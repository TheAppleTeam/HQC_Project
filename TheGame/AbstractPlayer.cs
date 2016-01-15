namespace GameObjects.Players
{
    using System;

    protected abstract class AbstractPlayer
    {
        public int Chips { get; set; }
        public double PokerHandMultiplier { get; set; }
        public double CardPower { get; set; }

    }
}
