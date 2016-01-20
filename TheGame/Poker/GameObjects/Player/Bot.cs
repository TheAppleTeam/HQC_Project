namespace Poker.GameObjects.Player
{
    public class Bot : AbstractPlayer 
    {
        public Bot( string name) : base(name)
        {
            this.Turn = false;
        }
    }
}
