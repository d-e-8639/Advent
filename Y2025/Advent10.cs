using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using Advent.lib;

namespace Advent.Y2025
{
    public class Advent10
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent10sample.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<Machine> machines = lines.Select(Machine.FromString).ToList();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(machines);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2();
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1(List<Machine> machines) {
        }

        public class Machine
        {
            public bool[] TargetIndicatorPattern;
            public bool[] Indicators;
            public List<List<int>> Wiring;
            public List<int> Joltage;

            private Machine() {}

            public void Press(int ButtonNumber)
            {
                foreach (int n in Wiring[ButtonNumber])
                {
                    
                }
            }

            public static Machine FromString(string s)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return null;
                }
                string[] parts = s.Split(' ');
                Machine m = new Machine();
                foreach (string part in parts)
                {
                    if (part.StartsWith("["))
                    {
                        m.TargetIndicatorPattern = part.Trim(['[', ']']).Select(HardCodedConverter.Factory(('.', false), ('#', true))).ToArray();
                        m.Indicators = new bool[m.TargetIndicatorPattern.Length];
                    }
                    else if (part.StartsWith("("))
                    {
                        if (m.Wiring == null) {m.Wiring = new List<List<int>>();}
                        m.Wiring.Add(part.Trim(['(', ')']).Split(',').Select(i => int.Parse(i)).ToList());
                    }
                    else if (part.StartsWith("{"))
                    {
                        m.Joltage = part.Trim(['{', '}']).Split(',').Select(i => int.Parse(i)).ToList();
                    }
                    else
                    {
                        throw new Exception("wut");
                    }
                }
                return m;
            }
        }

        private static void task2() {
        }

    }
}
