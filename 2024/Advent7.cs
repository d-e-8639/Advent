using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;

namespace HelloWorld.A2024
{
    public class Advent7
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent7.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<Equation> eqs = lines.Select(l => Equation.FromString(l)).ToList();

            task1(eqs);
            task2(eqs);
        }

        private static void task1(List<Equation> eqs) {
            long sum=0;

            foreach (Equation e in eqs) {
                if (e.Solve1()) {
                    sum += e.Result;
                }
            }

            Console.WriteLine(sum);
            
        }

        private static void task2(List<Equation> eqs) {
            long sum=0;

            foreach (Equation e in eqs) {
                if (e.Solve2()) {
                    sum += e.Result;
                }
            }

            Console.WriteLine("with concat: " + sum);
        }


        private class Equation {
            public long Result;
            public long[] Parts;

            public static Equation FromString(string s) {
                string[] res = s.Split(':', StringSplitOptions.RemoveEmptyEntries);
                Equation eq = new Equation();
                eq.Result = long.Parse(res[0]);

                string[] parts = res[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                eq.Parts = parts.Select(n => long.Parse(n)).ToArray();

                return eq;
            }


            public bool Solve1 () {
                return solve1(Parts[0], 1);
            }

            private bool solve1 (long current, int index) {
                if (index == Parts.Length) {
                    if (Result == current) {
                        return true;
                    }
                    return false;
                }

                if (solve1(current * Parts[index], index + 1)) {
                    return true;
                }

                if (solve1(current + Parts[index], index + 1)) {
                    return true;
                }

                return false;
            }

            public bool Solve2 () {
                return solve2(Parts[0], 1);
            }

            private bool solve2 (long current, int index) {
                if (index == Parts.Length) {
                    if (Result == current) {
                        return true;
                    }
                    return false;
                }

                if (solve2(current * Parts[index], index + 1)) {
                    return true;
                }

                if (solve2(current + Parts[index], index + 1)) {
                    return true;
                }

                long exp = ((long) Math.Log10(Parts[index])) + 1;
                long concat =  ((long)Math.Pow(10, exp) * current) + Parts[index];

                if (concat != long.Parse(current.ToString() + Parts[index].ToString())) {
                    throw new Exception();
                }

                if (solve2( long.Parse(current.ToString() + Parts[index].ToString()) , index + 1)) {
                    return true;
                }
                // 424261660171557
                // 424977609625985

                return false;
            }
        }
    }
}
