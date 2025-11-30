using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace HelloWorld
{
    public class Advent11
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent11.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            task1(lines);
        }

        private static void task1(string[] lines) {
            StarMap m = new StarMap();
            for (int y=0; y < lines.Length; y++) {
                for (int x=0; x < lines[y].Length; x++){
                    if (lines[y][x] == '#') {
                        m.Stars.Add(new Star(x, y));
                    }
                }
            }
            // Console.WriteLine(m.ToString());
            m.Expand(1000000);
            // Console.WriteLine(m.ToString());

            List<Pair> pairs = new List<Pair>();
            for (int i=0; i < m.Stars.Count; i++) {
                for (int j=i+1; j < m.Stars.Count; j++) {
                    pairs.Add(new Pair(m.Stars[i], m.Stars[j]));
                }
            }
            Console.WriteLine("Total shortest path: " + pairs.Sum(p => p.Distance()));
        }

        private static void task2() {
        }

        private class Star {
            public long X,Y;
            public Star (long x, long y){
                X = x;
                Y = y;
            }
        }

        private class Pair {
            public Star A;
            public Star B;

            public Pair (Star a, Star b) {
                A = a;
                B = b;
            }

            public long Distance (){
                return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
            }
        }

        private class StarMap {
            public List<Star> Stars = new List<Star>();

            public StarMap() {
            }

            public void Expand (long expansionFactor) {
                long lastX=0;
                long incrementX=0;
                foreach (Star s in Stars.OrderBy(s => s.X)) {
                    if (s.X > lastX + 1) {
                        incrementX += (s.X - (lastX + 1)) * (expansionFactor - 1);
                    }
                    lastX = s.X;
                    s.X += incrementX;
                }

                long lastY=0;
                long incrementY=0;
                foreach (Star s in Stars.OrderBy(s => s.Y)) {
                    if (s.Y > lastY + 1) {
                        incrementY += (s.Y - (lastY + 1)) * (expansionFactor - 1);
                    }
                    lastY = s.Y;
                    s.Y += incrementY;
                }
            }

            public override string ToString()
            {
                long yMax = Stars.Select(s => s.Y).Max();
                long xMax = Stars.Select(s => s.X).Max();
                StringBuilder output = new StringBuilder();

                long x=0,y=0;
                foreach (Star s in Stars.OrderBy(s => s.X).OrderBy(s => s.Y)) {
                    while (true) {
                        bool b = false;
                        if ((x == s.X)&&(y == s.Y)){
                            output.Append('#');
                            b = true;
                        }
                        else {
                            output.Append('.');
                        }
                        x++;
                        if (x > xMax) {
                            y++;
                            x = 0;
                            output.AppendLine();
                        }
                        if (b) {
                            break;
                        }
                    }
                }
                return output.ToString();
            }
        }
    }
}
