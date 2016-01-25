namespace Poker.GameObjects.Player
{
    public class Gamer : AbstractPlayer
    {
        public Gamer()
            : base(0, "Player", 0, 1)
        {
            this.Turn = true;
        }

        public bool CanCall { get; set; }

        public bool CanRaise { get; set; }

        public bool CanFold { get; set; }

        public bool CanCheck { get; set; }

        public bool IsAllIn { get; set; }
    }
}
