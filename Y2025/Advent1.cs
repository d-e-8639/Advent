using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.lib;

namespace Advent.Y2025
{
    public class Advent1
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent1.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);


            List<int> rotations = new List<int>();
            foreach (string l in lines) {
                int n = int.Parse(l.Substring(1));
                if (l[0] == 'L') {
                    n = -n;
                }
                rotations.Add(n);
            }

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(rotations);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(rotations);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1(List<int> rotations) {
        }

        private static void task2(List<int> rotations) {
            int n = 50;
            Dial d = new Dial();

            int passwordCount = 0;
            foreach (int r in rotations) {
                int clicks = 0;

                if (r == -829) {
                    ;
                }

                if (n > 0 && (n + (r % 100)) < 0) {
                    clicks++;
                }

                if ((n > 0) && (r > 0) && (n+ AdventMath.Modulo(r, 100)) > 100) {
                    clicks++;
                }

                if (Math.Abs(r) / 100 > 0) {
                    clicks += Math.Abs(r) / 100;
                }

                int dialClicks = d.Turn(r);

                if (clicks != dialClicks) {
                    throw new Exception();
                }
                passwordCount += clicks;

                n = n + r;
                n = AdventMath.Modulo(n, 100);
                if (n == 0) {
                    passwordCount++;
                }
                ;
            }

            Console.WriteLine("password: " + passwordCount);
        }

        private class Dial
        {
            public int n=50;

            public int Turn(int t) {
                int clicks = 0;
                if (t > 0) {
                    for (int i=0; i < t; i++) {
                        n++;
                        n = AdventMath.Modulo(n, 100);
                        if (n == 0) {
                            clicks++;
                        }
                    }
                }
                else {
                    for (int i = 0; i > t; i--) {
                        n--;
                        n = AdventMath.Modulo(n, 100);
                        if (n == 0) {
                            clicks++;
                        }
                    }
                }
                if (n == 0) {
                    clicks--;
                }
                return clicks;
            }
        }

    }
}
