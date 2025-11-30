using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent
{

    public class Advent7
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent7.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            task1(lines);
            task2(lines);
        }

        private static void task1(string[] lines) {
            List<Tuple<Hand, long>> hands = 
                lines.Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(spl => new Tuple<Hand, long>(new Hand(spl[0]), long.Parse(spl[1])))
                .OrderBy(handWithBet => handWithBet.Item1)
                .ToList();
            
            long totalWin = 0;
            for (int i=0; i < hands.Count; i++) {
                totalWin += (i + 1) * hands[i].Item2;
            }
            Console.WriteLine("Total winnings: " + totalWin);
        }

        private static void task2(string[] lines) {
            List<Tuple<WildCardHand, long>> hands = 
                lines.Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(spl => new Tuple<WildCardHand, long>(new WildCardHand(spl[0], 'J'), long.Parse(spl[1])))
                .OrderBy(handWithBet => handWithBet.Item1)
                .ToList();

            long totalWin = 0;
            for (int i=0; i < hands.Count; i++) {
                totalWin += (i + 1) * hands[i].Item2;
            }
            Console.WriteLine("Wildcard total winnings: " + totalWin);

            hands[0].Item1.CompareTo(hands[1].Item1);
        }

        private enum HandType {
            FiveOfAKind = 7,
            FourOfAKind = 6,
            FullHouse = 5,
            ThreeOfAKind = 4,
            TwoPair = 3,
            OnePair = 2,
            HighCard = 1
        }

        private enum Card {
            cA = 13,
            cK = 12,
            cQ = 11,
            cJ = 10,
            cT = 9,
            c9 = 8,
            c8 = 7,
            c7 = 6,
            c6 = 5,
            c5 = 4,
            c4 = 3,
            c3 = 2,
            c2 = 1,
            cw = 0 // wildcard
        }

        private class Hand : IComparable<Hand> {
            protected Card[] cards;
            public readonly HandType type;

            public Hand(string cards){
                this.cards = cards.ToCharArray().Select(parseCard).ToArray();
                type = findCardType();
            }

            public Hand(IEnumerable<Card> cards){
                this.cards = cards.ToArray();
                type = findCardType();
            }


            private HandType findCardType() {
                Dictionary<char, int> counts = new Dictionary<char, int>();
                foreach (char c in cards) {
                    if (!counts.ContainsKey(c)) {
                        counts[c] = 0;
                    }
                    counts[c] = counts[c] + 1;
                }

                if (counts.Any(count => count.Value == 5)) {
                    return HandType.FiveOfAKind;
                }
                else if (counts.Any(count => count.Value == 4)) {
                    return HandType.FourOfAKind;
                }
                else if (counts.Any(count => count.Value == 3) && counts.Any(count => count.Value == 2)) {
                    return HandType.FullHouse;
                }
                else if (counts.Any(count => count.Value == 3) && (counts.Count(count => count.Value == 1) == 2)) {
                    return HandType.ThreeOfAKind;
                }
                else if (counts.Count(count => count.Value == 2) == 2) {
                    return HandType.TwoPair;
                }
                else if ((counts.Count(count => count.Value == 2) == 1) && (counts.Count(count => count.Value == 1) == 3)) {
                    return HandType.OnePair;
                }
                else if (counts.Count(count => count.Value == 1) == 5) {
                    return HandType.HighCard;
                }
                else {
                    throw new Exception("wtf");
                }
            }

            protected static Dictionary<char, Card> charToCardMap = new Dictionary<char, Card> {
                {'A', Card.cA},
                {'K', Card.cK},
                {'Q', Card.cQ},
                {'J', Card.cJ},
                {'T', Card.cT},
                {'9', Card.c9},
                {'8', Card.c8},
                {'7', Card.c7},
                {'6', Card.c6},
                {'5', Card.c5},
                {'4', Card.c4},
                {'3', Card.c3},
                {'2', Card.c2}};
            protected Card parseCard (char c) {
                return charToCardMap[c];
            }

            protected static Dictionary<Card, char> cardToCharMap = new Dictionary<Card, char> {
                {Card.cA, 'A'},
                {Card.cK, 'K'},
                {Card.cQ, 'Q'},
                {Card.cJ, 'J'},
                {Card.cT, 'T'},
                {Card.c9, '9'},
                {Card.c8, '8'},
                {Card.c7, '7'},
                {Card.c6, '6'},
                {Card.c5, '5'},
                {Card.c4, '4'},
                {Card.c3, '3'},
                {Card.c2, '2'},
                {Card.cw, 'J'}};
            public override string ToString()
            {
                return string.Concat(cards.Select(c => cardToCharMap[c]));
            }

            public int CompareTo(Hand other)
            {
                if (other == null) {
                    return 1;
                }

                int typeCompare = (int)(this.type) - (int)(other.type);
                if (typeCompare != 0) {
                    return typeCompare;
                }
                
                for (int i=0; i < cards.Length; i++) {
                    if (this.cards[i] != other.cards[i]) {
                        return (int)(this.cards[i]) - (int)(other.cards[i]);
                    }
                }

                return 0;
            }
        }

        private class WildCardHand : Hand, IComparable<WildCardHand> {
            private Card[] wildCards;
            private Hand bestHand;

            public WildCardHand(string cards, char wildCard)
                : base(cards){
                this.wildCards = cards.ToCharArray().Select(c => parseCard(c, wildCard)).ToArray();

                CalculateBestHand();
            }


            private Card parseCard (char c, char wildCardChar) {
                if (c == wildCardChar) {
                    return Card.cw;
                }
                return charToCardMap[c];
            }

            public void CalculateBestHand(){
                bestHand = bestFillWildcards(new LinkedList<Card>(wildCards).First, new Stack<Card>(), null);
            }

            private static Card[] nonWildcards = new Card[]{Card.cA,Card.cK,Card.cQ,Card.cT,Card.c9,Card.c8,Card.c7,Card.c6,Card.c5,Card.c4,Card.c3,Card.c2};

            private static Hand bestFillWildcards(LinkedListNode<Card> currentCard, Stack<Card> hand, Hand bestSoFar){
                if (hand.Count == 5) {
                    Hand possible = new Hand(hand.Reverse());
                    if (possible.CompareTo(bestSoFar) > 0){
                        return possible;
                    }
                    else {
                        return bestSoFar;
                    }
                }

                Hand _bestSoFar = bestSoFar;
                if (currentCard.Value == Card.cw) {
                    foreach(Card c in nonWildcards){
                        hand.Push(c);
                        Hand subBest = bestFillWildcards(currentCard.Next, hand, _bestSoFar);
                        if (subBest.CompareTo(_bestSoFar) > 0){
                            _bestSoFar = subBest;
                        }
                        hand.Pop();
                    }
                }
                else {
                    hand.Push(currentCard.Value);
                    _bestSoFar = bestFillWildcards(currentCard.Next, hand, _bestSoFar);
                    hand.Pop();
                }

                return _bestSoFar;
            }

            public int CompareTo(WildCardHand other)
            {
                if (other == null) {
                    return 1;
                }

                int typeCompare = (int)(this.bestHand.type) - (int)(other.bestHand.type);
                if (typeCompare != 0) {
                    return typeCompare;
                }
                
                for (int i=0; i < wildCards.Length; i++) {
                    if (this.wildCards[i] != other.wildCards[i]) {
                        return (int)(this.wildCards[i]) - (int)(other.wildCards[i]);
                    }
                }

                return 0;
            }

        }
    }
}

