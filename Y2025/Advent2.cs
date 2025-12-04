using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.A2024;
using System.Runtime.ConstrainedExecution;

namespace Advent.Y2025
{
    public class Advent2
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent2.txt")) {
                file = sr.ReadToEnd();
            }
            //string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);
            string[] prodIds = string.Concat(file.Where(c => c != '\r' && c != '\n')).Split(',', StringSplitOptions.RemoveEmptyEntries);

            List<long[]> ranges = prodIds.Select(range => range.Split('-').Select(i => long.Parse(i)).ToArray()).ToList();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(ranges);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(ranges);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1(List<long[]> ranges) {
            long largest = ranges.Max(r => r.Max()) + 1;

            List<long> invalids = new List<long>();
            for (long i=1;
                i * (long)Math.Pow(10, ((long)Math.Log10(i) + 1)) + i <= largest;
                i++) {
                invalids.Add(
                    i * (long)Math.Pow(10, ((long)Math.Log10(i) + 1))
                    + i
                );
            }

            List<long> selectedInvalids = new List<long>();
            foreach (long[] range in ranges ) {
                int i=0;
                for (; i < invalids.Count && invalids[i] < range[0]; i++) {

                }
                int startRng = i;

                for (; i < invalids.Count && invalids[i] <= range[1] ; i++) {

                }
                int endRng = i;
                List<long> selectBitOfRange = invalids.Slice(startRng, endRng - startRng);
                selectedInvalids.AddRange(selectBitOfRange);
            }

            Console.WriteLine("Sum of invalids: " + selectedInvalids.Sum());
        }

        private static long repeated(long n, int times) {
            long ret = n;

            long base10offset = (long)Math.Pow(10, ((long)Math.Log10(n) + 1));

            for (int i=1; i < times; i++) {
                ret = ret * base10offset + n;
            }

            return ret;
        }

        private static void task2(List<long[]> ranges) {
            long largest = ranges.Max(r => r.Max()) + 1;

            List<long> invalids = new List<long>();
            for (long i = 1;
                repeated(i, 2) <= largest;
                i++) {

                for (int times = 2; repeated(i, times) <= largest; times++) {
                    invalids.Add(repeated(i, times));
                }

                //for (int n=2; )
                //invalids.Add(
                //    i * (long)Math.Pow(10, ((long)Math.Log10(i) + 1))
                //    + i
                //);


            }

            invalids = invalids.OrderBy(i => i).Distinct().ToList();

            List<long> selectedInvalids = new List<long>();
            foreach (long[] range in ranges) {
                int i = 0;
                for (; i < invalids.Count && invalids[i] < range[0]; i++) {

                }
                int startRng = i;

                for (; i < invalids.Count && invalids[i] <= range[1]; i++) {

                }
                int endRng = i;
                List<long> selectBitOfRange = invalids.Slice(startRng, endRng - startRng);
                selectedInvalids.AddRange(selectBitOfRange);
            }

            Console.WriteLine("Sum of invalids: " + selectedInvalids.Sum());
        }

    }
}
