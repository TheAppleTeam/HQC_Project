//namespace Poker.Engines
//{
//    using System;
//    using System.Globalization;
//    using System.Windows.Forms;
//    using GameObjects.Player;

//    public class AI
//    {
       
//        //// Plamena

//        #region AI logic
//        //// AI   (     2,      3, ref    bot1Chips, ref this.players[1].Turn,ref this.players[1].GameEnded, labelBot1Status,       0, this.bot1CardPower, this.bot1HandMultiplier);
//        /* wika se ot Turns : 
//            if (!this.players[1].GameEnded) => if (this.players[1].Turn):
//            if (!this.players[2].GameEnded) => if (this.players[2].Turn):
//            if (!this.players[3].GameEnded) => if (this.bo3Turn) :
//            if (!this.players[4].GameEnded) => if (this.players[4].Turn) :
//            if (!players[5].GameEnded) =>  if (this.players[5].Turn) =>  
//        => AI(2, 3, ref bot1Chips, ref this.players[1].Turn, ref this.players[1].GameEnded, labelBot1Status, 0, this.bot1CardPower, this.bot1HandMultiplier);
//        => AI(4, 5, ref bot2Chips, ref this.players[2].Turn, ref this.players[2].GameEnded, labelBot2Status, 1, this.bot2CardPower, this.bot2HandMultiplier);
//        => AI(6, 7, ref bot3Chips, ref this.players[3].Turn, ref this.players[3].GameEnded, labelBot3Status, 2, this.bot3CardPower, this.bot3HandMultiplier);
//        => AI(8, 9, ref bot4Chips, ref this.players[4].Turn, ref this.players[4].GameEnded, labelBot4Status, 3, this.bot4CardPower, this.bot4HandMultiplier);
//        => AI(10, 11, ref bot5Chips, ref this.players[5].Turn, ref  players[5].GameEnded, labelBot5Status, 4, this.bot5CardPower, this.bot5HandMultiplier);
//         note: int name se polzwa samo pri wikaneto na smooth*/

//        private void AI(IPlayer player)
//        {
//            // TODO: remove first and second numeration
//            var firstCardNumeration = player.FirstCardPosition;
//            var secondNumeration = player.SecondCardPosition;
//            if (!player.GameEnded)
//            {
//                switch (player.PokerHandMultiplier.ToString(CultureInfo.CreateSpecificCulture("en-GB")))
//                {
//                    case "-1":
//                        this.AIHighCard(player);
//                        break;
//                    case "0":
//                        this.AIPairTable(player);
//                        break;
//                    case "1":
//                        this.AIPairHand(player);
//                        break;
//                    case "2":
//                        this.AITwoPair(player);
//                        break;
//                    case "3":
//                        this.AIThreeOfAKind(player);
//                        break;
//                    case "4":
//                        this.AIStraight(player);
//                        break;
//                    case "5":
//                    case "5.5":
//                        this.AIFlush(player);
//                        break;
//                    case "6":
//                        this.AIFullHouse(player);
//                        break;
//                    case "7":
//                        this.AIFourOfAKind(player);
//                        break;
//                    case "8":
//                    case "9":
//                        this.AIStraightFlush(player);
//                        break;
//                }
//            }

//            if (player.GameEnded)
//            {
//                this.cardHolder.cardHolder[firstCardNumeration].Visible = false;
//                this.cardHolder[secondNumeration].Visible = false;
//            }
//        }

//        // Wika se ot AI this.AIHighCard(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika HP
//        private void AIHighCard(IPlayer player)
//        {
//            this.AIHP(player, 20, 25);
//        }

//        //// wika se ot AI this.AIPairTable(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika HP
//        private void AIPairTable(IPlayer player)
//        {
//            this.AIHP(player, 16, 25);
//        }

//        //// wika se ot AI this.AIPairHand(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
//        private void AIPairHand(IPlayer player)
//        {
//            Random rPair = new Random();

//            int rCall = rPair.Next(10, 16);

//            int rRaise = rPair.Next(10, 13);

//            if (player.CardPower <= 199 &&
//                player.CardPower >= 140)
//            {
//                this.AIPH(player, rCall, 6, rRaise);
//            }

//            if (player.CardPower <= 139 &&
//                player.CardPower >= 128)
//            {
//                this.AIPH(player, rCall, 7, rRaise);
//            }

//            if (player.CardPower < 128 &&
//                player.CardPower >= 101)
//            {
//                this.AIPH(player, rCall, 9, rRaise);
//            }
//        }

//        //// wika se ot AI this.AITwoPair(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, botPower); wika AIPH
//        private void AITwoPair(IPlayer player)
//        {
//            Random rPair = new Random();
//            int rCall = rPair.Next(6, 11);
//            int rRaise = rPair.Next(6, 11);

//            if (player.CardPower <= 290 &&
//                player.CardPower >= 246)
//            {
//                this.AIPH(player, rCall, 3, rRaise);
//            }

//            if (player.CardPower <= 244 &&
//                player.CardPower >= 234)
//            {
//                this.AIPH(player, rCall, 4, rRaise);
//            }

//            if (player.CardPower < 234 &&
//                player.CardPower >= 201)
//            {
//                this.AIPH(player, rCall, 4, rRaise);
//            }
//        }

//        // wika se ot AI this.AIThreeOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth
//        private void AIThreeOfAKind(IPlayer player)
//        {
//            Random tk = new Random();
//            int tCall = tk.Next(3, 7);
//            int tRaise = tk.Next(4, 8);

//            if (player.CardPower <= 390 &&
//                player.CardPower >= 330)
//            {
//                this.AISmooth(player, tCall, tRaise);
//            }

//            if (player.CardPower <= 327 &&
//                player.CardPower >= 321) //10  8
//            {
//                this.AISmooth(player, tCall, tRaise);
//            }

//            if (player.CardPower < 321 &&
//                player.CardPower >= 303) //7 2
//            {
//                this.AISmooth(player, tCall, tRaise);
//            }
//        }

//        // wika se ot AI this.AIStraight(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
//        private void AIStraight(IPlayer player)
//        {
//            Random str = new Random();
//            int sCall = str.Next(3, 6);
//            int sRaise = str.Next(3, 8);

//            if (player.CardPower <= 480 &&
//                player.CardPower >= 410)
//            {
//                this.AISmooth(player, sCall, sRaise);
//            }

//            if (player.CardPower <= 409 &&
//                player.CardPower >= 407) //10  8
//            {
//                this.AISmooth(player, sCall, sRaise);
//            }

//            if (player.CardPower < 407 &&
//                player.CardPower >= 404)
//            {
//                this.AISmooth(player, sCall, sRaise);
//            }
//        }

//        //// wika se ot AI this.AIFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
//        private void AIFlush(IPlayer player)
//        {
//            Random fsh = new Random();
//            int fCall = fsh.Next(2, 6);
//            int fRaise = fsh.Next(3, 7);
//            this.AISmooth(player, fCall, fRaise);
//        }

//        //// wika se ot AI : this.AIFullHouse(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth
//        private void AIFullHouse(IPlayer player)
//        {
//            Random random = new Random();
//            int fhCall = random.Next(1, 5);
//            int fhRaise = random.Next(2, 6);

//            if (player.CardPower <= 626 &&
//                player.CardPower >= 620)
//            {
//                this.AISmooth(player, fhCall, fhRaise);
//            }

//            if (player.CardPower < 620 &&
//                player.CardPower >= 602)
//            {
//                this.AISmooth(player, fhCall, fhRaise);
//            }
//        }

//        //// wika se ot AI: this.AIFourOfAKind(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); break; wika Smooth
//        private void AIFourOfAKind(IPlayer player)
//        {
//            Random random = new Random();
//            int fkCall = random.Next(1, 4);
//            int fkRaise = random.Next(2, 5);
//            if (player.CardPower <= 752 &&
//                player.CardPower >= 704)
//            {
//                this.AISmooth(player, fkCall, fkRaise);
//            }
//        }

//        //// wika se ot AI: this.AIStraightFlush(ref botChips, ref botTurn, ref botGameEnded, labelBotStatus, name, botPower); wika Smooth 
//        private void AIStraightFlush(IPlayer player)
//        {
//            Random random = new Random();
//            int sfCall = random.Next(1, 3);
//            int sfRaise = random.Next(1, 3);
//            if (player.CardPower <= 913 &&
//                player.CardPower >= 804)
//            {
//                this.AISmooth(player, sfCall, sfRaise);
//            }
//        }

//        /* wika se ot: AIHighCard, AIPairTable, 
//         randoma e ot 1-3
//         ako pokerCall <= 0 bota igrae CHeck
//         ako pokerCall > 0 w zawisimost ot randoma ima 3 wyzmovnosti:
//         * 1 -> move da igrae Call ili Fold
//         * 2 -> move da igrae Call ili Fold
//         * 3 -> move da wdigne ili da Fold 
//         */

//        private void AIHP(IPlayer player, int n, int n1)
//        {
//            Random rand = new Random();

//            //// staro:int rnd = rand.Next(1, 4); t.kato rnd = 4 nikyde ne se polzwa
//            int rnd = rand.Next(1, 4);

//            if (this.pokerCall <= 0)
//            {
//                // bota igrae CHeck
//                this.AICheck(player);
//            }

//            if (this.pokerCall > 0)
//            {
//                if (rnd == 1)
//                {
//                    if (this.pokerCall <= AIRoundNumber(player.Chips, n))
//                    {
//                        this.AICall(player);
//                    }
//                    else
//                    {
//                        this.AIFold(player);
//                    }
//                }

//                if (rnd == 2)
//                {
//                    if (this.pokerCall <= AIRoundNumber(player.Chips, n1))
//                    {
//                        this.AICall(player);
//                    }
//                    else
//                    {
//                        this.AIFold(player);
//                    }
//                }
//            }

//            if (rnd == 3)
//            {
//                if (this.raise == 0)
//                {
//                    ////smqta s kolko da wdigne bota
//                    this.raise = this.pokerCall * 2;

//                    ////cbota igrae Raise
//                    this.AIRaised(player);
//                }
//                else
//                {
//                    if (this.raise <= AIRoundNumber(player.Chips, n))
//                    {
//                        this.raise = this.pokerCall * 2;
//                        this.AIRaised(player);
//                    }
//                    else
//                    {
//                        this.AIFold(player);
//                    }
//                }
//            }

//            if (player.Chips <= 0)
//            {
//                player.GameEnded = true;
//            }
//        }

//        /*wika se ot: AIPairHand, AITwoPair
//         randoma e ot 1-3
//         ako  this.rounds < 2 
//            ako pokerCall <= 0 bota igrae CHeck
//            ako pokerCall > 0 w zawisimost ot matematikata na chipowete move da FOLD, CALL, RASE
//         ako this.rounds >= 2
//                ako pokerCall <= 0 bota igrae Raise
//                ako pokerCall > 0 w zawisimost ot matematikata na chipowete i rnd, move da FOLD, CALL, RASE
//             ako botChips <= 0 => setwa botEndGame = true;
//         */

//        private void AIPH(IPlayer player, int n, int n1, int r)
//        {
//            Random rand = new Random();
//            int rnd = rand.Next(1, 3);

//            if (this.rounds < 2)
//            {
//                if (this.pokerCall <= 0)
//                {
//                    this.AICheck(player);
//                }

//                if (this.pokerCall > 0)
//                {
//                    if (this.pokerCall >= AIRoundNumber(player.Chips, n1))
//                    {
//                        this.AIFold(player);
//                    }

//                    if (this.raise > AIRoundNumber(player.Chips, n))
//                    {
//                        this.AIFold(player);
//                    }

//                    if (!player.GameEnded)
//                    {
//                        if (this.pokerCall >= AIRoundNumber(player.Chips, n) &&
//                            this.pokerCall <= AIRoundNumber(player.Chips, n1))
//                        {
//                            this.AICall(player);
//                        }

//                        if (this.raise <= AIRoundNumber(player.Chips, n) &&
//                            this.raise >= AIRoundNumber(player.Chips, n) / 2)
//                        {
//                            this.AICall(player);
//                        }

//                        if (this.raise <= AIRoundNumber(player.Chips, n) / 2)
//                        {
//                            if (this.raise > 0)
//                            {
//                                this.raise = AIRoundNumber(player.Chips, n);
//                                this.AIRaised(player);
//                            }
//                            else
//                            {
//                                this.raise = this.pokerCall * 2;
//                                this.AIRaised(player);
//                            }
//                        }
//                    }
//                }
//            }

//            if (this.rounds >= 2)
//            {
//                if (this.pokerCall > 0)
//                {
//                    if (this.pokerCall >= AIRoundNumber(player.Chips, n1 - rnd))
//                    {
//                        this.AIFold(player);
//                    }

//                    if (this.raise > AIRoundNumber(player.Chips, n - rnd))
//                    {
//                        this.AIFold(player);
//                    }

//                    if (!player.GameEnded)
//                    {
//                        if (this.pokerCall >= AIRoundNumber(player.Chips, n - rnd) &&
//                            this.pokerCall <= AIRoundNumber(player.Chips, n1 - rnd))
//                        {
//                            this.AICall(player);
//                        }

//                        if (this.raise <= AIRoundNumber(player.Chips, n - rnd) &&
//                            this.raise >= AIRoundNumber(player.Chips, n - rnd) / 2)
//                        {
//                            this.AICall(player);
//                        }

//                        if (this.raise <= AIRoundNumber(player.Chips, n - rnd) / 2)
//                        {
//                            if (this.raise > 0)
//                            {
//                                this.raise = AIRoundNumber(player.Chips, n - rnd);
//                                this.AIRaised(player);
//                            }
//                            else
//                            {
//                                this.raise = this.pokerCall * 2;
//                                this.AIRaised(player);
//                            }
//                        }
//                    }
//                }

//                if (this.pokerCall <= 0)
//                {
//                    this.raise = AIRoundNumber(player.Chips, r - rnd);
//                    this.AIRaised(player);
//                }
//            }

//            if (player.Chips <= 0)
//            {
//                player.GameEnded = true;
//            }
//        }

//        /*wika se ot: AIThreeOfAKind, AIStraight, AIFlush, AIFullHouse, AIFourOfAKind, AIStraightFlush
//         randoma e ot 1-3, NO NE SE POLZWA!
//         ako pokerCall <= 0 bota igrae CHeck
//         ako pokerCall > 0 w zawisimost ot matematikata na chipowete move da CALL, RASE
//         ako botChips <= 0 => setwa botEndGame = true;
//         */

//        private void AISmooth(IPlayer player, int n, int r)
//        {
//            //// star kod - > zakomentiran t.kato ne se polzwa: 
//            //// Random rand = new Random();
//            //// int rnd = rand.Next(1, 3);
//            if (this.pokerCall <= 0)
//            {
//                this.AICheck(player);
//            }
//            else
//            {
//                if (this.pokerCall >= AIRoundNumber(player.Chips, n))
//                {
//                    if (player.Chips > this.pokerCall)
//                    {
//                        this.AICall(player);
//                    }
//                    else if (player.Chips <= this.pokerCall)
//                    {
//                        this.raising = false;
//                        player.Turn = false;
//                        player.Chips = 0;
//                        player.Status = "Call " + player.Chips;
//                        player.Label.Text = "Call " + player.Chips;
//                        this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + player.Chips).ToString();
//                    }
//                }
//                else
//                {
//                    if (this.raise > 0)
//                    {
//                        if (player.Chips >= this.raise * 2)
//                        {
//                            this.raise *= 2;
//                            this.AIRaised(player);
//                        }
//                        else
//                        {
//                            this.AICall(player);
//                        }
//                    }
//                    else
//                    {
//                        this.raise = this.pokerCall * 2;
//                        this.AIRaised(player);
//                    }
//                }
//            }

//            if (player.Chips <= 0)
//            {
//                player.GameEnded = true;
//            }
//        }

//        // wika se ot AIHP ili AIPH  this.AIFold(ref bothTurn, ref botEndGame, sStatus);
//        private void AIFold(IPlayer player)
//        {
//            this.raising = false;
//            player.Label.Text = "Fold";
//            player.Turn = false;
//            player.Folded = true;
//            player.GameEnded = true;
//        }

//        //// вика се от AIHP, AIPH, AISmooth  this.AICheck(ref bothTurn, sStatus);

//        /// <summary>
//        /// the bot plays CHECK. 
//        /// The method sets bot's statusLable on Check; botTurn on False and both raising on False;
//        /// </summary>
//        /// <param name="cTurn">
//        /// podawa se poreferenciq t.kato nqma obekt kojto da dyrvi stojnostite.
//        /// </param>
//        /// <param name="cStatus">
//        /// podawa se poreferenciq t.kato nqma obekt kojto da dyrvi stojnostite.
//        /// </param>
//        private void AICheck(IPlayer player)
//        {
//            player.Label.Text = "Check";
//            player.Turn = false;
//            this.raising = false;
//        }

//        //// вика се от  AIHP AIPH, AISmooth this.AICall(ref botChips, ref bothTurn, sStatus);
//        private void AICall(IPlayer player)
//        {
//            this.raising = false;
//            player.Turn = false;
//            player.Chips -= this.pokerCall;
//            player.Label.Text = "Call " + this.pokerCall;
//            this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + this.pokerCall).ToString();
//        }

//        //// вика се от  AIHP AIPH, AISmooth this.AIRaised(ref botChips, ref bothTurn, sStatus);
//        private void AIRaised(IPlayer player)
//        {
//            player.Chips -= Convert.ToInt32(this.raise);
//            player.Label.Text = "Raise " + this.raise;
//            this.textBoxPot.Text = (int.Parse(this.textBoxPot.Text) + Convert.ToInt32(this.raise)).ToString();
//            this.pokerCall = Convert.ToInt32(this.raise);
//            this.raising = true;
//            player.Turn = false;
//        }

//        ////вика се от  AIHP AIPH, AISmooth  this.pokerCall <= AIRoundNumber(botChips, n) числото е различно има и математика
//        private static double AIRoundNumber(int botChips, int n)
//        {
//            double a = Math.Round((botChips / n) / 100d, 0) * 100;
//            return a;
//        }
//        #endregion

//    }
//}
