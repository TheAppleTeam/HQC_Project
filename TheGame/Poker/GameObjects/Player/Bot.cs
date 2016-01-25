namespace Poker.GameObjects.Player
{
    public class Bot : AbstractPlayer
    {
        public Bot(int id, string name, int firstCardNumeration, int secondCardNumeration)
            : base(id, name, firstCardNumeration, secondCardNumeration)
        {
            this.Turn = false;
        }
    }
}