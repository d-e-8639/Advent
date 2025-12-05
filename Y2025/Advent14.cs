using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent.Y2025
{
    public class Advent14
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent14sample.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1();
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2();
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1() {
        }

        private static void task2() {
        }

    }
}
