namespace Poker.Engines
{
    using System;
    using System.Windows.Forms;
    using GameObjects.Player;

    public class Renderer
    {
        private GameForm form;

        public Renderer(GameForm form)
        {
            this.form = form;
        }

        public void SetLabelStatus(IPlayer player)
        {
            Label label = this.GetLabelControl(player.Name);
            label.Text = player.Status;
        }

        private Label GetLabelControl(string name)
        {
            switch (name)
            {
                case "player":
                    return this.form.labelPlayerStatus;
                case "Bot 1":
                    return this.form.labelBot1Status;
                case "Bot 2":
                    return this.form.labelBot2Status;
                case "Bot 3":
                    return this.form.labelBot3Status;
                case "Bot 4":
                    return this.form.labelBot4Status;
                case "Bot 5":
                    return this.form.labelBot5Status;
                default: throw new ArgumentOutOfRangeException("no such player");
            }
        }
    }
}
