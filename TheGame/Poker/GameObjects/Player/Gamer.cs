namespace Poker.GameObjects.Player
{
    using System.Windows.Forms;

    public class Gamer : AbstractPlayer
    {
        public Gamer(Label label) : base("player",label)
        {
            this.Turn = true;
        }
    }
}
