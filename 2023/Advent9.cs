using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Advent
{
    public class Advent9
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent9.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<History> hs = new List<History>();
            foreach (string line in lines) {
                History h = new History();
                h.Populate(line);
                hs.Add(h);
            }

            task1(hs);
            
            task2(hs);
        }

        private static void task1(List<History> hs) {

            // foreach (History h in hs) {
            //     Console.WriteLine(h);
            // }
            Console.WriteLine("Prediction Sum: " +  hs.Sum(h => h.Prediction));

        }

        private static void task2(List<History> hs) {

            foreach (History h in hs) {
                Console.WriteLine(h);
            }

            Console.WriteLine("Extrapolation Sum: " +  hs.Sum(h => h.Extrapolation));

        }

    }

    public class History {
        List<Sequence> Diffs = new List<Sequence>();

        public int Prediction {
            get { return Diffs[0].Prediction; }
        }

        public int Extrapolation {
            get { return Diffs[0].Extrapolation; }
        }

        public void Populate(string line) {
            Diffs.Add(new Sequence(line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList()));
            while (! Diffs.Last().Measured.All(x => x == 0)) {
                Diffs.Add(createDiff(Diffs.Last()));
            }

            Diffs.Last().Prediction = 0;
            Diffs.Last().Extrapolation = 0;
            int lastP=0;
            int lastE=0;
            foreach (Sequence s in System.Linq.Enumerable.Reverse(Diffs).Skip(1)) {
                s.Prediction = s.Measured.Last() + lastP;
                s.Extrapolation = s.Measured.First() - lastE;
                lastP = s.Prediction;
                lastE = s.Extrapolation;
            }
        }

        private Sequence createDiff(Sequence original) {
            List<int> diff = new List<int>();
            for (int i=1; i < original.Measured.Count; i++)
            {
                diff.Add(original.Measured[i] - original.Measured[i-1]);
            }
            return new Sequence(diff);
        }

        public class Sequence{
            public List<int> Measured;
            public int Prediction;
            public int Extrapolation;

            public Sequence(IEnumerable<int> measured) {
                Measured = new List<int>(measured);
            }

            public override string ToString()
            {
                return "*" + Extrapolation + " " + string.Join(" ", Measured) + " *" + Prediction;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string indent = "";
            foreach (Sequence s in Diffs) {
                sb.Append(indent);
                sb.AppendLine(s.ToString());
                indent += " ";
            }
            return sb.ToString();
        }

    }

}
