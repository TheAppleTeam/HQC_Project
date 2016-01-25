namespace Poker.UI
{
    using System.Drawing;
    using System.Windows.Forms;
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
                    this.SetButonsValues((Gamer)player)
                    ;
                }

            }
        }

        private void SetButonsValues(Gamer player)
        {

        }

        public void ShowOrHidePlayersButtons(Gamer player)
        {
            this.form.buttonCall.Enabled = player.CanCall;
            this.form.buttonRaise.Enabled = player.CanRaise;
            this.form.buttonFold.Enabled = player.CanFold;
            this.form.buttonCheck.Enabled = player.CanCheck;
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
            this.form.DealtCardHolder[player.Id + 1].Visible = false;
            this.form.DealtCardHolder[player.Id + 2].Visible = false;
        }

        private void HideCardsControls(IPlayer player)
        {
            this.form.DealtCardHolder[player.Id + 1].Visible = false;
            this.form.DealtCardHolder[player.Id + 2].Visible = false;
        }

        public void Draw(params PepsterCard[] gameObjects)
        {
            foreach (var card in gameObjects)
            {
                /*излишно е защото се създава масив на имиджи които по същество са пропърти на пикчър бокса и си се сетват там.
                 this.form.DealtCardImages[card.DealtPosition] = Image.FromFile(card.CardFrontImageUri);
                 */

                this.form.DealtCardHolder[card.DealtPosition] = new PictureBox()
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Height = this.form.CardHeight,
                    Width = this.form.CardWidth,
                    Name = "pictureBox" + card.DealtPosition,
                    Tag = card.Id,
                    Image = this.GetCardImage(card),
                    Anchor = AnchorStyles.Bottom,
                    Location = this.CalculateCardControlLocation(card)
                };
                this.form.Controls.Add(this.form.DealtCardHolder[card.DealtPosition]);
                this.SetPanelsLocation(card.DealtPosition);
            }

            // this.deckImages[cardIndex] = Image.FromFile(this.imageURIArray[cardIndex]);
            // this.cardHolder[cardIndex] = new PictureBox();
            // this.cardHolder[cardIndex].SizeMode = PictureBoxSizeMode.StretchImage;
            // this.cardHolder[cardIndex].Height = CardHeight;
            // this.cardHolder[cardIndex].Width = CardWidth;
            // this.cardHolder[cardIndex].Name = "pb" + cardIndex.ToString();
            // this.form.Controls.Add(this.cardHolder[cardIndex]);
        }

        private void SetPanelsLocation(int dealtPosition)
        {
            int left = this.form.DealtCardHolder[dealtPosition].Left - 10;
            int top = this.form.DealtCardHolder[dealtPosition].Top - 10;
            switch (dealtPosition)
            {
                case 0:
                    this.form.PlayersPanels[0].Location = new Point(left, top);
                    break;
                case 2:
                    this.form.PlayersPanels[1].Location = new Point(left, top);
                    break;
                case 4:
                    this.form.PlayersPanels[2].Location = new Point(left, top);
                    break;
                case 6:
                    this.form.PlayersPanels[3].Location = new Point(left, top);
                    break;
                case 8:
                    this.form.PlayersPanels[4].Location = new Point(left, top);
                    break;
                case 10:
                    this.form.PlayersPanels[5].Location = new Point(left, top);
                    break;
            }
        }

        private Point CalculateCardControlLocation(PepsterCard card)
        {
            int horisontalAnchor = this.GetCardHorisontalAnchor(card.DealtPosition);
            int verticalAnchor = this.GetCardVerticalAnchor(card.DealtPosition);
            if (card.DealtPosition % 2 == 1 && card.DealtPosition < 12)
            {
                horisontalAnchor += this.form.CardWidth;
            }
            var point = new Point(horisontalAnchor, verticalAnchor);
            return point;
        }

        private int GetCardVerticalAnchor(int dealtPosition)
        {
            int anchor = 0;
            switch (dealtPosition)
            {
                case 0:
                case 1: anchor = 480;
                    break;
                case 2:
                case 3: anchor = 420;
                    break;
                case 4:
                case 5: anchor = 65;
                    break;
                case 6:
                case 7: anchor = 25;
                    break;
                case 8:
                case 9: anchor = 65;
                    break;
                case 10:
                case 11: anchor = 420;
                    break;

                default: anchor = 265;
                    break;
            }

            return anchor;
        }

        private int GetCardHorisontalAnchor(int dealtPosition)
        {
            int anchor = 0;
            switch (dealtPosition)
            {
                case 0:
                case 1:
                    anchor = 580;
                    break;
                case 2:
                case 3:
                    anchor = 15;
                    break;
                case 4:
                case 5:
                    anchor = 75;
                    break;
                case 6:
                case 7:
                    anchor = 590;
                    break;
                case 8:
                case 9:
                    anchor = 1115;
                    break;
                case 10:
                case 11:
                    anchor = 1160;
                    break;
                default: anchor = 410 + ((dealtPosition % 12) * (this.form.CardWidth + 10));
                    break;
            }

            return anchor;
        }

        private Image GetCardImage(PepsterCard card)
        {
            /*кода е закоментиран за да могат всички карти да са видими. 
            if (card.DealtPosition < 2)
            {
                return Image.FromFile(card.CardFrontImageUri);
            }
            else
            {
                return Image.FromFile(card.CardBackImageUri);
            }
            */
            return Image.FromFile(card.CardFrontImageUri);
        }

        public void Clear()
        {
            //idwat ot reseta na gameengina
            foreach (var panel in this.form.PlayersPanels)
            {
                panel.Visible = false;
            }
            this.form.height = 0;
            this.form.width = 0;
            this.form.textBoxPot.Text = "0";
            foreach (var cardHolder in this.form.DealtCardHolder)
            {
                cardHolder.Image = null;
                cardHolder.Invalidate();
                cardHolder.Visible = false;
            }

            //idwat ot gameEngina - InIt method;
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
            this.form.textBoxRaise.Text = (this.bb * 2).ToString();
        }

        public void ShowGamerTurnTimer()
        {
            this.form.progressBarTimer.Visible = true;
            this.form.progressBarTimer.Value = 1000;
            ////MessageBox.Show("Player's Turn");
            // TODO: ewentualno da se mahne stwaneto ako nikyde ne se promenq
            this.form.timeForPlayerTurn = 60;

            this.form.progresiveBarTimer.Start();
            //this.form.buttonRaise.Enabled = true;
            //this.form.buttonCall.Enabled = true;
            //this.form.buttonFold.Enabled = true;
        }

        public void HideGamerTurnTimer()
        {
            this.form.progressBarTimer.Visible = false;
            this.form.progresiveBarTimer.Stop();
        }

        public void EnablingFormMinimizationAndMaximization()
        {
            this.form.MaximizeBox = true;
            this.form.MinimizeBox = true;
        }

        public void ShowAllCards()
        {

            for (int j = 0; j <= 16; j++)
            {
                //await Task.Delay(5);
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

        //     this.FillInPlayerPanel(cardIndex, this.form.PlayersPanels[0]);
    }
}


