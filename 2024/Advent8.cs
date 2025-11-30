using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HelloWorld.A2024
{
    public class Advent8
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent8.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<char, List<Antenna>> map = new Dictionary<char, List<Antenna>>();

            for (int y=0; y < lines.Length; y++) {
                for (int x=0; x < lines[y].Length; x++) {
                    if (lines[y][x] == '.') {
                        continue;
                    }
                    if (!map.ContainsKey(lines[y][x])) {
                        map[lines[y][x]] = new List<Antenna>();
                    }
                    map[lines[y][x]].Add(new Antenna(lines[y][x], x, y));
                }
            }

            task1(map, lines[0].Length, lines.Length);
            task2(map, lines[0].Length, lines.Length);
        }

        private static void task1(Dictionary<char, List<Antenna>> map, int mX, int mY) {
            List<Point> allAntinodes = new List<Point>();
            foreach(List<Antenna> ant in map.Values) {
                allAntinodes.AddRange(Antenna.Antinodes(ant));
            }

            int count = allAntinodes.Where(p => p.X >= 0 && p.Y >=0 && p.X < mX && p.Y < mY).Distinct().Count();
            Console.WriteLine("Node count: " + count);
        }

        private static void task2(Dictionary<char, List<Antenna>> map, int mX, int mY) {
            List<Point> allAntinodes = new List<Point>();
            foreach(List<Antenna> ant in map.Values) {
                allAntinodes.AddRange(Antenna.HarmonicAntinodes(ant, mX, mY));
            }

            int count = allAntinodes.Where(p => p.X >= 0 && p.Y >=0 && p.X < mX && p.Y < mY).Distinct().Count();
            Console.WriteLine("Harmoninc AntiNode count: " + count);
        }

        public class Antenna {
            public char Label;
            public Point P;

            public Antenna(char l, int x, int y) {
                Label = l;
                P = new Point(x, y);
            }

            public static List<Point> Antinodes (List<Antenna> antennas) {
                List<Point> result = new  List<Point>();
                for (int i=0; i < antennas.Count - 1; i++) {
                    for (int j=i+1; j < antennas.Count; j++) {
                        result.Add(
                            new Point(antennas[i].P.X + (antennas[i].P.X - antennas[j].P.X),
                                    antennas[i].P.Y + (antennas[i].P.Y - antennas[j].P.Y))
                        );

                        result.Add(
                            new Point(antennas[j].P.X + (antennas[j].P.X - antennas[i].P.X),
                                    antennas[j].P.Y + (antennas[j].P.Y - antennas[i].P.Y))
                        );
                    }
                }

                return result;
            }


            public static List<Point> HarmonicAntinodes (List<Antenna> antennas, int mX, int mY) {
                List<Point> result = new  List<Point>();
                for (int i=0; i < antennas.Count - 1; i++) {
                    for (int j=i+1; j < antennas.Count; j++) {

                        for (int n=0; ; n++) {
                            int nuX = antennas[i].P.X + n * (antennas[i].P.X - antennas[j].P.X);
                            int nuY = antennas[i].P.Y + n * (antennas[i].P.Y - antennas[j].P.Y);
                            if (nuX < 0 || nuY < 0 || nuX >= mX || nuY >= mY) {
                                break;
                            }
                            result.Add(new Point(nuX, nuY));
                        }

                        for (int n=0; ; n++) {
                            int nuX = antennas[j].P.X + n * (antennas[j].P.X - antennas[i].P.X);
                            int nuY = antennas[j].P.Y + n * (antennas[j].P.Y - antennas[i].P.Y);
                            if (nuX < 0 || nuY < 0 || nuX >= mX || nuY >= mY) {
                                break;
                            }
                            result.Add(new Point(nuX, nuY));
                        }

                    }
                }

                return result;
            }

        }

    }
}
