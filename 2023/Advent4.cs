using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HelloWorld
{
    public class Advent4
    {
        public void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent4.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r", "\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            List<Card> cards = new List<Card>();
            foreach (string line in lines){
                cards.Add(Card.FromString(line));
            }

            task1(cards);
            task2(cards);
        }

        public void task1(List<Card> cards){
            int points=0;
            foreach (Card card in cards){
                int win=0;
                foreach (int n in card.Have) {
                    if (card.Winning.Contains(n)) {
                        win++;
                    }
                }
                if (win > 0){
                    points += 1 << (win - 1);
                }
            }

            Console.WriteLine("total points: " + points);
        }

        public void task2(List<Card> cards) {
            for (int i=0; i < cards.Count; i++) {
                Card c = cards[i];
                int win=0;
                foreach (int n in c.Have) {
                    if (c.Winning.Contains(n)) {
                        win++;
                    }
                }
                for (int j = i+1; (j - i <= win) && j < cards.Count; j++) {
                    cards[j].copies += cards[i].copies;
                }
            }
            Console.WriteLine("total cards: " + cards.Sum(c => c.copies));
        }

        public class Card {
            public int Number;
            public HashSet<int> Winning;
            public HashSet<int> Have;
            public long copies=1;

            private Card() {}

            public static Card FromString(string line){
                Regex r =  new Regex("Card +([0-9]+):([0-9 ]+)\\|([0-9 ]+)$");

                Match m = r.Match(line);
                string cardN = m.Groups[1].Value;
                string winningStr = m.Groups[2].Value;
                string haveStr = m.Groups[3].Value;

                Card c = new Card();
                c.Number = int.Parse(cardN);
                c.Winning = new HashSet<int>(winningStr.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)));
                c.Have = new HashSet<int>(haveStr.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)));

                return c;
            }

        }
    }
}
