namespace Poker.GameObjects.Player
{
    using System.Windows.Forms;

    public interface IPlayer
    {
        string Name { get; set; }

        int Chips { get; set; }

        bool Turn { get; set; }

        bool GameEnded { get; set; }

        string Status { get; set; }

        double PokerHandMultiplier { get; set; }

        double CardPower { get; set; }

        bool Folded { get; set; }

        int Call { get; set; }

        int Raise { get; set; }

        Panel CardsPanel { get;  }

        Label Label { get; set; }
    }
}
