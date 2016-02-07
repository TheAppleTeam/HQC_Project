namespace Poker.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameObjects.Cards;
    using GameObjects.Player;

    public class HandCalculator
    {
        private GameEngine gameEngine;

        public HandCalculator(GameEngine gameEngine)
        {
            this.gameEngine = gameEngine;
        }

        public void CalculateHighHand(IPlayer player)
        {
            #region Variables
            GameCard[] cardsOnTable = new GameCard[5];

            cardsOnTable[0] = gameEngine.GameDealtCards[12];
            cardsOnTable[1] = gameEngine.GameDealtCards[13];
            cardsOnTable[2] = gameEngine.GameDealtCards[14];
            cardsOnTable[3] = gameEngine.GameDealtCards[15];
            cardsOnTable[4] = gameEngine.GameDealtCards[16];

            GameCard playerFirstCard = gameEngine.GameDealtCards[player.FirstCardPosition];
            GameCard playerSecondCard = gameEngine.GameDealtCards[player.SecondCardPosition];
            #endregion

            double maxCardPower = 0;

            #region Straight Flush PokerHandMultiplier = 8 || 9
            this.StraightFlushCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region Four of a Kind PokerHandMultiplier = 7
            this.FourOfAKindCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region Full House PokerHandMultiplier = 6
            this.FullHouseCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region Flush PokerHandMultiplier = 5 || 5.5
            this.FlushCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region Straight PokerHandMultiplier = 4
            this.StraightCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }


            #region Three of a kind PokerHandMultiplier = 3
            this.ThreeOfAKindCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region Two Pair PokerHandMultiplier = 2
            this.TwoPairCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region Pair from hand PokerHandMultiplier = 1
            this.PairFromHandCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region Pair from Table PokerHandMultiplier = 0
            this.PairCheck(playerFirstCard, playerSecondCard, cardsOnTable, player);
            #endregion

            if (maxCardPower > player.CardPower)
            {
                return;
            }
            else
            {
                maxCardPower = player.CardPower;
            }

            #region High Card PokerHandMultiplier = -1
            this.HighCardCheck(playerFirstCard, playerSecondCard, player);
            #endregion
        }

        private void StraightFlushCheck(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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

        private void FourOfAKindCheck(GameCard playerFirstCard, GameCard playerSecondCard, GameCard[] cardsOnTable, IPlayer player)
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

        private void FullHouseCheck(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
        {
            List<GameCard> allCards = new List<GameCard>();
            allCards.Add(firstCard);
            allCards.Add(secondCard);
            allCards.AddRange(cardsOnTable);

            var tableCardsGrouped = allCards
                .GroupBy(card => card.Rank)
                .Select(group => new
                {
                    Rank = @group.Key,
                    Count = @group.Count()
                })
                .OrderByDescending(group => @group.Count)
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

        private void FlushCheck(GameCard playerFirstCard, GameCard playerSecondCard, GameCard[] cardsOnTable, IPlayer player)
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

        private void StraightCheck(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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

        private void ThreeOfAKindCheck(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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
                .Select(group => new { Rank = @group.Key, Count = @group.Count() })
                .OrderByDescending(group => @group.Count)
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

        private void TwoPairCheck(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
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
                        player.PokerHandMultiplier = 2;
                        player.CardPower = firstCard.Rank * 2 + secondCard.Rank * 2 + player.PokerHandMultiplier * 100;
                    }

                    break;
                case 1:
                    if (firstCard.Rank == secondCard.Rank)
                    {
                        player.PokerHandMultiplier = 2;
                        player.CardPower = firstCard.Rank * 2 + highestTablePairRank * 2 + player.PokerHandMultiplier * 100;
                    }
                    else if (firstCardPairCount == 1 && secondCardPairCount != 1)
                    {
                        player.PokerHandMultiplier = 2;
                        player.CardPower = firstCard.Rank * 2 + highestTablePairRank * 2 + player.PokerHandMultiplier * 100;
                    }
                    else if (secondCardPairCount == 1 && firstCardPairCount != 1)
                    {
                        player.PokerHandMultiplier = 2;
                        player.CardPower = secondCard.Rank * 2 + highestTablePairRank * 2 + player.PokerHandMultiplier * 100;
                    }
                    else if (secondCardPairCount == 1 && firstCardPairCount == 1)
                    {
                        int highestRank;
                        int secondHighestRank;

                        if (firstCard.Rank > secondCard.Rank)
                        {
                            highestRank = firstCard.Rank;
                            secondHighestRank = secondCard.Rank;
                        }
                        else
                        {
                            highestRank = secondCard.Rank;
                            secondHighestRank = firstCard.Rank;
                        }

                        if (secondHighestRank < highestTablePairRank)
                        {
                            secondHighestRank = highestTablePairRank;
                        }

                        player.PokerHandMultiplier = 2;
                        player.CardPower = highestRank * 2 + secondHighestRank * 2 +  player.PokerHandMultiplier * 100;
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

        private void PairCheck(GameCard firstCard, GameCard secondCard, GameCard[] cardsOnTable, IPlayer player)
        {
            int firstCardPairCount = cardsOnTable.Count(card => firstCard.Rank == card.Rank);
            int secondCardPairCount = cardsOnTable.Count(card => secondCard.Rank == card.Rank);

            int pairsOnTableCount = cardsOnTable
                .GroupBy(card => card.Rank)
                .Select(group => new { Count = @group.Count() })
                .Count(group => @group.Count == 2);


            if ((firstCardPairCount == 1 ^ secondCardPairCount == 1) &&
                firstCard.Rank == secondCard.Rank)
            {
                if (pairsOnTableCount != 1)
                {
                    return;
                }
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

            var pairOnTableRank = cardsOnTable
                .GroupBy(card => card.Rank)
                .Select(group => new
                {
                    Rank = @group.Key,
                    Count = @group.Count()
                })
                .Where(group => @group.Count == 1)
                .Select(newGroup => newGroup.Rank)
                .First();

            player.PokerHandMultiplier = 0;
            player.CardPower = pairOnTableRank * 4 + player.PokerHandMultiplier * 100;
        }

        private void PairFromHandCheck(GameCard firstCard, GameCard secondCard, IEnumerable<GameCard> cardsOnTable, IPlayer player)
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

        private void HighCardCheck(GameCard firstCard, GameCard secondCard, IPlayer player)
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
    }
}