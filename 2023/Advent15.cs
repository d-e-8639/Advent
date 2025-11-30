using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent
{
    public class Advent15
    {
        public static void Do()
        {
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent15.txt"))
            {
                file = sr.ReadToEnd();
            }

            string[] steps = file.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            task1(steps);
            task2(steps);
        }

        private static void task1(string[] steps)
        {
            Console.WriteLine("Hash sum:" + steps.Select(x => hash(x)).Sum());
        }

        private static int hash(string s)
        {
            int hash = 0;
            foreach (char c in s)
            {
                int cVal = (int)c;
                hash += cVal;
                hash *= 17;
                hash %= 256;
            }
            return hash;
        }

        private static void task2(string[] steps)
        {
            Box[] boxes = new Box[256];
            for (int i=0; i < boxes.Length; i++) {
                boxes[i] = new Box();
            }

            List<Lens> allLens = steps.Select(s => new Lens(s)).ToList();

            foreach (Lens l in allLens)
            {
                if (l.Operation == '=')
                {
                    boxes[l.LabelHash].Insert(l);
                }
                else if (l.Operation == '-') {
                    boxes[l.LabelHash].Remove(l);
                }
            }

            int totalFoc = 0;
            int boxNumber = 1;
            foreach (Box b in boxes) {
                int lensNumber = 1;
                foreach (Lens l in b.Lenses) {
                    int focPower = boxNumber * lensNumber * l.FocLen;
                    totalFoc += focPower;
                    lensNumber ++;
                }
                boxNumber ++;
            }

            Console.Write("Total focussing power: " + totalFoc);

        }

        public class Lens
        {
            private string definition;
            public string Label { get; private set; }
            private int labelHash = -1;
            public char Operation { get; private set; }
            public int FocLen { get; private set; }


            public int LabelHash
            {
                get
                {
                    if (labelHash == -1 ) {
                        labelHash = hash(Label);
                    }
                    return labelHash;
                }
            }

            public Lens(string def)
            {
                this.definition = def;
                int opIndex = 0;
                while (def[opIndex] != '-' && def[opIndex] != '=')
                {
                    opIndex++;
                }
                Label = def.Substring(0, opIndex);
                Operation = def[opIndex];
                if (Operation == '='){
                    FocLen = int.Parse(def.Substring(opIndex + 1));
                }
            }
        }

        public class Box
        {
            public LinkedList<Lens> Lenses = new LinkedList<Lens>();
            public Dictionary<string, LinkedListNode<Lens>> Index = new Dictionary<string, LinkedListNode<Lens>>();

            public void Insert(Lens l)
            {
                if (Index.ContainsKey(l.Label))
                {
                    Index[l.Label].Value = l;
                }
                else
                {
                    Index[l.Label] = Lenses.AddLast(l);
                }
            }

            public void Remove(Lens l)
            {
                if (Index.ContainsKey(l.Label))
                {
                    Lenses.Remove(Index[l.Label]);
                    Index.Remove(l.Label);
                }
            }
        }
    }
}
