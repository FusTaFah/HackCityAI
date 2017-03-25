using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AIGaming.Client;
using AIGaming.Core.Games;
using AIGaming.Core.Games.Helpers;
using AIGaming.Core.Helpers;

namespace TexasHoldEm.Client
{
    public class TexasHoldEm : ClientGameBase<TexasHoldEmClient, TexasHoldEmDesk, TexasHoldEmGameState, TexasHoldEmStartState>
    {
        readonly Random _random = new Random();

        public override GameType GameType
        {
            get { return GameType.TexasHoldEm; }
        }

        protected override void ProcessGameState(TexasHoldEmGameState state)
        {
            // Update display with latest game state
            Desk.ShowMove(state);

            if (state.IsMover)
            {
                CalculateMove(state);
            }
            else
            {
                Thread.Sleep(ThinkingTime);
            }
        }

        public override void CalculateMove(TexasHoldEmGameState state)
        {
            #region Helper Values
            // calculate min bet
            var minBet = (state.OpponentRoundBetTotal ?? 0) - (state.PlayerRoundBetTotal ?? 0); // If you try and bet less than minBet, that will be a Fold
            var maxBet = minBet + state.OpponentStack;
            // If you try and bet more than maxBet, that will be a bet of maxBet
            if (maxBet > state.PlayerStack) {
                maxBet = state.PlayerStack; 
            }

            var opponentId = Opponent.Name;
            var boardCards = state.BoardCards;
            var boardCardsCount = boardCards.Count;

            var playerHand = state.PlayerHand;

            var hole1 = playerHand[0];
            var hole2 = playerHand[1];
            var flop1 = boardCards.FirstOrDefault();
            var flop2 = boardCards.SecondOrDefault();
            var flop3 = boardCards.ThirdOrDefault();
            var turn = boardCards.FourthOrDefault();
            var river = boardCards.FifthOrDefault();

            var round = state.Round;
            var isOurFirstBetOfRound = round == TexasHoldEmRound.Preflop 
                ? state.PlayerRoundBetTotal <= state.BigBlind 
                : state.PlayerRoundBetTotal == 0;

            var bigBlind = state.BigBlind;
            var smallBlind = state.SmallBlind;
            var dealCount = state.DealCount;
            var dealNumber = state.DealNumber;
            var isDealer = state.IsDealer;
            var opponentRoundBetTotal = state.OpponentRoundBetTotal;
            var opponentStack = state.OpponentStack;
            var playerRoundBetTotal = state.PlayerRoundBetTotal;
            var playerStack = state.PlayerStack;
            var potAfterPreviousRound = state.PotAfterPreviousRound;
            var responseDeadline = state.ResponseDeadline;
            #endregion
            
            #region Helper Methods
            #region Random Helpers
            // next line of code will return 1 upto 10 including 10 value
            var random10 = RangeRand(1, 10);

            // next line of code will return from 1 upto 219 with extra math 
            // for example we get random value (200) from 1 to 219
            // then we perform next math: 200 - 200 % 20
            var random219 = RangeRand(1, 219, 20);
            #endregion

            #region IsPicture Helpers
            var isPictureHole1 = hole1.IsPictureOrAce();
            var isPictureHole2 = hole2.IsPictureOrAce();
            var isPictureOrTenHole1 = hole1.IsPictureOrAceOrTen();
            var isPictureOrTenHole2 = hole2.IsPictureOrAceOrTen();
            #endregion

            #region IsPair Helpers
            // IsPair could check a list of cards with any number of cards
            var isPairPlayerHand = playerHand.IsPair();
            // IsPair could take any number of parameters
            var isPair = hole1.IsPair(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsTwoPair Helpers
            // IsTwoPair could check a list of cards with any number of cards
            var isTwoPairBoardCards = boardCards.IsTwoPair();
            // IsTwoPair could take any number of parameters
            var isTwoPair = hole1.IsTwoPair(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsThreeOfAKind Helpers
            // IsThreeOfAKind could check a list of cards with any number of cards
            var isThreeOfAKindBoardCards = boardCards.IsThreeOfAKind();
            // IsThreeOfAKind could take any number of parameters
            var isThreeOfAKind = hole1.IsThreeOfAKind(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsStraight Helpers
            // IsStraight could check a list of cards with any number of cards
            var isStraightBoardCards = boardCards.IsStraight();
            // IsStraight could take any number of parameters
            var isStraight = hole1.IsStraight(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsFlush Helpers
            // IsFlush could check a list of cards with any number of cards
            var isFlushBoardCards = boardCards.IsFlush();
            // IsFlush could take any number of parameters
            var isFlush = hole1.IsFlush(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsFullHouse Helpers
            // IsFullHouse could check a list of cards with any number of cards
            var isFullHouseBoardCards = boardCards.IsFullHouse();
            // IsFullHouse could take any number of parameters
            var isFullHouse = hole1.IsFullHouse(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsFourOfAKind Helpers
            // IsFourOfAKind could check a list of cards with any number of cards
            var isFourOfAKindBoardCards = boardCards.IsFourOfAKind();
            // IsFourOfAKind could take any number of parameters
            var isFourOfAKind = hole1.IsFourOfAKind(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsStraightFlush Helpers
            // IsStraightFlush could check a list of cards with any number of cards
            var isStraightFlushBoardCards = boardCards.IsStraightFlush();
            // IsStraightFlush could take any number of parameters
            var isStraightFlush = hole1.IsStraightFlush(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsFourCardFlush Helpers
            // IsFourCardFlush could check a list of cards with any number of cards
            var isFourCardFlushBoardCards = boardCards.IsFourCardFlush();
            // IsFourCardFlush could take any number of parameters
            var isFourCardFlush = hole1.IsFourCardFlush(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsFourCardStraight Helpers
            // IsFourCardStraight could check a list of cards with any number of cards
            var isFourCardStraightBoardCards = boardCards.IsFourCardStraight();
            // IsFourCardStraight could take any number of parameters
            var isFourCardStraight = hole1.IsFourCardStraight(hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region IsSuitedConnector Helpers
            bool isSuitedConnector;
            // IsSuitedConnector checks a list of EXACTLY 2 cards
            isSuitedConnector = playerHand.IsSuitedConnector();
            isSuitedConnector = hole1.IsSuitedConnector(hole2);
            #endregion

            #region IsHiddenPair Helpers
            int pairRankPlayerHand;
            // IsHiddenPair could check a list of cards with any number of cards
            playerHand.IsHiddenPair(boardCards, out pairRankPlayerHand);

            int pairRank;
            // IsHiddenPair could take any number of parameters
            hole1.IsHiddenPair(out pairRank, hole2, flop1, flop2, flop3, turn, river);
            #endregion

            #region ToShorthandString Helpers
            string shorthand;
            // you can use list or card method to get shorthand string
            // see next examples:
            shorthand = playerHand.ToShorthandString();
            shorthand = hole1.ToShorthandString(hole2);
            #endregion

            #region HoleRank Helpers
            int holeRank;
            // HoleRank checks a list of EXACTLY 2 cards
            holeRank = playerHand.HoleRank();
            holeRank = hole1.HoleRank(hole2);
            #endregion

            #region HandRank Helpers
            // HandRank could check a list of cards with AT LEAST 5 cards
            var boardCardsRank = boardCards.HandRank();
            // Or you can call card method with AT LEAST 4 parameters
            var handRank = hole1.HandRank(hole2, flop1, flop2, flop3);

            // HandRankDesc returns English description of the hand
            string handRankDescription = handRank.HandRankDesc();
            #endregion
            #endregion
            
            // Wait time to allow user to see move.
            // This can be removed or thinking time set to 0 on client interface to play at full speed
            Thread.Sleep(ThinkingTime);

            // create move request
            var move = new TexasHoldEmMove();

            ChanceOfWinning(hole1, hole2, new List<Card>(boardCards).ToList());

            // Bet the minimum and one in three times raise by 0, 10, 20, 30 or 40
            //move.BetSize = minBet + (_random.Next(3) == 0 ? 10 : 0) * _random.Next(5);
            //move.BetSize = minBet;
            //if (isOurFirstBetOfRound)
            //{
            //    move.BetSize = 0;
            //}else
            //{
            //    List<Card> cards = state.BoardCards; //.Concat(playerHand);
            //    foreach (Card c in playerHand) cards.Add(c);
            //    List<bool> confidenceQualifiers = new List<bool>();
            //    if (isPair) confidenceQualifiers.Add(isPair);
            //    if (isTwoPair) confidenceQualifiers.Add(isTwoPair);
            //    if (isThreeOfAKind) confidenceQualifiers.Add(isThreeOfAKind);
            //    if (isStraight) confidenceQualifiers.Add(isStraight);
            //    if (isFlush) confidenceQualifiers.Add(isFlush);
            //    if (isFullHouse) confidenceQualifiers.Add(isFullHouse);
            //    if (isFourOfAKind) confidenceQualifiers.Add(isFourOfAKind);
            //    if (isFourCardFlush) confidenceQualifiers.Add(isFourCardFlush);
            //    if (isFourCardStraight) confidenceQualifiers.Add(isFourCardStraight);
            //    if(confidenceQualifiers.Count < 3)
            //    {
            //        move.Fold = true;
            //    }
            //    //FUCK OFF INTELLISENSE
            //    int confidence = (int)((float)confidenceQualifiers.Count * ((float)playerStack / 1000.0f));

            //    move.BetSize = minBet + confidence /*+ _random.Next(confidence / 2)*/;
            //}

            // call server to process our move
            ClientMoved(move);
        }

        private float ChanceOfWinning(Card hole1, Card hole2, List<Card> boardCards)
        {
            List<Card> cardsInDeck = new List<Card>();
            //populate all cards

            //populate suits
            for(int i = 1; i <= 4; i++)
            {
                //populate indexes
                for(int j = 2; j <= 14; j++)
                {
                    Card temp = new Card();
                    temp.Index = (CardIndexType)j;
                    temp.Suit = (CardSuitType)i;
                    cardsInDeck.Add(temp);
                }
            }

            boardCards.Add(hole1);
            boardCards.Add(hole2);

            cardsInDeck.RemoveAll(x => { return x.Suit == hole1.Suit && x.Index == hole1.Index;});
            cardsInDeck.RemoveAll(x => { return x.Suit == hole2.Suit && x.Index == hole2.Index; });
            foreach(Card c in boardCards) cardsInDeck.RemoveAll(x => { return x.Suit == c.Suit && x.Index == c.Index; });

            

            //int currentHandRank = hole1.HandRank(boardCards.ToArray());

            foreach (Card c in cardsInDeck) ;
            return 0.0f;
        }

        //public List<float> possibilityOfRank(List<Card> knownCards, List<Card> unknownCards, int roundsLeft)
        //{
            
        //    for(int i = 0; i < unknownCards.Count; i++)
        //    {
                
        //    }
        //    return 0.0f;
        //}

        #region Helper Methods
        private int RangeRand(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        private int RangeRand(int min, int max, int roundTo)
        {
            int res = _random.Next(min, max + 1);
            return (res - res % roundTo);
        }
        #endregion
    }
}
