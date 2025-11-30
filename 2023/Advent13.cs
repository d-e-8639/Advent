using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace Advent
{
    public class Advent13
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent13.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<string>> linesGrp = [new List<string>()];
            foreach (string l in lines) {
                if ((l == "")&&(linesGrp[linesGrp.Count - 1].Count != 0)) {
                    linesGrp.Add(new List<string>());
                }
                else {
                    linesGrp[linesGrp.Count - 1].Add(l);
                }
            }
            if (linesGrp.Last().Count == 0) {
                linesGrp.RemoveAt(linesGrp.Count - 1);
            }

            List<Pattern> patterns = linesGrp.Select(x => new Pattern(x)).ToList();

            task1(patterns);
            task2(patterns);
        }

        private static void task1(List<Pattern> patterns) {
            long total = patterns.Select (p => p.GetVerticalReflect()).Where(x => x >=0).Sum();
            total += patterns.Select (p => p.GetHorizontalReflect()).Where(x => x >=0).Sum() * 100;

            Console.WriteLine("summary: " + total);
        }

        private static void task2(List<Pattern> patterns) {
            long total = patterns.Select (p => p.GetVerticalReflectSmudge()).Where(x => x >=0).Sum();
            total += patterns.Select (p => p.GetHorizontalReflectSmudge()).Where(x => x >=0).Sum() * 100;

            Console.WriteLine("summary: " + total);
        }


        public class Pattern {
            public bool[][] Coords;
            public List<List<uint>> HorzRows = new List<List<uint>>();
            public List<List<uint>> VertRows = new List<List<uint>>();


            public Pattern(IEnumerable<string> lines) {
                Coords = lines.Select(l => l.Select(c => (c == '#')).ToArray()).ToArray();

                for (uint y=0; y < Coords.Length; y++) {
                    List<uint> row = new List<uint>();
                    for (uint x=0; x < Coords[y].Length; ) {
                        uint next = 0;
                        for (int i=31; i >= 0 && x < Coords[y].Length; i--, x++) {
                            if (Coords[y][x]) {
                                next |= ((uint)0x1) << i;
                            }
                        }
                        row.Add(next);
                    }
                    HorzRows.Add(row);
                }

                for (uint x=0; x < Coords[0].Length; x++) {
                    List<uint> col = new List<uint>();
                    for (uint y=0; y < Coords.Length; ) {
                        uint next = 0;
                        for (int i=31; i >= 0 && y < Coords.Length; i--, y++) {
                            if (Coords[y][x]) {
                                next |= ((uint)0x1) << i;
                            }
                        }
                        col.Add(next);
                    }
                    VertRows.Add(col);
                }

            }

            private bool checkMirror (List<List<uint>> lines, int index) {
                if (index <= 0){
                    throw new ArgumentException();
                }

                for (int left=index-1, right=index; left >= 0 && right < lines.Count; left--, right++) {
                    if (!(lines[left].SequenceEqual(lines[right]))) {
                        return false;
                    }
                }
                return true;
            }

            private int scan(List<List<uint>> lines) {
                for (int x=1; x < lines.Count; x++) {
                    if (checkMirror(lines, x)) {
                        return x;
                    }
                }
                return -1;
            }

            private int scanSmudge(List<List<uint>> lines, int rowLen, int preSmudge) {

                for (int y=0; y < lines.Count; y++) {
                    int bitdex = 31;
                    for (int x=0; x < rowLen; x++, bitdex--) {
                        if (bitdex < 0) {
                            bitdex = 31;
                        }
                        lines[y][(x >> 8)] = lines[y][(x >> 8)] ^ (((uint) 0x1) << bitdex);

                        for (int i=1; i < lines.Count; i++) {
                            if (i != preSmudge && checkMirror(lines, i)) {
                                return i;
                            }
                        }

                        lines[y][(x >> 8)] = lines[y][(x >> 8)] ^ (((uint) 0x1) << bitdex);
                    }
                }

                return -1;
            }

            private int preSmudgeVert = -1;
            public int GetVerticalReflect() {
                preSmudgeVert = scan(VertRows);
                return preSmudgeVert;
            }

            private int preSmudgeHorz = -1;
            public int GetHorizontalReflect() {
                preSmudgeHorz = scan(HorzRows);
                return preSmudgeHorz;
            }

            public int GetVerticalReflectSmudge() {
                return scanSmudge(VertRows, Coords.Length, preSmudgeVert);
            }

            public int GetHorizontalReflectSmudge() {
                return scanSmudge(HorzRows, Coords[0].Length, preSmudgeHorz);
            }
        }
    }
}
