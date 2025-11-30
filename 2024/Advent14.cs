using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using HelloWorld.lib;

namespace HelloWorld.A2024
{
    public class Advent14
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent14.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //Space sp = new Space(11, 7, lines.Select(l => Robot.FromString(l)).ToList());
            Space sp = new Space(101, 103, lines.Select(l => Robot.FromString(l)).ToList());

            task1(sp);

            task2(new Space(101, 103, lines.Select(l => Robot.FromString(l)).ToList()));
            //task2(new Space(11, 7, lines.Select(l => Robot.FromString(l)).ToList()));
        }

        private static void task1(Space sp) {
            sp.Move(100);
            Console.WriteLine(sp.ToString());
            Console.WriteLine("Safety: " + sp.Safety());
        }

        private static void task2(Space sp) {

            for (int i=0; i <= 101 * 103; i++) {
                if (sp.IsTree()) {
                    Console.WriteLine(i);
                    Console.WriteLine(sp.ToString());
                }
                sp.Move(1);
            }
        }

        public class Space {
            public int Width;
            public int Height;

            public List<Robot> Robots;

            public Space (int width, int height, List<Robot> robots) {
                Width = width;
                Height = height;
                Robots = robots;
            }

            public long Safety() {
                Tuple<int, int, int, int> t = Quads();
                return t.Item1 * t.Item2 * t.Item3 * t.Item4;
            }

            public Tuple<int, int, int, int> Quads() {
                int halfWidth = Width / 2;
                int halfHeight = Height / 2;
                int tl=0, tr=0, bl=0, br=0;
                foreach (Robot r in Robots) {
                    if ((r.position.X < halfWidth)&&(r.position.Y < halfHeight)) {
                        tl++;
                    }
                    else if ((r.position.X > halfWidth)&&(r.position.Y < halfHeight)) {
                        tr++;
                    }
                    else if ((r.position.X < halfWidth)&&(r.position.Y > halfHeight)) {
                        bl++;
                    }
                    else if ((r.position.X > halfWidth)&&(r.position.Y > halfHeight)) {
                        br++;
                    }
                }
                return new Tuple<int, int, int, int>(tl, tr, bl, br);
            }

            public void Move(int n)
            {
                foreach (Robot r in Robots) {r.Move(n, Width, Height);}
            }

            public override string ToString()
            {
                int [][] grid = new int[Height][];
                for (int i=0; i < grid.Length; i++) {
                     grid[i] = new int[Width];
                     for (int j=0; j < grid[i].Length; j++) {
                        grid[i][j] = 0;
                     }
                }

                foreach (Robot r in Robots) {
                    grid[r.position.Y][r.position.X] += 1;
                }

                StringBuilder sb = new StringBuilder();
                foreach (int[] row in grid) {
                    sb.AppendLine(string.Concat(row.Select(i => i == 0 ? "." : i.ToString())));
                }

                return sb.ToString();
            }

            public bool IsTree()
            {
                int [][] grid = new int[Height][];
                for (int i=0; i < grid.Length; i++) {
                     grid[i] = new int[Width];
                     for (int j=0; j < grid[i].Length; j++) {
                        grid[i][j] = 0;
                     }
                }

                foreach (Robot r in Robots) {
                    grid[r.position.Y][r.position.X] += 1;
                }

                for (int y=0; y < 100; y+=10) {
                    for (int x=0; x < 100; x+= 10) {
                        int sum=0;
                        for (int yy=y; yy < y + 10; yy++) {
                            for (int xx=x; xx < x + 10; xx++) {
                                if (grid[yy][xx] > 0) {
                                    sum ++;
                                }
                            }
                        }
                        if (sum > 25) {
                            return true;
                        }
                    }
                }

                return false;
            }

        }

        public class Robot {
            public Point position;
            public int VX;
            public int VY;

            public Robot (int pX, int pY, int vX, int vY) {
                position = new Point(pX, pY);
                VX = vX;
                VY = vY;
            }

            public static Robot FromString(string str) {
                //
                Regex p = new Regex("p=(\\d+),(\\d+) v=([-\\d]+),([-\\d]+)");
                Match m = p.Match(str);
                return new Robot(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));

            }

            // % operator is the remainder, and doesn't calculate modulo correctly for negative numbers.
            private static long modulo(long x, long N) {
                return (x % N + N) % N;
            }

            public void Move (int steps, int width, int height) {
                position.X = (int) modulo((long) position.X + (long) VX * steps, width);
                position.Y = (int) modulo((long) position.Y + (long) VY * steps, height);

            }
        }

    }
}
