using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent.A2024
{
    public class Advent3
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent3.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            task1(lines);
        }

        private static void task1(string[] lines) {
            List<string> ops = new List<string>();
            foreach (string l in lines) {
                MatchCollection matches = Regex.Matches(l, "mul\\([0-9]{1,3},[0-9]{1,3}\\)|do\\(\\)|don't\\(\\)");
                ops.AddRange(matches.Select(m => m.Value));
            }

            long sum = 0;
            bool dooo = true;
            foreach (string s in ops) {
                if (s == "do()") {
                    dooo = true;
                }
                else if (s == "don't()") {
                    dooo = false;
                }
                else if (dooo) {
                    sum += MulOp.FromString(s).Result();
                }
            }

            Console.WriteLine("Result: " + sum);
            //Console.WriteLine("Result: " + ops.Select(o => MulOp.FromString(o)).Select(mOp => mOp.Result()).Sum());
            
        }

        private static void task2() {
        }

        public class MulOp {
            string oper;
            int op1, op2;

            public MulOp(string oper, int op1, int op2) {
                this.oper = oper;
                this.op1 = op1;
                this.op2 = op2;
            }

            public static MulOp FromString(string s) {
                string[] parts = s.Split(['(', ',',')'], StringSplitOptions.RemoveEmptyEntries);
                return new MulOp(parts[0], int.Parse(parts[1]), int.Parse(parts[2]));
            }

            public int Result() {
                return op1 * op2;
            }
        }

    }
}
