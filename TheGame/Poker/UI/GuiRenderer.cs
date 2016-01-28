namespace Poker.UI
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using GameObjects;
    using GameObjects.Cards;
    using GameObjects.Player;

    public class GuiRenderer : IRenderer
    {
        private GameForm form;

        public GuiRenderer(GameForm form)
        {
            this.form = form;
        }

        public void Draw(params IPlayer[] gameObjects)
        {
            foreach (var player in gameObjects)
            {
                this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
                this.form.PlayersTextBoxsChips[player.Id].Text = player.Chips.ToString();
                if (player is Bot)
                {
                    this.ShowOrHideCardsControls(player);
                }
                else
                {
                    this.ShowOrHidePlayersButtons((Gamer)player);
                    // this.SetButonsValues((Gamer)player);
                }
            }
        }

        public void Draw(params PepsterCard[] gameObjects)
        {
            foreach (var card in gameObjects)
            {
                if (card.IsVisible)
                {
                    this.form.DealtCardHolder[card.DealtPosition].Image = this.form.DealtCardImages[card.DealtPosition];
                }
                //this.form.DealtCardHolder[card.DealtPosition].Image = this.form.DealtCardImages[card.DealtPosition];
            }
        }

        public void Draw(Table table)
        {
            this.form.textBoxPot.Text = table.Pot.ToString();
            this.form.textBoxRaise.Text = table.LastRaise.ToString();
            this.form.buttonCall.Text = table.PosibleCall;
            this.form.buttonRaise.Text = table.PosibleRaise;
            // other properties and controls to be added;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowMessage(string message, string message1)
        {
            var dialogResult = MessageBox.Show(message, message1, MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Restart();
            }
            else if (dialogResult == DialogResult.No)
            {
                Application.Exit();
            }
        }

        public void Clear()
        {
            //idwat ot reseta na gameengina
            foreach (var panel in this.form.PlayersPanels)
            {
                panel.Visible = false;
            }

            this.form.FormHeight = 0;
            this.form.FormWidth = 0;
            this.form.textBoxPot.Text = "0";

            foreach (var cardHolder in this.form.DealtCardHolder)
            {
                cardHolder.Image = null;
                cardHolder.Invalidate();
                cardHolder.Visible = false;
            }

            this.form.MaximizeBox = false;
            this.form.MinimizeBox = false;

            this.form.buttonCall.Enabled = false;
            this.form.buttonRaise.Enabled = false;
            this.form.buttonFold.Enabled = false;
            this.form.buttonCheck.Enabled = false;

            this.form.textBoxPot.Enabled = false;
            this.form.textBoxPlayerChips.Enabled = false;
            this.form.textBoxBot1Chips.Enabled = false;
            this.form.textBoxBot2Chips.Enabled = false;
            this.form.textBoxBot3Chips.Enabled = false;
            this.form.textBoxBot4Chips.Enabled = false;
            this.form.textBoxBot5Chips.Enabled = false;

            this.form.textBoxBigBlind.Visible = true;
            this.form.textBoxSmallBlind.Visible = true;
            this.form.buttonBigBlind.Visible = true;
            this.form.buttonSmallBlind.Visible = true;
            this.form.textBoxBigBlind.Visible = true;
            this.form.textBoxSmallBlind.Visible = true;
            this.form.buttonBigBlind.Visible = true;
            this.form.buttonSmallBlind.Visible = true;
            this.form.textBoxBigBlind.Visible = false;
            this.form.textBoxSmallBlind.Visible = false;
            this.form.buttonBigBlind.Visible = false;
            this.form.buttonSmallBlind.Visible = false;

            this.form.textBoxRaise.Text = (2 * GlobalConstants.InitialBigBlind).ToString();
        }

        public void StartGamerTurnTimer()
        {
            // this.form.progressBarTimer.Visible = true;
            // this.form.progressBarTimer.Value = 1000;

            // TODO: ewentualno da se mahne stwaneto ako nikyde ne se promenq
            //this.form.TimeForPlayerTurn = 60; - ima go kato konstanta wyw formata
            this.form.ProgresiveBarTimer.Start();
        }

        public void StopGamerTurnTimer()
        {
            this.form.progressBarTimer.Visible = false;
            this.form.ProgresiveBarTimer.Stop();
        }

        public void EnablingFormMinimizationAndMaximization()
        {
            this.form.MaximizeBox = true;
            this.form.MinimizeBox = true;
        }

        public void ShowOrHidePlayersButtons(Gamer player)
        {
            this.form.textBoxRaise.Text = player.ValueToRaise.ToString();
            this.form.buttonCall.Enabled = player.CanCall;
            this.form.buttonRaise.Enabled = player.CanRaise;
            this.form.buttonFold.Enabled = player.CanFold;
            this.form.buttonCheck.Enabled = player.CanCheck;
        }

        public void ShowAllCards()
        {
            for (int j = 0; j <= 16; j++)
            {
                if (this.form.DealtCardHolder[j].Visible)
                {
                    this.form.DealtCardHolder[j].Image = this.form.DealtCardImages[j];
                }
            }
        }

        public void SetAllLabelStatus(IPlayer[] players)
        {
            this.form.labelPlayerStatus.Text = players[0].Status;
            this.form.labelBot1Status.Text = players[1].Status;
            this.form.labelBot2Status.Text = players[2].Status;
            this.form.labelBot3Status.Text = players[3].Status;
            this.form.labelBot4Status.Text = players[4].Status;
            this.form.labelBot5Status.Text = players[5].Status;
        }

        public void SetLabelStatus(IPlayer player, string labelText)
        {
            switch (player.Name)
            {
                case "Player":
                    this.form.labelPlayerStatus.Text = labelText;
                    break;
                case "Bot 1":
                    this.form.labelBot1Status.Text = labelText; 
                    break;
                case "Bot 2":
                    this.form.labelBot2Status.Text = labelText;
                    break;
                case "Bot 3":
                    this.form.labelBot3Status.Text = labelText;
                    break;
                case "Bot 4":
                    this.form.labelBot4Status.Text = labelText; 
                    break;
                case "Bot 5":
                    this.form.labelBot5Status.Text = labelText; 
                    break;
                default: throw new ArgumentException("Invalid player name");
            }
        }

        public void SetPanelStatus(Panel panel, bool isVisible)
        {
            panel.Visible = isVisible;
        }

        public void SetTextBoxPlayerChips(IPlayer player)
        {
            switch (player.Name)
            {
                case "Player":
                    this.form.textBoxPlayerChips.Text = player.Chips.ToString();
                    break;
                case "Bot 1":
                    this.form.textBoxBot1Chips.Text = player.Chips.ToString(); 
                    break;
                case "Bot 2":
                    this.form.textBoxBot2Chips.Text = player.Chips.ToString(); 
                    break;
                case "Bot 3":
                    this.form.textBoxBot3Chips.Text = player.Chips.ToString(); 
                    break;
                case "Bot 4":
                    this.form.textBoxBot4Chips.Text = player.Chips.ToString();
                    break;
                case "Bot 5":
                    this.form.textBoxBot5Chips.Text = player.Chips.ToString();
                    break;
                default: throw new ArgumentException("Invalid player chips");
            }
        }

        public void GetCardsImages(PepsterCard[] pepsterDealtCards)
        {
            for (int i = 0; i < pepsterDealtCards.Length; i++)
            {
                this.form.DealtCardImages[i] = Image.FromFile(pepsterDealtCards[i].CardFrontImageUri);
            }
        }

        private void SetButonsValues(Gamer player)
        {
            //TODO : implement;
        }

        private void ShowOrHideCardsControls(IPlayer player)
        {
            if (player.GameEnded)
            {
                this.HideCardsControls(player);
            }
            else
            {
                this.ShowCardsControls(player);
            }
        }

        private void ShowCardsControls(IPlayer player)
        {
            if (this.form.DealtCardHolder[player.Id + 2] == null)
            {
                return;
            }

            this.form.DealtCardHolder[player.Id + 2].Visible = true;
            this.form.DealtCardHolder[player.Id + 1].Visible = true;
        }

        private void HideCardsControls(IPlayer player)
        {
            this.form.DealtCardHolder[player.Id + 1].Visible = false;
            this.form.DealtCardHolder[player.Id + 2].Visible = false;
        }
    }
}