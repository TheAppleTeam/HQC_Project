﻿namespace Poker
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Engines;
    using Exception;
    using UI;

    public partial class GameForm : Form
    {
        public readonly int CardWidth = 80;
        public readonly int CardHeight = 130;
        public readonly int CardPanelWidth = 180;
        public readonly int CardPanelHeight = 150;
        public readonly int TimeForPlayerTurn = 60;

        public int height;
        public int width;


        private readonly Label[] playersLabelsStatus = new Label[6];
        private readonly TextBox[] playersTextBoxsChips = new TextBox[6];
        private readonly GameEngine gameEngine;
        private readonly Panel[] playersPanels = new Panel[6];
        private readonly Image cardBackImage = Image.FromFile(GlobalConstants.CardBackImageUri);
        private readonly PictureBox[] dealtCardHolder = new PictureBox[GlobalConstants.DealtCardsCount];
        private readonly Image[] dealtCardImages = new Image[GlobalConstants.DealtCardsCount];
        private readonly Timer updateControlsTimer = new Timer();

        public GameForm()
        {
            this.width = this.Width;
            this.height = this.Height;

            this.InitializeComponent();
            this.ProgresiveBarTimer = new Timer();
            this.ProgresiveBarTimer.Start();
            this.ProgresiveBarTimer.Interval = 1 * 1 * 60 * 1000;
            this.ProgresiveBarTimer.Tick += this.ProgresiveBarTimerTick;

            this.updateControlsTimer.Start();
            this.updateControlsTimer.Interval = 1 * 1 * 2000;
            this.updateControlsTimer.Tick += this.UpdateControlsTick;

            this.InitializeControlsArrays();

            IRenderer renderer = new GuiRenderer(this);

            IInputHandlerer inputHandlerer = new GuiInputHandlerer(this);

            this.gameEngine = new GameEngine(renderer, inputHandlerer);

            try
            {
                this.gameEngine.GameInit();
            }
            catch (InputValueException ex)
            {
                renderer.ShowMessage(ex.Message);
            }
           
            #region ToBEDeleted
            //this.gameEngine.pokerCall = InitialBigBlind;
            //this.MaximizeBox = false;
            //this.MinimizeBox = false;
            //this.updates.Start();


            //this.width = this.Width;
            //this.height = this.Height;
            //this.SetupPokerTable();
            //this.textBoxPot.Enabled = false;
            //this.textBoxPlayerChips.Enabled = false;
            //this.textBoxBot1Chips.Enabled = false;
            //this.textBoxBot2Chips.Enabled = false;
            //this.textBoxBot3Chips.Enabled = false;
            //this.textBoxBot4Chips.Enabled = false;
            //this.textBoxBot5Chips.Enabled = false;

        //    this.gameEngine.SetAllTextBoxChips();
            
            //this.textBoxPlayerChips.Text = "Chips : " + this.players[0].Chips.ToString();
            //this.textBoxBot1Chips.Text = "Chips : " + this.players[1].Chips.ToString();
            //this.textBoxBot2Chips.Text = "Chips : " + this.players[2].Chips.ToString();
            //this.textBoxBot3Chips.Text = "Chips : " + this.players[3].Chips.ToString();
            //this.textBoxBot4Chips.Text = "Chips : " + this.players[4].Chips.ToString();
            //this.textBoxBot5Chips.Text = "Chips : " + this.players[5].Chips.ToString();

            //this.timer.Interval = 1 * 1 * 1000;
            //this.timer.Tick += this.timer_Tick;
            //this.updates.Interval = 1 * 1 * 100;
            //this.updates.Tick += this.Update_Tick;
            //this.textBoxBigBlind.Visible = true;
            //this.textBoxSmallBlind.Visible = true;
            //this.buttonBigBlind.Visible = true;
            //this.buttonSmallBlind.Visible = true;
            //this.textBoxBigBlind.Visible = true;
            //this.textBoxSmallBlind.Visible = true;
            //this.buttonBigBlind.Visible = true;
            //this.buttonSmallBlind.Visible = true;
            //this.textBoxBigBlind.Visible = false;
            //this.textBoxSmallBlind.Visible = false;
            //this.buttonBigBlind.Visible = false;
            //this.buttonSmallBlind.Visible = false;
            //this.textBoxRaise.Text = (this.gameEngine.bb * 2).ToString();
            #endregion
        }

        public Timer ProgresiveBarTimer { get; set; }

        public void UpdateControlsTick(object sender, object e)
        {
            this.gameEngine.UpdateControls();
        }

        public Label[] PlayersLabelsStatus
        {
            get { return this.playersLabelsStatus; }
        }

        public TextBox[] PlayersTextBoxsChips
        {
            get { return this.playersTextBoxsChips; }
        }

        public Panel[] PlayersPanels 
        {
            get { return this.playersPanels; }
        }

        public Image[] DealtCardImages 
        {
            get { return this.dealtCardImages; }
        }

        public PictureBox[] DealtCardHolder
        {
            get { return this.dealtCardHolder; }
        }

        private void InitializeControlsArrays()
        {
            this.playersLabelsStatus[0] = this.labelPlayerStatus;
            this.playersLabelsStatus[1] = this.labelBot1Status;
            this.playersLabelsStatus[2] = this.labelBot2Status;
            this.playersLabelsStatus[3] = this.labelBot3Status;
            this.playersLabelsStatus[4] = this.labelBot4Status;
            this.playersLabelsStatus[5] = this.labelBot5Status;

            this.playersTextBoxsChips[0] = this.textBoxPlayerChips;
            this.playersTextBoxsChips[1] = this.textBoxBot1Chips;
            this.playersTextBoxsChips[2] = this.textBoxBot2Chips;
            this.playersTextBoxsChips[3] = this.textBoxBot3Chips;
            this.playersTextBoxsChips[4] = this.textBoxBot4Chips;
            this.playersTextBoxsChips[5] = this.textBoxBot5Chips;

            for (int playersCount = 0; playersCount < GlobalConstants.PlayersCount; playersCount++)
            {
                this.playersPanels[playersCount] = new Panel()
                {
                    BackColor = Color.DarkBlue,
                    Height = this.CardPanelHeight,
                    Width = this.CardPanelWidth,
                    Visible = false
                };
                this.Controls.Add(this.playersPanels[playersCount]);
            }

            for (int cardsCount = 0; cardsCount < GlobalConstants.DealtCardsCount; cardsCount++)
            {
                this.DealtCardHolder[cardsCount] = new PictureBox()
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Height = this.CardHeight,
                    Width = this.CardWidth,
                    Name = "pictureBox" + cardsCount,
                    // Tag = card.Id, t.e. dyrvi imeto na fajla. move bi ne se polzwa i ne e neobhodimo da se setwa TAg
                    //   Image = this.GetCardImage(card),
                    Image = cardBackImage,
                    Anchor = AnchorStyles.Bottom,
                    Location = this.CalculateCardControlLocation(cardsCount)
                };
                this.Controls.Add(this.DealtCardHolder[cardsCount]);
                this.SetPanelsLocation(cardsCount);
            }
        }
        private void SetPanelsLocation(int dealtPosition)
        {
            int left = this.DealtCardHolder[dealtPosition].Left - 10;
            int top = this.DealtCardHolder[dealtPosition].Top - 10;
            switch (dealtPosition)
            {
                case 0:
                    this.PlayersPanels[0].Location = new Point(left, top);
                    break;
                case 2:
                    this.PlayersPanels[1].Location = new Point(left, top);
                    break;
                case 4:
                    this.PlayersPanels[2].Location = new Point(left, top);
                    break;
                case 6:
                    this.PlayersPanels[3].Location = new Point(left, top);
                    break;
                case 8:
                    this.PlayersPanels[4].Location = new Point(left, top);
                    break;
                case 10:
                    this.PlayersPanels[5].Location = new Point(left, top);
                    break;
            }
        }
        private Point CalculateCardControlLocation(int cardsCount)
        {
            int horisontalAnchor = this.GetCardHorisontalAnchor(cardsCount);
            int verticalAnchor = this.GetCardVerticalAnchor(cardsCount);
            if (cardsCount % 2 == 1 && cardsCount < 12)
            {
                horisontalAnchor += this.CardWidth;
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
                default: anchor = 410 + ((dealtPosition % 12) * (this.CardWidth + 10));
                    break;
            }

            return anchor;
        }

        #region UI

        //wika se w GameFormDesigner -> InitializeComponent
        public void Layout_Change(object sender, LayoutEventArgs e)
        {
            this.width = this.Width;
            this.height = this.Height;
        }


        private async void ProgresiveBarTimerTick(object sender, object e)
        {
            this.gameEngine.GammerMoveTimeExpired();
            //if (this.progressBarTimer.Value <= 0)
            //{
            //   await this.gameEngine.Turns();
            //}

            //if (this.timeForPlayerTurn > 0)
            //{
            //    this.timeForPlayerTurn--;
            //    this.progressBarTimer.Value = (this.timeForPlayerTurn / 6) * 100;
            //}
        }

        //private void GameLoopTimerTick(object sender, object e)
        //{
        //    this.gameEngine.UpdateOnTick();
        //}

        private async void ButtonFold_Click(object sender, EventArgs e)
        {
            this.gameEngine.Players[0].Status = "Fold";
            this.ProgresiveBarTimer.Stop();
            this.gameEngine.GammerPlayesFold();
            await this.gameEngine.Turns();
        }

        private async void ButtonCheck_Click(object sender, EventArgs e)
        {
            this.gameEngine.Players[0].Status = "Check";
            this.ProgresiveBarTimer.Stop();
            this.gameEngine.GammerPlayesCheck();
            await this.gameEngine.Turns();
        }

        private async void ButtonCall_Click(object sender, EventArgs e)
        {
            this.gameEngine.Players[0].Status = "Call";
            this.ProgresiveBarTimer.Stop();
            this.gameEngine.GammerPlayesCall();
            await this.gameEngine.Turns();
        }

        private async void ButtonRaise_Click(object sender, EventArgs e)
        {
            this.gameEngine.Players[0].Status = "Raise";
            this.ProgresiveBarTimer.Stop();
            this.gameEngine.GammerPlayesRaise();
            await this.gameEngine.Turns();
        }

        private void ButtonAddChips_Click(object sender, EventArgs e)
        {
            this.gameEngine.GammerAddsChips();
        }

        private void ButtonChooseBlind_Click(object sender, EventArgs e)
        {
            this.textBoxBigBlind.Text = this.gameEngine.Table.BigBlind.ToString();
            this.textBoxSmallBlind.Text = this.gameEngine.Table.SmallBlind.ToString();

            if (this.textBoxBigBlind.Visible == false)
            {
                this.textBoxBigBlind.Visible = true;
                this.textBoxSmallBlind.Visible = true;
                this.buttonBigBlind.Visible = true;
                this.buttonSmallBlind.Visible = true;
            }
            else
            {
                this.textBoxBigBlind.Visible = false;
                this.textBoxSmallBlind.Visible = false;
                this.buttonBigBlind.Visible = false;
                this.buttonSmallBlind.Visible = false;
            }
        }

        private void ButtonSmallBlind_Click(object sender, EventArgs e)
        {
            this.gameEngine.SetSmallBlind();
        }
        
        private void ButtonBigBlind_Click(object sender, EventArgs e)
        {
            this.gameEngine.SetBigBlind();
        }
        
        #endregion
    }
}