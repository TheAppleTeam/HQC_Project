namespace Poker.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Exception;
    using GameObjects;
    using GameObjects.Cards;
    using GameObjects.Player;
    using UI;

    public class GameEngine
    {
        //// used in CheckRaise method :  (Table.Rounds == River); in Finish method is seted again on 3;
        private readonly int cardFlop = 1;
        private readonly int cardTurn = 2;
        private readonly int cardRiver = 3;
        private readonly int cardEnd = 4;

        private readonly Random random = new Random();
        private readonly IRenderer renderer;
        private readonly IInputHandlerer inputHandlerer;


        private bool intsadded;
        private bool changed;
        private double type;

        private List<WinningHand> winningCards = new List<WinningHand>();
        private List<bool?> playersNotGameEnded = new List<bool?>();
        private List<string> checkWinners = new List<string>();
        private List<int> playersWithoutChips = new List<int>();

        private WinningHand winningCard;

        /// <summary>
        /// Array of Integers -> dealed cars. Array.Lengt = 17
        /// towa sa nomerata na fajlowete, pri men sa ID-ta
        /// </summary>
        private int[] dealtCardsNumbers = new int[17];


        public GameEngine(IRenderer renderer, IInputHandlerer inputHandlerer)
        {
            this.Table = new Table();
            this.Players = new IPlayer[GlobalConstants.PlayersCount];
            this.InitializePlayers();
            this.renderer = renderer;
            this.inputHandlerer = inputHandlerer;
            this.PepsterDeck = new PepsterCard[52];
            this.PepsterDealtCards = new PepsterCard[17];
            this.SetAllPepsterDeck();
            this.GameEnd = false;
        }

        public Table Table { get; set; }

        public Gamer Gamer { get; set; }

        public IPlayer[] Players { get; private set; }

        public PepsterCard[] PepsterDeck { get; private set; }

        public PepsterCard[] PepsterDealtCards { get; set; }

        private bool GameEnd { get; set; }

        public void GameInit()
        {
            this.Table.PokerCall = GlobalConstants.InitialBigBlind;

            this.SetupPokerTable();

            this.renderer.Draw(this.Players);
        }


        public void UpdateControls()
        {
            if (this.Gamer.Chips <= 0)
            {
                this.Gamer.Turn = false;
                this.Gamer.GameEnded = true;
                this.Gamer.CanCall = false;
                this.Gamer.CanCheck = false;
                this.Gamer.CanRaise = false;
                this.Gamer.CanFold = false;
            }

            if (this.Gamer.Chips >= this.Table.PokerCall)
            {
                this.Gamer.CanCall = true;
                this.Table.PosibleCall = "Call" + this.Table.PokerCall;
            }
            else
            {
                this.Table.PosibleCall = "Call" + this.Table.PokerCall;
                this.Gamer.CanRaise = false;
            }

            if (this.Table.PokerCall > 0)
            {
                this.Gamer.CanCheck = false;
            }

            if (this.Table.PokerCall <= 0)
            {
                this.Table.PosibleCall = "Call";
                this.Gamer.CanCheck = true;
                this.Gamer.CanCall = false;
            }

            if (this.Gamer.CanRaise)
            {
                if (this.Gamer.Chips <= this.Table.LastRaise)
                {
                    this.Gamer.IsAllIn = true;
                    this.Table.PosibleRaise = "All in";
                }
                else
                {
                    this.Gamer.CanRaise = true;
                    this.Table.PosibleRaise = "Raise";
                }
            }

            if (this.Gamer.Chips < this.Table.PokerCall)
            {
                this.Gamer.CanRaise = false;
            }

            this.renderer.Draw(this.Players);
            this.renderer.Draw(this.PepsterDealtCards);
            this.renderer.Draw(this.Table);
        }

        private void SetAllPepsterDeck()
        {
            int pepsterDeckIndex = 0;
            int idCounter = 5;

            for (int baseCardRank = 0; baseCardRank < 13; baseCardRank++)
            {
                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Clubs,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                pepsterDeckIndex++;

                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Diamonds,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                pepsterDeckIndex++;

                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Hearts,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                pepsterDeckIndex++;

                this.PepsterDeck[pepsterDeckIndex] = new PepsterCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Spades,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                pepsterDeckIndex++;
            }
        }

        #region Refactored Shuffle to SetupPokerTable

        /// <summary>
        /// Setups a poker table with all the players holders, cards on the table and buttons
        /// </summary>
        /// <returns></returns>
        private async Task SetupPokerTable()
        {
            this.playersNotGameEnded.Add(this.Gamer.GameEnded);
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

                await Task.Delay(GlobalConstants.DealingCardsDelay);
            }

            this.EnablingFormMinimizationAndMaximization();

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
        }

        /// <summary>
        /// Randomise the cars and take 17 to deal. 
        /// Set to gamer Cards.IsVisible = true; -> so Cards faces to be shown;
        /// Call renderer to create Images for dealt cards
        /// </summary>
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
            this.PepsterDealtCards[0].IsVisible = true;
            this.PepsterDealtCards[1].IsVisible = true;
            this.renderer.GetCardsImages(this.PepsterDealtCards);
        }

        /// <summary>
        /// Check if all bots are out of the game
        /// </summary>
        private void CheckForGameEnd()
        {
            for (int botId = 1; botId < this.Players.Length; botId++)
            {
                if (this.Players[botId].GameEnded)
                {
                    this.Table.FoldedBots++;
                }
            }

            if (this.Table.FoldedBots == 5)
            {
                this.renderer.ShowMessage("Would You Like To Play Again ?", "You Won , Congratulations !");
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
            if (!this.GameEnd)
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

        public async Task Turns()
        {
            #region Rotating

            if (!this.Gamer.GameEnded && this.Gamer.Turn)
            {
                this.CallAllPlayerActionsOnTurn(this.Gamer);
            }
            else
            {
                await this.AllIn();

                //ako gamera e umrql zashtoto e foldnal
                if (this.Gamer.GameEnded && !this.Gamer.Folded)
                {
                    if (this.Gamer.IsAllIn == false || this.Gamer.IsAllIn == false)
                    {
                        this.RemovePlayerFromTheGame(0);
                    }
                }

                await this.CheckRaise(0);

                this.Gamer.CanRaise = true;
                this.Gamer.CanCall = true;
                this.Gamer.CanCheck = true;
                this.Gamer.CanFold = true;

                this.renderer.ShowOrHidePlayersButtons(this.Gamer);
                this.renderer.HideGamerTurnTimer();

                this.Players[1].Turn = true;

                for (int i = 1; i < this.Players.Length; i++)
                {
                    if (!this.Players[i].GameEnded)
                    {
                        if (this.Players[i].Turn)
                        {
                            this.CallAllBotActionsOnTheirTurn(this.Players[i]);
                        }
                    }

                    if (this.Players[i].GameEnded && !this.Players[i].Folded)
                    {
                        ////TODO _ActivePlayers rename
                        this.RemovePlayerFromTheGame(i);
                    }

                    if (this.Players[i].GameEnded || !this.Players[i].Turn)
                    {
                        await this.CheckRaise(i);
                        if (i + 1 == this.Players.Length)
                        {
                            this.Gamer.Turn = true;
                        }
                        else
                        {
                            this.Players[i + 1].Turn = true;
                        }
                    }
                }

                if (this.Gamer.GameEnded && !this.Gamer.Folded)
                {
                    if (this.Gamer.IsAllIn == false)
                    {
                        this.RemovePlayerFromTheGame(0);
                    }
                }
            #endregion

                await this.AllIn();

                // game loop => while the game end is not true call turns again
                if (this.GameEnd == false)
                {
                    await this.Turns();
                }

                this.GameEnd = false;
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

            if (!player.Folded || (player.FirstCardPosition == 0 && player.SecondCardPosition == 1))
            {
                #region Variables

                PepsterCard[] cardsOnTable = new PepsterCard[5];
                PepsterCard[] cardsOnTableWithPlayerCards = new PepsterCard[7];

                cardsOnTableWithPlayerCards[0] = this.PepsterDealtCards[player.FirstCardPosition];
                cardsOnTableWithPlayerCards[1] = this.PepsterDealtCards[player.SecondCardPosition];

                cardsOnTable[0] = cardsOnTableWithPlayerCards[2] = this.PepsterDealtCards[12];
                cardsOnTable[1] = cardsOnTableWithPlayerCards[3] = this.PepsterDealtCards[13];
                cardsOnTable[2] = cardsOnTableWithPlayerCards[4] = this.PepsterDealtCards[14];
                cardsOnTable[3] = cardsOnTableWithPlayerCards[5] = this.PepsterDealtCards[15];
                cardsOnTable[4] = cardsOnTableWithPlayerCards[6] = this.PepsterDealtCards[16];

                var cardsOfClubs = cardsOnTableWithPlayerCards.Where(o => o.Suit == CardSuit.Clubs).OrderByDescending(c => c.Rank).ToArray();
                var cardsOfDiamonds = cardsOnTableWithPlayerCards.Where(o => o.Suit == CardSuit.Diamonds).OrderByDescending(c => c.Rank).ToArray();
                var cardsOfHearts = cardsOnTableWithPlayerCards.Where(o => o.Suit == CardSuit.Hearts).OrderByDescending(c => c.Rank).ToArray();
                var cardsOfSpades = cardsOnTableWithPlayerCards.Where(o => o.Suit == CardSuit.Spades).OrderByDescending(c => c.Rank).ToArray();


                PepsterCard playerFirstCard = this.PepsterDealtCards[player.FirstCardPosition];
                PepsterCard playerSecondCard = this.PepsterDealtCards[player.SecondCardPosition];

                #endregion

                ////for (int i = 0; i < DealtCards - 1; i++)
                ////{
                ////int parse = int.Parse(cardHolder[firstCard].Tag.ToString());
                ////int parse1 = int.Parse(cardHolder[secondCard].Tag.ToString());

                ////if (this.dealtCardsNumbers[i] == parse && this.dealtCardsNumbers[i + 1] == parse1)
                ////{

                /*                    
                 * case "-1": CardPower 2 - 14
                        this.AIHighCard(player);
                 * 
                    case "0": CardPower 2 - 14
                        this.AIPairTable(player);
                    case "1":
                        this.AIPairHand(player);
                    case "2":
                        this.AITwoPair(player);
                    case "3":
                        this.AIThreeOfAKind(player);
                    case "4":
                        this.AIStraight(player);
                    case "5":
                    case "5.5":
                        this.AIFlush(player);
                    case "6":
                        this.AIFullHouse(player);
                    case "7":
                        this.AIFourOfAKind(player);
                    case "8":
                    case "9":
                        this.AIStraightFlush(player);
l                */


                #region High Card PokerHandMultiplier = -1
                this.rHighCard(playerFirstCard, playerSecondCard, player);
                #endregion

                //#region High Card PokerHandMultiplier = 0
                //this.rPairTable(playerFirstCard, playerSecondCard, player);
                //#endregion

                #region Pair from Table PokerHandMultiplier = 0
                this.rPair(playerFirstCard, playerSecondCard, cardsOnTable, player);
                #endregion

                #region Pair from hand PokerHandMultiplier = 1
                this.rPairFromHand(playerFirstCard, playerSecondCard, cardsOnTable, player);
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

                #region Flush PokerHandMultiplier = 5 || 5.5

                this.rFlush(playerFirstCard, playerSecondCard, cardsOnTable, player);

                #endregion

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


        private void rFlush(PepsterCard playerFirstCard, PepsterCard playerSecondCard, PepsterCard[] cardsOnTable, IPlayer player)
        {

            var cardsOfClubs = cardsOnTable.Where(o => o.Suit == CardSuit.Clubs).ToArray();
            var cardsOfDiamonds = cardsOnTable.Where(o => o.Suit == CardSuit.Diamonds).ToArray();
            var cardsOfHearts = cardsOnTable.Where(o => o.Suit == CardSuit.Hearts).ToArray();
            var cardsOfSpades = cardsOnTable.Where(o => o.Suit == CardSuit.Spades).ToArray();

            if (cardsOfClubs.Length >= 3)
            {
                this.CheckForFlush(playerFirstCard, playerSecondCard, cardsOfClubs, player);
            }

            if (cardsOfDiamonds.Length >= 3)
            {
                this.CheckForFlush(playerFirstCard, playerSecondCard, cardsOfDiamonds, player);
            }

            if (cardsOfHearts.Length >= 3)
            {
                this.CheckForFlush(playerFirstCard, playerSecondCard, cardsOfHearts, player);
            }

            if (cardsOfSpades.Length >= 3)
            {
                this.CheckForFlush(playerFirstCard, playerSecondCard, cardsOfSpades, player);
            }

        }

        private void CheckForFlush(PepsterCard firstCard, PepsterCard secondCard, PepsterCard[] cardsOfSameSuitOnTable, IPlayer player)
        {
            var allcardsOnTableAndPlayers = new PepsterCard[cardsOfSameSuitOnTable.Length + 2];
            allcardsOnTableAndPlayers[0] = firstCard;
            allcardsOnTableAndPlayers[1] = secondCard;

            var highestCardFromTable = cardsOfSameSuitOnTable.OrderByDescending(c => c.Rank).First();

            for (int i = 2, counter = 0; i < allcardsOnTableAndPlayers.Length; i++, counter++)
            {
                allcardsOnTableAndPlayers[i] = cardsOfSameSuitOnTable[counter];
            }
            var maxCardValue = allcardsOnTableAndPlayers.OrderByDescending(c => c.Rank).First();

            if (cardsOfSameSuitOnTable.Length == 3)
            {
                // check if the 3 cards on the table with the same suit are the same suit with the player's cards
                // takes the most high card by rank and multiplies *500
                if (firstCard.Suit == secondCard.Suit && firstCard.Suit == highestCardFromTable.Suit)
                {
                    player.PokerHandMultiplier = 5;
                    player.CardPower = maxCardValue.Rank + player.PokerHandMultiplier * 100;
                }
                // check if one of the player cards match by suit with the cards on the table
            }
            else if (cardsOfSameSuitOnTable.Length == 4 &&
               (firstCard.Suit == highestCardFromTable.Suit || secondCard.Suit == highestCardFromTable.Suit))
            {
                if (firstCard.Suit == highestCardFromTable.Suit)
                {
                    var highCardByRank = firstCard.Suit > highestCardFromTable.Suit ? firstCard : highestCardFromTable;
                    player.PokerHandMultiplier = 5;
                    player.CardPower = highCardByRank.Rank + player.PokerHandMultiplier * 100;
                }
                if (secondCard.Suit == highestCardFromTable.Suit && secondCard.Rank > firstCard.Rank)
                {
                    var highCardByRank = secondCard.Suit > highestCardFromTable.Suit ? secondCard : highestCardFromTable;
                    player.PokerHandMultiplier = 5;
                    player.CardPower = highCardByRank.Rank + player.PokerHandMultiplier * 100;
                }
            }
            //check if player's cards match by suit with card on table if so takes the strongest by power and finds CardPower 
            //else takes the strongerst card from the table
            else if (cardsOfSameSuitOnTable.Length == 5)
            {
                if (firstCard.Suit == highestCardFromTable.Suit || secondCard.Suit == highestCardFromTable.Suit)
                {
                    if (firstCard.Suit == highestCardFromTable.Suit)
                    {
                        var highCardByRank = firstCard.Suit > highestCardFromTable.Suit ? firstCard : highestCardFromTable;
                        player.PokerHandMultiplier = 5;
                        player.CardPower = highCardByRank.Rank + player.PokerHandMultiplier * 100;
                    }
                    if (secondCard.Suit == highestCardFromTable.Suit && secondCard.Rank > firstCard.Rank)
                    {
                        var highCardByRank = secondCard.Suit > highestCardFromTable.Suit ? secondCard : highestCardFromTable;
                        player.PokerHandMultiplier = 5;
                        player.CardPower = highCardByRank.Rank + player.PokerHandMultiplier * 100;
                    }
                }
                else
                {
                    player.PokerHandMultiplier = 5;
                    player.CardPower = maxCardValue.Rank + player.PokerHandMultiplier * 100;
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


        private void rStraight(PepsterCard firstCard, PepsterCard secondCard, PepsterCard[] cardsOnTable, IPlayer player)
        {
            int firstCardPairs = cardsOnTable.Count(card => firstCard.Rank == card.Rank);
            int secondCardPairs = cardsOnTable.Count(card => secondCard.Rank == card.Rank);
            List<int> allCards = new List<int>();

            if (firstCardPairs == 0)
            {
                if (firstCard.Rank == 14)
                {
                    allCards.Add(firstCard.Rank);
                    allCards.Add(1);
                }
                else
                {
                    allCards.Add(firstCard.Rank);
                }
            }

            if (secondCardPairs == 0)
            {
                if (firstCard.Rank == 14)
                {
                    allCards.Add(secondCard.Rank);
                    allCards.Add(1);
                }
                else
                {
                    allCards.Add(firstCard.Rank);
                }
            }

            if (cardsOnTable.Any(card => card.Rank == 14))
            {
                allCards.Add(1);
            }

            allCards.AddRange(cardsOnTable.Select(card => card.Rank));

            allCards.Sort();
            int countConsecutives = 0;
            int highestCardRankInStreight = 0;
            for (int startCard = 0; startCard < allCards.Count - 1; startCard++)
            {
                if (allCards[startCard] + 1 == allCards[startCard + 1])
                {
                    countConsecutives++;
                    if (countConsecutives >= 5)
                    {
                        highestCardRankInStreight = allCards[startCard + 1];
                    }
                }
            }

            player.PokerHandMultiplier = 4;
            player.CardPower = highestCardRankInStreight * 4 + player.PokerHandMultiplier * 100;

        }

        private void rThreeOfAKind(PepsterCard firstCard, PepsterCard secondCard, PepsterCard[] cardsOnTable, IPlayer player)
        {
            int firstCardThreeOfAKindCount = cardsOnTable.Count(card => firstCard.Rank == card.Rank);
            int secondCardThreeOfAKindCount = cardsOnTable.Count(card => secondCard.Rank == card.Rank);

            if (firstCardThreeOfAKindCount > 2 || secondCardThreeOfAKindCount > 2)
            {
                return;
            }

            if (firstCardThreeOfAKindCount == 2 && secondCardThreeOfAKindCount == 2 && firstCard.Rank == secondCard.Rank)
            {
                return;
            }

            PepsterCard highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            if (firstCardThreeOfAKindCount == 2 || secondCardThreeOfAKindCount == 2)
            {
                player.PokerHandMultiplier = 3;
                if (firstCardThreeOfAKindCount == 2 && secondCardThreeOfAKindCount != 2)
                {
                    player.CardPower = firstCard.Rank * 4 + player.PokerHandMultiplier * 100;
                }
                else if (firstCardThreeOfAKindCount != 2 && secondCardThreeOfAKindCount == 2)
                {
                    player.CardPower = secondCard.Rank * 4 + player.PokerHandMultiplier * 100;
                }
                else
                {
                    player.CardPower = highestCard.Rank * 4 + player.PokerHandMultiplier * 100;
                }
            }

            if (firstCardThreeOfAKindCount == 1 &&
                secondCardThreeOfAKindCount == 1 &&
                firstCard.Rank == secondCard.Rank)
            {
                player.PokerHandMultiplier = 3;
                player.CardPower = firstCard.Rank * 4 + player.PokerHandMultiplier * 100;
            }

            var tableCardsTreeOfAKind = cardsOnTable
                .GroupBy(card => card.Rank)
                .Select(group => new { Rank = group.Key, Count = group.Count() })
                .OrderByDescending(group => group.Count)
                .First();

            if (tableCardsTreeOfAKind.Count != 3)
            {
                return;
            }

            if (firstCardThreeOfAKindCount == 1 &&
                secondCardThreeOfAKindCount == 1 &&
                firstCard.Rank == secondCard.Rank)
            {
                int highestCardHandOrTable = tableCardsTreeOfAKind.Rank > firstCard.Rank ? tableCardsTreeOfAKind.Rank : firstCard.Rank;
                player.PokerHandMultiplier = 3;
                player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
            }

            if (firstCardThreeOfAKindCount == 0 &&
                secondCardThreeOfAKindCount == 0)
            {
                player.PokerHandMultiplier = 3;
                player.CardPower = tableCardsTreeOfAKind.Rank * 4 + player.PokerHandMultiplier * 100;
            }
        }

        private void rTwoPair(PepsterCard firstCard, PepsterCard secondCard, PepsterCard[] cardsOnTable, IPlayer player)
        {
            int firstCardPairCount = cardsOnTable.Count(card => firstCard.Rank == card.Rank);
            int secondCardPairCount = cardsOnTable.Count(card => secondCard.Rank == card.Rank);

            if (firstCardPairCount > 1 || secondCardPairCount > 1)
            {
                return;
            }

            if (firstCardPairCount == 1 && secondCardPairCount == 1 && firstCard.Rank == secondCard.Rank)
            {
                return;
            }

            int pairsOnTable = 0;
            int highestTablePairRank = 0;

            for (int startCardIndex = 0; startCardIndex < cardsOnTable.Length - 1; startCardIndex++)
            {
                int pairCount = 0;

                for (int endCardIndex = cardsOnTable.Length - 1; endCardIndex > 0; endCardIndex--)
                {
                    if (endCardIndex == startCardIndex)
                    {
                        continue;
                    }

                    if (cardsOnTable[startCardIndex].Rank != cardsOnTable[endCardIndex].Rank)
                    {
                        continue;
                    }

                    pairCount++;

                    if (highestTablePairRank < cardsOnTable[startCardIndex].Rank)
                    {
                        highestTablePairRank = cardsOnTable[startCardIndex].Rank;
                    }
                }

                if (pairCount > 1)
                {
                    return;
                }

                if (pairCount == 1)
                {
                    pairsOnTable++;
                }
            }

            switch (pairsOnTable)
            {
                case 0:
                    if (firstCardPairCount == 1 && secondCardPairCount == 1)
                    {
                        PepsterCard highestCardInHand = this.GetHighestCardInHand(firstCard, secondCard);
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardInHand.Rank * 4 + highestCardInHand.Rank + player.PokerHandMultiplier * 100;
                    }

                    break;
                case 1:
                    if (firstCard.Rank == secondCard.Rank)
                    {
                        int highestCardHandOrTable = firstCard.Rank > highestTablePairRank ? firstCard.Rank : highestTablePairRank;
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
                    }
                    else if (firstCardPairCount == 1 && secondCardPairCount != 1)
                    {
                        int highestCardHandOrTable = firstCard.Rank > highestTablePairRank ? firstCard.Rank : highestTablePairRank;
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
                    }
                    else if (secondCardPairCount == 1 && firstCardPairCount != 1)
                    {
                        int highestCardHandOrTable = secondCard.Rank > highestTablePairRank ? secondCard.Rank : highestTablePairRank;
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
                    }
                    else if (secondCardPairCount == 1 && firstCardPairCount == 1)
                    {
                        int highestCardHandOrTable = Math.Max(highestTablePairRank, Math.Max(firstCard.Rank, secondCard.Rank));
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
                    }

                    break;
                case 2:
                    if (firstCard.Rank == secondCard.Rank)
                    {
                        int highestCardHandOrTable = Math.Max(highestTablePairRank, Math.Max(firstCard.Rank, secondCard.Rank));
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
                    }
                    else if (firstCardPairCount == 1)
                    {
                        int highestCardHandOrTable = firstCard.Rank > highestTablePairRank ? firstCard.Rank : highestTablePairRank;
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
                    }
                    else if (secondCardPairCount == 1)
                    {
                        int highestCardHandOrTable = secondCard.Rank > highestTablePairRank ? secondCard.Rank : highestTablePairRank;
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestCardHandOrTable * 4 + player.PokerHandMultiplier * 100;
                    }
                    else
                    {
                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestTablePairRank * 4 + player.PokerHandMultiplier * 100;
                    }

                    break;
            }
        }

        private void rPair(PepsterCard firstCard, PepsterCard secondCard, PepsterCard[] cardsOnTable, IPlayer player)
        {
            int firstCardPairCount = cardsOnTable.Count(card => firstCard.Rank == card.Rank);
            int secondCardPairCount = cardsOnTable.Count(card => secondCard.Rank == card.Rank);

            if ((firstCardPairCount == 1 ^ secondCardPairCount == 1) &&
                firstCard.Rank == secondCard.Rank)
            {
                return;
            }

            if (firstCardPairCount == 1)
            {
                player.PokerHandMultiplier = 0;
                player.CardPower = firstCard.Rank * 4 + player.PokerHandMultiplier * 100;
            }

            if (secondCardPairCount == 1)
            {
                player.PokerHandMultiplier = 0;
                player.CardPower = secondCard.Rank * 4 + player.PokerHandMultiplier * 100;
            }
        }

        private void rPairFromHand(PepsterCard firstCard, PepsterCard secondCard, IEnumerable<PepsterCard> cardsOnTable, IPlayer player)
        {
            if (firstCard.Rank != secondCard.Rank)
            {
                return;
            }

            int tableCardPairCount = cardsOnTable.Count(card => firstCard.Rank == card.Rank);

            if (tableCardPairCount > 0)
            {
                return;
            }

            player.PokerHandMultiplier = 1;
            player.CardPower = firstCard.Rank * 4 + player.PokerHandMultiplier * 100;
        }

        private void rHighCard(PepsterCard firstCard, PepsterCard secondCard, IPlayer player)
        {
            PepsterCard highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            player.PokerHandMultiplier = -1;
            player.CardPower = highestCard.Rank;
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

        private PepsterCard GetHighestCardInHand(PepsterCard firstCard, PepsterCard secondCard)
        {
            PepsterCard highestCard = firstCard.Rank > secondCard.Rank ? firstCard : secondCard;
            return highestCard;
        }

        //private int GetHighestCardInAllPlayableCards(int firstCard, int secondCard, int[] cardsOnTable)
        //{
        //    int firstCardPower = this.GetCardRank(firstCard);
        //    int secondCardPower = this.GetCardRank(secondCard);
        //    int highestCardOnTable = cardsOnTable.Select(this.GetCardRank).Max();

        //    int maxCard = Math.Max(highestCardOnTable, Math.Max(firstCardPower, secondCardPower));

        //    return maxCard;
        //}


        //TODO: to be redesigned
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
                        this.renderer.ShowMessage(playerName + " High Card ");
                    }

                    if (PokerHandMultiplier == 1 || PokerHandMultiplier == 0)
                    {
                        this.renderer.ShowMessage(playerName + " Pair ");
                    }

                    if (PokerHandMultiplier == 2)
                    {
                        this.renderer.ShowMessage(playerName + " Two Pair ");
                    }

                    if (PokerHandMultiplier == 3)
                    {
                        this.renderer.ShowMessage(playerName + " Three of a Kind ");
                    }

                    if (PokerHandMultiplier == 4)
                    {
                        this.renderer.ShowMessage(playerName + " Straight ");
                    }

                    if (PokerHandMultiplier == 5 || PokerHandMultiplier == 5.5)
                    {
                        this.renderer.ShowMessage(playerName + " Flush ");
                    }

                    if (PokerHandMultiplier == 6)
                    {
                        this.renderer.ShowMessage(playerName + " Full House ");
                    }

                    if (PokerHandMultiplier == 7)
                    {
                        this.renderer.ShowMessage(playerName + " Four of a Kind ");
                    }

                    if (PokerHandMultiplier == 8)
                    {
                        this.renderer.ShowMessage(playerName + " Straight Flush ");
                    }

                    if (PokerHandMultiplier == 9)
                    {
                        this.renderer.ShowMessage(playerName + " Royal Flush ! ");
                    }
                }
            }

            if (playerName == lastly) //lastfixed
            {
                if (this.Table.WinnersCount > 1)
                {
                    if (this.checkWinners.Contains("Player"))
                    {
                        this.Gamer.Chips += this.Table.Pot / this.Table.WinnersCount;

                        //this.form.textBoxPlayerChips.Text = this.Gamer.Chips.ToString();
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
                        this.Gamer.Chips += this.Table.Pot;

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

        private async Task CheckRaise(int playerId)
        {
            if (this.Table.IsRaising)
            {
                this.Table.TurnCount = 0;
                this.Table.IsRaising = false;
                this.Table.LastRaisedPlayerId = playerId;
                this.changed = true;
            }
            else
            {
                if (this.Table.TurnCount >= this.Table.PlayersInTheGame - 1 || !this.changed && this.Table.TurnCount == this.Table.PlayersInTheGame)
                {
                    if (playerId == this.Table.LastRaisedPlayerId - 1 || !this.changed && this.Table.TurnCount == this.Table.PlayersInTheGame || this.Table.LastRaisedPlayerId == 0 && playerId == 5)
                    {
                        this.changed = false;
                        this.Table.TurnCount = 0;
                        this.Table.LastRaise = 0;
                        this.Table.PokerCall = 0;
                        this.Table.LastRaisedPlayerId = 123;
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

            if (this.Table.Rounds == this.cardFlop)
            {
                for (int j = 12; j <= 14; j++)
                {
                    #region code to delete
                    //if (this.cardHolder[j].Image == this.deckImages[j])
                    //{
                    //    continue;
                    //}

                    //this.cardHolder[j].Image = this.deckImages[j];

                    ////this.Gamer.Call = 0;
                    ////this.Gamer.Raise = 0;

                    ////this.Players[1].Call = 0;
                    ////this.Players[1].Raise = 0;

                    ////this.Players[2].Call = 0;
                    ////this.Players[2].Raise = 0;

                    ////this.Players[3].Call = 0;
                    ////this.Players[3].Raise = 0;

                    ////this.Players[4].Call = 0;
                    ////this.Players[4].Raise = 0;

                    ////this.Players[5].Call = 0;
                    ////this.Players[5].Raise = 0;
                    #endregion
                    this.PepsterDealtCards[j].IsVisible = true;
                    this.renderer.Draw(this.PepsterDealtCards[j]);
                }
                foreach (var player in this.Players)
                {
                    player.Call = 0;
                    player.Raise = 0;
                }
            }

            if (this.Table.Rounds == this.cardTurn)
            {
                for (int j = 14; j <= 15; j++)
                {
                    this.PepsterDealtCards[j].IsVisible = true;
                    this.renderer.Draw(this.PepsterDealtCards[j]);
                }

                foreach (var player in this.Players)
                {
                    player.Call = 0;
                    player.Raise = 0;
                }
            }

            if (this.Table.Rounds == this.cardRiver)
            {
                for (int j = 15; j <= 16; j++)
                {
                    this.PepsterDealtCards[j].IsVisible = true;
                    this.renderer.Draw(this.PepsterDealtCards[j]);
                }

                foreach (var player in this.Players)
                {
                    player.Call = 0;
                    player.Raise = 0;
                }
            }

            if (this.Table.Rounds == this.cardEnd && this.Table.PlayersInTheGame == 6)
            {
                string fixedLast = "";

                // TODO: change to new logic
                foreach (var player in this.Players)
                {
                    if (!player.Folded)
                    {
                        fixedLast = player.Name;
                        this.Rules(player);
                    }
                }

                foreach (IPlayer player in this.Players)
                {
                    this.Winner(player.PokerHandMultiplier,
                        player.CardPower,
                        player.Name,
                        player.Chips,
                       fixedLast);
                }

                this.GameEnd = true;

                this.Gamer.Turn = true;

                for (int playerIndex = 0; playerIndex < this.Players.Length; playerIndex++)
                {
                    this.Players[playerIndex].GameEnded = false;
                }

                if (this.Gamer.Chips <= 0)
                {
                    AddChips f2 = new AddChips();
                    f2.ShowDialog();
                    if (f2.AddedChips != 0)
                    {
                        this.Gamer.Chips = f2.AddedChips;
                        this.Players[1].Chips += f2.AddedChips;
                        this.Players[2].Chips += f2.AddedChips;
                        this.Players[3].Chips += f2.AddedChips;
                        this.Players[4].Chips += f2.AddedChips;
                        this.Players[5].Chips += f2.AddedChips;
                        this.Gamer.GameEnded = false;
                        this.Gamer.Turn = true;

                        this.Gamer.CanRaise = true;
                        this.Gamer.CanFold = true;
                        this.Gamer.CanCheck = true;

                        this.renderer.Draw(this.Players);
                        //this.renderer.buttonRaise.Text = "Raise";
                    }
                }

                /* old code for panels
                  this.Gamer.CardsPanel.Visible = false;
                  this.players[1].CardsPanel.Visible = false;
                  this.players[2].CardsPanel.Visible = false;
                  this.players[3].CardsPanel.Visible = false;
                  this.players[4].CardsPanel.Visible = false;
                  this.players[5].CardsPanel.Visible = false;
                  */
                //TODO: veronika check panel setup
                //foreach (var panel in this.form.PlayersPanels)
                //{
                //    panel.Visible = false;
                //}

                for (int playerIndex = 0; playerIndex < this.Players.Length; playerIndex++)
                {
                    this.Players[playerIndex].Call = 0;
                    this.Players[playerIndex].Raise = 0;
                    this.Players[playerIndex].CardPower = 0;
                    this.Players[playerIndex].PokerHandMultiplier = -1;
                    this.Players[playerIndex].Call = 0;
                }

                this.Table.LastBotPlayed = 0;
                this.Table.PokerCall = this.Table.BigBlind;
                this.Table.LastRaise = 0;
                //            this.imageURIArray = Directory.GetFiles("Assets\\Cards", "*.png", SearchOption.TopDirectoryOnly);
                this.playersNotGameEnded.Clear();
                this.Table.Rounds = 0;
                this.type = 0;


                this.playersWithoutChips.Clear();
                this.checkWinners.Clear();
                this.Table.WinnersCount = 0;
                this.winningCards.Clear();
                this.winningCard.Current = 0;
                this.winningCard.Power = 0;

                for (int os = 0; os < 17; os++)
                {
                    this.PepsterDealtCards[os].CardFrontImageUri = null;
                    //TODO: veronika not sure what Invalidate()
                    //this.PepsterDealtCards[os].Invalidate();
                    this.PepsterDealtCards[os].IsVisible = false;
                }

                //this.form.textBoxPot.Text = "0";
                this.Table.Pot = 0;
                this.Gamer.Status = string.Empty;
                // set label status 
                //this.form.PlayersLabelsStatus[0].Text = this.Gamer.Status;
                this.renderer.SetAllLabelStatus((IPlayer[])this.Players.Where(p => p.Name == "Player"));
                await this.SetupPokerTable();
                this.renderer.Draw(this.Players);
                this.renderer.Draw(this.PepsterDealtCards);
                await this.Turns();
            }
        }

        private void FixCall(IPlayer player, int options)
        {
            string playerLableText = player.Status;

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

                    if (player.Status == "Call")
                    {
                        var changeCall = playerLableText.Substring(5);
                        player.Call = int.Parse(changeCall);
                    }

                    if (player.Status == "Check")
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
                        this.Gamer.CanCall = false;
                        //this.form.buttonCall.Enabled = false;
                        this.renderer.Draw(this.Players);
                        //TODO: veronika sets some text value ?!?
                        //this.form.buttonCall.Text = "Callisfuckedup";
                    }
                }
            }
        }

        private async Task AllIn()
        {

            #region All in
            /*
             * old code:
                 * this.Gamer.Label.Text.Contains("Raise")
                 * this.Gamer.Label.Text.Contains("Call")
             *new code: 
                 *this.Gamer.Status.Contains("Raise")
                 *this.Gamer.Status.Contains("Raise")
         */

            if (this.Gamer.Chips <= 0 && !this.intsadded)
            {
                if (this.Gamer.Status.Contains("Raise"))
                {
                    this.playersWithoutChips.Add(this.Gamer.Chips);
                    this.intsadded = true;
                }

                if (this.Gamer.Status.Contains("Raise"))
                {
                    this.playersWithoutChips.Add(this.Gamer.Chips);
                    this.intsadded = true;
                }
            }

            this.intsadded = false;
            for (int playerIndex = 1; playerIndex < this.Players.Length; playerIndex++)
            {
                if (this.Players[playerIndex].Chips <= 0 && !this.Players[playerIndex].GameEnded)
                {
                    if (!this.intsadded)
                    {
                        this.playersWithoutChips.Add(this.Players[playerIndex].Chips);
                        this.intsadded = true;
                    }

                    this.intsadded = false;
                }
            }


            if (this.playersWithoutChips.ToArray().Length == this.Table.PlayersInTheGame)
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
                    //this.Gamer.Chips += int.Parse(this.form.textBoxPot.Text);
                    this.Gamer.Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[0]);
                    //TODO : panels ..
                    //this.form.PlayersPanels[0].Visible = true;
                    MessageBox.Show("Player Wins");
                }

                if (index == 1)
                {
                    //this.Players[1].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.Players[1].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[1]);
                    //TODO : panels ..
                    //this.form.PlayersPanels[1].Visible = true;
                    MessageBox.Show("Bot 1 Wins");
                }

                if (index == 2)
                {
                    //this.Players[2].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.Players[2].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[2]);
                    //TODO : panels ..
                    //this.form.PlayersPanels[2].Visible = true;
                    MessageBox.Show("Bot 2 Wins");
                }

                if (index == 3)
                {
                    //this.Players[3].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.Players[3].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[3]);
                    //TODO : panels ..
                    //this.form.PlayersPanels[3].Visible = true;
                    MessageBox.Show("Bot 3 Wins");
                }

                if (index == 4)
                {
                    //this.Players[4].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.Players[4].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[4]);
                    //TODO : panels ..
                    //this.form.PlayersPanels[4].Visible = true;
                    MessageBox.Show("Bot 4 Wins");
                }

                if (index == 5)
                {
                    //this.Players[5].Chips += int.Parse(this.form.textBoxPot.Text);
                    this.Players[5].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[5]);
                    //TODO : panels ..
                    //this.form.PlayersPanels[5].Visible = true;
                    MessageBox.Show("Bot 5 Wins");
                }

                for (int j = 0; j <= 16; j++)
                {
                    this.PepsterDealtCards[j].IsVisible = false;
                }

                await this.Finish(1);
            }
            ////TODO find what is intsadded
            this.intsadded = false;
            #endregion

            #region FiveOrLessLeft
            if (playersNotGameEndedCount < 6 &&
                playersNotGameEndedCount > 1 &&
                this.Table.Rounds >= this.cardEnd)
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
            this.Table.FoldedBots = 5;
            this.type = 0;
            this.Table.Rounds = 0;

            this.Table.LastRaise = 0;

            this.GameEnd = false;
            this.Table.IsRaising = false;


            this.Table.WinnersCount = 0;
            this.Table.PlayersInTheGame = 6;
            this.Table.LastBotPlayed = 123;
            this.Table.LastRaisedPlayerId = 1;
            this.playersNotGameEnded.Clear();
            this.checkWinners.Clear();
            this.playersWithoutChips.Clear();
            this.winningCards.Clear();
            this.winningCard.Current = 0;
            this.winningCard.Power = 0;

            this.Table.TurnCount = 0;

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

            if (this.Gamer.Chips <= 0)
            {
                AddChips addChipsForm = new AddChips();
                addChipsForm.ShowDialog();
                if (addChipsForm.AddedChips != 0)
                {
                    this.Gamer.Chips = addChipsForm.AddedChips;
                    this.Players[1].Chips += addChipsForm.AddedChips;
                    this.Players[2].Chips += addChipsForm.AddedChips;
                    this.Players[3].Chips += addChipsForm.AddedChips;
                    this.Players[4].Chips += addChipsForm.AddedChips;
                    this.Players[5].Chips += addChipsForm.AddedChips;
                    this.Gamer.GameEnded = false;
                    this.Gamer.Turn = true;
                    this.Gamer.CanRaise = true;
                    this.Gamer.CanFold = true;
                    this.Gamer.CanCheck = true;
                    //this.form.buttonRaise.Enabled = true;
                    //this.form.buttonFold.Enabled = true;
                    //this.form.buttonCheck.Enabled = true;
                    this.renderer.Draw(this.Players);
                    //TODO :  raise value setup
                    //this.form.buttonRaise.Text = "Raise";
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
            string fixedLast = "";

            //TODO: change to new logic - > DONE -> see region " code to be deleted" new code is the foreach
            #region old code to be deleted

            //if (!this.Gamer.Label.Text.Contains("Fold"))
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

            foreach (IPlayer player in this.Players)
            {
                this.Winner(player.PokerHandMultiplier,
                    player.CardPower,
                    player.Name,
                    player.Chips,
                   fixedLast);
            }
        }

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
            //var firstCardNumeration = player.FirstCardPosition;
            //var secondNumeration = player.SecondCardPosition;
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

            this.renderer.Draw(player);
            #region code to delete
            //if (player.GameEnded)
            //{
            //    this.cardHolder[firstCardNumeration].Visible = false;
            //    this.cardHolder[secondNumeration].Visible = false;
            //}
            #endregion
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
                                this.Table.LastRaise = (int)AIRoundNumber(player.Chips, n);
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
                                this.Table.LastRaise = (int)AIRoundNumber(player.Chips, n - rnd);
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
                    this.Table.LastRaise = (int)AIRoundNumber(player.Chips, r - rnd);
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
                        // old code to delete : this.Table.IsRaising = false;
                        this.Table.IsRaising = false;
                        player.Turn = false;
                        player.Chips = 0;
                        player.Status = "Call " + player.Chips;
                        this.Table.Pot += player.Chips;
                        this.renderer.Draw(player);
                        // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
                        // old code to delete : this.form.textBoxPot.Text = (int.Parse(this.form.textBoxPot.Text) + player.Chips).ToString();
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
            // old code to delete : this.Table.IsRaising = false;
            // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status; 
            this.Table.IsRaising = false;
            player.Status = "Fold";
            player.Turn = false;
            player.Folded = true;
            player.GameEnded = true;
            this.renderer.Draw(player);
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
            // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status; this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
            player.Turn = false;
            // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status;  this.Table.IsRaising = false;
            this.Table.IsRaising = false;
            this.renderer.Draw(player);
        }

        //// вика се от  AIHP AIPH, AISmooth this.AICall(ref botChips, ref bothTurn, sStatus);
        private void AICall(IPlayer player)
        {
            this.Table.IsRaising = false;
            player.Turn = false;
            player.Chips -= this.Table.PokerCall;
            player.Status = "Call " + this.Table.PokerCall;
            this.Table.Pot += this.Table.PokerCall;
            this.renderer.Draw(player);
            this.renderer.Draw(this.Table);
            // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status; this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
            // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status; this.form.textBoxPot.Text = (int.Parse(this.form.textBoxPot.Text) + this.Table.PokerCall).ToString();
        }

        //// вика се от  AIHP AIPH, AISmooth this.AIRaised(ref botChips, ref bothTurn, sStatus);
        private void AIRaised(IPlayer player)
        {
            player.Chips -= Convert.ToInt32(this.Table.LastRaise);
            player.Status = "Raise " + this.Table.LastRaise;
            this.Table.Pot += this.Table.LastRaise;

            // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status; this.form.PlayersLabelsStatus[player.Id].Text = player.Status;
            // old code to delete : this.form.PlayersLabelsStatus[player.Id].Text = player.Status; this.form.textBoxPot.Text = (int.Parse(this.form.textBoxPot.Text) + Convert.ToInt32(this.Table.LastRaise)).ToString();
            this.Table.PokerCall = Convert.ToInt32(this.Table.LastRaise);
            this.Table.IsRaising = true;
            player.Turn = false;
            this.renderer.Draw(player);
        }

        ////вика се от  AIHP AIPH, AISmooth  this.Table.PokerCall <= AIRoundNumber(botChips, n) числото е различно има и математика
        private static double AIRoundNumber(int botChips, int n)
        {
            double a = Math.Round((botChips / n) / 100d, 0) * 100;
            return a;
        }
        #endregion

        #region push buttons logic
        public void GammerPlayesFold()
        {
            this.Gamer.Status = "Fold";
            this.Gamer.Turn = false;
            this.Gamer.GameEnded = true;
            this.renderer.Draw(this.Gamer);
        }

        public void GammerPlayesCheck()
        {
            Gamer gamer = (Gamer)this.Gamer;

            if (this.Table.PokerCall <= 0)
            {
                gamer.Turn = false;
                gamer.Status = "Check";
                this.renderer.Draw(gamer);
            }
            else
            {
                gamer.CanCheck = false;
                this.renderer.Draw(gamer);
            }
        }

        public void GammerMoveTimeExpired()
        {
            this.Gamer.GameEnded = true;
            this.renderer.Draw(this.Gamer);
        }

        public void GammerPlayesCall()
        {
            Gamer gamer = (Gamer)this.Gamer;
            this.Rules(gamer);

            if (gamer.Chips >= this.Table.PokerCall)
            {
                gamer.Chips -= this.Table.PokerCall;
                this.Table.Pot += this.Table.PokerCall;

                #region old code to delete
                //this.textBoxPlayerChips.Text = "Chips : " + this.Gamer.Chips.ToString();
                //if (this.textBoxPot.Text != "")
                //{
                //    this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + this.Table.PokerCall).ToString();
                //}
                //else
                //{
                //    this.textBoxPot.Text = this.Table.PokerCall.ToString();
                //}
                //  this.PlayersLabelsStatus[0].Text = this.Gamer.Status;
                #endregion to delete

                gamer.Turn = false;
                gamer.Status = "Call " + this.Table.PokerCall;
                gamer.Call = this.Table.PokerCall;
            }
            else if (gamer.Chips <= this.Table.PokerCall && this.Table.PokerCall > 0)
            {
                this.Table.Pot += gamer.Chips;
                gamer.Status = "All in " + gamer.Chips;
                gamer.Chips = 0;
                gamer.Turn = false;
                gamer.Call = gamer.Chips;
                gamer.CanFold = false;
                #region old code to to delete
                // this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + this.Gamer.Chips).ToString();
                // this.PlayersLabelsStatus[0].Text = this.Gamer.Status;
                // this.textBoxPlayerChips.Text = "Chips : " + this.Gamer.Chips.ToString();
                // this.buttonFold.Enabled = false;
                #endregion
            }

            this.renderer.Draw(gamer);
            this.renderer.Draw(this.Table);
        }

        public void GammerPlayesRaise()
        {
            Gamer gamer = (Gamer)this.Gamer;

            this.Rules(gamer);

            int valueToRaise = this.inputHandlerer.ReadRaise();
            gamer.ValueToRaise = valueToRaise;

            if (gamer.Chips > this.Table.PokerCall)
            {
                if (this.Table.LastRaise * 2 > gamer.ValueToRaise)
                {
                    this.renderer.ShowMessage("You must raise atleast twice as the current raise !");
                    gamer.ValueToRaise = this.Table.LastRaise * 2;
                }
                else if (gamer.Chips >= gamer.ValueToRaise)
                {
                    this.Table.PokerCall = gamer.ValueToRaise;
                    this.Table.LastRaise = gamer.ValueToRaise;
                    this.Table.Pot += this.Table.PokerCall;
                    gamer.Raise = gamer.ValueToRaise;
                    gamer.Status = "Raise " + gamer.Raise;
                    gamer.Chips -= gamer.ValueToRaise;
                    this.Table.IsRaising = true;
                    this.Table.LastBotPlayed = 0;
                    gamer.Turn = false;
                }
                else
                {
                    // player playes All In
                    this.Table.PokerCall = gamer.Chips;
                    this.Table.LastRaise = gamer.Chips;
                    this.Table.Pot += gamer.Chips;
                    gamer.Raise = gamer.Chips;
                    gamer.Status = "Raise" + this.Table.PokerCall;
                    gamer.Chips = 0;
                    this.Table.IsRaising = true;
                    this.Table.LastBotPlayed = 0;
                    gamer.Turn = false;
                }
            }
            this.renderer.Draw(gamer);
            this.renderer.Draw(this.Table);
        }

        public void GammerAddsChips()
        {
            int valueToAdd = this.inputHandlerer.ReadChipsToAdd();
            foreach (var player in this.Players)
            {
                player.Chips += valueToAdd;
            }
            this.renderer.Draw(this.Players);
            #region old code to delete
            //if (this.textBoxAddChips.Text == "")
            //{
            //}
            //else
            //{
            //    this.Gamer.Chips += int.Parse(this.textBoxAddChips.Text);
            //    this.Players[1].Chips += int.Parse(this.textBoxAddChips.Text);
            //    this.Players[2].Chips += int.Parse(this.textBoxAddChips.Text);
            //    this.Players[3].Chips += int.Parse(this.textBoxAddChips.Text);
            //    this.Players[4].Chips += int.Parse(this.textBoxAddChips.Text);
            //    this.Players[5].Chips += int.Parse(this.textBoxAddChips.Text);
            //}
            //this.textBoxPlayerChips.Text = "Chips : " + this.Gamer.Chips.ToString();
            #endregion
        }

        public void SetSmallBlind()
        {
            try
            {
                int inputValue = this.inputHandlerer.ReadSmallBlind();
                this.Table.SmallBlind = inputValue;
                this.renderer.ShowMessage("The changes have been saved ! They will become available the next hand you play. ");
            }
            catch (InputValueException ex)
            {
                this.renderer.ShowMessage(ex.Message);
            }
        }

        public void SetBigBlind()
        {
            try
            {
                int inputValue = this.inputHandlerer.ReadBigBlind();
                this.Table.BigBlind = inputValue;
                this.renderer.ShowMessage("The changes have been saved ! They will become available the next hand you play. ");
            }
            catch (InputValueException ex)
            {
                this.renderer.ShowMessage(ex.Message);
            }

        }
        #endregion



        private void InitializePlayers()
        {
            for (int playerCount = 0, cardFirstPosition = 0; playerCount < 6; playerCount++, cardFirstPosition += 2)
            {
                if (playerCount == 0)
                {
                    this.Gamer = new Gamer();
                    this.Players[playerCount] = this.Gamer;
                }
                else
                {
                    this.Players[playerCount] = new Bot(playerCount, "Bot " + playerCount, cardFirstPosition, cardFirstPosition + 1);
                }
            }
        }


        private void RemovePlayerFromTheGame(int playerIndex)
        {
            this.playersNotGameEnded.RemoveAt(playerIndex);
            this.playersNotGameEnded.Insert(playerIndex, null);
            this.Table.PlayersInTheGame--;
            this.Players[playerIndex].Folded = true;
        }

        private void CallAllPlayerActionsOnTurn(IPlayer player)
        {
            this.FixCall(player, 1);
            this.renderer.ShowGamerTurnTimer();
            ////MessageBox.Show("Player's Turn");
            this.Table.TurnCount++;
            this.FixCall(player, 2);
        }

        /// <summary>
        /// Calls all actions that must be called if it is bot turn
        /// </summary>
        private void CallAllBotActionsOnTheirTurn(IPlayer player)
        {
            var botNumber = player.Id;
            /////Bot 1 -> 0 ,Bot 2 ->1 etc.. 
            ///// used in this.Ai(...)
            //var botPresentedAsNumber = int.Parse(botNumber) - 1;

            this.FixCall(player, 1);
            this.FixCall(player, 2);

            this.Rules(player);

            MessageBox.Show("Bot  " + botNumber + @"'s Turn");
            this.AI(player);

            this.Table.TurnCount++;
            this.Table.LastBotPlayed = botNumber;
            player.Turn = false;

            ////TODO must be implemented

            var nextBotIndex = botNumber + 1;
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


    }
}

