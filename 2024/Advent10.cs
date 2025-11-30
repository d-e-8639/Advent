using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HelloWorld.lib;

namespace HelloWorld.A2024
{
    public class Advent10
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent10.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<List<MapSquare>> map = new List<List<MapSquare>>();
            foreach (string l in lines) {
                map.Add(l.Select(c => MapSquare.FromChar(c)).ToList());
            }
            for (int y=0; y < map.Count; y++) {
                for (int x=0; x < map[y].Count; x++) {
                    MapSquare.Init(x, y, map);
                }
            }

            task1(map);
            task2(map);
        }

        private static void task1(List<List<MapSquare>> map) {
            long allTrails = map.SelectMany(ms => ms).Where(ms => ms.height == 0).Sum(ms => ms.TrailScore());

            Console.WriteLine("Trail sum:" + allTrails);
        }

        private static void task2(List<List<MapSquare>> map) {
            long allTrails = map.SelectMany(ms => ms).Where(ms => ms.height == 0).Sum(ms => ms.TrailScore2());

            Console.WriteLine("Trail rating:" + allTrails);
        }

        public class MapSquare {
            public int height;
            public int x, y;
            public Point point;
            public List<MapSquare> Neighbors;

            public MapSquare (int h) {
                height = h;
            }

            public static void Init(int x, int y, List<List<MapSquare>> grid) {
                MapSquare ms = grid[y][x];
                ms.x = x;
                ms.y = y;
                ms.point = new Point(x,y);

                ms.Neighbors = new List<MapSquare>();
                if (x > 0) {
                    ms.Neighbors.Add(grid[y][x-1]);
                }
                if (y > 0) {
                    ms.Neighbors.Add(grid[y-1][x]);
                }
                if (x < grid[0].Count - 1) {
                    ms.Neighbors.Add(grid[y][x+1]);
                }
                if (y < grid.Count - 1){
                    ms.Neighbors.Add(grid[y+1][x]);
                }

            }

            public static MapSquare FromChar(Char c) {
                return new MapSquare(int.Parse(new string(c, 1)));
            }

            public long TrailScore() {
                HashSet<Point> endpoints = new HashSet<Point>();
                trailScore(new HashSet<Point>(), endpoints);
                return endpoints.Count;
            }

            private void trailScore(HashSet<Point> visited, HashSet<Point> endpoints) {
                visited.Add(point);

                if (this.height == 9) {
                    endpoints.Add(new Point(x, y));
                    return;
                }

                foreach (MapSquare ms in Neighbors) {
                    if ((ms.height == this.height + 1)&&(!visited.Contains(ms.point))) {
                        ms.trailScore(visited, endpoints);
                    }
                }
            }


            public long TrailScore2() {
                if (this.height == 9) {
                    return 1;
                }

                long sum=0;
                foreach (MapSquare ms in Neighbors) {
                    if (ms.height == this.height + 1) {
                        sum += ms.TrailScore2();
                    }
                }
                return sum;
            }
        }
    }
}
