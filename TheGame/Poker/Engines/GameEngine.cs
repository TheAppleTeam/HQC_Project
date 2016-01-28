namespace Poker.Engines
{
    using System;
    using System.Collections.Generic;
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
        private const int PokerFlop = 1;
        private const int PokerTurn = 2;
        private const int PokerRiver = 3;
        private const int PokerEndRound = 4;

        private readonly Random random = new Random();
        private readonly IRenderer renderer;
        private readonly IInputHandlerer inputHandlerer;

        private bool intsadded;
        private bool tableIsChanged;
        private double type;

        private List<bool?> playersNotGameEnded = new List<bool?>();
        private List<string> winnersNames = new List<string>();
        private List<int> playersWithoutChips = new List<int>();

        ///// <summary>
        ///// Array of Integers -> dealed cars. Array.Lengt = 17
        ///// All the numbers correspont to file name numbers of the cards
        ///// </summary>
        //private int[] dealtCardsNumbers = new int[17];

        public GameEngine(IRenderer renderer, IInputHandlerer inputHandlerer)
        {
            this.Table = new Table();
            this.Players = new IPlayer[GlobalConstants.PlayersCount];
            this.InitializePlayers();
            this.renderer = renderer;
            this.inputHandlerer = inputHandlerer;
            this.GameDeck = new GameCard[52];
            this.GameDealtCards = new GameCard[17];
            this.SetAllGameCardsDeck();
            this.GameEnd = false;
        }

        public Table Table { get; set; }

        public Gamer Gamer { get; set; }

        public IPlayer[] Players { get; private set; }

        public GameCard[] GameDeck { get; private set; }

        public GameCard[] GameDealtCards { get; set; }

        private bool GameEnd { get; set; }

        public async void GameInit()
        {
            this.Table.PokerCall = GlobalConstants.InitialBigBlind;

            await this.SetupPokerTable();
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
            this.renderer.Draw(this.Table);
        }

        private void SetAllGameCardsDeck()
        {
            int cardDeckIndex = 0;
            int idCounter = 5;

            for (int baseCardRank = 0; baseCardRank < 13; baseCardRank++)
            {
                this.GameDeck[cardDeckIndex] = new GameCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Clubs,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                cardDeckIndex++;

                this.GameDeck[cardDeckIndex] = new GameCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Diamonds,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                cardDeckIndex++;

                this.GameDeck[cardDeckIndex] = new GameCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Hearts,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                cardDeckIndex++;

                this.GameDeck[cardDeckIndex] = new GameCard()
                {
                    Id = idCounter,
                    Suit = CardSuit.Spades,
                    CardFrontImageUri = "../../Resources/Cards/" + idCounter + ".png",
                    Rank = baseCardRank + 2
                };
                idCounter++;
                cardDeckIndex++;
            }
        }

        #region SetupPokerTable

        /// <summary>
        /// Setups a poker table with all the players holders, cards on the table and buttons
        /// </summary>
        /// <returns></returns>
        private async Task SetupPokerTable()
        {
            this.RandomCardsToDeal();

            this.renderer.Draw(this.GameDealtCards);

            this.Gamer.CanRaise = true;
            this.Gamer.CanCall = true;
            this.Gamer.CanCheck = true;
            this.Gamer.CanFold = true;

            foreach (var player in this.Players)
            {
                this.playersNotGameEnded.Add(player.GameEnded);

                this.Rules(player);

                this.renderer.Draw(player);

                await Task.Delay(GlobalConstants.DealingCardsDelay);
            }

            this.renderer.DisplayCardsOnTable();

            this.EnablingFormControls();

            this.CheckForGameEnd();

            this.renderer.StartGamerTurnTimer();
        }

        /// <summary>
        /// Randomise the cars and take 17 to deal. 
        /// Set to gamer Cards.IsVisible = true; -> so Cards faces to be shown;
        /// Call renderer to create Images for dealt cards
        /// </summary>
        private void RandomCardsToDeal()
        {
            int dealtCardIndex = 0;
            do
            {
                var randomIndex = this.random.Next(this.GameDeck.Length);
                var card = this.GameDeck[randomIndex];
                bool cardIsDealt = this.GameDealtCards.Contains(card);
                if (!cardIsDealt)
                {
                    card.DealtPosition = dealtCardIndex;
                    this.GameDealtCards[dealtCardIndex] = card;
                    dealtCardIndex++;
                }
            }
            while (dealtCardIndex < this.GameDealtCards.Length);

            this.GameDealtCards[0].IsVisible = true;
            this.GameDealtCards[1].IsVisible = true;

            this.renderer.GetCardsImages(this.GameDealtCards);
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
        /// Enabling maximization or minimization of the Form window 
        /// </summary>
        private void EnablingFormControls()
        {
            if (!this.GameEnd)
            {
                //this are form controls
                this.renderer.EnablingFormMinimizationAndMaximization();
            }
        }
        #endregion

        #region Rules logic
        private void Rules(IPlayer player)
        {
            #region Variables
            GameCard[] cardsOnTable = new GameCard[5];

            cardsOnTable[0] = this.GameDealtCards[12];
            cardsOnTable[1] = this.GameDealtCards[13];
            cardsOnTable[2] = this.GameDealtCards[14];
            cardsOnTable[3] = this.GameDealtCards[15];
            cardsOnTable[4] = this.GameDealtCards[16];

            GameCard playerFirstCard = this.GameDealtCards[player.FirstCardPosition];
            GameCard playerSecondCard = this.GameDealtCards[player.SecondCardPosition];
            #endregion

            #region High Card PokerHandMultiplier = -1
            this.rHighCard(playerFirstCard, playerSecondCard, player);
            #endregion

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

            #region Full House PokerHandMultiplier = 6
            this.rFullHouse(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            #region Four of a Kind PokerHandMultiplier = 7
            this.rFourOfAKind(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            #region Straight Flush PokerHandMultiplier = 8 || 9
            this.rStraightFlush(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion
        }

        private void rStraightFlush(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
        {
            int firstCardPairs = cardsOnTable.Count(card => firstCard.Rank == card.Rank);
            int secondCardPairs = cardsOnTable.Count(card => secondCard.Rank == card.Rank);

            SortedDictionary<int, CardSuit> allCards = new SortedDictionary<int, CardSuit>();

            if (firstCard.Rank == 14)
            {
                allCards[firstCard.Rank] = firstCard.Suit;
                allCards[1] = firstCard.Suit;
            }
            else
            {
                allCards[firstCard.Rank] = firstCard.Suit;
            }

            if (secondCard.Rank == 14)
            {
                allCards[secondCard.Rank] = secondCard.Suit;
                allCards[1] = secondCard.Suit;
            }
            else
            {
                allCards[secondCard.Rank] = secondCard.Suit;
            }

            foreach (GameCard card in cardsOnTable)
            {
                if (card.Rank == 14)
                {
                    allCards[card.Rank] = card.Suit;
                    allCards[1] = card.Suit;
                }
                else
                {
                    allCards[card.Rank] = card.Suit;
                }
            }

            List<int> allCardsKeys = new List<int>(allCards.Keys);

            int streightFlushCardsCount = 0;
            int streightFlushCardsHighestRank = 0;

            for (int index = 0; index < allCardsKeys.Count - 1; index++)
            {
                if (allCardsKeys[index] + 1 == allCardsKeys[index + 1] && allCards[allCardsKeys[index]] == allCards[allCardsKeys[index + 1]])
                {
                    streightFlushCardsCount++;

                    if (streightFlushCardsCount >= 5)
                    {
                        streightFlushCardsHighestRank = allCardsKeys[index + 1];
                    }
                }
            }

            if (streightFlushCardsCount < 5)
            {
                return;
            }

            player.PokerHandMultiplier = 8;
            player.CardPower = streightFlushCardsHighestRank * 4 + player.PokerHandMultiplier * 100;
        }

        private void rFourOfAKind(GameCard playerFirstCard, GameCard playerSecondCard, GameCard[] cardsOnTable, IPlayer player)
        {
            var allcards = new GameCard[cardsOnTable.Length + 2];
            allcards[0] = playerFirstCard;
            allcards[1] = playerSecondCard;

            for (int i = 2, counter = 0; i < allcards.Length; i++, counter++)
            {
                allcards[i] = cardsOnTable[counter];
            }

            for (int i = 2; i < 15; i++)
            {
                var cardsGroupedByRank = allcards.Where(c => c.Rank == i);
                if (cardsGroupedByRank.Count() == 4)
                {
                    var firstCard = cardsGroupedByRank.First();
                    player.PokerHandMultiplier = 7;
                    player.CardPower = firstCard.Rank * 4 + player.PokerHandMultiplier * 100;
                }
            }
        }

        private void rFullHouse(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
        {
            List<GameCard> allCards = new List<GameCard>();
            allCards.Add(firstCard);
            allCards.Add(secondCard);
            allCards.AddRange(cardsOnTable);

            var tableCardsGrouped = allCards
                .GroupBy(card => card.Rank)
                .Select(group => new
                {
                    Rank = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(group => group.Count)
                .ThenByDescending(rank => rank.Rank);

            bool foundTreeOfAKind = false;
            int foundTreeOfAKindRank = 0;

            bool foundTwoOfAKind = false;
            int foundTwoOfAKindRank = 0;


            foreach (var cardGroup in tableCardsGrouped)
            {
                if (cardGroup.Count == 3)
                {
                    foundTreeOfAKind = true;
                    foundTreeOfAKindRank = cardGroup.Rank;
                }

                if (cardGroup.Count == 2)
                {
                    foundTwoOfAKind = true;
                    foundTwoOfAKindRank = cardGroup.Rank;
                    break;
                }
            }

            if (!foundTreeOfAKind || !foundTwoOfAKind)
            {
                return;
            }

            player.PokerHandMultiplier = 6;
            player.CardPower = foundTreeOfAKindRank + foundTwoOfAKindRank / 10 + player.PokerHandMultiplier * 100;

        }

        private void rFlush(GameCard playerFirstCard, GameCard playerSecondCard, GameCard[] cardsOnTable, IPlayer player)
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

        private void CheckForFlush(GameCard firstCard, GameCard secondCard, GameCard[] cardsOfSameSuitOnTable, IPlayer player)
        {
            var allcardsOnTableAndPlayers = new GameCard[cardsOfSameSuitOnTable.Length + 2];
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

        private void rStraight(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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
                if (secondCard.Rank == 14)
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

            if (countConsecutives < 5)
            {
                return;
            }

            player.PokerHandMultiplier = 4;
            player.CardPower = highestCardRankInStreight * 4 + player.PokerHandMultiplier * 100;
        }

        private void rThreeOfAKind(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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

            GameCard highestCard = this.GetHighestCardInHand(firstCard, secondCard);

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

        private void rTwoPair(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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
                        GameCard highestCardInHand = this.GetHighestCardInHand(firstCard, secondCard);
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

        private void rPair(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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

        private void rPairFromHand(GameCard firstCard, GameCard secondCard, IEnumerable<GameCard> cardsOnTable, IPlayer player)
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

        private void rHighCard(GameCard firstCard, GameCard secondCard, IPlayer player)
        {
            GameCard highestCard = this.GetHighestCardInHand(firstCard, secondCard);

            player.PokerHandMultiplier = -1;
            player.CardPower = highestCard.Rank;
        }

        private GameCard GetHighestCardInHand(GameCard firstCard, GameCard secondCard)
        {
            GameCard highestCard = firstCard.Rank > secondCard.Rank ? firstCard : secondCard;
            return highestCard;
        }
        #endregion

        public async Task Turns()
        {
            #region Rotating
            if (!this.Gamer.GameEnded && this.Gamer.Turn && !this.Gamer.Folded)
            {
                this.CallAllPlayerActionsOnTurn(this.Gamer);
            }
            else
            {
                await this.CheckForDealingFinish();

                if (this.Gamer.GameEnded && !this.Gamer.Folded)
                {
                    if (this.Gamer.IsAllIn == false)
                    {
                        this.RemovePlayerFromTheGame(this.Gamer.Id);
                    }
                }

                await this.CheckForGameRound(this.Gamer.Id);

                if (this.Gamer.Folded)
                {
                    this.Gamer.CanRaise = false;
                    this.Gamer.CanCall = false;
                    this.Gamer.CanCheck = false;
                    this.Gamer.CanFold = false;
                }
                else
                {
                    this.Gamer.CanRaise = true;
                    this.Gamer.CanCall = true;
                    this.Gamer.CanCheck = true;
                    this.Gamer.CanFold = true;
                }

                this.renderer.ManageGamersEntities(this.Gamer);

                this.Players[1].Turn = true;
                
                for (int botIndex = 1; botIndex < this.Players.Length; botIndex++)
                {
                    if (!this.Players[botIndex].GameEnded && !this.Players[botIndex].Folded)
                    {
                        if (this.Players[botIndex].Turn)
                        {
                            this.CallAllBotActionsOnTheirTurn(this.Players[botIndex]);
                        }
                    }

                    if (this.Players[botIndex].GameEnded || this.Players[botIndex].Folded)
                    {
                        this.RemovePlayerFromTheGame(botIndex);
                    }

                    if (!this.Players[botIndex].GameEnded && this.Players[botIndex].Turn)
                    {
                        continue;
                    }

                    //Calls all the rest of the bot to act
                    await this.CheckForGameRound(botIndex);

                    if (botIndex + 1 == this.Players.Length)
                    {
                        this.Gamer.Turn = true;
                    }
                    else
                    {
                        this.Players[botIndex + 1].Turn = true;
                    }
                }
            #endregion

                await this.CheckForDealingFinish();

                // game loop => while the game end is not true call turns again
                if (!this.GameEnd)
                {
                    await this.Turns();
                }

                this.GameEnd = false;
            }
        }

        private void SetWinners(string lastly, IPlayer player)
        {
            if (lastly == " ")
            {
                lastly = "Bot 5";
            }

            this.renderer.ShowAllCards();

            var highestCardPowerInCurrentGame = this.Players
                .Where(pl => !pl.GameEnded)
                .Where(b => !b.Folded)
                .OrderByDescending(p => p.CardPower)
                .First().CardPower;

            if (player.CardPower == highestCardPowerInCurrentGame)
            {
                this.Table.WinnersCount++;

                this.winnersNames.Add(player.Name);

                if (player.PokerHandMultiplier == -1)
                {
                    this.renderer.ShowMessage(player.Name + " High Card ");
                }

                if (player.PokerHandMultiplier == 1 || player.PokerHandMultiplier == 0)
                {
                    this.renderer.ShowMessage(player.Name + " Pair ");
                }

                if (player.PokerHandMultiplier == 2)
                {
                    this.renderer.ShowMessage(player.Name + " Two Pair ");
                }

                if (player.PokerHandMultiplier == 3)
                {
                    this.renderer.ShowMessage(player.Name + " Three of a Kind ");
                }

                if (player.PokerHandMultiplier == 4)
                {
                    this.renderer.ShowMessage(player.Name + " Straight ");
                }

                if (player.PokerHandMultiplier == 5 || player.PokerHandMultiplier == 5.5)
                {
                    this.renderer.ShowMessage(player.Name + " Flush ");
                }

                if (player.PokerHandMultiplier == 6)
                {
                    this.renderer.ShowMessage(player.Name + " Full House ");
                }

                if (player.PokerHandMultiplier == 7)
                {
                    this.renderer.ShowMessage(player.Name + " Four of a Kind ");
                }

                if (player.PokerHandMultiplier == 8)
                {
                    this.renderer.ShowMessage(player.Name + " Straight Flush ");
                }

                if (player.PokerHandMultiplier == 9)
                {
                    this.renderer.ShowMessage(player.Name + " Royal Flush ! ");
                }
            }


            if (player.Name == lastly) //lastfixed
            {
                if (this.Table.WinnersCount > 1)
                {
                    if (this.winnersNames.Contains("Player"))
                    {
                        this.Gamer.Chips += this.Table.Pot / this.Table.WinnersCount;
                    }

                    if (this.winnersNames.Contains("Bot 1"))
                    {
                        this.Players[1].Chips += this.Table.Pot / this.Table.WinnersCount;
                    }

                    if (this.winnersNames.Contains("Bot 2"))
                    {
                        this.Players[2].Chips += this.Table.Pot / this.Table.WinnersCount;
                    }

                    if (this.winnersNames.Contains("Bot 3"))
                    {
                        this.Players[3].Chips += this.Table.Pot / this.Table.WinnersCount;
                    }

                    if (this.winnersNames.Contains("Bot 4"))
                    {
                        this.Players[4].Chips += this.Table.Pot / this.Table.WinnersCount;
                    }

                    if (this.winnersNames.Contains("Bot 5"))
                    {
                        this.Players[5].Chips += this.Table.Pot / this.Table.WinnersCount;
                    }
                }

                if (this.Table.WinnersCount == 1)
                {
                    if (this.winnersNames.Contains("Player"))
                    {
                        this.Gamer.Chips += this.Table.Pot;
                    }

                    if (this.winnersNames.Contains("Bot 1"))
                    {
                        this.Players[1].Chips += this.Table.Pot;
                    }

                    if (this.winnersNames.Contains("Bot 2"))
                    {
                        this.Players[2].Chips += this.Table.Pot;
                    }

                    if (this.winnersNames.Contains("Bot 3"))
                    {
                        this.Players[3].Chips += this.Table.Pot;
                    }

                    if (this.winnersNames.Contains("Bot 4"))
                    {
                        this.Players[4].Chips += this.Table.Pot;
                    }

                    if (this.winnersNames.Contains("Bot 5"))
                    {
                        this.Players[5].Chips += this.Table.Pot;
                    }
                }
            }
        }

        private async Task CheckForGameRound(int playerId)
        {
            if (this.Table.IsRaising)
            {
                this.Table.TurnCount = 0;
                this.Table.IsRaising = false;
                this.Table.LastRaisedPlayerId = playerId;
                this.tableIsChanged = true;
            }
            else
            {
                if ((this.Table.TurnCount >= this.Table.PlayersInTheGame - 1 || !this.tableIsChanged) &&
                    this.Table.TurnCount == this.Table.PlayersInTheGame)
                {
                    if ((playerId == this.Table.LastRaisedPlayerId - 1 || !this.tableIsChanged) &&
                        (this.Table.TurnCount == this.Table.PlayersInTheGame || this.Table.LastRaisedPlayerId == 0) &&
                        playerId == 5)
                    {
                        this.tableIsChanged = false;
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

            if (this.Table.Rounds == PokerFlop)
            {
                for (int cardForFlop = 12; cardForFlop <= 14; cardForFlop++)
                {
                    this.GameDealtCards[cardForFlop].IsVisible = true;
                    this.renderer.Draw(this.GameDealtCards[cardForFlop]);
                }

                foreach (var player in this.Players)
                {
                    player.Call = 0;
                    player.Raise = 0;
                }
            }

            if (this.Table.Rounds == PokerTurn)
            {
                for (int j = 14; j <= 15; j++)
                {
                    this.GameDealtCards[j].IsVisible = true;
                    this.renderer.Draw(this.GameDealtCards[j]);
                }

                foreach (var player in this.Players)
                {
                    player.Call = 0;
                    player.Raise = 0;
                }
            }

            if (this.Table.Rounds == PokerRiver)
            {
                for (int j = 15; j <= 16; j++)
                {
                    this.GameDealtCards[j].IsVisible = true;
                    this.renderer.Draw(this.GameDealtCards[j]);
                }

                foreach (var player in this.Players)
                {
                    player.Call = 0;
                    player.Raise = 0;
                }
            }

            if (this.Table.Rounds == PokerEndRound &&
                this.Table.PlayersInTheGame == 6)
            {
                string fixedLast = "";

                foreach (var player in this.Players)
                {
                    if (!player.Folded)
                    {
                        fixedLast = player.Name;
                    }
                }

                foreach (IPlayer player in this.Players)
                {
                    this.SetWinners(fixedLast, player);
                }

                this.GameEnd = true;

                this.Gamer.Turn = true;

                for (int playerIndex = 0; playerIndex < this.Players.Length; playerIndex++)
                {
                    this.Players[playerIndex].GameEnded = false;
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

                        this.renderer.Draw(this.Gamer);
                    }
                }

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
                this.playersNotGameEnded.Clear();
                this.Table.Rounds = 0;
                this.type = 0;

                this.playersWithoutChips.Clear();
                this.winnersNames.Clear();
                this.Table.WinnersCount = 0;

                for (int os = 0; os < 17; os++)
                {
                    this.GameDealtCards[os].CardFrontImageUri = null;
                    this.GameDealtCards[os].IsVisible = false;
                }

                this.Table.Pot = 0;
                this.Gamer.Status = string.Empty;
                this.renderer.SetAllLabelStatus((IPlayer[])this.Players.Where(p => p.Name == "Player"));

                await this.Turns();
            }
        }

        private void CalculatePlayersBets(IPlayer player, int options)
        {
            string playerLableText = player.Status;

            if (this.Table.Rounds == 4)
            {
                return;
            }

            if (options == 1)
            {
                if (player.Status == "Check")
                {
                    player.Raise = 0;
                    player.Call = 0;
                }
            }

            if (options != 2)
            {
                return;
            }

            if (player.Raise != this.Table.LastRaise &&
                player.Raise <= this.Table.LastRaise)
            {
                this.Table.PokerCall = this.Table.LastRaise - player.Raise;
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
            }
        }

        private async Task CheckForDealingFinish()
        {
            #region All in
            if (this.Gamer.Chips <= 0 && !this.intsadded)
            {
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

            if (this.playersWithoutChips.Count == this.Table.PlayersInTheGame)
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
                    this.Gamer.Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[0]);
                    this.renderer.ShowMessage("Player Wins");
                }

                if (index == 1)
                {
                    this.Players[1].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[1]);
                    this.renderer.ShowMessage("Bot 1 Wins");
                }

                if (index == 2)
                {
                    this.Players[2].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[2]);
                    this.renderer.ShowMessage("Bot 2 Wins");
                }

                if (index == 3)
                {
                    this.Players[3].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[3]);
                    this.renderer.ShowMessage("Bot 3 Wins");
                }

                if (index == 4)
                {
                    this.Players[4].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[4]);
                    this.renderer.ShowMessage("Bot 4 Wins");
                }

                if (index == 5)
                {
                    this.Players[5].Chips += this.Table.Pot;
                    this.renderer.SetTextBoxPlayerChips(Players[5]);
                    this.renderer.ShowMessage("Bot 5 Wins");
                }

                await this.Finish(1);
            }
            this.intsadded = false;
            #endregion

            #region FiveOrLessLeft
            if (playersNotGameEndedCount > 1 &&
                this.Table.Rounds >= PokerEndRound)
            {
                await this.Finish(2);
            }
            #endregion
        }

        private async Task Finish(int n)
        {
            if (n == 2)
            {
                this.SetWinners();
            }

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
            this.winnersNames.Clear();
            this.playersWithoutChips.Clear();

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
                    //this.renderer.Draw(this.Players);
                    //TODO :  raise value setup
                    //this.form.buttonRaise.Text = "Raise";
                }
            }

            await this.SetupPokerTable();
        }

        private void SetWinners()
        {
            string setLast = "";

            foreach (var player in this.Players)
            {
                if (player.Status == "Fold")
                {
                    setLast = player.Name;
                }
            }

            foreach (IPlayer player in this.Players)
            {
                this.SetWinners(setLast, player);
            }
        }

        #region AI logic
        private void AI(IPlayer player)
        {
            if (player.GameEnded)
            {
                return;
            }

            switch (player.PokerHandMultiplier)
            {
                case -1:
                    this.AIHighCard(player);
                    break;
                case 0:
                    this.AIPairTable(player);
                    break;
                case 1:
                    this.AIPairHand(player);
                    break;
                case 2:
                    this.AITwoPair(player);
                    break;
                case 3:
                    this.AIThreeOfAKind(player);
                    break;
                case 4:
                    this.AIStraight(player);
                    break;
                case 5:
                    this.AIFlush(player);
                    break;
                case 6:
                    this.AIFullHouse(player);
                    break;
                case 7:
                    this.AIFourOfAKind(player);
                    break;
                case 8:
                    this.AIStraightFlush(player);
                    break;
            }
        }

        private void AIHighCard(IPlayer player)
        {
            this.AIHP(player, 20, 25);
        }

        private void AIPairTable(IPlayer player)
        {
            this.AIHP(player, 16, 25);
        }

        private void AIPairHand(IPlayer player)
        {
            //108 - 156
            Random rPair = new Random();

            int rCall = rPair.Next(10, 16);

            int rRaise = rPair.Next(10, 13);

            if (player.CardPower <= 199 &&
                player.CardPower > 140)
            {
                this.AIPH(player, rCall, 6, rRaise);
            }

            if (player.CardPower <= 140 &&
                player.CardPower >= 124)
            {
                this.AIPH(player, rCall, 7, rRaise);
            }

            if (player.CardPower < 124 &&
                player.CardPower >= 108)
            {
                this.AIPH(player, rCall, 9, rRaise);
            }
        }

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
                player.CardPower >= 400)
            {
                this.AISmooth(player, sCall, sRaise);
            }
        }

        private void AIFlush(IPlayer player)
        {
            Random fsh = new Random();
            int fCall = fsh.Next(2, 6);
            int fRaise = fsh.Next(3, 7);
            this.AISmooth(player, fCall, fRaise);
        }

        private void AIFullHouse(IPlayer player)
        {
            Random random = new Random();
            int fhCall = random.Next(1, 5);
            int fhRaise = random.Next(2, 6);

            if (player.CardPower <= 615 &&
                player.CardPower >= 610)
            {
                this.AISmooth(player, fhCall, fhRaise);
            }

            if (player.CardPower < 610 &&
                player.CardPower > 602)
            {
                this.AISmooth(player, fhCall, fhRaise);
            }
        }

        private void AIFourOfAKind(IPlayer player)
        {
            Random random = new Random();
            int fkCall = random.Next(1, 4);
            int fkRaise = random.Next(2, 5);
            if (player.CardPower <= 756 &&
                player.CardPower >= 702)
            {
                this.AISmooth(player, fkCall, fkRaise);
            }
        }

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

        private void AIHP(IPlayer player, int n, int n1)
        {
            Random rand = new Random();

            int round = rand.Next(1, 4);

            if (this.Table.PokerCall == 0)
            {
                this.AICheck(player);
            }

            if (this.Table.PokerCall > 0)
            {
                if (round == 1)
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

                if (round == 2)
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

            if (round == 3)
            {
                if (this.Table.LastRaise == 0)
                {
                    this.Table.LastRaise = this.Table.PokerCall * 2;

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

        private void AIPH(IPlayer player, int n, int n1, int r)
        {
            Random rand = new Random();
            int randomNumber = rand.Next(1, 3);

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
                    if (this.Table.PokerCall >= AIRoundNumber(player.Chips, n1 - randomNumber))
                    {
                        this.AIFold(player);
                    }

                    if (this.Table.LastRaise > AIRoundNumber(player.Chips, n - randomNumber))
                    {
                        this.AIFold(player);
                    }

                    if (!player.GameEnded)
                    {
                        if (this.Table.PokerCall >= AIRoundNumber(player.Chips, n - randomNumber) &&
                            this.Table.PokerCall <= AIRoundNumber(player.Chips, n1 - randomNumber))
                        {
                            this.AICall(player);
                        }

                        if (this.Table.LastRaise <= AIRoundNumber(player.Chips, n - randomNumber) &&
                            this.Table.LastRaise >= AIRoundNumber(player.Chips, n - randomNumber) / 2)
                        {
                            this.AICall(player);
                        }

                        if (this.Table.LastRaise <= AIRoundNumber(player.Chips, n - randomNumber) / 2)
                        {
                            if (this.Table.LastRaise > 0)
                            {
                                this.Table.LastRaise = (int)AIRoundNumber(player.Chips, n - randomNumber);
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
                    this.Table.LastRaise = (int)AIRoundNumber(player.Chips, r - randomNumber);
                    this.AIRaised(player);
                }
            }

            if (player.Chips <= 0)
            {
                player.GameEnded = true;
            }
        }

        private void AISmooth(IPlayer player, int n, int r)
        {
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
                        this.Table.IsRaising = false;
                        player.Turn = false;
                        player.Chips = 0;
                        player.Status = "All In " + player.Chips;
                        this.Table.Pot += player.Chips;
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

        private void AIFold(IPlayer player)
        {
            this.Table.IsRaising = false;
            player.Status = "Fold";
            player.Turn = false;
            player.Folded = true;
            this.renderer.Draw(player);
        }

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
            player.Turn = false;
            this.Table.IsRaising = false;
            this.renderer.Draw(player);
        }

        private void AICall(IPlayer player)
        {
            this.Table.IsRaising = false;
            player.Turn = false;
            player.Chips -= this.Table.PokerCall;
            player.Status = "Call " + this.Table.PokerCall;
            this.Table.Pot += this.Table.PokerCall;
            this.renderer.Draw(player);
        }

        private void AIRaised(IPlayer player)
        {
            player.Chips -= this.Table.LastRaise;
            player.Status = "Raise " + this.Table.LastRaise;
            this.Table.Pot += this.Table.LastRaise;
            this.Table.PokerCall = Convert.ToInt32(this.Table.LastRaise);
            this.Table.IsRaising = true;
            player.Turn = false;
            this.renderer.Draw(player);
        }

        private static double AIRoundNumber(int botChips, int n)
        {
            double a = Math.Round((botChips / n) / 100d, 0) * 100;
            return a;
        }
        #endregion

        #region push buttons logic
        public void GammerFolds()
        {
            this.Gamer.Status = "Fold";
            this.Gamer.Folded = true;
            this.Gamer.Turn = false;
            //this.Gamer.GameEnded = true;
            this.renderer.Draw(this.Gamer);
        }

        public void GammerChecks()
        {
            Gamer gamer = (Gamer)this.Gamer;

            if (this.Table.PokerCall <= 0)
            {
                gamer.Turn = false;
                gamer.Status = "Check";
            }
            else
            {
                gamer.CanCheck = false;
            }

            this.renderer.Draw(gamer);
        }

        public void GammerMoveTimeExpired()
        {
            this.Gamer.GameEnded = true;
            this.renderer.ShowMessage("Your time is up");
            this.renderer.Draw(this.Gamer);
        }

        public void GammerCalls()
        {
            Gamer gamer = (Gamer)this.Gamer;

            if (gamer.Chips >= this.Table.PokerCall)
            {
                gamer.Chips -= this.Table.PokerCall;
                this.Table.Pot += this.Table.PokerCall;
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
            }

            this.renderer.Draw(gamer);
            this.renderer.Draw(this.Table);
        }

        public void GammerRaises()
        {
            Gamer gamer = (Gamer)this.Gamer;

            int valueToRaise = this.inputHandlerer.ReadRaise();
            gamer.ValueToRaise = valueToRaise;

            if (gamer.Chips > this.Table.PokerCall)
            {
                if (this.Table.LastRaise * 2 > gamer.ValueToRaise)
                {
                    this.renderer.ShowMessage("You must raise at least doubling the current raise!");
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
                    gamer.Status = "All In" + this.Table.PokerCall;
                    gamer.Chips = 0;
                    gamer.IsAllIn = true;
                    gamer.GameEnded = true;
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
            //this.renderer.Draw(this.Players);
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
            for (int playerCount = 0, cardFirstPosition = 0; playerCount < GlobalConstants.PlayersCount; playerCount++, cardFirstPosition += 2)
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
            this.playersNotGameEnded[playerIndex] = null;

            this.Table.PlayersInTheGame--;

            this.Players[playerIndex].Folded = true;

            this.Players[playerIndex].Status = string.Empty;
        }

        private void CallAllPlayerActionsOnTurn(IPlayer player)
        {
            this.CalculatePlayersBets(player, 1);
            this.Table.TurnCount++;
            this.CalculatePlayersBets(player, 2);
        }

        /// <summary>
        /// Calls all actions that must be called if it is bot turn
        /// </summary>
        private void CallAllBotActionsOnTheirTurn(IPlayer player)
        {
            var botNumber = player.Id;

            this.CalculatePlayersBets(player, 1);
            this.CalculatePlayersBets(player, 2);

            this.renderer.ShowMessage("Bot  " + botNumber + @"'s Turn");
            this.AI(player);

            this.Table.TurnCount++;
            this.Table.LastBotPlayed = botNumber;
            player.Turn = false;

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