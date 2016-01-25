namespace Poker.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using GameObjects.Player;
    using System.Linq;
    using GameObjects;
    using GameObjects.Cards;
    using UI;

    public class GameEngine
    {
        private const int DealingCardsDelay = 200;

        private readonly Random random = new Random();
        private IRenderer renderer;

        #region Variables
        private int foldedPlayers = GlobalConstants.BotCount;
        //// used in CheckRaise method :   if (Table.Rounds == End && maxPlayersLeftCount == 6);
        ////used in AllIn method->  #region FiveOrLessLeft: if (abc < 6 && abc > 1 && Table.Rounds >= End) 
        ////in Finish method is seted again on 4;
        private int maxPlayersLeftCount = GlobalConstants.PlayersCount;


        // TODO: create public property;

        public bool raising = false;
        //   public int bb = GlobalConstants.InitialBigBlind;
        public int sb = GlobalConstants.InitialSmallBlind;

        public int turnCount = 0;

        private bool restart = false;
        private bool intsadded;
        private bool changed;
        private double type;

        private int raisedTurn = 1;
        ////used in Turns method -> region Rotating : in every positive check is game ending  maxPlayersLeftCount--;
        //// used in CheckRaise method :   if (Table.Rounds == End && maxPlayersLeftCount == 6); and  if (turnCount >= maxPlayersLeftCount - 1 || !changed && turnCount == maxPlayersLeftCount);
        ////used in AllIn method : if (ints.ToArray().Length == maxPlayersLeftCount)
        ////in Finish method is seted again on 6;
        //TODO: Create public property;
        public int lastBotPlayed = 123;

        private int t = 60;

        private bool skipInitialPlayerCall = false;
        private bool skipInitialBotCall = false;


        //names for cards on the table
        ////used in CheckRaise method :   if (Table.Rounds == Flop); in Finish method is seted again on 1;
        private int flop = 1;
        ////used in CheckRaise method :  if (Table.Rounds == Turn); in Finish method is seted again on 2;
        private int turn = 2;
        private int river = 3;
        //// used in CheckRaise method :  (Table.Rounds == River); in Finish method is seted again on 3;
        private int end = 4;

        ////Output from method Rules
        private List<WinningHand> winningCards = new List<WinningHand>();
        private List<bool?> playersNotGameEnded = new List<bool?>();
        private List<string> checkWinners = new List<string>();
        private List<int> playersWithoutChips = new List<int>();

        private WinningHand winningCard;

        /// <summary>
        /// Array of all cards files names; 
        /// [
        /// Assets\\Cards\\1.png    
        /// Assets\\Cards\\2.png
        /// ]
        /// </summary>
        private string[] imageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Array of Integers -> dealed cars. Array.Lengt = 17
        /// towa sa nomerata na fajlowete, pri men sa ID-ta
        /// </summary>
        private int[] dealtCardsNumbers = new int[17];

        /*string[] ImgLocation ={
                   "Assets\\Cards\\33.png","Assets\\Cards\\22.png",
                    "Assets\\Cards\\29.png","Assets\\Cards\\21.png",
                    "Assets\\Cards\\36.png","Assets\\Cards\\17.png",
                    "Assets\\Cards\\40.png","Assets\\Cards\\16.png",
                    "Assets\\Cards\players\5.png","Assets\\Cards\\47.png",
                    "Assets\\Cards\\37.png","Assets\\Cards\\13.png",
                    
                    "Assets\\Cards\\12.png",
                    "Assets\\Cards\\8.png","Assets\\Cards\\18.png",
                    "Assets\\Cards\\15.png","Assets\\Cards\\27.png"};
         */


        /// <summary>
        /// Array of Images. All 52 Cards's Images
        /// </summary>
        //  private Image[] deckImages = new Image[52];

        // trqbwa da e wyw formata
        ///// <summary>
        ///// Array of Form Controls -> PictureBox. For All cards
        ///// </summary>
        //private PictureBox[] cardHolder = new PictureBox[52];

        #endregion

        // new filds


        public GameEngine(IRenderer renderer)
        {
            this.Table = new Table();
            this.Players = new IPlayer[GlobalConstants.PlayersCount];
            this.InitializePlayers();
            this.renderer = renderer;
            this.PepsterDeck = new PepsterCard[52];
            this.PepsterDealtCards = new PepsterCard[17];
            this.SetAllPepsterDeck();
        }

        public Table Table { get; set; }
        public const int PlayersCount = 6;

        public IPlayer[] Players { get; private set; }

        public Gamer Gamer { get; set; }

        private void InitializePlayers()
        {
            for (int playerCount = 0, cardFirstPosition = 0; playerCount < 6; playerCount++, cardFirstPosition += 2)
            {
                if (playerCount == 0)
                {
                    this.Gamer = new Gamer();
                    this.Players[playerCount] =  this.Gamer;
                }
                else
                {
                    this.Players[playerCount] = new Bot(playerCount, "Bot " + playerCount, cardFirstPosition, cardFirstPosition + 1);
                }
            }
        }

        public PepsterCard[] PepsterDeck { get; private set; }

        public PepsterCard[] PepsterDealtCards { get; set; }

        private void SetAllPepsterDeck()
        {
            int pepsterDeckIndex = 0;
            int idCounter = 1;
            for (int i = 0; i < 13; i++)
            {
                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Clubs,
                    CardFrontImageUri = "../Resources/Cards/" + idCounter + ".png"
                };
                idCounter++;
                pepsterDeckIndex++;

                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Diamonds,
                    CardFrontImageUri = "../Resources/Cards/" + idCounter + ".png"
                };
                idCounter++;
                pepsterDeckIndex++;

                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Hearts,
                    CardFrontImageUri = "../Resources/Cards/" + idCounter + ".png"
                };
                idCounter++;
                pepsterDeckIndex++;

                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Spades,
                    CardFrontImageUri = "../Resources/Cards/" + idCounter + ".png"
                };
                idCounter++;
                pepsterDeckIndex++;
            }
        }

        public void GameInit()
        {
            this.Table.PokerCall = GlobalConstants.InitialBigBlind;
            this.renderer.Clear();
            this.renderer.Draw(this.Players);

            this.SetupPokerTable();
        }

        //from GameForm Methods
        //Region 1 to fix errors  - Stanimir
        #region Refactored Shuffle to SetupPokerTable

        /// <summary>
        /// Setups a poker table with all the players holders, cards on the table and buttons
        /// </summary>
        /// <returns></returns>
        private async Task SetupPokerTable()
        {
            this.playersNotGameEnded.Add(this.Players[0].GameEnded);
            this.playersNotGameEnded.Add(this.Players[1].GameEnded);
            this.playersNotGameEnded.Add(this.Players[2].GameEnded);
            this.playersNotGameEnded.Add(this.Players[3].GameEnded);
            this.playersNotGameEnded.Add(this.Players[4].GameEnded);
            this.playersNotGameEnded.Add(this.Players[5].GameEnded);

            this.PepsterRandomCardsToDeal();
            this.FillInDealtCardsNumbers();

            for (int cardIndex = 0; cardIndex < GlobalConstants.DealtCardsCount; cardIndex++)
            {
                this.renderer.Draw(this.PepsterDealtCards[cardIndex]);
                this.DrawHideBotsCards();
                await Task.Delay(DealingCardsDelay);
            }

            this.EnablingFormMinimizationAndMaximization();
            # region 1part to fix errors
            //this.form.timer.Start();

            this.CheckForGameEnd();

            this.Gamer.CanRaise = true;
            this.Gamer.CanCall = true;
            this.Gamer.CanCheck = true;
            this.Gamer.CanFold = true;

            this.renderer.ShowOrHidePlayersButtons(this.Gamer);
            this.renderer.ShowGamerTurnTimer();
        }

        /// <summary>
        /// Fill in a dealtCards array with integers coresponding to the cards that are designated for dealing
        /// </summary>
        /// <param name="cardIndex"> Index of the cards that are dealt</param>
        private void FillInDealtCardsNumbers()
        {
            for (int i = 0; i < this.dealtCardsNumbers.Length; i++)
            {
                this.dealtCardsNumbers[i] = this.PepsterDealtCards[i].Id;
            }

            //string removeURI = "Assets\\Cards\\";
            //string removeFileExtension = ".png";

            //this.imageURIArray[cardIndex] = this.imageURIArray[cardIndex].Replace(removeURI, string.Empty);
            //this.imageURIArray[cardIndex] = this.imageURIArray[cardIndex].Replace(removeFileExtension, string.Empty);

            // this.dealtCardsNumbers[cardIndex] = int.Parse(this.imageURIArray[cardIndex]) - 1;
        }

        /// <summary>
        /// prehwyrlen w Renderer.DrowCard
        /// Prepearing CardHolder for every card on the table
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        //private void SetupCardHolder(int cardIndex)
        //{
        //    this.cardHolder[cardIndex] = new PictureBox();
        //    this.cardHolder[cardIndex].SizeMode = PictureBoxSizeMode.StretchImage;
        //    this.cardHolder[cardIndex].Height = CardHeight;
        //    this.cardHolder[cardIndex].Width = CardWidth;
        //    this.cardHolder[cardIndex].Name = "pb" + cardIndex.ToString();
        //    this.form.Controls.Add(this.cardHolder[cardIndex]);
        //}

        /// <summary>
        /// pepster
        /// Shuffles the entire deck ImageURIArray randomly
        /// </summary>
        //private void ShuffleDeck()
        //{
        //    for (int i = this.imageURIArray.Length; i > 0; i--)
        //    {
        //        int randomIndex = this.random.Next(i);
        //        var randomImageURI = this.imageURIArray[randomIndex];
        //        this.imageURIArray[randomIndex] = this.imageURIArray[i - 1];
        //        this.imageURIArray[i - 1] = randomImageURI;
        //    }
        //}

        private void PepsterRandomCardsToDeal()
        {
            int dealtCardIndex = 0;
            do
            {
                var randomIndex = this.random.Next(this.PepsterDeck.Length);
                var card = this.PepsterDeck[randomIndex];
                bool cardIsDealt = this.PepsterDealtCards.Contains(card);
                if (!cardIsDealt)
                {
                    card.DealtPosition = dealtCardIndex;
                    this.PepsterDealtCards[dealtCardIndex] = card;
                    dealtCardIndex++;
                }
            }
            while (dealtCardIndex < this.PepsterDealtCards.Length);
        }

        /// <summary>
        /// Check for game completion 
        /// </summary>
        private void CheckForGameEnd()
        {
            if (this.foldedPlayers == 5)
            {
                var dialogResult = MessageBox.Show("Would You Like To Play Again ?",
                                                   "You Won , Congratulations ! ",
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
        /// Checks which bots have money to continue playing.
        /// if the bot has not money the cardControls are invisible;
        /// </summary>
        /// <param name="cardIndex">Index of the cards that are dealt</param>
        private void DrawHideBotsCards()
        {
            for (int i = 1; i < 5; i++)
            {
                var bot = this.Players[i];
                if (bot.Chips <= 0)
                {
                    bot.GameEnded = true;
                }
                this.renderer.Draw(bot);
            }
        }

        /// <summary>
        /// Enabling maximization or minimization of the Form window 
        /// </summary>
        private void EnablingFormMinimizationAndMaximization()
        {
            if (!this.restart)
            {
                //this are form controls
                this.renderer.EnablingFormMinimizationAndMaximization();
            }
        }

        /// <summary>
        /// Displaying the bots cards
        /// </summary>
        /// <param name="botNumber">Number of the bot starting from 1 up to 5</param>
        /// <param name="cardIndex">Index of the cards that are dealt</param>


            #endregion

        //Veronika
        public async Task Turns()
        {
            #region Rotating

            if (!this.Players[0].GameEnded)
            {
                if (this.Players[0].Turn)
                {
                    var text = this.Players[0].Status;
                    this.CallAllPlayerActionsOnTurn(this.Players[0]);
                }
            }

            if (this.Players[0].GameEnded ||
                !this.Players[0].Turn)
            {
                await this.AllIn();
                if (this.Players[0].GameEnded &&
                    !this.Players[0].Folded)
                {
                    if (this.Gamer.IsAllIn == false ||
                        this.Gamer.IsAllIn == false)
                    {
                        this.RemovePlayerFromTheGame(0);
                    }
                }

                await this.CheckRaise(0, 0);

                this.Gamer.CanRaise = true;
                this.Gamer.CanCall = true;
                this.Gamer.CanCheck = true;
                this.Gamer.CanFold = true;

                this.renderer.ShowOrHidePlayersButtons(this.Gamer);
                this.renderer.HideGamerTurnTimer();

                this.Players[1].Turn = true;

                for (int i = 1; i < this.Players.Length; i++)
                {
                    /* old code: var text = this.players[i].Label.Text; 
                     * new code: 
                     */
                    //var text = this.form.PlayersLabelsStatus[this.Players[i].Id].Text;

                    if (!this.Players[i].GameEnded)
                    {
                        if (this.Players[i].Turn)
                        {
                            this.CallAllBotActionsOnTheirTurn(this.Players[i]);
                        }
                    }

                    if (this.Players[i].GameEnded &&
                        !this.Players[i].Folded)
                    {
                        ////TODO _ActivePlayers rename
                        this.RemovePlayerFromTheGame(i);
                    }

                    if (this.Players[i].GameEnded ||
                        !this.Players[i].Turn)
                    {
                        await this.CheckRaise(i, i);
                        if (i + 1 == this.Players.Length)
                        {
                            this.Players[0].Turn = true;
                        }
                        else
                        {
                            this.Players[i + 1].Turn = true;
                        }
                    }
                }

                if (this.Players[0].GameEnded &&
                    !this.Players[0].Folded)
                {
                    if (this.Gamer.IsAllIn == false)
                    {
                        this.RemovePlayerFromTheGame(0);
                    }
                }
            #endregion

                await this.AllIn();
                if (!this.restart)
                {
                    await this.Turns();
                }

                this.restart = false;
            }
        }

        private void RemovePlayerFromTheGame(int playerIndex)
        {
            this.playersNotGameEnded.RemoveAt(playerIndex);
            this.playersNotGameEnded.Insert(playerIndex, null);
            this.maxPlayersLeftCount--;
            this.Players[playerIndex].Folded = true;
        }

        private void CallAllPlayerActionsOnTurn(IPlayer player)
        {
            this.FixCall(player, 1);
            this.renderer.ShowGamerTurnTimer();
            ////MessageBox.Show("Player's Turn");
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
            /////Bot 1 -> 0 ,Bot 2 ->1 etc.. 
            ///// used in this.Ai(...)
            //var botPresentedAsNumber = int.Parse(botNumber) - 1;

            this.FixCall(player, 1);
            this.FixCall(player, 2);

            this.Rules(player);

            MessageBox.Show("Bot  " + botNumber + @"'s Turn");
            this.AI(player);

            this.turnCount++;
            this.lastBotPlayed = int.Parse(botNumber);
            player.Turn = false;

            ////TODO must be implemented

            var nextBotIndex = int.Parse(botNumber) + 1;
            if (nextBotIndex <= 5)
            {
                this.Players[nextBotIndex].Turn = true;
            }
            else
            {
                nextBotIndex = 0;
                this.Players[nextBotIndex].Turn = true;
            }
        }

        public void Rules(IPlayer player)
        {
            ////if (firstCard == 0 && secondCard == 1)
            ////{
            ////}

            // playerlabelText.Contains("Fold") == false can be replaces with player.Status == "Fold"

            //The playerlabelText is commented out because its value is replaced with player.Folded
            //string playerlabelText = this.form.PlayersLabelsStatus[player.Id].Text;

            if (!player.Folded ||
                (player.FirstCardPosition == 0 && player.SecondCardPosition == 1 && player.Folded == false))
            {
                #region Variables
                bool done = false;
                bool vf = false;
                int[] cardsOnTable = new int[5];
                int[] cardsOnTableWithPlayerCards = new int[7];
                cardsOnTableWithPlayerCards[0] = this.dealtCardsNumbers[player.FirstCardPosition];
                cardsOnTableWithPlayerCards[1] = this.dealtCardsNumbers[player.SecondCardPosition];

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

                int playerFirstCard = this.dealtCardsNumbers[player.FirstCardPosition];
                int playerSecondCard = this.dealtCardsNumbers[player.SecondCardPosition];

                #endregion

                ////for (int i = 0; i < DealtCards - 1; i++)
                ////{
                ////int parse = int.Parse(cardHolder[firstCard].Tag.ToString());
                ////int parse1 = int.Parse(cardHolder[secondCard].Tag.ToString());

                ////if (this.dealtCardsNumbers[i] == parse && this.dealtCardsNumbers[i + 1] == parse1)
                ////{

                #region High Card PokerHandMultiplier = -1
                this.rHighCard(playerFirstCard, playerSecondCard, player);
                #endregion

                #region Pair from Table PokerHandMultiplier = 0
                this.rPair(playerFirstCard, playerSecondCard, cardsOnTable, player);
                #endregion

                #region Pair from hand PokerHandMultiplier = 1
                this.rPairFromHand(playerFirstCard, playerSecondCard, player);
                #endregion

                #region Two Pair PokerHandMultiplier = 2
                this.rTwoPair(playerFirstCard, playerSecondCard, cardsOnTable, player);
                #endregion

                #region Three of a kind PokerHandMultiplier = 3
                this.rThreeOfAKind(playerFirstCard, playerSecondCard, cardsOnTable, player);
                #endregion

                #region Straight PokerHandMultiplier = 4

                this.rStraight(playerFirstCard, playerSecondCard, cardsOnTable, player);

                #endregion

                //#region Flush PokerHandMultiplier = 5 || 5.5

                //this.rFlush(firstCard, secondCard, player.PokerHandMultiplier, player.CardPower, cardsOnTable, firstCard);

                //#endregion

                //#region Full House PokerHandMultiplier = 6

                //this.rFullHouse(firstCard, secondCard, player.PokerHandMultiplier, player.CardPower, ref done, cardsOnTableWithPlayerCards);

                //#endregion

                //#region Four of a Kind PokerHandMultiplier = 7

                //this.rFourOfAKind(firstCard, secondCard, player.PokerHandMultiplier, player.CardPower, cardsOnTableWithPlayerCards);

                //#endregion

                //#region Straight Flush PokerHandMultiplier = 8 || 9

                //this.rStraightFlush(firstCard, secondCard, player.PokerHandMultiplier,
                //               player.CardPower,
                //               cardsOfClubs,
                //               cardsOfDiamonds,
                //               cardsOfHearts,
                //               cardsOfSpades);

                //#endregion

                ////}
                ////}
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
                        Power = (cardsOfClubs.Max() / 4) + (PokerHandMultiplier * 100);
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 8
                        });
                        this.winningCard =
                            this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }

                    if (cardsOfClubs[0] == 0 &&
                        cardsOfClubs[1] == 9 &&
                        cardsOfClubs[2] == 10 &&
                        cardsOfClubs[3] == 11 &&
                        cardsOfClubs[0] + 12 == cardsOfClubs[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = (cardsOfClubs.Max() / 4) + (PokerHandMultiplier * 100);
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 9
                        });
                        this.winningCard =
                            this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }
                }

                if (cardsOfDiamonds.Length >= 5)
                {
                    if (cardsOfDiamonds[0] + 4 == cardsOfDiamonds[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = cardsOfDiamonds.Max() / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 8
                        });
                        this.winningCard =
                            this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }

                    if (cardsOfDiamonds[0] == 0 &&
                        cardsOfDiamonds[1] == 9 &&
                        cardsOfDiamonds[2] == 10 &&
                        cardsOfDiamonds[3] == 11 &&
                        cardsOfDiamonds[0] + 12 == cardsOfDiamonds[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = cardsOfDiamonds.Max() / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 9
                        });
                        this.winningCard =
                            this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }
                }

                if (cardsOfHearts.Length >= 5)
                {
                    if (cardsOfHearts[0] + 4 == cardsOfHearts[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = cardsOfHearts.Max() / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 8
                        });
                        this.winningCard =
                            this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }

                    if (cardsOfHearts[0] == 0 &&
                        cardsOfHearts[1] == 9 &&
                        cardsOfHearts[2] == 10 &&
                        cardsOfHearts[3] == 11 &&
                        cardsOfHearts[0] + 12 == cardsOfHearts[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = cardsOfHearts.Max() / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 9
                        });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }
                }

                if (cardsOfSpades.Length >= 5)
                {
                    if (cardsOfSpades[0] + 4 == cardsOfSpades[4])
                    {
                        PokerHandMultiplier = 8;
                        Power = cardsOfSpades.Max() / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 8
                        });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }

                    if (cardsOfSpades[0] == 0 &&
                        cardsOfSpades[1] == 9 &&
                        cardsOfSpades[2] == 10 &&
                        cardsOfSpades[3] == 11 &&
                        cardsOfSpades[0] + 12 == cardsOfSpades[4])
                    {
                        PokerHandMultiplier = 9;
                        Power = cardsOfSpades.Max() / 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 9
                        });
                        this.winningCard = this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
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
                    if (Straight[j] / 4 == Straight[j + 1] / 4 &&
                        Straight[j] / 4 == Straight[j + 2] / 4 &&
                        Straight[j] / 4 == Straight[j + 3] / 4)
                    {
                        PokerHandMultiplier = 7;
                        Power = (Straight[j] / 4) * 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 7
                        });
                        this.winningCard =
                            this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }

                    if (Straight[j] / 4 == 0 &&
                        Straight[j + 1] / 4 == 0 &&
                        Straight[j + 2] / 4 == 0 &&
                        Straight[j + 3] / 4 == 0)
                    {
                        PokerHandMultiplier = 7;
                        Power = 13 * 4 + PokerHandMultiplier * 100;
                        this.winningCards.Add(new WinningHand()
                        {
                            Power = Power,
                            Current = 7
                        });
                        this.winningCard =
                            this.winningCards.OrderByDescending(op1 => op1.Current)
                                .ThenByDescending(op1 => op1.Power)
                                .First();
                    }
                }
            }
        }

        private void rFullHouse(double PokerHandMultiplier, double power, ref bool done, int[] straight)
        {
            if (PokerHandMultiplier >= -1)
            {
                this.type = power;
                for (int j = 0; j <= 12; j++)
                {
                    var fh = straight.Where(o => o / 4 == j).ToArray();
                    if (fh.Length == 3 || done)
                    {
                        if (fh.Length == 2)
                        {
                            if (fh.Max() / 4 == 0)
                            {
                                PokerHandMultiplier = 6;
                                power = 13 * 2 + PokerHandMultiplier * 100;
                                this.winningCards.Add(new WinningHand()
                                {
                                    Power = power,
                                    Current = 6
                                });
                                this.winningCard =
                                    this.winningCards.OrderByDescending(op1 => op1.Current)
                                        .ThenByDescending(op1 => op1.Power)
                                        .First();
                                break;
                            }

                            if (fh.Max() / 4 > 0)
                            {
                                PokerHandMultiplier = 6;
                                power = fh.Max() / 4 * 2 + PokerHandMultiplier * 100;
                                this.winningCards.Add(new WinningHand()
                                {
                                    Power = power,
                                    Current = 6
                                });
                                this.winningCard =
                                    this.winningCards.OrderByDescending(op1 => op1.Current)
                                        .ThenByDescending(op1 => op1.Power)
                                        .First();
                                break;
                            }
                        }

                        if (!done)
                        {
                            if (fh.Max() / 4 == 0)
                            {
                                power = 13;
                                done = true;
                                j = -1;
                            }
                            else
                            {
                                power = fh.Max() / 4;
                                done = true;
                                j = -1;
                            }
                        }
                    }
                }

                if (PokerHandMultiplier != 6)
                {
                    power = this.type;
                }
            }
        }
        #region Flush

        private void rFlush(double PokerHandMultiplier, double power, int[] cardsOnTable, int index)
        {
            ////if (PokerHandMultiplier >= -1)
            ////{
            var cardsOfClubs = cardsOnTable.Where(o => o % 4 == (int)CardSuit.Clubs).ToArray();
            var cardsOfDiamonds = cardsOnTable.Where(o => o % 4 == (int)CardSuit.Diamonds).ToArray();
            var cardsOfHearts = cardsOnTable.Where(o => o % 4 == (int)CardSuit.Hearts).ToArray();
            var cardsOfSpades = cardsOnTable.Where(o => o % 4 == (int)CardSuit.Spades).ToArray();

            if (cardsOfClubs.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, power, index, cardsOfClubs, (int)CardSuit.Clubs);
            }

            if (cardsOfDiamonds.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, power, index, cardsOfDiamonds, (int)CardSuit.Diamonds);
            }

            if (cardsOfHearts.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, power, index, cardsOfHearts, (int)CardSuit.Hearts);
            }

            if (cardsOfSpades.Length > 2)
            {
                this.CheckForFlush(PokerHandMultiplier, power, index, cardsOfSpades, (int)CardSuit.Spades);
            }
            ////}
        }

        private void CheckForFlush(double pokerHandMultiplier,
                                   double Power,
                                   int index,
                                   int[] cardsOfSuit,
                                   int suitNumber)
        {
            pokerHandMultiplier = 5;

            if (this.GetCardSuit(index) == suitNumber &&
                this.GetCardSuit(index + 1) == this.GetCardSuit(index + 1))
            {
                if (this.GetCardRank(index) > cardsOfSuit.Max() / 4)
                {
                    this.SetWinningCard(pokerHandMultiplier, index);
                }
                else if (this.GetCardRank(index + 1) > cardsOfSuit.Max() / 4)
                {
                    this.SetWinningCard(pokerHandMultiplier, index + 1);
                }
                else
                {
                    this.SetWinningCardFromSuitMax(pokerHandMultiplier, cardsOfSuit);
                }
            }

            if (cardsOfSuit.Length == 4) //different cards in hand
            {
                if (this.GetCardSuit(index) == suitNumber &&
                    this.GetCardSuit(index) != this.GetCardSuit(index + 1))
                {
                    if (this.GetCardRank(index) > cardsOfSuit.Max() / 4)
                    {
                        this.SetWinningCard(pokerHandMultiplier, index);
                    }
                    else
                    {
                        this.SetWinningCardFromSuitMax(pokerHandMultiplier, cardsOfSuit);
                    }
                }

                if (this.GetCardSuit(index + 1) == suitNumber &&
                    this.GetCardSuit(index + 1) != this.GetCardSuit(index))
                {
                    if (this.GetCardRank(index + 1) > cardsOfSuit.Max() / 4)
                    {
                        this.SetWinningCard(pokerHandMultiplier, index + 1);
                    }
                    else
                    {
                        this.SetWinningCardFromSuitMax(pokerHandMultiplier, cardsOfSuit);
                    }
                }
            }

            if (cardsOfSuit.Length == 5)
            {
                if (this.GetCardSuit(index) == cardsOfSuit[0] % 4 &&
                    this.GetCardRank(index) > cardsOfSuit.Min() / 4)
                {
                    this.SetWinningCard(pokerHandMultiplier, index);
                }
                else if (this.GetCardSuit(index + 1) == cardsOfSuit[0] % 4 &&
                         this.GetCardRank(index + 1) > cardsOfSuit.Min() / 4)
                {
                    this.SetWinningCard(pokerHandMultiplier, index + 1);
                }
                else if (this.GetCardRank(index) < cardsOfSuit.Min() / 4 &&
                         this.GetCardRank(index + 1) < cardsOfSuit.Min())
                {
                    this.SetWinningCardFromSuitMax(pokerHandMultiplier, cardsOfSuit);
                }
            }
        }

        private void SetWinningCardFromSuitMax(double pokerHandMultiplier, int[] cardsOfSuit)
        {
            double power = cardsOfSuit.Max() + pokerHandMultiplier * 100;
            this.FindWinnigCard(pokerHandMultiplier, power);
        }

        private void SetWinningCard(double pokerHandMultiplier, int index)
        {
            double power = this.dealtCardsNumbers[index] + pokerHandMultiplier * 100;
            this.FindWinnigCard(pokerHandMultiplier, power);
        }
        #endregion

        private void rStraight(int firstCard, int secondCard, int[] cardsOnTable, IPlayer player)
        {
            int firstCardStraightCount = cardsOnTable.Count(card => this.GetCardSuit(firstCard) == this.GetCardSuit(card));
            int secondCardStraightCount = cardsOnTable.Count(card => this.GetCardSuit(secondCard) == this.GetCardSuit(card));

            int highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            if (firstCardStraightCount == 4 ^ secondCardStraightCount == 4)
            {
                player.PokerHandMultiplier = 4;
                player.CardPower = this.GetCardRank(highestCard) * 4 + this.GetCardSuit(highestCard) + player.PokerHandMultiplier * 100;
            }

            if ((firstCardStraightCount == 3 ^ secondCardStraightCount == 3) && this.GetCardSuit(firstCard) == this.GetCardSuit(secondCard))
            {
                player.PokerHandMultiplier = 4;
                player.CardPower = this.GetCardRank(highestCard) * 4 + this.GetCardSuit(highestCard) + player.PokerHandMultiplier * 100;
            }
        }

        private int GetHighestCardInHand(int firstCard, int secondCard)
        {
            int firstCardPower = this.GetCardRank(firstCard) + this.GetCardSuit(firstCard);
            int secondCardPower = this.GetCardRank(secondCard) + this.GetCardSuit(secondCard);

            int highestCard = firstCardPower > secondCardPower ? firstCard : secondCard;

            return highestCard;
        }

        private void rThreeOfAKind(int firstCard, int secondCard, int[] cardsOnTable, IPlayer player)
        {
            int firstCardThreeOfAKindCount = cardsOnTable.Count(card => this.GetCardRank(firstCard) == this.GetCardRank(card));
            int secondCardThreeOfAKindCount = cardsOnTable.Count(card => this.GetCardRank(secondCard) == this.GetCardRank(card));

            int highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            if (firstCardThreeOfAKindCount == 2 ^ secondCardThreeOfAKindCount == 2)
            {
                player.PokerHandMultiplier = 3;
                player.CardPower = this.GetCardRank(highestCard) * 4 + this.GetCardSuit(highestCard) + player.PokerHandMultiplier * 100;
            }

            if ((firstCardThreeOfAKindCount == 1 ^ secondCardThreeOfAKindCount == 1) && this.GetCardRank(firstCard) == this.GetCardRank(secondCard))
            {
                player.PokerHandMultiplier = 3;
                player.CardPower = this.GetCardRank(highestCard) * 4 + this.GetCardSuit(highestCard) + player.PokerHandMultiplier * 100;
            }
        }

        private void rTwoPair(int firstCard, int secondCard, int[] cardsOnTable, IPlayer player)
        {
            int firstCardPairCount = cardsOnTable.Count(card => this.GetCardRank(firstCard) == this.GetCardRank(card));
            int secondCardPairCount = cardsOnTable.Count(card => this.GetCardRank(secondCard) == this.GetCardRank(card));
            int pairOnTaible = 0;

            for (int i = 0; i < cardsOnTable.Length - 1; i++)
            {
                int pairCount = 0;

                for (int j = cardsOnTable.Length - 1; j > i; j--)
                {
                    if (this.GetCardRank(cardsOnTable[i]) == this.GetCardRank(cardsOnTable[j]))
                    {
                        pairCount++;
                    }
                }

                if (pairCount == 1)
                {
                    pairOnTaible = 1;
                    break;
                }
            }

            int highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            if (firstCardPairCount == 1 && secondCardPairCount == 1 && this.GetCardRank(firstCard) != this.GetCardRank(secondCard))
            {
                player.PokerHandMultiplier = 2;
                player.CardPower = this.GetCardRank(highestCard) * 4 + this.GetCardSuit(highestCard) + player.PokerHandMultiplier * 100;
            }

            if (pairOnTaible == 1 && this.GetCardRank(firstCard) == this.GetCardRank(secondCard))
            {
                player.PokerHandMultiplier = 2;
                player.CardPower = this.GetCardRank(highestCard) * 4 + this.GetCardSuit(highestCard) + player.PokerHandMultiplier * 100;
            }
        }

        private void rPair(int firstCard, int secondCard, int[] cardsOnTable, IPlayer player)
        {
            int firstCardPairCount = cardsOnTable.Count(card => this.GetCardRank(firstCard) == this.GetCardRank(card));
            int secondCardPairCount = cardsOnTable.Count(card => this.GetCardRank(secondCard) == this.GetCardRank(card));

            if ((firstCardPairCount == 1 ^ secondCardPairCount == 1) &&
                this.GetCardRank(firstCard) == this.GetCardRank(secondCard))
            {
                return;
            }

            if (firstCardPairCount == 1)
            {
                player.PokerHandMultiplier = 0;
                player.CardPower = this.GetCardRank(firstCard) * 4 + GetCardSuit(firstCard) + player.PokerHandMultiplier * 100;
            }

            if (secondCardPairCount == 1)
            {
                player.PokerHandMultiplier = 0;
                player.CardPower = this.GetCardRank(secondCard) * 4 + GetCardSuit(secondCard) + player.PokerHandMultiplier * 100;
            }

        }

        private void rPairFromHand(int firstCard, int secondCard, IPlayer player)
        {
            if (this.GetCardRank(firstCard) != this.GetCardRank(secondCard))
            {
                return;
            }

            int highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            player.PokerHandMultiplier = 1;
            player.CardPower = ((this.GetCardRank(highestCard)) * 4) + (player.PokerHandMultiplier * 100);
        }

        private void rHighCard(int firstCard, int secondCard, IPlayer player)
        {
            int highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            player.PokerHandMultiplier = -1;
            player.CardPower = this.GetCardRank(highestCard) + this.GetCardSuit(highestCard) + player.PokerHandMultiplier * 100;
        }

        private int GetCardSuit(int card)
        {
            return card % 4;
        }

        private int GetCardRank(int card)
        {
            return card / 4;
        }

        private void FindWinnigCard(double PokerHandMultiplier, double power)
        {
            this.winningCards.Add(new WinningHand()
            {
                Power = power,
                Current = PokerHandMultiplier
            });
            this.winningCard = this.winningCards
                                   .OrderByDescending(op1 => op1.Current)
                                   .ThenByDescending(op1 => op1.Power)
                                   .First();
        }

        ////Veronika
        private void Winner(double PokerHandMultiplier, double Power, string playerName, int chips, string lastly)
        {
            if (lastly == " ")
            {
                lastly = "Bot 5";
            }

            this.renderer.ShowAllCards();

            if (PokerHandMultiplier == this.winningCard.Current)
            {
                if (Power == this.winningCard.Power)
                {
                    this.Table.WinnersCount++;
                    this.checkWinners.Add(playerName);

                    if (PokerHandMultiplier == -1)
                    {
                        MessageBox.Show(playerName + " High Card ");
                    }

                    if (PokerHandMultiplier == 1 ||
                        PokerHandMultiplier == 0)
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

                    if (PokerHandMultiplier == 5 ||
                        PokerHandMultiplier == 5.5)
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

            if (playerName == lastly) //lastfixed
            {
                if (this.Table.WinnersCount > 1)
                {
                    if (this.checkWinners.Contains("Player"))
                    {
                        this.Players[0].Chips += this.Table.Pot / this.Table.WinnersCount;

                        //this.form.textBoxPlayerChips.Text = this.Players[0].Chips.ToString();
                        ////pPanel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 1"))
                    {
                        this.Players[1].Chips += this.Table.Pot / this.Table.WinnersCount;

                        //this.form.textBoxBot1Chips.Text = this.Players[1].Chips.ToString();
                        ////b1Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 2"))
                    {
                        this.Players[2].Chips += this.Table.Pot / this.Table.WinnersCount;

                        //this.form.textBoxBot2Chips.Text = this.Players[2].Chips.ToString();
                        ////b2Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 3"))
                    {
                        this.Players[3].Chips += this.Table.Pot / this.Table.WinnersCount;

                        //this.form.textBoxBot3Chips.Text = this.Players[3].Chips.ToString();
                        ////b3Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 4"))
                    {
                        this.Players[4].Chips += this.Table.Pot / this.Table.WinnersCount;

                        //this.form.textBoxBot4Chips.Text = this.Players[4].Chips.ToString();
                        ////b4Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 5"))
                    {
                        this.Players[5].Chips += this.Table.Pot / this.Table.WinnersCount;

                        //this.form.textBoxBot5Chips.Text = this.Players[5].Chips.ToString();
                        ////b5Panel.Visible = true;
                    }
                    ////await Finish(1);
                }

                if (this.Table.WinnersCount == 1)
                {
                    if (this.checkWinners.Contains("Player"))
                    {
                        this.Players[0].Chips += this.Table.Pot;

                        ////await Finish(1);
                        ////pPanel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 1"))
                    {
                        this.Players[1].Chips += this.Table.Pot;

                        ////await Finish(1);
                        ////b1Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 2"))
                    {
                        this.Players[2].Chips += this.Table.Pot;

                        ////await Finish(1);
                        ////b2Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 3"))
                    {
                        this.Players[3].Chips += this.Table.Pot;

                        ////await Finish(1);
                        ////b3Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 4"))
                    {
                        this.Players[4].Chips += this.Table.Pot;

                        ////await Finish(1);
                        ////b4Panel.Visible = true;
                    }

                    if (this.checkWinners.Contains("Bot 5"))
                    {
                        this.Players[5].Chips += this.Table.Pot;

                        ////await Finish(1);
                        ////b5Panel.Visible = true;
                    }
                }
            }
        }

        private async Task CheckRaise(int currentTurn, int raiseTurn)
        {
            if (this.raising)
            {
                this.turnCount = 0;
                this.raising = false;
                this.raisedTurn = currentTurn;
                this.changed = true;
            }
            else
            {
                if (this.turnCount >= this.maxPlayersLeftCount - 1 ||
                    !this.changed && this.turnCount == this.maxPlayersLeftCount)
                {
                    if (currentTurn == this.raisedTurn - 1 ||
                        !this.changed && this.turnCount == this.maxPlayersLeftCount ||
                        this.raisedTurn == 0 && currentTurn == 5)
                    {
                        this.changed = false;
                        this.turnCount = 0;
                        this.Table.LastRaise = 0;
                        this.Table.PokerCall = 0;
                        this.raisedTurn = 123;
                        this.Table.Rounds++;

                        foreach (var player in this.Players)
                        {
                            if (!player.GameEnded)
                            {
                                player.Status = string.Empty;
                            }
                        }

                        this.renderer.SetAllLabelStatus(this.Players);
                    }
                }
            }

            if (this.Table.Rounds == this.flop)
            {
                for (int j = 12; j <= 14; j++)
                {
                    if (this.cardHolder[j].Image == this.deckImages[j])
                    {
                        continue;
                    }

                    this.cardHolder[j].Image = this.deckImages[j];

                    this.Players[0].Call = 0;
                    this.Players[0].Raise = 0;

                    this.Players[1].Call = 0;
                    this.Players[1].Raise = 0;

                    this.Players[2].Call = 0;
                    this.Players[2].Raise = 0;

                    this.Players[3].Call = 0;
                    this.Players[3].Raise = 0;

                    this.Players[4].Call = 0;
                    this.Players[4].Raise = 0;

                    this.Players[5].Call = 0;
                    this.Players[5].Raise = 0;
                }
            }

            if (this.Table.Rounds == this.turn)
            {
                for (int j = 14; j <= 15; j++)
                {
                    if (this.cardHolder[j].Image == this.deckImages[j])
                    {
                        continue;
                    }

                    this.cardHolder[j].Image = this.deckImages[j];

                    this.Players[0].Call = 0;
                    this.Players[0].Raise = 0;

                    this.Players[1].Call = 0;
                    this.Players[1].Raise = 0;
    
                    this.Players[2].Call = 0;
                    this.Players[2].Raise = 0;
                    
                    this.Players[3].Call = 0;
                    this.Players[3].Raise = 0;
                    
                    this.Players[4].Call = 0;
                    this.Players[4].Raise = 0;
                    
                    this.Players[5].Call = 0;
                    this.Players[5].Raise = 0;
                }
            }
        #endregion part1 to fix errors
            // region 2 to fix error - veronika
            #region part2 to fix errors
            if (this.Table.Rounds == this.river)
            {
                for (int j = 15; j <= 16; j++)
                {
                    if (this.cardHolder[j].Image != this.deckImages[j])
                    {
                        this.cardHolder[j].Image = this.deckImages[j];
                        this.Players[0].Call = 0;
                        this.Players[0].Raise = 0;
                        this.Players[1].Call = 0;
                        this.Players[1].Raise = 0;
                        this.Players[2].Call = 0;
                        this.Players[2].Raise = 0;
                        this.Players[3].Call = 0;
                        this.Players[3].Raise = 0;
                        this.Players[4].Call = 0;
                        this.Players[4].Raise = 0;
                        this.Players[5].Call = 0;
                        this.Players[5].Raise = 0;
                    }
                }
            }

            if (this.Table.Rounds == this.end && this.maxPlayersLeftCount == 6)
            {
                string fixedLast = "qwerty";

                // TODO: change to new logic
                foreach (var player in this.Players)
                {
                    if (player.Status != "Fold")
                    {
                        fixedLast = player.Name;
                        this.Rules(player);
                    }
                }

                this.Winner(this.Players[0].PokerHandMultiplier,
                       this.Players[0].CardPower,
                       this.Players[0].Name,
                       this.Players[0].Chips,
                       fixedLast);
                this.Winner(this.Players[1].PokerHandMultiplier,
                       this.Players[1].CardPower,
                       this.Players[1].Name,
                       this.Players[1].Chips,
                       fixedLast);
                this.Winner(this.Players[2].PokerHandMultiplier,
                       this.Players[2].CardPower,
                       this.Players[2].Name,
                       this.Players[2].Chips,
                       fixedLast);
                this.Winner(this.Players[3].PokerHandMultiplier,
                       this.Players[3].CardPower,
                       this.Players[3].Name,
                       this.Players[3].Chips,
                       fixedLast);
                this.Winner(this.Players[4].PokerHandMultiplier,
                       this.Players[4].CardPower,
                       this.Players[4].Name,
                       this.Players[4].Chips,
                       fixedLast);
                this.Winner(this.Players[5].PokerHandMultiplier,
                       this.Players[5].CardPower,
                       this.Players[5].Name,
                       this.Players[5].Chips,
                       fixedLast);

                this.restart = true;
                this.Players[0].Turn = true;

                this.Players[0].GameEnded = false;
                this.Players[1].GameEnded = false;
                this.Players[2].GameEnded = false;
                this.Players[3].GameEnded = false;
                this.Players[4].GameEnded = false;
                this.Players[5].GameEnded = false;

                if (this.Players[0].Chips <= 0)
                {
                    AddChips f2 = new AddChips();
                    f2.ShowDialog();
                    if (f2.AddedChips != 0)
                    {
                        this.Players[0].Chips = f2.AddedChips;
                        this.Players[1].Chips += f2.AddedChips;
                        this.Players[2].Chips += f2.AddedChips;
                        this.Players[3].Chips += f2.AddedChips;
                        this.Players[4].Chips += f2.AddedChips;
                        this.Players[5].Chips += f2.AddedChips;
                        this.Players[0].GameEnded = false;
                        this.Players[0].Turn = true;
                        this.form.buttonRaise.Enabled = true;
                        this.form.buttonFold.Enabled = true;
                        this.form.buttonCheck.Enabled = true;
                        this.form.buttonRaise.Text = "Raise";
                    }
                }

                /* old code for panels
                  this.players[0].CardsPanel.Visible = false;
                  this.players[1].CardsPanel.Visible = false;
                  this.players[2].CardsPanel.Visible = false;
                  this.players[3].CardsPanel.Visible = false;
                  this.players[4].CardsPanel.Visible = false;
                  this.players[5].CardsPanel.Visible = false;
                  */

                foreach (var panel in this.form.PlayersPanels)
                {
                    panel.Visible = false;
                }

                this.Players[0].Call = 0;
                this.Players[1].Call = 0;
                this.Players[2].Call = 0;
                this.Players[3].Call = 0;
                this.Players[4].Call = 0;
                this.Players[5].Call = 0;

                this.Players[0].Raise = 0;
                this.Players[2].Raise = 0;
                this.Players[1].Raise = 0;
                this.Players[3].Raise = 0;
                this.Players[4].Raise = 0;
                this.Players[5].Raise = 0;

                this.lastBotPlayed = 0;
                this.Table.PokerCall = this.Table.BigBlind;
                this.Table.LastRaise = 0;
                this.imageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
                this.playersNotGameEnded.Clear();
                this.Table.Rounds = 0;
                this.Players[0].CardPower = 0;
                this.Players[0].PokerHandMultiplier = -1;
                this.type = 0;

                this.Players[1].CardPower = 0;
                this.Players[2].CardPower = 0;
                this.Players[3].CardPower = 0;
                this.Players[4].CardPower = 0;
                this.Players[5].CardPower = 0;

                this.Players[1].PokerHandMultiplier = -1;
                this.Players[2].PokerHandMultiplier = -1;
                this.Players[3].PokerHandMultiplier = -1;
                this.Players[4].PokerHandMultiplier = -1;
                this.Players[5].PokerHandMultiplier = -1;

                this.playersWithoutChips.Clear();
                this.checkWinners.Clear();
                this.Table.WinnersCount = 0;
                this.winningCards.Clear();
                this.winningCard.Current = 0;
                this.winningCard.Power = 0;

                for (int os = 0; os < 17; os++)
                {
                    this.cardHolder[os].Image = null;
                    this.cardHolder[os].Invalidate();
                    this.cardHolder[os].Visible = false;
                }

                this.form.textBoxPot.Text = "0";
                this.Players[0].Status = string.Empty;
                this.form.PlayersLabelsStatus[0].Text = this.Players[0].Status;

                await this.SetupPokerTable();
                await this.Turns();
            }
        }

        private void FixCall(IPlayer player, int options)
        {
            string playerLableText = this.form.PlayersLabelsStatus[player.Id].Text;

            if (this.Table.Rounds != 4)
            {
                if (options == 1)
                {
                    if (player.Status == "Raise")
                    {
                        // old code: var changeRaise = player.Label.Text.Substring(6);
                        var changeRaise = playerLableText.Substring(6);
                        player.Raise = int.Parse(changeRaise);
                    }

                    if (playerLableText.Contains("Call"))
                    {
                        var changeCall = playerLableText.Substring(5);
                        player.Call = int.Parse(changeCall);
                    }

                    if (playerLableText.Contains("Check"))
                    {
                        player.Raise = 0;
                        player.Call = 0;
                    }
                }

                if (options == 2)
                {
                    if (player.Raise != this.Table.LastRaise &&
                        player.Raise <= this.Table.LastRaise)
                    {
                        this.Table.PokerCall = Convert.ToInt32(this.Table.LastRaise) - player.Raise;
                    }

                    if (player.Call != this.Table.PokerCall ||
                        player.Call <= this.Table.PokerCall)
                    {
                        this.Table.PokerCall = this.Table.PokerCall - player.Call;
                    }

                    if (player.Raise == this.Table.LastRaise &&
                        this.Table.LastRaise > 0)
                    {
                        this.Table.PokerCall = 0;
                        this.form.buttonCall.Enabled = false;
                        this.form.buttonCall.Text = "Callisfuckedup";
                    }
                }
            }
        }

        private async Task AllIn()
        {

            #region All in
            /*
             * old code:
                 * this.players[0].Label.Text.Contains("Raise")
                 * this.players[0].Label.Text.Contains("Call")
             *new code: 
                 *this.players[0].Status.Contains("Raise")
                 *this.players[0].Status.Contains("Raise")
         */

            if (this.Players[0].Chips <= 0 && !this.intsadded)
            {
                if (this.Players[0].Status.Contains("Raise"))
                {
                    this.playersWithoutChips.Add(this.Players[0].Chips);
                    this.intsadded = true;
                }

                if (this.Players[0].Status.Contains("Raise"))
                {
                    this.playersWithoutChips.Add(this.Players[0].Chips);
                    this.intsadded = true;
                }
            }

            this.intsadded = false;

            if (this.Players[1].Chips <= 0 && !this.Players[1].GameEnded)
            {
                if (!this.intsadded)
                {
                    this.playersWithoutChips.Add(this.Players[1].Chips);
                    this.intsadded = true;
                }

                this.intsadded = false;
            }

            if (this.Players[2].Chips <= 0 && !this.Players[2].GameEnded)
            {
                if (!this.intsadded)
                {
                    this.playersWithoutChips.Add(this.Players[2].Chips);
                    this.intsadded = true;
                }

                this.intsadded = false;
            }

            if (this.Players[3].Chips <= 0 &&
                !this.Players[3].GameEnded)
            {
                if (!this.intsadded)
                {
                    this.playersWithoutChips.Add(this.Players[3].Chips);
                    this.intsadded = true;
                }

                this.intsadded = false;
            }

            if (this.Players[4].Chips <= 0 &&
                !this.Players[4].GameEnded)
            {
                if (!this.intsadded)
                {
                    this.playersWithoutChips.Add(this.Players[4].Chips);
                    this.intsadded = true;
                }

                this.intsadded = false;
            }

            if (this.Players[5].Chips <= 0 &&
                !this.Players[5].GameEnded)
            {
                if (!this.intsadded)
                {
                    this.playersWithoutChips.Add(this.Players[5].Chips);
                    this.intsadded = true;
                }
            }

            if (this.playersWithoutChips.ToArray().Length == this.maxPlayersLeftCount)
            {
                await this.Finish(2);
            }
            else
            {
                this.playersWithoutChips.Clear();
            }
            #endregion

            int playersNotGameEndedCount = this.playersNotGameEnded.Count(x => x == false);

            #region LastManStanding
            if (playersNotGameEndedCount == 1)
            {
                int index = this.playersNotGameEnded.IndexOf(false);

                if (index == 0)
                {
                    this.Players[0].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.form.textBoxPlayerChips.Text = this.Players[0].Chips.ToString();
                    this.form.PlayersPanels[0].Visible = true;
                    MessageBox.Show("Player Wins");
                }

                if (index == 1)
                {
                    this.Players[1].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.form.textBoxBot1Chips.Text = this.Players[1].Chips.ToString();
                    this.form.PlayersPanels[1].Visible = true;
                    MessageBox.Show("Bot 1 Wins");
                }

                if (index == 2)
                {
                    this.Players[2].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.form.textBoxBot2Chips.Text = this.Players[2].Chips.ToString();
                    this.form.PlayersPanels[2].Visible = true;
                    MessageBox.Show("Bot 2 Wins");
                }

                if (index == 3)
                {
                    this.Players[3].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.form.textBoxBot3Chips.Text = this.Players[3].Chips.ToString();
                    this.form.PlayersPanels[3].Visible = true;
                    MessageBox.Show("Bot 3 Wins");
                }

                if (index == 4)
                {
                    this.Players[4].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.form.textBoxBot4Chips.Text = this.Players[4].Chips.ToString();
                    this.form.PlayersPanels[4].Visible = true;
                    MessageBox.Show("Bot 4 Wins");
                }

                if (index == 5)
                {
                    this.Players[5].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.form.textBoxBot5Chips.Text = this.Players[5].Chips.ToString();
                    this.form.PlayersPanels[5].Visible = true;
                    MessageBox.Show("Bot 5 Wins");
                }

                for (int j = 0; j <= 16; j++)
                {
                    this.cardHolder[j].Visible = false;
                }

                await this.Finish(1);
            }
            ////TODO find what is intsadded
            this.intsadded = false;
            #endregion

            #region FiveOrLessLeft
            if (playersNotGameEndedCount < 6 &&
                playersNotGameEndedCount > 1 &&
                this.Table.Rounds >= this.end)
            {
                await this.Finish(2);
            }
            #endregion
        }

        private async Task Finish(int n)
        {
            if (n == 2)
            {
                ////Fix can be substitude by Set
                this.FixWinners();
            }

            ////Resets all fields
            //premesteno w renderer Clear;
            //foreach (var panel in this.form.PlayersPanels)
            //{
            //    panel.Visible = false;
            //}
            // this.form.height = 0;
            //this.form.width = 0;
            // this.form.textBoxPot.Text = "0";
            /* //for (int os = 0; os < this.PepsterDealtCards.Length; os++)
            //{
            //    this.cardHolder[os].Image = null;
            //    this.cardHolder[os].Invalidate();
            //    this.cardHolder[os].Visible = false;
            //}
             */

            this.Table.PokerCall = this.Table.BigBlind;
            this.Table.LastRaise = 0;
            this.foldedPlayers = 5;
            this.type = 0;
            this.Table.Rounds = 0;

            this.Table.LastRaise = 0;

            this.restart = false;
            this.raising = false;


            this.Table.WinnersCount = 0;
            this.flop = 1;
            this.turn = 2;
            this.river = 3;
            this.end = 4;
            this.maxPlayersLeftCount = 6;
            this.lastBotPlayed = 123;
            this.raisedTurn = 1;
            this.playersNotGameEnded.Clear();
            this.checkWinners.Clear();
            this.playersWithoutChips.Clear();
            this.winningCards.Clear();
            this.winningCard.Current = 0;
            this.winningCard.Power = 0;
            this.t = 60;
            this.turnCount = 0;

            foreach (var player in this.Players)
            {
                if (player is Gamer)
                {
                    player.Turn = true;
                }
                else
                {
                    player.Turn = true;
                }
                player.CardPower = 0;
                player.PokerHandMultiplier = -1;
                player.GameEnded = false;
                player.Status = string.Empty;
                player.Folded = false;
                player.Call = 0;
                player.Raise = 0;
                this.renderer.Draw(player);
            }

            if (this.Players[0].Chips <= 0)
            {
                AddChips addChipsForm = new AddChips();
                addChipsForm.ShowDialog();
                if (addChipsForm.AddedChips != 0)
                {
                    this.Players[0].Chips = addChipsForm.AddedChips;
                    this.Players[1].Chips += addChipsForm.AddedChips;
                    this.Players[2].Chips += addChipsForm.AddedChips;
                    this.Players[3].Chips += addChipsForm.AddedChips;
                    this.Players[4].Chips += addChipsForm.AddedChips;
                    this.Players[5].Chips += addChipsForm.AddedChips;
                    this.Players[0].GameEnded = false;
                    this.Players[0].Turn = true;
                    this.form.buttonRaise.Enabled = true;
                    this.form.buttonFold.Enabled = true;
                    this.form.buttonCheck.Enabled = true;
                    this.form.buttonRaise.Text = "Raise";
                }
            }

            //nqma da se polzwa zatowa e zakomentiran
            //this.imageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);


            await this.SetupPokerTable();
            ////await Turns();
        }

        private void FixWinners()
        {
            this.winningCards.Clear();
            this.winningCard.Current = 0;
            this.winningCard.Power = 0;
            string fixedLast = "qwerty";

            //TODO: change to new logic - > DONE -> see region " code to be deleted" new code is the foreach
            #region old code to be deleted

            //if (!this.players[0].Label.Text.Contains("Fold"))
            //{
            //    fixedLast = "Player";
            //    this.Rules(this.gamer);
            //}

            //if (!this.labelBot1Status.Text.Contains("Fold"))
            //{
            //    fixedLast = "Bot 1";
            //    this.Rules(this.bot1);
            //}

            //if (!this.labelBot2Status.Text.Contains("Fold"))
            //{
            //    fixedLast = "Bot 2";
            //    this.Rules(this.bot2);
            //}

            //if (!this.labelBot3Status.Text.Contains("Fold"))
            //{
            //    fixedLast = "Bot 3";
            //    this.Rules(this.bot3);
            //}

            //if (!this.labelBot4Status.Text.Contains("Fold"))
            //{
            //    fixedLast = "Bot 4";
            //    this.Rules(this.bot4);
            //}

            //if (!this.labelBot5Status.Text.Contains("Fold"))
            //{
            //    fixedLast = "Bot 5";
            //    this.Rules(this.bot5);
            //}

            #endregion
            foreach (var player in this.Players)
            {
                if (player.Status == "Fold")
                {
                    fixedLast = player.Name;
                    this.Rules(player);
                }
            }

            this.Winner(this.Players[0].PokerHandMultiplier,
                   this.Players[0].CardPower,
                   "Player",
                   this.Players[0].Chips,
                   fixedLast);
            this.Winner(this.Players[1].PokerHandMultiplier,
                   this.Players[1].CardPower,
                   "Bot 1",
                   this.Players[1].Chips,
                   fixedLast);
            this.Winner(this.Players[2].PokerHandMultiplier,
                   this.Players[2].CardPower,
                   "Bot 2",
                   this.Players[2].Chips,
                   fixedLast);
            this.Winner(this.Players[3].PokerHandMultiplier,
                   this.Players[3].CardPower,
                   "Bot 3",
                   this.Players[3].Chips,
                   fixedLast);
            this.Winner(this.Players[4].PokerHandMultiplier,
                   this.Players[4].CardPower,
                   "Bot 4",
                   this.Players[4].Chips,
                   fixedLast);
            this.Winner(this.Players[5].PokerHandMultiplier,
                   this.Players[5].CardPower,
                   "Bot 5",
                   this.Players[5].Chips,
                   fixedLast);
        }

        public void UpdateControls()
        {
            this.renderer.Draw(this.Players);
            this.renderer.Draw(this.PepsterDealtCards);

            if (this.Players[0].Chips <= 0)
            {
                var gamer = (Gamer)this.Players[0];
                gamer.Turn = false;
                gamer.GameEnded = true;
                gamer.CanCall = false;
                gamer.CanCheck = false;
                gamer.CanRaise = false;
                gamer.CanFold = false;
            }

            this.renderer.Draw(this.Players);

            if (this.Players[0].Chips >= this.Table.PokerCall)
            {
                this.form.buttonCall.Text = "Call " + this.Table.PokerCall.ToString();
            }
            else
            {
                this.form.buttonCall.Text = "All in";
                this.form.buttonRaise.Enabled = false;
            }

            if (this.Table.PokerCall > 0)
            {
                this.form.buttonCheck.Enabled = false;
            }

            if (this.Table.PokerCall <= 0)
            {
                this.form.buttonCheck.Enabled = true;
                this.form.buttonCall.Text = "Call";
                this.form.buttonCall.Enabled = false;
            }



            int parsedValue;

            if (this.form.textBoxRaise.Text != "" && int.TryParse(this.form.textBoxRaise.Text, out parsedValue))
            {
                if (this.Players[0].Chips <= int.Parse(this.form.textBoxRaise.Text))
                {
                    this.form.buttonRaise.Text = "All in";
                }
                else
                {
                    this.form.buttonRaise.Text = "Raise";
                }
            }

            if (this.Players[0].Chips < this.Table.PokerCall)
            {
                this.form.buttonRaise.Enabled = false;
            }
        }
            #endregion part2 to fix errors
        //region 3 to fix errors - plamena
        #region part3 to fix errors

        //// Plamena

        #region AI logic
        //// AI   (     2,      3, ref    bot1Chips, ref this.players[1].Turn,ref this.players[1].GameEnded, labelBot1Status,       0, this.bot1CardPower, this.bot1HandMultiplier);
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

        private void AI(IPlayer player)
        {
            // TODO: remove first and second numeration
            var firstCardNumeration = player.FirstCardPosition;
            var secondNumeration = player.SecondCardPosition;
            if (!player.GameEnded)
            {
                switch (player.PokerHandMultiplier.ToString(CultureInfo.CreateSpecificCulture("en-GB")))
                {
                    case "-1":
                        this.AIHighCard(player);
                        break;
                    case "0":
                        this.AIPairTable(player);
                        break;
                    case "1":
                        this.AIPairHand(player);
                        break;
                    case "2":
                        this.AITwoPair(player);
                        break;
                    case "3":
                        this.AIThreeOfAKind(player);
                        break;
                    case "4":
                        this.AIStraight(player);
                        break;
                    case "5":
                    case "5.5":
                        this.AIFlush(player);
                        break;
                    case "6":
                        this.AIFullHouse(player);
                        break;
                    case "7":
                        this.AIFourOfAKind(player);
                        break;
                    case "8":
                    case "9":
                        this.AIStraightFlush(player);
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
            this.AIHP(player, 20, 25);
        }

        //// wika se ot AI this.AIPairTable(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika HP
        private void AIPairTable(IPlayer player)
        {
            this.AIHP(player, 16, 25);
        }

        //// wika se ot AI this.AIPairHand(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
        private void AIPairHand(IPlayer player)
        {
            Random rPair = new Random();

            int rCall = rPair.Next(10, 16);

            int rRaise = rPair.Next(10, 13);

            if (player.CardPower <= 199 &&
                player.CardPower >= 140)
            {
                this.AIPH(player, rCall, 6, rRaise);
            }

            if (player.CardPower <= 139 &&
                player.CardPower >= 128)
            {
                this.AIPH(player, rCall, 7, rRaise);
            }

            if (player.CardPower < 128 &&
                player.CardPower >= 101)
            {
                this.AIPH(player, rCall, 9, rRaise);
            }
        }

        //// wika se ot AI this.AITwoPair(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
        private void AITwoPair(IPlayer player)
        {
            Random rPair = new Random();
            int rCall = rPair.Next(6, 11);
            int rRaise = rPair.Next(6, 11);

            if (player.CardPower <= 290 &&
                player.CardPower >= 246)
            {
                this.AIPH(player, rCall, 3, rRaise);
            }

            if (player.CardPower <= 244 &&
                player.CardPower >= 234)
            {
                this.AIPH(player, rCall, 4, rRaise);
            }

            if (player.CardPower < 234 &&
                player.CardPower >= 201)
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

            if (player.CardPower <= 390 &&
                player.CardPower >= 330)
            {
                this.AISmooth(player, tCall, tRaise);
            }

            if (player.CardPower <= 327 &&
                player.CardPower >= 321) //10  8
            {
                this.AISmooth(player, tCall, tRaise);
            }

            if (player.CardPower < 321 &&
                player.CardPower >= 303) //7 2
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

            if (player.CardPower <= 480 &&
                player.CardPower >= 410)
            {
                this.AISmooth(player, sCall, sRaise);
            }

            if (player.CardPower <= 409 &&
                player.CardPower >= 407) //10  8
            {
                this.AISmooth(player, sCall, sRaise);
            }

            if (player.CardPower < 407 &&
                player.CardPower >= 404)
            {
                this.AISmooth(player, sCall, sRaise);
            }
        }

        //// wika se ot AI this.AIFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIFlush(IPlayer player)
        {
            Random fsh = new Random();
            int fCall = fsh.Next(2, 6);
            int fRaise = fsh.Next(3, 7);
            this.AISmooth(player, fCall, fRaise);
        }

        //// wika se ot AI : this.AIFullHouse(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth
        private void AIFullHouse(IPlayer player)
        {
            Random random = new Random();
            int fhCall = random.Next(1, 5);
            int fhRaise = random.Next(2, 6);

            if (player.CardPower <= 626 &&
                player.CardPower >= 620)
            {
                this.AISmooth(player, fhCall, fhRaise);
            }

            if (player.CardPower < 620 &&
                player.CardPower >= 602)
            {
                this.AISmooth(player, fhCall, fhRaise);
            }
        }

        //// wika se ot AI: this.AIFourOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
        private void AIFourOfAKind(IPlayer player)
        {
            Random random = new Random();
            int fkCall = random.Next(1, 4);
            int fkRaise = random.Next(2, 5);
            if (player.CardPower <= 752 &&
                player.CardPower >= 704)
            {
                this.AISmooth(player, fkCall, fkRaise);
            }
        }

        //// wika se ot AI: this.AIStraightFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth 
        private void AIStraightFlush(IPlayer player)
        {
            Random random = new Random();
            int sfCall = random.Next(1, 3);
            int sfRaise = random.Next(1, 3);
            if (player.CardPower <= 913 &&
                player.CardPower >= 804)
            {
                this.AISmooth(player, sfCall, sfRaise);
            }
        }

        /* wika se ot: AIHighCard, AIPairTable, 
         randoma e ot 1-3
         ako Table.PokerCall <= 0 bota igrae CHeck
         ako Table.PokerCall > 0 w zawisimost ot randoma ima 3 wyzmovnosti:
         * 1 -> move da igrae Call ili Fold
         * 2 -> move da igrae Call ili Fold
         * 3 -> move da wdigne ili da Fold 
         */

        private void AIHP(IPlayer player, int n, int n1)
        {
            Random rand = new Random();

            //// staro:int rnd = rand.Next(1, 4); t.kato rnd = 4 nikyde ne se polzwa
            int rnd = rand.Next(1, 4);

            if (this.Table.PokerCall <= 0)
            {
                // bota igrae CHeck
                this.AICheck(player);
            }

            if (this.Table.PokerCall > 0)
            {
                if (rnd == 1)
                {
                    if (this.Table.PokerCall <= AIRoundNumber(player.Chips, n))
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
                    if (this.Table.PokerCall <= AIRoundNumber(player.Chips, n1))
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
                if (this.Table.LastRaise == 0)
                {
                    ////smqta s kolko da wdigne bota
                    this.Table.LastRaise = this.Table.PokerCall * 2;

                    ////cbota igrae Raise
                    this.AIRaised(player);
                }
                else
                {
                    if (this.Table.LastRaise <= AIRoundNumber(player.Chips, n))
                    {
                        this.Table.LastRaise = this.Table.PokerCall * 2;
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
         ako  this.Table.Rounds < 2 
            ako Table.PokerCall <= 0 bota igrae CHeck
            ako Table.PokerCall > 0 w zawisimost ot matematikata na chipowete move da FOLD, CALL, RASE
         ako this.Table.Rounds >= 2
                ako Table.PokerCall <= 0 bota igrae Raise
                ako Table.PokerCall > 0 w zawisimost ot matematikata na chipowete i rnd, move da FOLD, CALL, RASE
             ako botChips <= 0 => setwa botEndGame = true;
         */

        private void AIPH(IPlayer player, int n, int n1, int r)
        {
            Random rand = new Random();
            int rnd = rand.Next(1, 3);

            if (this.Table.Rounds < 2)
            {
                if (this.Table.PokerCall <= 0)
                {
                    this.AICheck(player);
                }

                if (this.Table.PokerCall > 0)
                {
                    if (this.Table.PokerCall >= AIRoundNumber(player.Chips, n1))
                    {
                        this.AIFold(player);
                    }

                    if (this.Table.LastRaise > AIRoundNumber(player.Chips, n))
                    {
                        this.AIFold(player);
                    }

                    if (!player.GameEnded)
                    {
                        if (this.Table.PokerCall >= AIRoundNumber(player.Chips, n) &&
                            this.Table.PokerCall <= AIRoundNumber(player.Chips, n1))
                        {
                            this.AICall(player);
                        }

                        if (this.Table.LastRaise <= AIRoundNumber(player.Chips, n) &&
                            this.Table.LastRaise >= AIRoundNumber(player.Chips, n) / 2)
                        {
                            this.AICall(player);
                        }

                        if (this.Table.LastRaise <= AIRoundNumber(player.Chips, n) / 2)
                        {
                            if (this.Table.LastRaise > 0)
                            {
                                this.Table.LastRaise = AIRoundNumber(player.Chips, n);
                                this.AIRaised(player);
                            }
                            else
                            {
                                this.Table.LastRaise = this.Table.PokerCall * 2;
                                this.AIRaised(player);
                            }
                        }
                    }
                }
            }

            if (this.Table.Rounds >= 2)
            {
                if (this.Table.PokerCall > 0)
                {
                    if (this.Table.PokerCall >= AIRoundNumber(player.Chips, n1 - rnd))
                    {
                        this.AIFold(player);
                    }

                    if (this.Table.LastRaise > AIRoundNumber(player.Chips, n - rnd))
                    {
                        this.AIFold(player);
                    }

                    if (!player.GameEnded)
                    {
                        if (this.Table.PokerCall >= AIRoundNumber(player.Chips, n - rnd) &&
                            this.Table.PokerCall <= AIRoundNumber(player.Chips, n1 - rnd))
                        {
                            this.AICall(player);
                        }

                        if (this.Table.LastRaise <= AIRoundNumber(player.Chips, n - rnd) &&
                            this.Table.LastRaise >= AIRoundNumber(player.Chips, n - rnd) / 2)
                        {
                            this.AICall(player);
                        }

                        if (this.Table.LastRaise <= AIRoundNumber(player.Chips, n - rnd) / 2)
                        {
                            if (this.Table.LastRaise > 0)
                            {
                                this.Table.LastRaise = AIRoundNumber(player.Chips, n - rnd);
                                this.AIRaised(player);
                            }
                            else
                            {
                                this.Table.LastRaise = this.Table.PokerCall * 2;
                                this.AIRaised(player);
                            }
                        }
                    }
                }

                if (this.Table.PokerCall <= 0)
                {
                    this.Table.LastRaise = AIRoundNumber(player.Chips, r - rnd);
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
         ako Table.PokerCall <= 0 bota igrae CHeck
         ako Table.PokerCall > 0 w zawisimost ot matematikata na chipowete move da CALL, RASE
         ako botChips <= 0 => setwa botEndGame = true;
         */

        private void AISmooth(IPlayer player, int n, int r)
        {
            //// star kod - > zakomentiran t.kato ne se polzwa: 
            //// Random rand = new Random();
            //// int rnd = rand.Next(1, 3);
            if (this.Table.PokerCall <= 0)
            {
                this.AICheck(player);
            }
            else
            {
                if (this.Table.PokerCall >= AIRoundNumber(player.Chips, n))
                {
                    if (player.Chips > this.Table.PokerCall)
                    {
                        this.AICall(player);
                    }
                    else if (player.Chips <= this.Table.PokerCall)
                    {
                        this.raising = false;
                        player.Turn = false;
                        player.Chips = 0;
                        player.Status = "Call " + player.Chips;
                        this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
                        this.form.textBoxPot.Text = (int.Parse(this.form.textBoxPot.Text) + player.Chips).ToString();
                    }
                }
                else
                {
                    if (this.Table.LastRaise > 0)
                    {
                        if (player.Chips >= this.Table.LastRaise * 2)
                        {
                            this.Table.LastRaise *= 2;
                            this.AIRaised(player);
                        }
                        else
                        {
                            this.AICall(player);
                        }
                    }
                    else
                    {
                        this.Table.LastRaise = this.Table.PokerCall * 2;
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
            this.raising = false;
            player.Status = "Fold";
            this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
            player.Turn = false;
            player.Folded = true;
            player.GameEnded = true;
        }

        //// вика се от AIHP, AIPH, AISmooth  this.AICheck(ref bothTurn, sStatus);

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
            player.Status = "Check";
            this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
            player.Turn = false;
            this.raising = false;
        }

        //// вика се от  AIHP AIPH, AISmooth this.AICall(ref botChips, ref bothTurn, sStatus);
        private void AICall(IPlayer player)
        {
            this.raising = false;
            player.Turn = false;
            player.Chips -= this.Table.PokerCall;
            player.Status = "Call " + this.Table.PokerCall;
            this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
            this.form.textBoxPot.Text = (int.Parse(this.form.textBoxPot.Text) + this.Table.PokerCall).ToString();
        }

        //// вика се от  AIHP AIPH, AISmooth this.AIRaised(ref botChips, ref bothTurn, sStatus);
        private void AIRaised(IPlayer player)
        {
            player.Chips -= Convert.ToInt32(this.Table.LastRaise);
            player.Status = "Raise " + this.Table.LastRaise;
            this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
            this.form.textBoxPot.Text = (int.Parse(this.form.textBoxPot.Text) + Convert.ToInt32(this.Table.LastRaise)).ToString();
            this.Table.PokerCall = Convert.ToInt32(this.Table.LastRaise);
            this.raising = true;
            player.Turn = false;
        }

        ////вика се от  AIHP AIPH, AISmooth  this.Table.PokerCall <= AIRoundNumber(botChips, n) числото е различно има и математика
        private static double AIRoundNumber(int botChips, int n)
        {
            double a = Math.Round((botChips / n) / 100d, 0) * 100;
            return a;
        }
        #endregion

        /// <summary>
        /// It's called when Gammer push Fold button
        /// </summary>
        public void GammerPlayesFold()
        {
            this.Players[0].Status = "Fold";
            this.PlayersLabelsStatus[0].Text = this.Players[0].Status;
            this.Players[0].Turn = false;
            this.Players[0].GameEnded = true;
        }

        public void GammerPlayesCheck()
        {
            if (this.Table.PokerCall <= 0)
            {
                this.Players[0].Turn = false;
                this.Players[0].Status = "Check";
                this.PlayersLabelsStatus[0].Text = this.Players[0].Status;
            }
            else
            {
                this.buttonCheck.Enabled = false;
            }
        }

        public void GammerMoveTimeExpired()
        {
            this.Players[0].GameEnded = true;
        }

        public void GammerPlayesCall()
        {
            this.Rules(this.Players[0]);

            if (this.Players[0].Chips >= this.Table.PokerCall)
            {
                this.Players[0].Chips -= this.Table.PokerCall;
                this.textBoxPlayerChips.Text = "Chips : " + this.Players[0].Chips.ToString();

                if (this.textBoxPot.Text != "")
                {
                    this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + this.Table.PokerCall).ToString();
                }
                else
                {
                    this.textBoxPot.Text = this.Table.PokerCall.ToString();
                }

                this.Players[0].Turn = false;
                this.Players[0].Status = "Call " + this.Table.PokerCall;
                this.PlayersLabelsStatus[0].Text = this.Players[0].Status;
                this.Players[0].Call = this.Table.PokerCall;
            }
            else if (this.Players[0].Chips <= this.Table.PokerCall &&
                     this.Table.PokerCall > 0)
            {
                this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + this.Players[0].Chips).ToString();
                this.Players[0].Status = "All in " + this.Players[0].Chips;
                this.PlayersLabelsStatus[0].Text = this.Players[0].Status;
                this.Players[0].Chips = 0;
                this.textBoxPlayerChips.Text = "Chips : " + this.Players[0].Chips.ToString();
                this.Players[0].Turn = false;
                this.buttonFold.Enabled = false;
                this.Players[0].Call = this.Players[0].Chips;
            }
        }

        public void GammerPlayesRaise()
        {
            this.Rules(this.Players[0]);
            int parsedValue;

            if (this.textBoxRaise.Text != "" &&
                int.TryParse(this.textBoxRaise.Text, out parsedValue))
            {
                if (this.Players[0].Chips > this.Table.PokerCall)
                {
                    if (this.Table.LastRaise * 2 > int.Parse(this.textBoxRaise.Text))
                    {
                        this.textBoxRaise.Text = (this.Table.LastRaise * 2).ToString();
                        MessageBox.Show("You must raise atleast twice as the PokerHandMultiplier raise !");
                        return;
                    }
                    else
                    {
                        if (this.Players[0].Chips >= int.Parse(this.textBoxRaise.Text))
                        {
                            this.Table.PokerCall = int.Parse(this.textBoxRaise.Text);
                            this.Table.LastRaise = int.Parse(this.textBoxRaise.Text);
                            this.Players[0].Status = "Raise " + this.Table.PokerCall.ToString();
                            this.PlayersLabelsStatus[0].Text = this.Players[0].Status;
                            this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + this.Table.PokerCall).ToString();
                            this.buttonCall.Text = "Call";
                            this.Players[0].Chips -= int.Parse(this.textBoxRaise.Text);
                            this.raising = true;
                            this.lastBotPlayed = 0;
                            this.Players[0].Raise = Convert.ToInt32(this.Table.LastRaise);
                        }
                        else
                        {
                            this.Table.PokerCall = this.Players[0].Chips;
                            this.Table.LastRaise = this.Players[0].Chips;
                            this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + this.Players[0].Chips).ToString();
                            this.Players[0].Status = "Raise " + this.Table.PokerCall.ToString();
                            this.PlayersLabelsStatus[0].Text = this.Players[0].Status;

                            this.Players[0].Chips = 0;
                            this.raising = true;
                            this.lastBotPlayed = 0;
                            this.Players[0].Raise = Convert.ToInt32(this.Table.LastRaise);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("This is a number only field");
                return;
            }

            this.Players[0].Turn = false;
        }

        public void GammerAddsChips()
        {
            if (this.textBoxAddChips.Text == "")
            {
            }
            else
            {
                this.Players[0].Chips += int.Parse(this.textBoxAddChips.Text);
                this.Players[1].Chips += int.Parse(this.textBoxAddChips.Text);
                this.Players[2].Chips += int.Parse(this.textBoxAddChips.Text);
                this.Players[3].Chips += int.Parse(this.textBoxAddChips.Text);
                this.Players[4].Chips += int.Parse(this.textBoxAddChips.Text);
                this.Players[5].Chips += int.Parse(this.textBoxAddChips.Text);
            }

            this.textBoxPlayerChips.Text = "Chips : " + this.Players[0].Chips.ToString();
        }
    }
}
        #endregion part3 to fix errors

