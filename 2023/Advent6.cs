using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HelloWorld
{
    public class Advent6
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent6.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<Tuple<long, long>> records = parse(lines);

            task1brute(records);
            task1(records);
            task2(lines);
        }

        private static void task1brute(List<Tuple<long, long>> records) {
            long margin = 1;
            foreach (Tuple<long, long> record in records) {
                long time = record.Item1;
                long dist = record.Item2;

                long recordBreaking = 0;
                for (long t1=0,t2=time; t1 <= time; t1++,t2--) {
                    if (t1 * t2 > dist) {
                        recordBreaking ++;
                    }
                }
                margin *= recordBreaking;
            }
            Console.WriteLine("brute margin: " + margin);
        }

        private static void task1(List<Tuple<long, long>> records) {
            long margin = 1;
            foreach (Tuple<long, long> record in records) {
                long time = record.Item1;
                long dist = record.Item2;

                long left = binSearchMin(1, time, time, dist);
                long right = binSearchMax(1, time, time, dist);
                margin *= (right - left + 1);
            }
            Console.WriteLine("binary search margin: " + margin);
        }


        private static void task2(string[] lines) {
            long time = long.Parse(string.Concat(lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)));
            long dist = long.Parse(string.Concat(lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)));

            long left = binSearchMin(1, time, time, dist);
            long right = binSearchMax(1, time, time, dist);
            Console.WriteLine("Ways to win: " + (right - left + 1));

        }

        private static long binSearchMin(long min, long max, long total, long dist) {
            long pin = (min + max) / 2;
            long prevDist = (pin - 1) * (total - (pin - 1));
            long currentDist = pin * (total - pin);
            if ((prevDist <= dist)&&(currentDist > dist)) {
                return pin;
            }
            else if ((prevDist <= currentDist) && (currentDist < dist)){
                return binSearchMin(pin, max, total, dist);
            }
            else if ((prevDist > currentDist) && (currentDist < dist)){
                return binSearchMin(min, pin, total, dist);
            }
            else if ((prevDist > dist) && (currentDist > dist)){
                return binSearchMin(min, pin, total, dist);
            }
            else {
                throw new Exception("what?");
            }
        }

        private static long binSearchMax(long min, long max, long total, long dist) {
            long pin = (min + max) / 2;
            long prevDist = (pin - 1) * (total - (pin - 1));
            long currentDist = pin * (total - pin);
            if ((prevDist > dist)&&(currentDist <= dist)) {
                return pin - 1;
            }
            else if ((prevDist <= currentDist) && (currentDist < dist)){
                return binSearchMax(pin, max, total, dist);
            }
            else if ((prevDist > currentDist) && (currentDist < dist)){
                return binSearchMax(min, pin, total, dist);
            }
            else if ((prevDist > dist) && (currentDist > dist)){
                return binSearchMax(pin, max, total, dist);
            }
            else {
                throw new Exception("what v2?");
            }
        }


        static List<Tuple<long, long>> parse(string[] lines) {
            string[] timeStr = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] distStr = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            List<Tuple<long, long>> result = new List<Tuple<long, long>>();
            for (int i=1; i < timeStr.Length; i++){
                result.Add(new Tuple<long, long>(long.Parse(timeStr[i]), long.Parse(distStr[i])));
            }
            return result;
        }
    }
}
