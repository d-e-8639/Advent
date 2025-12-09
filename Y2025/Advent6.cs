using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.A2024;
using System.Text;

namespace Advent.Y2025
{
    public class Advent6
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent6.txt")) {
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
            task2(lines);
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

        private static void task2(string[] lines) {
            List<string[]> chunks = new List<string[]>();

            int lastSplit = 0;
            for(int i=0; i < lines[0].Length; i++) {
                if (lines.All(l => l[i] == ' ')) {
                    chunks.Add(lines.Select(l => l.Substring(lastSplit, i - lastSplit)).ToArray());
                    lastSplit = i + 1;
                }
            }
            chunks.Add(lines.Select(l => l.Substring(lastSplit)).ToArray());

            List<long> results = new List<long>();
            foreach (string[] prob in chunks) {
                List<long> n = new List<long>();
                for (int i= prob[0].Length -1; i >= 0 ; i--) {
                    StringBuilder sb = new StringBuilder();
                    n.Add(long.Parse(new string (prob.Take(prob.Length - 1).Select(x => x[i]).ToArray())));
                }
                if (prob.Last().Trim() == "*") {
                    results.Add(n.Aggregate((a, b) => a * b));
                }
                else {
                    results.Add(n.Sum());
                }
            }

            Console.WriteLine("Grand total: " + results.Sum());
        }

    }
}
