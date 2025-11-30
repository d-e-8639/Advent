using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Text;

namespace Advent.A2024
{
    public class Advent11
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent11.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            long[] numbers = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(n => long.Parse(n)).ToArray();

            LinkedList<Stone> ls = new LinkedList<Stone>();
            foreach (long n in numbers) { new Stone(n, ls); }

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(ls);
            st1.Stop();

            LinkedList<Stone> ls2 = new LinkedList<Stone>();
            foreach (long n in numbers) { new Stone(n, ls2); }

            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(ls2);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );
        }

        private static void task1(LinkedList<Stone> ls) {
            for (int i=0; i < 25; i++) {
                foreach (Stone s in ls.ToArray()) { s.UpdateBlink(); };
            }
            //Console.WriteLine(ToString(ls));
            // 228668
            Console.WriteLine("Count: " + ls.Count);
        }

        private static void task2(LinkedList<Stone> ls) {
            // for (int i=0; i < 25; i++) {
            //     foreach (Stone s in ls.ToArray()) { s.UpdateBlink(); };
            // }
            // long uniq = ls.Select(x => x.Number).Distinct().Count();
            // //Console.WriteLine(ToString(ls));
            // //Console.WriteLine("Count: " + ls.Count);

            long sum = 0;
            Dictionary<long, StoneTree> map = new Dictionary<long, StoneTree>();
            foreach (Stone s in ls) {
                StoneTree.PopulateMap(s.Number, map, 75);
                sum += map[s.Number].CalculateDepth(75, map);
            }

            // StringBuilder sb = new StringBuilder();
            // foreach (KeyValuePair<long, StoneTree> kv in map) {
            //     sb.AppendLine(string.Join(',', kv.Value.CountAtDepth.Select(x => x.ToString())));
            // }
            // Console.WriteLine(sb.ToString());

            Console.WriteLine("Count: " + sum);
        }

        public class StoneTree {
            public Stone Stone;
            public long[] CountAtDepth;

            public List<StoneTree> Children;
            //private int maxDepth;

            private StoneTree(long n, int maxDepth) {
                Stone = new Stone(n);
                //this.maxDepth = maxDepth;
                CountAtDepth = Enumerable.Repeat<long>(-1, maxDepth + 1).ToArray();
                CountAtDepth[0] = 1;
                Children = new List<StoneTree>();
            }

            public static StoneTree PopulateMap(long n, Dictionary<long, StoneTree> map, int maxDepth) {
                
                if (map.ContainsKey(n)) {
                    return map[n];
                }

                StoneTree st = new StoneTree(n, maxDepth);
                map.Add(n, st);

                long c1, c2;
                st.Stone.Blink(out c1, out c2);

                st.Children.Add(PopulateMap(c1, map, maxDepth));
                if (c2 == -1) {
                    st.CountAtDepth[1] = 1;
                }
                if (c2 >= 0) {
                    st.Children.Add(PopulateMap(c2, map, maxDepth));
                    st.CountAtDepth[1] = 2;
                }

                return st;
            }

            public long CalculateDepth(int depth, Dictionary<long, StoneTree> map) {
                if (CountAtDepth[depth] != -1) {
                    return CountAtDepth[depth];
                }

                long sum=0;
                foreach (StoneTree st in Children) {
                    sum += st.CalculateDepth(depth - 1, map);
                }

                CountAtDepth[depth] = sum;
                return sum;
            }
        }

        public static string ToString(LinkedList<Stone> ls) {
            return string.Join(',', ls.Select(s => s.ToString()));
        }

        public class Stone {
            public long Number;
            private LinkedList<Stone> LnkLst;
            private LinkedListNode<Stone> Node;

            public Stone(long n, LinkedList<Stone> lst) {
                Number = n;
                LnkLst = lst;
                Node = lst.AddLast(this);
                //Node = new LinkedListNode<Stone>(this);
            }

            public Stone(long n) {
                Number = n;
            }

            public void UpdateBlink() {
                long n, extra;
                Blink(out n, out extra);
                Number = n;
                if (extra >= 0) {
                    Stone nuStone = new Stone(extra);
                    nuStone.LnkLst = LnkLst;
                    nuStone.Node = LnkLst.AddAfter(Node, nuStone);

                }
            }

            public void Blink(out long s1, out long s2) {
                if (Number == 0) {
                    //If the stone is engraved with the number 0, it is replaced by a stone engraved with the number 1.
                    s1 = 1;
                    s2 = -1;
                    return;
                }

                double digits = Math.Floor(Math.Log10(Number)) + 1;
                if (digits >= 2 && (digits % 2 == 0)) {
                    long divisor = ((long) Math.Pow(10, digits / 2));

                    s1 = Number / divisor;
                    s2 = Number % divisor;
                    return;
                    //If the stone is engraved with a number that has an even number of digits, it is replaced by two stones. The left half of the digits are engraved on the new left stone, and the right half of the digits are engraved on the new right stone. (The new numbers don't keep extra leading zeroes: 1000 would become stones 10 and 0.)
                }

                s1 = this.Number * 2024;
                s2 = -1;
                //If none of the other rules apply, the stone is replaced by a new stone; the old stone's number multiplied by 2024 is engraved on the new stone.
            }

            public override string ToString()
            {
                return Number.ToString();
            }
        }

    }
}
