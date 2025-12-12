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
            task1(points, 1000);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(points, 10000);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        private static void task1(List<Point3D> points, int joinsToMake) {
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
                        if (ShortestDistances.Count > joinsToMake)
                        {
                            break;
                        }
                    }
                    else {
                        ShortestDistances.AddBefore(pin, (point, d.Item1, d.Item2));
                    }
                }

                while (ShortestDistances.Count > joinsToMake)
                {
                    ShortestDistances.RemoveLast();
                }
            }

            LinkedList<HashSet<Point3D>> buckets = new LinkedList<HashSet<Point3D>>();
            foreach (Point3D p in points) {
                buckets.AddLast(new HashSet<Point3D>() { p });
            }

            foreach ((Point3D, Point3D, double) d in ShortestDistances) {
                LinkedListNode<HashSet<Point3D>> bucket1=null, bucket2=null;
                for (LinkedListNode<HashSet<Point3D>> node= buckets.First; node != null; node = node.Next) {
                    if (node.Value.Contains(d.Item1)) {
                        bucket1 = node;
                    }
                    if (node.Value.Contains(d.Item2)) {
                        bucket2 = node;
                    }
                }

                if (bucket1 == null || bucket2 == null) {
                    throw new Exception();
                }
                if (bucket1 == bucket2) {

                }
                else {
                    bucket1.Value.UnionWith(bucket2.Value);
                    buckets.Remove(bucket2);
                }
            }

            int result = buckets.Select(b => b.Count).OrderByDescending(c => c).Take(3).Aggregate((a, b) => a * b);
            Console.WriteLine("result: " + result);
        }

        private static void task2(List<Point3D> points, int joinsToMake) {
            LinkedList<(Point3D, Point3D, double)> ShortestDistances = new LinkedList<(Point3D, Point3D, double)>();

            for (int i = 0; i < points.Count; i++) {
                Point3D point = points[i];

                List<(Point3D, double)> distances = new List<(Point3D, double)>();
                for (int j = i + 1; j < points.Count; j++) {
                    Point3D other = points[j];
                    if (point != other) {
                        distances.Add((other, point.Distance(other)));
                    }
                }

                distances = distances.OrderBy(d => d.Item2).ToList();

                LinkedListNode<(Point3D, Point3D, double)> pin = ShortestDistances.First;
                foreach ((Point3D, double) d in distances) {
                    while (pin != null && pin.Value.Item3 < d.Item2) {
                        pin = pin.Next;
                    }

                    if (ShortestDistances.Count == 0) {
                        ShortestDistances.AddFirst((point, d.Item1, d.Item2));
                        pin = ShortestDistances.First;
                    }
                    else if (pin == null) {
                        ShortestDistances.AddLast((point, d.Item1, d.Item2));
                        if (ShortestDistances.Count > joinsToMake) {
                            break;
                        }
                    }
                    else {
                        ShortestDistances.AddBefore(pin, (point, d.Item1, d.Item2));
                    }
                }

                while (ShortestDistances.Count > joinsToMake) {
                    ShortestDistances.RemoveLast();
                }
            }

            LinkedList<HashSet<Point3D>> buckets = new LinkedList<HashSet<Point3D>>();
            foreach (Point3D p in points) {
                buckets.AddLast(new HashSet<Point3D>() { p });
            }

            (Point3D, Point3D, double) lastConnection = default;
            foreach ((Point3D, Point3D, double) d in ShortestDistances) {
                LinkedListNode<HashSet<Point3D>> bucket1 = null, bucket2 = null;
                for (LinkedListNode<HashSet<Point3D>> node = buckets.First; node != null; node = node.Next) {
                    if (node.Value.Contains(d.Item1)) {
                        bucket1 = node;
                    }
                    if (node.Value.Contains(d.Item2)) {
                        bucket2 = node;
                    }
                }

                if (bucket1 == null || bucket2 == null) {
                    throw new Exception();
                }
                if (bucket1 == bucket2) {

                }
                else {
                    if (buckets.Count == 2) {

                    }
                    bucket1.Value.UnionWith(bucket2.Value);
                    buckets.Remove(bucket2);
                    if (buckets.Count == 1) {
                        lastConnection = d;
                        break;
                    }
                }
            }

            if (lastConnection == default) {
                throw new Exception("Didn't find it!");
            }
            long result = (long)lastConnection.Item1.X * (long)lastConnection.Item2.X;
            Console.WriteLine("Last connection X mult : " + result);
        }

    }
}
