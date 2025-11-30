using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Diagnostics;

namespace HelloWorld.A2024
{
    public class Advent9
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent9.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<int?> blocks = createBlocks(lines.Single());

            task1(blocks);

            LinkedList<EFile> blocks2 = createLLBlocks(lines.Single());

            Stopwatch st = new Stopwatch();
            st.Start();
            task2(blocks2);
            st.Stop();
            Console.WriteLine("t2 time taken: " + st.ElapsedMilliseconds);
        }

        private static void task1(List<int?> blocks) {

            //Console.WriteLine(ToString(blocks));

            int popIndex=blocks.Count - 1;
            while (blocks[popIndex] == null) {
                popIndex --;
            }

            for (int i=0; i < blocks.Count; i++) {
                if (popIndex < i) {
                    break;
                }
                if (blocks[i] == null) {
                    blocks[i] = blocks[popIndex];
                    blocks[popIndex] = null;
                    while (blocks[popIndex] == null) {
                        popIndex --;
                    }
                }
            }

            //Console.WriteLine(ToString(blocks));
            Console.WriteLine("Checksum: " + checksum(blocks));
        }

        private static void task2(LinkedList<EFile> blocks) {
            LinkedListNode<EFile> pin = blocks.Last;
            while (pin != blocks.First) {
                LinkedListNode<EFile> search = blocks.First;
                LinkedListNode<EFile> nextPin = pin.Previous;
                while (search != pin) {
                    if (search.Value.FreeTail >= pin.Value.Size) {
                        // switch
                        nextPin.Value.FreeTail += pin.Value.Size + pin.Value.FreeTail;
                        pin.Value.FreeTail = search.Value.FreeTail - pin.Value.Size;
                        search.Value.FreeTail = 0;
                        blocks.AddAfter(search, pin.Value);
                        blocks.Remove(pin);
                        break;
                    }
                    search = search.Next;
                }
                pin = nextPin;
            }

            Console.WriteLine("Checksum 2: " + checksum(blocks));
        }

        public static long checksum(List<int?> blocks) {
            long sum = 0;
            for (int i=0; i < blocks.Count; i++) {
                sum += (i * (blocks[i]??0));
            }
            return sum;
        }

        public static long checksum(LinkedList<EFile> blocks) {
            long sum = 0;
            long tmp=0;
            int i=0;
            foreach (EFile file in blocks) {
                tmp += (long) (((((double)2 * i) + (file.Size - 1)) / 2) * file.Size) * file.FileId;
                for (int j=0; j < (file.Size); j++) {
                    sum += ((j + i) * file.FileId);
                }
                i += file.Size + file.FreeTail;
            }

            return sum;
        }

        public static string ToString(List<int?> blocks) {
            StringBuilder sb = new StringBuilder();
            for (int i=0; i < blocks.Count; i++) {
                sb.Append(blocks[i] == null ? '.' : blocks[i].Value.ToString());
                if (i%80 == 79) {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public static List<int?> createBlocks(string line) {
            List<int?> result = new List<int?>();
            int? id = 0;
            for (int i=0; i < line.Length; i++) {
                int n = int.Parse(new string(line[i], 1));
                if ((i & 1) == 0) {
                    result.AddRange(Enumerable.Repeat(id, n));
                    id ++;
                }
                else {
                    result.AddRange(Enumerable.Repeat((int?) null, n));
                }
            }
            return result;
        }

        public static LinkedList<EFile> createLLBlocks(string line) {
            LinkedList<EFile> result = new LinkedList<EFile>();
            int id = 0;
            for (int i=0; i < line.Length; i++) {
                int n = int.Parse(new string(line[i], 1));
                if ((i & 1) == 0) {
                    EFile current = new EFile();
                    current.FileId = id;
                    current.Size = n;
                    result.AddLast(current);
                    id ++;
                }
                else {
                    result.Last.Value.FreeTail = n;
                }
            }
            return result;
        }

        public class EFile {
            public int FileId;
            public int Size;
            public int FreeTail=0;

            public override string ToString()
            {
                return $"({FileId}, {Size}, {FreeTail})";
            }
        }
    }
}
