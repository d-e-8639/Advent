using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.lib;

namespace Advent.Y2025
{
    public class Advent8
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent8.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<Point3D> points = lines.Select(l => l.Split(',').Select(n => int.Parse(n)).ToArray())
                                            .Select(l => new Point3D(l[0], l[1], l[2])).ToList();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(points);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2();
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1(List<Point3D> points) {
            LinkedList<(Point3D, Point3D, double)> ShortestDistances = new LinkedList<(Point3D, Point3D, double)>();
            
            for (int i=0; i < points.Count; i++)
            {
                Point3D point = points[i];

                List<(Point3D, double)> distances = new List<(Point3D, double)>();
                for (int j=i+1; j < points.Count; j++)
                {
                    Point3D other = points[j];
                    if (point != other) {
                        distances.Add((other, point.Distance(other)));
                    }
                }

                distances = distances.OrderBy(d => d.Item2).ToList();

                LinkedListNode<(Point3D, Point3D, double)> pin = ShortestDistances.First;
                foreach ((Point3D, double) d in distances)
                {
                    while (pin != null && pin.Value.Item3 < d.Item2)
                    {
                        pin = pin.Next;
                    }

                    if (ShortestDistances.Count == 0)
                    {
                        ShortestDistances.AddFirst((point, d.Item1, d.Item2));
                        pin = ShortestDistances.First;
                    }
                    else if (pin == null)
                    {
                        ShortestDistances.AddLast((point, d.Item1, d.Item2));
                        if (ShortestDistances.Count > 1000)
                        {
                            break;
                        }
                    }
                    else {
                        ShortestDistances.AddBefore(pin, (point, d.Item1, d.Item2));
                    }
                }

                while (ShortestDistances.Count > 1000)
                {
                    ShortestDistances.RemoveLast();
                }
            }

        }

        private static void task2() {
        }

    }
}
