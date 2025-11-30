using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent.A2024
{
    public class Advent2
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent2.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<int>> reports = new List<List<int>>();

            foreach (string l in lines) {
                reports.Add(l.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => int.Parse(x))
                                .ToList());
            }

            task1(reports);
            task2(reports);
        }

        private static void task1(List<List<int>> reports) {
            Verifier v = new Verifier();

            int safe=0;
            foreach (List<int> report in reports) {
                if (v.Verify(report)) {
                    safe++;
                }
            }
            Console.Write("Safe: " + safe);
        }

        private static void task2(List<List<int>> reports) {
            Verifier v = new Verifier();

            int safe=0;
            foreach (List<int> report in reports) {

                for (int i=0; i < report.Count; i++) {
                    if (v.Verify(report.Where((x, index) => index != i))) {
                        safe++;
                        break;
                    }
                }


            }
            Console.Write("Safe - level: " + safe);
        }

        public class Verifier {
            public int MinDiff=1, MaxDiff=3;

            public bool Verify(IEnumerable<int> report) {

                return 
                    (runCheck(report, isGreater) || runCheck(report, isLesser))
                    && runCheck(report, isInRange);
            }

            private bool runCheck(IEnumerable<int> report, Check check) {
                using (IEnumerator<int> e = report.GetEnumerator()){
                    e.MoveNext();
                    int previous = e.Current;
                    while (e.MoveNext() != false) {
                        if (!check(previous, e.Current)) {
                            return false;
                        }
                        previous = e.Current;
                    }
                }
                return true;
            }

            private delegate bool Check (int a, int b);

            private bool isGreater(int a, int b) {
                return a > b;
            }

            private bool isLesser(int a, int b) {
                return a < b;
            }

            private bool isInRange(int a, int b) {
                int diff = Math.Abs(a - b);
                return (diff >= MinDiff) && (diff <= MaxDiff);
            }
        }

    }
}
