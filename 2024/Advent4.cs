using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Advent.A2024
{
    public class Advent4
    {
        public static void Do(string wd){
            string file;
            using (StreamReader sr = new StreamReader(wd + "Advent4.txt")) {
                file = sr.ReadToEnd();
            }
            string[] lines = file.Split(new string[]{"\r\n", "\r", "\n" }, StringSplitOptions.None);

            task1(lines.Select(c => c.ToArray()).ToArray());
            
            task2(lines.Select(c => c.ToArray()).ToArray());
        }

        private static void task1(char[][] grid) {
            char [] search = new char[]{'X','M','A','S'};

            long sum=0;
            for (int y=0; y < grid.Length; y++) {
                for (int x=0; x < grid[0].Length; x++) {
                    bool up, down, left, right;
                    up = y - (search.Length - 1) >= 0;
                    down = y + (search.Length - 1) < grid.Length;
                    left = x - (search.Length - 1) >= 0;
                    right = x + (search.Length -1) < grid[0].Length;

                    if (up && test(grid, search, 0, -1, x, y)) { sum++; }
                    if (up && right && test(grid, search, 1, -1, x, y)) { sum++; }
                    if (right && test(grid, search, 1, 0, x, y)) { sum++; }
                    if (right && down && test(grid, search, 1, 1, x, y)) { sum++; }
                    if (down && test(grid, search, 0, 1, x, y)) { sum++; }
                    if (down && left && test(grid, search, -1, 1, x, y)) { sum++; }
                    if (left && test(grid, search, -1, 0, x, y)) { sum++; }
                    if (left && up && test(grid, search, -1, -1, x, y)) { sum++; }
                }
            }
            Console.WriteLine("sum: " +sum);
        }


        private static void task2(char[][] grid) {
            long sum=0;
            for (int y=1; y < grid.Length - 1; y++) {
                for (int x=1; x < grid[0].Length - 1; x++) {
                    if (test2(grid, x, y)) {
                        sum++;
                    }
                }
            }
            Console.WriteLine("sum2: " + sum);
        }

        private static bool test (char[][] grid, char[] search, int xOffset, int yOffset, int xOrigin, int yOrigin) {
            for(int i=0; i < search.Length; i++) {
                if (grid[yOrigin + (i * yOffset)][xOrigin + (i * xOffset)] != search[i]) {
                    return false;
                }
            }
            return true;
        }

        private static bool test2 (char[][] grid, int x, int y) {
            if (grid[y][x] != 'A'){
                return false;
            }
            if (! ((grid[y-1][x-1] == 'M' && grid[y+1][x+1] == 'S')||
                  (grid[y-1][x-1] == 'S' && grid[y+1][x+1] == 'M'))){
                return false;
            }
            if (! ((grid[y-1][x+1] == 'M' && grid[y+1][x-1] == 'S')||
                  (grid[y-1][x+1] == 'S' && grid[y+1][x-1] == 'M'))){
                return false;
            }
            return true;
        }

    }
}
