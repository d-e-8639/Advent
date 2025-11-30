using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.A2023;
using System.Numerics;
using System.Xml;
using System.Text;

namespace Advent.A2024
{
    public class Advent13
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent13.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<string>> chunks = lines.SplitBy("", true, false).ToList();

            task1(chunks.Select(m => Machine.Deserialize(m)).ToList());

            task2(chunks.Select(m => Machine.Deserialize(m)).ToList());
        }

        private static void task1(List<Machine> machines) {
            long totalTokens = 0;
            foreach (Machine m in machines) {
                List<Tuple<long, long>> xMoves = calcAllSplits(m.ButtonA.XDiff, m.ButtonB.XDiff, m.PrizeX);
                List<Tuple<long, long>> yMoves = calcAllSplits(m.ButtonA.YDiff, m.ButtonB.YDiff, m.PrizeY);

                List<Tuple<long, long>> intersect = xMoves.Intersect(yMoves).OrderBy(x => PlayCost(m, x)).ToList();
                if (intersect.Any()) {
                    totalTokens += PlayCost(m, intersect.First());
                }
            }
            Console.WriteLine("Total cost: " + totalTokens + " tokens");
        }

        private static long PlayCost(Machine m, Tuple<long, long> play) {
            return (play.Item1 * m.ButtonA.Cost) + (play.Item2 * m.ButtonB.Cost);
        }

        private static List<Tuple<long, long>> calcAllSplits(long a, long b, long total) {
            List<Tuple<long, long>> results = new List<Tuple<long, long>>();

            for (long i=total / a; i >= 0; i--) {
                long remainder = total - (i * a);
                if (remainder % b == 0) {
                    results.Add(new Tuple<long, long>(i, remainder / b));
                }
            }
            return results;
        }


        private static void task2(List<Machine> machines) {
            foreach (Machine m in machines) {
                m.PrizeX += 10000000000000;
                m.PrizeY += 10000000000000;
            }

            long totalTokens = 0;

            foreach (Machine m in machines) {
                List<Tuple<long, long>> xMoves = calcStartEndDiff(m.ButtonA.XDiff, m.ButtonB.XDiff, m.PrizeX);
                List<Tuple<long, long>> yMoves = calcStartEndDiff(m.ButtonA.YDiff, m.ButtonB.YDiff, m.PrizeY);
                if (xMoves == null || yMoves == null) {
                    continue;
                }

                Equation one = new Equation(xMoves[0], xMoves[1], xMoves[1].Item1 - xMoves[2].Item1, xMoves[2].Item2 - xMoves[1].Item2, m);
                Equation two = new Equation(yMoves[0], yMoves[1], yMoves[1].Item1 - yMoves[2].Item1, yMoves[2].Item2 - yMoves[1].Item2, m);

                Tuple<long, long> match = search (one.LowLimit.Item1, one.HighLimit.Item1, one, two);
                if (match != null) {
                    Console.WriteLine(m);
                    Console.WriteLine(match);

                    totalTokens += PlayCost(m, match);
                }
            }
            Console.WriteLine("Big total cost: " + totalTokens + " tokens");

        }


        // Binary search to find the matching spot where lines intersect.
        private static Tuple<long, long> search (long rangeStart, long rangeEnd, Equation one, Equation two) {
            long pin = (rangeStart + rangeEnd) / 2;
            double diffStart = one.AtPointA(rangeStart) - two.AtPointA(rangeStart);
            double diffPin = one.AtPointA(pin) - two.AtPointA(pin);
            double diffEnd = one.AtPointA(rangeEnd) - two.AtPointA(rangeEnd);

            if (diffStart == 0) {
                return new Tuple<long, long>(rangeStart, (long) one.AtPointA(rangeStart));
                // Match found at start
            }
            if (diffPin == 0) {
                return new Tuple<long, long>(pin, (long) one.AtPointA(pin));
                // Match found at pin
            }
            if (diffEnd == 0) {
                return new Tuple<long, long>(rangeEnd, (long) one.AtPointA(rangeEnd));
                //Match found at End
            }

            if (rangeStart == rangeEnd || rangeStart + 1 == rangeEnd) {
                return null;
                // No whole number match; i.e. the match is fractional, which is not allowed, therefore not valid
            }

            if ((diffStart < 0 && diffEnd < 0) || (diffStart > 0 && diffEnd > 0)) {
                return null;
                // Lines must cross within the valid range for a match to be possible. If start and end are both
                // above zero or both below zero, no match possible.
            }

            // Recurse down into the half of the range where the line crosses.
            // This is where the pin has a different sign (e.g. + or -) to either the end or the start of the range.
            if ((diffStart < 0 && diffPin > 0)||(diffStart > 0 && diffPin < 0)) {
                return search (rangeStart, pin, one, two);
            }
            if ((diffEnd < 0 && diffPin > 0)||(diffEnd > 0 && diffPin < 0)) {
                return search (pin, rangeEnd, one, two);
            }
            throw new Exception("How get here?");
        }



        private static List<Tuple<long, long>> calcStartEndDiff(long a, long b, long total) {
            List<Tuple<long, long>> points = new List<Tuple<long, long>>();

            // Get the first valid combo on the X axis; this is the lower bound on possible number of button presses for button A
            for (long i=0, loops=0; i*a < total; i++, loops++) {
                long remainder = total - (i * a);
                if (remainder % b == 0) {
                    points.Add(new Tuple<long, long>(i, remainder / b));
                    break;
                }
                if (loops > 10000) {
                    return null;
                }
            }

            // Get the last and second last valid combo on the X axis; this is the upper bound on number of button presses for A.
            // The second last valid combo tells us the increment in number of button presses which will be valid on the x axis,
            // e.g. the number of A and B presses is a consistent pattern along the whole range.
            for (long i=total / a, loops=0; i >= 0; i--, loops++) {
                long remainder = total - (i * a);
                if (remainder % b == 0) {
                    points.Add(new Tuple<long, long>(i, remainder / b));
                }
                if (points.Count == 3) {
                    break;
                }
                if (loops > 10000) {
                    return null;
                }
            }

            if (! points.Any()) {
                return null;
            }
            return points;
        }


        public class Machine {
            public Button ButtonA;
            public Button ButtonB;
            public long PrizeX;
            public long PrizeY;

            public Machine (Button a, Button b, long x, long y) {
                ButtonA = a;
                ButtonB = b;
                PrizeX = x;
                PrizeY = y;
            }

            private static Regex parser = new Regex("Prize: X=(\\d+), Y=(\\d+)");
            public static Machine Deserialize(List<string> def) {
                Match m = parser.Match(def[2]);

                return new Machine(
                    Button.Deserialize(3, def[0]),
                    Button.Deserialize(1, def[1]),
                    long.Parse(m.Groups[1].Value), long.Parse(m.Groups[2].Value)
                );
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Button A: {ButtonA.ToString()}");
                sb.AppendLine($"Button B: {ButtonB.ToString()}");
                sb.AppendLine($"Prize: X={PrizeX}, Y={PrizeY}");
                return sb.ToString();
            }
        }

        public class Button {
            public long Cost;
            public long XDiff;
            public long YDiff;

            public Button (long cost, long xDiff, long yDiff) {
                Cost = cost;
                XDiff = xDiff;
                YDiff = yDiff;
            }

// Button A: X+94, Y+34
// Button B: X+22, Y+67
// Prize: X=8400, Y=5400
            private static Regex parser = new Regex("Button .: X\\+(\\d+), Y\\+(\\d+)");
            public static Button Deserialize(long cost, string def) {
                Match m = parser.Match(def);
                return new Button(cost, long.Parse(m.Groups[1].Value), long.Parse(m.Groups[2].Value));
            }

            public override string ToString()
            {
                return $"X+{XDiff}, Y+{YDiff}";
            }
        }
        // public class ButtonA : Button {
        //     public ButtonA (long xDiff, long yDiff)
        //         : base (3, xDiff, yDiff) {}
        // }
        // public class ButtonB : Button {
        //     public ButtonB (long xDiff, long yDiff)
        //         : base (1, xDiff, yDiff) {}
        // }

        public class Equation {
            public Tuple<long, long> LowLimit;
            public Tuple<long, long> HighLimit;
            public long Span;
            public long ButtonAIncrement;
            public long ButtonBIncrement;
            public Machine M;

            public Equation(Tuple<long, long> lowLimit, Tuple<long, long> highLimit, long buttonAIncrement, long buttonBIncrement, Machine m) {
                LowLimit = lowLimit;
                HighLimit = highLimit;
                ButtonAIncrement = buttonAIncrement;
                ButtonBIncrement = buttonBIncrement;
                M = m;

                Span = (HighLimit.Item1 - LowLimit.Item1) / ButtonAIncrement;
                //(LowLimit.Item2 - HighLimit.Item2) / ButtonBIncrement;
            }

            public double AtPointA(long A) {
                double partSpan = (double)(A - LowLimit.Item1) / (double) ButtonAIncrement;
                double ret = HighLimit.Item2 + (((double) Span) - partSpan) * ButtonBIncrement;
                return ret;
            }
        }

    }
}

