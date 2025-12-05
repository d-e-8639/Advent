using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Advent.lib;

namespace Advent.Y2025
{
    public class Advent4
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent4sample.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<List<WarehouseContent>> warehouseContents = lines.Select(l => l.Select(c => c == '@' ? WarehouseContent.Paper : WarehouseContent.Empty).ToList()).ToList();

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            task1(warehouseContents);
            st1.Stop();


            Stopwatch st2 = new Stopwatch();
            st2.Start();
            task2(warehouseContents);
            st2.Stop();

            Console.WriteLine($"Time 1: {st1.ElapsedMilliseconds}, Time 2: {st2.ElapsedMilliseconds}" );

        }

        public enum WarehouseContent
        {
            Paper,
            Empty
        }

        private static void task1(List<List<WarehouseContent>> warehouseContents)
        {
            Grid<WarehouseContent> grid = new Grid<WarehouseContent>(warehouseContents);
            
            int count=0;
            foreach (GridItem<WarehouseContent> i in grid)
            {
                if (i.Item == WarehouseContent.Paper && i.NeighborsCardinalAndOrdinal().Where(n => n.Item == WarehouseContent.Paper).Count() < 4)
                {
                    count++;
                }
            }
            Console.WriteLine("Accessible count: " + count);
        }

        private static void task2(List<List<WarehouseContent>> warehouseContents) {
        }

    }
}
