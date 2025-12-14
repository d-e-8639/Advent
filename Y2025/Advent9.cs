using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.lib;

namespace Advent.Y2025
{
    public class Advent9
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent9.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<Point> points = lines.Select(l => l.Split(',').Select(n => int.Parse(n)).ToArray()).Select(l => new Point(l[0], l[1])).ToList();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(points);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(points);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static long area(Point a, Point b) {
            // THis bug took 5 hours to find
            //long width = Math.Abs(a.X - b.X + 1);
            //long height = Math.Abs(a.Y - b.Y + 1);
            long width = Math.Abs(a.X - b.X) + 1;
            long height = Math.Abs(a.Y - b.Y) + 1;
            return width * height;
        }

        private static void task1(List<Point> points) {
            long bestArea = 0;

            for (int i=0; i < points.Count; i++) {
                for (int j=i+1; j < points.Count; j++) {
                    long a = area(points[i], points[j]);
                    if (a > bestArea) {
                        Console.WriteLine($"New best. Point {points[i]}, {points[j]}, area: {a}");
                        bestArea = a;
                    }
                }
            }
            Console.WriteLine(bestArea);
        }

        private static void task2(List<Point> points) {
            long bestArea = 0;
            List<Line> edges = new List<Line>();
            for (int i = 1; i < points.Count; i++) {
                edges.Add(new Line(points[i - 1], points[i]));
            }
            edges.Add(new Line(points.First(), points.Last()));

            graph(points, edges, null, null, null);

            for (int i = 0; i < points.Count; i++) {
                for (int j=0; j < points.Count; j++) {
                    long a = area(points[i], points[j]);

                    if (a == 1654104320) {
                        long b = area(points[i], points[j]);
                    }

                    if (a == 1654141440) {
                        Rectangle r = new Rectangle(points[i], points[j]);
                        graph(points, edges, r, points[i], points[j]);
                    }
                }
            }

            for (int i = 0; i < points.Count; i++) {
                for (int j = i + 1; j < points.Count; j++) {
                    long a = area(points[i], points[j]);

                    if (a == 1654141440) {
                        Rectangle r = new Rectangle(points[i], points[j]);
                        graph(points, edges, r, points[i], points[j]);
                    }

                    if (a >= bestArea) {

                        Rectangle r = new Rectangle(points[i], points[j]);



                        if (points.Any(p => r.Inside(p))) {
                            // There's a point inside this rectangle, definitely not valid.
                            continue;
                        }

                        //if (a == 1654104320) {
                        //    graph(points, edges, r, points[i], points[j]);
                        //    ;
                        //}

                        //foreach (Line l in edges) {
                        //    if ((r.Edges.Count(r => r.Crossing(l)) == 2) && !r.Edges.Any(re => re.IsEquivalentTrajectory(l))) {
                        //        // Invalid
                        //        if (a > 1654104320) {
                        //            //graph(points, edges, r);
                        //            ;
                        //        }
                        //    }
                        //}
                        if (edges.Any(l => ((r.Edges.Count(r => r.Crossing(l)) == 2) && !r.Edges.Any(re => re.IsEquivalentTrajectory(l))))) {
                            // There is a green tile edge crossing through the interior of this rectangle, not a valid rectangle
                            if (a > 1654104320L) {
                                //graph(points, edges, r, points[i], points[j]);
                                Console.WriteLine($"Not valid: Point {points[i]}, {points[j]}, area: {a}");
                            }
                            continue;
                        }

                        Console.WriteLine($"New best. Point {points[i]}, {points[j]}, area: {a}");
                        graph(points, edges, r, points[i], points[j]);
                        bestArea = a;

                    }
                }
            }
            Console.WriteLine(bestArea);
        }

        private static void graph(List<Point> points, List<Line> edges, Rectangle r = null, Point p1=null, Point p2= null) {
            int shrinkFactor = 300;
            char[][] c = new char[points.Max(p => p.X) / shrinkFactor  + 1][];
            int maxY = (int)points.Max(p => p.Y) / shrinkFactor  + 1;
            for (int i = 0; i < c.Length; i++) {
                c[i] = new char[maxY];
                for (int j=0; j < c[i].Length; j++) {
                    c[i][j] = '.';
                }
            }


            foreach (Line l in edges) {
                if (l.IsHorizontal) {
                    for (int i = (int)Math.Min(l.Start.X, l.End.X); i < (int)Math.Max(l.Start.X, l.End.X) ; i++) {
                        c[i / shrinkFactor][(int)l.Start.Y / shrinkFactor] = '|';
                    }
                }
                else if (l.IsVertical) {
                    for (int i = (int)Math.Min(l.Start.Y, l.End.Y); i < (int)Math.Max(l.Start.Y, l.End.Y); i++) {
                        c[(int)l.Start.X / shrinkFactor][i / shrinkFactor] = '-';
                    }
                }

            }

            if (r != null) {
                for (int x = (int) r.SmallCorner.X; x < (int) r.LargeCorner.X; x+=(shrinkFactor / 2)) {
                    for (int y = (int) r.SmallCorner.Y; y < (int) r.LargeCorner.Y; y+= (shrinkFactor / 2)) {
                        c[x / shrinkFactor][y / shrinkFactor] = '0';
                    }
                }
            }

            foreach (Point p in points) {
                c[p.X / shrinkFactor][p.Y / shrinkFactor] = '*';
            }

            if (p1 != null) {
                c[p1.X / shrinkFactor][p1.Y / shrinkFactor] = '%';
            }

            if (p2 != null) {
                c[p2.X / shrinkFactor][p2.Y / shrinkFactor] = '%';
            }

            foreach (char[] line in c) {
                Console.WriteLine(new string(line));
            }

        }

        public class Rectangle {
            public Point SmallCorner;
            public Point LargeCorner;
            public Line[] Edges;

            public Rectangle(Point a, Point b) {
                SmallCorner = new Point(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
                LargeCorner = new Point(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

                Edges = new Line[] {
                    new Line(SmallCorner, new Point(SmallCorner.X, LargeCorner.Y)),
                    new Line(new Point(SmallCorner.X, LargeCorner.Y), LargeCorner),
                    new Line(LargeCorner, new Point(LargeCorner.X, SmallCorner.Y)),
                    new Line(new Point(LargeCorner.X, SmallCorner.Y), SmallCorner),
                };
            }

            public bool Inside(Point p) {
                return ((p.X > SmallCorner.X && p.X < LargeCorner.X) &&
                    (p.Y > SmallCorner.Y && p.Y < LargeCorner.Y));
            }

            public bool IsOnSameEdge(Point a, Point b) {
                if (Edges.Any(e => e.PointIsOn(a) && e.PointIsOn(b))) {
                    return true;
                }
                return false;
            }
        }

    }
}
