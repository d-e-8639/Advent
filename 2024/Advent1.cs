using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Advent.A2024
{
    public class Advent1
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent1.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<int> list1 = new List<int>();
            List<int> list2 = new List<int>();
            foreach (string l in lines) {
                string[] bits = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                list1.Add(int.Parse(bits[0]));
                list2.Add(int.Parse(bits[1]));
            }

            task1(list1, list2);
            task2(list1, list2);
        }

        private static void task1(List<int> list1, List<int> list2) {
            List<int> sList1 = list1.OrderBy(x => x).ToList();
            List<int> sList2 = list2.OrderBy(x => x).ToList();

            List<int> distance = new List<int>();
            for (int i=0; i < sList1.Count; i++) {
                distance.Add(Math.Abs(sList2[i] - sList1[i]));
            }

            Console.WriteLine("Distance sum: " + distance.Sum());
        }

        private static void task2(List<int> list1, List<int> list2) {
            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (int n in list2){
                if (! counts.ContainsKey(n)) {
                    counts.Add(n, 0);
                }
                counts[n] = counts[n] + 1;
            }

            int sum = 0;
            foreach (int n in list1) {
                sum += n * (counts.ContainsKey(n) ? counts[n] : 0);
            }

            Console.WriteLine("other dist: " + sum);
        }

    }
}
