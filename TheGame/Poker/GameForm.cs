namespace Poker
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using GameObjects.Cards;

    public partial class GameForm : Form
    {
        #region Constants
        private const int InitialBigBlind = 500;
        private const int InitialSmallBlind = 250;
        private const int BotCount = 5;
        private const int InitialChips = 10000;
        private const int DealtCards = 17;
        private const int CardWidth = 80;
        private const int CardHeight = 130;
        private const int DealingCardsDelay = 200;
        private const int CardPanelWidth = 180;
        private const int CardPanelHeight = 150;
        #endregion

        #region Variables
        private int pokerCall = InitialBigBlind;
        private int foldedPlayers = BotCount;
        private int playerChips = InitialChips;
        private int bot1Chips = InitialChips;
        private int bot2Chips = InitialChips;
        private int bot3Chips = InitialChips;
        private int bot4Chips = InitialChips;
        private int bot5Chips = InitialChips;

        private readonly Panel playerCardsPanel = new Panel();
        private readonly Panel bot1CardsPanel = new Panel();
        private readonly Panel bot2CardsPanel = new Panel();
        private readonly Panel bot3CardsPanel = new Panel();
        private readonly Panel bot4CardsPanel = new Panel();
        private readonly Panel bot5CardsPanel = new Panel();

        private double type;
        private double rounds;
        private double bot1CardPower;
        private double bot2CardPower;
        private double bot3CardPower;
        private double bot4CardPower;
        private double bot5CardPower;
        private double playerCardPower;
        private double playerHandMultiplier = -1;
        private double bot1HandMultiplier = -1;
        private double bot2HandMultiplier = -1;
        private double bot3HandMultiplier = -1;
        private double bot4HandMultiplier = -1;
        private double bot5HandMultiplier = -1;
        private double raise;
        private bool bot1Turn = false;
        private bool bot2Turn = false;
        private bool bot3Turn = false;
        private bool bot4Turn = false;
        private bool bot5Turn = false;

        // is set TRUE when bot's chips (bot{num}Chips) goes <=0 
        private bool bot1GameEnded = false;
        private bool bot2GameEnded = false;
        private bool bot3GameEnded = false;
        private bool bot4GameEnded = false;
        private bool bot5GameEnded = false;

        private bool playerFolded;
        private bool bot1Folded;
        private bool bot2Folded;
        private bool bot3Folded;
        private bool bot4Folded;
        private bool bot5Folded;

        private bool intsadded;
        private bool changed;

        private int playerCall = 0;
        private int bot1Call = 0;
        private int bot2Call = 0;
        private int bot3Call = 0;
        private int bot4Call = 0;
        private int bot5Call = 0;
        private int playerRaise = 0;
        private int bot1Raise = 0;
        private int bot2Raise = 0;
        private int bot3Raise = 0;
        private int bot4Raise = 0;
        private int bot5Raise = 0;

        private int height;
        private int width;

        private int winnersCount = 0;
        //used in CheckRaise method :   if (rounds == Flop); in Finish method is seted again on 1;
        private int Flop = 1;
        //used in CheckRaise method :  if (rounds == Turn); in Finish method is seted again on 2;
        private int Turn = 2;
        private int River = 3;
        // used in CheckRaise method :  (rounds == River); in Finish method is seted again on 3;
        private int End = 4;
        // used in CheckRaise method :   if (rounds == End && maxPlayersLeftCount == 6);
        //used in AllIn method->  #region FiveOrLessLeft: if (abc < 6 && abc > 1 && rounds >= End) 
        //in Finish method is seted again on 4;
        private int maxPlayersLeftCount = 6;
        //used in Turns method -> region Rotating : in every positive check is game ending  maxPlayersLeftCount--;
        // used in CheckRaise method :   if (rounds == End && maxPlayersLeftCount == 6); and  if (turnCount >= maxPlayersLeftCount - 1 || !changed && turnCount == maxPlayersLeftCount);
        //used in AllIn method : if (ints.ToArray().Length == maxPlayersLeftCount)
        //in Finish method is seted again on 6;
        private int lastBotPlayed = 123;
        int raisedTurn = 1;
        List<bool?> bools = new List<bool?>();

        List<Type> winningCards = new List<Type>();

        List<string> CheckWinners = new List<string>();
        List<int> ints = new List<int>();
        bool playerGameEnded = false, Pturn = true, restart = false, raising = false;
        Poker.Type winningCard;



        string[] ImageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
        /*string[] ImgLocation ={
                   "Assets\\Cards\\33.png","Assets\\Cards\\22.png",
                    "Assets\\Cards\\29.png","Assets\\Cards\\21.png",
                    "Assets\\Cards\\36.png","Assets\\Cards\\17.png",
                    "Assets\\Cards\\40.png","Assets\\Cards\\16.png",
                    "Assets\\Cards\\5.png","Assets\\Cards\\47.png",
                    "Assets\\Cards\\37.png","Assets\\Cards\\13.png",
                    
                    "Assets\\Cards\\12.png",
                    "Assets\\Cards\\8.png","Assets\\Cards\\18.png",
                    "Assets\\Cards\\15.png","Assets\\Cards\\27.png"};*/
        int[] dealtCardsNumbers = new int[17];
        Image[] DeckImages = new Image[52];
        PictureBox[] cardHolder = new PictureBox[52];
        Timer timer = new Timer();
        Timer Updates = new Timer();

        private int t = 60;
        //int i;
        int bb = InitialBigBlind, sb = InitialSmallBlind, up = 10000000, turnCount = 0;
        #endregion

        public GameForm()
        {
            //bools.Add(PFturn); bools.Add(B1Fturn); bools.Add(bot2GameEnded); bools.Add(bot3GameEnded); bools.Add(bot4GameEnded); bools.Add(bot5GameEnded);
            this.pokerCall = InitialBigBlind;
            MaximizeBox = false;
            MinimizeBox = false;
            Updates.Start();
            InitializeComponent();
            width = this.Width;
            height = this.Height;
            this.SetupPokerTable();
            textBoxPot.Enabled = false;
            textBoxPlayerChips.Enabled = false;
            textBoxBot1Chips.Enabled = false;
            textBoxBot2Chips.Enabled = false;
            textBoxBot3Chips.Enabled = false;
            textBoxBot4Chips.Enabled = false;
            textBoxBot5Chips.Enabled = false;
            textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
            textBoxBot1Chips.Text = "Chips : " + bot1Chips.ToString();
            textBoxBot2Chips.Text = "Chips : " + bot2Chips.ToString();
            textBoxBot3Chips.Text = "Chips : " + bot3Chips.ToString();
            textBoxBot4Chips.Text = "Chips : " + bot4Chips.ToString();
            textBoxBot5Chips.Text = "Chips : " + bot5Chips.ToString();
            timer.Interval = (1 * 1 * 1000);
            timer.Tick += timer_Tick;
            Updates.Interval = (1 * 1 * 100);
            Updates.Tick += Update_Tick;
            textBoxBigBlind.Visible = true;
            textBoxSmallBlind.Visible = true;
            buttonBigBlind.Visible = true;
            buttonSmallBlind.Visible = true;
            textBoxBigBlind.Visible = true;
            textBoxSmallBlind.Visible = true;
            buttonBigBlind.Visible = true;
            buttonSmallBlind.Visible = true;
            textBoxBigBlind.Visible = false;
            textBoxSmallBlind.Visible = false;
            buttonBigBlind.Visible = false;
            buttonSmallBlind.Visible = false;
            textBoxRaise.Text = (bb * 2).ToString();
        }

        //Stani

        #region Refactored Shuffle to SetupPokerTable
        /// <summary>
        /// Setups a poker table with all the players holders, cards on the table and buttons
        /// </summary>
        /// <returns></returns>
        async Task SetupPokerTable()
        {
            this.bools.Add(this.playerGameEnded);
            this.bools.Add(this.bot1GameEnded);
            this.bools.Add(this.bot2GameEnded);
            this.bools.Add(this.bot3GameEnded);
            this.bools.Add(this.bot4GameEnded);
            this.bools.Add(this.bot5GameEnded);

            this.buttonCall.Enabled = false;
            this.buttonRaise.Enabled = false;
            this.buttonFold.Enabled = false;
            this.buttonCheck.Enabled = false;

            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Bitmap backImage = new Bitmap("Assets\\Back\\Back.png");

            this.ShuffleDeck();

            for (int cardIndex = 0; cardIndex < DealtCards; cardIndex++)
            {
                this.DeckImages[cardIndex] = Image.FromFile(this.ImageURIArray[cardIndex]);

                this.FillInDealtCardsNumbers(cardIndex);

                this.SetupCardHolder(cardIndex);

                this.DealCards(cardIndex);

                this.CheckToEnableBots(cardIndex);

                await Task.Delay(DealingCardsDelay);
            }

            this.EnablingFormMinimizationAndMaximization();

            this.timer.Start();

            this.CheckForGameEnd();

            this.buttonRaise.Enabled = true;
            this.buttonCall.Enabled = true;
            this.buttonFold.Enabled = true;
        }

        /// <summary>
        /// Fill in a dealtCards array with integers coresponding to the cards that are designated for dealing
        /// </summary>
        /// <param name="cardIndex"> Index of the cards that are dealt</param>
        private void FillInDealtCardsNumbers(int cardIndex)
        {
            string removeURI = "Assets\\Cards\\";
            string removeFileExtension = ".png";

            this.ImageURIArray[cardIndex] = this.ImageURIArray[cardIndex].Replace(removeURI, string.Empty);
            this.ImageURIArray[cardIndex] = this.ImageURIArray[cardIndex].Replace(removeFileExtension, string.Empty);

            this.dealtCardsNumbers[cardIndex] = int.Parse(this.ImageURIArray[cardIndex]) - 1;
        }

        /// <summary>
        /// Prepearing CardHolder for every card on the table
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        private void SetupCardHolder(int cardIndex)
        {
            this.cardHolder[cardIndex] = new PictureBox();
            this.cardHolder[cardIndex].SizeMode = PictureBoxSizeMode.StretchImage;
            this.cardHolder[cardIndex].Height = CardHeight;
            this.cardHolder[cardIndex].Width = CardWidth;
            this.Controls.Add(this.cardHolder[cardIndex]);
            this.cardHolder[cardIndex].Name = "pb" + cardIndex.ToString();
        }

        /// <summary>
        /// Shuffles the entire deck ImageURIArray randomly
        /// </summary>
        private void ShuffleDeck()
        {
            Random random = new Random();

            for (int i = this.ImageURIArray.Length; i > 0; i--)
            {
                int randomIndex = random.Next(i);
                var randomImageURI = this.ImageURIArray[randomIndex];
                this.ImageURIArray[randomIndex] = this.ImageURIArray[i - 1];
                this.ImageURIArray[i - 1] = randomImageURI;
            }
        }

        /// <summary>
        /// Check for game completion 
        /// </summary>
        private void CheckForGameEnd()
        {
            if (this.foldedPlayers == 5)
            {
                var dialogResult = MessageBox.Show("Would You Like To Play Again ?", "You Won , Congratulations ! ",
                    MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Application.Restart();
                }
                else if (dialogResult == DialogResult.No)
                {
                    Application.Exit();
                }
            }
            else
            {
                this.foldedPlayers = 5;
            }
        }

        /// <summary>
        /// Checks which bots have money to continue playing
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        private void CheckToEnableBots(int cardIndex)
        {
            if (this.bot1Chips <= 0)
            {
                this.bot1GameEnded = true;
                this.HideBotCards(1);
            }
            else
            {
                this.bot1GameEnded = false;
                this.DisplayBotCards(1, cardIndex);
            }

            if (this.bot2Chips <= 0)
            {
                this.bot2GameEnded = true;
                this.HideBotCards(2);
            }
            else
            {
                this.bot2GameEnded = false;
                this.DisplayBotCards(2, cardIndex);
            }

            if (this.bot3Chips <= 0)
            {
                this.bot3GameEnded = true;
                this.HideBotCards(3);
            }
            else
            {
                this.DisplayBotCards(3, cardIndex);
            }

            if (this.bot4Chips <= 0)
            {
                this.bot4GameEnded = true;
                this.HideBotCards(4);
            }
            else
            {
                this.DisplayBotCards(4, cardIndex);
            }

            if (this.bot5Chips <= 0)
            {
                this.bot5GameEnded = true;
                this.HideBotCards(5);
            }
            else
            {
                this.bot5GameEnded = false;
                this.DisplayBotCards(5, cardIndex);
            }
        }

        /// <summary>
        /// Enabling maximization or minimization of the Form window 
        /// </summary>
        private void EnablingFormMinimizationAndMaximization()
        {
            if (!this.restart)
            {
                this.MaximizeBox = true;
                this.MinimizeBox = true;
            }
        }

        /// <summary>
        /// Displaying the bots cards
        /// </summary>
        /// <param name="botNumber">Number of the bot starting from 1 up to 5</param>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        private void DisplayBotCards(int botNumber, int cardIndex)
        {
            if (cardIndex != botNumber + 2)
            {
                return;
            }

            if (this.cardHolder[botNumber + 2] == null)
            {
                return;
            }

            this.cardHolder[botNumber + 1].Visible = true;
            this.cardHolder[botNumber + 2].Visible = true;
        }

        /// <summary>
        /// Hides bots cards
        /// </summary>
        /// <param name="botNumber">Number of the bot starting from 1 up to 5</param>
        private void HideBotCards(int botNumber)
        {
            this.cardHolder[botNumber + 1].Visible = false;
            this.cardHolder[botNumber + 2].Visible = false;
        }

        /// <summary>
        /// Deals cards to every player on the table and leave five cards on the table
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        private void DealCards(int cardIndex)
        {
            int anchorPointHorizontalPosition;
            int anchorPointVerticalPosition;

            if (cardIndex < 2)
            {
                anchorPointHorizontalPosition = 580;
                anchorPointVerticalPosition = 480;

                this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
            }

            if (cardIndex >= 2 && cardIndex < 4)
            {
                if (this.bot1Chips > 0)
                {
                    anchorPointHorizontalPosition = 15;
                    anchorPointVerticalPosition = 420;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 4 && cardIndex < 6)
            {
                if (this.bot2Chips > 0)
                {
                    anchorPointHorizontalPosition = 75;
                    anchorPointVerticalPosition = 65;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 6 && cardIndex < 8)
            {
                if (this.bot3Chips > 0)
                {
                    anchorPointHorizontalPosition = 590;
                    anchorPointVerticalPosition = 25;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 8 && cardIndex < 10)
            {
                if (this.bot4Chips > 0)
                {
                    anchorPointHorizontalPosition = 1115;
                    anchorPointVerticalPosition = 65;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 10 && cardIndex < 12)
            {
                if (this.bot5Chips > 0)
                {
                    anchorPointHorizontalPosition = 1160;
                    anchorPointVerticalPosition = 420;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 12)
            {
                anchorPointVerticalPosition = 265;
                anchorPointHorizontalPosition = 410 + (cardIndex % 12) * (CardWidth + 10);

                this.FillInCardHolder(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
            }
        }

        /// <summary>
        /// Fill in all the cards controlls
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        /// <param name="anchorPointHorizontalLocation">Horizontal location of the anchor of the image to desplay</param>
        /// <param name="anchorPointVerticalLocation">Horizontal location of the anchor of the image to desplay</param>
        private void FillInCardsControls(int cardIndex, int anchorPointHorizontalLocation, int anchorPointVerticalLocation)
        {
            if (cardIndex >= 2 && cardIndex < 12)
            {
                this.foldedPlayers--;
            }

            this.FillInCardHolder(cardIndex, anchorPointHorizontalLocation, anchorPointVerticalLocation);

            this.FillInPlayerPanel(cardIndex, this.playerCardsPanel);
        }

        /// <summary>
        /// Fill in all of the player card panell properties
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        /// <param name="playerCardsPanel">A panel holding players cards</param>
        private void FillInPlayerPanel(int cardIndex, Panel playerCardsPanel)
        {
            if (cardIndex % 2 == 0)
            {
                playerCardsPanel.Location = new Point(this.cardHolder[cardIndex].Left - 10,
                    this.cardHolder[cardIndex].Top - 10);
                playerCardsPanel.BackColor = Color.DarkBlue;
                playerCardsPanel.Height = CardPanelHeight;
                playerCardsPanel.Width = CardPanelWidth;
                playerCardsPanel.Visible = false;
                this.Controls.Add(playerCardsPanel);
            }
        }

        /// <summary>
        /// Fill in all the properties of the player card holder
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        /// <param name="anchorPointHorizontalLocation"></param>
        /// <param name="anchorPointVerticalLocation"></param>
        private void FillInCardHolder(int cardIndex, int anchorPointHorizontalLocation, int anchorPointVerticalLocation)
        {
            this.cardHolder[cardIndex].Tag = this.dealtCardsNumbers[cardIndex];
            this.cardHolder[cardIndex].Image = this.DeckImages[cardIndex];
            this.cardHolder[cardIndex].Anchor = (AnchorStyles.Bottom);
            if (cardIndex % 2 == 1 && cardIndex < 12)
            {
                anchorPointHorizontalLocation += CardWidth;
            }

            this.cardHolder[cardIndex].Location = new Point(anchorPointHorizontalLocation, anchorPointVerticalLocation);
        }
        #endregion

        //Veronika
        async Task Turns()
        {
            #region Rotating
            if (!this.playerGameEnded)
            {
                if (Pturn)
                {
                    FixCall(labelPlayerStatus, ref this.playerCall, ref playerRaise, 1);
                    //MessageBox.Show("Player's Turn");
                    progressBarTimer.Visible = true;
                    progressBarTimer.Value = 1000;

                    t = 60;
                    up = 10000000;

                    timer.Start();
                    buttonRaise.Enabled = true;
                    buttonCall.Enabled = true;
                    buttonFold.Enabled = true;
                    //buttonRaise.Enabled = true;
                    //buttonRaise.Enabled = true;
                    turnCount++;

                    FixCall(labelPlayerStatus, ref this.playerCall, ref playerRaise, 2);
                }
            }

            if (this.playerGameEnded || !Pturn)
            {
                await AllIn();
                if (this.playerGameEnded && !this.playerFolded)
                {
                    if (buttonCall.Text.Contains("All in") == false || buttonRaise.Text.Contains("All in") == false)
                    {
                        bools.RemoveAt(0);
                        bools.Insert(0, null);
                        maxPlayersLeftCount--;
                        this.playerFolded = true;
                    }
                }
                await CheckRaise(0, 0);
                progressBarTimer.Visible = false;
                buttonRaise.Enabled = false;
                buttonCall.Enabled = false;
                buttonRaise.Enabled = false;
                buttonRaise.Enabled = false;
                buttonFold.Enabled = false;
                timer.Stop();
                this.bot1Turn = true;
                if (!this.bot1GameEnded)
                {
                    if (this.bot1Turn)
                    {
                        FixCall(labelBot1Status, ref this.bot1Call, ref bot1Raise, 1);
                        FixCall(labelBot1Status, ref this.bot1Call, ref bot1Raise, 2);
                        Rules(2, 3, "Bot 1", ref this.bot1HandMultiplier, ref this.bot1CardPower, this.bot1GameEnded);
                        MessageBox.Show("Bot 1's Turn");
                        AI(2, 3, ref bot1Chips, ref this.bot1Turn, ref this.bot1GameEnded, labelBot1Status, 0, this.bot1CardPower, this.bot1HandMultiplier);
                        turnCount++;
                        lastBotPlayed = 1;
                        this.bot1Turn = false;
                        this.bot2Turn = true;
                    }
                }
                if (this.bot1GameEnded && !this.bot1Folded)
                {
                    bools.RemoveAt(1);
                    bools.Insert(1, null);
                    maxPlayersLeftCount--;
                    this.bot1Folded = true;
                }
                if (this.bot1GameEnded || !this.bot1Turn)
                {
                    await CheckRaise(1, 1);
                    this.bot2Turn = true;
                }
                if (!this.bot2GameEnded)
                {
                    if (this.bot2Turn)
                    {
                        FixCall(labelBot2Status, ref this.bot2Call, ref bot2Raise, 1);
                        FixCall(labelBot2Status, ref this.bot2Call, ref bot2Raise, 2);
                        Rules(4, 5, "Bot 2", ref this.bot2HandMultiplier, ref this.bot2CardPower, this.bot2GameEnded);
                        MessageBox.Show("Bot 2's Turn");
                        AI(4, 5, ref bot2Chips, ref this.bot2Turn, ref this.bot2GameEnded, labelBot2Status, 1, this.bot2CardPower, this.bot2HandMultiplier);
                        turnCount++;
                        lastBotPlayed = 2;
                        this.bot2Turn = false;
                        this.bot3Turn = true;
                    }
                }
                if (this.bot2GameEnded && !this.bot2Folded)
                {
                    bools.RemoveAt(2);
                    bools.Insert(2, null);
                    maxPlayersLeftCount--;
                    this.bot2Folded = true;
                }
                if (this.bot2GameEnded || !this.bot2Turn)
                {
                    await CheckRaise(2, 2);
                    this.bot3Turn = true;
                }
                if (!this.bot3GameEnded)
                {
                    if (this.bot3Turn)
                    {
                        FixCall(labelBot3Status, ref bot3Call, ref bot3Raise, 1);
                        FixCall(labelBot3Status, ref bot3Call, ref bot3Raise, 2);
                        Rules(6, 7, "Bot 3", ref this.bot3HandMultiplier, ref this.bot3CardPower, this.bot3GameEnded);
                        MessageBox.Show("Bot 3's Turn");
                        AI(6, 7, ref bot3Chips, ref this.bot3Turn, ref this.bot3GameEnded, labelBot3Status, 2, this.bot3CardPower, this.bot3HandMultiplier);
                        turnCount++;
                        lastBotPlayed = 3;
                        this.bot3Turn = false;
                        this.bot4Turn = true;
                    }
                }
                if (this.bot3GameEnded && !this.bot3Folded)
                {
                    bools.RemoveAt(3);
                    bools.Insert(3, null);
                    maxPlayersLeftCount--;
                    this.bot3Folded = true;
                }
                if (this.bot3GameEnded || !this.bot3Turn)
                {
                    await CheckRaise(3, 3);
                    this.bot4Turn = true;
                }
                if (!this.bot4GameEnded)
                {
                    if (this.bot4Turn)
                    {
                        FixCall(labelBot4Status, ref bot4Call, ref bot4Raise, 1);
                        FixCall(labelBot4Status, ref bot4Call, ref bot4Raise, 2);
                        Rules(8, 9, "Bot 4", ref this.bot4HandMultiplier, ref this.bot4CardPower, this.bot4GameEnded);
                        MessageBox.Show("Bot 4's Turn");
                        AI(8, 9, ref bot4Chips, ref this.bot4Turn, ref this.bot4GameEnded, labelBot4Status, 3, this.bot4CardPower, this.bot4HandMultiplier);
                        turnCount++;
                        lastBotPlayed = 4;
                        this.bot4Turn = false;
                        this.bot5Turn = true;
                    }
                }
                if (this.bot4GameEnded && !this.bot4Folded)
                {
                    bools.RemoveAt(4);
                    bools.Insert(4, null);
                    maxPlayersLeftCount--;
                    this.bot4Folded = true;
                }
                if (bot4GameEnded || !this.bot4Turn)
                {
                    await CheckRaise(4, 4);
                    this.bot5Turn = true;
                }
                if (!bot5GameEnded)
                {
                    if (this.bot5Turn)
                    {
                        FixCall(labelBot5Status, ref bot5Call, ref bot5Raise, 1);
                        FixCall(labelBot5Status, ref bot5Call, ref bot5Raise, 2);
                        Rules(10, 11, "Bot 5", ref this.bot5HandMultiplier, ref this.bot5CardPower, bot5GameEnded);
                        MessageBox.Show("Bot 5's Turn");
                         AI(10, 11, ref bot5Chips, ref this.bot5Turn, ref  bot5GameEnded, labelBot5Status, 4, this.bot5CardPower, this.bot5HandMultiplier);
                        turnCount++;
                        lastBotPlayed = 5;
                        this.bot5Turn = false;
                    }
                }
                if (bot5GameEnded && !this.bot5Folded)
                {
                    bools.RemoveAt(5);
                    bools.Insert(5, null);
                    maxPlayersLeftCount--;
                    this.bot5Folded = true;
                }
                if (bot5GameEnded || !this.bot5Turn)
                {
                    await CheckRaise(5, 5);
                    Pturn = true;
                }
                if (this.playerGameEnded && !this.playerFolded)
                {
                    if (buttonCall.Text.Contains("All in") == false || buttonRaise.Text.Contains("All in") == false)
                    {
                        bools.RemoveAt(0);
                        bools.Insert(0, null);
                        maxPlayersLeftCount--;
                        this.playerFolded = true;
                    }
                }
            #endregion
                await AllIn();
                if (!restart)
                {
                    await Turns();
                }
                restart = false;
            }
        }

        void Rules(int firstCard, int secondCard, string playerName, ref double pokerHandMultiplier, ref double power, bool foldedTurn)
        {
            //if (firstCard == 0 && secondCard == 1)
            //{
            //}

            //if (!foldedTurn || firstCard == 0 && secondCard == 1 && labelPlayerStatus.Text.Contains("Fold") == false)
            //{
            #region Variables
            bool done = false;
            bool vf = false;
            int[] cardsOnTable = new int[5];
            int[] cardsOnTableWithPlayerCards = new int[7];
            cardsOnTableWithPlayerCards[0] = this.dealtCardsNumbers[firstCard];
            cardsOnTableWithPlayerCards[1] = this.dealtCardsNumbers[secondCard];

            cardsOnTable[0] = cardsOnTableWithPlayerCards[2] = this.dealtCardsNumbers[12];
            cardsOnTable[1] = cardsOnTableWithPlayerCards[3] = this.dealtCardsNumbers[13];
            cardsOnTable[2] = cardsOnTableWithPlayerCards[4] = this.dealtCardsNumbers[14];
            cardsOnTable[3] = cardsOnTableWithPlayerCards[5] = this.dealtCardsNumbers[15];
            cardsOnTable[4] = cardsOnTableWithPlayerCards[6] = this.dealtCardsNumbers[16];

            var clubs = cardsOnTableWithPlayerCards.Where(o => o % 4 == 0).ToArray();
            var diamonds = cardsOnTableWithPlayerCards.Where(o => o % 4 == 1).ToArray();
            var hearts = cardsOnTableWithPlayerCards.Where(o => o % 4 == 2).ToArray();
            var spades = cardsOnTableWithPlayerCards.Where(o => o % 4 == 3).ToArray();

            var cardsOfClubs = clubs.Select(o => o / 4).Distinct().ToArray();
            var cardsOfDiamonds = diamonds.Select(o => o / 4).Distinct().ToArray();
            var cardsOfHearts = hearts.Select(o => o / 4).Distinct().ToArray();
            var cardsOfSpades = spades.Select(o => o / 4).Distinct().ToArray();

            Array.Sort(cardsOnTableWithPlayerCards);
            Array.Sort(cardsOfClubs);
            Array.Sort(cardsOfDiamonds);
            Array.Sort(cardsOfHearts);
            Array.Sort(cardsOfSpades);
            #endregion

            for (int i = 0; i < DealtCards - 1; i++)
            {
                //if (this.dealtCards[i] == int.Parse(Holder[firstCard].Tag.ToString()) && this.dealtCards[i + 1] == int.Parse(Holder[secondCard].Tag.ToString()))
                //{
                //Pair from Hand PokerHandMultiplier = 1

                rPairFromHand(ref pokerHandMultiplier, ref power, i);

                #region Pair or Two Pair from Table PokerHandMultiplier = 2 || 0
                rPairTwoPair(ref pokerHandMultiplier, ref power, i);
                #endregion

                #region Two Pair PokerHandMultiplier = 2
                rTwoPair(ref pokerHandMultiplier, ref power, i);
                #endregion

                #region Three of a kind PokerHandMultiplier = 3
                rThreeOfAKind(ref pokerHandMultiplier, ref power, cardsOnTableWithPlayerCards);
                #endregion

                #region Straight PokerHandMultiplier = 4
                rStraight(ref pokerHandMultiplier, ref power, cardsOnTableWithPlayerCards);
                #endregion

                #region Flush PokerHandMultiplier = 5 || 5.5
                rFlush(ref pokerHandMultiplier, ref power, cardsOnTable, i);
                #endregion

                #region Full House PokerHandMultiplier = 6
                rFullHouse(ref pokerHandMultiplier, ref power, ref done, cardsOnTableWithPlayerCards);
                #endregion

                #region Four of a Kind PokerHandMultiplier = 7
                rFourOfAKind(ref pokerHandMultiplier, ref power, cardsOnTableWithPlayerCards);
                #endregion

                #region Straight Flush PokerHandMultiplier = 8 || 9
                rStraightFlush(ref pokerHandMultiplier, ref power, cardsOfClubs, cardsOfDiamonds, cardsOfHearts, cardsOfSpades);
                #endregion

                #region High Card PokerHandMultiplier = -1
                rHighCard(ref pokerHandMultiplier, ref power, i);
                #endregion
                //}
            }
            //}
        }

        //Stani
        private void rStraightFlush(ref double PokerHandMultiplier, ref double Power, int[] cardsOfClubs, int[] cardsOfDiamonds, int[] cardsOfHearts, int[] cardsOfSpades)
        {
            if (PokerHandMultiplier >= -1)
            {
                if (cardsOfClubs.Length >= 5)
                {
                    if (cardsOfClubs[0] + 4 == cardsOfClubs[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfClubs.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfClubs[0] == 0 && cardsOfClubs[1] == 9 && cardsOfClubs[2] == 10 && cardsOfClubs[3] == 11 && cardsOfClubs[0] + 12 == cardsOfClubs[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfClubs.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (cardsOfDiamonds.Length >= 5)
                {
                    if (cardsOfDiamonds[0] + 4 == cardsOfDiamonds[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfDiamonds.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfDiamonds[0] == 0 && cardsOfDiamonds[1] == 9 && cardsOfDiamonds[2] == 10 && cardsOfDiamonds[3] == 11 && cardsOfDiamonds[0] + 12 == cardsOfDiamonds[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfDiamonds.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (cardsOfHearts.Length >= 5)
                {
                    if (cardsOfHearts[0] + 4 == cardsOfHearts[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfHearts.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfHearts[0] == 0 && cardsOfHearts[1] == 9 && cardsOfHearts[2] == 10 && cardsOfHearts[3] == 11 && cardsOfHearts[0] + 12 == cardsOfHearts[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfHearts.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (cardsOfSpades.Length >= 5)
                {
                    if (cardsOfSpades[0] + 4 == cardsOfSpades[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfSpades.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfSpades[0] == 0 && cardsOfSpades[1] == 9 && cardsOfSpades[2] == 10 && cardsOfSpades[3] == 11 && cardsOfSpades[0] + 12 == cardsOfSpades[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfSpades.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rFourOfAKind(ref double PokerHandMultiplier, ref double Power, int[] Straight)
        {
            if (PokerHandMultiplier >= -1)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (Straight[j] / 4 == Straight[j + 1] / 4 && Straight[j] / 4 == Straight[j + 2] / 4 &&
                        Straight[j] / 4 == Straight[j + 3] / 4)
                    {
                        PokerHandMultiplier = 7;
                        Power = (Straight[j] / 4) * 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 7 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (Straight[j] / 4 == 0 && Straight[j + 1] / 4 == 0 && Straight[j + 2] / 4 == 0 && Straight[j + 3] / 4 == 0)
                    {
                        PokerHandMultiplier = 7;
                        Power = 13 * 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 7 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rFullHouse(ref double PokerHandMultiplier, ref double Power, ref bool done, int[] Straight)
        {
            if (PokerHandMultiplier >= -1)
            {
                this.type = Power;
                for (int j = 0; j <= 12; j++)
                {
                    var fh = Straight.Where(o => o / 4 == j).ToArray();
                    if (fh.Length == 3 || done)
                    {
                        if (fh.Length == 2)
                        {
                            if (fh.Max() / 4 == 0)
                            {
                                PokerHandMultiplier = 6;
                                Power = 13 * 2 + PokerHandMultiplier * 100;
                                this.winningCards.Add(new Type() { Power = Power, Current = 6 });
                                this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                                break;
                            }
                            if (fh.Max() / 4 > 0)
                            {
                                PokerHandMultiplier = 6;
                                Power = fh.Max() / 4 * 2 + PokerHandMultiplier * 100;
                                this.winningCards.Add(new Type() { Power = Power, Current = 6 });
                                this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                                break;
                            }
                        }
                        if (!done)
                        {
                            if (fh.Max() / 4 == 0)
                            {
                                Power = 13;
                                done = true;
                                j = -1;
                            }
                            else
                            {
                                Power = fh.Max() / 4;
                                done = true;
                                j = -1;
                            }
                        }
                    }
                }
                if (PokerHandMultiplier != 6)
                {
                    Power = this.type;
                }
            }
        }

        #region Flush
        private void rFlush(ref double PokerHandMultiplier, ref double Power, int[] cardsOnTable, int index)
        {
            //if (PokerHandMultiplier >= -1)
            //{
            var cardsOfClubs = cardsOnTable.Where(o => o % 4 == (int)CardSuit.clubs).ToArray();
            var cardsOfDiamonds = cardsOnTable.Where(o => o % 4 == (int)CardSuit.diamonds).ToArray();
            var cardsOfHearts = cardsOnTable.Where(o => o % 4 == (int)CardSuit.hearts).ToArray();
            var cardsOfSpades = cardsOnTable.Where(o => o % 4 == (int)CardSuit.spades).ToArray();

            if (cardsOfClubs.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, Power, index, cardsOfClubs, (int)CardSuit.clubs);
            }

            if (cardsOfDiamonds.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, Power, index, cardsOfDiamonds, (int)CardSuit.diamonds);
            }

            if (cardsOfHearts.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, Power, index, cardsOfHearts, (int)CardSuit.hearts);
            }
            if (cardsOfSpades.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, Power, index, cardsOfSpades, (int)CardSuit.spades);
            }
            //}
        }
        private void CheckForFlush(double PokerHandMultiplier, double Power, int index, int[] cardsOfSuit, int suitNumber)
        {
            PokerHandMultiplier = 5;

            if (this.GetCardSuit(index) == suitNumber && this.GetCardSuit(index + 1) == this.GetCardSuit(index + 1))
            {
                if (this.GetCardIndex(index) > cardsOfSuit.Max() / 4)
                {
                    this.SetWinningCard(PokerHandMultiplier, index);
                }
                else if (this.GetCardIndex(index + 1) > cardsOfSuit.Max() / 4)
                {
                    this.SetWinningCard(PokerHandMultiplier, index + 1);
                }
                else
                {
                    this.SetWinningCardFromSuitMax(PokerHandMultiplier, cardsOfSuit);
                }
            }

            if (cardsOfSuit.Length == 4) //different cards in hand
            {
                if (this.GetCardSuit(index) == suitNumber &&
                    this.GetCardSuit(index) != this.GetCardSuit(index + 1))
                {
                    if (this.GetCardIndex(index) > cardsOfSuit.Max() / 4)
                    {
                        this.SetWinningCard(PokerHandMultiplier, index);
                    }
                    else
                    {
                        this.SetWinningCardFromSuitMax(PokerHandMultiplier, cardsOfSuit);
                    }
                }

                if (this.GetCardSuit(index + 1) == suitNumber && this.GetCardSuit(index + 1) != this.GetCardSuit(index))
                {
                    if (this.GetCardIndex(index + 1) > cardsOfSuit.Max() / 4)
                    {
                        this.SetWinningCard(PokerHandMultiplier, index + 1);
                    }
                    else
                    {
                        this.SetWinningCardFromSuitMax(PokerHandMultiplier, cardsOfSuit);
                    }
                }
            }

            if (cardsOfSuit.Length == 5)
            {
                if (this.GetCardSuit(index) == cardsOfSuit[0] % 4 &&
                    this.GetCardIndex(index) > cardsOfSuit.Min() / 4)
                {
                    this.SetWinningCard(PokerHandMultiplier, index);
                }
                else if (this.GetCardSuit(index + 1) == cardsOfSuit[0] % 4 &&
                         this.GetCardIndex(index + 1) > cardsOfSuit.Min() / 4)
                {
                    this.SetWinningCard(PokerHandMultiplier, index + 1);
                }
                else if (this.GetCardIndex(index) < cardsOfSuit.Min() / 4 &&
                         this.GetCardIndex(index + 1) < cardsOfSuit.Min())
                {
                    this.SetWinningCardFromSuitMax(PokerHandMultiplier, cardsOfSuit);
                }
            }
        }

        private void SetWinningCardFromSuitMax(double PokerHandMultiplier, int[] cardsOfSuit)
        {
            double power = cardsOfSuit.Max() + PokerHandMultiplier * 100;
            this.FindWinnigCard(PokerHandMultiplier, power);
        }

        private void SetWinningCard(double PokerHandMultiplier, int index)
        {
            double power = this.dealtCardsNumbers[index] + PokerHandMultiplier * 100;
            this.FindWinnigCard(PokerHandMultiplier, power);
        }
        #endregion

        private void rStraight(ref double PokerHandMultiplier, ref double Power, int[] Straight)
        {
            if (PokerHandMultiplier >= -1)
            {
                var op = Straight.Select(o => o / 4).Distinct().ToArray();
                for (int j = 0; j < op.Length - 4; j++)
                {
                    if (op[j] + 4 == op[j + 4])
                    {
                        if (op.Max() - 4 == op[j])
                        {
                            PokerHandMultiplier = 4;
                            Power = op.Max() + PokerHandMultiplier * 100;
                            this.winningCards.Add(new Type() { Power = Power, Current = 4 });
                            this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        }
                        else
                        {
                            PokerHandMultiplier = 4;
                            Power = op[j + 4] + PokerHandMultiplier * 100;
                            this.winningCards.Add(new Type() { Power = Power, Current = 4 });
                            this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        }
                    }
                    if (op[j] == 0 && op[j + 1] == 9 && op[j + 2] == 10 && op[j + 3] == 11 && op[j + 4] == 12)
                    {
                        PokerHandMultiplier = 4;
                        Power = 13 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new Type() { Power = Power, Current = 4 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rThreeOfAKind(ref double PokerHandMultiplier, ref double Power, int[] cardsOnTableWithPlayerCards)
        {
            if (PokerHandMultiplier >= -1)
            {
                for (int j = 0; j <= 12; j++)
                {
                    var fh = cardsOnTableWithPlayerCards.Where(o => o / 4 == j).ToArray();
                    if (fh.Length == 3)
                    {
                        if (fh.Max() / 4 == 0)
                        {
                            PokerHandMultiplier = 3;
                            Power = 13 * 3 + PokerHandMultiplier * 100;
                            this.winningCards.Add(new Type() { Power = Power, Current = 3 });
                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                        }
                        else
                        {
                            PokerHandMultiplier = 3;
                            Power = fh[0] / 4 + fh[1] / 4 + fh[2] / 4 + PokerHandMultiplier * 100;
                            this.winningCards.Add(new Type() { Power = Power, Current = 3 });
                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                        }
                    }
                }
            }
        }
        private void rTwoPair(ref double PokerHandMultiplier, ref double Power, int index)
        {
            if (PokerHandMultiplier >= -1)
            {
                bool msgbox = false;
                for (int tc = 16; tc >= 12; tc--)
                {
                    int max = tc - 12;
                    if (this.dealtCardsNumbers[index] / 4 != this.dealtCardsNumbers[index + 1] / 4)
                    {
                        for (int k = 1; k <= max; k++)
                        {
                            if (tc - k < 12)
                            {
                                max--;
                            }
                            if (tc - k >= 12)
                            {
                                if (this.dealtCardsNumbers[index] / 4 == this.dealtCardsNumbers[tc] / 4 && this.dealtCardsNumbers[index + 1] / 4 == this.dealtCardsNumbers[tc - k] / 4 ||
                                    this.dealtCardsNumbers[index + 1] / 4 == this.dealtCardsNumbers[tc] / 4 && this.dealtCardsNumbers[index] / 4 == this.dealtCardsNumbers[tc - k] / 4)
                                {
                                    if (!msgbox)
                                    {
                                        if (this.dealtCardsNumbers[index] / 4 == 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = 13 * 4 + (this.dealtCardsNumbers[index + 1] / 4) * 2 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new Type() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index + 1] / 4 == 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = 13 * 4 + (this.dealtCardsNumbers[index] / 4) * 2 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new Type() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index + 1] / 4 != 0 && this.dealtCardsNumbers[index] / 4 != 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[index] / 4) * 2 + (this.dealtCardsNumbers[index + 1] / 4) * 2 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new Type() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                    }
                                    msgbox = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void rPairTwoPair(ref double PokerHandMultiplier, ref double Power, int index)
        {
            if (PokerHandMultiplier >= -1)
            {
                bool msgbox = false;
                bool msgbox1 = false;
                for (int tc = 16; tc >= 12; tc--)
                {
                    int max = tc - 12;
                    for (int k = 1; k <= max; k++)
                    {
                        if (tc - k < 12)
                        {
                            max--;
                        }
                        if (tc - k >= 12)
                        {
                            if (this.dealtCardsNumbers[tc] / 4 == this.dealtCardsNumbers[tc - k] / 4)
                            {
                                if (this.dealtCardsNumbers[tc] / 4 != this.dealtCardsNumbers[index] / 4 && this.dealtCardsNumbers[tc] / 4 != this.dealtCardsNumbers[index + 1] / 4 && PokerHandMultiplier == 1)
                                {
                                    if (!msgbox)
                                    {
                                        if (this.dealtCardsNumbers[index + 1] / 4 == 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[index] / 4) * 2 + 13 * 4 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new Type() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index] / 4 == 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[index + 1] / 4) * 2 + 13 * 4 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new Type() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index + 1] / 4 != 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[tc] / 4) * 2 + (this.dealtCardsNumbers[index + 1] / 4) * 2 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new Type() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index] / 4 != 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[tc] / 4) * 2 + (this.dealtCardsNumbers[index] / 4) * 2 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new Type() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                    }
                                    msgbox = true;
                                }
                                if (PokerHandMultiplier == -1)
                                {
                                    if (!msgbox1)
                                    {
                                        if (this.dealtCardsNumbers[index] / 4 > this.dealtCardsNumbers[index + 1] / 4)
                                        {
                                            if (this.dealtCardsNumbers[tc] / 4 == 0)
                                            {
                                                PokerHandMultiplier = 0;
                                                Power = 13 + this.dealtCardsNumbers[index] / 4 + PokerHandMultiplier * 100;
                                                this.winningCards.Add(new Type() { Power = Power, Current = 1 });
                                                this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                            else
                                            {
                                                PokerHandMultiplier = 0;
                                                Power = this.dealtCardsNumbers[tc] / 4 + this.dealtCardsNumbers[index] / 4 + PokerHandMultiplier * 100;
                                                this.winningCards.Add(new Type() { Power = Power, Current = 1 });
                                                this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                        }
                                        else
                                        {
                                            if (this.dealtCardsNumbers[tc] / 4 == 0)
                                            {
                                                PokerHandMultiplier = 0;
                                                Power = 13 + this.dealtCardsNumbers[index + 1] + PokerHandMultiplier * 100;
                                                this.winningCards.Add(new Type() { Power = Power, Current = 1 });
                                                this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                            else
                                            {
                                                PokerHandMultiplier = 0;
                                                Power = this.dealtCardsNumbers[tc] / 4 + this.dealtCardsNumbers[index + 1] / 4 + PokerHandMultiplier * 100;
                                                this.winningCards.Add(new Type() { Power = Power, Current = 1 });
                                                this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                        }
                                    }
                                    msgbox1 = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void rPairFromHand(ref double PokerHandMultiplier, ref double power, int index)
        {
            if (!(PokerHandMultiplier >= -1))
            {
                return;
            }

            bool msgbox = false;

            if (this.GetCardIndex(index) == this.GetCardIndex(index + 1))
            {
                if (!msgbox)
                {
                    PokerHandMultiplier = 1;
                    if (this.GetCardIndex(index) == 0)
                    {
                        power = 13 * 4 + PokerHandMultiplier * 100;
                    }
                    else
                    {
                        power = (this.GetCardIndex(index + 1)) * 4 + PokerHandMultiplier * 100;
                    }

                    this.FindWinnigCard(PokerHandMultiplier, power);
                }
                msgbox = true;
            }

            for (int tc = 16; tc >= 12; tc--)
            {
                if (this.GetCardIndex(index + 1) == this.GetCardIndex(tc))
                {
                    if (!msgbox)
                    {
                        PokerHandMultiplier = 1;

                        if (this.GetCardIndex(index + 1) == 0)
                        {
                            power = 13 * 4 + this.GetCardIndex(index) + PokerHandMultiplier * 100;
                        }
                        else
                        {
                            power = (this.GetCardIndex(index + 1)) * 4 + this.GetCardIndex(index) + PokerHandMultiplier * 100;
                        }

                        this.FindWinnigCard(PokerHandMultiplier, power);

                    }
                    msgbox = true;
                }

                if (this.GetCardIndex(index + 1) != this.GetCardIndex(tc))
                {
                    continue;
                }

                if (!msgbox)
                {
                    PokerHandMultiplier = 1;

                    if (this.GetCardIndex(index) == 0)
                    {
                        power = 13 * 4 + this.GetCardIndex(index + 1) + PokerHandMultiplier * 100;
                    }
                    else
                    {
                        power = (this.GetCardIndex(tc)) * 4 + this.GetCardIndex(index + 1) + PokerHandMultiplier * 100;
                    }

                    this.FindWinnigCard(PokerHandMultiplier, power);
                }
                msgbox = true;
            }
        }

        private void rHighCard(ref double PokerHandMultiplier, ref double Power, int index)
        {
            //if (PokerHandMultiplier != -1)
            //{
            //    return;
            //}

            if (this.GetCardIndex(index) > this.GetCardIndex(index + 1))
            {
                PokerHandMultiplier = -1;
                double power = this.GetCardIndex(index);
                this.FindWinnigCard(PokerHandMultiplier, power);
            }
            else
            {
                PokerHandMultiplier = -1;
                double power = this.GetCardIndex(index + 1);
                this.FindWinnigCard(PokerHandMultiplier, power);
            }

            if (this.GetCardIndex(index) == 0 || this.GetCardIndex(index + 1) == 0)
            {
                PokerHandMultiplier = -1;
                double power = 13;
                this.FindWinnigCard(PokerHandMultiplier, power);
            }
        }

        private int GetCardSuit(int index)
        {
            return this.dealtCardsNumbers[index] % 4;
        }

        private int GetCardIndex(int index)
        {
            return this.dealtCardsNumbers[index] / 4;
        }

        private void FindWinnigCard(double PokerHandMultiplier, double power)
        {
            this.winningCards.Add(new Type() { Power = power, Current = PokerHandMultiplier });
            this.winningCard = this.winningCards
                .OrderByDescending(op1 => op1.Current)
                .ThenByDescending(op1 => op1.Power)
                .First();
        }

        //Veronika
        void Winner(double PokerHandMultiplier, double Power, string playerName, int chips, string lastly)
        {
            if (lastly == " ")
            {
                lastly = "Bot 5";
            }
            for (int j = 0; j <= 16; j++)
            {
                //await Task.Delay(5);
                if (this.cardHolder[j].Visible)
                    this.cardHolder[j].Image = this.DeckImages[j];
            }
            if (PokerHandMultiplier == this.winningCard.Current)
            {
                if (Power == this.winningCard.Power)
                {

                    winnersCount++;
                    CheckWinners.Add(playerName);

                    if (PokerHandMultiplier == -1)
                    {
                        MessageBox.Show(playerName + " High Card ");
                    }
                    if (PokerHandMultiplier == 1 || PokerHandMultiplier == 0)
                    {
                        MessageBox.Show(playerName + " Pair ");
                    }
                    if (PokerHandMultiplier == 2)
                    {
                        MessageBox.Show(playerName + " Two Pair ");
                    }
                    if (PokerHandMultiplier == 3)
                    {
                        MessageBox.Show(playerName + " Three of a Kind ");
                    }
                    if (PokerHandMultiplier == 4)
                    {
                        MessageBox.Show(playerName + " Straight ");
                    }
                    if (PokerHandMultiplier == 5 || PokerHandMultiplier == 5.5)
                    {
                        MessageBox.Show(playerName + " Flush ");
                    }
                    if (PokerHandMultiplier == 6)
                    {
                        MessageBox.Show(playerName + " Full House ");
                    }
                    if (PokerHandMultiplier == 7)
                    {
                        MessageBox.Show(playerName + " Four of a Kind ");
                    }
                    if (PokerHandMultiplier == 8)
                    {
                        MessageBox.Show(playerName + " Straight Flush ");
                    }
                    if (PokerHandMultiplier == 9)
                    {
                        MessageBox.Show(playerName + " Royal Flush ! ");
                    }
                }
            }
            if (playerName == lastly)//lastfixed
            {
                if (winnersCount > 1)
                {
                    if (CheckWinners.Contains("Player"))
                    {
                        this.playerChips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxPlayerChips.Text = this.playerChips.ToString();
                        //pPanel.Visible = true;

                    }
                    if (CheckWinners.Contains("Bot 1"))
                    {
                        bot1Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot1Chips.Text = bot1Chips.ToString();
                        //b1Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 2"))
                    {
                        bot2Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot2Chips.Text = bot2Chips.ToString();
                        //b2Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 3"))
                    {
                        bot3Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot3Chips.Text = bot3Chips.ToString();
                        //b3Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 4"))
                    {
                        bot4Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot4Chips.Text = bot4Chips.ToString();
                        //b4Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 5"))
                    {
                        bot5Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot5Chips.Text = bot5Chips.ToString();
                        //b5Panel.Visible = true;
                    }
                    //await Finish(1);
                }
                if (winnersCount == 1)
                {
                    if (CheckWinners.Contains("Player"))
                    {
                        this.playerChips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //pPanel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 1"))
                    {
                        bot1Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b1Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 2"))
                    {
                        bot2Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b2Panel.Visible = true;

                    }
                    if (CheckWinners.Contains("Bot 3"))
                    {
                        bot3Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b3Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 4"))
                    {
                        bot4Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b4Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 5"))
                    {
                        bot5Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b5Panel.Visible = true;
                    }
                }
            }
        }
        async Task CheckRaise(int currentTurn, int raiseTurn)
        {
            if (raising)
            {
                turnCount = 0;
                raising = false;
                raisedTurn = currentTurn;
                changed = true;
            }
            else
            {
                if (turnCount >= maxPlayersLeftCount - 1 || !changed && turnCount == maxPlayersLeftCount)
                {
                    if (currentTurn == raisedTurn - 1 || !changed && turnCount == maxPlayersLeftCount || raisedTurn == 0 && currentTurn == 5)
                    {
                        changed = false;
                        turnCount = 0;
                        this.raise = 0;
                        this.pokerCall = 0;
                        raisedTurn = 123;
                        rounds++;
                        if (!this.playerGameEnded)
                            labelPlayerStatus.Text = "";
                        if (!this.bot1GameEnded)
                            labelBot1Status.Text = "";
                        if (!this.bot2GameEnded)
                            labelBot2Status.Text = "";
                        if (!this.bot3GameEnded)
                            labelBot3Status.Text = "";
                        if (!this.bot4GameEnded)
                            labelBot4Status.Text = "";
                        if (!this.bot5GameEnded)
                            labelBot5Status.Text = "";
                    }
                }
            }
            if (rounds == Flop)
            {
                for (int j = 12; j <= 14; j++)
                {
                    if (this.cardHolder[j].Image != this.DeckImages[j])
                    {
                        this.cardHolder[j].Image = this.DeckImages[j];
                        this.playerCall = 0; playerRaise = 0;
                        this.bot1Call = 0; bot1Raise = 0;
                        this.bot2Call = 0; bot2Raise = 0;
                        bot3Call = 0; bot3Raise = 0;
                        bot4Call = 0; bot4Raise = 0;
                        bot5Call = 0; bot5Raise = 0;
                    }
                }
            }
            if (rounds == Turn)
            {
                for (int j = 14; j <= 15; j++)
                {
                    if (this.cardHolder[j].Image != this.DeckImages[j])
                    {
                        this.cardHolder[j].Image = this.DeckImages[j];
                        this.playerCall = 0; playerRaise = 0;
                        this.bot1Call = 0; bot1Raise = 0;
                        this.bot2Call = 0; bot2Raise = 0;
                        bot3Call = 0; bot3Raise = 0;
                        bot4Call = 0; bot4Raise = 0;
                        bot5Call = 0; bot5Raise = 0;
                    }
                }
            }
            if (rounds == River)
            {
                for (int j = 15; j <= 16; j++)
                {
                    if (this.cardHolder[j].Image != this.DeckImages[j])
                    {
                        this.cardHolder[j].Image = this.DeckImages[j];
                        this.playerCall = 0; playerRaise = 0;
                        this.bot1Call = 0; bot1Raise = 0;
                        this.bot2Call = 0; bot2Raise = 0;
                        bot3Call = 0; bot3Raise = 0;
                        bot4Call = 0; bot4Raise = 0;
                        bot5Call = 0; bot5Raise = 0;
                    }
                }
            }
            if (rounds == End && maxPlayersLeftCount == 6)
            {
                string fixedLast = "qwerty";
                if (!labelPlayerStatus.Text.Contains("Fold"))
                {
                    fixedLast = "Player";
                    Rules(0, 1, "Player", ref this.playerHandMultiplier, ref this.playerCardPower, this.playerGameEnded);
                }
                if (!labelBot1Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 1";
                    Rules(2, 3, "Bot 1", ref this.bot1HandMultiplier, ref this.bot1CardPower, this.bot1GameEnded);
                }
                if (!labelBot2Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 2";
                    Rules(4, 5, "Bot 2", ref this.bot2HandMultiplier, ref this.bot2CardPower, this.bot2GameEnded);
                }
                if (!labelBot3Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 3";
                    Rules(6, 7, "Bot 3", ref this.bot3HandMultiplier, ref this.bot3CardPower, this.bot3GameEnded);
                }
                if (!labelBot4Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 4";
                    Rules(8, 9, "Bot 4", ref this.bot4HandMultiplier, ref this.bot4CardPower, this.bot4GameEnded);
                }
                if (!labelBot5Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 5";
                    Rules(10, 11, "Bot 5", ref this.bot5HandMultiplier, ref this.bot5CardPower, this.bot5GameEnded);
                }
                Winner(this.playerHandMultiplier, this.playerCardPower, "Player", this.playerChips, fixedLast);
                Winner(this.bot1HandMultiplier, this.bot1CardPower, "Bot 1", bot1Chips, fixedLast);
                Winner(this.bot2HandMultiplier, this.bot2CardPower, "Bot 2", bot2Chips, fixedLast);
                Winner(this.bot3HandMultiplier, this.bot3CardPower, "Bot 3", bot3Chips, fixedLast);
                Winner(this.bot4HandMultiplier, this.bot4CardPower, "Bot 4", bot4Chips, fixedLast);
                Winner(this.bot5HandMultiplier, this.bot5CardPower, "Bot 5", bot5Chips, fixedLast);
                restart = true;
                Pturn = true;
                this.playerGameEnded = false;
                this.bot1GameEnded = false;
                this.bot2GameEnded = false;
                this.bot3GameEnded = false;
                this.bot4GameEnded = false;
                this.bot5GameEnded = false;
                if (this.playerChips <= 0)
                {
                    AddChips f2 = new AddChips();
                    f2.ShowDialog();
                    if (f2.a != 0)
                    {
                        this.playerChips = f2.a;
                        bot1Chips += f2.a;
                        bot2Chips += f2.a;
                        bot3Chips += f2.a;
                        bot4Chips += f2.a;
                        bot5Chips += f2.a;
                        this.playerGameEnded = false;
                        Pturn = true;
                        buttonRaise.Enabled = true;
                        buttonFold.Enabled = true;
                        buttonCheck.Enabled = true;
                        buttonRaise.Text = "Raise";
                    }
                }
                this.playerCardsPanel.Visible = false; this.bot1CardsPanel.Visible = false; this.bot2CardsPanel.Visible = false; this.bot3CardsPanel.Visible = false; this.bot4CardsPanel.Visible = false; this.bot5CardsPanel.Visible = false;
                this.playerCall = 0; playerRaise = 0;
                this.bot1Call = 0; bot1Raise = 0;
                this.bot2Call = 0; bot2Raise = 0;
                bot3Call = 0; bot3Raise = 0;
                bot4Call = 0; bot4Raise = 0;
                bot5Call = 0; bot5Raise = 0;
                lastBotPlayed = 0;
                this.pokerCall = bb;
                this.raise = 0;
                this.ImageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
                bools.Clear();
                rounds = 0;
                this.playerCardPower = 0; this.playerHandMultiplier = -1;
                type = 0; this.bot1CardPower = 0; this.bot2CardPower = 0; this.bot3CardPower = 0; this.bot4CardPower = 0; this.bot5CardPower = 0;
                this.bot1HandMultiplier = -1; this.bot2HandMultiplier = -1; this.bot3HandMultiplier = -1; this.bot4HandMultiplier = -1; this.bot5HandMultiplier = -1;
                ints.Clear();
                CheckWinners.Clear();
                winnersCount = 0;
                this.winningCards.Clear();
                this.winningCard.Current = 0;
                this.winningCard.Power = 0;
                for (int os = 0; os < 17; os++)
                {
                    this.cardHolder[os].Image = null;
                    this.cardHolder[os].Invalidate();
                    this.cardHolder[os].Visible = false;
                }
                textBoxPot.Text = "0";
                labelPlayerStatus.Text = "";
                await this.SetupPokerTable();
                await Turns();
            }
        }
        void FixCall(Label status, ref int cCall, ref int cRaise, int options)
        {
            if (rounds != 4)
            {
                if (options == 1)
                {
                    if (status.Text.Contains("Raise"))
                    {
                        var changeRaise = status.Text.Substring(6);
                        cRaise = int.Parse(changeRaise);
                    }
                    if (status.Text.Contains("Call"))
                    {
                        var changeCall = status.Text.Substring(5);
                        cCall = int.Parse(changeCall);
                    }
                    if (status.Text.Contains("Check"))
                    {
                        cRaise = 0;
                        cCall = 0;
                    }
                }
                if (options == 2)
                {
                    if (cRaise != this.raise && cRaise <= this.raise)
                    {
                        this.pokerCall = Convert.ToInt32(this.raise) - cRaise;
                    }
                    if (cCall != this.pokerCall || cCall <= this.pokerCall)
                    {
                        this.pokerCall = this.pokerCall - cCall;
                    }
                    if (cRaise == this.raise && this.raise > 0)
                    {
                        this.pokerCall = 0;
                        buttonCall.Enabled = false;
                        buttonCall.Text = "Callisfuckedup";
                    }
                }
            }
        }
        async Task AllIn()
        {
            #region All in
            if (this.playerChips <= 0 && !intsadded)
            {
                if (labelPlayerStatus.Text.Contains("Raise"))
                {
                    ints.Add(this.playerChips);
                    intsadded = true;
                }
                if (labelPlayerStatus.Text.Contains("Call"))
                {
                    ints.Add(this.playerChips);
                    intsadded = true;
                }
            }
            intsadded = false;
            if (bot1Chips <= 0 && !this.bot1GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot1Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot2Chips <= 0 && !this.bot2GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot2Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot3Chips <= 0 && !this.bot3GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot3Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot4Chips <= 0 && !this.bot4GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot4Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (bot5Chips <= 0 && !this.bot5GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(bot5Chips);
                    intsadded = true;
                }
            }
            if (ints.ToArray().Length == this.maxPlayersLeftCount)
            {
                await Finish(2);
            }
            else
            {
                ints.Clear();
            }
            #endregion

            var abc = bools.Count(x => x == false);

            #region LastManStanding
            if (abc == 1)
            {
                int index = bools.IndexOf(false);
                if (index == 0)
                {
                    this.playerChips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.playerChips.ToString();
                    this.playerCardsPanel.Visible = true;
                    MessageBox.Show("Player Wins");
                }
                if (index == 1)
                {
                    bot1Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot1Chips.ToString();
                    this.bot1CardsPanel.Visible = true;
                    MessageBox.Show("Bot 1 Wins");
                }
                if (index == 2)
                {
                    bot2Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot2Chips.ToString();
                    this.bot2CardsPanel.Visible = true;
                    MessageBox.Show("Bot 2 Wins");
                }
                if (index == 3)
                {
                    bot3Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot3Chips.ToString();
                    this.bot3CardsPanel.Visible = true;
                    MessageBox.Show("Bot 3 Wins");
                }
                if (index == 4)
                {
                    bot4Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot4Chips.ToString();
                    this.bot4CardsPanel.Visible = true;
                    MessageBox.Show("Bot 4 Wins");
                }
                if (index == 5)
                {
                    bot5Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = bot5Chips.ToString();
                    this.bot5CardsPanel.Visible = true;
                    MessageBox.Show("Bot 5 Wins");
                }
                for (int j = 0; j <= 16; j++)
                {
                    this.cardHolder[j].Visible = false;
                }
                await Finish(1);
            }
            intsadded = false;
            #endregion

            #region FiveOrLessLeft
            if (abc < 6 && abc > 1 && rounds >= End)
            {
                await Finish(2);
            }
            #endregion


        }
        async Task Finish(int n)
        {
            if (n == 2)
            {
                FixWinners();
            }
            this.playerCardsPanel.Visible = false; this.bot1CardsPanel.Visible = false; this.bot2CardsPanel.Visible = false; this.bot3CardsPanel.Visible = false; this.bot4CardsPanel.Visible = false; this.bot5CardsPanel.Visible = false;
            this.pokerCall = bb; this.raise = 0;
            foldedPlayers = 5;
            type = 0; rounds = 0; this.bot1CardPower = 0; this.bot2CardPower = 0; this.bot3CardPower = 0; this.bot4CardPower = 0; this.bot5CardPower = 0; this.playerCardPower = 0; this.playerHandMultiplier = -1; this.raise = 0;
            this.bot1HandMultiplier = -1; this.bot2HandMultiplier = -1; this.bot3HandMultiplier = -1; this.bot4HandMultiplier = -1; this.bot5HandMultiplier = -1;
            this.bot1Turn = false; this.bot2Turn = false; this.bot3Turn = false; this.bot4Turn = false; this.bot5Turn = false;
            this.bot1GameEnded = false; this.bot2GameEnded = false; this.bot3GameEnded = false; this.bot4GameEnded = false; this.bot5GameEnded = false;
            this.playerFolded = false; this.bot1Folded = false; this.bot2Folded = false; this.bot3Folded = false; this.bot4Folded = false; this.bot5Folded = false;
            this.playerGameEnded = false; Pturn = true; restart = false; raising = false;
            this.playerCall = 0; this.bot1Call = 0; this.bot2Call = 0; bot3Call = 0; bot4Call = 0; bot5Call = 0; playerRaise = 0; bot1Raise = 0; bot2Raise = 0; bot3Raise = 0; bot4Raise = 0; bot5Raise = 0;
            height = 0; width = 0; winnersCount = 0; Flop = 1; Turn = 2; River = 3; End = 4; maxPlayersLeftCount = 6;
            lastBotPlayed = 123; raisedTurn = 1;
            bools.Clear();
            CheckWinners.Clear();
            ints.Clear();
            this.winningCards.Clear();
            this.winningCard.Current = 0;
            this.winningCard.Power = 0;
            textBoxPot.Text = "0";
            t = 60; up = 10000000; turnCount = 0;
            labelPlayerStatus.Text = "";
            labelBot1Status.Text = "";
            labelBot2Status.Text = "";
            labelBot3Status.Text = "";
            labelBot4Status.Text = "";
            labelBot5Status.Text = "";
            if (this.playerChips <= 0)
            {
                AddChips f2 = new AddChips();
                f2.ShowDialog();
                if (f2.a != 0)
                {
                    this.playerChips = f2.a;
                    bot1Chips += f2.a;
                    bot2Chips += f2.a;
                    bot3Chips += f2.a;
                    bot4Chips += f2.a;
                    bot5Chips += f2.a;
                    this.playerGameEnded = false;
                    Pturn = true;
                    buttonRaise.Enabled = true;
                    buttonFold.Enabled = true;
                    buttonCheck.Enabled = true;
                    buttonRaise.Text = "Raise";
                }
            }
            this.ImageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
            for (int os = 0; os < 17; os++)
            {
                this.cardHolder[os].Image = null;
                this.cardHolder[os].Invalidate();
                this.cardHolder[os].Visible = false;
            }
            await this.SetupPokerTable();
            //await Turns();
        }
        void FixWinners()
        {
            this.winningCards.Clear();
            this.winningCard.Current = 0;
            this.winningCard.Power = 0;
            string fixedLast = "qwerty";
            if (!labelPlayerStatus.Text.Contains("Fold"))
            {
                fixedLast = "Player";
                Rules(0, 1, "Player", ref this.playerHandMultiplier, ref this.playerCardPower, this.playerGameEnded);
            }
            if (!labelBot1Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 1";
                Rules(2, 3, "Bot 1", ref this.bot1HandMultiplier, ref this.bot1CardPower, this.bot1GameEnded);
            }
            if (!labelBot2Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 2";
                Rules(4, 5, "Bot 2", ref this.bot2HandMultiplier, ref this.bot2CardPower, bot2GameEnded);
            }
            if (!labelBot3Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 3";
                Rules(6, 7, "Bot 3", ref this.bot3HandMultiplier, ref this.bot3CardPower, bot3GameEnded);
            }
            if (!labelBot4Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 4";
                Rules(8, 9, "Bot 4", ref this.bot4HandMultiplier, ref this.bot4CardPower, bot4GameEnded);
            }
            if (!labelBot5Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 5";
                Rules(10, 11, "Bot 5", ref this.bot5HandMultiplier, ref this.bot5CardPower, bot5GameEnded);
            }
            Winner(this.playerHandMultiplier, this.playerCardPower, "Player", this.playerChips, fixedLast);
            Winner(this.bot1HandMultiplier, this.bot1CardPower, "Bot 1", bot1Chips, fixedLast);
            Winner(this.bot2HandMultiplier, this.bot2CardPower, "Bot 2", bot2Chips, fixedLast);
            Winner(this.bot3HandMultiplier, this.bot3CardPower, "Bot 3", bot3Chips, fixedLast);
            Winner(this.bot4HandMultiplier, this.bot4CardPower, "Bot 4", bot4Chips, fixedLast);
            Winner(this.bot5HandMultiplier, this.bot5CardPower, "Bot 5", bot5Chips, fixedLast);
        }

        //Plamena
        #region AI logic
        // AI   (     2,      3, ref    bot1Chips, ref this.bot1Turn,ref this.bot1GameEnded, labelBot1Status,       0, this.bot1CardPower, this.bot1HandMultiplier);
        /* wika se ot Turns : 
            if (!this.bot1GameEnded) => if (this.bot1Turn):
            if (!this.bot2GameEnded) => if (this.bot2Turn):
            if (!this.bot3GameEnded) => if (this.bo3Turn) :
            if (!this.bot4GameEnded) => if (this.bot4Turn) :
            if (!bot5GameEnded) =>  if (this.bot5Turn) =>  
        => AI(2, 3, ref bot1Chips, ref this.bot1Turn, ref this.bot1GameEnded, labelBot1Status, 0, this.bot1CardPower, this.bot1HandMultiplier);
        => AI(4, 5, ref bot2Chips, ref this.bot2Turn, ref this.bot2GameEnded, labelBot2Status, 1, this.bot2CardPower, this.bot2HandMultiplier);
        => AI(6, 7, ref bot3Chips, ref this.bot3Turn, ref this.bot3GameEnded, labelBot3Status, 2, this.bot3CardPower, this.bot3HandMultiplier);
        => AI(8, 9, ref bot4Chips, ref this.bot4Turn, ref this.bot4GameEnded, labelBot4Status, 3, this.bot4CardPower, this.bot4HandMultiplier);
        => AI(10, 11, ref bot5Chips, ref this.bot5Turn, ref  bot5GameEnded, labelBot5Status, 4, this.bot5CardPower, this.bot5HandMultiplier);
         note: int name se polzwa samo pri wikaneto na smooth*/
        private void AI(int positionCard1, int positionCard2, ref int botChips, ref bool botTurn, ref bool botGameEnded, Label labelBotStatus, int name, double botPower, double botHandMultiplier)
        {
           if (!botGameEnded)
            {
                switch (botHandMultiplier.ToString(CultureInfo.CreateSpecificCulture("en-GB")))
                {
                    case "-1": 
                        this.AIHighCard(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower);
                        break;
                    case "0": this.AIPairTable(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); 
                        break;
                    case "1": this.AIPairHand(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower);
                        break;
                    case "2": this.AITwoPair(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower);
                        break;
                    case "3": this.AIThreeOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower);
                        break;
                    case "4": this.AIStraight(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); 
                        break;
                    case "5":
                    case "5.5": this.AIFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); 
                        break;
                    case "6": this.AIFullHouse(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower);
                        break;
                    case "7": this.AIFourOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); 
                        break;
                    case "8":
                    case "9": this.AIStraightFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower);
                        break;
                }
           }

            if (botGameEnded)
            {
                this.cardHolder[positionCard1].Visible = false;
                this.cardHolder[positionCard2].Visible = false;
            }
        }

        // Wika se ot AI this.AIHighCard(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika HP
        private void AIHighCard(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            this.AIHP(ref botChips, ref sTurn, ref sFTurn, sStatus, botPower, 20, 25);
        }

        // wika se ot AI this.AIPairTable(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika HP
        private void AIPairTable(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            this.AIHP(ref botChips, ref sTurn, ref sFTurn, sStatus, botPower, 16, 25);
        }
        
        // wika se ot AI this.AIPairHand(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
        private void AIPairHand(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            Random rPair = new Random();
            int rCall = rPair.Next(10, 16);
            int rRaise = rPair.Next(10, 13);
            if (botPower <= 199 && botPower >= 140)
            {
                this.AIPH(ref botChips, ref sTurn, ref sFTurn, sStatus, rCall, 6, rRaise);
            }
            if (botPower <= 139 && botPower >= 128)
            {
                this.AIPH(ref botChips, ref sTurn, ref sFTurn, sStatus, rCall, 7, rRaise);
            }
            if (botPower < 128 && botPower >= 101)
            {
                this.AIPH(ref botChips, ref sTurn, ref sFTurn, sStatus, rCall, 9, rRaise);
            }
        }

        // wika se ot AI this.AITwoPair(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
        private void AITwoPair(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, double botPower)
        {
            Random rPair = new Random();
            int rCall = rPair.Next(6, 11);
            int rRaise = rPair.Next(6, 11);
            if (botPower <= 290 && botPower >= 246)
            {
                this.AIPH(ref botChips, ref sTurn, ref sFTurn, sStatus, rCall, 3, rRaise);
            }
            if (botPower <= 244 && botPower >= 234)
            {
                this.AIPH(ref botChips, ref sTurn, ref sFTurn, sStatus, rCall, 4, rRaise);
            }
            if (botPower < 234 && botPower >= 201)
            {
                this.AIPH(ref botChips, ref sTurn, ref sFTurn, sStatus, rCall, 4, rRaise);
            }
        }

        // wika se ot AI this.AIThreeOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth
        private void AIThreeOfAKind(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random tk = new Random();
            int tCall = tk.Next(3, 7);
            int tRaise = tk.Next(4, 8);
            if (botPower <= 390 && botPower >= 330)
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, tCall, tRaise);
            }
            if (botPower <= 327 && botPower >= 321)//10  8
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, tCall, tRaise);
            }
            if (botPower < 321 && botPower >= 303)//7 2
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, tCall, tRaise);
            }
        }

        // wika se ot AI this.AIStraight(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIStraight(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random str = new Random();
            int sCall = str.Next(3, 6);
            int sRaise = str.Next(3, 8);
            if (botPower <= 480 && botPower >= 410)
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, sCall, sRaise);
            }
            if (botPower <= 409 && botPower >= 407)//10  8
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, sCall, sRaise);
            }
            if (botPower < 407 && botPower >= 404)
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, sCall, sRaise);
            }
        }

        // wika se ot AI this.AIFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIFlush(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random fsh = new Random();
            int fCall = fsh.Next(2, 6);
            int fRaise = fsh.Next(3, 7);
            this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, fCall, fRaise);
        }

        // wika se ot AI : this.AIFullHouse(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth
        private void AIFullHouse(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random flh = new Random();
            int fhCall = flh.Next(1, 5);
            int fhRaise = flh.Next(2, 6);
            if (botPower <= 626 && botPower >= 620)
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, fhCall, fhRaise);
            }
            if (botPower < 620 && botPower >= 602)
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, fhCall, fhRaise);
            }
        }

        // wika se ot AI: this.AIFourOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIFourOfAKind(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random fk = new Random();
            int fkCall = fk.Next(1, 4);
            int fkRaise = fk.Next(2, 5);
            if (botPower <= 752 && botPower >= 704)
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, fkCall, fkRaise);
            }
        }

        // wika se ot AI: this.AIStraightFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth 
        private void AIStraightFlush(ref int botChips, ref bool sTurn, ref bool sFTurn, Label sStatus, int name, double botPower)
        {
            Random sf = new Random();
            int sfCall = sf.Next(1, 3);
            int sfRaise = sf.Next(1, 3);
            if (botPower <= 913 && botPower >= 804)
            {
                this.AISmooth(ref botChips, ref sTurn, ref sFTurn, sStatus, name, sfCall, sfRaise);
            }
        }

        /* wika se ot: AIHighCard, AIPairTable, 
         randoma e ot 1-3
         ako pokerCall <= 0 bota igrae CHeck
         ako pokerCall > 0 w zawisimost ot randoma ima 3 wyzmovnosti:
         * 1 -> move da igrae Call ili Fold
         * 2 -> move da igrae Call ili Fold
         * 3 -> move da wdigne ili da Fold 
         */
        private void AIHP(ref int botChips, ref bool bothTurn, ref bool botEndGame, Label sStatus, double botPower, int n, int n1)
        {
            Random rand = new Random();
            
            // staro:int rnd = rand.Next(1, 4); t.kato rnd = 4 nikyde ne se polzwa
            int rnd = rand.Next(1, 4);
            
            if (this.pokerCall <= 0)
            {
                // bota igrae CHeck
                this.AICheck(ref bothTurn, sStatus);
            }

            if (this.pokerCall > 0)
            {
                if (rnd == 1)
                {
                    if (this.pokerCall <= AIRoundNumber(botChips, n))
                    {
                        this.AICall(ref botChips, ref bothTurn, sStatus);
                    }
                    else
                    {
                        this.AIFold(ref bothTurn, ref botEndGame, sStatus);
                    }
                }

                if (rnd == 2)
                {
                    if (this.pokerCall <= AIRoundNumber(botChips, n1))
                    {
                        this.AICall(ref botChips, ref bothTurn, sStatus);
                    }
                    else
                    {
                        this.AIFold(ref bothTurn, ref botEndGame, sStatus);
                    }
                }
            }
            if (rnd == 3)
            {
                if (this.raise == 0)
                {
                    //smqta s kolko da wdigne bota
                    this.raise = this.pokerCall * 2;
                    
                    //cbota igrae Raise
                    this.AIRaised(ref botChips, ref bothTurn, sStatus);
                }
                else
                {
                    if (this.raise <= AIRoundNumber(botChips, n))
                    {
                        this.raise = this.pokerCall * 2;
                        this.AIRaised(ref botChips, ref bothTurn, sStatus);
                    }
                    else
                    {
                        this.AIFold(ref bothTurn, ref botEndGame, sStatus);
                    }
                }
            }

            if (botChips <= 0)
            {
                botEndGame = true;
            }
        }

        /*wika se ot: AIPairHand, AITwoPair
         randoma e ot 1-3
         ako  this.rounds < 2 
            ako pokerCall <= 0 bota igrae CHeck
            ako pokerCall > 0 w zawisimost ot matematikata na chipowete move da FOLD, CALL, RASE
         ako this.rounds >= 2
                ako pokerCall <= 0 bota igrae Raise
                ako pokerCall > 0 w zawisimost ot matematikata na chipowete i rnd, move da FOLD, CALL, RASE
             ako botChips <= 0 => setwa botEndGame = true;
         */
        private void AIPH(ref int botChips, ref bool sTurn, ref bool botEndGame, Label sStatus, int n, int n1, int r)
        {
            Random rand = new Random();
            int rnd = rand.Next(1, 3);
            if (this.rounds < 2)
            {
                if (this.pokerCall <= 0)
                {
                    this.AICheck(ref sTurn, sStatus);
                }

                if (this.pokerCall > 0)
                {
                    if (this.pokerCall >= AIRoundNumber(botChips, n1))
                    {
                        this.AIFold(ref sTurn, ref botEndGame, sStatus);
                    }

                    if (this.raise > AIRoundNumber(botChips, n))
                    {
                        this.AIFold(ref sTurn, ref botEndGame, sStatus);
                    }

                    if (!botEndGame)
                    {
                        if (this.pokerCall >= AIRoundNumber(botChips, n) && this.pokerCall <= AIRoundNumber(botChips, n1))
                        {
                            this.AICall(ref botChips, ref sTurn, sStatus);
                        }

                        if (this.raise <= AIRoundNumber(botChips, n) && this.raise >= (AIRoundNumber(botChips, n)) / 2)
                        {
                            this.AICall(ref botChips, ref sTurn, sStatus);
                        }

                        if (this.raise <= (AIRoundNumber(botChips, n)) / 2)
                        {
                            if (this.raise > 0)
                            {
                                this.raise = AIRoundNumber(botChips, n);
                                this.AIRaised(ref botChips, ref sTurn, sStatus);
                            }
                            else
                            {
                                this.raise = this.pokerCall * 2;
                                this.AIRaised(ref botChips, ref sTurn, sStatus);
                            }
                        }
                    }
                }
            }

            /*
             
             */
            if (this.rounds >= 2)
            {
                if (this.pokerCall > 0)
                {
                    if (this.pokerCall >= AIRoundNumber(botChips, n1 - rnd))
                    {
                        this.AIFold(ref sTurn, ref botEndGame, sStatus);
                    }
                    if (this.raise > AIRoundNumber(botChips, n - rnd))
                    {
                        this.AIFold(ref sTurn, ref botEndGame, sStatus);
                    }
                    if (!botEndGame)
                    {
                        if (this.pokerCall >= AIRoundNumber(botChips, n - rnd) && this.pokerCall <= AIRoundNumber(botChips, n1 - rnd))
                        {
                            this.AICall(ref botChips, ref sTurn, sStatus);
                        }
                        if (this.raise <= AIRoundNumber(botChips, n - rnd) && this.raise >= (AIRoundNumber(botChips, n - rnd)) / 2)
                        {
                            this.AICall(ref botChips, ref sTurn, sStatus);
                        }
                        if (this.raise <= (AIRoundNumber(botChips, n - rnd)) / 2)
                        {
                            if (this.raise > 0)
                            {
                                this.raise = AIRoundNumber(botChips, n - rnd);
                                this.AIRaised(ref botChips, ref sTurn, sStatus);
                            }
                            else
                            {
                                this.raise = this.pokerCall * 2;
                                this.AIRaised(ref botChips, ref sTurn, sStatus);
                            }
                        }
                    }
                }
                if (this.pokerCall <= 0)
                {
                    this.raise = AIRoundNumber(botChips, r - rnd);
                    this.AIRaised(ref botChips, ref sTurn, sStatus);
                }
            }

            if (botChips <= 0)
            {
                botEndGame = true;
            }
        }

        /*wika se ot: AIThreeOfAKind, AIStraight, AIFlush, AIFullHouse, AIFourOfAKind, AIStraightFlush
         randoma e ot 1-3, NO NE SE POLZWA!
         ako pokerCall <= 0 bota igrae CHeck
         ako pokerCall > 0 w zawisimost ot matematikata na chipowete move da CALL, RASE
         ako botChips <= 0 => setwa botEndGame = true;
         */
        private void AISmooth(ref int botChips, ref bool botTurn, ref bool botEndGame, Label botStatus, int name, int n, int r)
        {
          // star kod - > zakomentiran t.kato ne se polzwa: 
          // Random rand = new Random();
          // int rnd = rand.Next(1, 3);
            if (this.pokerCall <= 0)
            {
                this.AICheck(ref botTurn, botStatus);
            }
            else
            {
                if (this.pokerCall >= AIRoundNumber(botChips, n))
                {
                    if (botChips > this.pokerCall)
                    {
                        this.AICall(ref botChips, ref botTurn, botStatus);
                    }
                    else if (botChips <= this.pokerCall)
                    {
                        raising = false;
                        botTurn = false;
                        botChips = 0;
                        botStatus.Text = "Call " + botChips;
                        textBoxPot.Text = (int.Parse(textBoxPot.Text) + botChips).ToString();
                    }
                }
                else
                {
                    if (this.raise > 0)
                    {
                        if (botChips >= this.raise * 2)
                        {
                            this.raise *= 2;
                            this.AIRaised(ref botChips, ref botTurn, botStatus);
                        }
                        else
                        {
                            this.AICall(ref botChips, ref botTurn, botStatus);
                        }
                    }
                    else
                    {
                        this.raise = this.pokerCall * 2;
                        this.AIRaised(ref botChips, ref botTurn, botStatus);
                    }
                }
            }
            if (botChips <= 0)
            {
                botEndGame = true;
            }
        }
      




        // wika se ot AIHP ili AIPH  this.AIFold(ref bothTurn, ref botEndGame, sStatus);
        private void AIFold(ref bool sTurn, ref bool sFTurn, Label sStatus)
        {
            raising = false;
            sStatus.Text = "Fold";
            sTurn = false;
            sFTurn = true;
        }

        // вика се от AIHP, AIPH, AISmooth  this.AICheck(ref bothTurn, sStatus);
        /// <summary>
        /// the bot plays CHECK. 
        /// The method sets bot's statusLable on Check; botTurn on False and both raising on False;
        /// </summary>
        /// <param name="cTurn">
        /// podawa se poreferenciq t.kato nqma obekt kojto da dyrvi stojnostite.
        /// </param>
        /// <param name="cStatus">
        /// podawa se poreferenciq t.kato nqma obekt kojto da dyrvi stojnostite.
        /// </param>
        private void AICheck(ref bool cTurn, Label cStatus)
        {
            cStatus.Text = "Check";
            cTurn = false;
            raising = false;
        }

        // вика се от  AIHP AIPH, AISmooth this.AICall(ref botChips, ref bothTurn, sStatus);
        private void AICall(ref int botChips, ref bool sTurn, Label sStatus)
        {
            this.raising = false;
            sTurn = false;
            botChips -= this.pokerCall;
            sStatus.Text = "Call " + this.pokerCall;
            this.textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
        }

        // вика се от  AIHP AIPH, AISmooth this.AIRaised(ref botChips, ref bothTurn, sStatus);
        private void AIRaised(ref int botChips, ref bool sTurn, Label sStatus)
        {
            botChips -= Convert.ToInt32(this.raise);
            sStatus.Text = "Raise " + this.raise;
            textBoxPot.Text = (int.Parse(textBoxPot.Text) + Convert.ToInt32(this.raise)).ToString();
            this.pokerCall = Convert.ToInt32(this.raise);
            raising = true;
            sTurn = false;
        }

        //вика се от  AIHP AIPH, AISmooth  this.pokerCall <= AIRoundNumber(botChips, n) числото е различно има и математика
        private static double AIRoundNumber(int botChips, int n)
        {
            double a = Math.Round((botChips / n) / 100d, 0) * 100;
            return a;
        }

        #endregion

        #region UI
        private async void timer_Tick(object sender, object e)
        {
            if (progressBarTimer.Value <= 0)
            {
                this.playerGameEnded = true;
                await Turns();
            }
            if (t > 0)
            {
                t--;
                progressBarTimer.Value = (t / 6) * 100;
            }
        }
        private void Update_Tick(object sender, object e)
        {
            if (this.playerChips <= 0)
            {
                textBoxPlayerChips.Text = "Chips : 0";
            }
            if (bot1Chips <= 0)
            {
                textBoxBot1Chips.Text = "Chips : 0";
            }
            if (bot2Chips <= 0)
            {
                textBoxBot2Chips.Text = "Chips : 0";
            }
            if (bot3Chips <= 0)
            {
                textBoxBot3Chips.Text = "Chips : 0";
            }
            if (bot4Chips <= 0)
            {
                textBoxBot4Chips.Text = "Chips : 0";
            }
            if (bot5Chips <= 0)
            {
                textBoxBot5Chips.Text = "Chips : 0";
            }
            textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
            textBoxBot1Chips.Text = "Chips : " + bot1Chips.ToString();
            textBoxBot2Chips.Text = "Chips : " + bot2Chips.ToString();
            textBoxBot3Chips.Text = "Chips : " + bot3Chips.ToString();
            textBoxBot4Chips.Text = "Chips : " + bot4Chips.ToString();
            textBoxBot5Chips.Text = "Chips : " + bot5Chips.ToString();
            if (this.playerChips <= 0)
            {
                Pturn = false;
                this.playerGameEnded = true;
                buttonCall.Enabled = false;
                buttonRaise.Enabled = false;
                buttonFold.Enabled = false;
                buttonCheck.Enabled = false;
            }
            if (up > 0)
            {
                up--;
            }
            if (this.playerChips >= this.pokerCall)
            {
                buttonCall.Text = "Call " + this.pokerCall.ToString();
            }
            else
            {
                buttonCall.Text = "All in";
                buttonRaise.Enabled = false;
            }
            if (this.pokerCall > 0)
            {
                buttonCheck.Enabled = false;
            }
            if (this.pokerCall <= 0)
            {
                buttonCheck.Enabled = true;
                buttonCall.Text = "Call";
                buttonCall.Enabled = false;
            }
            if (this.playerChips <= 0)
            {
                buttonRaise.Enabled = false;
            }
            int parsedValue;

            if (textBoxRaise.Text != "" && int.TryParse(textBoxRaise.Text, out parsedValue))
            {
                if (this.playerChips <= int.Parse(textBoxRaise.Text))
                {
                    buttonRaise.Text = "All in";
                }
                else
                {
                    buttonRaise.Text = "Raise";
                }
            }
            if (this.playerChips < this.pokerCall)
            {
                buttonRaise.Enabled = false;
            }
        }
        private async void ButtonFold_Click(object sender, EventArgs e)
        {
            //TODO To be implemented button click for fold
        }
        private async void ButtonCheck_Click(object sender, EventArgs e)
        {
            if (this.pokerCall <= 0)
            {
                Pturn = false;
                labelPlayerStatus.Text = "Check";
            }
            else
            {
                //pStatus.Text = "All in " + Chips;

                buttonCheck.Enabled = false;
            }
            await Turns();
        }
        private async void ButtonCall_Click(object sender, EventArgs e)
        {
            Rules(0, 1, "Player", ref this.playerHandMultiplier, ref this.playerCardPower, this.playerGameEnded);

            if (this.playerChips >= this.pokerCall)
            {
                this.playerChips -= this.pokerCall;
                textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();

                if (textBoxPot.Text != "")
                {
                    textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
                }
                else
                {
                    textBoxPot.Text = this.pokerCall.ToString();
                }

                Pturn = false;
                labelPlayerStatus.Text = "Call " + this.pokerCall;
                this.playerCall = this.pokerCall;
            }
            else if (this.playerChips <= this.pokerCall && this.pokerCall > 0)
            {
                textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.playerChips).ToString();
                labelPlayerStatus.Text = "All in " + this.playerChips;
                this.playerChips = 0;
                textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
                Pturn = false;
                buttonFold.Enabled = false;
                this.playerCall = this.playerChips;
            }
            await Turns();
        }
        private async void ButtonRaise_Click(object sender, EventArgs e)
        {
            Rules(0, 1, "Player", ref this.playerHandMultiplier, ref this.playerCardPower, this.playerGameEnded);
            int parsedValue;
            if (textBoxRaise.Text != "" && int.TryParse(textBoxRaise.Text, out parsedValue))
            {
                if (this.playerChips > this.pokerCall)
                {
                    if (this.raise * 2 > int.Parse(textBoxRaise.Text))
                    {
                        textBoxRaise.Text = (this.raise * 2).ToString();
                        MessageBox.Show("You must raise atleast twice as the PokerHandMultiplier raise !");
                        return;
                    }
                    else
                    {
                        if (this.playerChips >= int.Parse(textBoxRaise.Text))
                        {
                            this.pokerCall = int.Parse(textBoxRaise.Text);
                            this.raise = int.Parse(textBoxRaise.Text);
                            labelPlayerStatus.Text = "Raise " + this.pokerCall.ToString();
                            textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
                            buttonCall.Text = "Call";
                            this.playerChips -= int.Parse(textBoxRaise.Text);
                            raising = true;
                            lastBotPlayed = 0;
                            playerRaise = Convert.ToInt32(this.raise);
                        }
                        else
                        {
                            this.pokerCall = this.playerChips;
                            this.raise = this.playerChips;
                            textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.playerChips).ToString();
                            labelPlayerStatus.Text = "Raise " + this.pokerCall.ToString();
                            this.playerChips = 0;
                            raising = true;
                            lastBotPlayed = 0;
                            playerRaise = Convert.ToInt32(this.raise);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("This is a number only field");
                return;
            }
            Pturn = false;
            await Turns();
        }
        private void ButtonAddChips_Click(object sender, EventArgs e)
        {
            if (textBoxAddChips.Text == "") { }
            else
            {
                this.playerChips += int.Parse(textBoxAddChips.Text);
                bot1Chips += int.Parse(textBoxAddChips.Text);
                bot2Chips += int.Parse(textBoxAddChips.Text);
                bot3Chips += int.Parse(textBoxAddChips.Text);
                bot4Chips += int.Parse(textBoxAddChips.Text);
                bot5Chips += int.Parse(textBoxAddChips.Text);
            }
            textBoxPlayerChips.Text = "Chips : " + this.playerChips.ToString();
        }
        private void ButtonChooseBlind_Click(object sender, EventArgs e)
        {
            textBoxBigBlind.Text = bb.ToString();
            textBoxSmallBlind.Text = sb.ToString();
            if (textBoxBigBlind.Visible == false)
            {
                textBoxBigBlind.Visible = true;
                textBoxSmallBlind.Visible = true;
                buttonBigBlind.Visible = true;
                buttonSmallBlind.Visible = true;
            }
            else
            {
                textBoxBigBlind.Visible = false;
                textBoxSmallBlind.Visible = false;
                buttonBigBlind.Visible = false;
                buttonSmallBlind.Visible = false;
            }
        }
        private void ButtonSmallBlind_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (textBoxSmallBlind.Text.Contains(",") || textBoxSmallBlind.Text.Contains("."))
            {
                MessageBox.Show("The Small Blind can be only round number !");
                textBoxSmallBlind.Text = sb.ToString();
                return;
            }
            if (!int.TryParse(textBoxSmallBlind.Text, out parsedValue))
            {
                MessageBox.Show("This is a number only field");
                textBoxSmallBlind.Text = sb.ToString();
                return;
            }
            if (int.Parse(textBoxSmallBlind.Text) > 100000)
            {
                MessageBox.Show("The maximum of the Small Blind is 100 000 $");
                textBoxSmallBlind.Text = sb.ToString();
            }
            if (int.Parse(textBoxSmallBlind.Text) < 250)
            {
                MessageBox.Show("The minimum of the Small Blind is 250 $");
            }
            if (int.Parse(textBoxSmallBlind.Text) >= 250 && int.Parse(textBoxSmallBlind.Text) <= 100000)
            {
                sb = int.Parse(textBoxSmallBlind.Text);
                MessageBox.Show("The changes have been saved ! They will become available the next hand you play. ");
            }
        }
        private void ButtonBigBlind_Click(object sender, EventArgs e)
        {
            int parsedValue;
            if (textBoxBigBlind.Text.Contains(",") || textBoxBigBlind.Text.Contains("."))
            {
                MessageBox.Show("The Big Blind can be only round number !");
                textBoxBigBlind.Text = bb.ToString();
                return;
            }
            if (!int.TryParse(textBoxSmallBlind.Text, out parsedValue))
            {
                MessageBox.Show("This is a number only field");
                textBoxSmallBlind.Text = bb.ToString();
                return;
            }
            if (int.Parse(textBoxBigBlind.Text) > 200000)
            {
                MessageBox.Show("The maximum of the Big Blind is 200 000");
                textBoxBigBlind.Text = bb.ToString();
            }
            if (int.Parse(textBoxBigBlind.Text) < 500)
            {
                MessageBox.Show("The minimum of the Big Blind is 500 $");
            }
            if (int.Parse(textBoxBigBlind.Text) >= 500 && int.Parse(textBoxBigBlind.Text) <= 200000)
            {
                bb = int.Parse(textBoxBigBlind.Text);
                MessageBox.Show("The changes have been saved ! They will become available the next hand you play. ");
            }
        }
        private void Layout_Change(object sender, LayoutEventArgs e)
        {
            width = this.Width;
            height = this.Height;
        }
        #endregion
    }
}