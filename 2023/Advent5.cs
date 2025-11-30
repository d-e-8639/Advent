using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Advent
{
    public class Advent5
    {
        public static void Do(){
            string file;
            using (StreamReader sr = new StreamReader("C:/src/HelloWorld/Advent5.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<List<string>> sections = new List<List<string>>();
            sections.Add(new List<string>());

            foreach (string line in lines) {
                if (line == ""){
                    sections.Add(new List<string>());
                }
                else {
                    sections[sections.Count - 1].Add(line);
                }
            }
            sections = sections.Where(s => s.Count > 0).ToList();

            string seedsLine = null;
            List<Map> maps = new List<Map>();
            foreach (List<string> section in sections) {
                if (section[0].StartsWith("seeds:")) {
                    //seeds = section[0].Substring(6).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();
                    seedsLine = section[0];
                }

                if (section[0].EndsWith("map:")) {
                    maps.Add(Map.FromString(section));
                }
            }

            task1(seedsLine, maps);
            //task2Brute(seedsLine, maps);
            task2(seedsLine, maps);
        }

        private static void task1(string seedsLine, List<Map> rawMaps) {
            List<long> seeds = seedsLine.Substring(6).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();

            Dictionary<string, Map> maps = rawMaps.ToDictionary(m => m.from);

            long closest= long.MaxValue;

            foreach (long seed in seeds) {
                string mapKey = "seed";
                long lookup = maps[mapKey].Get(seed);
                do {
                    mapKey = maps[mapKey].to;
                    lookup = maps[mapKey].Get(lookup);
                } while (maps.ContainsKey(maps[mapKey].to));
                closest = Math.Min(closest, lookup);
            }

            Console.WriteLine("Closest seed: " + closest);
        }

        private static void task2(string seedsLine, List<Map> rawMaps) {
            List<long> seeds = seedsLine.Substring(6).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();

            Dictionary<string, Map> maps = rawMaps.ToDictionary(m => m.from);

            LinkedList<Map> sortedMaps = new LinkedList<Map>();

            string nextMapKey = "seed";
            while (maps.Count > 0){
                sortedMaps.AddLast(maps[nextMapKey]);
                maps.Remove(nextMapKey);
                nextMapKey = sortedMaps.Last.Value.to;
            }

            long lowestSeed = long.MaxValue;
            for (int i=0; i < seeds.Count ; i += 2) {
                Console.WriteLine($"Seed {seeds[i]} lowest: {rangeScanRecurse(seeds[i], seeds[i+1], sortedMaps.First)}");
                lowestSeed = Math.Min(lowestSeed, rangeScanRecurse(seeds[i], seeds[i+1], sortedMaps.First));
            }
            Console.WriteLine("lowest: " + lowestSeed);

        }

        private static long rangeScanRecurse(long rangeStart, long rangeLen, LinkedListNode<Map> current) {
            long subRangeStart=rangeStart;
            long lowestValue = long.MaxValue;
            long rangeEnd = rangeStart + rangeLen;

            List<Tuple<long, long>> subRanges = new List<Tuple<long, long>>();
            //subRanges.Add(rangeStart);
            while (subRangeStart < rangeEnd) {
                long subRangeLen = current.Value.GetRangeLen(subRangeStart, rangeEnd - subRangeStart);
                subRanges.Add(new Tuple<long, long>(subRangeStart, subRangeLen));
                subRangeStart = subRangeStart + subRangeLen;
            }

            foreach(Tuple<long, long> subRange in subRanges){
                if (current.Next != null){
                    long nextRangeStart = current.Value.Get(subRange.Item1);
                    long rangeLowestValue = rangeScanRecurse(
                                        nextRangeStart,
                                        subRange.Item2,
                                        current.Next);
                    lowestValue = Math.Min(rangeLowestValue, lowestValue);
                }
                else {
                    long rangeLowestValue = current.Value.Get(subRange.Item1);
                    lowestValue = Math.Min(lowestValue, rangeLowestValue);
                }
            }

            return lowestValue;
        }



        private static void task2Brute(string seedsLine, List<Map> rawMaps) {
            List<long> seeds = seedsLine.Substring(6).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();

            Dictionary<string, Map> maps = rawMaps.ToDictionary(m => m.from);

            long closest= long.MaxValue;

            for (int i=0; i < seeds.Count ; i += 2) {
                long thisSeedClosest = long.MaxValue;
                for (int j=0; j < seeds[i+1]; j++) {
                    string mapKey = "seed";
                    long lookup = maps[mapKey].Get(seeds[i] + j);
                    do {
                        mapKey = maps[mapKey].to;
                        lookup = maps[mapKey].Get(lookup);
                    } while (maps.ContainsKey(maps[mapKey].to));
                    thisSeedClosest = Math.Min(thisSeedClosest, lookup);
                }
                Console.WriteLine($"Closest for seed {seeds[i]}: {thisSeedClosest}");
                closest = Math.Min(thisSeedClosest, closest);
            }

            Console.WriteLine("Closest seed in range: " + closest);
        }


        public class Map {
            public string from;
            public string to;

            private List<Range> ranges = new List<Range>();

            public static Map FromString (List<string> section) {
                Map m = new Map();
                string[] mapName = section[0].Split(' ').First().Split('-');
                m.from = mapName[0];
                m.to = mapName[2];
                foreach (string line in section.Skip(1)) {
                    m.ranges.Add(Range.FromString(line));
                }
                return m;
            }

            public long Get(long l){
                foreach (Range r in ranges) {
                    if (r.Contains(l)){
                        return r.Calculate(l);
                    }
                }
                return l;
            }

            public long GetRangeLen(long start, long len) {
                // If the start number is inside a range
                foreach (Range r in ranges) {
                    if (r.Contains(start)) {
                        return Math.Min(r.SourceRangeEnd, start + len) - start;
                    }
                }
                // If the start number is not inside a range, find the start point of the next range
                long nextRangeStart = long.MaxValue;
                foreach (Range r in ranges) {
                    // Find the lowest range start that is above the requested point
                    if (r.SourceRangeStart > start) {
                        nextRangeStart = Math.Min(r.SourceRangeStart, nextRangeStart);
                    }
                }
                if (nextRangeStart > start + len) {
                    return len;
                }
                else {
                    return nextRangeStart - start;
                }
            }

        }

        private class Range {
            public long DestRangeStart;
            public long SourceRangeStart;
            private long diff;
            public long RangeLen;
            public long SourceRangeEnd;

            public static Range FromString(string line) {
                return new Range(line.Split(' ').Select(x => long.Parse(x)).ToArray());
            }

            public Range(params long[] rangeData)
                : this(rangeData[0], rangeData[1], rangeData[2]) {}

            public Range(long drs, long srs, long rl){
                DestRangeStart = drs;
                SourceRangeStart = srs;
                diff = SourceRangeStart - DestRangeStart;
                RangeLen = rl;
                SourceRangeEnd = SourceRangeStart + RangeLen;
            }

            public bool Contains (long l){
                return l >= SourceRangeStart && l < SourceRangeStart + RangeLen;
            }

            public long Calculate (long l) {
                return l - diff;
            }
        }
    }
}
