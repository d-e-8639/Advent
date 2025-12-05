using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent.Y2025
{
    public class Advent3
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent3sample.txt")) {
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
        }

    }
}
