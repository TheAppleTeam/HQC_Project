namespace UnitTestsPoker
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Poker;
    using Poker.Engines;
    using Poker.GameObjects.Player;
    using Poker.UI;

    [TestClass]
    public class CheckForHand
    {
        private IPlayer player;
        private GameEngine engine;

        [AssemblyInitialize]
        private void SetupGamer()
        {
            this.player = new Gamer();
            var form = new GameForm();
            IRenderer renderer = new GuiRenderer(form);
            IInputHandlerer handler = new GuiInputHandlerer();
            this.engine = new GameEngine(renderer, handler);
        }

        [TestMethod]
        public void CheckForPair_ReturnsPokerhandMultiplier()
        {

        }
    }
}
