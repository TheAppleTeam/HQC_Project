namespace Poker.GameObjects.Player
{
    public class Gamer : AbstractPlayer
    {
        public Gamer() : base("player")
        {
            this.Turn = true;
        }
    }
}
