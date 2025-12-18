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

        private static void task1(List<Machine> machines)
        {

            // only press each button once!

            // foreach(Machine m in machines)
            // {
            //     bool[] indicators = Enumerable.Repeat(false, m.TargetIndicatorPattern.Length).ToArray();
            //     Stack<int> buttonSequence = new Stack<int>();
            //     List<int[]> successSequences = new List<int[]>();
            //     for (int presses=0; successSequences.Count == 0; presses++)
            //     {
            //         m.PressButtons(presses, indicators, buttonSequence, successSequences);
            //     }

            //     foreach (int[] successSeq in successSequences)
            //     {
            //         Console.WriteLine("Successful button sequence: " + string.Join(",", successSeq.Select(i => "(" + string.Join(",", m.Buttons[i].Select(j => j.ToString())) + ")" )));
            //     }
            // }
        }

        public class Machine
        {
            public bool[] TargetIndicatorPattern;
            //public bool[] Indicators;
            public List<List<int>> Buttons;
            public List<int> Joltage;

            private Machine() {}


            private void _flipBits(int ButtonNumber, bool[] indicators)
            {
                foreach (int n in Buttons[ButtonNumber])
                {
                    indicators[n] = !indicators[n];
                }
            }

            private void Press(int ButtonNumber, bool[] indicators, Stack<int> buttonSequence)
            {
                buttonSequence.Push(ButtonNumber);
                _flipBits(ButtonNumber, indicators);
            }

            private void UndoPress(int ButtonNumber, bool[] indicators, Stack<int> buttonSequence)
            {
                _flipBits(ButtonNumber, indicators);
                buttonSequence.Pop();
            }

            public void PressButtons(int presses, bool[] indicators, Stack<int> buttonSequence, List<int[]> successSequences)
            {
                if (presses == 0)
                {
                    if (TargetIndicatorPattern.SequenceEqual(indicators))
                    {
                        successSequences.Add(buttonSequence.ToArray());
                    }
                    return;
                }

                for (int i=0; i < Buttons.Count; i++)
                {
                    Press(i, indicators, buttonSequence);
                    PressButtons(presses - 1, indicators, buttonSequence, successSequences);
                    UndoPress(i, indicators, buttonSequence);
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
                        //m.Indicators = new bool[m.TargetIndicatorPattern.Length];
                    }
                    else if (part.StartsWith("("))
                    {
                        if (m.Buttons == null) {m.Buttons = new List<List<int>>();}
                        m.Buttons.Add(part.Trim(['(', ')']).Split(',').Select(i => int.Parse(i)).ToList());
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
