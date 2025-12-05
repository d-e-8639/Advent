using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Advent.Y2025
{
    public class Advent3
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent3.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<List<int>> ratings = lines.Select(l => l.Select(c => int.Parse(new string([ c]))).ToList()).ToList();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(ratings);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(ratings);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1(List<List<int>> ratings) {
            

            List<int> bestRatings = new List<int>();
            foreach (List<int> bank in ratings)
            {
                int bestRating=0;
                for (int i=0; i < bank.Count - 1; i++)
                {
                    for (int j=i+1; j < bank.Count; j++)
                    {
                        int testRating = (bank[i] * 10 + bank[j]);
                        if (testRating > bestRating)
                        {
                            bestRating = testRating;
                        }
                    }
                }
                bestRatings.Add(bestRating);
                Console.WriteLine(bestRating);
            }
            Console.WriteLine("sum: " + bestRatings.Sum());
        }

        private static void task2(List<List<int>> ratings) {
            //List<long> bestRatings = new List<long>();
            //foreach (List<int> bank in ratings) {
            //    bestRatings.Add(maxJolt(bank, 0, 0, 12));
            //    Console.WriteLine("Best: " + bestRatings.Last());
            //}
            //Console.WriteLine("sum: " + bestRatings.Sum());

            List<long> bestRatings = new List<long>();
            foreach (List<int> bank in ratings) {
                bestRatings.Add(maxJolt2(bank, 0, 12));
                Console.WriteLine("Best: " + bestRatings.Last());
            }
            Console.WriteLine("sum: " + bestRatings.Sum());
        }

        // This didn't work, obviously
        private static long maxJolt(List<int> bank, int startIndex, long currentNumber, int depth) {
            if (depth == 0) {
                return currentNumber;
            }

            long bestRating = 0;
            for (int i = startIndex; i < bank.Count - depth + 1; i++) {
                long testRating = maxJolt(bank, i + 1, currentNumber * 10 + (long)bank[i], depth - 1);
                bestRating = Math.Max(bestRating, testRating);
            }
            return bestRating;
        }

        private static long maxJolt2(List<int> bank, int startIndex, int depth) {
            if (depth == 0) {
                return 0;
            }

            for (int digit = 9; digit > 0; digit--) {
                for (int i = startIndex; i < bank.Count - depth + 1; i++) {
                    if (bank[i] == digit) {
                        return (digit * (long)Math.Pow(10, depth - 1)) +  maxJolt2(bank, i + 1, depth - 1);
                    }
                }
            }

            throw new Exception("wtf");
        }

    }
}
