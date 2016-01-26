namespace Poker.UI
{
    using GameObjects;
    using GameObjects.Cards;
    using GameObjects.Player;

    public interface IRenderer
    {
        void Clear();

        void Draw(params IPlayer[] gameObjects);

        void Draw(params PepsterCard[] gameObjects);
        
        void Draw(Table table);

        void ShowMessage(string msg);

        void ShowGamerTurnTimer();
        
        // да се провери необходимо ли е 
        void ShowOrHidePlayersButtons(Gamer player);

        void EnablingFormMinimizationAndMaximization();
        
        // да се провери необходимо ли е 
        void HideGamerTurnTimer();

        // да се провери необходимо ли е 
        void ShowAllCards();

        // да се провери необходимо ли е 
        void SetAllLabelStatus(IPlayer[] players);
    }
}
