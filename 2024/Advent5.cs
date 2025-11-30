using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace HelloWorld.A2024
{
    public class Advent5
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent5.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<Rule> rules = new List<Rule>();
            List<Manual> manuals = new List<Manual>();

            bool flip = false;
            for (int i=0; i < lines.Length; i++) {
                if (lines[i] == "") {
                    flip = true;
                }
                else if (! flip) {
                    rules.Add(new Rule(lines[i]));
                }
                else {
                    manuals.Add(new Manual(lines[i]));
                }
            }

            task1(rules, manuals);

            task2(rules, manuals);
        }

        private static void task1(List<Rule> rules, List<Manual> manuals) {
            Console.WriteLine("sum: " + manuals.Where(m => rules.All(r => r.Test(m))).Select(m => m.MiddleNumber()).Sum());
        }

        private static void task2(List<Rule> rules, List<Manual> manuals) {
            int sum = 0;
            List<Manual> failed = manuals.Where(m => !rules.All(r => r.Test(m))).ToList();
            foreach (Manual m in failed) {

                while(!rules.All(r => r.Test(m))) {
                    foreach (Rule r in rules.Where(r => !r.Test(m))) {
                        m.Fix(r);
                    }
                }
                sum += m.MiddleNumber();
            }
            Console.WriteLine("Sum 2 : " + sum);
        }

        public class Rule {
            public int A,B;
            public Rule (string line) {
                string[] parts = line.Split('|');
                A = int.Parse(parts[0]);
                B = int.Parse(parts[1]);
            }

            public bool Test(Manual m) {
                int i1 = m.IndexOf(A);
                int i2 = m.IndexOf(B);
                if (i1 >= 0 && i2 >= 0) {
                    return (i2 > i1);
                }
                return true;
            }

            public override string ToString()
            {
                return A + "|" + B;
            }
        }

        public class Manual {
            public List<int> Pages;
            private Dictionary<int, int> indicies;

            public Manual (string line) {
                Pages = line.Split(',').Select(i => int.Parse(i)).ToList();
                indicies = Pages.Select((n, i) => new Tuple<int, int>(n, i)).ToDictionary(n => n.Item1, n => n.Item2);
            }

            public int IndexOf(int n) {
                int i;
                if (indicies.TryGetValue(n, out i)){
                    return i;
                }
                return -1;
            }

            public int MiddleNumber() {
                return Pages[Pages.Count / 2];
            }

            public void Fix(Rule r) {
                int i1 = IndexOf(r.A);
                int i2 = IndexOf(r.B);
                Pages[i2] = r.A;
                Pages[i1] = r.B;
                indicies[r.A] = i2;
                indicies[r.B] = i1;
            }

            public override string ToString()
            {
                return string.Join(',', Pages.Select(i => i.ToString()));
            }
        }
    }
}
