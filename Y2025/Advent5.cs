using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.A2024;
using System.Collections.Frozen;

namespace Advent.Y2025
{
    public class Advent5
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent5sample.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<string>> sLines = lines.SplitBy("", true, false).ToList();

            List<int[]> freshIds = sLines[0].Select(l => l.Split('-').Select(i => int.Parse(i)).ToArray()).ToList();
            List<int> ingredients = sLines[1].Select(l => int.Parse(l)).ToList();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(freshIds, ingredients);
            st1.Stop();

            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(freshIds, ingredients);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1(List<int[]> freshIds, List<int> ingredients)
        {
            HashSet<int> freshCount = new HashSet<int>();
            foreach(int ingredient in ingredients)
            {
                foreach (int[] freshIdRange in freshIds)
                {
                    if (ingredient >= freshIdRange[0] && ingredient <= freshIdRange[1])
                    {
                        freshCount.Add(ingredient);
                    }
                }
            }
            Console.WriteLine("Count: " + freshCount.Count());
        }

        private static void task2(List<int[]> freshIds, List<int> ingredients) {
        }

    }
}
