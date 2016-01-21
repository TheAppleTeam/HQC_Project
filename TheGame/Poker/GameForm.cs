namespace Poker
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Poker.GameObjects.Cards;
    using Poker.GameObjects.Player;

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

        private IPlayer gamer;
        private IPlayer bot1;
        private IPlayer bot2;
        private IPlayer bot3;
        private IPlayer bot4;
        private IPlayer bot5;

        private IPlayer[] players = new IPlayer[6];

        private int pokerCall = InitialBigBlind;
        private int foldedPlayers = BotCount;

        private double type;
        private double rounds;
        
        private double raise;

        // is set TRUE when bot's chips (bot{num}Chips) goes <=0 

        private bool intsadded;
        private bool changed;

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

        List<WinningHand> winningCards = new List<WinningHand>();

        List<string> CheckWinners = new List<string>();
        List<int> ints = new List<int>();
        private bool restart = false;
        bool raising = false;
        Poker.WinningHand winningCard;

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
        int bb = InitialBigBlind, 
            sb = InitialSmallBlind, 
            up = 10000000, 
            turnCount = 0;
        #endregion

        public GameForm()
        {
            InitializeComponent();

            this.gamer = new Gamer(this.labelPlayerStatus);
            this.bot1 = new Bot("Bot 1",this.labelBot1Status);
            this.bot2 = new Bot("Bot 2", this.labelBot2Status);
            this.bot3 = new Bot("Bot 3", this.labelBot3Status);
            this.bot4 = new Bot("Bot 4", this.labelBot4Status);
            this.bot5 = new Bot("Bot 5", this.labelBot5Status);

            this.players[0] = this.gamer;
            this.players[1] = this.bot1;
            this.players[2] = this.bot2;
            this.players[3] = this.bot3;
            this.players[4] = this.bot4;
            this.players[5] = this.bot5;

            //bools.Add(PFturn); bools.Add(B1Fturn); bools.Add(players[2].GameEnded); bools.Add(players[3].GameEnded); bools.Add(players[4].GameEnded); bools.Add(players[5].GameEnded);
            this.pokerCall = InitialBigBlind;
            MaximizeBox = false;
            MinimizeBox = false;
            Updates.Start();
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
            textBoxPlayerChips.Text = "Chips : " + this.players[0].Chips.ToString();
            textBoxBot1Chips.Text = "Chips : " + this.players[1].Chips.ToString();
            textBoxBot2Chips.Text = "Chips : " + this.players[2].Chips.ToString();
            textBoxBot3Chips.Text = "Chips : " + this.players[3].Chips.ToString();
            textBoxBot4Chips.Text = "Chips : " + this.players[4].Chips.ToString();
            textBoxBot5Chips.Text = "Chips : " + this.players[5].Chips.ToString();
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
            this.bools.Add(this.players[0].GameEnded);
            this.bools.Add(this.players[1].GameEnded);
            this.bools.Add(this.players[2].GameEnded);
            this.bools.Add(this.players[3].GameEnded);
            this.bools.Add(this.players[4].GameEnded);
            this.bools.Add(this.players[5].GameEnded);

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
            if (this.players[1].Chips <= 0)
            {
                this.players[1].GameEnded = true;
                this.HideBotCards(1);
            }
            else
            {
                this.players[1].GameEnded = false;
                this.DisplayBotCards(1, cardIndex);
            }

            if (this.players[2].Chips <= 0)
            {
                this.players[2].GameEnded = true;
                this.HideBotCards(2);
            }
            else
            {
                this.players[2].GameEnded = false;
                this.DisplayBotCards(2, cardIndex);
            }

            if (this.players[3].Chips <= 0)
            {
                this.players[3].GameEnded = true;
                this.HideBotCards(3);
            }
            else
            {
                this.DisplayBotCards(3, cardIndex);
            }

            if (this.players[4].Chips <= 0)
            {
                this.players[4].GameEnded = true;
                this.HideBotCards(4);
            }
            else
            {
                this.DisplayBotCards(4, cardIndex);
            }

            if (this.players[5].Chips <= 0)
            {
                this.players[5].GameEnded = true;
                this.HideBotCards(5);
            }
            else
            {
                this.players[5].GameEnded = false;
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
                if (this.players[1].Chips > 0)
                {
                    anchorPointHorizontalPosition = 15;
                    anchorPointVerticalPosition = 420;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 4 && cardIndex < 6)
            {
                if (this.players[2].Chips > 0)
                {
                    anchorPointHorizontalPosition = 75;
                    anchorPointVerticalPosition = 65;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 6 && cardIndex < 8)
            {
                if (this.players[3].Chips > 0)
                {
                    anchorPointHorizontalPosition = 590;
                    anchorPointVerticalPosition = 25;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 8 && cardIndex < 10)
            {
                if (this.players[4].Chips > 0)
                {
                    anchorPointHorizontalPosition = 1115;
                    anchorPointVerticalPosition = 65;

                    this.FillInCardsControls(cardIndex, anchorPointHorizontalPosition, anchorPointVerticalPosition);
                }
            }

            if (cardIndex >= 10 && cardIndex < 12)
            {
                if (this.players[5].Chips > 0)
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

            this.FillInPlayerPanel(cardIndex, this.players[0].CardsPanel);
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
            //TODO note:open all cards
            Bitmap backImage = new Bitmap("Assets\\Back\\Back.png");

            this.cardHolder[cardIndex].Tag = this.dealtCardsNumbers[cardIndex];
            this.cardHolder[cardIndex].Image = this.DeckImages[cardIndex];
            //if (cardIndex < 2 )
            //{
            //    this.cardHolder[cardIndex].Image = this.DeckImages[cardIndex];
            //}
            //else
            //{
            //    this.cardHolder[cardIndex].Image = backImage;
            //}
            
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
            if (!this.players[0].GameEnded)
            {
                if (players[0].Turn)
                {
                    this.CallAllPlayerActionsOnTurn(this.players[0]);
                }
            }

            if (this.players[0].GameEnded || !players[0].Turn)
            {
                await AllIn();
                if (this.players[0].GameEnded && !this.players[0].Folded)
                {
                    if (buttonCall.Text.Contains("All in") == false || buttonRaise.Text.Contains("All in") == false)
                    {
                        RemovePlayerFromTheGame(0);
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
                this.players[1].Turn = true;

                
                for (int i = 1; i < this.players.Length; i++)
                {
                    if (!this.players[i].GameEnded)
                    {
                        if (this.players[i].Turn)
                        {
                            this.CallAllBotActionsOnTheirTurn(this.players[i]);
                    }
                }
                    if (this.players[i].GameEnded && !this.players[i].Folded)
                {
                    //TODO _ActivePlayers rename
                        this.RemovePlayerFromTheGame(i);
                }
                    if (this.players[i].GameEnded || !this.players[i].Turn)
                {
                        await CheckRaise(i, i);
                        if (i + 1 == this.players.Length)
                        {
                            this.players[0].Turn = true;
                }
                        else
                {
                            this.players[i + 1].Turn = true;
                    }
                }
                }
                
                if (this.players[0].GameEnded && !this.players[0].Folded)
                {
                    if (buttonCall.Text.Contains("All in") == false || buttonRaise.Text.Contains("All in") == false)
                {
                        RemovePlayerFromTheGame(0);
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

        private void RemovePlayerFromTheGame(int playerIndex)
                {
            this.bools.RemoveAt(playerIndex);
            this.bools.Insert(playerIndex, null);
            this.maxPlayersLeftCount--;
            this.players[playerIndex].Folded = true;
                }

        private void CallAllPlayerActionsOnTurn(IPlayer player)
                {
            this.FixCall(player, 1);
            //MessageBox.Show("Player's Turn");
            this.progressBarTimer.Visible = true;
            this.progressBarTimer.Value = 1000;

            this.t = 60;
            this.up = 10000000;

            this.timer.Start();
            this.buttonRaise.Enabled = true;
            this.buttonCall.Enabled = true;
            this.buttonFold.Enabled = true;
            //buttonRaise.Enabled = true;
            //buttonRaise.Enabled = true;
            this.turnCount++;

            this.FixCall(player, 2);
                }
        /// <summary>
        /// Calls all actions that must be called if it is bot turn
        /// </summary>
        private void CallAllBotActionsOnTheirTurn(IPlayer player)
                {
            var botNumber = player.Name.Split(' ')[1]
                .Trim();
            ///Bot 1 -> 0 ,Bot 2 ->1 etc.. 
            /// used in this.Ai(...)
            var botPresentedAsNumber = int.Parse(botNumber) - 1;

            var firstCardNumeration = 0;
            var secondCardNumeration = 0;

            switch (botNumber)
                    {
                case "1":
                    firstCardNumeration = 2;secondCardNumeration =3;
                    break;
                case "2":
                    firstCardNumeration = 4; secondCardNumeration = 5;
                    break;
                case "3":
                    firstCardNumeration = 6; secondCardNumeration = 7;
                    break;
                case "4":
                    firstCardNumeration = 8; secondCardNumeration = 9;
                    break;
                case "5":
                    firstCardNumeration = 10; secondCardNumeration = 11;
                    break;
                    }

            this.FixCall(player, 1);
            this.FixCall(player, 2);

            this.Rules(firstCardNumeration, secondCardNumeration, player.Name, player.PokerHandMultiplier, player.CardPower, player.GameEnded);

            MessageBox.Show("Bot  " + botNumber + @"'s Turn");
            this.AI(firstCardNumeration, secondCardNumeration, player);

            this.turnCount++;
            this.lastBotPlayed = int.Parse(botNumber);
            player.Turn = false;
            //TODO must be implemented

            var nextBotIndex = int.Parse(botNumber)+1;
            if (nextBotIndex<=5)
                {
                this.players[nextBotIndex].Turn = true;
                }
            else
                {
                nextBotIndex = 0;
                this.players[nextBotIndex].Turn = true;
                }
                    }

        void Rules(int firstCard, int secondCard, string playerName, double pokerHandMultiplier, double power, bool foldedTurn)
        {
            //if (firstCard == 0 && secondCard == 1)
            //{
            //}

            if (!foldedTurn || firstCard == 0 && secondCard == 1 && labelPlayerStatus.Text.Contains("Fold") == false)
            {
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
                    //int parse = int.Parse(cardHolder[firstCard].Tag.ToString());
                    //int parse1 = int.Parse(cardHolder[secondCard].Tag.ToString());

                    //if (this.dealtCardsNumbers[i] == parse && this.dealtCardsNumbers[i + 1] == parse1)
                //{

                //Pair from Hand PokerHandMultiplier = 1

                    //rPairFromHand(pokerHandMultiplier, power, i);

                    //#region Pair or Two Pair from Table PokerHandMultiplier = 2 || 0
                    rPairTwoPair(pokerHandMultiplier, power, i, cardsOnTable);
                    //#endregion

                    //#region Two Pair PokerHandMultiplier = 2
                    //rTwoPair(pokerHandMultiplier, power, i);
                    //#endregion

                    //#region Three of a kind PokerHandMultiplier = 3
                    //rThreeOfAKind(pokerHandMultiplier, power, cardsOnTableWithPlayerCards);
                    //#endregion

                    //#region Straight PokerHandMultiplier = 4
                    //rStraight(pokerHandMultiplier, power, cardsOnTableWithPlayerCards);
                    //#endregion

                    //#region Flush PokerHandMultiplier = 5 || 5.5
                    //rFlush(pokerHandMultiplier, power, cardsOnTable, i);
                    //#endregion

                    //#region Full House PokerHandMultiplier = 6
                    //rFullHouse(pokerHandMultiplier, power, ref done, cardsOnTableWithPlayerCards);
                    //#endregion

                    //#region Four of a Kind PokerHandMultiplier = 7
                    //rFourOfAKind(pokerHandMultiplier, power, cardsOnTableWithPlayerCards);
                    //#endregion

                    //#region Straight Flush PokerHandMultiplier = 8 || 9
                    //rStraightFlush(pokerHandMultiplier, power, cardsOfClubs, cardsOfDiamonds, cardsOfHearts, cardsOfSpades);
                    //#endregion

                    //#region High Card PokerHandMultiplier = -1
                    //rHighCard(pokerHandMultiplier, power, i);
                    //#endregion
                //}
            }
            }
        }

        //Stani
        private void rStraightFlush(double PokerHandMultiplier, double Power, int[] cardsOfClubs, int[] cardsOfDiamonds, int[] cardsOfHearts, int[] cardsOfSpades)
        {
            if (PokerHandMultiplier >= -1)
            {
                if (cardsOfClubs.Length >= 5)
                {
                    if (cardsOfClubs[0] + 4 == cardsOfClubs[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfClubs.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfClubs[0] == 0 && cardsOfClubs[1] == 9 && cardsOfClubs[2] == 10 && cardsOfClubs[3] == 11 && cardsOfClubs[0] + 12 == cardsOfClubs[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfClubs.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (cardsOfDiamonds.Length >= 5)
                {
                    if (cardsOfDiamonds[0] + 4 == cardsOfDiamonds[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfDiamonds.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfDiamonds[0] == 0 && cardsOfDiamonds[1] == 9 && cardsOfDiamonds[2] == 10 && cardsOfDiamonds[3] == 11 && cardsOfDiamonds[0] + 12 == cardsOfDiamonds[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfDiamonds.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (cardsOfHearts.Length >= 5)
                {
                    if (cardsOfHearts[0] + 4 == cardsOfHearts[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfHearts.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfHearts[0] == 0 && cardsOfHearts[1] == 9 && cardsOfHearts[2] == 10 && cardsOfHearts[3] == 11 && cardsOfHearts[0] + 12 == cardsOfHearts[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfHearts.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
                if (cardsOfSpades.Length >= 5)
                {
                    if (cardsOfSpades[0] + 4 == cardsOfSpades[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = (cardsOfSpades.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 8 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (cardsOfSpades[0] == 0 && cardsOfSpades[1] == 9 && cardsOfSpades[2] == 10 && cardsOfSpades[3] == 11 && cardsOfSpades[0] + 12 == cardsOfSpades[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfSpades.Max()) / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 9 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rFourOfAKind(double PokerHandMultiplier, double Power, int[] Straight)
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
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 7 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                    if (Straight[j] / 4 == 0 && Straight[j + 1] / 4 == 0 && Straight[j + 2] / 4 == 0 && Straight[j + 3] / 4 == 0)
                    {
                        PokerHandMultiplier = 7;
                        Power = 13 * 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 7 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rFullHouse(double PokerHandMultiplier, double Power, ref bool done, int[] Straight)
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
                                this.winningCards.Add(new WinningHand() { Power = Power, Current = 6 });
                                this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                                break;
                            }
                            if (fh.Max() / 4 > 0)
                            {
                                PokerHandMultiplier = 6;
                                Power = fh.Max() / 4 * 2 + PokerHandMultiplier * 100;
                                this.winningCards.Add(new WinningHand() { Power = Power, Current = 6 });
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
        private void rFlush(double PokerHandMultiplier, double Power, int[] cardsOnTable, int index)
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

        private void rStraight(double PokerHandMultiplier, double Power, int[] Straight)
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
                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 4 });
                            this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        }
                        else
                        {
                            PokerHandMultiplier = 4;
                            Power = op[j + 4] + PokerHandMultiplier * 100;
                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 4 });
                            this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                        }
                    }
                    if (op[j] == 0 && op[j + 1] == 9 && op[j + 2] == 10 && op[j + 3] == 11 && op[j + 4] == 12)
                    {
                        PokerHandMultiplier = 4;
                        Power = 13 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 4 });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current).ThenByDescending(op1 => op1.Power).First();
                    }
                }
            }
        }
        private void rThreeOfAKind(double PokerHandMultiplier, double Power, int[] cardsOnTableWithPlayerCards)
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
                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 3 });
                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                        }
                        else
                        {
                            PokerHandMultiplier = 3;
                            Power = fh[0] / 4 + fh[1] / 4 + fh[2] / 4 + PokerHandMultiplier * 100;
                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 3 });
                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                        }
                    }
                }
            }
        }
        private void rTwoPair(double PokerHandMultiplier, double Power, int index)
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
                                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index + 1] / 4 == 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = 13 * 4 + (this.dealtCardsNumbers[index] / 4) * 2 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index + 1] / 4 != 0 && this.dealtCardsNumbers[index] / 4 != 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[index] / 4) * 2 + (this.dealtCardsNumbers[index + 1] / 4) * 2 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 2 });
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
        private void rPairTwoPair(double PokerHandMultiplier, double Power, int index, int[] cardsOnTable)
        {
            bool pairFound = false;

            if (this.dealtCardsNumbers[index] % 2 == 0 &&
                this.dealtCardsNumbers[index] == this.dealtCardsNumbers[index + 1])
            {
                return;
            }

            foreach (var card in cardsOnTable)
            {
                if (this.GetCardIndex(index) != this.GetCardIndex(card))
                {
                    continue;
                }

                if (pairFound)
                {
                    break;
                }

                pairFound = true;
            }

            if (!(PokerHandMultiplier >= -1))
            {
                return;
            }

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
                                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index] / 4 == 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[index + 1] / 4) * 2 + 13 * 4 + PokerHandMultiplier * 100;
                                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index + 1] / 4 != 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[tc] / 4) * 2 + (this.dealtCardsNumbers[index + 1] / 4) * 2 + PokerHandMultiplier * 100;
                                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 2 });
                                            this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                        }
                                        if (this.dealtCardsNumbers[index] / 4 != 0)
                                        {
                                            PokerHandMultiplier = 2;
                                            Power = (this.dealtCardsNumbers[tc] / 4) * 2 + (this.dealtCardsNumbers[index] / 4) * 2 + PokerHandMultiplier * 100;
                                        this.winningCards.Add(new WinningHand() { Power = Power, Current = 2 });
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
                                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 1 });
                                                this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                            else
                                            {
                                                PokerHandMultiplier = 0;
                                                Power = this.dealtCardsNumbers[tc] / 4 + this.dealtCardsNumbers[index] / 4 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 1 });
                                                this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                        }
                                        else
                                        {
                                            if (this.dealtCardsNumbers[tc] / 4 == 0)
                                            {
                                                PokerHandMultiplier = 0;
                                                Power = 13 + this.dealtCardsNumbers[index + 1] + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 1 });
                                                this.winningCard = this.winningCards.OrderByDescending(op => op.Current).ThenByDescending(op => op.Power).First();
                                            }
                                            else
                                            {
                                                PokerHandMultiplier = 0;
                                                Power = this.dealtCardsNumbers[tc] / 4 + this.dealtCardsNumbers[index + 1] / 4 + PokerHandMultiplier * 100;
                                            this.winningCards.Add(new WinningHand() { Power = Power, Current = 1 });
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
        private void rPairFromHand(double PokerHandMultiplier, double power, int index)
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

            //for (int tc = 16; tc >= 12; tc--)
            //{
            //    if (this.GetCardIndex(index + 1) == this.GetCardIndex(tc))
            //    {
            //        if (!msgbox)
            //        {
            //            PokerHandMultiplier = 1;

            //            if (this.GetCardIndex(index + 1) == 0)
            //            {
            //                power = 13 * 4 + this.GetCardIndex(index) + PokerHandMultiplier * 100;
            //            }
            //            else
            //            {
            //                power = (this.GetCardIndex(index + 1)) * 4 + this.GetCardIndex(index) + PokerHandMultiplier * 100;
            //            }

            //            this.FindWinnigCard(PokerHandMultiplier, power);

            //        }
            //        msgbox = true;
            //    }

            //    if (this.GetCardIndex(index + 1) != this.GetCardIndex(tc))
            //    {
            //        continue;
            //    }

            //    if (!msgbox)
            //    {
            //        PokerHandMultiplier = 1;

            //        if (this.GetCardIndex(index) == 0)
            //        {
            //            power = 13 * 4 + this.GetCardIndex(index + 1) + PokerHandMultiplier * 100;
            //        }
            //        else
            //        {
            //            power = (this.GetCardIndex(tc)) * 4 + this.GetCardIndex(index + 1) + PokerHandMultiplier * 100;
            //        }

            //        this.FindWinnigCard(PokerHandMultiplier, power);
            //    }
            //    msgbox = true;
            //}
        }

        private void rHighCard(double PokerHandMultiplier, double Power, int index)
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

            if (this.GetCardIndex(index) == 0 )
            {
                PokerHandMultiplier = -1;
                double power = 13 ;
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
            this.winningCards.Add(new WinningHand() { Power = power, Current = PokerHandMultiplier });
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
                        this.players[0].Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxPlayerChips.Text = this.players[0].Chips.ToString();
                        //pPanel.Visible = true;

                    }
                    if (CheckWinners.Contains("Bot 1"))
                    {
                        this.players[1].Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot1Chips.Text = this.players[1].Chips.ToString();
                        //b1Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 2"))
                    {
                        this.players[2].Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot2Chips.Text = this.players[2].Chips.ToString();
                        //b2Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 3"))
                    {
                        this.players[3].Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot3Chips.Text = this.players[3].Chips.ToString();
                        //b3Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 4"))
                    {
                        this.players[4].Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot4Chips.Text = this.players[4].Chips.ToString();
                        //b4Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 5"))
                    {
                        this.players[5].Chips += int.Parse(textBoxPot.Text) / winnersCount;
                        textBoxBot5Chips.Text = this.players[5].Chips.ToString();
                        //b5Panel.Visible = true;
                    }
                    //await Finish(1);
                }

                if (winnersCount == 1)
                {
                    if (CheckWinners.Contains("Player"))
                    {
                        this.players[0].Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //pPanel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 1"))
                    {
                        this.players[1].Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b1Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 2"))
                    {
                        this.players[2].Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b2Panel.Visible = true;

                    }
                    if (CheckWinners.Contains("Bot 3"))
                    {
                        this.players[3].Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b3Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 4"))
                    {
                        this.players[4].Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b4Panel.Visible = true;
                    }
                    if (CheckWinners.Contains("Bot 5"))
                    {
                        this.players[5].Chips += int.Parse(textBoxPot.Text);
                        //await Finish(1);
                        //b5Panel.Visible = true;
                    }
                }
            }
        }
        async Task CheckRaise(int currentTurn, int raiseTurn)
        {
            if (this.raising)
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
                        if (!this.players[0].GameEnded)
                            labelPlayerStatus.Text = "";
                        if (!this.players[1].GameEnded)
                            labelBot1Status.Text = "";
                        if (!this.players[2].GameEnded)
                            labelBot2Status.Text = "";
                        if (!this.players[3].GameEnded)
                            labelBot3Status.Text = "";
                        if (!this.players[4].GameEnded)
                            labelBot4Status.Text = "";
                        if (!this.players[5].GameEnded)
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
                        this.players[0].Call = 0;
                        players[0].Raise = 0;
                        this.players[1].Call = 0;
                        players[1].Raise = 0;
                        this.players[2].Call = 0;

                        players[2].Raise = 0;
                        players[3].Call = 0;
                        players[3].Raise = 0;
                        players[4].Call = 0;
                        players[4].Raise = 0;
                        players[5].Call = 0;
                        players[5].Raise = 0;
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
                        this.players[0].Call = 0; players[0].Raise = 0;
                        this.players[1].Call = 0; players[1].Raise = 0;
                        this.players[2].Call = 0; players[2].Raise = 0;
                        players[3].Call = 0; players[3].Raise = 0;
                        players[4].Call = 0; players[4].Raise = 0;
                        players[5].Call = 0; players[5].Raise = 0;
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
                        this.players[0].Call = 0; players[0].Raise = 0;
                        this.players[1].Call = 0; players[1].Raise = 0;
                        this.players[2].Call = 0; players[2].Raise = 0;
                        players[3].Call = 0; players[3].Raise = 0;
                        players[4].Call = 0; players[4].Raise = 0;
                        players[5].Call = 0; players[5].Raise = 0;
                    }
                }
            }
            if (rounds == End && maxPlayersLeftCount == 6)
            {
                string fixedLast = "qwerty";
                if (!labelPlayerStatus.Text.Contains("Fold"))
                {
                    fixedLast = "Player";
                    Rules(0, 1, "Player", this.players[0].PokerHandMultiplier, this.players[0].CardPower, this.players[0].GameEnded);
                }
                if (!labelBot1Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 1";
                    Rules(2, 3, "Bot 1", this.players[1].PokerHandMultiplier, this.players[1].CardPower, this.players[1].GameEnded);
                }
                if (!labelBot2Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 2";
                    Rules(4, 5, "Bot 2", this.players[2].PokerHandMultiplier, this.players[2].CardPower, this.players[2].GameEnded);
                }
                if (!labelBot3Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 3";
                    Rules(6, 7, "Bot 3", this.players[3].PokerHandMultiplier, this.players[3].CardPower, this.players[3].GameEnded);
                }
                if (!labelBot4Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 4";
                    Rules(8, 9, "Bot 4", this.players[4].PokerHandMultiplier, this.players[4].CardPower, this.players[4].GameEnded);
                }
                if (!labelBot5Status.Text.Contains("Fold"))
                {
                    fixedLast = "Bot 5";
                    Rules(10, 11, "Bot 5", this.players[5].PokerHandMultiplier, this.players[5].CardPower, this.players[5].GameEnded);
                }
                Winner(this.players[0].PokerHandMultiplier, this.players[0].CardPower, "Player", this.players[0].Chips, fixedLast);
                Winner(this.players[1].PokerHandMultiplier, this.players[1].CardPower, "Bot 1", this.players[1].Chips, fixedLast);
                Winner(this.players[2].PokerHandMultiplier, this.players[2].CardPower, "Bot 2", this.players[2].Chips, fixedLast);
                Winner(this.players[3].PokerHandMultiplier, this.players[3].CardPower, "Bot 3", this.players[3].Chips, fixedLast);
                Winner(this.players[4].PokerHandMultiplier, this.players[4].CardPower, "Bot 4", this.players[4].Chips, fixedLast);
                Winner(this.players[5].PokerHandMultiplier, this.players[5].CardPower, "Bot 5", this.players[5].Chips, fixedLast);
                restart = true;
                players[0].Turn = true;
                this.players[0].GameEnded = false;
                this.players[1].GameEnded = false;
                this.players[2].GameEnded = false;
                this.players[3].GameEnded = false;
                this.players[4].GameEnded = false;
                this.players[5].GameEnded = false;
                if (this.players[0].Chips <= 0)
                {
                    AddChips f2 = new AddChips();
                    f2.ShowDialog();
                    if (f2.a != 0)
                    {
                        this.players[0].Chips = f2.a;
                        this.players[1].Chips += f2.a;
                        this.players[2].Chips += f2.a;
                        this.players[3].Chips += f2.a;
                        this.players[4].Chips += f2.a;
                        this.players[5].Chips += f2.a;
                        this.players[0].GameEnded = false;
                        players[0].Turn = true;
                        buttonRaise.Enabled = true;
                        buttonFold.Enabled = true;
                        buttonCheck.Enabled = true;
                        buttonRaise.Text = "Raise";
                    }
                }
                this.players[0].CardsPanel.Visible = false;
                this.players[1].CardsPanel.Visible = false;
                this.players[2].CardsPanel.Visible = false;
                this.players[3].CardsPanel.Visible = false;
                this.players[4].CardsPanel.Visible = false;
                this.players[5].CardsPanel.Visible = false;

                this.players[0].Call = 0; players[0].Raise = 0;
                this.players[1].Call = 0; players[1].Raise = 0;
                this.players[2].Call = 0; players[2].Raise = 0;
                players[3].Call = 0; players[3].Raise = 0;
                players[4].Call = 0; players[4].Raise = 0;
                players[5].Call = 0; players[5].Raise = 0;
                lastBotPlayed = 0;
                this.pokerCall = bb;
                this.raise = 0;
                this.ImageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
                bools.Clear();
                rounds = 0;
                this.players[0].CardPower = 0;
                this.players[0].PokerHandMultiplier = -1;
                type = 0;

                this.players[1].CardPower = 0;
                this.players[2].CardPower = 0;
                this.players[3].CardPower = 0;
                this.players[4].CardPower = 0;
                this.players[5].CardPower = 0;

                this.players[1].PokerHandMultiplier = -1;
                this.players[2].PokerHandMultiplier = -1;
                this.players[3].PokerHandMultiplier = -1;
                this.players[4].PokerHandMultiplier = -1;
                this.players[5].PokerHandMultiplier = -1;

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
        void FixCall(IPlayer player, int options)
        {
            if (this.rounds != 4)
            {
                if (options == 1)
                {
                    if (player.Label.Text.Contains("Raise"))
                    {
                        var changeRaise = player.Label.Text.Substring(6);
                        player.Raise = int.Parse(changeRaise);
                    }
                    if (player.Label.Text.Contains("Call"))
                    {
                        var changeCall = player.Label.Text.Substring(5);
                        player.Call = int.Parse(changeCall);
                    }
                    if (player.Label.Text.Contains("Check"))
                    {
                        player.Raise = 0;
                        player.Call = 0;
                    }
                }
                if (options == 2)
                {
                    if (player.Raise != this.raise && player.Raise <= this.raise)
                    {
                        this.pokerCall = Convert.ToInt32(this.raise) - player.Raise;
                    }
                    if (player.Call != this.pokerCall || player.Call <= this.pokerCall)
                    {
                        this.pokerCall = this.pokerCall - player.Call;
                    }
                    if (player.Raise == this.raise && this.raise > 0)
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
            if (this.players[0].Chips <= 0 && !intsadded)
            {
                if (labelPlayerStatus.Text.Contains("Raise"))
                {
                    ints.Add(this.players[0].Chips);
                    intsadded = true;
                }
                if (labelPlayerStatus.Text.Contains("Call"))
                {
                    ints.Add(this.players[0].Chips);
                    intsadded = true;
                }
            }
            intsadded = false;
            if (this.players[1].Chips <= 0 && !this.players[1].GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(this.players[1].Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (this.players[2].Chips <= 0 && !this.players[2].GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(this.players[2].Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (this.players[3].Chips <= 0 && !this.players[3].GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(this.players[3].Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (this.players[4].Chips <= 0 && !this.players[4].GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(this.players[4].Chips);
                    intsadded = true;
                }
                intsadded = false;
            }
            if (this.players[5].Chips <= 0 && !this.players[5].GameEnded)
            {
                if (!intsadded)
                {
                    ints.Add(this.players[5].Chips);
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
                    this.players[0].Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.players[0].Chips.ToString();
                    this.players[0].CardsPanel.Visible = true;
                    MessageBox.Show("Player Wins");
                }
                if (index == 1)
                {
                    this.players[1].Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.players[1].Chips.ToString();
                    this.players[1].CardsPanel.Visible = true;
                    MessageBox.Show("Bot 1 Wins");
                }
                if (index == 2)
                {
                    this.players[2].Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.players[2].Chips.ToString();
                    this.players[2].CardsPanel.Visible = true;
                    MessageBox.Show("Bot 2 Wins");
                }
                if (index == 3)
                {
                    this.players[3].Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.players[3].Chips.ToString();
                    this.players[3].CardsPanel.Visible = true;
                    MessageBox.Show("Bot 3 Wins");
                }
                if (index == 4)
                {
                    this.players[4].Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.players[4].Chips.ToString();
                    this.players[4].CardsPanel.Visible = true;
                    MessageBox.Show("Bot 4 Wins");
                }
                if (index == 5)
                {
                    this.players[5].Chips += int.Parse(textBoxPot.Text);
                    textBoxPlayerChips.Text = this.players[5].Chips.ToString();
                    this.players[5].CardsPanel.Visible = true;
                    MessageBox.Show("Bot 5 Wins");
                }
                for (int j = 0; j <= 16; j++)
                {
                    this.cardHolder[j].Visible = false;
                }
                await Finish(1);
            }
            //TODO find what is intsadded
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
            this.players[0].CardsPanel.Visible = false;
            this.players[1].CardsPanel.Visible = false;
            this.players[2].CardsPanel.Visible = false;
            this.players[3].CardsPanel.Visible = false;
            this.players[4].CardsPanel.Visible = false;
            this.players[5].CardsPanel.Visible = false;

            this.pokerCall = bb; this.raise = 0;
            foldedPlayers = 5;
            type = 0;
            rounds = 0;
            this.players[1].CardPower = 0;
            this.players[2].CardPower = 0;
            this.players[3].CardPower = 0;
            this.players[4].CardPower = 0;
            this.players[5].CardPower = 0;
            this.players[0].CardPower = 0;

            this.players[0].PokerHandMultiplier = -1;
            this.raise = 0;
            this.players[1].PokerHandMultiplier = -1;
            this.players[2].PokerHandMultiplier = -1;
            this.players[3].PokerHandMultiplier = -1;
            this.players[4].PokerHandMultiplier = -1;
            this.players[5].PokerHandMultiplier = -1;

            this.players[1].Turn = false;
            this.players[2].Turn = false;
            this.players[3].Turn = false;
            this.players[4].Turn = false;
            this.players[5].Turn = false;
            this.players[1].GameEnded = false;
            this.players[2].GameEnded = false;
            this.players[3].GameEnded = false;
            this.players[4].GameEnded = false;
            this.players[5].GameEnded = false;
            this.players[0].Folded = false;
            this.players[1].Folded = false;
            this.players[2].Folded = false;
            this.players[3].Folded = false;
            this.players[4].Folded = false;
            this.players[5].Folded = false;
            this.players[0].GameEnded = false;
            players[0].Turn = true;
            restart = false;
            raising = false;
            this.players[0].Call = 0;
            this.players[1].Call = 0;
            this.players[2].Call = 0;

            players[3].Call = 0;
            players[4].Call = 0;
            players[5].Call = 0;
            players[0].Raise = 0;
            players[1].Raise = 0;
            players[2].Raise = 0;
            players[3].Raise = 0;
            players[4].Raise = 0;
            players[5].Raise = 0;

            height = 0;
            width = 0;
            winnersCount = 0;
            Flop = 1;
            Turn = 2;
            River = 3;
            End = 4;
            maxPlayersLeftCount = 6;
            lastBotPlayed = 123;
            raisedTurn = 1;
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

            if (this.players[0].Chips <= 0)
            {
                AddChips f2 = new AddChips();
                f2.ShowDialog();
                if (f2.a != 0)
                {
                    this.players[0].Chips = f2.a;
                    this.players[1].Chips += f2.a;
                    this.players[2].Chips += f2.a;
                    this.players[3].Chips += f2.a;
                    this.players[4].Chips += f2.a;
                    this.players[5].Chips += f2.a;
                    this.players[0].GameEnded = false;
                    players[0].Turn = true;
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
                Rules(0, 1, "Player", this.players[0].PokerHandMultiplier, this.players[0].CardPower, this.players[0].GameEnded);
            }
            if (!labelBot1Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 1";
                Rules(2, 3, "Bot 1", this.players[1].PokerHandMultiplier, this.players[1].CardPower, this.players[1].GameEnded);
            }
            if (!labelBot2Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 2";
                Rules(4, 5, "Bot 2", this.players[2].PokerHandMultiplier, this.players[2].CardPower, players[2].GameEnded);
            }
            if (!labelBot3Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 3";
                Rules(6, 7, "Bot 3", this.players[3].PokerHandMultiplier, this.players[3].CardPower, players[3].GameEnded);
            }
            if (!labelBot4Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 4";
                Rules(8, 9, "Bot 4", this.players[4].PokerHandMultiplier, this.players[4].CardPower, players[4].GameEnded);
            }
            if (!labelBot5Status.Text.Contains("Fold"))
            {
                fixedLast = "Bot 5";
                Rules(10, 11, "Bot 5", this.players[5].PokerHandMultiplier, this.players[5].CardPower, players[5].GameEnded);
            }
            Winner(this.players[0].PokerHandMultiplier, this.players[0].CardPower, "Player", this.players[0].Chips, fixedLast);
            Winner(this.players[1].PokerHandMultiplier, this.players[1].CardPower, "Bot 1", this.players[1].Chips, fixedLast);
            Winner(this.players[2].PokerHandMultiplier, this.players[2].CardPower, "Bot 2", this.players[2].Chips, fixedLast);
            Winner(this.players[3].PokerHandMultiplier, this.players[3].CardPower, "Bot 3", this.players[3].Chips, fixedLast);
            Winner(this.players[4].PokerHandMultiplier, this.players[4].CardPower, "Bot 4", this.players[4].Chips, fixedLast);
            Winner(this.players[5].PokerHandMultiplier, this.players[5].CardPower, "Bot 5", this.players[5].Chips, fixedLast);
        }

        // Plamena
        #region AI logic
        // AI   (     2,      3, ref    bot1Chips, ref this.players[1].Turn,ref this.players[1].GameEnded, labelBot1Status,       0, this.bot1CardPower, this.bot1HandMultiplier);
        /* wika se ot Turns : 
            if (!this.players[1].GameEnded) => if (this.players[1].Turn):
            if (!this.players[2].GameEnded) => if (this.players[2].Turn):
            if (!this.players[3].GameEnded) => if (this.bo3Turn) :
            if (!this.players[4].GameEnded) => if (this.players[4].Turn) :
            if (!players[5].GameEnded) =>  if (this.players[5].Turn) =>  
        => AI(2, 3, ref bot1Chips, ref this.players[1].Turn, ref this.players[1].GameEnded, labelBot1Status, 0, this.bot1CardPower, this.bot1HandMultiplier);
        => AI(4, 5, ref bot2Chips, ref this.players[2].Turn, ref this.players[2].GameEnded, labelBot2Status, 1, this.bot2CardPower, this.bot2HandMultiplier);
        => AI(6, 7, ref bot3Chips, ref this.players[3].Turn, ref this.players[3].GameEnded, labelBot3Status, 2, this.bot3CardPower, this.bot3HandMultiplier);
        => AI(8, 9, ref bot4Chips, ref this.players[4].Turn, ref this.players[4].GameEnded, labelBot4Status, 3, this.bot4CardPower, this.bot4HandMultiplier);
        => AI(10, 11, ref bot5Chips, ref this.players[5].Turn, ref  players[5].GameEnded, labelBot5Status, 4, this.bot5CardPower, this.bot5HandMultiplier);
         note: int name se polzwa samo pri wikaneto na smooth*/
        private void AI(int firstCardNumeration, int secondNumeration, IPlayer player)
        {
           if (!player.GameEnded)
            {
                switch (player.PokerHandMultiplier.ToString(CultureInfo.CreateSpecificCulture("en-GB")))
                {
                    case "-1": 
                        this.AIHighCard(player);
                        break;
                    case "0": this.AIPairTable(player); 
                        break;
                    case "1": this.AIPairHand(player);
                        break;
                    case "2": this.AITwoPair(player);
                        break;
                    case "3": this.AIThreeOfAKind(player);
                        break;
                    case "4": this.AIStraight(player); 
                        break;
                    case "5":
                    case "5.5": this.AIFlush(player); 
                        break;
                    case "6": this.AIFullHouse(player);
                        break;
                    case "7": this.AIFourOfAKind(player); 
                        break;
                    case "8":
                    case "9": this.AIStraightFlush(player);
                        break;
                }
           }

            if (player.GameEnded)
            {
                this.cardHolder[firstCardNumeration].Visible = false;
                this.cardHolder[secondNumeration].Visible = false;
            }
        }

        // Wika se ot AI this.AIHighCard(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika HP
        private void AIHighCard(IPlayer player)
        {
            this.AIHP(player,20,25);
        }

        // wika se ot AI this.AIPairTable(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika HP
        private void AIPairTable(IPlayer player)
        {
            this.AIHP(player, 16, 25);
        }
        
        // wika se ot AI this.AIPairHand(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
        private void AIPairHand(IPlayer player)
        {
            Random rPair = new Random();
            
            int rCall = rPair.Next(10, 16);
            
            int rRaise = rPair.Next(10, 13);

            if (player.CardPower <= 199 && player.CardPower >= 140)
            {
                this.AIPH(player,rCall, 6, rRaise);
            }
            if (player.CardPower <= 139 && player.CardPower >= 128)
            {
                this.AIPH(player, rCall, 7, rRaise);
            }
            if (player.CardPower < 128 && player.CardPower >= 101)
            {
                this.AIPH(player, rCall, 9, rRaise);
            }
        }

        // wika se ot AI this.AITwoPair(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
        private void AITwoPair(IPlayer player)
        {
            Random rPair = new Random();
            int rCall = rPair.Next(6, 11);
            int rRaise = rPair.Next(6, 11);
            if (player.CardPower <= 290 && player.CardPower >= 246)
            {
                this.AIPH(player, rCall, 3, rRaise);
            }
            if (player.CardPower <= 244 && player.CardPower >= 234)
            {
                this.AIPH(player, rCall, 4, rRaise);
            }
            if (player.CardPower < 234 && player.CardPower >= 201)
            {
                this.AIPH(player, rCall, 4, rRaise);
            }
        }

        // wika se ot AI this.AIThreeOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth
        private void AIThreeOfAKind(IPlayer player)
        {
            Random tk = new Random();
            int tCall = tk.Next(3, 7);
            int tRaise = tk.Next(4, 8);
            if (player.CardPower <= 390 && player.CardPower >= 330)
            {
                this.AISmooth(player, tCall, tRaise);
            }
            if (player.CardPower <= 327 && player.CardPower >= 321)//10  8
            {
                this.AISmooth(player, tCall, tRaise);
            }
            if (player.CardPower < 321 && player.CardPower >= 303)//7 2
            {
                this.AISmooth(player, tCall, tRaise);
            }
        }

        // wika se ot AI this.AIStraight(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIStraight(IPlayer player)
        {
            Random str = new Random();
            int sCall = str.Next(3, 6);
            int sRaise = str.Next(3, 8);
            if (player.CardPower <= 480 && player.CardPower >= 410)
            {
                this.AISmooth(player, sCall, sRaise);
            }
            if (player.CardPower <= 409 && player.CardPower >= 407)//10  8
            {
                this.AISmooth(player, sCall, sRaise);
            }
            if (player.CardPower < 407 && player.CardPower >= 404)
            {
                this.AISmooth(player, sCall, sRaise);
            }
        }

        // wika se ot AI this.AIFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIFlush(IPlayer player)
        {
            Random fsh = new Random();
            int fCall = fsh.Next(2, 6);
            int fRaise = fsh.Next(3, 7);
            this.AISmooth(player, fCall, fRaise);
        }

        // wika se ot AI : this.AIFullHouse(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth
        private void AIFullHouse(IPlayer player)
        {
            Random flh = new Random();
            int fhCall = flh.Next(1, 5);
            int fhRaise = flh.Next(2, 6);
            if (player.CardPower <= 626 && player.CardPower >= 620)
            {
                this.AISmooth(player, fhCall, fhRaise);
            }
            if (player.CardPower < 620 && player.CardPower >= 602)
            {
                this.AISmooth(player, fhCall, fhRaise);
            }
        }

        // wika se ot AI: this.AIFourOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIFourOfAKind(IPlayer player)
        {
            Random fk = new Random();
            int fkCall = fk.Next(1, 4);
            int fkRaise = fk.Next(2, 5);
            if (player.CardPower <= 752 && player.CardPower >= 704)
            {
                this.AISmooth(player, fkCall, fkRaise);
            }
        }

        // wika se ot AI: this.AIStraightFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth 
        private void AIStraightFlush(IPlayer player)
        {
            Random sf = new Random();
            int sfCall = sf.Next(1, 3);
            int sfRaise = sf.Next(1, 3);
            if (player.CardPower <= 913 && player.CardPower >= 804)
            {
                this.AISmooth(player, sfCall, sfRaise);
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
        private void AIHP(IPlayer player, int n, int n1)
        {
            Random rand = new Random();
            
            // staro:int rnd = rand.Next(1, 4); t.kato rnd = 4 nikyde ne se polzwa
            int rnd = rand.Next(1, 4);
            
            if (this.pokerCall <= 0)
            {
                // bota igrae CHeck
                this.AICheck(player);
            }

            if (this.pokerCall > 0)
            {
                if (rnd == 1)
                {
                    if (this.pokerCall <= AIRoundNumber(player.Chips, n))
                    {
                        this.AICall(player);
                    }
                    else
                    {
                        this.AIFold(player);
                    }
                }

                if (rnd == 2)
                {
                    if (this.pokerCall <= AIRoundNumber(player.Chips, n1))
                    {
                        this.AICall(player);
                    }
                    else
                    {
                        this.AIFold(player);
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
                    this.AIRaised(player);
                }
                else
                {
                    if (this.raise <= AIRoundNumber(player.Chips, n))
                    {
                        this.raise = this.pokerCall * 2;
                        this.AIRaised(player);
                    }
                    else
                    {
                        this.AIFold(player);
                    }
                }
            }

            if (player.Chips <= 0)
            {
                player.GameEnded = true;
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
        private void AIPH(IPlayer player, int n, int n1, int r)
        {
            Random rand = new Random();
            int rnd = rand.Next(1, 3);
            if (this.rounds < 2)
            {
                if (this.pokerCall <= 0)
                {
                    this.AICheck(player);
                }

                if (this.pokerCall > 0)
                {
                    if (this.pokerCall >= AIRoundNumber(player.Chips, n1))
                    {
                        this.AIFold(player);
                    }

                    if (this.raise > AIRoundNumber(player.Chips, n))
                    {
                        this.AIFold(player);
                    }

                    if (!player.GameEnded)
                    {
                        if (this.pokerCall >= AIRoundNumber(player.Chips, n) && this.pokerCall <= AIRoundNumber(player.Chips, n1))
                        {
                            this.AICall(player);
                        }

                        if (this.raise <= AIRoundNumber(player.Chips, n) && this.raise >= (AIRoundNumber(player.Chips, n)) / 2)
                        {
                            this.AICall(player);
                        }

                        if (this.raise <= (AIRoundNumber(player.Chips, n)) / 2)
                        {
                            if (this.raise > 0)
                            {
                                this.raise = AIRoundNumber(player.Chips, n);
                                this.AIRaised(player);
                            }
                            else
                            {
                                this.raise = this.pokerCall * 2;
                                this.AIRaised(player);
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
                    if (this.pokerCall >= AIRoundNumber(player.Chips, n1 - rnd))
                    {
                        this.AIFold(player);
                    }
                    if (this.raise > AIRoundNumber(player.Chips, n - rnd))
                    {
                        this.AIFold(player);
                    }
                    if (!player.GameEnded)
                    {
                        if (this.pokerCall >= AIRoundNumber(player.Chips, n - rnd) && this.pokerCall <= AIRoundNumber(player.Chips, n1 - rnd))
                        {
                            this.AICall(player);
                        }
                        if (this.raise <= AIRoundNumber(player.Chips, n - rnd) && this.raise >= (AIRoundNumber(player.Chips, n - rnd)) / 2)
                        {
                            this.AICall(player);
                        }
                        if (this.raise <= (AIRoundNumber(player.Chips, n - rnd)) / 2)
                        {
                            if (this.raise > 0)
                            {
                                this.raise = AIRoundNumber(player.Chips, n - rnd);
                                this.AIRaised(player);
                            }
                            else
                            {
                                this.raise = this.pokerCall * 2;
                                this.AIRaised(player);
                            }
                        }
                    }
                }
                if (this.pokerCall <= 0)
                {
                    this.raise = AIRoundNumber(player.Chips, r - rnd);
                    this.AIRaised(player);
                }
            }

            if (player.Chips <= 0)
            {
                player.GameEnded = true;
            }
        }

        /*wika se ot: AIThreeOfAKind, AIStraight, AIFlush, AIFullHouse, AIFourOfAKind, AIStraightFlush
         randoma e ot 1-3, NO NE SE POLZWA!
         ako pokerCall <= 0 bota igrae CHeck
         ako pokerCall > 0 w zawisimost ot matematikata na chipowete move da CALL, RASE
         ako botChips <= 0 => setwa botEndGame = true;
         */
        private void AISmooth(IPlayer player, int n, int r)
        {
          // star kod - > zakomentiran t.kato ne se polzwa: 
          // Random rand = new Random();
          // int rnd = rand.Next(1, 3);
            if (this.pokerCall <= 0)
            {
                this.AICheck(player);
            }
            else
            {
                if (this.pokerCall >= AIRoundNumber(player.Chips, n))
                {
                    if (player.Chips > this.pokerCall)
                    {
                        this.AICall(player);
                    }
                    else if (player.Chips <= this.pokerCall)
                    {
                        raising = false;
                        player.Turn = false;
                        player.Chips = 0;
                        player.Label.Text = "Call " + player.Chips;
                        textBoxPot.Text = (int.Parse(textBoxPot.Text) + player.Chips).ToString();
                    }
                }
                else
                {
                    if (this.raise > 0)
                    {
                        if (player.Chips >= this.raise * 2)
                        {
                            this.raise *= 2;
                            this.AIRaised(player);
                        }
                        else
                        {
                            this.AICall(player);
                        }
                    }
                    else
                    {
                        this.raise = this.pokerCall * 2;
                        this.AIRaised(player);
                    }
                }
            }
            if (player.Chips <= 0)
            {
                player.GameEnded = true;
            }
        }
      

        // wika se ot AIHP ili AIPH  this.AIFold(ref bothTurn, ref botEndGame, sStatus);
        private void AIFold(IPlayer player)
        {
            raising = false;
            player.Label.Text = "Fold";
            player.Turn = false;
            player.Folded = true;
            player.GameEnded = true;
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
        private void AICheck(IPlayer player)
        {
            player.Label.Text = "Check";
            player.Turn = false;
            this.raising = false;
        }

        // вика се от  AIHP AIPH, AISmooth this.AICall(ref botChips, ref bothTurn, sStatus);
        private void AICall(IPlayer player)
        {
            this.raising = false;
            player.Turn = false;
            player.Chips -= this.pokerCall;
            player.Label.Text = "Call " + this.pokerCall;
            this.textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
        }

        // вика се от  AIHP AIPH, AISmooth this.AIRaised(ref botChips, ref bothTurn, sStatus);
        private void AIRaised(IPlayer player)
        {
            player.Chips -= Convert.ToInt32(this.raise);
            player.Label.Text = "Raise " + this.raise;
            textBoxPot.Text = (int.Parse(textBoxPot.Text) + Convert.ToInt32(this.raise)).ToString();
            this.pokerCall = Convert.ToInt32(this.raise);
            raising = true;
            player.Turn = false;
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
                this.players[0].GameEnded = true;
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
            if (this.players[0].Chips <= 0)
            {
                textBoxPlayerChips.Text = "Chips : 0";
            }
            if (this.players[1].Chips <= 0)
            {
                textBoxBot1Chips.Text = "Chips : 0";
            }
            if (this.players[2].Chips <= 0)
            {
                textBoxBot2Chips.Text = "Chips : 0";
            }
            if (this.players[3].Chips <= 0)
            {
                textBoxBot3Chips.Text = "Chips : 0";
            }
            if (this.players[4].Chips <= 0)
            {
                textBoxBot4Chips.Text = "Chips : 0";
            }
            if (this.players[5].Chips <= 0)
            {
                textBoxBot5Chips.Text = "Chips : 0";
            }
            textBoxPlayerChips.Text = "Chips : " + this.players[0].Chips.ToString();
            textBoxBot1Chips.Text = "Chips : " + this.players[1].Chips.ToString();
            textBoxBot2Chips.Text = "Chips : " + this.players[2].Chips.ToString();
            textBoxBot3Chips.Text = "Chips : " + this.players[3].Chips.ToString();
            textBoxBot4Chips.Text = "Chips : " + this.players[4].Chips.ToString();
            textBoxBot5Chips.Text = "Chips : " + this.players[5].Chips.ToString();
            if (this.players[0].Chips <= 0)
            {
                players[0].Turn = false;
                this.players[0].GameEnded = true;
                buttonCall.Enabled = false;
                buttonRaise.Enabled = false;
                buttonFold.Enabled = false;
                buttonCheck.Enabled = false;
            }
            if (up > 0)
            {
                up--;
            }
            if (this.players[0].Chips >= this.pokerCall)
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
            if (this.players[0].Chips <= 0)
            {
                buttonRaise.Enabled = false;
            }
            int parsedValue;

            if (textBoxRaise.Text != "" && int.TryParse(textBoxRaise.Text, out parsedValue))
            {
                if (this.players[0].Chips <= int.Parse(textBoxRaise.Text))
                {
                    buttonRaise.Text = "All in";
                }
                else
                {
                    buttonRaise.Text = "Raise";
                }
            }
            if (this.players[0].Chips < this.pokerCall)
            {
                buttonRaise.Enabled = false;
            }
        }
        private async void ButtonFold_Click(object sender, EventArgs e)
        {
            this.gamer.Label.Text = "Fold";
            this.gamer.Turn = false;
            this.gamer.GameEnded = true;
            await Turns();
        }
        private async void ButtonCheck_Click(object sender, EventArgs e)
        {
            if (this.pokerCall <= 0)
            {
                players[0].Turn = false;
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
            Rules(0, 1, "Player", this.players[0].PokerHandMultiplier, this.players[0].CardPower, this.players[0].GameEnded);

            if (this.players[0].Chips >= this.pokerCall)
            {
                this.players[0].Chips -= this.pokerCall;
                textBoxPlayerChips.Text = "Chips : " + this.players[0].Chips.ToString();

                if (textBoxPot.Text != "")
                {
                    textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
                }
                else
                {
                    textBoxPot.Text = this.pokerCall.ToString();
                }

                players[0].Turn = false;
                labelPlayerStatus.Text = "Call " + this.pokerCall;
                this.players[0].Call = this.pokerCall;
            }
            else if (this.players[0].Chips <= this.pokerCall && this.pokerCall > 0)
            {
                textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.players[0].Chips).ToString();
                labelPlayerStatus.Text = "All in " + this.players[0].Chips;
                this.players[0].Chips = 0;
                textBoxPlayerChips.Text = "Chips : " + this.players[0].Chips.ToString();
                players[0].Turn = false;
                buttonFold.Enabled = false;
                this.players[0].Call = this.players[0].Chips;
            }
            await Turns();
        }
        private async void ButtonRaise_Click(object sender, EventArgs e)
        {
            Rules(0, 1, "Player", this.players[0].PokerHandMultiplier, this.players[1].CardPower, this.players[0].GameEnded);
            int parsedValue;
            if (textBoxRaise.Text != "" && int.TryParse(textBoxRaise.Text, out parsedValue))
            {
                if (this.players[0].Chips > this.pokerCall)
                {
                    if (this.raise * 2 > int.Parse(textBoxRaise.Text))
                    {
                        textBoxRaise.Text = (this.raise * 2).ToString();
                        MessageBox.Show("You must raise atleast twice as the PokerHandMultiplier raise !");
                        return;
                    }
                    else
                    {
                        if (this.players[0].Chips >= int.Parse(textBoxRaise.Text))
                        {
                            this.pokerCall = int.Parse(textBoxRaise.Text);
                            this.raise = int.Parse(textBoxRaise.Text);
                            labelPlayerStatus.Text = "Raise " + this.pokerCall.ToString();
                            textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.pokerCall).ToString();
                            buttonCall.Text = "Call";
                            this.players[0].Chips -= int.Parse(textBoxRaise.Text);
                            raising = true;
                            lastBotPlayed = 0;
                            players[0].Raise = Convert.ToInt32(this.raise);
                        }
                        else
                        {
                            this.pokerCall = this.players[0].Chips;
                            this.raise = this.players[0].Chips;
                            textBoxPot.Text = (int.Parse(textBoxPot.Text) + this.players[0].Chips).ToString();
                            labelPlayerStatus.Text = "Raise " + this.pokerCall.ToString();
                            this.players[0].Chips = 0;
                            raising = true;
                            lastBotPlayed = 0;
                            players[0].Raise = Convert.ToInt32(this.raise);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("This is a number only field");
                return;
            }
            players[0].Turn = false;
            await Turns();
        }
        private void ButtonAddChips_Click(object sender, EventArgs e)
        {
            if (textBoxAddChips.Text == "") { }
            else
            {
                this.players[0].Chips += int.Parse(textBoxAddChips.Text);
                this.players[1].Chips += int.Parse(textBoxAddChips.Text);
                this.players[2].Chips += int.Parse(textBoxAddChips.Text);
                this.players[3].Chips += int.Parse(textBoxAddChips.Text);
                this.players[4].Chips += int.Parse(textBoxAddChips.Text);
                this.players[5].Chips += int.Parse(textBoxAddChips.Text);
            }
            textBoxPlayerChips.Text = "Chips : " + this.players[0].Chips.ToString();
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