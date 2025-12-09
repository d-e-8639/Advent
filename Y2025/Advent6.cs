using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.A2024;

namespace Advent.Y2025
{
    public class Advent6
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent6sample.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            long[][] numbers = lines.Take(lines.Length - 1).Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray()).ToArray();
            char[] ops = lines.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Single()).ToArray();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(numbers, ops);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(numbers, ops);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        static Func<long, long, long> mult = delegate (long a, long b)
        {
            return a * b;
        };

        private static void task1(long[][] numbers, char[] ops)
        {
            List<long> results = new List<long>();
            for (int i=0; i < numbers[0].Length; i++)
            {
                if (ops[i] == '+')
                {
                    results.Add(numbers.Select(Line => Line[i]).Sum());
                }
                else if (ops[i] == '*')
                {
                    results.Add(numbers.Select(Line => Line[i]).Aggregate((a, b) => a * b));
                    // numbers.Select(Line => Line[i]).Aggregate(delegate(long a, long b)
                    // {
                    //     return a * b;
                    // });
                    // 
                    //numbers.Select(Line => Line[i]).Aggregate(mult);
                }
                
            }
            Console.WriteLine("Result: " + results.Sum());
        }

        private static void task2(long[][] numbers, char[] ops) {
        }

    }
}
