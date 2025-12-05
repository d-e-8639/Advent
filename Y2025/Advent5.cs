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
            using (StreamReader sr = new StreamReader(wd + "Advent5.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<string>> sLines = lines.SplitBy("", true, false).ToList();

            List<long[]> freshIds = sLines[0].Select(l => l.Split('-').Select(i => long.Parse(i)).ToArray()).ToList();
            List<long> ingredients = sLines[1].Select(l => long.Parse(l)).ToList();

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

        private static void task1(List<long[]> freshIds, List<long> ingredients)
        {
            HashSet<long> freshCount = new HashSet<long>();
            foreach(long ingredient in ingredients)
            {
                foreach (long[] freshIdRange in freshIds)
                {
                    if (ingredient >= freshIdRange[0] && ingredient <= freshIdRange[1])
                    {
                        freshCount.Add(ingredient);
                    }
                }
            }
            Console.WriteLine("Count: " + freshCount.Count());
        }

        private static void task2(List<long[]> freshIds, List<long> ingredients) {
            LinkedList<long[]> mergeList = new LinkedList<long[]>(freshIds.OrderBy(x => x[0]));

            for (LinkedListNode<long[]> i = mergeList.First; i != mergeList.Last && i != null; i = i.Next) {
                bool removed=true;
                while (removed) {
                    removed = false;
                    for (LinkedListNode<long[]> j = i.Next; j != null; j = j.Next) {
                        if (i.Value[1] >= j.Value[0]) {
                            // ranges overlap
                            i.Value[1] = Math.Max(i.Value[1], j.Value[1]);
                            mergeList.Remove(j);
                            removed = true;
                            break;
                        }
                    }
                }
            }

            Console.WriteLine(string.Join("\r\n", mergeList.Select(range => range[0].ToString() + "-" + range[1].ToString())));

            Console.WriteLine("all fresh id count:" + mergeList.Select(range => range[1] - range[0] + 1).Sum());
        }

    }
}
