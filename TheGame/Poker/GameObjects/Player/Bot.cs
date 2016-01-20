namespace Poker.GameObjects.Player
{
    using System.Reflection.Emit;
    using Label = System.Windows.Forms.Label;

    public class Bot : AbstractPlayer 
    {
        public Bot( string name,Label label) : base(name,label)
        {
            this.Turn = false;
        }
    }
}
