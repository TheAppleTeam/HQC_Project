namespace Poker.UI
{
    using GameObjects.Cards;
    using GameObjects.Player;

    public interface IRenderer
    {
        void Clear();

        void Draw(params IPlayer[] gameObjects);

        void Draw(params PepsterCard[] gameObjects);

        void ShowGamerTurnTimer();
    }
}
